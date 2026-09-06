# Legacy fresh-run fixture

This directory preserves one byte-for-byte fresh single-window report for
compatibility checks while the gameplay path moves to consecutive phases. The
fixture is deliberately a legacy report: it has no phase, acquisition-tick,
checkpoint, or continued-world fields.

| Field | Value |
| --- | --- |
| Source artifact | `artifacts/cellular-experiment-20260903-153000/report.json` |
| SHA-256 | `D2DA974444DADE64C2B44D22EC2C570E109D4EF18EB48E328204EC941791230F` |
| Source commit | Not embedded in schema 21; the source path and hash are authoritative. |
| Report schema | `21` |
| Created UTC | `2026-09-03T19:30:14Z` |
| Scenario | `Assets/Data/CellularSimulation/Scenarios/ForestEdge.asset` |
| Player species | `hare` |
| Loadout | `none` |
| Seeds | `101–105` (5 runs) |
| Grid | `32 × 32` |
| Window | `20` seconds at `0.1` seconds per tick (`200` ticks) |

The JSON file is an exact copy of the source artifact. Do not edit it in
place. New continued-world fixtures belong beside it under a new lifecycle
and report-contract version so old readers and new readers can be run against
the same fresh baseline without relabelling historical evidence.

This fixture is referenced by the [continuous simulation flow plan](../../../CONTINUOUS_SIMULATION_FLOW_PLAN.md)
and its evidence-validity register. It is a compatibility baseline, not
evidence that continuation is implemented.
