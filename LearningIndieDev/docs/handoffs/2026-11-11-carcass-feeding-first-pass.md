# Handoff — Fox carcass feeding first pass

**Date:** November 11, 2026  
**Branch:** `codex/s6-predator-first`  
**Commit:** `5eead86a` (working tree contains broader pre-existing S6 changes)  
**Status:** Implementation and portable checks complete; Unity validation and
seed experiment pending

## Requested change

On a lethal attack, remove the Hare as a living creature and create a shareable
meat resource carrying its authored 96 energy. Foxes consume only enough meat
to fill their 96-energy capacity; the remainder remains available to nearby
Foxes. Retune Fox upkeep so 600 ticks represents 48 hours and costs 48 energy.

## Implementation

- `SpeciesCell` now distinguishes meat from plants and keeps a meat resource on
  the terrain underneath it. A lethal attack creates meat with the prey's
  `EnergyValue`; it no longer feeds the attacker automatically.
- Diet targeting can locate meat by prey species ID. Movement feeding consumes
  only available stomach capacity, records energy consumed, and preserves a
  partial carcass for later feeding. Aging, movement, reproduction, and board
  snapshots account for carcasses.
- The board draws carcasses with a small meat-and-bone glyph.
- Provisional Fox values are start 48, maximum 96, metabolism 2 every 25 ticks,
  and effective forage threshold 72. `reproductionFoodRequired` remains 48
  because it sets newborn energy; the Fox's 15% mating cost and 25% energy
  threshold are separate fraction fields.

## Checks

- `dotnet build tools/CellSim.Batch/CellSim.Batch.csproj -c Release
  --no-restore` passed with one NU1900 warning because NuGet vulnerability
  metadata could not be reached.
- Portable `self-test` passed after a normal-host retry. It confirms one Hare
  kill creates a 96-energy carcass, two Foxes share it to their 96-energy caps,
  and both finish a 600-tick no-reproduction fixture alive at 48 energy.
  The first sandbox attempt passed the carcass check but the unrelated replay
  self-test could not atomically rename a temp file; the host retry passed all
  portable checks. The .NET 8 runtime is absent, so the net8 assembly ran under
  the installed .NET 10 runtime with major roll-forward.
- `git diff --check` passed at this checkpoint.
- `CellSim.ps1 Doctor` reported `Unreachable`, `HasLockFile=True`, Pipeline
  unreachable. Unity EditMode tests and board visual inspection are therefore
  not verified; no lock was deleted and no other project's Editor was used.

## Unity verification — November 12

- With Josh's authorization, a headless Clean EditMode run filtered to
  `SpeciesDomainTests` was attempted. The first launch reached Unity but stopped
  before NUnit ran: `SpeciesUpgradeContractTests.cs:79` called `CreateRules`
  without its required `metabolism` and `awareness` arguments. The fixture now
  passes `metabolism: 1, awareness: null`.
- A single corrected launch was attempted. The project wrapper stopped before
  starting Unity because the worktree was `Unreachable` with `Temp/UnityLockfile`
  present. The lock is zero bytes and dated November 12 at 12:42:52 PM. It was
  left untouched; no Unity process was visible to this session, but Pipeline
  could not establish that the project was closed.
- The first attempt's result is
  [`EditMode-cli.json`](../../artifacts/unity-tests-20261112-124249/EditMode-cli.json),
  with its log alongside. It records compilation failure and zero test results.
  `git diff --check` passes after the fixture correction. No board visual test
  or ecology seed experiment was run.

## Balance implication

The source energy ledger predicts one Hare per two Foxes for maintenance alone:
96 Hare energy versus 2 × 48 Fox energy per node. This excludes hunt failures,
reproduction, timing, and spatial encounter rates. It is not yet an empirical
Fox:Hare survival ratio.

The 1,500-run node-failure draft was superseded before execution because Josh
clarified that the present question is “how many Hares is a Fox worth?” The
replacement 100-run diagnostic proposal (five arms × 20 matched seeds, one
600-tick node) is at
[S6 Fox carcass-value contract](../Research/Experiments/S6-Fox-Carcass-Value-20261111/CONTRACT-DRAFT.md).
Josh approved the scope on November 12. No ecology seed runs have started.

Predictive AI was not used: the new carcass-sharing rule has no matching
held-out calibration evidence. Direct seeded simulation is the useful next
evidence source once the contract is approved.

## Next step

When the worktree's Editor is reachable, rerun the focused EditMode check, then
export a fresh snapshot, validate the hashes, dry-run the approved plan, and
execute only the approved 100 runs. The Unity CLI's two `open` attempts
returned successfully but launched no Editor process; `unity status` reports
no instances. The Clean wrapper refuses to run while the current lock is
present and Pipeline is unreachable. Leave that lock untouched until the
project state can be confirmed.

## Not checked / risk

Unity domain tests, Noesis rendering, and production-scale encounter outcomes
remain unverified. The no-decay carcass persists until consumed; that is the
current first-pass rule and may need a later decay decision if it causes stale
food to accumulate. Unity launch troubleshooting is the current external-state
blocker; filing a Unity issue would send logs and needs separate user approval.
