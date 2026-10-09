"""Prepare a readable worksheet from existing CellSim evidence; no simulation or AI calls."""
import argparse
import collections
import datetime as dt
import hashlib
import json
import math
import sys
import os
import posixpath
import tempfile
import xml.etree.ElementTree as ET
import zipfile
from pathlib import Path

from sweep import completed, load, planned_ticks, tokens


SPECIES = ("hare", "fox", "plant")
REASONS = {
    "insufficient-currency": "Not enough Field Data",
    "placement-or-capacity": "No placement space or population capacity",
    "target-met": "Population target already reached",
    "policy-cap": "Policy purchase limit reached",
}


def readable(value):
    return str(value).replace("-", " ").replace("_", " ").strip().capitalize()


def population(snapshot):
    entries = {s["speciesId"]: s["population"] for s in snapshot.get("species", [])}
    for value in entries.values():
        if type(value) is not int or value < 0:
            raise ValueError("Population counts must be nonnegative integers")
    return tuple(entries.get(name) for name in SPECIES)


def counts(values):
    return " / ".join("Not recorded" if v is None else str(v) for v in values)


def snapshots(run):
    history = {s["tick"]: population(s) for s in run.get("populationHistory") or []}
    # Phase closes precede boundary purchases; the next phase opens after them.
    for phase in run.get("phaseResults") or []:
        for snapshot in phase.get("closingPopulation") or []:
            history[snapshot["tick"]] = population(snapshot)
    return history


def endpoint(run):
    if type(run.get("seed")) is not int or type(run.get("ticks")) is not int or run["ticks"] < 0:
        raise ValueError("Each run needs an integer seed and nonnegative endpoint tick")
    return snapshots(run).get(run["ticks"], (None, None, None))


def checkpoint(run, tick, history):
    if run["ticks"] < tick:
        return "Not reached"
    return counts(checkpoint_values(run, tick, history))


def checkpoint_values(run, tick, history):
    if run['ticks'] < tick:
        return (None,) * len(SPECIES)
    values = list(history.get(tick, (None, None, None)))
    audit = next((p for p in run.get("purchaseWindows") or [] if p["decisionTick"] == tick), None)
    if audit is not None:
        values[0] = audit["populationBefore"]
    return tuple(values)


def choice_lines(run, horizon, phase_length):
    choices = {c["decisionTick"]: c for c in run.get("mutationChoices") or []}
    acquisitions = run.get("upgradeAcquisitionTimeline") or []
    purchases = {p["decisionTick"]: p for p in run.get("purchaseWindows") or []}
    names = {u["upgradeId"]: u.get("displayName") or readable(u["upgradeId"])
             for u in run.get("upgradeLoadout") or []}
    ticks = set(choices) | set(purchases) | {a["effectiveTick"] for a in acquisitions}
    if phase_length and horizon:
        ticks.update(range(phase_length, horizon, phase_length))
    result = []
    for tick in sorted(ticks):
        if tick > run["ticks"]:
            result.append(f"Tick {tick}: not reached")
            continue
        parts = []
        choice = choices.get(tick)
        if choice:
            uid = choice.get("chosenUpgradeId")
            if uid:
                parts.append(names.get(uid, readable(uid)))
            else:
                parts.append("Skip")
        else:
            gained = [a for a in acquisitions if a["effectiveTick"] == tick]
            parts.extend(a["upgrade"].get("displayName") or readable(a["upgrade"]["upgradeId"]) for a in gained)
        purchase = purchases.get(tick)
        if purchase:
            parts.append(f"+{purchase['added']} Hares; {purchase['spent']} Field Data")
        elif run.get("purchaseWindows") is None:
            parts.append("purchases not recorded")
        if not parts:
            parts.append("choice not recorded")
        result.append(f"Tick {tick}: " + "; ".join(parts))
    return "\n".join(result) or "Decisions and purchases not recorded"


def hare_peak(run, history):
    observed = [(values[0], t) for t, values in history.items() if values[0] is not None]
    for snapshot in run.get('populationHistory') or []:
        value = population(snapshot)[0]
        if value is not None:
            observed.append((value, snapshot['tick']))
    observed.extend((p['populationAfter'], p['decisionTick']) for p in run.get('purchaseWindows') or [])
    if observed:
        return max(observed, key=lambda p: (p[0], -p[1]))
    return None, None


