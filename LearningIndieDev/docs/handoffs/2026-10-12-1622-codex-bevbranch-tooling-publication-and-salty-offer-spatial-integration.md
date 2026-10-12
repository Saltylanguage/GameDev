# BevBranch tooling publication and Salty offer-spatial integration

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-10-12-1622-codex-bevbranch-tooling-publication-and-salty-offer-spatial-integration
- Owner: Codex
- Branch: BevBranch
- Baseline commit: 07d78de9
- Date: 2026-10-12
- Supersedes: none

## Summary

Bevin requested preservation of the confirmation-review notes, then a push of
our work, merge of Salty's updates, and another push. The first push published
`07d78de9` to `origin/BevBranch`. The incoming branch is
`origin/codex/s4-gestation-checkpoint-20261010`, tip `876f3cf7`; it contains seven
new commits. ProjectMain had no newer commits to integrate.

## Changes

- Published gestation controls, large-plan metadata handling, statistical
  worksheets, the large-XLSX memory fix, tests and their existing handoffs.
- Preserved the [confirmation review](../Research/Results/2026-10-12-Forest-Edge-Confirmation/README.md)
  in Git: report, chart, verification record and compact condition/choice tables.
  Original raw reports, frozen runners, workbooks and presets remain local and
  unchanged under ignored artifacts folders.
- Merged Salty's research records, Hare spatial observer, player-flow test,
  deterministic first-offer path coverage and authored opening rewards
  (Movement / Tough Hide / Efficient Digestion).
- Resolved three content conflicts: experiment runner, species domain tests and
  working state. Both branches' tests and research history are preserved.
- Retained BevBranch's existing scheduled-avoidance implementation and portable
  state/stat records. Removed the redundant incoming avoidance helper and adapted
  its regression to the shared implementation. The stale flee-speed gate was
  already removed on BevBranch; this merge does not newly change that rule.
- Report schema advances from 35 to 36. Spatial records are captured for ordinary,
  scheduled and automatic Mutation-policy runs, including Workbench strategies.
- Updated four PlayMode tests whose assumptions depended on old opening card
  order: select the intended skill by ID, use deterministic first-offer seeds,
  and verify the new authored rewards and Movement carryover. Their currency,
  level, acquisition, counterpart and continuity assertions remain intact.

## Decisions and assumptions

- This integrates existing approved behavior; no new population, gestation or
  balance value was selected.
- The preserved 8,000-run confirmation used its frozen premerge runner. New
  opening offers can change automatic choice acquisition and outcomes. Keep those
  historical results attached to their original source hash; fresh merged-code
  claims require a new snapshot/runner and matched controls.
- Salty's approved four-neighbor density surcharge and burrow mechanic remain
  unimplemented. Existing crowding telemetry measures the current rule.

## Validation

- Before merging: 31 worksheet tests, runner self-test, runner/Workbench Release
  builds and 16 native Workbench check groups passed.
- Merged code: both Release builds passed with zero warnings/errors; runner
  self-test, compact/detailed automatic-policy spatial parity, 31 worksheet
  tests, JavaScript syntax and 16 native Workbench check groups passed.
- Unity 6000.4.6f1 focused EditMode: 160 passed, zero failed/skipped. Filter:
  `SaltyGame.Tests.SpeciesDomainTests;SaltyGame.EditorTests.PopulationConfigurationTests;SaltyGame.EditorTests.SimulationReportTelemetryTests`.
  This includes gestation overrides, path coverage/caps, avoidance and spatial
  observer checks.
- Unity presentation PlayMode: initial run 21 passed / 4 failed, all traced to
  old offer assumptions. After the targeted test updates, 25 passed with zero
  failures/skips. Filter:
  `SaltyGame.PlayModeTests.SpeciesPresentationPlayModeTests`.
- Fresh Unity portable export and two-worker standalone batch: four matched
  Trailblazer seeds (42-45), six 100-tick phases, authored Forest Edge rules with
  400 Plants / 20 Hares / 10 Foxes. Verification passed exact state, counters,
  choices, timing and spatial records; derived rates use the verifier's 1e-6
  tolerance. Fresh source hash:
  `f6766e02b8dfecb24e74303da5a9dd02f993c17cf6a2b3cba45cef3d9269fd84`.
  Inputs and results: `artifacts/integration-20261012/merged/portable/`.
- The initial portable export command incorrectly combined automatic policy and
  explicit run length. Validation rejected it before writing frozen inputs;
  the corrected command uses phase length alone. Its failing log is retained.
- Evidence is under `artifacts/integration-20261012/{premerge,merged}/`, including
  the initial failing XML/log and the final passing XML/log.
- Direct Unity CLI was used because the previously recorded project execution
  wrapper defects remain outside this integration. Vendor package/import and
  transient licensing messages remain in logs; final test commands exited zero.

## Risks and incomplete work

- No full Unity suite, Windows player build, human visual acceptance or new
  balance matrix was run. The focused evidence is not full-game acceptance.
- The existing Workbench window/executable was not replaced. New-source runs
  require rebuilding the runner and exporting matching inputs. Saved prepared
  runs retain their pinned executables and remain historical.
- Spatial distance is Manhattan distance between observed positions; wrapped
  edge crossings can inflate that diagnostic. Do not interpret it as exact
  path length on wrapped maps.

## Next useful step

Review the retained findings and Salty's spatial screen before choosing the next
experiment. If testing the merged offer flow, freeze fresh inputs and compare on
matched seeds; do not reuse the old confirmation as current-build evidence.
