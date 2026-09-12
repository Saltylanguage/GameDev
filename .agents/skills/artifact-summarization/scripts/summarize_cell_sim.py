#!/usr/bin/env python3
"""Create a compact, decision-oriented summary of one Unity CellSim report.

ponytail: V1 loads one report with the standard library. Add a streaming JSON
reader only if report size becomes a demonstrated bottleneck; do not add a
dependency just to avoid a measured problem.
"""

from __future__ import annotations

import argparse
import json
import math
import re
import statistics
from collections import Counter, defaultdict
from pathlib import Path
from typing import Any, Iterable


CORE_REPORT_FIELDS = {
    "schemaVersion",
    "createdUtc",
    "scenarioAssetPath",
    "outputPath",
    "rulesetFingerprint",
    "runProvenanceFingerprint",
    "playerSpeciesId",
    "upgradeId",
    "upgradeType",
    "upgradeValue",
    "orderedLoadout",
    "upgradeContractVersion",
    "upgradeCatalogPath",
    "upgradeRegistryFingerprint",
    "upgradeLoadoutFingerprint",
    "predictionInput",
    "combatResolutionMode",
    "attackOpportunityMode",
    "experimentalFeatures",
    "foxAttackCooldownTicks",
    "preContactAvoidanceChance",
    "seedStart",
    "seedCount",
    "gridWidth",
    "gridHeight",
    "runTicks",
    "phaseLengthTicks",
    "phaseCount",
    "phaseUpgradeSchedule",
    "runDurationSeconds",
    "stepIntervalSeconds",
    "runs",
}

RUN_FIELDS = {
    "seed",
    "ticks",
    "durationSeconds",
    "playerPopulation",
    "currencyEarned",
    "upgradeLoadout",
    "populationHistory",
    "activity",
    "behavior",
    "behaviorTransitions",
    "trackedBehavior",
    "deathEvents",
    "combatRolls",
    "combatCooldownSuppressions",
    "phaseResults",
    "upgradeAcquisitionTimeline",
    "herbivoreStatLine",
    "opportunityControl",
}


def load_json(path: Path, diagnostics: list[str] | None = None) -> dict[str, Any]:
    with path.open("r", encoding="utf-8-sig") as handle:
        try:
            return json.load(handle)
        except json.JSONDecodeError as original_error:
            handle.seek(0)
            raw = handle.read()
    repaired = re.sub(r",(\s*[}\]])", r"\1", raw)
    if repaired == raw:
        raise original_error
    try:
        parsed = json.loads(repaired)
    except json.JSONDecodeError:
        raise original_error
    if diagnostics is not None:
        diagnostics.append(
            "Recovered legacy JSON by removing trailing commas; inspect the source before treating it as clean evidence."
        )
    return parsed


def number(value: Any, default: float = 0.0) -> float:
    return value if isinstance(value, (int, float)) and not isinstance(value, bool) else default


def round_number(value: float) -> int | float:
    if not math.isfinite(value):
        return 0
    rounded = round(value, 6)
    return int(rounded) if float(rounded).is_integer() else rounded


def mean(values: Iterable[float]) -> int | float | None:
    values = list(values)
    return round_number(statistics.fmean(values)) if values else None


def copy_population(snapshot: Any) -> dict[str, int]:
    result: dict[str, int] = {}
    if not isinstance(snapshot, list):
        return result
    for species in snapshot[0].get("species", []) if snapshot else []:
        if isinstance(species, dict) and species.get("speciesId"):
            result[str(species["speciesId"])] = int(number(species.get("population")))
    return result


def activity_by_species(entries: Any) -> dict[str, dict[str, Any]]:
    result: dict[str, dict[str, Any]] = {}
    if not isinstance(entries, list):
        return result
    for entry in entries:
        if not isinstance(entry, dict) or not entry.get("speciesId"):
            continue
        result[str(entry["speciesId"])] = {
            key: value
            for key, value in entry.items()
            if key != "speciesId" and isinstance(value, (int, float)) and not isinstance(value, bool)
        }
    return result