def story(run, history, final, horizon, detailed=False):
    tick = run["ticks"]
    if final[0] == 0:
        lines = [f"Hares became extinct by tick {tick}."]
    elif horizon is not None and tick >= horizon:
        lines = [f"Reached the {horizon}-tick horizon with {counts(final)} Hares / Foxes / Plants."]
    else:
        lines = [f"Recorded endpoint: tick {tick}. Completion reason not recorded."]
    peak, peak_tick = hare_peak(run, history)
    if peak is not None:
        lines.append(f"Highest recorded Hare count: {peak} at tick {peak_tick}.")
    stats = next((s for s in run.get("herbivoreStatLines") or [] if s.get("speciesId") == "hare"), None)
    if stats is None and (run.get("herbivoreStatLine") or {}).get("speciesId") == "hare":
        stats = run["herbivoreStatLine"]
    if stats:
        labels = (("BIR", "births"), ("PREY", "predation deaths"), ("STRV", "starvation deaths"), ("CRWD", "crowding deaths"))
        lines.append("Whole run: " + ", ".join(f"{stats[k]} {label}" for k, label in labels if k in stats) + ".")
    for audit in (run.get("purchaseWindows") or []) if detailed else []:
        if not audit["added"]:
            continue
        following = next((p for p in run.get("phaseResults") or []
                          if p["windowStartTickExclusive"] == audit["decisionTick"]), None)
        if following:
            following_tick = following["windowEndTickInclusive"]
            hares = history.get(following_tick, (None,))[0]
            if hares is not None:
                change = hares - audit["populationAfter"]
                lines.append(f"After +{audit['added']} at tick {audit['decisionTick']}, Hares went "
                             f"{audit['populationAfter']} -> {hares} by tick {following_tick} "
                             f"({change:+d}, excluding the immediate purchase).")
    if any(v is None for v in final):
        lines.append("Some endpoint populations were not recorded.")
    return "\n".join(lines)


def review_row(run, context, checkpoints, detailed=False):
    final = endpoint(run)
    history = snapshots(run)
    windows = run.get("purchaseWindows")
    for audit in windows or []:
        fields = ('decisionTick', 'populationBefore', 'populationAfter', 'requested', 'added', 'spent')
        if any(type(audit.get(k)) is not int or audit[k] < 0 for k in fields):
            raise ValueError(f"Missing or invalid purchase counts on seed {run['seed']}")
        if audit['populationAfter'] != audit['populationBefore'] + audit['added'] or audit['added'] > audit['requested']:
            raise ValueError(f"Purchase population ledger does not reconcile on seed {run['seed']}")
        if 'price' in audit and audit['spent'] != audit['price'] * audit['added']:
            raise ValueError(f"Purchase spending does not reconcile on seed {run['seed']}")
    if windows is None:
        limits = "Purchase ledger not recorded"
    elif not windows:
        limits = "No purchase windows recorded"
    else:
        limits = "\n".join(f"Tick {p['decisionTick']}: {p['added']} of {p['requested']}; "
                           + REASONS.get(p.get("stopReason"), readable(p.get("stopReason", "not recorded")))
                           for p in windows)
    return [f"{run['seed']}\n{context['label']}",
            *(checkpoint(run, tick, history) for tick in checkpoints),
            f"Tick {run['ticks']}\n{counts(final)}", choice_lines(run, context['horizon'], context['phaseLength']),
            limits, story(run, history, final, context['horizon'], detailed), "",
            f"{Path(context['source']).name}\nCondition: {context['condition']}\nSeed: {run['seed']}"]


