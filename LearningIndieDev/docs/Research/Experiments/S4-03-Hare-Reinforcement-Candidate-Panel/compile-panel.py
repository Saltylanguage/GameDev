"""Freeze the approved panel's tools, inputs and executable sweep; run no simulations."""
import json
import shutil
import subprocess
import sys
from pathlib import Path

HERE = Path(__file__).resolve().parent
PROJECT = HERE.parents[3]
ROOT = Path(sys.argv[1]).resolve()
assert ROOT.is_relative_to(PROJECT / "artifacts")
assert not (ROOT / "spec.json").exists()
subprocess.run([sys.executable, str(HERE / "prepare-panel.py")], check=True)
panel = json.loads((HERE / "candidate-purchase-matrix.json").read_text())
source = PROJECT / "artifacts" / panel["source"]["run"]
old_spec = json.loads((source / "spec.json").read_text())
snapshot = json.loads((ROOT / "snapshot.json").read_text())
previous = json.loads((source / "snapshot.json").read_text())
assert {k:v for k,v in snapshot.items() if k not in ("sourceHash", "sourceCommit")} == {
    k:v for k,v in previous.items() if k not in ("sourceHash", "sourceCommit")}
tool = ROOT / "tool"
shutil.copytree(PROJECT / "tools/CellSim.Batch/bin/Release/net8.0", tool / "bin/Release/net8.0")
for name in ("sweep.py", "purchase_report.py"):
    shutil.copy2(PROJECT / "tools/CellSim.Batch" / name, tool / name)
shutil.copy2(HERE / "candidate-purchase-matrix.json", ROOT / "candidate-purchase-matrix.json")
shutil.copy2(HERE / "PROTOCOL.md", ROOT / "PROTOCOL.md")
def options(args): return dict(zip(args[::2], args[1::2]))
def flat(args): return [token for pair in sorted(args.items()) for token in pair]
candidates = []
for candidate in panel["candidates"]:
    args = options(candidate["basePathCases"]["skip-all"]["arguments"])
    del args["-phaseUpgradeSchedule"]
    del args["-phaseLengthTicks"]
    candidates.append(dict(id=candidate["id"].replace("/", "-"), arguments=flat(args)))
purchase_metrics = ["purchase."+key for key in ("requested", "added", "earned", "spent", "balance",
    "windowsReached", "windowsNotReached", "stopped.insufficient-currency", "stopped.placement-or-capacity", "stopped.target-met")]
spec = dict(schemaVersion=1, snapshot="snapshot.json", seedStart=50000, seedCount=100, workers=16,
    chunkSize=25, timeoutSeconds=3600, maxRuns=12800, horizonTicks=600,
    baseArguments=["-phaseLengthTicks", "100"],
    axes=[dict(id="candidate", values=candidates), dict(id="purchase", values=[
        dict(id=p["id"], arguments=["-harePurchasePolicy",p["id"]]) for p in panel["purchasePolicies"]])],
    paths=old_spec["paths"], referencePath="skip-all",
    metrics=old_spec["metrics"] + ["herbivore.hare.ADD", "population.plant.final", "population.plant.min", "population.plant.mean"] + purchase_metrics)
(ROOT / "spec.json").write_text(json.dumps(spec,indent=2)+"\n")
subprocess.run([sys.executable, str(tool / "sweep.py"), "compile", str(ROOT / "spec.json"), str(ROOT / "sweep")], check=True)
sweep = json.loads((ROOT / "sweep/sweep.json").read_text())
assert len(sweep["cases"]) == 128 and sweep["runs"] == 12800
lookup = {c["id"].replace("/", "-"):c for c in panel["candidates"]}
for case in sweep["cases"]:
    original = options(lookup[case["axes"]["candidate"]]["basePathCases"][case["path"]]["arguments"])
    actual = options(case["arguments"])
    assert actual.pop("-harePurchasePolicy") == case["axes"]["purchase"] and actual == original
print("Frozen 128 exact approved arms, 100 fresh matched seeds, 16 workers; authored inputs match prior export.")
