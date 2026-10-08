"""Compile reproducible sweeps, analyze completed batches, export bounded legacy cohorts.

Standard library only. Run `python sweep.py --help` for commands.
"""
import argparse
import csv
import hashlib
import json
import math
import random
import re
import sqlite3
import subprocess
from pathlib import Path

HERE = Path(__file__).resolve().parent
RUNNER = HERE / "bin/Release/net8.0/CellSim.Batch.dll"
STATUS = {"Valid": 0, "NotApplicable": 1, "Invalid": 2, "Missing": 3}


def load(path):
    return json.loads(Path(path).read_text(encoding="utf-8-sig"), parse_constant=lambda x: fail("Nonfinite JSON: " + x))


def fail(message):
    raise ValueError(message)


def strict(value, allowed, label):
    if not isinstance(value, dict) or value.keys() - set(allowed):
        fail("Unknown fields in " + label)


def write(path, value):
    with Path(path).open("x", encoding="utf-8") as stream:
        json.dump(value, stream, indent=2, allow_nan=False)


def digest(path):
    with Path(path).open("rb") as stream:
        return hashlib.file_digest(stream, "sha256").hexdigest()


def invoke(*args):
    result = subprocess.run(["dotnet", str(RUNNER), *map(str, args)], capture_output=True, text=True)
    if result.returncode:
        fail(result.stdout + result.stderr)
    return result.stdout


def tokens(arguments):
    if not isinstance(arguments, list) or len(arguments) % 2 or any(not isinstance(a, str) for a in arguments):
        fail("Arguments must be option/value pairs")
    result = dict(zip(arguments[::2], arguments[1::2]))
    if len(result) * 2 != len(arguments):
        fail("Repeated option")
    return result


def merge(layers):
    values, components = {}, {}
    for layer in layers:
        for key, value in tokens(layer).items():
            if key in ("-speciesStats", "-startingPopulations"):
                target = components.setdefault(key, {})
                for item in value.split(","):
                    pair = item.split("=")
                    if len(pair) != 2 or pair[0] in target:
                        fail("Conflicting compound axis: " + item)
                    target[pair[0]] = pair[1]
            else:
                if key in values:
                    fail("Conflicting axes/options: " + key)
                values[key] = value
    values.update({key: ",".join(k + "=" + v for k, v in sorted(items.items())) for key, items in components.items()})
    return [token for key in sorted(values) for token in (key, values[key])]


def phase_count(options):
    if "-mutationPolicy" in options:
        return 6
    for name in ("-phaseUpgradeSchedule", "-phaseUpgradeAssetSchedule"):
        if name in options:
            return len(options[name].split(";"))
    return 0


def planned_ticks(options):
    phases = phase_count(options)
    if phases:
        return phases * int(options.get("-phaseLengthTicks", "0"))
    if "-runTicks" not in options:
        fail("Sweeps require explicit -runTicks or a phase flow to define the observation horizon")
    return int(options["-runTicks"])


