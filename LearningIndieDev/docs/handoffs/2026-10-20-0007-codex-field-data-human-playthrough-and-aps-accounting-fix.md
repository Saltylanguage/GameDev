# Field Data human playthrough and APS accounting fix

[Working state](../WORKING_STATE.md) | Status: complete

- Handoff schema: 1
- Handoff ID: 2026-10-20-0007-codex-field-data-human-playthrough-and-aps-accounting-fix
- Owner: codex
- Branch: BevBranch
- Baseline commit: acc371a
- Date: 2026-10-20
- Supersedes: earlier statements that no full human playthrough had been reported; previous frozen evidence remains intact.

## Summary

Bevin completed the five-node route and said it felt good, including near
population misses for both Foxes and Hares. The remaining reported defect was
APS showing INVALID. Read-only inspection of the still-open completed run
confirmed that paid population controls changed the board without updating
the population accounting used by the slashline. That defect is now fixed.

## Changes

- Record committed Field Data additions/removals per species on the retained
  run, exactly once. Rejected/stale commits and replayed purchase intents do
  not add counts. Natural birth/death counters remain separate.
- Carry an optional removed-population count through live and snapshot metric
  factories, Hare APS, Fox AHS, UI summaries and serialized reports (`REM`).
- Carry REM through worker CSV and Markdown reports and independent herbivore
  validation, with zero as the default for older reports. Update both role
  measurement contracts; preserve historical experiment artifacts/dictionaries.
- Include external changes in population reconciliation and remove their
  contribution from natural replication growth. Preserve INVALID for real
  count mismatches rather than suppressing the diagnostic.
- Add six-control integration/export checks, the captured human-run regression
  and a UI purchase/removal/reconciliation check. Record the playthrough in
  `docs/FIELD_DATA_SELF_PLAY_REVIEW.md` without inventing decision times.

## Decisions and assumptions

- Live run: seed 10100, tick 1800, final Hare 143 / Fox 26 / Plant 434.
- Confirmed receipts: AddHare 8, RemoveHare 9, AddFox 43, RemoveFox 9,
  AddPlants 10 total individuals. Old ADD was zero for both animal species.
- Screenshot/live counts: Hare start55 + births610 - predation327 - starvation194
  predicts144 when interventions are omitted. Correct accounting is
  **55 + 8 - 9 + 610 - 327 - 194 = 143**. Correct APS is approximately1.1866641.
- Fox: **20 + 43 - 9 + 68 - 96 = 26**. Correct AHS is approximately-0.5192977.
- Preserved read-only live receipts and both stat lines under
  `artifacts/aps-accounting-20261020/` before editing or leaving Play Mode.
  Bevin exited Play Mode to allow focused tests; it was not forcibly stopped.
- Positive full-route self-play is direct human evidence. It is not a matched
  human policy comparison, measured deliberation time, or a new tuning approval.

## Validation

- Pre-fix standalone regression reproduced ADD=0 after a successful board
  addition (`regression-before-v2.log`); the initial probe had a corrected
  assembly-path setup error and is not the bug reproduction.
- Current portable Release build: zero warnings/errors; existing self-test passes.
  Corrected captured Hare/Fox counts reconcile and yield valid scores in the
  standalone probe (`human-run-corrected.log`).
- Live Unity focused EditMode: **16/16** population-control/accounting/report
  tests at `artifacts/unity-tests-20261020-000637/`; **140/140** species-domain
  tests at `...000423/`; **3/3** report telemetry tests at `...000415/`.
  Total159 unique passing EditMode tests. The first combined pipe filter selected
  zero tests (`...000355/`); it is explicitly not a passing validation result.
- Live Unity Field Data PlayMode: **4 passed, zero failed, 2 graphics skips**
  at `artifacts/unity-tests-20261020-000507/`. Includes UI ADD/REM and reconciled
  APS after a paid add/remove, plus the full authored route and lifecycle checks.
- Unity Console after checks: compilationFailed=false, compiling=false,
  consoleErrors=0. `git diff --check` passed for touched code.
- Independent validator passes a deliberately synthetic10+2-3=9 external-control
  calculator fixture and the preserved legacy ordinary run (no REM), both
  VALIDATED_WITH_LIMITATIONS because some raw per-step audit events are absent.
  An earlier incomplete captured-count wrapper lacked required history and is
  not accepted validation evidence. The worker CSV helper retains ADD2/REM3
  and defaults legacy REM to0. Pure helper extraction avoids executing the queue
  worker; touched PowerShell files parse. No new ecological result is claimed.

## Risks and incomplete work

- No new standalone player build or graphics capture; the UI test verifies
  actual summary bindings and command behavior, not a fresh human visual check.
- The prior600-run panel remains frozen evidence of its declared revision.
  This change corrects reporting/counters, not population rules, prices or RNG;
  no cohort was rerun and no new direct parity claim is made for this source hash.
- T16 has positive complete custom-path play feedback and a keep-loop decision
  recorded; real decision times, matched human policy observations and any
  next-content decision remain unrecorded. Do not fabricate them or call all
  acceptance criteria complete. No Trello mutation, commit or push occurred.

## Next useful step

Use a fresh journey to see the corrected ADD/REM slashline. Existing ephemeral
run state predates the fix; its captured counts were used to verify the corrected
calculation. Continue from the recorded positive self-play evidence without
turning the descriptive automated panel into a numeric acceptance gate.
