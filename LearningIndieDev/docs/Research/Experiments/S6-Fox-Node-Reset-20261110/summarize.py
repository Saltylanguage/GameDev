"""Validate node openings and summarize the frozen paired Fox sweep JSONL files."""

import hashlib
import json
import math
import statistics
import sys
from pathlib import Path


def population(phase, side, species):
    return next((item["population"] for item in phase[side][0]["species"]
                 if item["speciesId"] == species), 0)


def activity(phases, species, field):
    return sum(next((item[field] for item in phase["activity"]
                     if item["speciesId"] == species), 0) for phase in phases)


def wilson(failures, total):
    z = 1.959963984540054
    p = failures / total
    denominator = 1 + z * z / total
    centre = (p + z * z / (2 * total)) / denominator
    spread = z * math.sqrt(p * (1 - p) / total + z * z / (4 * total * total)) / denominator
    return [round(centre - spread, 4), round(centre + spread, 4)]


def panel(path, first_seed, seed_count):
    runs = {}
    with path.open(encoding="utf-8-sig") as stream:
        for line in stream:
            item = json.loads(line)
            condition, run = item["condition"], item["run"]
            seed = run["seed"]
            foxes = int(condition.removeprefix("fox-"))
            phases = run["phaseResults"]
            if condition not in ("fox-16", "fox-18", "fox-20") or len(phases) != 12 or run["ticks"] != 1200:
                raise ValueError(f"Unexpected arm, phase count, or duration: {condition} {seed}")
            if not first_seed <= seed < first_seed + seed_count or (condition, seed) in runs:
                raise ValueError(f"Unexpected or duplicate seed: {condition} {seed}")
            first, close1, open2, close2 = phases[0], phases[5], phases[6], phases[11]
            if (population(first, "openingPopulation", "hare") != 55
                    or population(first, "openingPopulation", "plant") != 400
                    or population(first, "openingPopulation", "fox") != foxes
                    or population(open2, "openingPopulation", "hare") != population(close1, "closingPopulation", "hare")
                    or population(open2, "openingPopulation", "plant") != 400
                    or population(open2, "openingPopulation", "fox") != foxes):
                raise ValueError(f"Node opening mismatch: {condition} {seed}")
            for species in ("hare", "plant", "fox"):
                delta = population(open2, "openingPopulation", species) - population(close1, "closingPopulation", species)
                resets = [action for action in run["populationInterventions"]
                          if action["source"] == "node-reset-2" and action["speciesId"] == species]
                if (delta != 0 or population(close1, "closingPopulation", species) > 0) and (
                        len(resets) != 1 or resets[0]["amount"] != delta):
                    raise ValueError(f"Node reset ledger mismatch: {condition} {seed} {species}")
            node1, node2 = phases[:6], phases[6:]
            runs[(condition, seed)] = {
                "hare600": population(close1, "closingPopulation", "hare"),
                "hare1200": population(close2, "closingPopulation", "hare"),
                "fox600": population(close1, "closingPopulation", "fox"),
                "fox1200": population(close2, "closingPopulation", "fox"),
                "plant600": population(close1, "closingPopulation", "plant"),
                "plant1200": population(close2, "closingPopulation", "plant"),
                "firstHareZeroTick": next(item["firstZeroTick"] for item in run["populationTrajectory"]
                                          if item["speciesId"] == "hare"),
                "node1": {"hareDeaths": activity(node1, "hare", "deaths"),
                          "hareStarvationDeaths": activity(node1, "hare", "starvationDeaths"),
                          "hareBirths": activity(node1, "hare", "births"),
                          "foxKills": activity(node1, "fox", "combatKills"),
                          "foxAttacks": activity(node1, "fox", "combatAttempts")},
                "node2": {"hareDeaths": activity(node2, "hare", "deaths"),
                          "hareStarvationDeaths": activity(node2, "hare", "starvationDeaths"),
                          "hareBirths": activity(node2, "hare", "births"),
                          "foxKills": activity(node2, "fox", "combatKills"),
                          "foxAttacks": activity(node2, "fox", "combatAttempts")},
            }
    if len(runs) != seed_count * 3:
        raise ValueError(f"Missing runs: {len(runs)} of {seed_count * 3}")
    by_seed = [{"seed": seed, **{arm: runs[(arm, seed)] for arm in ("fox-16", "fox-18", "fox-20")}}
               for seed in range(first_seed, first_seed + seed_count)]
    summary = {}
    for arm in ("fox-16", "fox-18", "fox-20"):
        values = [runs[(arm, seed)] for seed in range(first_seed, first_seed + seed_count)]
        first_failures = sum(value["hare600"] == 0 for value in values)
        total_failures = sum(value["hare1200"] == 0 for value in values)
        summary[arm] = {
            "runs": seed_count, "node1Failures": first_failures,
            "node1FailureWilson95": wilson(first_failures, seed_count),
            "cumulativeNode2Failures": total_failures,
            "cumulativeNode2FailureWilson95": wilson(total_failures, seed_count),
            "node2FailuresAmongSurvivors": total_failures - first_failures,
            "node1Survivors": seed_count - first_failures,
            "node2ConditionalFailureWilson95": wilson(total_failures - first_failures,
                                                       seed_count - first_failures),
            "meanHare600": round(statistics.mean(value["hare600"] for value in values), 2),
            "medianHare600": statistics.median(value["hare600"] for value in values),
            "meanHare1200": round(statistics.mean(value["hare1200"] for value in values), 2),
            "meanPlant600": round(statistics.mean(value["plant600"] for value in values), 2),
            "meanPlant1200": round(statistics.mean(value["plant1200"] for value in values), 2),
            "node1Totals": {field: sum(value["node1"][field] for value in values)
                            for field in values[0]["node1"]},
            "node2Totals": {field: sum(value["node2"][field] for value in values)
                            for field in values[0]["node2"]},
        }
    pairs = {}
    for lower, higher in (("fox-16", "fox-18"), ("fox-16", "fox-20"), ("fox-18", "fox-20")):
        pairs[f"{lower}_vs_{higher}"] = {
            "lowerFailsOnly": sum(runs[(lower, seed)]["hare1200"] == 0
                                  and runs[(higher, seed)]["hare1200"] > 0
                                  for seed in range(first_seed, first_seed + seed_count)),
            "higherFailsOnly": sum(runs[(higher, seed)]["hare1200"] == 0
                                   and runs[(lower, seed)]["hare1200"] > 0
                                   for seed in range(first_seed, first_seed + seed_count)),
        }
    return {"input": str(path), "sha256": hashlib.sha256(path.read_bytes()).hexdigest(),
            "validatedRuns": len(runs), "summary": summary, "pairedOutcomes": pairs, "seeds": by_seed}


if __name__ == "__main__":
    diagnostic, confirmation, output = map(Path, sys.argv[1:4])
    result = {"schemaVersion": 1, "experimentId": "S6-FOX-NODE-RESET-20261110",
              "diagnostic": panel(diagnostic, 10100, 12),
              "confirmation": panel(confirmation, 10200, 100)}
    output.write_text(json.dumps(result, indent=2) + "\n", encoding="utf-8")
    print(json.dumps({name: result[name]["summary"] for name in ("diagnostic", "confirmation")}, indent=2))
