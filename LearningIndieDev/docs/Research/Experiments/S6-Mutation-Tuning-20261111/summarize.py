#!/usr/bin/env python3
"""Validate and summarize the S6 Mutation screen and matched confirmation."""

from __future__ import annotations

import hashlib
import json
import math
import statistics
import subprocess
from collections import defaultdict
from pathlib import Path
from typing import Any


PROJECT = Path(__file__).resolve().parents[4]
EXPERIMENT = Path(__file__).resolve().parent
ARTIFACTS = PROJECT / "artifacts" / "S6-Mutation-Tuning-20261111"
SPECIES = ("hare", "fox", "plant")
ARMS = ("control", "faster-movement", "keen-senses", "tough-hide", "reproductive-drive")


def load_runs(path: Path) -> list[dict[str, Any]]:
    return [json.loads(line) for line in path.read_text(encoding="utf-8-sig").splitlines() if line.strip()]


def sha256(path: Path) -> str:
    return hashlib.sha256(path.read_bytes()).hexdigest()


def population(snapshots: list[dict[str, Any]], species_id: str) -> int:
    if not snapshots:
        return 0
    return next((int(item["population"]) for item in snapshots[0]["species"]
                 if item["speciesId"] == species_id), 0)


def activity(phases: list[dict[str, Any]], species_id: str, key: str) -> int:
    return sum(next((int(item.get(key, 0)) for item in phase.get("activity", [])
                     if item["speciesId"] == species_id), 0) for phase in phases)


def behavior(phases: list[dict[str, Any]], species_id: str, state: str) -> int:
    return sum(int(item["ticks"]) for phase in phases for item in phase.get("behavior", [])
               if item["speciesId"] == species_id and item["state"] == state)


def wilson(successes: int, trials: int) -> list[float]:
    if trials == 0:
        return [0.0, 0.0]
    z = 1.959963984540054
    p = successes / trials
    denominator = 1 + z * z / trials
    midpoint = (p + z * z / (2 * trials)) / denominator
    half_width = z * math.sqrt(p * (1 - p) / trials + z * z / (4 * trials * trials)) / denominator
    return [round((midpoint - half_width) * 100, 1), round((midpoint + half_width) * 100, 1)]


