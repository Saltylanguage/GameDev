# Approved Hare purchase panel: implemented and running

2026-10-08. BevBranch / HEAD e7a6c602. Existing standalone/export/metrics work remains dirty; this block made no commit or push.

Bevin retained D4/S19, D4/S14, C2/S14 and D5/S25 and approved the purchase screen with "yes run it". Research contract: `docs/Research/Experiments/S4-03-Hare-Reinforcement-Candidate-Panel/PROTOCOL.md`.

## Execution

Root: `artifacts/cellsim-hare-purchase-20261008-132317/`.
Started 2026-10-08 13:34:56 EDT / 17:34:56 UTC. Pipeline process at launch: 36288.
128 conditions x 100 matched fresh seeds 50000-50099 = 12,800 runs; 16 workers, 25-seed chunks, 512 chunks, six 100-tick phases.
Batch identity: `7fc4666c6ad2459af4c6917e32ca21987d8399a0417f2cfaf6c8a1767899fafa`.
New source hash: `8e61009504e4610c77a37132f47f8ef75da15333dd678a029b1bacc0310fe3b1`.
The new Unity export's authored data, arguments and catalog match the previous export apart from source metadata. Historical hashes were preserved.

At the first post-launch checkpoint, 16 of 512 chunks / 400 runs were validated, all 16 workers active, pipeline Running, error log empty. This is launch evidence. Reacquire live status before reporting completion.

`run.ps1` automatically runs the frozen .NET batch, `tool/sweep.py` analysis, then `tool/purchase_report.py`. Read `pipeline-state.json`, `sweep/batch/status.json`, `run.stdout.log`, and `run.stderr.log`.
Completed outputs: `analysis/metrics.csv`, `analysis/paired-deltas.csv`, `analysis/metrics.sqlite`, `purchase-analysis/report.md`, `purchase-analysis/results.json`, and `purchase-analysis/windows.csv` (64,000 assigned window records).
For a verified resumable interruption, use the frozen `run.ps1 -Resume`. Do not change frozen inputs or executable during the active batch.

## Implementation

The shared experiment runner now handles all eight policies in both Mutation and scheduled Skip-all flows: none; five early/middle/late; one/three/five each window; restoration toward the starting Hare count capped at five.
Reached ticks 100/200/300/400/500 credit living Hares once, then attempt individual purchases subject to funds and production placement on current/restart grids and population limits. Initial Field Data is zero; currency carries between windows. Terminated runs cannot be rescued.

`SpeciesProgression.HarePurchaseCost=10` is shared with the player's public `SpeciesSimulationPreview.HareCost`, preserving the price. Legacy catalog reinforcement cost five remains separate. Buying does not consume the Mutation choice or add a reinforcement Mutation to its loadout.
Schema 35 adds `purchaseWindows`. Balances, rewards, spend, requested/actual ADD, before/after populations and stop reasons reconcile.
The analyzer records purchase totals and status-aware APS/AHS. Reports compare same-path/no-purchase and Skip-all/same-purchase, with following-phase population change starting after immediate ADD.
Currency totals cover upgrade-window credits/balances; final-run reward remains separate as `run.currencyEarned`.

## Validation

- .NET Release build: zero warnings/errors; self-checks passed for all policies, affordability, carryover, placement/capacity without charging, repeat determinism, compact/detailed parity, invalid input rejection and none parity.
- Eight Unity-to-.NET reference runs passed: exact state/counters/choices/timing, derived report-rate tolerance 1e-6. Four exercised Trailblazer with each-five.
- All 16 candidate/path controls matched the original frozen executable on seed 40000. A validation-script ordering mismatch was corrected by matching condition IDs.
- The 128-arm smoke on separate seed 49980 passed reconciliation, identical pre-purchase phases, unchanged reachable Mutation choices and restore caps.
- Smoke analysis/report completed: 35,840 metric windows, 120,680 paired comparisons, 128-arm tables and 640 assigned window records. A smoke report path assumption was corrected to read the frozen manifest snapshot path.
- Two focused SimulationReportTelemetryTests EditMode tests passed. CellSim.ps1 Test encountered the existing missing Resolve-UnityExecutionLane helper; the same Unity CLI clean lane ran directly. XML/log are in validation/. No unrelated tooling repair or full PlayMode run was attempted.
- git diff --check passed.

prepare-panel.py, compile-panel.py, validate-panel.py and run-panel.ps1 retain the repeatable preparation, verification and launch steps. Frozen artifacts contain matrix, spec, protocol, executable and analysis scripts.

## Research boundary

The smoke verifies execution only. Interpret the screen after simulation and automatic analysis complete. Report survival, APS/AHS with Valid n and matched deltas, alongside purchase/currency/recovery/resource effects.
Selection uncertainty and multiple comparisons remain exploratory. Gameplay enjoyment and production defaults require human review. Tick-700 viability remains undefined and is not a new gate here.
