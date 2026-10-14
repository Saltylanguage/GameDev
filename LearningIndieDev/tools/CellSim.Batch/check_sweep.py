"""One bounded sweep/analysis regression. python check_sweep.py <sweep> <analysis> <NEW-output-dir>."""
import csv
import importlib.util
import json
import math
import shutil
import sqlite3
import statistics
import sys
from pathlib import Path

import sweep


def main():
    root, analysis, target = (Path(p).resolve() for p in sys.argv[1:])
    assert not target.exists(), "Use a new check directory"
    target.mkdir(parents=True)
    config = sweep.load(root / "sweep.json")
    spec = sweep.load(config["specPath"])
    spec["snapshot"] = sweep.load(root / "plan.json")["snapshot"]

    def refuses(call):
        try:
            call()
        except ValueError:
            return
        raise AssertionError("Invalid input was accepted")

    assert sweep.merge([["-speciesStats", "fox:energy.starting=80"],
        ["-speciesStats", "hare:awareness.vision-range=7"]]) == [
        "-speciesStats", "fox:energy.starting=80,hare:awareness.vision-range=7"]
    refuses(lambda: sweep.merge([["-speciesStats", "fox:energy.starting=80"],
        ["-speciesStats", "fox:energy.starting=120"]]))
    refuses(lambda: sweep.merge([["-gridWidth", "20"], ["-gridWidth", "24"]]))
    assert sweep.planned_ticks({"-mutationPolicy": "warren", "-phaseLengthTicks": "100"}) == 600
    refuses(lambda: sweep.planned_ticks({"-runDurationSeconds": "20"}))
    stats = sweep.distribution(iter([(1.,), (2.,), (3.,), (4.,)]))
    assert stats["mean"] == 2.5 and math.isclose(stats["sd"], statistics.stdev([1, 2, 3, 4]))
    assert math.isclose(sweep.mcnemar(3, 1), .625)
    assert math.isclose(sweep.mcnemar(10000, 10000), 1., abs_tol=1e-8)
    assert sweep.mcnemar(0, 0) == 1.

    # Sparse/random screening is reproducible and keeps every path per context.
    spec["sampleContexts"] = 3
    spec["samplingSeed"] = 112
    sample = target / "sample-spec.json"
    sweep.write(sample, spec)
    sweep.compile_sweep(sample, target / "sample-a")
    sweep.compile_sweep(sample, target / "sample-b")
    a, b = (sweep.load(target / name / "sweep.json") for name in ["sample-a", "sample-b"])
    assert a["cases"] == b["cases"] and a["selectedContexts"] == 3
    assert a["conditions"] == 3 * len(spec["paths"])
    too_large = dict(spec, seedCount=1000000)
    sweep.write(target / "over-budget.json", too_large)
    refuses(lambda: sweep.compile_sweep(target / "over-budget.json", target / "over-budget"))
    assert not (target / "over-budget").exists(), "Budget rejection allocated output"

    # Reconcile SQL distributions and paired deltas against the original rows.
    rows = [json.loads(line) for line in (root / "batch/runs.jsonl").read_text().splitlines()]
    case = next(c for c in config["cases"] if c["id"] != c["reference"])
    candidate = {r["run"]["seed"]: r["run"] for r in rows if r["condition"] == case["id"]}
    reference = {r["run"]["seed"]: r["run"] for r in rows if r["condition"] == case["reference"]}
    deltas = [candidate[s]["playerPopulation"] - reference[s]["playerPopulation"] for s in candidate]
    paired = list(csv.DictReader((analysis / "paired-deltas.csv").open()))
    result = next(r for r in paired if r["condition"] == case["id"] and r["scope"] == "same-context-path"
        and r["phase"] == "0" and r["metric"] == "population.hare.final")
    assert math.isclose(float(result["mean"]), statistics.mean(deltas)) and int(result["n"]) == config["seedCount"]
    assert int(result["wins"]) == sum(d > 0 for d in deltas)
    assert int(result["losses"]) == sum(d < 0 for d in deltas)
    cases = {c["id"]: c for c in config["cases"]}
    for row in paired:
        if row["scope"].startswith("axis:"):
            axis = row["scope"][5:]
            arm, control = cases[row["condition"]], cases[row["reference"]]
            assert arm["path"] == control["path"]
            assert all(arm["axes"][key] == control["axes"][key] for key in arm["axes"] if key != axis)
    with sqlite3.connect(analysis / "metrics.sqlite") as db:
        assert db.execute("SELECT count(*) FROM observations WHERE phase=0").fetchone()[0] == config["runs"]
        assert db.execute("SELECT count(*) FROM metrics").fetchone()[0] == len(config["metrics"])
    summaries = list(csv.DictReader((analysis / "metrics.csv").open()))
    manifest = sweep.load(root / "batch/batch.json")
    expected_windows = sum(1 + sweep.phase_count(sweep.tokens(c["arguments"])) for c in manifest["plan"]["conditions"])
    assert len(summaries) == expected_windows * len(config["metrics"]), "Entirely unreached phases disappeared"
    first = rows[0]["run"]
    for row in first["populationTrajectory"]:
        assert row["observedTicks"] == first["ticks"] and row["min"] <= row["last"] <= row["max"]
    unavailable = json.loads(json.dumps(first))
    stat = unavailable["herbivoreStatLines"][0]
    stat["pAVI"], stat["pAVIStatus"] = 0, "NotApplicable"
    stat["sAVI"], stat["sAVIStatus"] = 0, "Invalid"
    measured, *_ = sweep.values(unavailable, config["horizonTicks"])
    assert measured["herbivore.hare.pAVI"] == (None, 1) and measured["herbivore.hare.sAVI"] == (None, 2)
    # Existing cohort tools must use full trajectories, not compact endpoints.
    script = Path(__file__).resolve().parents[3] / ".agents/skills/artifact-summarization/scripts/summarize_cell_sim.py"
    module_spec = importlib.util.spec_from_file_location("cohort_summary", script)
    summarizer = importlib.util.module_from_spec(module_spec)
    module_spec.loader.exec_module(summarizer)
    sweep.export_report(root, case["id"], target / "cohort.json", 4)
    cohort = sweep.load(target / "cohort.json")
    compact = summarizer.core_summary(cohort, target / "cohort.json")
    assert compact["source"]["portableCohort"]["batchIdentity"] == sweep.load(root / "batch/batch.json")["identity"]
    run = compact["runs"][0]
    for row in cohort["runs"][0]["populationTrajectory"]:
        population = run["populationStats"][row["speciesId"]]
        assert population["max"] == row["max"] and population["populationSampleIntegral"] == row["first"] + row["populationTickIntegral"]
    assert run["predatorStatLines"] and "combatRolls" in run["unavailableDetailFields"]
    legacy = [{"species": [{"speciesId": "hare", "population": n}]} for n in [1, 2, 3, 4]]
    assert summarizer.population_stats(legacy)["hare"]["mean"] == 2.5

    # Checks hashes AND seed coverage, even if someone rewrites the summary hash.
    corrupt = target / "corrupt"
    (corrupt / "batch").mkdir(parents=True)
    for name in ["sweep.json", "plan.json", "batch/batch.json", "batch/summary.json", "batch/runs.jsonl"]:
        shutil.copyfile(root / name, corrupt / name)
    raw = corrupt / "batch/runs.jsonl"
    original = raw.read_text().splitlines()
    raw.write_text(original[0] + "\n" + "\n".join(original) + "\n")
    refuses(lambda: sweep.completed(corrupt))
    summary = sweep.load(corrupt / "batch/summary.json")
    summary["outputHash"] = sweep.digest(raw)
    (corrupt / "batch/summary.json").write_text(json.dumps(summary))
    refuses(lambda: sweep.analyze(corrupt, target / "invalid-analysis", "population.hare.final"))
    sweep.write(target / "checks.json", dict(state="Passed", checks=["compound stat axes and conflict rejection",
        "sampling reproducibility", "budget before expansion", "hand-calculated distributions and McNemar",
        "matched seed deltas", "one-axis held-context comparisons", "SQL coverage and metric mapping",
        "population trajectory", "N/A and Invalid stay unavailable", "portable cohort summary and legacy behavior", "hash and duplicate seed rejection"]))
    print("PASS: sweep generation, analysis, paired deltas and integrity regression")


if __name__ == "__main__":
    main()