def behavior_totals(entries: Any) -> dict[str, dict[str, int]]:
    result: dict[str, Counter[str]] = defaultdict(Counter)
    if not isinstance(entries, list):
        return {}
    for entry in entries:
        if not isinstance(entry, dict):
            continue
        species = str(entry.get("speciesId", "unknown"))
        state = str(entry.get("state", "unknown"))
        result[species][state] += int(number(entry.get("ticks")))
    return {species: dict(states) for species, states in result.items()}


def transition_totals(entries: Any) -> dict[str, dict[str, int]]:
    result: dict[str, Counter[str]] = defaultdict(Counter)
    if not isinstance(entries, list):
        return {}
    for entry in entries:
        if not isinstance(entry, dict):
            continue
        species = str(entry.get("speciesId", "unknown"))
        transition = f"{entry.get('previousState', 'unknown')}->{entry.get('currentState', 'unknown')}"
        result[species][transition] += 1
    return {species: dict(transitions) for species, transitions in result.items()}


def death_totals(entries: Any) -> dict[str, int]:
    result: Counter[str] = Counter()
    if not isinstance(entries, list):
        return {}
    for entry in entries:
        if isinstance(entry, dict):
            result[f"{entry.get('speciesId', 'unknown')}:{entry.get('cause', 'unknown')}"] += 1
    return dict(result)


def combat_totals(entries: Any) -> dict[str, dict[str, Any]]:
    grouped: dict[str, list[dict[str, Any]]] = defaultdict(list)
    if not isinstance(entries, list):
        return {}
    for entry in entries:
        if isinstance(entry, dict):
            key = f"{entry.get('attackerSpeciesId', 'unknown')}->{entry.get('targetSpeciesId', 'unknown')}"
            grouped[key].append(entry)

    result: dict[str, dict[str, Any]] = {}
    numeric_fields = (
        "attackRoll",
        "attackModifier",
        "blockRoll",
        "blockModifier",
        "attackTotal",
        "blockTotal",
        "expectedHitProbability",
    )
    for key, values in grouped.items():
        row: dict[str, Any] = {
            "attempts": len(values),
            "hits": sum(1 for value in values if value.get("hit") is True),
            "misses": sum(1 for value in values if value.get("hit") is False),
        }
        for field in numeric_fields:
            numbers = [number(value.get(field)) for value in values if field in value]
            if numbers:
                row[field] = {
                    "min": round_number(min(numbers)),
                    "max": round_number(max(numbers)),
                    "mean": mean(numbers),
                }
        result[key] = row
    return result


def population_stats(history: Any) -> dict[str, dict[str, Any]]:
    samples: dict[str, list[float]] = defaultdict(list)
    if not isinstance(history, list):
        return {}
    for snapshot in history:
        if not isinstance(snapshot, dict):
            continue
        for species in snapshot.get("species", []):
            if isinstance(species, dict) and species.get("speciesId"):
                samples[str(species["speciesId"])].append(number(species.get("population")))
    result: dict[str, dict[str, Any]] = {}
    for species, values in samples.items():
        result[species] = {
            "samples": len(values),
            "first": round_number(values[0]) if values else 0,
            "last": round_number(values[-1]) if values else 0,
            "min": round_number(min(values)) if values else 0,
            "max": round_number(max(values)) if values else 0,
            "mean": mean(values),
            "populationSampleIntegral": round_number(sum(values)),
        }
    return result


def compact_upgrade(upgrade: Any) -> dict[str, Any]:
    if not isinstance(upgrade, dict):
        return {}
    result = {
        key: upgrade[key]
        for key in (
            "order",
            "upgradeId",
            "displayName",
            "targetSpeciesId",
            "scope",
            "cost",
            "contractVersion",
            "registryFingerprint",
            "fingerprint",
        )
        if key in upgrade
    }
    result["modifiers"] = [
        {"attributeId": modifier.get("attributeId"), "signedValue": modifier.get("signedValue")}
        for modifier in upgrade.get("modifiers", [])
        if isinstance(modifier, dict)
    ]
    return result


def acquisition_timeline(entries: Any) -> list[dict[str, Any]]:
    result = []
    if not isinstance(entries, list):
        return result
    for entry in entries:
        if isinstance(entry, dict):
            result.append(
                {
                    "effectiveTick": entry.get("effectiveTick"),
                    "phaseIndex": entry.get("phaseIndex"),
                    "order": entry.get("order"),
                    "upgrade": compact_upgrade(entry.get("upgrade")),
                }
            )
    return result