def compile_sweep(spec_path, destination, contexts=None, seed_start=None, seed_count=None):
    spec_path, destination = Path(spec_path).resolve(), Path(destination).resolve()
    spec = load(spec_path)
    strict(spec, ["schemaVersion", "snapshot", "seedStart", "seedCount", "workers", "chunkSize", "timeoutSeconds",
        "baseArguments", "axes", "paths", "referencePath", "horizonTicks", "metrics", "maxRuns", "sampleContexts", "samplingSeed"], "sweep")
    if spec.get("schemaVersion") != 1 or destination.exists():
        fail("Unsupported schema or existing destination")
    axes, paths = spec["axes"], spec["paths"]
    ids = set()
    for item in axes + paths:
        name = item.get("id", "")
        if not re.fullmatch(r"[A-Za-z0-9_-]{1,40}", name) or name in ids:
            fail("Axis/path IDs must be unique safe names: " + name)
        ids.add(name)
    for axis in axes:
        strict(axis, ["id", "values"], "axis")
        if not axis["values"] or len({v["id"] for v in axis["values"]}) != len(axis["values"]):
            fail("Empty axis or duplicate value IDs")
        for value in axis["values"]:
            strict(value, ["id", "arguments"], "axis value")
            tokens(value["arguments"])
    for path in paths:
        strict(path, ["id", "arguments"], "path")
    reference = spec["referencePath"]
    if not paths or reference not in [p["id"] for p in paths]:
        fail("Reference path must be included")
    metrics = spec["metrics"]
    if not isinstance(metrics, list) or not metrics or any(not isinstance(m, str) for m in metrics) or len(set(metrics)) != len(metrics) or len(metrics) > 100 or type(spec["horizonTicks"]) is not int or spec["horizonTicks"] <= 0:
        fail("Declare unique metrics and a positive observation horizon")
    total = math.prod(len(a["values"]) for a in axes)
    seeds = spec["seedCount"] if seed_count is None else seed_count
    start = spec["seedStart"] if seed_start is None else seed_start
    budget = spec.get("maxRuns", 1000000)
    if type(seeds) is not int or seeds < 1 or type(start) is not int or type(budget) is not int or budget < 1:
        fail("Seed values and run budget must be integers; counts must be positive")
    chosen = None
    if contexts is not None:
        if max(start, spec["seedStart"]) < min(start + seeds, spec["seedStart"] + spec["seedCount"]):
            fail("Shortlist confirmation requires seeds outside the screening range")
        selected = load(contexts)
        if not isinstance(selected, list) or not selected:
            fail("Shortlist must contain axis contexts")
        chosen = {json.dumps(c, sort_keys=True) for c in selected}
        count = len(chosen)
        def selected_combination(labels):
            if not isinstance(labels, dict) or set(labels) != {a["id"] for a in axes}:
                fail("Unknown shortlisted axes")
            result = []
            for axis in axes:
                value = next((v for v in axis["values"] if v["id"] == labels[axis["id"]]), None)
                if value is None:
                    fail("Unknown shortlisted value")
                result.append(value)
            return tuple(result)
        combinations = (selected_combination(json.loads(key)) for key in sorted(chosen))
    else:
        count = spec.get("sampleContexts", total)
        if type(count) is not int or count < 1 or count > total:
            fail("Invalid sampleContexts")
        if count * len(paths) > 10000 or count * len(paths) * seeds > budget:
            fail("Run/condition budget exceeded")
        # Sample mixed-radix indices without materializing the Cartesian space.
        indices = sorted(random.Random(spec.get("samplingSeed", 1)).sample(range(total), count)) if count < total else range(total)
        def decode(index):
            result = []
            for axis in reversed(axes):
                index, digit = divmod(index, len(axis["values"]))
                result.append(axis["values"][digit])
            return tuple(reversed(result))
        combinations = (decode(index) for index in indices)
    if count * len(paths) > 10000 or count * len(paths) * seeds > budget:
        fail("Run/condition budget exceeded")
    cases, conditions, context_ids = [], [], set()
    snapshot_path = (spec_path.parent / spec["snapshot"]).resolve()
    snapshot = load(snapshot_path)
    baseline_options = tokens(snapshot["arguments"])
    for combination in combinations:
        labels = {a["id"]: v["id"] for a, v in zip(axes, combination)}
        key = json.dumps(labels, sort_keys=True)
        if chosen is not None and key not in chosen:
            continue
        context = hashlib.sha256(key.encode()).hexdigest()[:16]
        context_ids.add(key)
        for path in paths:
            arguments = merge([spec.get("baseArguments", []), *(v["arguments"] for v in combination), path["arguments"]])
            # Compound overrides also retain explicitly frozen baseline entries.
            options = tokens(arguments)
            for name in ("-speciesStats", "-startingPopulations"):
                if name in options and name in baseline_options:
                    frozen = dict(item.split("=") for item in baseline_options[name].split(","))
                    frozen.update(dict(item.split("=") for item in options[name].split(",")))
                    options[name] = ",".join(k + "=" + v for k, v in sorted(frozen.items()))
            arguments = [t for k in sorted(options) for t in (k, options[k])]
            resolved = dict(baseline_options, **options)
            if planned_ticks(resolved) != spec["horizonTicks"]:
                fail("horizonTicks must match the planned run ticks for every condition")
            case_id = "c" + context + "-" + path["id"]
            conditions.append(dict(id=case_id, arguments=arguments))
            cases.append(dict(id=case_id, context=context, axes=labels, path=path["id"],
                reference="c" + context + "-" + reference, arguments=arguments))
    runs = len(cases) * seeds
    if not cases or len(cases) > 10000 or seeds < 1 or runs > spec.get("maxRuns", 1000000):
        fail("Empty selection or run/condition budget exceeded")
    if chosen is not None and context_ids != chosen:
        fail("Unknown shortlisted context")
    destination.mkdir(parents=True)
    plan = dict(schemaVersion=1, snapshot=str(snapshot_path), output="batch", seedStart=start, seedCount=seeds,
        chunkSize=spec.get("chunkSize", 100), workers=spec.get("workers", 8), timeoutSeconds=spec.get("timeoutSeconds", 3600), conditions=conditions)
    plan_path = destination / "plan.json"
    write(plan_path, plan)
    try:
        preflight = invoke("batch", plan_path, "--dry-run")
    except ValueError:
        # Retain the invalid plan for a concrete, reviewable diagnostic.
        raise
    write(destination / "sweep.json", dict(schemaVersion=1, specPath=str(spec_path), specHash=digest(spec_path), planHash=digest(plan_path),
        snapshotHash=digest(snapshot_path), seedStart=start, seedCount=seeds, horizonTicks=spec["horizonTicks"], metrics=metrics,
        cartesianContexts=total, selectedContexts=len(context_ids), conditions=len(cases), runs=runs,
        globalReference=cases[0]["reference"], cases=cases,
        axisReferences={a["id"]: a["values"][0]["id"] for a in axes},
        note="Screening configuration; candidate identification requires fresh-seed confirmation and gameplay review."))
    print(preflight.strip())
    print("Compiled", len(context_ids), "contexts x", len(paths), "paths x", seeds, "matched seeds")
    return destination


