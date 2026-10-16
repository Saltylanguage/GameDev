# Field Data publication Salty merge and planning alignment

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-10-16-0016-codex-field-data-publication-salty-merge-and-planning-alignment
- Owner: codex
- Branch: BevBranch
- Baseline commit: 0928359
- Date: 2026-10-16
- Supersedes: none

## Summary

Published Bevin's reviewed Field Data design/PDD/SDD and readable diagram, then
merged Salty's nine incoming ProjectMain commits. Created a combined planning
draft to reconcile overlap before ticket selection. No new Field Data gameplay
implementation or sprint kickoff occurred.

## Changes

- Planning publication: `d178a62`, pushed to `origin/BevBranch` before merging.
- Incoming tip: `341cc702`; merge commit: `0928359`. All 55 incoming Unity
  source/test/asset paths match the incoming blobs exactly. There were no code
  conflicts; resolved three documentation conflict regions in Working State
  and the S4 control record, retaining both dated histories.
- [Combined planning draft](../FIELD_DATA_AND_SIMULATION_ALIGNMENT.md) maps
  approved first-test rules, implemented foundations, Salty's proposed ecology/
  upgrade work, dependencies and unresolved scheduling/product decisions.
- Updated navigation and integration notes in the two S5 drafts, PDD/SDD,
  baseline, economy summary, prototype, product brief and project context.

## Decisions and assumptions

- S4's October 15 closeout supersedes the October 14 Active/P1-032 checkpoint.
  S5 remains Proposed; the older Bevin-side sprint draft remains on hold.
- Reuse the scrolling map, schema-v2 feature availability and new-journey FD
  reset. They do not implement the full 18-round economy, shops or permanent
  payout system. Preserve the accepted artwork contract and profile state.
- Use Bevin's Field Data baseline for the first test; retain Salty's broader
  simulation/permanent-content proposal without automatically scheduling it.
- The roadmap's six-round expedition wording needs owner alignment with the
  three-simulation test journey before declaring an overall revised contract.
- Do not change ecology between matched economy arms. Any selected ecology
  correction must have its own evidence and be frozen before that comparison.

## Validation

- `CellSim.ps1 -Command Doctor`: healthy, project closed, license ready.
- `CellSim.ps1 -Command Test -Mode All -Execution Clean` on merged source:
  **340/340 EditMode passed; 45 PlayMode passed, 0 failed, 4 skipped**.
  The four skips explicitly require a graphics-capable player/device: animal
  sprite initialization, terrain sprite initialization, terrain seams, and
  terrain topology. Do not report this as fresh visual/player-build acceptance.
- Evidence: `artifacts/unity-tests-20261016-001334/` contains both result XMLs,
  CLI records and logs. Doctor evidence is in
  `artifacts/unity-doctor-20261016-001315/`. These artifacts are local/ignored.
- Staged diff hygiene and documentation link/structure checks passed. The
  original 124 checklist IDs remain unchanged. No unresolved merge markers.
- The ten pre-existing flourish `.meta` files have identical SHA-256 bytes to
  the pre-merge backup; they remain unstaged and unpublished. Backup manifest:
  `C:/Users/bevin/AppData/Local/Temp/field-data-integration-20261016-kld38cgz/manifest.json`.
- Live development-board readback on October 16: Current Work 0, Upcoming 0,
  In Progress 0, Blocked 0, Done 13, Backlog 26. S4 control is complete in Done;
  P1-032 is Backlog. No S5 control appeared in the board inventory. Publication
  handoff belongs on the existing [player-agency reference](https://trello.com/c/lbwZtdrq),
  preserving its description/status and leaving ticket creation for next review.

## Risks and incomplete work

- Planning alignment is a draft. Team package selection, exact five-node route
  entry, frozen scenario/revision/seeds, owners/reviewers/estimates, permanent
  content inclusion and roadmap terminology still need review before commitment.
- No 200-seed economy run, new ecology implementation, standalone journey parity,
  player build, real exit/relaunch acceptance or fresh human visual review ran.
- Ten unrelated local importer edits remain outside the commits. Git is not a
  completely clean working tree; do not discard these edits to make it clean.

## Next useful step

Review the combined scope, choose a bounded first package, then prepare tickets
from the existing checklist/stages with explicit dependencies and evidence.
Do not duplicate already merged profile/map/reset implementation or turn
historical S4 acceptance gaps into claims of completion.
