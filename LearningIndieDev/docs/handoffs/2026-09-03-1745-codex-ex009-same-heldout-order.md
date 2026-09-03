# Handoff — EX-009 same-held-out-seed upgrade-order comparison

**Date:** 2026-09-03 17:45
**Owner:** Codex
**Branch:** `UI/ControlLibrary`
**Status:** Package locked; forward arm blocked before simulation

## Goal

Compare `faster-movement,crowding-tolerance` against
`crowding-tolerance,faster-movement` on the same ForestEdge Hare held-out seeds
106–110. This closes the EX-007/EX-008 gap caused by using different held-out
panels for the two orders.

## What changed

- Added the EX-009 experiment package under
  `docs/Research/Experiments/EX-009-Same-Heldout-Order-Comparison/`.
- Added EX-009 to the canonical research plan and experiment index.
- Locked the same-seed A/B contract, corrected `PREY` semantics, discrete-range
  limits, and the no-bypass evidence gate in the package.

## Execution

The forward arm was attempted twice with the same configuration and stopped at
the intentional Unity preflight gate. Unity could not connect to its Package
Manager IPC service. Logs:

- `artifacts/unity-preflight-20260903-173849/license-probe.log`
- `artifacts/unity-preflight-20260903-174035/license-probe.log`

No forward-order report exists. The existing reversed-order bundle at
`artifacts/cellular-experiment-20260903-160827` must not be compared against
the prior forward bundle on seeds 101–105.

## Next safe action

Repair or restart Unity Hub/Package Manager licensing IPC, confirm
`Test-UnityPreflight.ps1` passes, then run the forward arm on seeds 106–110 and
validate the complete artifact bundle before calculating pairwise A/B deltas.
