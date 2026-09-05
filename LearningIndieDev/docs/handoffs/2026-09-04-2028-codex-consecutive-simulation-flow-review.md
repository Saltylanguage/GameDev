# consecutive-simulation-flow-review

[Working state](../WORKING_STATE.md) | Status: CF-0 complete; runtime migration ready for assignment

- Owner: codex
- Branch: NF/ConsecutiveRuns
- Baseline commit: d252255a
- Date: 2026-09-04

## Summary

Completed Josh's requested review and plan for keeping one evolving ecosystem
across simulation phases after an upgrade or Skip. Josh explicitly selected
review/planning first. CF-0 is now complete: the contract is locked and one
fresh legacy report is preserved as a versioned fixture. Runtime code, tools,
assets and experiments were not changed or newly implemented by this task.

Review started from `f8ccbdb4` on `NF/UpgradeContract`. During the session the
workspace gained `62996b4a` (main plan) and `d252255a` (evidence impact); those
commits were observed and preserved, not created by a Git command in this review.
The remaining documentation changes are local at this handoff. No push was
performed by this task.

## Changes

- [Architecture review and migration plan](../CONTINUOUS_SIMULATION_FLOW_PLAN.md):
  12 findings, locked CF-0 state/clock/upgrade/report decisions, seven proposed
  packages, 44–68h planning estimate and verification gates.
- [Evidence impact](../CONTINUOUS_SIMULATION_EVIDENCE_IMPACT.md): Stat-Line
  ticket mapping, prediction input changes, EX-010 prerequisites, telemetry
  producer/consumer coverage and explicit historical applicability register.
- [Documentation audit](../CONTINUOUS_SIMULATION_DOCUMENTATION_AUDIT.md): active
  document changes and implementation closure list; historical evidence retained.
- [Legacy fresh-run fixture](../fixtures/continuous-simulation/legacy-fresh-schema-21/README.md):
  byte-for-byte schema-21 report with seed/configuration/hash provenance for
  old-reader and continued-world reader comparisons.
- Updated 35 existing design/planning/reference documents, including GDD,
  product brief, TDD, project context, architecture diagrams, upgrade/economy
  guidance, active work plans, Stat-Line and research entry points.
- Posted and read back planning dependency notices on the existing
  [S2.3 Stat-Line card](https://trello.com/c/pZ4qG2DM) and
  [Predictive AI program card](https://trello.com/c/DViOsvbd). No assignments,
  estimates, list positions or completion states changed. Delivery is confirmed;
  owner acknowledgment and acceptance remain pending. The research notice was
  shortened after the connector's 2048-character limit rejected its first form.

## Locked decisions and assumptions

- Continue preserves the same world through purchase and Skip. The runner,
  absolute tick, prior grid, identities, cooldowns, resources and cumulative
  history survive each boundary.
- One gameplay expedition has five consecutive 200-tick phases. The prototype
  keeps its 20-second viewing cadence during migration; fresh-window research
  remains an explicit mode of the same step engine.
- Starting population, starting energy and starting reserve effects are
  launch-only. Live modifiers begin on the next tick and do not refill, heal or
  reset existing state.
- A signed maximum-energy change preserves current energy at the boundary,
  including temporary above-cap values. Later gains use the authored cap; an
  immediate clamp or grant requires a separately designed mechanic.
- Explicit Restart abandons the attempt and starts a new expedition from its
  original launch configuration with a new attempt identity. Phase retry is a
  future checkpoint/rollback feature.
- Phase results are windowed raw observations with actual partial/invalid
  status and acquisition decision/effective ticks. Final expedition results
  pool raw counters and recompute rates; they are not averages of phase ratios.
- In-memory continuation and research checkpoint reproduction do not authorize
  player disk save/load, permanent Lab economy or EX-010 experimental execution.
- Preserve completed reports/preregistrations/decisions; invalidate only their
  use as support for untested continuation claims. Proposed CF concerns remain
  unaccepted proposals; existing concern records retain their prior status.

## Validation

Ran the existing suites with the repository's standing approved elevated host
permissions through `tools/Invoke-UnityTests.ps1 -Mode All`:

- `artifacts/unity-tests-20260904-201307/EditMode-results.xml`: **200 passed,
  0 failed**.
- `artifacts/unity-tests-20260904-201307/PlayMode-results.xml`: **14 passed,
  0 failed, 1 ignored**. The ignored test is
  `CellularPrototypeInitializesEveryAuthoredAnimalSprite`, which needs graphics.
- These are current-runtime baseline tests, not proof of continuation behavior.
  No new research trial, new behavior test, Windows build or graphics-capable
  visual verification was claimed.
- Inspected the existing schema-23 EX-009 artifact
  `artifacts/cellular-experiment-20260904-192559/report.json`: one run per seed,
  200 ticks, no phase/acquisition/checkpoint fields.
- Preserved the fresh schema-21 single-window report from
  `artifacts/cellular-experiment-20260903-153000/report.json` at
  `docs/fixtures/continuous-simulation/legacy-fresh-schema-21/report.json`.
  The copied file is byte-identical (SHA-256
  `D2DA974444DADE64C2B44D22EC2C570E109D4EF18EB48E328204EC941791230F`).
- `git diff --check f8ccbdb4` passed after removing trailing Markdown whitespace
  in the new review documents. The final link pass checked **245 local links
  across 39 documents with zero missing targets**, including the two newly
  committed review documents and the remaining local changes.
- Verified no runtime, tool, serialized-asset or `.meta` changes in this task.

## Risks and incomplete work

The restart is at `PlayNextSimulation -> PrepareNextRun`: new grid, seed and
runner. A retained runner also needs phase-aware completion, immutable boundary
rule installation, prior-grid retention, deterministic IDs, atomic rewards and
windowed telemetry. Source inspection found a launch-provenance reset path and
starting-only Seed Pouches effects that need direct tests/decisions.

Sim's live S2.3 card assigns 3h to upgrade report/Stat-Line integration, whereas
the current repository S2 plan excludes Sim from the upgrade stream. Josh must
reconcile this mismatch before assigning the proposed packages. No allocation
was changed by the review.

Player/UI/tool-help and actual telemetry schemas remain unchanged until runtime
implementation. The documentation audit keeps those implementation closure
items open. EX-010 remains blocked on verified checkpoint/continuation
capability plus an approved experiment contract. No statement that old tests
pass closes those gates.

## Next useful step

Assign CF-1 through CF-6 with Sim's metric review, capture the implementation
baseline on the then-current branch, and execute the gates in the canonical
plan. Keep historical fresh-run evidence and new continuation evidence
distinguishable; do not begin EX-010 until checkpoint and producer/consumer
parity are verified.
