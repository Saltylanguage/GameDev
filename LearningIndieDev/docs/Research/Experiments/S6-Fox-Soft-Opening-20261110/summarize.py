"""Validate fresh-node openings and summarize the lower-Fox diagnostic panel."""
import hashlib
import json
import math
import statistics
import sys
from pathlib import Path


ARMS = ("fox-08", "fox-10", "fox-12")
SPECIES = ("hare", "fox", "plant")
FIRST_SEED = 10100
SEED_COUNT = 12


def population(phase, side, species):
    return next((item["population"] for item in phase[side][0]["species"]
                 if item["speciesId"] == species), 0)


def activity(phases, species, field):
    return sum(next((item[field] for item in phase["activity"]
                     if item["speciesId"] == species), 0) for phase in phases)


def wilson(failures, total):
    z = 1.959963984540054
    p = failures / total
    d = 1 + z * z / total
    centre = (p + z * z / (2 * total)) / d
    spread = z * math.sqrt(p * (1 - p) / total + z * z / (4 * total * total)) / d
    return [round(centre - spread, 4), round(centre + spread, 4)]


def summarize(source):
    runs = {}
    with source.open(encoding="utf-8-sig") as stream:
        for line in stream:
            record = json.loads(line)
            condition, run = record["condition"], record["run"]
            seed = run["seed"]
            fox_count = int(condition.removeprefix("fox-"))
            phases = run["phaseResults"]
            key = (condition, seed)
            if condition not in ARMS or len(phases) != 12 or run["ticks"] != 1200:
                raise ValueError(f"Unexpected arm, phase count, or duration: {condition} {seed}")
            if not FIRST_SEED <= seed < FIRST_SEED + SEED_COUNT or key in runs:
                raise ValueError(f"Unexpected or duplicate run: {condition} {seed}")

            close1, open2, close2 = phases[5], phases[6], phases[11]
            if (population(phases[0], "openingPopulation", "hare") != 55
                    or population(phases[0], "openingPopulation", "fox") != fox_count
                    or population(phases[0], "openingPopulation", "plant") != 400
                    or population(open2, "openingPopulation", "hare")
                    != population(close1, "closingPopulation", "hare")
                    or population(open2, "openingPopulation", "fox") != fox_count
                    or population(open2, "openingPopulation", "plant") != 400):
                raise ValueError(f"Node opening mismatch: {condition} {seed}")

            for species in SPECIES:
                delta = (population(open2, "openingPopulation", species)
                         - population(close1, "closingPopulation", species))
                resets = [item for item in run["populationInterventions"]
                          if item["source"] == "node-reset-2" and item["speciesId"] == species]
                after = population(open2, "openingPopulation", species)
                if delta == 0 and after == 0:
                    if resets and (len(resets) != 1 or resets[0]["amount"] != 0):
                        raise ValueError(f"Unexpected empty-population reset ledger: {condition} {seed} {species}")
                elif len(resets) != 1 or resets[0]["amount"] != delta:
                    raise ValueError(f"Reset ledger mismatch: {condition} {seed} {species}")

            runs[key] = {
                "hare600": population(close1, "closingPopulation", "hare"),
                "hare1200": population(close2, "closingPopulation", "hare"),
                "fox600": population(close1, "closingPopulation", "fox"),
                "plant600": population(close1, "closingPopulation", "plant"),
                "plant1200": population(close2, "closingPopulation", "plant"),
                "node1": {field: activity(phases[:6], "hare", field)
                          for field in ("deaths", "starvationDeaths", "births")},
                "node1FoxKills": activity(phases[:6], "fox", "combatKills"),
                "node2": {field: activity(phases[6:], "hare", field)
                          for field in ("deaths", "starvationDeaths", "births")},
                "node2FoxKills": activity(phases[6:], "fox", "combatKills"),
            }

    expected = len(ARMS) * SEED_COUNT
    if len(runs) != expected:
        raise ValueError(f"Missing runs: {len(runs)} of {expected}")

    arm_summaries, by_seed = {}, []
    for arm in ARMS:
        values = [runs[(arm, seed)] for seed in range(FIRST_SEED, FIRST_SEED + SEED_COUNT)]
        first_fail = sum(item["hare600"] == 0 for item in values)
        total_fail = sum(item["hare1200"] == 0 for item in values)
        survivors = SEED_COUNT - first_fail
        arm_summaries[arm] = {
            "runs": SEED_COUNT,
            "node1Failures": first_fail,
            "node1FailureWilson95": wilson(first_fail, SEED_COUNT),
            "cumulativeNode2Failures": total_fail,
            "cumulativeNode2FailureWilson95": wilson(total_fail, SEED_COUNT),
            "node2FailuresAmongSurvivors": total_fail - first_fail,
            "node1Survivors": survivors,
            "node2ConditionalFailureWilson95": wilson(total_fail - first_fail, survivors),
            "meanHare600": round(statistics.mean(item["hare600"] for item in values), 2),
            "meanPlant600": round(statistics.mean(item["plant600"] for item in values), 2),
            "meanPlant1200": round(statistics.mean(item["plant1200"] for item in values), 2),
            "minPlant600": min(item["plant600"] for item in values),
            "minPlant1200": min(item["plant1200"] for item in values),
            "node1Totals": {
                "hareDeaths": sum(item["node1"]["deaths"] for item in values),
                "hareStarvationDeaths": sum(item["node1"]["starvationDeaths"] for item in values),
                "hareBirths": sum(item["node1"]["births"] for item in values),
                "foxKills": sum(item["node1FoxKills"] for item in values),
            },
            "node2Totals": {
                "hareDeaths": sum(item["node2"]["deaths"] for item in values),
                "hareStarvationDeaths": sum(item["node2"]["starvationDeaths"] for item in values),
                "hareBirths": sum(item["node2"]["births"] for item in values),
                "foxKills": sum(item["node2FoxKills"] for item in values),
            },
        }

    for seed in range(FIRST_SEED, FIRST_SEED + SEED_COUNT):
        by_seed.append({"seed": seed, **{arm: runs[(arm, seed)] for arm in ARMS}})
    return {
        "schemaVersion": 1,
        "experimentId": "S6-FOX-SOFT-OPENING-20261110",
        "input": str(source),
        "sha256": hashlib.sha256(source.read_bytes()).hexdigest(),
        "validatedRuns": len(runs),
        "armSummaries": arm_summaries,
        "seeds": by_seed,
    }


if __name__ == "__main__":
    result = summarize(Path(sys.argv[1]))
    output = Path(sys.argv[2])
    output.write_text(json.dumps(result, indent=2) + "\n", encoding="utf-8")
    for arm, values in result["armSummaries"].items():
        print(f"{arm}: node1 {values['node1Failures']}/12; "
              f"cumulative node2 {values['cumulativeNode2Failures']}/12; "
              f"conditional node2 {values['node2FailuresAmongSurvivors']}/"
              f"{values['node1Survivors']}")
