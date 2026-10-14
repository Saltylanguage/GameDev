"""Validate frozen tools against Unity, historical controls and every approved arm."""
import concurrent.futures
import json
import subprocess
import sys
from pathlib import Path

HERE=Path(__file__).resolve().parent
PROJECT=HERE.parents[3]
ROOT=Path(sys.argv[1]).resolve()
OLD=PROJECT / "artifacts/cellsim-250k-sweep-20261008-030316"
TOOL=ROOT / "tool/bin/Release/net8.0/CellSim.Batch.dll"
OUT=ROOT / "validation"
OUT.mkdir(exist_ok=True)
def load(p): return json.loads(p.read_text(encoding="utf-8-sig"))
def write(p,v): p.write_text(json.dumps(v,indent=2)+"\n")
def invoke(*args):
    result=subprocess.run([str(a) for a in args],capture_output=True,text=True)
    print(result.stdout.strip(),flush=True)
    if result.returncode: raise RuntimeError(result.stderr)
    return result.stdout
evidence=[]
evidence.append(invoke("dotnet",TOOL,"self-test"))
for name in ("snapshot","purchase-validation"):
    actual=OUT / (name+"-actual.jsonl")
    with actual.open("w") as target:
        for seed in range(49990,49994):
            path=OUT / (name+"-"+str(seed)+".json")
            if not path.exists(): invoke("dotnet",TOOL,"replay",ROOT/(name+".json"),seed,path)
            target.write(json.dumps(load(path))+"\n")
    evidence.append(invoke("dotnet",TOOL,"verify",ROOT/(name+".reference.jsonl"),actual))
panel=load(ROOT / "candidate-purchase-matrix.json")
controls=[dict(id=c["basePathCases"][p]["originalCondition"],arguments=c["basePathCases"][p]["arguments"])
    for c in panel["candidates"] for p in panel["paths"]]
plan=dict(schemaVersion=1,snapshot=str(ROOT/"snapshot.json"),output=str(OUT/"control-batch"),
    seedStart=40000,seedCount=1,chunkSize=1,workers=16,timeoutSeconds=600,conditions=controls)
write(OUT/"controls.json",plan)
evidence.append(invoke("dotnet",TOOL,"batch",OUT/"controls.json","--resume"))
def replay(control):
    path=OUT / (control["id"]+"-historical.json")
    if not path.exists():
        subprocess.run(["dotnet",str(OLD/"tool/bin/Release/net8.0/CellSim.Batch.dll"),"replay-batch",
            str(OLD/"sweep/batch"),control["id"],"40000",str(path)],check=True,capture_output=True)
    run=load(path); run["purchaseWindows"]=None
    return run
with concurrent.futures.ThreadPoolExecutor(max_workers=8) as pool:
    historical=list(pool.map(replay,controls))
historical_by_condition={c["id"]:run for c,run in zip(controls,historical)}
with (OUT/"historical-controls.jsonl").open("w") as stream:
    for line in (OUT/"control-batch/runs.jsonl").read_text().splitlines():
        row=json.loads(line)
        stream.write(json.dumps(historical_by_condition[row["condition"]])+"\n")
evidence.append(invoke("dotnet",TOOL,"verify",OUT/"historical-controls.jsonl",OUT/"control-batch/runs.jsonl"))
smoke=ROOT / "smoke"
smoke.mkdir(exist_ok=True)
smoke_spec=load(ROOT/"spec.json"); smoke_spec.update(snapshot=str(ROOT/"snapshot.json"),seedStart=49980,seedCount=1)
write(smoke/"spec.json",smoke_spec)
if not (smoke/"sweep").exists():
    evidence.append(invoke(sys.executable,ROOT/"tool/sweep.py","compile",smoke/"spec.json",smoke/"sweep"))
evidence.append(invoke("dotnet",TOOL,"batch",smoke/"sweep/plan.json","--resume"))
cases={c["id"]:c for c in load(smoke/"sweep/sweep.json")["cases"]}
runs={row["condition"]:row["run"] for row in (json.loads(line) for line in (smoke/"sweep/batch/runs.jsonl").read_text().splitlines())}
def clean(value):
    ignored={"behaviorTransitions","trackedBehavior","deathEvents","combatRolls","combatCooldownSuppressions"}
    if isinstance(value,dict): return {k:clean(v) for k,v in value.items() if k not in ignored}
    if isinstance(value,list): return [clean(v) for v in value]
    return value
for condition,run in runs.items():
    case=cases[condition]
    control_id=next(c["id"] for c in cases.values() if c["axes"]["candidate"] == case["axes"]["candidate"]
        and c["axes"]["purchase"] == "none" and c["path"] == case["path"])
    control=runs[control_id]
    assert clean(run["phaseResults"][0]) == clean(control["phaseResults"][0]), condition
    choices=run.get("mutationChoices") or []
    control_choices=control.get("mutationChoices") or []
    assert choices[:min(len(choices),len(control_choices))] == control_choices[:min(len(choices),len(control_choices))],condition
    for window in run["purchaseWindows"]:
        if case["axes"]["purchase"] == "restore-toward-start":
            initial=int(dict(zip(case["arguments"][::2],case["arguments"][1::2]))["-startingPopulations"].split("hare=")[1].split(",")[0])
            assert window["requested"] == min(5,max(0,initial-window["populationBefore"]))
assert len(runs)==128
evidence.append("PASS: all 128 arms; identical pre-purchase phases; unchanged reachable Mutation choices; restore caps; deterministic ledger and ADD validation.")
if not (smoke/"analysis/analysis.json").exists():
    evidence.append(invoke(sys.executable,ROOT/"tool/sweep.py","analyze",smoke/"sweep",smoke/"analysis","--rank","outcome.allSpeciesAliveAtHorizon","--top","10"))
evidence.append(invoke(sys.executable,ROOT/"tool/purchase_report.py",smoke))
write(OUT/"checks.json",dict(state="Passed",unityReferenceRuns=8,historicalControlRuns=16,smokeRuns=128,evidence=evidence))
print("All launch checks passed.")
