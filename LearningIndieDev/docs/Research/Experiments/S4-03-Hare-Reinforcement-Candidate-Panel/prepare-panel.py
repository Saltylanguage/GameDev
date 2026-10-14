"""Verify retained candidates and write the approved purchase matrix, without runs."""
import hashlib
import json
import re
from pathlib import Path

OUT = Path(__file__).resolve().parent
PROJECT = Path(__file__).resolve().parents[4]
SOURCE = PROJECT / "artifacts/cellsim-250k-sweep-20261008-030316"
PATHS = ("skip-all", "trailblazer", "warren", "gardeners")
EXPECTED = {
    "D4/S19": {"populations":"325-30-15","grid":"48x28","fox-energy":"120","hare-vision":"8"},
    "D4/S14": {"populations":"325-30-15","grid":"48x28","fox-energy":"80","hare-vision":"8"},
    "C2/S14": {"populations":"250-20-10","grid":"36x20","fox-energy":"80","hare-vision":"8"},
    "D5/S25": {"populations":"325-30-15","grid":"54x32","fox-energy":"160","hare-vision":"9"},
}


def load(path):
    return json.loads(path.read_text(encoding="utf-8-sig"))


def digest(path):
    return hashlib.sha256(path.read_bytes()).hexdigest()


results = load(SOURCE / "all-path-benefit/results-with-statlines.json")
sweep = load(SOURCE / "sweep/sweep.json")
snapshot = load(SOURCE / "snapshot.json")
assert sweep["specHash"] == digest(SOURCE / "spec.json")
assert results["provenance"]["sweepHash"] == digest(SOURCE / "sweep/sweep.json")
cases = {c["id"]:c for c in sweep["cases"]}
records = {r["matrixCell"]:r for r in results["records"]}
selected = []
for cell,axes in EXPECTED.items():
    record = records[cell]
    assert record["axes"] == axes
    assert record["allSpecies"]["allThreePositive"] and record["hare"]["allThreePositive"]
    selected_cases = {}
    for path in PATHS:
        case = cases[record["conditions"][path]]
        assert case["axes"] == axes and case["path"] == path
        assert case["reference"] == record["conditions"]["skip-all"]
        args = dict(zip(case["arguments"][::2],case["arguments"][1::2]))
        assert args["-phaseLengthTicks"] == "100"
        selected_cases[path] = dict(originalCondition=case["id"],arguments=case["arguments"])
    selected.append(dict(id=cell,context=record["context"],axes=axes,basePathCases=selected_cases))

# Verify the active player cost, rather than accidentally using the legacy catalog.
preview = PROJECT / "Assets/Scripts/Game/Presentation/SpeciesSimulationPreview.cs"
progression = PROJECT / "Assets/Scripts/Game/Species/SpeciesProgression.cs"
match = re.search(r"public const int HarePurchaseCost\s*=\s*(\d+)\s*;",progression.read_text(encoding="utf-8-sig"))
assert match and int(match.group(1)) == 10
assert "public const int HareCost = SpeciesProgression.HarePurchaseCost;" in preview.read_text(encoding="utf-8-sig")
assert "TrySpend(HareCost)" in preview.read_text(encoding="utf-8-sig")
policies = [
    dict(id="none",caps=[0,0,0,0,0]),
    dict(id="early-five",caps=[5,0,0,0,0]),
    dict(id="middle-five",caps=[0,0,5,0,0]),
    dict(id="late-five",caps=[0,0,0,0,5]),
    dict(id="each-one",caps=[1,1,1,1,1]),
    dict(id="each-three",caps=[3,3,3,3,3]),
    dict(id="each-five",caps=[5,5,5,5,5]),
    dict(id="restore-toward-start",caps=[5,5,5,5,5],target="initialHarePopulation",onlyBelowTarget=True),
]
assert len({p["id"] for p in policies}) == 8
assert all(len(p["caps"]) == 5 and all(type(n) is int and 0 <= n <= 5 for n in p["caps"]) for p in policies)
conditions = [dict(id=cell.replace("/","-")+"-"+path+"-"+policy["id"],candidate=cell,path=path,purchasePolicy=policy["id"])
              for cell in EXPECTED for path in PATHS for policy in policies]
assert len(conditions) == len({c["id"] for c in conditions}) == 128
assert all(re.fullmatch(r"[A-Za-z0-9_-]{1,80}",c["id"]) for c in conditions)
assert all(sum(c["candidate"]==cell and c["path"]==path for c in conditions)==8 for cell in EXPECTED for path in PATHS)
assert set(range(50000,50100)).isdisjoint(range(sweep["seedStart"],sweep["seedStart"]+sweep["seedCount"]))
panel = dict(schemaVersion=1,recordedDate="2026-10-08",owner="Bevin / SimMasterBev",
    candidateSelection="Approved by user; candidates for development, not production defaults",
    purchaseProtocol="Approved by Bevin on 2026-10-08: yes run it",
    executionReady=True,executionLimitation="Use a fresh export and frozen build after purchase self-checks, Unity parity, historical control parity and the 128-arm smoke pass",
    source=dict(run=SOURCE.name,specHash=sweep["specHash"],sourceHash=snapshot["sourceHash"],
                batchIdentity=results["provenance"]["batchIdentity"],rawHash=results["provenance"]["rawHash"],
                survivalAndStatlineAnalysis="artifacts/"+SOURCE.name+"/all-path-benefit/results-with-statlines.json"),
    candidates=selected,paths=list(PATHS),purchasePolicies=policies,conditions=conditions,
    plannedScreen=dict(seedStart=50000,seedCount=100,conditionCount=128,runs=12800,workers=16,
        phaseLengthTicks=100,horizonTicks=600,decisionTicks=[100,200,300,400,500]),
    purchaseContract=dict(fieldDataPerHare=10,haresPerSuccessfulClick=1,initialFieldData=0,
        reward="Living Hare count credited once at each reached upgrade window",
        repeatUntil="Policy cap, target, insufficient currency or placement/capacity failure",
        mutationChoice="Separate; preserve original reachable offer-aware path choices",
        placement="Production TryAddBoundaryPopulation on current and restart grids",
        recordUnreachedWindows=True,allowTerminalResurrection=False,
        fieldDataPriceSource="Assets/Scripts/Game/Presentation/SpeciesSimulationPreview.cs:HareCost",
        legacyCatalogPriceExcluded=5),
    reportContract=dict(primary=["Hare survival at600","All-species survival at600"],
        supporting=["Mean Hare APS with Valid n","Mean Fox AHS with Valid n","Matched deltas",
            "Observed windows and phase slashlines","Successful ADD and spend","Earned and unspent Field Data",
            "Reached/unreached windows and failed-buy reasons","Recovery, populations, birth and death cause rates"],
        purchaseReference="Same candidate, same Mutation path, no purchases, same seed",
        mutationReference="Same candidate, Skip all, same purchase policy, same seed",
        acceptance="Exploratory candidates; fresh-seed and human gameplay review, no automatic production gate"))
(OUT / "candidate-purchase-matrix.json").write_text(json.dumps(panel,indent=2)+"\n",encoding="utf-8")
print("Verified four accepted candidates, 16 frozen path arms, eight approved purchase policies, 128 conditions / 12,800 planned runs.")
print("Verified player price10, candidate axes, both-endpoint qualification, source hashes and disjoint proposed seeds.")
print("Matrix validation only. No simulations launched by this script.")
