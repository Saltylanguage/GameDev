# feeding-fox-ramp-research-preparation

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-10-21-0717-codex-feeding-fox-ramp-research-preparation
- Owner: codex
- Branch: codex/audio-direction-and-sourcing
- Baseline commit: 6ffb7b10
- Date: 2026-10-21
- Supersedes: none

## Summary

Prepared the requested earlier-feeding and gradual-Fox comparison. Research
input/report support and verification are complete. Exact approval of the
proposed Fox schedule, observation window and optional combined arm is pending;
no ecology runs have started.

## Changes

- Public CellSim Run now forwards SpeciesStats to the existing registry-backed
  isolated rule override, also used by the portable runner.
- Added FoxIntroductionTicks: unique increasing explicit phase boundaries;
  one authored Fox is added via the existing boundary-population API at each
  tick. Failed placement throws with seed/tick. Policy/authored-phase/paired
  combinations are rejected rather than partially supported.
- Schema 39 adds requested stats, resolved Hare feeding threshold, schedule,
  actual introduction audits, and exact first successful feeding/mating/birth
  ticks. Introduction logs retain the same seed/tick/population values.
- Extended portable scientific arguments and export allowlist; validated
  detailed/compact deterministic replay and empty-phase world-state parity.
- Added focused tests, scope document, tooling guidance and working-state link.
  A prepared execution script is at
  `artifacts/feeding-fox-comparison-20261021-01/run-comparison.ps1`.

## Decisions and assumptions

- Josh asked for the threshold test and a lower starting Fox population with
  arrivals during the first minute. The proposed exact scope is 12 matched seeds
  10120–10131, 1200 ticks, Forest Edge 36×20, 400 Plants/20 Hares, no Genome or
  Mutations. Controls/feeding-only start with ten Foxes; ramp arms start with two
  and add one after ticks 75/150/225/300/375/450/525/600. Feeding threshold is
  6 versus 25. Use sixteen continuous 75-tick empty-loadout windows in every arm.
- Four arms include the combination; the alternate approval choice keeps only
  baseline and the two separate changes. The exact panel question is pending
  under SG-001/cellsim-experiment; existing authorization covers preparation
  and technical verification, not this unresolved panel choice.
- Introductions use authored starting energy and zero age on deterministic open
  passable tiles. They do not replace deaths or implement edge-entry migration.
  Tick-600 introduction acts from tick 601, followed by a full minute observed.
- Initial placements can differ between two- and ten-Fox setups. All fixed
  fields and same-starting-count pairs must match; no identical starting-world
  claim is made across different population inputs.

## Validation

- Standalone project final build passed with zero warnings/errors; portable
  regression self-test passed using installed runtime major roll-forward.
- Focused PopulationConfigurationTests passed 39/39, including exact schedule
  records, compact/detailed digest and metadata parity, resource-capacity failure,
  invalid tick inputs and no-op phase versus unphased final state.
  `artifacts/unity-tests-20261021-071252/EditMode-results.xml`.
- Final missing-phase/case-insensitive paired-mode guard passed 1/1:
  `artifacts/unity-tests-20261021-071554/EditMode-results.xml`.
- Earlier full set passed 39/39 at `unity-tests-20261021-070942/`. A prior
  namespaced filter selected zero tests (the class is SaltyGame.EditorTests);
  that empty result was not accepted as a pass and the corrected filter ran.
- Execution script passed PowerShell AST parsing; scoped git diff check passed.
  Runner XML and portable-self-test log are retained in the preparation bundle.
- No production values/scenes/assets, runtime domain behavior, branch,
  commit/push or external board were changed by this preparation.

## Risks and incomplete work

- Main objective remains unexecuted pending Josh's exact scope choice. There
  are no new ecological results or claims of improvement yet.
- Arrival records must be included in population accounting. Temporary zero-Fox
  intervals during the ramp are gaps, not irreversible extinction. Tick-600
  counts include the final new arrival; tick 1200 measures persistence after
  arrivals stop. Check both opening and later windows.
- Shared-tile diet-target rejection remains fixed across the arms and limits
  generalization. No automatic production adoption or extra sweep is approved.
- Preserve all prior dirty work. Pre-change copies of the touched tooling files
  are under the preparation bundle's pre-change directory.

## Next useful step

Read Josh's pending panel answer. Record the approved arm count/scope before
execution, then run (with elevated host permissions, from LearningIndieDev):

`powershell -NoProfile -ExecutionPolicy Bypass -File artifacts/feeding-fox-comparison-20261021-01/run-comparison.ps1 -IncludeCombined`

Omit IncludeCombined if Josh selects only the separate tests. Preserve all raw
reports/manifests and source snapshots, validate each intervention and population
account, summarize same-seed early and late differences, and record factual
results separately from interpretation and Josh's production decision. The
scope document is `docs/Research/2026-10-21-Feeding-and-Fox-introduction-comparison.md`.