def run_data(run, context, checkpoints):
    """One scalar per column, one run per row; unavailable numbers stay blank."""
    history, final = snapshots(run), endpoint(run)
    arguments = context.get('arguments', {})
    strategy = context.get('strategy') or arguments.get('-mutationPolicy')
    if not strategy and arguments.get('-phaseUpgradeSchedule') == 'none;none;none;none;none;none':
        strategy = 'skip-all'
    windows = run.get('purchaseWindows')
    policies = {p['policy'] for p in windows or [] if p.get('policy')}
    policy = arguments.get('-harePurchasePolicy') or context.get('purchasePolicy')
    if not policy:
        policy = next(iter(policies)) if len(policies) == 1 else 'Varies by decision' if policies else None
    horizon = context['horizon']
    reached = run['ticks'] >= horizon if horizon is not None else None
    peak, peak_tick = hare_peak(run, history)
    data = {'Seed': run['seed'], 'Strategy': strategy, 'Purchase policy': policy,
            'Condition': context['label'], 'Endpoint tick': run['ticks'], 'Horizon tick': horizon,
            'Reached horizon (0/1)': int(reached) if reached is not None else None,
            'Hares alive at horizon (0/1)': int(reached and final[0] > 0) if reached is not None and final[0] is not None else None,
            'All three alive at horizon (0/1)': int(reached and all(v > 0 for v in final)) if reached is not None and all(v is not None for v in final) else None,
            'Hare extinction endpoint (0/1)': int(final[0] == 0) if final[0] is not None else None,
            'Duration seconds': run.get('durationSeconds'), 'Highest recorded Hares': peak, 'Hare peak tick': peak_tick}
    for option, label in [('-gridWidth', 'Map width'), ('-gridHeight', 'Map height')]:
        data[label] = int(arguments[option]) if option in arguments else None
    data['Wrap map edges (0/1)'] = int(arguments['-wrapEdges'] == 'true') if arguments.get('-wrapEdges') in ('true', 'false') else None
    for entry in arguments.get('-speciesStats', '').split(','):
        if not entry:
            continue
        target, raw = entry.split('=')
        species, attribute = target.split(':')
        value = float(raw)
        if not math.isfinite(value):
            raise ValueError('Stat overrides must be finite')
        data[f'Stat override {readable(species)} {attribute}'] = int(value) if value.is_integer() else value
    def populations(prefix, values):
        for species, value in zip(SPECIES, values):
            data[f'{prefix} {readable(species)} population'] = value
    populations('Initial', history.get(0, (None,) * len(SPECIES)))
    populations('Final', final)
    for tick in checkpoints:
        values = checkpoint_values(run, tick, history)
        populations(f'Tick {tick} before purchases', values)
        data[f'Tick {tick} population status'] = ('Not reached' if run['ticks'] < tick else
            'Recorded' if all(v is not None for v in values) else 'Not recorded' if all(v is None for v in values) else 'Partially recorded')
    def stat_lines(payload, prefix):
        for role, species in [('herbivore', 'hare'), ('predator', 'fox')]:
            plural, singular = role + 'StatLines', role + 'StatLine'
            lines = payload.get(plural) or []
            legacy = payload.get(singular) or {}
            if not lines and legacy.get('speciesId') == species:
                lines = [legacy]
            seen = set()
            for line in lines:
                identity = line['speciesId']
                if identity in seen:
                    raise ValueError(f'Duplicate {role} stat line for {identity}')
                seen.add(identity)
                for metric, value in line.items():
                    if metric == 'speciesId':
                        continue
                    if value is not None and not isinstance(value, (str, int, float, bool)):
                        raise ValueError(f'Expected a scalar stat-line metric: {metric}')
                    label = metric[:-6] + ' status' if metric.endswith('Status') else metric
                    # Source report zero placeholders for N/A/invalid rates must not bias pivot averages.
                    status = line.get(metric + 'Status')
                    if status is not None and status != 'Valid':
                        value = None
                    data[f'{prefix} {readable(identity)} {role} {label}'] = int(value) if isinstance(value, bool) else value
    stat_lines(run, 'Run')
    phase_rows = run.get('phaseResults') or []
    phases = {p['phaseIndex']: p for p in phase_rows}
    if len(phases) != len(phase_rows):
        raise ValueError('Duplicate round index in Run data')
    count = max(context.get('phaseCount', 0), max(phases, default=0))
    if count > 100:
        raise ValueError('Run data supports up to 100 recorded rounds; split a larger experiment before export')
    for index in range(1, count + 1):
        phase = phases.get(index)
        prefix = f'Round {index}'
        data[prefix + ' status'] = 'Recorded' if phase else 'Not reached' if context['phaseLength'] and run['ticks'] <= (index - 1) * context['phaseLength'] else 'Not recorded'
        if not phase:
            continue
        start, end = phase['windowStartTickExclusive'], phase['windowEndTickInclusive']
        data[prefix + ' start tick (exclusive)'] = start
        data[prefix + ' end tick (inclusive)'] = end
        data[prefix + ' observed ticks'] = end - start
        for name in ('openingPopulation', 'closingPopulation'):
            observed = phase.get(name) or []
            populations(prefix + (' opening' if name == 'openingPopulation' else ' closing'), population(observed[-1]) if observed else (None,) * len(SPECIES))
        stat_lines(phase, prefix)
    choices = {c['decisionTick']: c for c in run.get('mutationChoices') or []}
    purchases = {p['decisionTick']: p for p in windows or []}
    ticks = set(choices) | set(purchases)
    acquired = collections.defaultdict(list)
    for item in run.get('upgradeAcquisitionTimeline') or []:
        acquired[item['effectiveTick']].append(item)
    ticks.update(acquired)
    if context['phaseLength'] and horizon:
        ticks.update(range(context['phaseLength'], horizon, context['phaseLength']))
    if len(ticks) > 100:
        raise ValueError('Run data supports up to 100 decision windows; split a larger experiment before export')
    index = 0
    for tick in sorted(ticks):
        if tick > 0:
            index += 1
        prefix = f'Decision {index}' if tick > 0 else 'Initial'
        data[prefix + ' tick'] = tick
        data[prefix + ' reached (0/1)'] = int(run['ticks'] >= tick)
        choice, purchase = choices.get(tick), purchases.get(tick)
        data[prefix + ' mutation status'] = 'Recorded' if choice else 'Not reached' if run['ticks'] < tick else 'Not recorded'
        data[prefix + ' purchase status'] = 'Recorded' if purchase else 'Not reached' if run['ticks'] < tick else 'Not recorded'
        if choice:
            for key, value in choice.items():
                if key == 'offeredUpgradeIds':
                    for slot, uid in enumerate(value or [], 1):
                        data[f'{prefix} offered mutation {slot} ID'] = uid
                else:
                    data[prefix + ' mutation ' + key] = value
        if purchase:
            for key, value in purchase.items():
                data[prefix + ' purchase ' + key] = value
            following = next((p for p in phases.values() if p['windowStartTickExclusive'] == tick), None)
            if following:
                end = following['windowEndTickInclusive']
                closing = history.get(end, (None,))[0]
                data[prefix + ' following endpoint tick'] = end
                data[prefix + ' Hare change after immediate purchase'] = closing - purchase['populationAfter'] if closing is not None else None
        for slot, item in enumerate(acquired[tick], 1):
            for key in ('upgradeId', 'displayName', 'targetSpeciesId', 'scope'):
                data[f'{prefix} acquisition {slot} {key}'] = item['upgrade'].get(key)
    data['Loadout status'] = 'Recorded' if isinstance(run.get('upgradeLoadout'), list) else 'Not recorded'
    for uid, amount in sorted(collections.Counter(u['upgradeId'] for u in run.get('upgradeLoadout') or []).items()):
        data[f'Final loadout {uid} count'] = amount
    data.update({'Condition ID': context['condition'], 'Batch identity': context.get('identity'),
                 'Source report': context['source'], 'Source hash': context.get('sourceHash')})
    return data