def summarize_arm(rows: list[dict[str, Any]], condition: str, expected_count: int,
                  expected_loadouts: list[list[str]] | None = None) -> dict[str, Any]:
    outcomes = []
    for wrapped in rows:
        if wrapped["condition"] != condition:
            continue
        run = wrapped["run"]
        phases = run["phaseResults"]
        assert len(phases) == 12, (condition, run["seed"], len(phases))
        first_open = {species: population(phases[0]["openingPopulation"], species) for species in SPECIES}
        first_close = {species: population(phases[5]["closingPopulation"], species) for species in SPECIES}
        second_open = {species: population(phases[6]["openingPopulation"], species) for species in SPECIES}
        second_close = {species: population(phases[11]["closingPopulation"], species) for species in SPECIES}
        assert first_open == {"hare": 55, "fox": 12, "plant": 500}, (condition, run["seed"], first_open)
        assert second_open == {"hare": first_close["hare"], "fox": 48, "plant": 500}, (
            condition, run["seed"], first_close, second_open)
        for phase in phases:
            ids = [item["upgradeId"] for item in phase.get("effectiveUpgradeLoadout", [])]
            expected_ids = (expected_loadouts[phase["phaseIndex"] - 1] if expected_loadouts is not None
                            else [] if condition == "control" or phase["phaseIndex"] == 1 else [condition])
            assert ids == expected_ids, (condition, run["seed"], phase["phaseIndex"], ids)
        spatial_windows = [window for window in (run.get("spatial") or {}).get("phaseWindows", [])
                           if window["phaseIndex"] >= 2]
        checkpoint_plants = [
            population(phase[field], "plant")
            for phase in phases
            for field in ("openingPopulation", "closingPopulation")
        ]
        post_mutation = phases[1:]
        outcomes.append({
            "seed": int(run["seed"]),
            "node1CloseHares": first_close["hare"],
            "node2CloseHares": second_close["hare"],
            "node1Failure": first_close["hare"] == 0,
            "cumulativeNode2Failure": second_close["hare"] == 0,
            "checkpointPlantMinimum": min(checkpoint_plants),
            "node1FoxKills": activity(phases[:6], "fox", "combatKills"),
            "node2FoxKills": activity(phases[6:], "fox", "combatKills"),
            "node1HareDeaths": activity(phases[:6], "hare", "deaths"),
            "node2HareDeaths": activity(phases[6:], "hare", "deaths"),
            "node1HareStarvationDeaths": activity(phases[:6], "hare", "starvationDeaths"),
            "node2HareStarvationDeaths": activity(phases[6:], "hare", "starvationDeaths"),
            "postAcquisitionHareBirths": activity(post_mutation, "hare", "births"),
            "postAcquisitionReproductionEligible": activity(post_mutation, "hare", "reproductionEligibleAttempts"),
            "postAcquisitionReproductionSuccessful": activity(post_mutation, "hare", "reproductionSuccessfulAttempts"),
            "postAcquisitionHareMovementSteps": activity(post_mutation, "hare", "movementSteps"),
            "postAcquisitionHareThreatenedTicks": behavior(post_mutation, "hare", "Threatened"),
            "postAcquisitionFoxAdjacentObservations": sum(int(window["foxAdjacentObservations"]) for window in spatial_windows),
            "postAcquisitionHareObservations": sum(int(window["hareObservations"]) for window in spatial_windows),
            "foxSuccessfulHits": activity(phases, "fox", "combatHits"),
            "foxNonlethalHits": activity(phases, "fox", "combatNonLethalHits"),
            "foxLethalHits": activity(phases, "fox", "combatLethalHits"),
            "foxAttackAttempts": activity(phases, "fox", "combatAttempts"),
        })
    assert len(outcomes) == expected_count, (condition, len(outcomes), expected_count)
    outcomes.sort(key=lambda item: item["seed"])
    seeds = [item["seed"] for item in outcomes]
    assert len(set(seeds)) == expected_count, (condition, "duplicate seeds")
    n1 = sum(item["node1Failure"] for item in outcomes)
    n2 = sum(item["cumulativeNode2Failure"] for item in outcomes)
    n1_survivors = [item for item in outcomes if not item["node1Failure"]]
    n2_after_n1 = sum(item["cumulativeNode2Failure"] for item in n1_survivors)

    def total(key: str) -> int:
        return sum(item[key] for item in outcomes)

    def mean_and_range(key: str) -> dict[str, Any]:
        values = [item[key] for item in outcomes]
        return {"mean": round(statistics.fmean(values), 2), "min": min(values), "max": max(values)}

    return {
        "runs": len(outcomes),
        "seeds": [seeds[0], seeds[-1]],
        "node1Failures": {"count": n1, "ratePercent": round(n1 / len(outcomes) * 100, 1), "wilson95Percent": wilson(n1, len(outcomes))},
        "cumulativeNode2Failures": {"count": n2, "ratePercent": round(n2 / len(outcomes) * 100, 1), "wilson95Percent": wilson(n2, len(outcomes))},
        "node2FailuresAmongNode1Survivors": {"count": n2_after_n1, "trials": len(n1_survivors), "wilson95Percent": wilson(n2_after_n1, len(n1_survivors))},
        "node1CloseHares": mean_and_range("node1CloseHares"),
        "node2CloseHares": mean_and_range("node2CloseHares"),
        "minimumObservedPlantCheckpoint": min(item["checkpointPlantMinimum"] for item in outcomes),
        "seedsWithZeroPlantCheckpoint": sum(item["checkpointPlantMinimum"] == 0 for item in outcomes),
        "node1FoxKills": total("node1FoxKills"),
        "node2FoxKills": total("node2FoxKills"),
        "node1HareDeaths": total("node1HareDeaths"),
        "node2HareDeaths": total("node2HareDeaths"),
        "node1HareStarvationDeaths": total("node1HareStarvationDeaths"),
        "node2HareStarvationDeaths": total("node2HareStarvationDeaths"),
        "postAcquisitionHareBirths": total("postAcquisitionHareBirths"),
        "postAcquisitionReproductionEligible": total("postAcquisitionReproductionEligible"),
        "postAcquisitionReproductionSuccessful": total("postAcquisitionReproductionSuccessful"),
        "postAcquisitionHareMovementSteps": total("postAcquisitionHareMovementSteps"),
        "postAcquisitionHareThreatenedTicks": total("postAcquisitionHareThreatenedTicks"),
        "postAcquisitionFoxAdjacentObservations": total("postAcquisitionFoxAdjacentObservations"),
        "postAcquisitionHareObservations": total("postAcquisitionHareObservations"),
        "postAcquisitionFoxAdjacentPer1000HareObservations": round(
            1000 * total("postAcquisitionFoxAdjacentObservations")
            / max(1, total("postAcquisitionHareObservations")), 2),
        "foxSuccessfulHits": total("foxSuccessfulHits"),
        "foxNonlethalHits": total("foxNonlethalHits"),
        "foxLethalHits": total("foxLethalHits"),
        "foxAttackAttempts": total("foxAttackAttempts"),
        "perSeed": outcomes,
    }


def paired(candidate: dict[str, Any], control: dict[str, Any]) -> dict[str, int]:
    left = {item["seed"]: item for item in candidate["perSeed"]}
    right = {item["seed"]: item for item in control["perSeed"]}
    assert left.keys() == right.keys()
    rescued = sum(right[seed]["cumulativeNode2Failure"] and not item["cumulativeNode2Failure"]
                  for seed, item in left.items())
    regressed = sum(not right[seed]["cumulativeNode2Failure"] and item["cumulativeNode2Failure"]
                    for seed, item in left.items())
    return {"pairedSeeds": len(left), "rescuedControlFailures": rescued,
            "newCandidateFailures": regressed, "netFewerFailures": rescued - regressed}