def values(window, horizon, phase=False):
    result = {}
    def put(key, value, status="Valid"):
        if isinstance(value, (int, float)) and math.isfinite(value):
            result[key] = (float(value) if status == "Valid" else None, STATUS.get(status, 2))
    start = window.get("windowStartTickExclusive", 0)
    end = window["windowEndTickInclusive"] if phase else window["ticks"]
    ticks = end - start
    put("ticks", ticks)
    closing = window["closingPopulation"] if phase else window["populationHistory"]
    final = {s["speciesId"]: s["population"] for s in closing[-1]["species"]}
    for species, population in final.items():
        put("population." + species + ".final", population)
    if not phase:
        if window.get("purchaseWindows") is not None:
            purchases = window["purchaseWindows"]
            for key in ("requested", "added", "earned", "spent"):
                put("purchase." + key, sum(p[key] for p in purchases))
            put("purchase.balance", purchases[-1]["balanceAfter"] if purchases else 0)
            put("purchase.windowsReached", len(purchases))
            put("purchase.windowsNotReached", 5 - len(purchases))
            for reason in ("insufficient-currency", "placement-or-capacity", "target-met"):
                put("purchase.stopped." + reason, sum(p["stopReason"] == reason for p in purchases))
        put("outcome.reachedHorizon", end >= horizon)
        put("outcome.playerAliveAtHorizon", end >= horizon and window["playerPopulation"] > 0)
        put("outcome.allSpeciesAliveAtHorizon", end >= horizon and all(p > 0 for p in final.values()))
        for row in window["populationTrajectory"]:
            for key in ("first", "last", "min", "max", "positiveTicks", "firstZeroTick", "populationTickIntegral"):
                put("population." + row["speciesId"] + "." + key, row[key])
            put("population." + row["speciesId"] + ".mean", row["populationTickIntegral"] / row["observedTicks"] if row["observedTicks"] else 0)
    for row in window["activity"]:
        for key, value in row.items():
            if key == "speciesId":
                continue
            name = "activity." + row["speciesId"] + "." + key
            put(name, value)
            if isinstance(value, (int, float)) and not isinstance(value, bool) and ticks:
                put(name + ".per100Ticks", 100 * value / ticks)
    for kind in ("herbivore", "predator"):
        for row in window[kind + "StatLines"]:
            if not row["fpoReconciled"]:
                fail("Unreconciled species stat line")
            for key, value in row.items():
                if key != "speciesId":
                    put(kind + "." + row["speciesId"] + "." + key, value, row.get(key + "Status", "Valid"))
    return result, ticks, start, end