def phase_summary(phase: dict[str, Any]) -> dict[str, Any]:
    closing = copy_population(phase.get("closingPopulation"))
    opening = copy_population(phase.get("openingPopulation"))
    return {
        "phaseIndex": phase.get("phaseIndex"),
        "window": [phase.get("windowStartTickExclusive"), phase.get("windowEndTickInclusive")],
        "contractVersion": phase.get("contractVersion"),
        "rulesetFingerprint": phase.get("rulesetFingerprint"),
        "effectiveUpgradeLoadout": [compact_upgrade(value) for value in phase.get("effectiveUpgradeLoadout", [])],
        "openingPopulation": opening,
        "closingPopulation": closing,
        "activity": activity_by_species(phase.get("activity")),
        "behaviorTicks": behavior_totals(phase.get("behavior")),
        "transitionCounts": transition_totals(phase.get("behaviorTransitions")),
        "deathCounts": death_totals(phase.get("deathEvents")),
        "combat": combat_totals(phase.get("combatRolls")),
        "combatCooldownSuppressions": len(phase.get("combatCooldownSuppressions", []) or []),
        "herbivoreStatLine": phase.get("herbivoreStatLine"),
    }


def run_summary(run: dict[str, Any]) -> dict[str, Any]:
    tracked = []
    for entry in run.get("trackedBehavior", []) or []:
        if not isinstance(entry, dict):
            continue
        tracked.append(
            {
                key: entry[key]
                for key in ("speciesId", "age", "x", "y", "state", "stateTicks")
                if key in entry
            }
        )
    opportunity = run.get("opportunityControl") or {}
    opportunity_summary = {
        key: opportunity[key]
        for key, value in opportunity.items()
        if key != "opportunityAudit" and isinstance(value, (int, float, str, bool))
    }
    opportunity_summary["opportunityAuditRows"] = len(opportunity.get("opportunityAudit", []) or [])
    return {
        "seed": run.get("seed"),
        "ticks": run.get("ticks"),
        "durationSeconds": run.get("durationSeconds"),
        "playerPopulation": run.get("playerPopulation"),
        "currencyEarned": run.get("currencyEarned"),
        "populationStats": population_stats(run.get("populationHistory")),
        "finalPopulation": copy_population(run.get("populationHistory", [])[-1:] if run.get("populationHistory") else []),
        "activity": activity_by_species(run.get("activity")),
        "behaviorTicks": behavior_totals(run.get("behavior")),
        "transitionCounts": transition_totals(run.get("behaviorTransitions")),
        "deathCounts": death_totals(run.get("deathEvents")),
        "combat": combat_totals(run.get("combatRolls")),
        "combatCooldownSuppressions": len(run.get("combatCooldownSuppressions", []) or []),
        "trackedBehavior": tracked,
        "upgradeAcquisitionTimeline": acquisition_timeline(run.get("upgradeAcquisitionTimeline")),
        "herbivoreStatLine": run.get("herbivoreStatLine"),
        "opportunityControl": opportunity_summary,
        "phases": [phase_summary(phase) for phase in run.get("phaseResults", []) if isinstance(phase, dict)],
    }


