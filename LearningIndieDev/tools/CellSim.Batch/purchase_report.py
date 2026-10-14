"""Write purchase-specific tables and reached/unreached window diagnostics after analysis."""
import csv
import json
import sys
from pathlib import Path
from sweep import completed, digest

root = Path(sys.argv[1]).resolve()
_, matrix, manifest, raw = completed(root / "sweep")
analysis = root / "analysis"
destination = root / "purchase-analysis"
destination.mkdir(exist_ok=True)
if any(destination.iterdir()): raise ValueError("Use an empty purchase analysis output directory")
def rows(name):
    with (analysis / name).open(newline="",encoding="utf-8-sig") as stream:
        return list(csv.DictReader(stream))
metrics = {(r["condition"],r["metric"]):r for r in rows("metrics.csv") if r["phase"] == "0"}
pairs = {(r["condition"],r["metric"],r["scope"]):r for r in rows("paired-deltas.csv") if r["phase"] == "0"}
by_case = {c["id"]:c for c in matrix["cases"]}
fields = {"hareSurvival":"outcome.playerAliveAtHorizon", "allSpeciesSurvival":"outcome.allSpeciesAliveAtHorizon",
    "meanAPS":"herbivore.hare.APS", "meanAHS":"predator.fox.AHS", "added":"purchase.added",
    "spent":"purchase.spent", "earned":"purchase.earned", "balance":"purchase.balance",
    "windowsReached":"purchase.windowsReached"}
records = []
for case in matrix["cases"]:
    record = dict(condition=case["id"],candidate=case["axes"]["candidate"],path=case["path"],purchasePolicy=case["axes"]["purchase"])
    for label, metric in fields.items():
        record[label] = metrics[(case["id"],metric)]
        for scope, suffix in (("axis:purchase","VsNoPurchase"),("same-context-path","VsSkipAllSamePurchase")):
            record[label+suffix] = pairs.get((case["id"],metric,scope))
            if scope == "axis:purchase" and case["axes"]["purchase"] != "none": assert record[label+suffix] is not None
            if scope == "same-context-path" and case["path"] != "skip-all": assert record[label+suffix] is not None
    records.append(record)
provenance = dict(batchIdentity=manifest["identity"],sourceHash=json.loads(Path(manifest["plan"]["snapshot"]).read_text())["sourceHash"],
    rawHash=digest(raw),sweepHash=digest(root/"sweep/sweep.json"),seeds=[matrix["seedStart"],matrix["seedStart"]+matrix["seedCount"]-1])
(destination / "results.json").write_text(json.dumps(dict(provenance=provenance,records=records),indent=2)+"\n")
def mean(r,key): return float(r[key]["mean"]) if r[key]["mean"] else None
def delta(r,key,suffix):
    item = r[key+suffix]
    return float(item["mean"])*100 if item and item["mean"] else (0. if item is None else None)
def percent(value): return "n/a" if value is None else f"{value:.1f}"
def rate(r,key): return percent(mean(r,key)*100)
def paired(r,key,suffix):
    value=delta(r,key,suffix)
    return "n/a" if value is None else f"{value:+.1f}"
def average(r,key):
    value=mean(r,key)
    return "n/a" if value is None else f"{value:.3f} ({r[key]['n']})"
with (destination / "report.md").open("w",encoding="utf-8") as stream:
    stream.write(f"# Hare purchase candidate panel\n\n{matrix['runs']:,} runs: {len({r['candidate'] for r in records})} candidates x {len({r['path'] for r in records})} paths x {len({r['purchasePolicy'] for r in records})} purchase policies x {matrix['seedCount']} matched fresh seeds.\n\n")
    stream.write("Survival columns are percentages at tick 600; deltas are percentage points. APS and AHS are per-run means over actual observed windows, with Valid n in parentheses. Δbuy compares the same path with no purchases. Δpath compares Skip all with the same purchase policy. Self comparisons are zero.\n\n")
    for candidate in dict.fromkeys(r["candidate"] for r in records):
        stream.write("## "+candidate.replace("-","/")+"\n\n")
        stream.write("| Path | Buy policy | Hare alive % | Δbuy | All alive % | Δbuy | Δpath all | Mean APS (n) | Mean AHS (n) | Mean ADD | Mean spend |\n")
        stream.write("|---|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|\n")
        for r in records:
            if r["candidate"] == candidate:
                cells=[r["path"],r["purchasePolicy"],rate(r,"hareSurvival"),paired(r,"hareSurvival","VsNoPurchase"),
                    rate(r,"allSpeciesSurvival"),paired(r,"allSpeciesSurvival","VsNoPurchase"),paired(r,"allSpeciesSurvival","VsSkipAllSamePurchase"),
                    average(r,"meanAPS"),average(r,"meanAHS"),f"{mean(r,'added'):.2f}",f"{mean(r,'spent'):.2f}"]
                stream.write("| "+" | ".join(cells)+" |\n")
        stream.write("\n")
    stream.write("All paired uncertainty, wins/losses, window differences and rate applicability are retained in results.json and ../analysis/paired-deltas.csv. Unadjusted screening comparisons identify follow-ups; enjoyment and production balance require gameplay review.\n\n")
    stream.write("Window diagnostics record all five assigned windows, including unreachable windows. Next-phase population change starts after the purchase, so the immediate ADD is not counted as recovery. Slashlines and observed phase lengths remain in the raw report and analysis.\n\n")
    stream.write("Batch identity: `"+manifest["identity"]+"`. Raw SHA256: `"+provenance["rawHash"]+"`.\n")
window_fields=["condition","seed","candidate","path","purchasePolicy","phase","decisionTick","reached",
    "price","populationBefore","populationAfter","requested","added","balanceBefore","earned","spent","balanceAfter",
    "stopReason","nextPhaseTicks","nextPhaseHares","nextPhasePopulationChange","nextPhaseBirths","nextPhasePredationDeaths","nextPhaseStarvationDeaths"]
with raw.open(encoding="utf-8-sig") as source, (destination/"windows.csv").open("w",newline="",encoding="utf-8") as target:
    writer=csv.DictWriter(target,fieldnames=window_fields); writer.writeheader(); count=0
    for line in source:
        row=json.loads(line); run=row["run"]; case=by_case[row["condition"]]
        audits={p["phase"]:p for p in run["purchaseWindows"]}
        phases={p["phaseIndex"]:p for p in run["phaseResults"]}
        for phase in range(1,6):
            audit=audits.get(phase)
            record=dict(condition=row["condition"],seed=run["seed"],candidate=case["axes"]["candidate"],path=case["path"],
                purchasePolicy=case["axes"]["purchase"],phase=phase,decisionTick=phase*100,reached=audit is not None,stopReason="not-reached")
            if audit:
                record.update({k:v for k,v in audit.items() if k in window_fields})
                following=phases.get(phase+1)
                if following:
                    stat=next(s for s in following["herbivoreStatLines"] if s["speciesId"] == "hare")
                    record.update(nextPhaseTicks=following["windowEndTickInclusive"]-following["windowStartTickExclusive"],
                        nextPhaseHares=stat["FPO"],nextPhasePopulationChange=stat["FPO"]-audit["populationAfter"],
                        nextPhaseBirths=stat["BIR"],nextPhasePredationDeaths=stat["PREY"],nextPhaseStarvationDeaths=stat["STRV"])
            writer.writerow(record); count+=1
    assert count == matrix["runs"]*5
print("Purchase analysis complete:",destination,";",len(records),"arms;",count,"assigned window records.")