def distribution(cursor):
    n, mean, m2, lo, hi = 0, 0., 0., None, None
    for (value,) in cursor:
        n += 1
        delta = value - mean
        mean += delta / n
        m2 += delta * (value - mean)
        lo = value if lo is None else min(lo, value)
        hi = value if hi is None else max(hi, value)
    sd = math.sqrt(max(0, m2 / (n - 1))) if n > 1 else None
    ci = 1.96 * sd / math.sqrt(n) if sd is not None else None
    return dict(n=n, mean=mean if n else None, sd=sd, min=lo, max=hi,
        meanCI95Low=mean-ci if ci is not None else None, meanCI95High=mean+ci if ci is not None else None)


def mcnemar(wins, losses):
    n, k = wins + losses, min(wins, losses)
    if not n:
        return 1.
    logp = math.lgamma(n + 1) - math.lgamma(k + 1) - math.lgamma(n - k + 1) - n * math.log(2)
    total = term = 1.
    for j in range(k, 0, -1):
        term *= j / (n - j + 1)
        total += term
        if term < total * 1e-15:
            break
    return min(1., 2 * math.exp(logp) * total)


def quantile(db, condition, phase, column, n, q):
    if not n:
        return None
    index = (n - 1) * q
    low = int(index)
    rows = [r[0] for r in db.execute(f"SELECT {column} FROM observations WHERE condition=? AND phase=? AND {column} IS NOT NULL ORDER BY {column} LIMIT 2 OFFSET ?", (condition, phase, low))]
    return rows[0] + (index - low) * (rows[-1] - rows[0])


def completed(root):
    root = Path(root).resolve()
    sweep, manifest, summary = load(root / "sweep.json"), load(root / "batch/batch.json"), load(root / "batch/summary.json")
    if digest(root / "plan.json") != sweep["planHash"] or digest(manifest["plan"]["snapshot"]) != sweep["snapshotHash"]:
        fail("Sweep input identity changed")
    plan = manifest["plan"]
    expected = {c["id"]: c["arguments"] for c in sweep["cases"]}
    frozen_plan = load(root / "plan.json")
    if expected != {c["id"]: c["arguments"] for c in frozen_plan["conditions"]}:
        fail("Sweep conditions differ from frozen plan")
    if set(expected) != {c["id"] for c in plan["conditions"]} or plan["seedStart"] != sweep["seedStart"] or plan["seedCount"] != sweep["seedCount"]:
        fail("Batch differs from sweep")
    baseline = tokens(load(plan["snapshot"])["arguments"])
    for condition in plan["conditions"]:
        resolved = dict(baseline)
        resolved.update(tokens(expected[condition["id"]]))
        if tokens(condition["arguments"]) != resolved:
            fail("Condition arguments differ")
        if planned_ticks(resolved) != sweep["horizonTicks"]:
            fail("Condition observation horizon differs")
    output = root / "batch/runs.jsonl"
    if summary["state"] != "Completed" or summary["identity"] != manifest["identity"] or summary["runs"] != sweep["runs"] or digest(output) != summary["outputHash"]:
        fail("Incomplete or corrupt batch")
    return root, sweep, manifest, output