def data_table(records):
    headers = list(dict.fromkeys(key for record in records for key in record))
    if len(headers) > 16384:
        raise ValueError('Run data exceeds Excel column limits; export fewer types of conditions together')
    rows = [[record.get(key, 0 if key.startswith('Final loadout ') and record.get('Loadout status') == 'Recorded' else None)
             for key in headers] for record in records]
    for row in rows:
        if any(value is not None and not isinstance(value, (str, int, float, bool)) for value in row):
            raise ValueError('Run data contains a non-scalar value')
    formats = []
    for index in range(len(headers)):
        values = [row[index] for row in rows if row[index] is not None]
        formats.append('0.0000' if any(type(value) is float for value in values) else
                       '#,##0' if values and all(isinstance(value, (int, bool)) for value in values) else 'General')
    return dict(headers=headers, rows=rows, formats=formats)


def resolve_input(path):
    path = Path(path).resolve()
    if path.is_dir():
        for candidate in (path / "sweep/sweep.json", path / "sweep.json"):
            if candidate.exists():
                return candidate
        if (path / "report.json").exists():
            return path / "report.json"
        raise ValueError(f"Choose a report.json file or a completed sweep folder: {path}")
    if not path.is_file():
        raise ValueError(f"Input does not exist: {path}")
    if path.name == "runs.jsonl":
        return resolve_input(path.parent.parent)
    return path