def report_for_arm(run_set: str, arm: str, rows: list[dict[str, Any]], snapshot: dict[str, Any], commit: str,
                   source_hash: str, batch: dict[str, Any], plan: dict[str, Any], metric: dict[str, Any]) -> None:
    arm_rows = [item["run"] for item in rows if item["condition"] == arm]
    arguments = next(item["arguments"] for item in plan["conditions"] if item["id"] == arm)
    args = dict(zip(arguments[::2], arguments[1::2]))
    schedule = args["-phaseUpgradeSchedule"].split(";")
    upgrade = arm_rows[0]["phaseResults"][-1].get("effectiveUpgradeLoadout", [])
    report = {
        "schemaVersion": 1,
        "metricDictionaryId": metric.get("dictionaryId", ""),
        "metricDictionaryVersion": metric.get("version", 0),
        "createdUtc": "2026-11-11",
        "scenarioAssetPath": snapshot.get("scenarioPath", ""),
        "rulesetFingerprint": arm_rows[0]["phaseResults"][0].get("rulesetFingerprint", ""),
        "playerSpeciesId": "hare",
        "combatResolutionMode": "opposed-roll",
        "attackOpportunityMode": "natural",
        "experimentalFeatures": "bev-experimental",
        "seedStart": plan["seedStart"],
        "seedCount": plan["seedCount"],
        "gridWidth": snapshot["width"],
        "gridHeight": snapshot["height"],
        "runTicks": 1200,
        "phaseLengthTicks": 100,
        "phaseCount": 12,
        "phaseUpgradeSchedule": schedule,
        "researchSpeciesStats": args.get("-speciesStats", ""),
        "runDurationSeconds": snapshot.get("runDurationSeconds"),
        "stepIntervalSeconds": snapshot.get("stepInterval"),
        "upgradeId": ",".join(item.get("upgradeId", "") for item in upgrade) or "none",
        "orderedLoadout": [item.get("upgradeId") for item in upgrade],
        "upgradeContractVersion": upgrade[0].get("contractVersion", "") if upgrade else "",
        "upgradeCatalogPath": snapshot.get("catalogPath", ""),
        "upgradeRegistryFingerprint": upgrade[0].get("registryFingerprint", "") if upgrade else "",
        "upgradeLoadoutFingerprint": upgrade[0].get("fingerprint", "") if upgrade else "",
        "runs": arm_rows,
        "portableCohort": {
            "sourceCommit": commit,
            "snapshotSourceCommit": snapshot.get("sourceCommit", ""),
            "sourceHash": source_hash,
            "batchIdentity": batch["identity"],
            "workerAssemblyHash": batch["assemblyHash"],
            "snapshotFingerprint": snapshot.get("fingerprint", ""),
            "detail": "CellSim.Batch compact runs retain phase activity and behavior totals; detailed transition, death-event, and combat-roll arrays were not captured.",
        },
    }
    directory = ARTIFACTS / run_set / "condition-reports" / arm
    directory.mkdir(parents=True, exist_ok=True)
    (directory / "report.json").write_text(json.dumps(report, separators=(",", ":")), encoding="utf-8")
    manifest = {
        "sourceCommit": commit,
        "sourceTreeDirtyBeforeRun": True,
        "sourceTreeDirtyAfterRun": True,
        "metricDictionaryFile": "docs/Research/METRIC_DICTIONARY_V1.json",
        "metricDictionarySha256": sha256(PROJECT / "docs" / "Research" / "METRIC_DICTIONARY_V1.json"),
    }
    (directory / "manifest.json").write_text(json.dumps(manifest, indent=2), encoding="utf-8")


