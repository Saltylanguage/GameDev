# Handoff — P3 worker execution and Unity preflight blocker

**Date:** 2026-09-03 03:44
**Owner:** Codex
**Branch:** `UI/ControlLibrary`
**Worker branch:** `codex/cellsim-worker`
**Status:** Worker publication verified; P3 execution remains blocked before simulation.

## What was executed

- Ran an isolated worker pass from the worker branch after applying
  `b52208f3`, which changes publication to push the detached `HEAD` explicitly
  to `refs/heads/codex/cellsim-worker`.
- The worker successfully published failure records instead of losing queue
  state. Published worker commits were `5a11fe37`, `de45d803`, and `ba92cb03`.
- The diagnostic jobs
  `20260902-233024-d8d75c20` and `20260902-233045-437566fd` failed before
  simulation and remain excluded from P3 evidence.
- Corrected EX-007 training job
  `20260903-033218-3b7607ba` also failed before simulation. The corrected
  held-out job `20260903-033240-b1b43c58` remains pending.

## Blocker

`Run-CellularExperiment.ps1` stopped at its intentional Unity preflight guard:
Unity was already running. The worker reported PIDs `88760` and `38616` during
the pass. No simulation output was generated, and no report, prediction, or
balance conclusion should be derived from these attempts.

The exact Unity process must be closed through an authorized desktop session
before resubmitting the training baseline and processing the held-out baseline.
The worker never closes Unity automatically, by design.

## Next P3 sequence

1. Close Unity and verify `Test-UnityPreflight.ps1` passes.
2. Resubmit the failed EX-007 training baseline with Forest Edge, Hare,
   `opposed-roll`, natural attack opportunity, `bev-experimental`, and seeds
   `1-20`.
3. Process the pending held-out baseline (`101-105`) and validate both bundles
   with `Test-CellSimArtifactBundle.ps1 -RequireUnityLog` plus the statline
   validator.
4. Pre-register the bounded prediction, then run S1/J1 training and held-out
   arms and complete the factual report, scoring, and human decision.

P3 is therefore carried forward, not passed. P4 design work may remain
preparatory only until this sequence produces a valid gate decision.