def contexts_and_runs(path):
    if path.name == "sweep.json":
        print("Verifying completed sweep inputs and raw-report hash ...", file=sys.stderr, flush=True)
        root, sweep, manifest, raw = completed(path.parent)
        contexts = {}
        for case in sweep["cases"]:
            arguments = tokens(next(c["arguments"] for c in manifest["plan"]["conditions"] if c["id"] == case["id"]))
            label = " / ".join([*(readable(v) for v in case["axes"].values()), readable(case["path"])])
            contexts[case["id"]] = dict(label=label, condition=case["id"], source=str(raw),
                horizon=sweep["horizonTicks"], phaseLength=int(arguments.get("-phaseLengthTicks", "0")),
                phaseCount=6 if arguments.get('-mutationPolicy') else len(arguments['-phaseUpgradeSchedule'].split(';')) if arguments.get('-phaseUpgradeSchedule') else 0,
                strategy=case['path'],
                identity=manifest["identity"], sourceHash=manifest["sourceHash"], arguments=arguments)

        def runs():
            with raw.open(encoding="utf-8-sig") as stream:
                for line_number, line in enumerate(stream, 1):
                    row = json.loads(line)
                    if row["condition"] not in contexts:
                        raise ValueError(f"Unknown condition in {raw}, line {line_number}")
                    yield row["condition"], row["run"]
        return contexts, runs(), manifest["plan"]["seedCount"], sweep["seedStart"]
    if path.stat().st_size > 128 * 1024 * 1024:
        raise ValueError("This JSON report is over 128 MB. Select the completed sweep folder for streaming export.")
    report = load(path)
    if not isinstance(report, dict) or not isinstance(report.get("runs"), list) or not report["runs"]:
        raise ValueError(f"Expected a full CellSim report with a nonempty runs array: {path}")
    cohort = report.get("portableCohort") or {}
    horizon = report.get("runTicks") or None
    if report.get("phaseLengthTicks") and report.get("phaseCount"):
        horizon = report["phaseLengthTicks"] * report["phaseCount"]
    condition = cohort.get("condition") or path.parent.name
    policy = {p.get("policy") for r in report["runs"] for p in r.get("purchaseWindows") or []}
    label = " / ".join(filter(None, [report.get("scenarioName") or path.parent.name,
        readable(report.get("mutationPolicyId") or report.get("upgradeId") or "Recorded choices"),
        ", ".join(readable(p) for p in sorted(policy) if p)]))
    context = dict(label=label, condition=condition, source=str(path), horizon=horizon,
        phaseLength=report.get("phaseLengthTicks", 0), identity=cohort.get("batchIdentity", "Not recorded"),
        phaseCount=report.get('phaseCount', 0), strategy=report.get('mutationPolicyId'),
        sourceHash=cohort.get("sourceHash", report.get("sourceHash", "Not recorded")),
        arguments=cohort.get("arguments", {}))
    return {condition: context}, ((condition, run) for run in report["runs"]), None, None


def settings(arguments):
    populations = dict(item.split('=') for item in arguments.get('-startingPopulations', '').split(',') if item)
    initial = ' / '.join(populations.get(s, 'Not recorded') for s in SPECIES)
    grid = f"{arguments.get('-gridWidth', 'Not recorded')} × {arguments.get('-gridHeight', 'Not recorded')}"
    wrap = arguments.get('-wrapEdges')
    grid += ' (wrapped)' if wrap == 'true' else ' (closed edges)' if wrap == 'false' else ' (wrapping not recorded)'
    names = {'fox:energy.starting': 'Fox starting energy', 'hare:awareness.vision-range': 'Hare vision'}
    overrides = [f"{names.get(key, key)} {value}" for key, value in
                 (item.split('=') for item in arguments.get('-speciesStats', '').split(',') if item)]
    return dict(populations=initial, map=grid,
                overrides=', '.join(overrides) or ('No explicit stat overrides' if arguments else 'Not recorded'))