def analyze(root, destination, rank, direction="higher", top=20):
    root, sweep, manifest, output = completed(root)
    destination = Path(destination).resolve()
    if destination.exists() or rank not in sweep["metrics"] or top < 1:
        fail("Use new output, a declared ranking metric and positive shortlist size")
    destination.mkdir(parents=True)
    metrics = sweep["metrics"]
    db = sqlite3.connect(destination / "metrics.sqlite")
    db.execute("PRAGMA temp_store=FILE")
    # ponytail: wide selected metrics bound row count; declare more metrics or
    # reanalyze raw JSONL when the next investigation needs different measures.
    db.execute("CREATE TABLE observations(condition TEXT, seed INTEGER, phase INTEGER, ticks INTEGER, start INTEGER, end INTEGER," +
        ",".join(f"m{i} REAL,s{i} INTEGER" for i in range(len(metrics))) + ",PRIMARY KEY(condition,seed,phase)) WITHOUT ROWID")
    db.execute("CREATE TABLE cases(condition TEXT PRIMARY KEY, metadata TEXT)")
    db.executemany("INSERT INTO cases VALUES (?,?)", ((c["id"], json.dumps(c)) for c in sweep["cases"]))
    db.execute("CREATE TABLE metrics(column_name TEXT PRIMARY KEY, metric TEXT, status_column TEXT)")
    db.executemany("INSERT INTO metrics VALUES (?,?,?)", (("m"+str(i), m, "s"+str(i)) for i, m in enumerate(metrics)))
    db.execute("CREATE TABLE provenance(metadata TEXT)")
    db.execute("INSERT INTO provenance VALUES (?)", (json.dumps(dict(batchIdentity=manifest["identity"], rawHash=digest(output),
        sweepHash=digest(root/"sweep.json"), statuses=STATUS)),))
    case_ids = {c["id"] for c in sweep["cases"]}
    counts = dict.fromkeys(case_ids, 0)
    known, indexed = set(), 0
    with output.open(encoding="utf-8-sig") as stream, db:
        for line in stream:
            row = json.loads(line, parse_constant=lambda x: fail("Nonfinite JSON: " + x))
            condition, run = row["condition"], row["run"]
            if condition not in counts or run["seed"] != sweep["seedStart"] + counts[condition]:
                fail("Missing, extra, unordered or duplicate seeds")
            counts[condition] += 1
            indexed += 1
            if run["ticks"] > sweep["horizonTicks"]:
                fail("Run exceeds the declared horizon")
            if counts[condition] > sweep["seedCount"]:
                fail("Extra seeds")
            for phase, window in [(0, run)] + [(p["phaseIndex"], p) for p in run["phaseResults"]]:
                measured, ticks, start, end = values(window, sweep["horizonTicks"], phase != 0)
                known.update(measured)
                numeric = [item for metric in metrics for item in measured.get(metric, (None, 3))]
                data = [condition, run["seed"], phase, ticks, start, end] + numeric
                db.execute("INSERT INTO observations VALUES (" + ",".join("?" * len(data)) + ")", data)
            if indexed % 10000 == 0:
                print("Indexed", indexed, "runs", flush=True)
    if any(n != sweep["seedCount"] for n in counts.values()) or set(metrics) - known:
        fail("Missing seeds or unknown requested metrics: " + str(set(metrics) - known))
    db.execute("CREATE INDEX window_index ON observations(condition,phase)")
    db.commit()
    # ponytail: summary memory scales with conditions x phases x metrics;
    # stream these CSV rows if a broad matrix makes this ceiling material.
    summaries, pairs, scores = [], [], {}
    by_axes_path = {(json.dumps(c["axes"], sort_keys=True), c["path"]): c["id"] for c in sweep["cases"]}
    condition_arguments = {c["id"]: c["arguments"] for c in manifest["plan"]["conditions"]}
    for case in sweep["cases"]:
        condition = case["id"]
        references = [("same-context-path", case["reference"]), ("global-context", sweep["globalReference"])]
        for axis, label in sweep["axisReferences"].items():
            held = dict(case["axes"], **{axis: label})
            reference = by_axes_path.get((json.dumps(held, sort_keys=True), case["path"]))
            if reference is not None:
                references.append(("axis:"+axis, reference))
        arguments = condition_arguments[condition]
        phases = range(phase_count(tokens(arguments)) + 1)
        for phase in phases:
            for i, metric in enumerate(metrics):
                stats = distribution(db.execute(f"SELECT m{i} FROM observations WHERE condition=? AND phase=? AND m{i} IS NOT NULL", (condition, phase)))
                status_counts = dict(db.execute(f"SELECT s{i},count(*) FROM observations WHERE condition=? AND phase=? GROUP BY s{i}", (condition, phase)))
                stats.update(dict(condition=condition, context=case["context"], path=case["path"], phase=phase, metric=metric,
                    expectedSeeds=sweep["seedCount"], invalid=status_counts.get(2, 0), notApplicable=status_counts.get(1, 0),
                    missing=status_counts.get(3, 0), notReached=sweep["seedCount"]-sum(status_counts.values())))
                stats.update({name: quantile(db, condition, phase, "m" + str(i), stats["n"], q) for name, q in [("p10", .1), ("median", .5), ("p90", .9)]})
                summaries.append(stats)
                if phase == 0 and metric == rank:
                    scores[condition] = stats
                for scope, reference in references:
                    if reference == condition:
                        continue
                    query = f"FROM observations a JOIN observations b ON a.seed=b.seed AND a.phase=b.phase WHERE a.condition=? AND b.condition=? AND a.phase=?"
                    parameters = (condition, reference, phase)
                    delta = distribution(db.execute(f"SELECT a.m{i}-b.m{i} " + query + f" AND a.m{i} IS NOT NULL AND b.m{i} IS NOT NULL", parameters))
                    wins, losses, duration_mismatches = db.execute(f"SELECT sum(a.m{i}>b.m{i}),sum(a.m{i}<b.m{i}),sum(a.start!=b.start OR a.end!=b.end) " + query + f" AND a.m{i} IS NOT NULL AND b.m{i} IS NOT NULL", parameters).fetchone()
                    delta.update(dict(condition=condition, reference=reference, scope=scope, phase=phase, metric=metric,
                        wins=wins or 0, losses=losses or 0, differentWindows=duration_mismatches or 0,
                        unpairedOrInvalid=sweep["seedCount"]-delta["n"], exactMcNemarP=mcnemar(wins or 0, losses or 0) if metric.startswith("outcome.") else None))
                    pairs.append(delta)
    def export_csv(name, rows):
        with (destination / name).open("x", encoding="utf-8", newline="") as stream:
            writer = csv.DictWriter(stream, fieldnames=list(rows[0]) if rows else ["condition"])
            writer.writeheader()
            writer.writerows(rows)
    export_csv("metrics.csv", summaries)
    export_csv("paired-deltas.csv", pairs)
    ranked = sorted((c for c in sweep["cases"] if scores[c["id"]]["mean"] is not None),
        key=lambda c: ((-1 if direction == "higher" else 1) * scores[c["id"]]["mean"], c["id"]))
    shortlist = []
    for case in ranked:
        if case["axes"] not in shortlist:
            shortlist.append(case["axes"])
        if len(shortlist) == top:
            break
    write(destination / "shortlist-contexts.json", shortlist)
    write(destination / "analysis.json", dict(schemaVersion=1, state="Completed", runs=sum(counts.values()), batchIdentity=manifest["identity"],
        rawHash=digest(output), sweepHash=digest(root / "sweep.json"), metrics=metrics, rank=rank, direction=direction,
        topCases=[dict(case=c, statistics=scores[c["id"]]) for c in ranked[:top]],
        inference="Exploratory screening. Mean CIs are normal approximations; phase metrics include only observed windows. Compare different-window counts and rates. P values are unadjusted for multiple comparisons. Confirm candidates on fresh seeds and review gameplay.",
        outputHashes={name: digest(destination/name) for name in ["metrics.csv", "paired-deltas.csv", "shortlist-contexts.json"]}))
    db.close()
    with (destination / "analysis.md").open("x", encoding="utf-8") as stream:
        stream.write(f"# Sweep screen\n\n{sum(counts.values()):,} runs across {len(sweep['cases'])} conditions. Ranking: `{rank}` ({direction}).\n\n")
        stream.write("| Condition | Axes | Mean | Median | p10 / p90 | Valid seeds |\n|---|---|---:|---:|---|---:|\n")
        for case in ranked[:top]:
            stats = scores[case["id"]]
            stream.write(f"| {case['id']} | {json.dumps(case['axes'])} | {stats['mean']:.5g} | {stats['median']:.5g} | {stats['p10']:.5g} / {stats['p90']:.5g} | {stats['n']} |\n")
        stream.write("\nThis is a candidate screen, not a gameplay or balance approval. See metrics.csv for status-aware distributions and paired-deltas.csv for same-context, held-axis and global deltas. Different windows and unavailable rates are explicit. Use fresh seeds for confirmation; statistical CIs/p values are exploratory and unadjusted.\n")
    print("Analyzed", sum(counts.values()), "runs;", len(summaries), "metric windows;", len(pairs), "paired comparisons")


