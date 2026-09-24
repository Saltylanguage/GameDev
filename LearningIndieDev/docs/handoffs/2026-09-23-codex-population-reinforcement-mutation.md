# Repeatable phase Reinforcements Mutation

[Working state](../WORKING_STATE.md) | Status: implementation pending Unity verification

- Owner: Codex
- Branch: ProjectMain
- Baseline: `82b0b1c0` (`NF/FoxHuntingImprovements` merge)
- Date: 2026-09-23

## Implemented contract

- The active experimental phase offer always places **Reinforcements** in slot
  three. It adds one individual of the player's species per selection.
- It can be selected again at later phase decisions; each decision still allows
  only one selection. The current maximum level is `int.MaxValue`.
- The spawn position is selected from unoccupied, passable cells using a seed
  derived from the run seed, tick, phase, and current loadout length. It does
  not consume the simulation's per-tick random stream.
- Placement respects the population cap, preserves tile terrain/resources, and
  fails as a whole if either the live or restart grid cannot fit the addition.
  Boundary history is updated for the next phase, and acquired reinforcements
  persist through a run restart.
- The current runtime phase-choice path charges the existing 5 Data cost. The
  product notes previously described Mutations as free; resolving that broader
  discrepancy was not part of this change.

## Verification and limits

- Added coverage for the fixed third offer, repeatable progression, deterministic
  placement, capacity rejection, next-phase population history, checkpoint
  restore, and restart persistence. The fox phase-choice UI test now checks the
  third offer's label and `+1 FOX` summary.
- Unity verification is pending. The focused EditMode run through
  `CellSim.ps1 -Command Test -Mode EditMode -Execution Auto` refused to run
  because the connected Editor reported `playing`; the active Editor was left
  untouched. Neither EditMode nor PlayMode tests have run for this change.
- No balance batch was run. The `+1` count and resulting population impact are
  provisional.