def experiment_details(path, contexts, seen):
    name, owner, question = path.parent.name, 'Not recorded', 'Not recorded'
    notes_path = path.parent.parent / 'experiment.json'
    manifest_path = path.parent.parent / 'workbench-run.json'
    if path.name == 'sweep.json' and manifest_path.exists():
        manifest = load(manifest_path)
        if notes_path.stat().st_size > 1024 * 1024:
            raise ValueError('Experiment notes are unexpectedly large')
        raw = notes_path.read_bytes()
        if hashlib.sha256(raw).hexdigest() != manifest['files']['experiment.json']:
            raise ValueError('Frozen experiment notes changed; use the original evidence')
        notes = json.loads(raw)
        name, owner, question = notes['Name'], notes['Owner'], notes['Question']
    elif path.name != 'sweep.json':
        name = next(iter(contexts.values()))['label']
    def common(field):
        values = {c['setup'][field] for c in contexts.values()}
        return next(iter(values)) if len(values) == 1 else 'Varies by condition; see settings columns'
    selected_seeds = sorted(set().union(*seen.values()))
    timing = {f"{c['phaseLength']} ticks per round; up to {c['horizon']} ticks" if c['phaseLength'] and c['horizon']
              else f"Up to {c['horizon']} ticks" if c['horizon'] is not None else 'Not recorded' for c in contexts.values()}
    strategies = {readable(c.get('arguments', {}).get('-mutationPolicy') or
                  ('skip-all' if c.get('arguments', {}).get('-phaseUpgradeSchedule') == 'none;none;none;none;none;none' else 'Not recorded'))
                  for c in contexts.values()}
    policies = {c.get('arguments', {}).get('-harePurchasePolicy', 'Not recorded') for c in contexts.values()}
    return dict(name=name, owner=owner, question=question,
                populations=common('populations'), map=common('map'), overrides=common('overrides'),
                timing=next(iter(timing)) if len(timing) == 1 else 'Varies by condition',
                strategies=', '.join(sorted(strategies)), policies=', '.join('No purchases' if p == 'none' else readable(p) for p in sorted(policies)),
                seeds=f"{selected_seeds[0]}–{selected_seeds[-1]} ({len(selected_seeds):,} selected seeds)")


def summary_rows(groups):
    rows = []
    for group in groups:
        c, t = group['context'], group['totals']
        known_horizon = t.get('knownHorizon', 0) == t['runs']
        hare_known = known_horizon and not t.get('missingHareEndpoint', 0)
        all_known = known_horizon and not t.get('missingEndpoint', 0)
        rows.append([c['label'], t['runs'], c['horizon'] if c['horizon'] is not None else 'Not recorded',
            t.get('aliveAtHorizon', 0) if hare_known else 'Not recorded',
            t.get('aliveAtHorizon', 0) / t['runs'] if hare_known else 'Not recorded',
            t.get('allSpeciesAliveAtHorizon', 0) if all_known else 'Not recorded',
            t.get('allSpeciesAliveAtHorizon', 0) / t['runs'] if all_known else 'Not recorded',
            t.get('reached', 0) if known_horizon else 'Not recorded', t.get('extinct', 0),
            t.get('missingEndpoint', 0), c['setup']['populations'], c['setup']['map'], c['setup']['overrides'],
            c['condition'], c['source'], c.get('identity', 'Not recorded')])
    return rows