def main() -> None:
    screen_path = ARTIFACTS / "mutation-screen-12" / "runs.jsonl"
    confirm_path = ARTIFACTS / "mutation-confirmation-100" / "runs.jsonl"
    screen_rows, confirm_rows = load_runs(screen_path), load_runs(confirm_path)
    screen = {arm: summarize_arm(screen_rows, arm, 12) for arm in ARMS}
    confirm_arms = ("control", "faster-movement", "tough-hide", "reproductive-drive")
    confirmed = {arm: summarize_arm(confirm_rows, arm, 100) for arm in confirm_arms}
    paired_results = {arm: paired(confirmed[arm], confirmed["control"])
                      for arm in confirm_arms if arm != "control"}
    snapshot_path = ARTIFACTS / "forest-edge-current-worker.json"
    snapshot = json.loads(snapshot_path.read_text(encoding="utf-8-sig"))
    migration = json.loads((ARTIFACTS / "snapshot-migration.json").read_text(encoding="utf-8-sig"))
    screen_batch = json.loads((ARTIFACTS / "mutation-screen-12" / "batch.json").read_text(encoding="utf-8-sig"))
    confirm_batch = json.loads((ARTIFACTS / "mutation-confirmation-100" / "batch.json").read_text(encoding="utf-8-sig"))
    screen_plan = json.loads((ARTIFACTS / "mutation-screen-12-plan.json").read_text(encoding="utf-8-sig"))
    confirm_plan = json.loads((ARTIFACTS / "mutation-confirmation-100-plan.json").read_text(encoding="utf-8-sig"))
    metric = json.loads((PROJECT / "docs" / "Research" / "METRIC_DICTIONARY_V1.json").read_text(encoding="utf-8-sig"))
    source = subprocess.run(["git", "-C", str(PROJECT.parent), "rev-parse", "HEAD"],
                            check=True, capture_output=True, text=True).stdout.strip()

    avoidance_path = ARTIFACTS / "avoidance-followup-screen-12"
    avoidance_rows = load_runs(avoidance_path / "runs.jsonl")
    avoidance_plan = json.loads((ARTIFACTS / "avoidance-followup-screen-12-plan.json").read_text(encoding="utf-8-sig"))
    avoidance_batch = json.loads((avoidance_path / "batch.json").read_text(encoding="utf-8-sig"))
    path_loadout = [[], ["faster-movement"]] + [["faster-movement", "keen-senses"]] * 10
    avoidance = {
        "control": summarize_arm(avoidance_rows, "control", 12),
        "threat-exposure": summarize_arm(avoidance_rows, "threat-exposure", 12),
        "avoidance-path": summarize_arm(avoidance_rows, "avoidance-path", 12, path_loadout),
    }
    repro20_path = ARTIFACTS / "reproductive-drive-20pp-screen-12"
    repro20_rows = load_runs(repro20_path / "runs.jsonl")
    repro20_plan = json.loads((ARTIFACTS / "reproductive-drive-20pp-screen-12-plan.json").read_text(encoding="utf-8-sig"))
    repro20_batch = json.loads((repro20_path / "batch.json").read_text(encoding="utf-8-sig"))
    repro20_snapshot_path = ARTIFACTS / "forest-edge-reproductive-drive-20pp-worker.json"
    repro20_snapshot = json.loads(repro20_snapshot_path.read_text(encoding="utf-8-sig"))
    repro20_migration = json.loads((ARTIFACTS / "snapshot-migration-reproductive-drive-20pp.json").read_text(encoding="utf-8-sig"))
    repro20 = {arm: summarize_arm(repro20_rows, arm, 12) for arm in ("control", "reproductive-drive")}
    candidate_path = ARTIFACTS / "avoidance-reproduction-screen-12"
    candidate_rows = load_runs(candidate_path / "runs.jsonl")
    candidate_plan_path = ARTIFACTS / "avoidance-reproduction-screen-12-plan.json"
    candidate_plan = json.loads(candidate_plan_path.read_text(encoding="utf-8-sig"))
    candidate_batch = json.loads((candidate_path / "batch.json").read_text(encoding="utf-8-sig"))
    candidate_snapshot_path = ARTIFACTS / "forest-edge-quick-escape-worker.json"
    candidate_snapshot = json.loads(candidate_snapshot_path.read_text(encoding="utf-8-sig"))
    candidates = {arm: summarize_arm(candidate_rows, arm, 12)
                  for arm in ("control", "quick-escape", "larger-maximum-litter")}
    rankpath_path = ARTIFACTS / "avoidance-reproduction-rankpath-screen-12"
    rankpath_rows = load_runs(rankpath_path / "runs.jsonl")
    rankpath_plan_path = ARTIFACTS / "avoidance-reproduction-rankpath-screen-12-plan.json"
    rankpath_plan = json.loads(rankpath_plan_path.read_text(encoding="utf-8-sig"))
    rankpath_batch = json.loads((rankpath_path / "batch.json").read_text(encoding="utf-8-sig"))
    quick_escape_ranks = [[], ["quick-escape"]] + [["quick-escape", "quick-escape"]] * 10
    larger_minimum_litter_ranks = [[], ["larger-minimum-litter"]] + [["larger-minimum-litter", "larger-minimum-litter"]] * 10
    rankpaths = {
        "control": summarize_arm(rankpath_rows, "control", 12),
        "quick-escape-rankpath": summarize_arm(
            rankpath_rows, "quick-escape-rankpath", 12, quick_escape_ranks),
        "larger-minimum-litter-rankpath": summarize_arm(
            rankpath_rows, "larger-minimum-litter-rankpath", 12, larger_minimum_litter_ranks),
    }
    gestation_path = ARTIFACTS / "faster-gestation-probe-36-screen-12"
    gestation_rows = load_runs(gestation_path / "runs.jsonl")
    gestation_plan_path = ARTIFACTS / "faster-gestation-probe-36-screen-12-plan.json"
    gestation_plan = json.loads(gestation_plan_path.read_text(encoding="utf-8-sig"))
    gestation_batch = json.loads((gestation_path / "batch.json").read_text(encoding="utf-8-sig"))
    no_upgrade_loadouts = [[]] * 12
    gestation = {
        "control": summarize_arm(gestation_rows, "control", 12),
        "faster-gestation-probe-36": summarize_arm(
            gestation_rows, "faster-gestation-probe-36", 12, no_upgrade_loadouts),
    }
    rank3_path = ARTIFACTS / "avoidance-reproduction-rank3-screen-12"
    rank3_rows = load_runs(rank3_path / "runs.jsonl")
    rank3_plan_path = ARTIFACTS / "avoidance-reproduction-rank3-screen-12-plan.json"
    rank3_plan = json.loads(rank3_plan_path.read_text(encoding="utf-8-sig"))
    rank3_batch = json.loads((rank3_path / "batch.json").read_text(encoding="utf-8-sig"))
    quick_escape_rank3 = [[], ["quick-escape"], ["quick-escape", "quick-escape"]]
    quick_escape_rank3 += [["quick-escape"] * 3] * 9
    litter_rank3 = [[], ["larger-minimum-litter"], ["larger-minimum-litter"] * 2]
    litter_rank3 += [["larger-minimum-litter"] * 3] * 9
    rank3 = {
        "control": summarize_arm(rank3_rows, "control", 12),
        "quick-escape-rank3-path": summarize_arm(
            rank3_rows, "quick-escape-rank3-path", 12, quick_escape_rank3),
        "larger-minimum-litter-rank3-path": summarize_arm(
            rank3_rows, "larger-minimum-litter-rank3-path", 12, litter_rank3),
    }
    confirm2_path = ARTIFACTS / "avoidance-reproduction-confirmation-100"
    confirm2_rows = load_runs(confirm2_path / "runs.jsonl")
    confirm2_plan_path = ARTIFACTS / "avoidance-reproduction-confirmation-100-plan.json"
    confirm2_plan = json.loads(confirm2_plan_path.read_text(encoding="utf-8-sig"))
    confirm2_batch = json.loads((confirm2_path / "batch.json").read_text(encoding="utf-8-sig"))
    confirm2 = {
        "control": summarize_arm(confirm2_rows, "control", 100),
        "quick-escape-rankpath": summarize_arm(confirm2_rows, "quick-escape-rankpath", 100, quick_escape_ranks),
        "larger-minimum-litter-rankpath": summarize_arm(
            confirm2_rows, "larger-minimum-litter-rankpath", 100, larger_minimum_litter_ranks),
    }
    paired_confirm2 = {
        arm: paired(confirm2[arm], confirm2["control"])
        for arm in ("quick-escape-rankpath", "larger-minimum-litter-rankpath")
    }

    threat_path = ARTIFACTS / "threat-exposure-dose-screen-12"
    threat_rows = load_runs(threat_path / "runs.jsonl")
    threat_plan_path = ARTIFACTS / "threat-exposure-dose-screen-12-plan.json"
    threat_plan = json.loads(threat_plan_path.read_text(encoding="utf-8-sig"))
    threat_batch = json.loads((threat_path / "batch.json").read_text(encoding="utf-8-sig"))
    threat_dose_loadout = lambda cap: [
        ["threat-exposure"] * min(cap, max(0, phase - 1)) for phase in range(1, 13)
    ]
    threat_dose = {
        "control": summarize_arm(threat_rows, "control", 12),
        "rank2": summarize_arm(threat_rows, "threat-exposure-rank2", 12, threat_dose_loadout(2)),
        "rank3": summarize_arm(threat_rows, "threat-exposure-rank3", 12, threat_dose_loadout(3)),
    }
    threat_high_path = ARTIFACTS / "threat-exposure-high-dose-screen-12"
    threat_high_rows = load_runs(threat_high_path / "runs.jsonl")
    threat_high_plan_path = ARTIFACTS / "threat-exposure-high-dose-screen-12-plan.json"
    threat_high_plan = json.loads(threat_high_plan_path.read_text(encoding="utf-8-sig"))
    threat_high_batch = json.loads((threat_high_path / "batch.json").read_text(encoding="utf-8-sig"))
    threat_high_dose = {
        "control": summarize_arm(threat_high_rows, "control", 12),
        "rank4": summarize_arm(threat_high_rows, "threat-exposure-rank4", 12, threat_dose_loadout(4)),
        "rank5": summarize_arm(threat_high_rows, "threat-exposure-rank5", 12, threat_dose_loadout(5)),
    }
    threat_confirm_path = ARTIFACTS / "threat-exposure-rank3-confirmation-100"
    threat_confirm_rows = load_runs(threat_confirm_path / "runs.jsonl")
    threat_confirm_plan_path = ARTIFACTS / "threat-exposure-rank3-confirmation-100-plan.json"
    threat_confirm_plan = json.loads(threat_confirm_plan_path.read_text(encoding="utf-8-sig"))
    threat_confirm_batch = json.loads((threat_confirm_path / "batch.json").read_text(encoding="utf-8-sig"))
    threat_confirmation = {
        "control": summarize_arm(threat_confirm_rows, "control", 100),
        "rank3-path": summarize_arm(
            threat_confirm_rows, "threat-exposure-rank3-path", 100, threat_dose_loadout(3)),
    }
    gestation_confirm_path = ARTIFACTS / "gestation36-upper-bound-diagnostic-100"
    gestation_confirm_rows = load_runs(gestation_confirm_path / "runs.jsonl")
    gestation_confirm_plan_path = ARTIFACTS / "gestation36-upper-bound-diagnostic-100-plan.json"
    gestation_confirm_plan = json.loads(gestation_confirm_plan_path.read_text(encoding="utf-8-sig"))
    gestation_confirm_batch = json.loads((gestation_confirm_path / "batch.json").read_text(encoding="utf-8-sig"))
    gestation_confirm = {
        "control": summarize_arm(gestation_confirm_rows, "control", 100, no_upgrade_loadouts),
        "gestation36-from-start": summarize_arm(
            gestation_confirm_rows, "gestation36-diagnostic", 100, no_upgrade_loadouts),
    }
    paired_threat_dose = {
        arm: paired(threat_dose[arm], threat_dose["control"])
        for arm in ("rank2", "rank3")
    }
    paired_threat_high_dose = {
        arm: paired(threat_high_dose[arm], threat_high_dose["control"])
        for arm in ("rank4", "rank5")
    }
    paired_threat_confirm = paired(threat_confirmation["rank3-path"], threat_confirmation["control"])
    paired_gestation_confirm = paired(gestation_confirm["gestation36-from-start"], gestation_confirm["control"])

    for arm in ARMS:
        report_for_arm("mutation-screen-12", arm, screen_rows, snapshot, source, migration["workerSourceHash"],
                       screen_batch, screen_plan, metric)
    for arm in confirm_arms:
        report_for_arm("mutation-confirmation-100", arm, confirm_rows, snapshot, source, migration["workerSourceHash"],
                       confirm_batch, confirm_plan, metric)
    for arm in avoidance:
        report_for_arm("avoidance-followup-screen-12", arm, avoidance_rows, snapshot, source,
                       migration["workerSourceHash"], avoidance_batch, avoidance_plan, metric)
    for arm in repro20:
        report_for_arm("reproductive-drive-20pp-screen-12", arm, repro20_rows, repro20_snapshot, source,
                       repro20_migration["workerSourceHash"], repro20_batch, repro20_plan, metric)
    for arm in candidates:
        report_for_arm("avoidance-reproduction-screen-12", arm, candidate_rows, candidate_snapshot, source,
                       candidate_snapshot["sourceHash"], candidate_batch, candidate_plan, metric)
    for arm in rankpaths:
        report_for_arm("avoidance-reproduction-rankpath-screen-12", arm, rankpath_rows, candidate_snapshot, source,
                       candidate_snapshot["sourceHash"], rankpath_batch, rankpath_plan, metric)
    for arm in gestation:
        report_for_arm("faster-gestation-probe-36-screen-12", arm, gestation_rows, candidate_snapshot, source,
                       candidate_snapshot["sourceHash"], gestation_batch, gestation_plan, metric)
    for arm in rank3:
        report_for_arm("avoidance-reproduction-rank3-screen-12", arm, rank3_rows, candidate_snapshot, source,
                       candidate_snapshot["sourceHash"], rank3_batch, rank3_plan, metric)
    for arm in confirm2:
        report_for_arm("avoidance-reproduction-confirmation-100", arm, confirm2_rows, candidate_snapshot, source,
                       candidate_snapshot["sourceHash"], confirm2_batch, confirm2_plan, metric)
    for arm in ("control", "threat-exposure-rank2", "threat-exposure-rank3"):
        report_for_arm("threat-exposure-dose-screen-12", arm, threat_rows, candidate_snapshot, source,
                       candidate_snapshot["sourceHash"], threat_batch, threat_plan, metric)
    for arm in ("control", "threat-exposure-rank4", "threat-exposure-rank5"):
        report_for_arm("threat-exposure-high-dose-screen-12", arm, threat_high_rows, candidate_snapshot, source,
                       candidate_snapshot["sourceHash"], threat_high_batch, threat_high_plan, metric)
    for arm in ("control", "threat-exposure-rank3-path"):
        report_for_arm("threat-exposure-rank3-confirmation-100", arm, threat_confirm_rows,
                       candidate_snapshot, source, candidate_snapshot["sourceHash"],
                       threat_confirm_batch, threat_confirm_plan, metric)
    for arm in ("control", "gestation36-diagnostic"):
        report_for_arm("gestation36-upper-bound-diagnostic-100", arm, gestation_confirm_rows,
                       candidate_snapshot, source, candidate_snapshot["sourceHash"],
                       gestation_confirm_batch, gestation_confirm_plan, metric)

    result = {
        "schemaVersion": 1,
        "experimentId": "S6-PF-MUTATION-TUNING-20261111",
        "provenance": {
            "workingCommit": source,
            "workingTreeDirty": True,
            "snapshotSourceCommit": snapshot.get("sourceCommit", ""),
            "snapshotPath": str(snapshot_path),
            "snapshotSha256AfterBinding": sha256(snapshot_path),
            "workerSourceHash": migration["workerSourceHash"],
            "dataFingerprint": snapshot.get("fingerprint", ""),
            "screenBatchIdentity": screen_batch["identity"],
            "screenPlanSha256": sha256(ARTIFACTS / "mutation-screen-12-plan.json"),
            "screenRunsSha256": sha256(screen_path),
            "screenWorkerAssemblyHash": screen_batch["assemblyHash"],
            "confirmationBatchIdentity": confirm_batch["identity"],
            "confirmationPlanSha256": sha256(ARTIFACTS / "mutation-confirmation-100-plan.json"),
            "confirmationRunsSha256": sha256(confirm_path),
            "confirmationWorkerAssemblyHash": confirm_batch["assemblyHash"],
            "avoidanceFollowupBatchIdentity": avoidance_batch["identity"],
            "avoidanceFollowupRunsSha256": sha256(avoidance_path / "runs.jsonl"),
            "reproductive20ppBatchIdentity": repro20_batch["identity"],
            "reproductive20ppWorkerSourceHash": repro20_migration["workerSourceHash"],
            "reproductive20ppRunsSha256": sha256(repro20_path / "runs.jsonl"),
            "avoidanceReproductionBatchIdentity": candidate_batch["identity"],
            "avoidanceReproductionPlanSha256": sha256(candidate_plan_path),
            "avoidanceReproductionRunsSha256": sha256(candidate_path / "runs.jsonl"),
            "avoidanceReproductionWorkerSourceHash": candidate_snapshot["sourceHash"],
            "avoidanceReproductionWorkerAssemblyHash": candidate_batch["assemblyHash"],
            "avoidanceReproductionSnapshotSha256": sha256(candidate_snapshot_path),
            "avoidanceReproductionRankpathBatchIdentity": rankpath_batch["identity"],
            "avoidanceReproductionRankpathPlanSha256": sha256(rankpath_plan_path),
            "avoidanceReproductionRankpathRunsSha256": sha256(rankpath_path / "runs.jsonl"),
            "avoidanceReproductionRankpathWorkerAssemblyHash": rankpath_batch["assemblyHash"],
            "fasterGestationProbeBatchIdentity": gestation_batch["identity"],
            "fasterGestationProbePlanSha256": sha256(gestation_plan_path),
            "fasterGestationProbeRunsSha256": sha256(gestation_path / "runs.jsonl"),
            "fasterGestationProbeWorkerAssemblyHash": gestation_batch["assemblyHash"],
            "avoidanceReproductionRank3BatchIdentity": rank3_batch["identity"],
            "avoidanceReproductionRank3PlanSha256": sha256(rank3_plan_path),
            "avoidanceReproductionRank3RunsSha256": sha256(rank3_path / "runs.jsonl"),
            "avoidanceReproductionRank3WorkerAssemblyHash": rank3_batch["assemblyHash"],
            "avoidanceReproductionConfirmationBatchIdentity": confirm2_batch["identity"],
            "avoidanceReproductionConfirmationPlanSha256": sha256(confirm2_plan_path),
            "avoidanceReproductionConfirmationRunsSha256": sha256(confirm2_path / "runs.jsonl"),
            "avoidanceReproductionConfirmationWorkerAssemblyHash": confirm2_batch["assemblyHash"],
            "threatExposureDoseBatchIdentity": threat_batch["identity"],
            "threatExposureDosePlanSha256": sha256(threat_plan_path),
            "threatExposureDoseRunsSha256": sha256(threat_path / "runs.jsonl"),
            "threatExposureDoseWorkerAssemblyHash": threat_batch["assemblyHash"],
            "threatExposureHighDoseBatchIdentity": threat_high_batch["identity"],
            "threatExposureHighDosePlanSha256": sha256(threat_high_plan_path),
            "threatExposureHighDoseRunsSha256": sha256(threat_high_path / "runs.jsonl"),
            "threatExposureHighDoseWorkerAssemblyHash": threat_high_batch["assemblyHash"],
            "threatExposureRank3ConfirmBatchIdentity": threat_confirm_batch["identity"],
            "threatExposureRank3ConfirmPlanSha256": sha256(threat_confirm_plan_path),
            "threatExposureRank3ConfirmRunsSha256": sha256(threat_confirm_path / "runs.jsonl"),
            "threatExposureRank3ConfirmWorkerAssemblyHash": threat_confirm_batch["assemblyHash"],
            "gestation36Diagnostic100BatchIdentity": gestation_confirm_batch["identity"],
            "gestation36Diagnostic100PlanSha256": sha256(gestation_confirm_plan_path),
            "gestation36Diagnostic100RunsSha256": sha256(gestation_confirm_path / "runs.jsonl"),
            "gestation36Diagnostic100WorkerAssemblyHash": gestation_confirm_batch["assemblyHash"],
            "snapshotMigration": "Rebound the previously Unity-exported Forest Edge portable data to the current worker source hash; the authored Forest Edge asset blob is unchanged.",
        },
        "scenario": {
            "playerSpeciesId": "hare", "openingHares": 55, "node1Foxes": 12,
            "node2Foxes": 48, "plantsPerNode": 500, "phaseLengthTicks": 100,
            "phasesPerNode": 6, "nodes": 2, "combat": "opposed-roll",
            "experimentalFeatures": "bev-experimental", "noPurchases": True,
            "acquisition": "one rank begins after phase 1 and remains active through phase 12",
        },
        "screen12": {"seedRange": [10100, 10111], "arms": screen},
        "confirmation100": {"seedRange": [20200, 20299], "arms": confirmed, "pairedVsControl": paired_results},
        "avoidanceFollowup12": {"seedRange": [10100, 10111], "arms": avoidance},
        "reproductiveDrive20ppScreen12": {"seedRange": [10100, 10111], "arms": repro20},
        "avoidanceReproductionScreen12": {"seedRange": [10100, 10111], "arms": candidates},
        "avoidanceReproductionRankpathScreen12": {"seedRange": [10100, 10111], "arms": rankpaths},
        "fasterGestationProbe36Screen12": {
            "seedRange": [10100, 10111],
            "gestationTicks": 36,
            "appliedFrom": "run start; diagnostic stat override, not a Mutation",
            "arms": gestation,
        },
        "avoidanceReproductionRank3Screen12": {"seedRange": [10100, 10111], "arms": rank3},
        "avoidanceReproductionConfirmation100": {
            "seedRange": [20200, 20299],
            "arms": confirm2,
            "pairedVsControl": paired_confirm2,
        },
        "threatExposureDoseScreen12": {
            "seedRange": [10100, 10111],
            "arms": threat_dose,
            "pairedVsControl": paired_threat_dose,
        },
        "threatExposureHighDoseScreen12": {
            "seedRange": [10100, 10111],
            "arms": threat_high_dose,
            "pairedVsControl": paired_threat_high_dose,
        },
        "threatExposureRank3Confirmation100": {
            "seedRange": [20200, 20299],
            "arms": threat_confirmation,
            "pairedVsControl": paired_threat_confirm,
        },
        "gestation36UpperBoundDiagnostic100": {
            "seedRange": [20200, 20299],
            "gestationTicks": 36,
            "appliedFrom": "run start; diagnostic stat override, not a Mutation",
            "arms": gestation_confirm,
            "pairedVsControl": paired_gestation_confirm,
        },
        "limitations": [
            "The 12-seed screen is a diagnostic; the 100-seed panel is the confirmation evidence.",
            "Plant minima are measured at phase checkpoints, not every tick.",
            "Faster Movement and Keen Senses have no direct detection counter; movement, Threatened-state ticks, fox proximity observations, attacks, and outcomes are mechanism proxies.",
            "Threat Exposure is measured through total Fox combat attempts and hits; compact reports do not record a standalone pre-contact-avoidance event count.",
            "The Threat Exposure rank-3 path is a three-choice, phase-by-phase path; it does not establish whether a one-choice avoidance Mutation can meet the same target.",
            "The 36-tick gestation confirmation is an upper-bound diagnostic applied from run start, not a phase-acquired Mutation.",
            "The portable snapshot was not freshly exported from Unity for this run; its stored data fingerprint was verified and the current worker-source hash was rebound.",
        ],
    }
    output = EXPERIMENT / "paired-summary.json"
    output.write_text(json.dumps(result, indent=2, ensure_ascii=False) + "\n", encoding="utf-8")
    print(f"Wrote {output}")
    print(f"Validated {len(screen_rows)} screen runs and {len(confirm_rows)} confirmation runs.")
    for arm, values in confirmed.items():
        failures = values["cumulativeNode2Failures"]
        print(f"{arm}: N1 {values['node1Failures']['count']}/100; N2 {failures['count']}/100 ({failures['ratePercent']}%, Wilson 95% {failures['wilson95Percent']}); min checkpoint plants {values['minimumObservedPlantCheckpoint']}")


if __name__ == "__main__":
    main()