def core_summary(report: dict[str, Any], source: Path, parse_warnings: list[str] | None = None) -> dict[str, Any]:
    manifest_path = source.with_name("manifest.json")
    manifest = load_json(manifest_path) if manifest_path.exists() else {}
    summary = {
        "summarySchemaVersion": 1,
        "source": {
            "reportPath": str(source),
            "manifestPath": str(manifest_path) if manifest_path.exists() else "",
            "reportSha256": manifest.get("reportSha256", ""),
            "sourceCommit": manifest.get("sourceCommit", ""),
            "sourceTreeDirtyBeforeRun": manifest.get("sourceTreeDirtyBeforeRun"),
            "sourceTreeDirtyAfterRun": manifest.get("sourceTreeDirtyAfterRun"),
            "scenarioAssetGuid": manifest.get("scenarioAssetGuid", ""),
            "parseWarnings": list(parse_warnings or []),
        },
        "experiment": {
            key: report.get(key)
            for key in (
                "schemaVersion",
                "createdUtc",
                "scenarioAssetPath",
                "rulesetFingerprint",
                "runProvenanceFingerprint",
                "playerSpeciesId",
                "combatResolutionMode",
                "attackOpportunityMode",
                "experimentalFeatures",
                "foxAttackCooldownTicks",
                "preContactAvoidanceChance",
                "seedStart",
                "seedCount",
                "gridWidth",
                "gridHeight",
                "runTicks",
                "phaseLengthTicks",
                "phaseCount",
                "phaseUpgradeSchedule",
                "runDurationSeconds",
                "stepIntervalSeconds",
            )
        },
        "upgradeContract": {
            key: report.get(key)
            for key in (
                "upgradeId",
                "upgradeType",
                "upgradeValue",
                "orderedLoadout",
                "upgradeContractVersion",
                "upgradeCatalogPath",
                "upgradeRegistryFingerprint",
                "upgradeLoadoutFingerprint",
                "predictionInput",
            )
        },
        "runs": [run_summary(run) for run in report.get("runs", []) if isinstance(run, dict)],
        "unrecognizedReportFields": sorted(set(report) - CORE_REPORT_FIELDS),
        "unrecognizedRunFields": sorted(
            {key for run in report.get("runs", []) if isinstance(run, dict) for key in set(run) - RUN_FIELDS}
        ),
        "compression": {
            "rawEventArraysOmitted": [
                "populationHistory",
                "behaviorTransitions",
                "deathEvents",
                "combatRolls",
                "opportunityAudit",
            ],
            "retainedAsAggregates": [
                "population first/last/min/max/mean/integral",
                "activity counters by species",
                "behavior ticks by state",
                "transition counts",
                "death counts by species and cause",
                "combat attempts, hits, misses, and roll distributions",
                "phase opening/closing populations and effective loadouts",
            ],
        },
    }
    return summary


def numeric_delta(left: Any, right: Any) -> Any:
    if isinstance(left, (int, float)) and isinstance(right, (int, float)):
        return round_number(left - right)
    return None


def comparison(
    summary: dict[str, Any],
    baseline: dict[str, Any],
    baseline_path: Path,
    baseline_parse_warnings: list[str] | None = None,
) -> dict[str, Any]:
    left_by_seed = {run.get("seed"): run for run in summary["runs"]}
    right_by_seed = {run.get("seed"): run for run in baseline["runs"]}
    seeds = sorted(set(left_by_seed) & set(right_by_seed))
    rows = []
    for seed in seeds:
        left = left_by_seed[seed]
        right = right_by_seed[seed]
        species = sorted(set(left.get("finalPopulation", {})) | set(right.get("finalPopulation", {})))
        rows.append(
            {
                "seed": seed,
                "currencyEarnedDelta": numeric_delta(left.get("currencyEarned"), right.get("currencyEarned")),
                "finalPopulationDelta": {
                    species_id: numeric_delta(
                        left.get("finalPopulation", {}).get(species_id, 0),
                        right.get("finalPopulation", {}).get(species_id, 0),
                    )
                    for species_id in species
                },
                "phaseClosingPopulationDelta": [
                    {
                        "phaseIndex": left_phase.get("phaseIndex"),
                        "population": {
                            species_id: numeric_delta(
                                left_phase.get("closingPopulation", {}).get(species_id, 0),
                                right_phase.get("closingPopulation", {}).get(species_id, 0),
                            )
                            for species_id in species
                        },
                    }
                    for left_phase, right_phase in zip(left.get("phases", []), right.get("phases", []))
                ],
            }
        )
    return {
        "baselineReportPath": str(baseline_path),
        "baselineParseWarnings": list(baseline_parse_warnings or []),
        "pairedSeeds": seeds,
        "unpairedReportSeeds": sorted(set(left_by_seed) - set(right_by_seed)),
        "unpairedBaselineSeeds": sorted(set(right_by_seed) - set(left_by_seed)),
        "rows": rows,
    }