def export_report(root, condition, destination, limit):
    root, sweep, manifest, output = completed(root)
    if digest(RUNNER) != manifest["assemblyHash"]:
        fail("Report header build differs from frozen batch; restore the original build")
    if limit < 1 or limit > 1000 or Path(destination).exists():
        fail("Use a new report path and a cohort limit of 1..1000")
    arguments = next(c["arguments"] for c in manifest["plan"]["conditions"] if c["id"] == condition)
    runs = []
    with output.open(encoding="utf-8-sig") as stream:
        for line in stream:
            row = json.loads(line)
            if row["condition"] == condition:
                runs.append(row["run"])
                if len(runs) == limit:
                    break
    if not runs:
        fail("Empty cohort")
    target = Path(destination).resolve()
    args_file = target.with_suffix(".arguments.json")
    header_file = target.with_suffix(".header.json")
    write(args_file, arguments)
    invoke("header", manifest["plan"]["snapshot"], args_file, runs[0]["seed"], len(runs), header_file)
    report = load(header_file)
    # Header-only aggregate placeholders are not measured aggregates.
    for key in list(report):
        if key.endswith("Summary"):
            del report[key]
    report["runs"] = runs
    snapshot = load(manifest["plan"]["snapshot"])
    report["portableCohort"] = dict(batchIdentity=manifest["identity"], condition=condition, totalConditionSeeds=sweep["seedCount"],
        exportedSeeds=len(runs), selection="First ordered seeds; bounded compatibility cohort, not the complete large batch", rawHash=digest(output),
        sourceHash=manifest["sourceHash"], assemblyHash=manifest["assemblyHash"], snapshotHash=sweep["snapshotHash"],
        sourceCommit=snapshot["sourceCommit"], arguments=arguments, detail="Event traces were omitted by compact workers; replay a seed for event detail.")
    write(target, report)
    print("Exported", len(runs), "existing runs; no simulations rerun:", target)


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    commands = parser.add_subparsers(dest="command", required=True)
    compile_parser = commands.add_parser("compile")
    compile_parser.add_argument("spec")
    compile_parser.add_argument("output")
    compile_parser.add_argument("--contexts")
    compile_parser.add_argument("--seed-start", type=int)
    compile_parser.add_argument("--seed-count", type=int)
    analyzer = commands.add_parser("analyze")
    analyzer.add_argument("sweep")
    analyzer.add_argument("output")
    analyzer.add_argument("--rank", required=True)
    analyzer.add_argument("--direction", choices=["higher", "lower"], default="higher")
    analyzer.add_argument("--top", type=int, default=20)
    exporter = commands.add_parser("export-report")
    exporter.add_argument("sweep")
    exporter.add_argument("condition")
    exporter.add_argument("output")
    exporter.add_argument("--max-runs", type=int, default=100)
    args = parser.parse_args()
    if args.command == "compile":
        compile_sweep(args.spec, args.output, args.contexts, args.seed_start, args.seed_count)
    elif args.command == "analyze":
        analyze(args.sweep, args.output, args.rank, args.direction, args.top)
    else:
        export_report(args.sweep, args.condition, args.output, args.max_runs)


if __name__ == "__main__":
    main()
