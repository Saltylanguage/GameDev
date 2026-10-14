"""Bounded integration check. Usage: python check.py <export.json> <NEW-artifact-dir> [runner.dll]."""
import json
import subprocess
import sys
import time
from pathlib import Path


def main():
    if len(sys.argv) not in (3, 4):
        raise ValueError(__doc__)
    snapshot, root = map(lambda p: Path(p).resolve(), sys.argv[1:3])
    assert snapshot.is_file() and not root.exists(), "Use an existing export and NEW output directory"
    root.mkdir(parents=True)
    dll = Path(sys.argv[3]) if len(sys.argv) == 4 else Path(__file__).parent / "bin/Release/net8.0/CellSim.Batch.dll"
    command = ["dotnet", str(dll.resolve())]

    def invoke(*args, success=True):
        result = subprocess.run(command + list(map(str, args)), capture_output=True, text=True, timeout=120)
        assert (result.returncode == 0) == success, result.stdout + result.stderr
        return result

    def plan(name, workers=2, seeds=6, chunks=2, ticks=20, timeout=60, arguments=None, seed_start=10):
        path = root / (name + ".json")
        path.write_text(json.dumps(dict(schemaVersion=1, snapshot=str(snapshot), output=name,
            seedStart=seed_start, seedCount=seeds, chunkSize=chunks, workers=workers, timeoutSeconds=timeout,
            conditions=[dict(id="control", arguments=arguments or ["-runTicks", str(ticks)])])))
        return path

    invoke("self-test")
    serial, parallel = plan("serial", workers=1), plan("parallel", workers=3)
    invoke("batch", serial, "--dry-run")
    assert not (root / "serial").exists(), "Dry run wrote output"
    invoke("batch", serial)
    invoke("batch", parallel)
    invoke("verify", root / "serial/runs.jsonl", root / "parallel/runs.jsonl")
    assert (root / "serial/runs.jsonl").read_bytes() == (root / "parallel/runs.jsonl").read_bytes()
    before = (root / "parallel/runs.jsonl").read_bytes()
    invoke("batch", parallel, "--resume")
    assert before == (root / "parallel/runs.jsonl").read_bytes()
    assert json.loads((root / "parallel/status.json").read_text())["state"] == "Completed"
    invoke("batch", parallel, success=False)
    detailed = root / "replay.json"
    invoke("replay-batch", root / "parallel", "control", "10", detailed)
    assert json.loads(detailed.read_text())["finalStateDigest"] == json.loads(before.splitlines()[0])["run"]["finalStateDigest"]

    invalid = plan("unknown-option", arguments=["-typo", "true"])
    invoke("batch", invalid, "--dry-run", success=False)
    assert not (root / "unknown-option").exists()
    overflow = plan("overflow", seeds=2, seed_start=2147483647)
    invoke("batch", overflow, "--dry-run", success=False)
    mismatched = json.loads(parallel.read_text())
    mismatched["seedCount"] += 1
    mismatch = root / "changed-plan.json"
    mismatch.write_text(json.dumps(mismatched))
    invoke("batch", mismatch, "--resume", success=False)

    # Kill only this controller. Its two workers may outlive it; resume must
    # respect their file claims, retain their output and avoid duplicate attempts.
    recovery = plan("recovery", seeds=8, chunks=2, ticks=700)
    with (root / "recovery-controller.log").open("w") as log:
        process = subprocess.Popen(command + ["batch", str(recovery)], stdout=log, stderr=log)
        try:
            deadline = time.monotonic() + 30
            while len(list((root / "recovery/chunks").glob("*/attempt-*/status.json"))) < 2:
                assert process.poll() is None and time.monotonic() < deadline, "Workers did not start"
                time.sleep(.05)
            invoke("batch", recovery, "--resume", success=False)  # concurrent controller refused
            process.kill()
            process.wait(timeout=10)
        finally:
            if process.poll() is None:
                process.kill()
                process.wait(timeout=10)
    invoke("batch", recovery, "--resume")
    rows = [json.loads(line) for line in (root / "recovery/runs.jsonl").read_text().splitlines()]
    assert [row["run"]["seed"] for row in rows] == list(range(10, 18))
    assert len(list((root / "recovery/chunks").glob("*/attempt-*"))) == 4, "Duplicate recovery work"

    # Recover an interrupted aggregate using already validated chunks.
    (root / "parallel/summary.json").unlink()
    (root / "parallel/runs.partial.jsonl").write_text("incomplete aggregate")
    invoke("batch", parallel, "--resume")
    assert before == (root / "parallel/runs.jsonl").read_bytes()

    completion = json.loads(next((root / "serial/chunks").glob("*/completion.json")).read_text())
    with Path(completion["output"]).open("a") as output:
        output.write("{}\n")
    invoke("batch", serial, "--resume", success=False)
    timed = plan("timeout", seeds=20, chunks=20, ticks=700, timeout=1)
    invoke("batch", timed, success=False)
    assert not (root / "timeout/summary.json").exists(), "Timed-out batch claimed success"

    source = json.loads(before.splitlines()[0])
    source["run"]["finalStateDigest"] = "0" * 64
    altered = root / "altered.jsonl"
    altered.write_text(json.dumps(source) + "\n")
    invoke("verify", root / "parallel/runs.jsonl", altered, success=False)
    result = dict(state="Passed", checks=["compact/detailed determinism", "strict input rejection", "dry run",
        "exact serial/parallel output", "detailed condition replay", "resume identity", "controller exclusion", "orphan worker recovery",
        "aggregate recovery", "corrupt chunk rejection", "timeout refusal", "parity mismatch refusal"])
    (root / "checks.json").write_text(json.dumps(result, indent=2))
    print(json.dumps(result, indent=2))


if __name__ == "__main__":
    main()