def prepare(inputs, checkpoints=(400, 500), max_runs=200, match="", seeds=(), detailed=False):
    if not 1 <= max_runs <= 1000 or len(checkpoints) != 2 or any(type(t) is not int or t < 0 for t in checkpoints):
        raise ValueError("Use a row limit of 1..1000 and exactly two nonnegative checkpoint ticks")
    paths = list(dict.fromkeys(resolve_input(p) for p in inputs))
    groups, sources, candidate_rows, experiments = [], [], [], []
    for path in paths:
        print(f"Reading {path} ...", file=sys.stderr, flush=True)
        contexts, runs, expected_count, seed_start = contexts_and_runs(path)
        for context in contexts.values():
            context['setup'] = settings(context.get('arguments', {}))
        selected = {key: c for key, c in contexts.items()
                    if all(term in (c['label'] + ' ' + key).lower().replace('-', ' ')
                           for term in match.lower().replace('-', ' ').split())}
        capacity = math.ceil(max_runs / max(1, len(selected)))
        samples = {key: {} for key in selected}
        totals = {key: collections.Counter() for key in selected}
        seen = {key: set() for key in contexts}
        selected_seeds = {key: set() for key in selected}
        for number, (key, run) in enumerate(runs, 1):
            if number % 10000 == 0:
                print(f"Read {number:,} saved runs ...", file=sys.stderr, flush=True)
            final = endpoint(run)
            seed = run["seed"]
            if seed in seen[key]:
                raise ValueError(f"Duplicate seed {seed} in {path}, condition {key}")
            seen[key].add(seed)
            if key not in selected or (seeds and seed not in seeds):
                continue
            context = selected[key]
            selected_seeds[key].add(seed)
            total = totals[key]
            total['runs'] += 1
            total['extinct'] += final[0] == 0
            known = context['horizon'] is not None
            total['knownHorizon'] += known
            total['reached'] += known and run['ticks'] >= context['horizon']
            total['aliveAtHorizon'] += known and run['ticks'] >= context['horizon'] and final[0] is not None and final[0] > 0
            total['allSpeciesAliveAtHorizon'] += known and run['ticks'] >= context['horizon'] and all(v is not None and v > 0 for v in final)
            total['missingHareEndpoint'] += final[0] is None
            total['missingEndpoint'] += any(v is None for v in final)
            sample = samples[key]
            if len(sample) < capacity or seed < max(sample):
                sample[seed] = (review_row(run, context, checkpoints, detailed), str(Path(context['source']).parent), run_data(run, context, checkpoints))
                if len(sample) > capacity:
                    del sample[max(sample)]
        if expected_count is not None:
            expected_seeds = set(range(seed_start, seed_start + expected_count))
            if any(found != expected_seeds for found in seen.values()):
                raise ValueError(f"Seed coverage differs from the completed sweep: {path}")
        active = {key: context for key, context in selected.items() if totals[key]['runs']}
        if active:
            experiments.append(experiment_details(path, active, selected_seeds))
        for key, context in selected.items():
            if not totals[key]['runs']:
                continue
            groups.append(dict(context=context, totals=dict(totals[key])))
            sources.append(dict(input=str(path), **context))
            for rank, seed in enumerate(sorted(samples[key])):
                row, folder, numeric = samples[key][seed]
                candidate_rows.append((rank, seed, context['label'], row, folder, numeric))
    if not groups:
        raise ValueError("No runs matched. Check the input, text filter and selected seeds.")
    # Lowest seeds across conditions: transparent sampling, never a claim of typicality.
    candidate_rows.sort(key=lambda r: (r[0], r[1], r[2]))
    selected_rows = candidate_rows[:max_runs]
    total_runs = sum(g['totals']['runs'] for g in groups)
    selection = ("All matching runs" if total_runs <= max_runs else
                 f"Lowest seeds, round-robin across {len(groups)} conditions; examples, not a statistical sample")
    selected_rows.sort(key=lambda r: (r[1], r[2]))
    numeric = data_table([dict(r[5], **{'Run review row': index}) for index, r in enumerate(selected_rows, 6)])
    return dict(schemaVersion=3, generatedUtc=dt.datetime.now(dt.timezone.utc).isoformat(),
        headers=["Seed / choices / purchase policy", *(f"Tick {t}\nHares / Foxes / Plants\nbefore purchases" for t in checkpoints),
                 "Endpoint tick\nHares / Foxes / Plants", "Choices and purchases", "Purchase limits and reasons",
                 "What happened? (recorded observations)", "Your playtest notes (optional)", "Source evidence"],
        rows=[r[3] for r in selected_rows], evidenceFolders=[r[4] for r in selected_rows],
        totalRuns=total_runs, exportedRows=len(selected_rows), selection=selection, groups=groups, sources=sources,
        experiments=experiments, summaryRows=summary_rows(groups), detailedObservations=detailed, runData=numeric)


