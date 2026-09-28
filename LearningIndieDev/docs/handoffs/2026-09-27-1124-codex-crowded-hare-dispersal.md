# Crowded Hare Dispersal

[Working state](../WORKING_STATE.md) | Status: in-progress

- Handoff schema: 1
- Handoff ID: 2026-09-27-1124-codex-crowded-hare-dispersal
- Owner: Codex
- Branch: codex/forest-edge-visual-pass
- Baseline commit: a51bf4e0
- Date: 2026-09-27
- Supersedes: none

## Goal

Reduce Hare lock-in in dense groups and encourage the population to spread
across open space when predators are absent, without changing Fox behavior or
the existing Hare reproduction and threat-response rules.

## Evidence and implementation

The existing general movement chooser prefers less-crowded adjacent cells, but
rejects every destination that would still exceed `MaxReproductionGroupSize`.
The same local threshold plus tolerance is used by crowding metabolism. This
can leave an over-cap Hare with no legal move even when an adjacent empty cell
would reduce its local Hare count. This is a code-based hypothesis for the
reported stationary clusters; the live board behavior has not yet been
reproduced or measured.

The fallback wandering path now gives an over-cap Hare a dispersal exception:
it can enter an adjacent passable, empty cell above the normal group cap only
when that destination has fewer nearby Hares than its current location. The
crowding predicate is shared with the metabolism check. The behavior is Hare-
specific; threatened movement, targeted food/mate behavior, and other species'
movement rules remain unchanged.

A focused regression test places one Hare in a local cluster where its only
available move remains above the group cap but reduces the local Hare count.

## Validation

- `git diff --check` passed; Git emitted only line-ending normalization warnings.
- The focused EditMode test was attempted through `CellSim.ps1` with
  `-Execution Auto` after an initial implementation, but before final
  consolidation into the existing movement chooser. The runner refused to
  start while the connected Unity Editor reported `playing`; no test result was
  produced, and the final version has not been exercised. The active Editor
  was not altered.
- `Test-Handoffs.ps1` reports nine pre-existing errors in four older notes
  (missing historical artifact/asset links and unsupported legacy status text).
  It reported no error for this handoff.
- `dotnet build` was not used as a substitute: the generated
  `Temp/obj/Assembly-CSharp/project.assets.json` is absent.
- No seeded no-predator spread experiment has run. Expansion across the whole
  board and the effect on starvation remain unverified.

## Risks and next step

- This is adjacent-cell dispersal, not long-range migration. A Hare still
  cannot move if all nearby destinations are occupied or no less-dense
  destination is available.
- Hare states that explicitly stop movement (for example, the minimum mating
  interval) still do so; this change only affects the fallback roaming path.
- After Play Mode is stopped safely, run the focused EditMode test:

  ```powershell
  powershell -NoProfile -ExecutionPolicy Bypass -File .\CellSim.ps1 `
      -Command Test -Execution Auto -Mode EditMode -FilterType TestName `
      -TestFilter OvercrowdedHareDispersesIntoALessCrowdedCellEvenIfItRemainsOverTheLimit
  ```

- Then compare a small fixed-seed no-Fox Forest Edge batch against the current
  baseline, measuring Hare map coverage, movement steps, births, grass use, and
  starvation. Treat this as a prototype until the comparison is reviewed.