def phase_averages(runs: list[dict[str, Any]]) -> list[dict[str, Any]]:
    grouped: dict[int, list[dict[str, Any]]] = defaultdict(list)
    for run in runs:
        for phase in run.get("phases", []):
            if isinstance(phase, dict) and phase.get("phaseIndex") is not None:
                grouped[int(phase["phaseIndex"])].append(phase)

    result = []
    for phase_index in sorted(grouped):
        phases = grouped[phase_index]
        windows = sorted({tuple(phase.get("window", [])) for phase in phases})
        loadouts = sorted(
            {
                ",".join(item.get("upgradeId", "") for item in phase.get("effectiveUpgradeLoadout", [])) or "none"
                for phase in phases
            }
        )
        species = sorted(
            {
                species_id
                for phase in phases
                for species_id in phase.get("closingPopulation", {})
            }
        )
        closing = {
            species_id: mean(
                [number(phase.get("closingPopulation", {}).get(species_id)) for phase in phases]
            )
            for species_id in species
        }
        activity: dict[str, dict[str, Any]] = {}
        for species_id in sorted(
            {
                species_id
                for phase in phases
                for species_id in phase.get("activity", {})
            }
        ):
            metrics = sorted(
                {
                    metric
                    for phase in phases
                    for metric in phase.get("activity", {}).get(species_id, {})
                }
            )
            activity[species_id] = {
                metric: mean(
                    [number(phase.get("activity", {}).get(species_id, {}).get(metric)) for phase in phases]
                )
                for metric in metrics
            }
        result.append(
            {
                "phaseIndex": phase_index,
                "sampleCount": len(phases),
                "windows": [list(window) for window in windows],
                "loadouts": loadouts,
                "closingPopulationMean": closing,
                "activityMean": activity,
            }
        )
    return result