def link_evidence(workbook_path, folders):
    """Add native links: artifact-tool cannot calculate HYPERLINK or expose native links."""
    workbook_path = Path(workbook_path).resolve()
    main = 'http://schemas.openxmlformats.org/spreadsheetml/2006/main'
    office = 'http://schemas.openxmlformats.org/officeDocument/2006/relationships'
    package = 'http://schemas.openxmlformats.org/package/2006/relationships'
    descriptor, temporary = tempfile.mkstemp(suffix='.xlsx', dir=workbook_path.parent)
    os.close(descriptor)
    try:
        with zipfile.ZipFile(workbook_path) as source:
            book = ET.fromstring(source.read('xl/workbook.xml'))
            review = next((s for s in book.find(f'{{{main}}}sheets') if s.get('name') == 'Run review'), None)
            if review is None:
                raise ValueError('Workbook has no Run review sheet for evidence links')
            book_links = ET.fromstring(source.read('xl/_rels/workbook.xml.rels'))
            target = next((r.get('Target') for r in book_links if r.get('Id') == review.get(f'{{{office}}}id')), None)
            if not target:
                raise ValueError('Run review sheet relationship is missing')
            sheet_name = target.lstrip('/') if target.startswith('/') else posixpath.normpath('xl/' + target)
            rel_name = posixpath.dirname(sheet_name) + '/_rels/' + posixpath.basename(sheet_name) + '.rels'
            sheet = ET.fromstring(source.read(sheet_name))
            relationships = ET.fromstring(source.read(rel_name)) if rel_name in source.namelist() else ET.Element(f'{{{package}}}Relationships')
            links = ET.Element(f'{{{main}}}hyperlinks')
            used = {r.get('Id') for r in relationships}
            for index, folder in enumerate(folders, 6):
                identity = f'CellSimEvidence{index}'
                if identity in used:
                    raise ValueError('Evidence links already exist; do not finalize a workbook twice')
                ET.SubElement(relationships, f'{{{package}}}Relationship', Id=identity,
                    Type=office + '/hyperlink', Target=Path(folder).resolve().as_uri() + '/', TargetMode='External')
                ET.SubElement(links, f'{{{main}}}hyperlink', {'ref': f'I{index}', f'{{{office}}}id': identity})
            # CT_Worksheet orders hyperlinks before print/drawing/table metadata.
            after_links = {'printOptions', 'pageMargins', 'pageSetup', 'headerFooter', 'rowBreaks', 'colBreaks',
                           'customProperties', 'cellWatches', 'ignoredErrors', 'smartTags', 'drawing',
                           'legacyDrawing', 'legacyDrawingHF', 'picture', 'oleObjects', 'controls',
                           'webPublishItems', 'tableParts', 'extLst'}
            position = next((i for i, child in enumerate(sheet) if child.tag.split('}')[-1] in after_links), len(sheet))
            sheet.insert(position, links)
            changes = {sheet_name: ET.tostring(sheet, encoding='utf-8', xml_declaration=True),
                       rel_name: ET.tostring(relationships, encoding='utf-8', xml_declaration=True)}
            with zipfile.ZipFile(temporary, 'w', compression=zipfile.ZIP_DEFLATED) as target:
                for info in source.infolist():
                    content = changes.pop(info.filename) if info.filename in changes else source.read(info.filename)
                    target.writestr(info, content)
                for name, content in changes.items():
                    target.writestr(name, content)
        os.replace(temporary, workbook_path)
    finally:
        if os.path.exists(temporary):
            os.unlink(temporary)


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("inputs", nargs="+")
    parser.add_argument("--output", required=True, help="New intermediate JSON path")
    parser.add_argument("--checkpoints", nargs=2, type=int, default=[400, 500])
    parser.add_argument("--max-runs", type=int, default=200)
    parser.add_argument("--match", default="", help="Words matched against readable condition names")
    parser.add_argument("--seeds", nargs="*", type=int, default=[])
    parser.add_argument("--detailed-observations", action="store_true", help="Include purchase-by-purchase population changes")
    parser.add_argument("--link-evidence", action="store_true", help=argparse.SUPPRESS)
    args = parser.parse_args()
    try:
        if args.link_evidence:
            link_evidence(args.output, load(args.inputs[0])['evidenceFolders'])
            return
        result = prepare(args.inputs, args.checkpoints, args.max_runs, args.match, args.seeds, args.detailed_observations)
        with Path(args.output).open("x", encoding="utf-8") as stream:
            json.dump(result, stream, ensure_ascii=False, allow_nan=False)
        print(f"Prepared {result['exportedRows']} worksheet rows from {result['totalRuns']:,} matching runs.")
    except (ValueError, OSError, KeyError, TypeError) as error:
        parser.exit(1, f"Worksheet export stopped: {error}\n")


if __name__ == "__main__":
    main()
