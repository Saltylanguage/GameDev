# fd2-t01-approved-definition-and-profile-integration

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-10-24-2158-codex-fd2-t01-approved-definition-and-profile-integration
- Owner: codex
- Branch: codex/fd2-t01-integration
- Baseline commit: 7ac2a54
- Date: 2026-10-24
- Supersedes: none

## Summary

Bevin approved the T01 effect/display-name proposal. The isolated integration
checkout now authors the five existing assets from one executable definition,
freezes owned-prerequisite loadouts without changing the POC resolver, and
reconciles local T02 with incoming profile data through schema v4. The definition
and migration tests pass. Full PlayMode retains seven failures reproduced
identically on unchanged incoming ProjectMain; no new PlayMode failure was found.

## Changes

- Added `FieldDataPermanentProgressionDefinition` (`field-data-permanent-v1`):
  the approved catalog, prices/caps and research terms, exact authored-map
  validation, and a Hare owned-prerequisite snapshot adapter.
- Added the explicit Unity authoring command and five authoring/domain tests.
  It updates the existing five assets and map through SerializedObject and
  AssetDatabase; all original node metadata/GUID hashes are preserved.
- Schema v4 combines the two prior v3 shapes safely. Canonical species wallets
  back the RabbitData projection; ResearchData backs Currency. Snapshot copies,
  incoming allocations/completion IDs, unlocks and zero-valued species keys
  survive. Incoming distinct generic Currency/RD combine with checked addition.
- Migration uses already-installed Newtonsoft JSON to retain missing-field
  information that JsonUtility fills. Raw pre-migration JSON is retained inside
  the new store; an occupied recovery key is preserved. Ambiguous/negative/
  overflowing inputs are left unchanged for recovery on load rejection.
- Carried forward the local T02 tests and added six compatibility/rejection cases.
- Added the new shared source to the portable compile and fingerprint manifests.
- Corrected two incoming stale EditMode expectations: 31 authored journey nodes;
  Undo/reload restores captured authored Hare/Fox counts instead of fixed old counts.
  Authored journey/scenario values were not changed.
- Copied/updated the FD2 documents only, preserving incoming Working State history.

## Decisions and assumptions

- Worktree: `C:\Users\bevin\.codex\worktrees\fd2-t01-integration\GameDev`.
- Branch: `codex/fd2-t01-integration`; incoming base `7ac2a54`.
- Original `F:\ForkBin\GameDev` stays on BevBranch with its local T02 code,
  metadata and recovery files unchanged. The isolated branch integrates that
  source deliberately; it has not been applied back to the active Editor.
- T01 is a content-definition gate. FD2 settlement, permanent purchase/reset
  operations, research transactions, next-launch composition and UI wiring
  remain T03 onward. The incoming POC UI retains its existing mechanics.
- The existing FD2 permanent ownership/no-refund decision is preserved. The
  definition's adapter validates owned prerequisites; the POC's stricter active
  prerequisite resolver is unchanged.
- Scope for this handoff is technically ready for closeout with the baseline
  failures stated. Full release/gameplay acceptance is not claimed.

## Validation

- Unity 6000.4.6f1 clean batch authoring completed. Five focused definition/
  asset tests passed in `artifacts/fd2-t01-authoring/definition-editmode.xml`.
- Profile EditMode: 33/33 at `artifacts/fd2-integration/profile-editmode-fixed.xml`.
  An earlier seven-failure run exposed JsonUtility's missing-list behavior;
  the Newtonsoft fix resolves those failures without weakening assertions.
- Full clean EditMode: 497 passed, zero failed, one dedicated parity-manifest
  skip at `artifacts/fd2-integration/all-editmode-fixed.xml`.
- Full clean PlayMode: 51 passed, seven failed, six graphics skips at
  `artifacts/fd2-integration/all-playmode.xml`. Both added migration/roundtrip
  tests appear by name with Passed status.
- Temporarily checkpointed integration changes and ran the untouched incoming
  base in the same isolated checkout: 49 passed, the same seven failures and
  six graphics skips at `artifacts/fd2-integration/baseline-playmode.xml`.
  All failure names/messages match exactly. See `baseline-comparison.json`.
  Restored tracked integration source hashes match the retained checkpoint.
- Original project profile/recovery preferences were preserved without
  displaying contents, restored after the baseline comparison and verified.
- Portable Release build: zero warnings/errors. Full `CellSim.Batch self-test`
  passed (18-round replay, receipts, determinism, schema/provenance and policies).
- Existing five node meta hashes match the pre-authoring capture. Unity generated
  metadata for new scripts. Focused diff hygiene and link checks are recorded
  before the local checkpoint commit.
- Commands used `unity test <isolated project> --mode EditMode|PlayMode ...` and
  `unity run <isolated project> ... -executeMethod
  SaltyGame.EditorTools.CreateFieldDataPermanentProgression.ApplyApprovedDefinition`.
  Project wrappers reject any open Editor, so direct CLI calls targeted only
  the closed isolated project, preserving the original open Editor.

## Risks and incomplete work

- Seven incoming PlayMode failures remain: Genome option count 7 vs 9; population
  control arithmetic 114 vs 90; legacy `hare.guard` absent from the launch catalog;
  two continuous-phase expectations 103/98 vs 0; a former failing seed now reaches
  a shop; and success seed 10100 dies at tick 1061. They reproduce on the untouched
  base and were not hidden or retuned to make this ticket green.
- One dedicated parity fixture and six graphics tests are skipped in these
  clean headless lanes. No new direct parity panel, player build, visual human
  acceptance or balance study was run.
- T01 definition and profile compatibility do not finish the rest of FD2. The
  active player screens still use the incoming POC flow until the later tickets
  explicitly wire the FD2 transactions and catalog.
- Work remains local. No push or live Trello update occurred in this turn.

## Next useful step

Review the isolated branch. T01 is technically ready for the approved-definition
card closeout; record the baseline limitations in any board comment. Continue
with FD2-T03's deterministic settlement calculator using the canonical terms,
then exactly-once persistence. Treat the seven incoming PlayMode failures as
separate integration debt rather than balance acceptance or a T01 effect failure.