def markdown(summary: dict[str, Any]) -> str:
    experiment = summary["experiment"]
    contract = summary["upgradeContract"]
    lines = [
        "# CellSim compressed artifact summary",
        "",
        f"Source: `{summary['source']['reportPath']}`  ",
        f"Summary schema: `{summary['summarySchemaVersion']}`  ",
        f"Scenario: `{experiment.get('scenarioAssetPath', '')}` · player `{experiment.get('playerSpeciesId', '')}` · seeds `{experiment.get('seedStart')}`–`{int(experiment.get('seedStart') or 0) + int(experiment.get('seedCount') or 0) - 1}`",
        f"Run: `{experiment.get('runTicks')}` ticks · `{experiment.get('phaseCount')}` phases × `{experiment.get('phaseLengthTicks')}` · combat `{experiment.get('combatResolutionMode')}` · opportunities `{experiment.get('attackOpportunityMode')}`",
        f"Ruleset `{experiment.get('rulesetFingerprint', '')}` · registry `{contract.get('upgradeRegistryFingerprint', '')}` · loadout `{contract.get('upgradeLoadoutFingerprint', '')}`",
        "",
        "## Provenance",
        "",
        f"- Source commit: `{summary['source'].get('sourceCommit', '')}`; source tree dirty before/after: `{summary['source'].get('sourceTreeDirtyBeforeRun')}` / `{summary['source'].get('sourceTreeDirtyAfterRun')}`.",
        f"- Catalog: `{contract.get('upgradeCatalogPath', '')}`; contract: `{contract.get('upgradeContractVersion', '')}`.",
        f"- Parse warnings: `{'; '.join(summary['source'].get('parseWarnings', [])) or 'none'}`.",
        "",
        "## Run outcomes",
        "",
        "| Seed | Final population | Player pop | Currency |",
        "| ---: | --- | ---: | ---: |",
    ]
    for run in summary["runs"]:
        populations = ", ".join(f"{key}={value}" for key, value in sorted(run.get("finalPopulation", {}).items()))
        lines.append(f"| {run.get('seed')} | {populations} | {run.get('playerPopulation')} | {run.get('currencyEarned')} |")

    lines.extend([
        "",
        "## Phase averages",
        "",
        "Cross-seed means keep the Markdown view compact. Exact per-seed phase windows and every source counter remain in `report-summary.json` under `runs[*].phases`.",
        "",
        "| Phase | Samples | Window(s) | Loadout(s) | Mean closing population | Mean activity counters by species |",
        "| ---: | ---: | --- | --- | --- | --- |",
    ])
    for phase in phase_averages(summary["runs"]):
        loadout = ",".join(phase["loadouts"])
        windows = ", ".join(str(window) for window in phase["windows"])
        closing = ", ".join(f"{key}={value}" for key, value in sorted(phase["closingPopulationMean"].items()))
        activity = "; ".join(
            f"{species}:" + ",".join(f"{key}={value}" for key, value in sorted(values.items()))
            for species, values in sorted(phase["activityMean"].items())
        )
        lines.append(
            f"| {phase['phaseIndex']} | {phase['sampleCount']} | {windows} | `{loadout}` | {closing} | {activity} |"
        )

    lines.extend(["", "## Stat lines", "", "All serialized herbivore stat-line fields are retained in the JSON summary.", ""])
    for run in summary["runs"]:
        statline = run.get("herbivoreStatLine")
        if statline:
            compact = ", ".join(f"{key}={value}" for key, value in sorted(statline.items()))
            lines.append(f"- Seed {run.get('seed')}: {compact}")

    if "comparison" in summary:
        lines.extend([
            "",
            "## Paired baseline deltas",
            "",
            f"Baseline report: `{summary['comparison']['baselineReportPath']}`",
            f"Baseline parse warnings: `{'; '.join(summary['comparison'].get('baselineParseWarnings', [])) or 'none'}`",
            "",
        ])
        lines.append("| Seed | Currency delta | Final population delta |")
        lines.append("| ---: | ---: | --- |")
        for row in summary["comparison"]["rows"]:
            delta = ", ".join(f"{key}={value}" for key, value in sorted(row["finalPopulationDelta"].items()))
            lines.append(f"| {row.get('seed')} | {row.get('currencyEarnedDelta')} | {delta} |")

    lines.extend([
        "",
        "## Compression contract",
        "",
        "Raw event arrays are omitted after decision-useful aggregates are retained. The source report remains the replay/evidence authority.",
        "",
        "- Retained: provenance, catalog/snapshot identity, seed-level outcomes, phase windows, populations, activity counters, stat lines, behavior totals, transitions, deaths, combat distributions, and opportunity counts.",
        "- Omitted raw arrays: `populationHistory`, `behaviorTransitions`, `deathEvents`, `combatRolls`, and `opportunityAudit`.",
        f"- Unrecognized report fields: `{summary.get('unrecognizedReportFields')}`; unrecognized run fields: `{summary.get('unrecognizedRunFields')}`.",
        "- A summary is not causal proof and must not replace a declared experiment method or human balance decision.",
    ])
    return "\n".join(lines) + "\n"


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--report", required=True, type=Path)
    parser.add_argument("--baseline", type=Path)
    parser.add_argument("--output-directory", type=Path)
    args = parser.parse_args()

    report_path = args.report.resolve()
    if not report_path.is_file():
        parser.error(f"Report does not exist: {report_path}")
    report_warnings: list[str] = []
    report = load_json(report_path, report_warnings)
    summary = core_summary(report, report_path, report_warnings)
    if args.baseline:
        baseline_path = args.baseline.resolve()
        if not baseline_path.is_file():
            parser.error(f"Baseline does not exist: {baseline_path}")
        baseline_warnings: list[str] = []
        baseline = core_summary(load_json(baseline_path, baseline_warnings), baseline_path, baseline_warnings)
        summary["comparison"] = comparison(summary, baseline, baseline_path, baseline_warnings)

    output_directory = (args.output_directory or report_path.parent).resolve()
    output_directory.mkdir(parents=True, exist_ok=True)
    json_path = output_directory / "report-summary.json"
    markdown_path = output_directory / "report-summary.md"
    with json_path.open("w", encoding="utf-8", newline="\n") as handle:
        json.dump(summary, handle, ensure_ascii=False, separators=(",", ":"))
    markdown_path.write_text(markdown(summary), encoding="utf-8", newline="\n")
    print(f"JSON: {json_path}")
    print(f"Markdown: {markdown_path}")
    print(f"Runs: {len(summary['runs'])}; phases: {sum(len(run.get('phases', [])) for run in summary['runs'])}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
