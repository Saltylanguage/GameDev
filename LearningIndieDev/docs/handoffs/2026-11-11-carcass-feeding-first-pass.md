# Handoff — Fox carcass feeding first pass

**Date:** November 11–13, 2026
**Branch:** `codex/s6-predator-first`  
**Current source commit:** `8a92425866b4b5ef3cd9c77968f53554e17f63b8`
**Status:** Implementation and focused validation complete; full test run retains
12 failures; approved diagnostic completed with 340 accepted runs. No production
balance decision has been made.

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

## Initial implementation checks — November 11

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

## Unity verification — November 12 (blocked; superseded November 13)

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
- The first attempt wrote `EditMode-cli.json` and `EditMode.log` under
  `LearningIndieDev/artifacts/unity-tests-20261112-124249`; those files are not
  present in the current main checkout. This handoff retains the recorded
  outcome: fixture compilation failure and zero test results. No board visual
  test or ecology seed experiment had run at that November 12 checkpoint.

## Balance implication

The source energy ledger predicts one Hare per two Foxes for maintenance alone:
96 Hare energy versus 2 × 48 Fox energy per node. This excludes hunt failures,
reproduction, timing, and spatial encounter rates. It is not yet an empirical
Fox:Hare survival ratio.

The 1,500-run node-failure draft was superseded before execution because Josh
clarified that the present question is “how many Hares is a Fox worth?” The
approved five-arm diagnostic and its conditional scale extension are now
complete. See the [contract](../Research/Experiments/S6-Fox-Carcass-Value-20261111/CONTRACT-DRAFT.md),
[factual report](../Research/Experiments/S6-Fox-Carcass-Value-20261111/REPORT.md),
and [separate AI analysis](../Research/Experiments/S6-Fox-Carcass-Value-20261111/AI_ANALYSIS.md).

Predictive AI was not used because the new carcass-sharing rule has no matching
held-out calibration evidence. The approved direct seeded experiment has since
completed; its factual results and separate interpretation are linked below.

## Unity verification and experiment — November 13

- The GUID conflict is resolved to the canonical `Genome.meta` GUID
  `c576ac05bbb49f7488aa54f8ac574eaa`. No conflicting serialized references
  were found, and the canonical file has no outstanding tracked diff.
- Focused `SpeciesDomainTests` passed **160/160**. The full Unity Clean run on
  Unity `6000.4.6f1` recorded EditMode **764/773 passed, 8 failed, 1 skipped**
  and PlayMode **92/103 passed, 4 failed, 7 skipped**. It therefore did not
  pass. Failures and logs are indexed in the scoped diagnostics summaries at
  [`unity-tests-20261113-152327`](../../artifacts/unity-tests-20261113-152327/diagnostics-summary.md)
  and [`unity-tests-20261113-152426`](../../artifacts/unity-tests-20261113-152426/diagnostics-summary.md).
- The first batch attempt found a real no-preset telemetry gap: Hare `PREY`
  stayed zero while Fox kills occurred. That batch was rejected and none of
  its runs were accepted. The fix is in commit
  `8a92425866b4b5ef3cd9c77968f53554e17f63b8`; fresh reference runs reconciled
  both species' population accounting.
- The initial five-arm experiment completed 100/100 runs. Since the three
  2:1 scale arms differed and the approved contract made expansion conditional
  on unresolved ratio uncertainty, those arms were extended by 80 matched
  seeds each. The resulting evidence is **340 accepted runs**: 100 seeds each
  for 2 Fox/1 Hare, 4/2, and 6/3; 20 seeds each for 6/1 and 6/6. The separate
  seed-62020 smoke run is excluded.
- The experiment's source hash, scenario fingerprint, raw JSONL hashes, plan
  identities, and per-seed ledgers are recorded in the [factual report](../Research/Experiments/S6-Fox-Carcass-Value-20261111/REPORT.md)
  and retained under `LearningIndieDev/artifacts/S6-Fox-Carcass-Value-20261113-153600/`.
- The run was headless. No board visual inspection or manual playtest was
  performed. No production energy or population values changed.

## Next step

Josh can now review the [factual report](../Research/Experiments/S6-Fox-Carcass-Value-20261111/REPORT.md)
and [AI analysis](../Research/Experiments/S6-Fox-Carcass-Value-20261111/AI_ANALYSIS.md),
then decide whether to preserve the provisional 96/48 energy values or request
a separate reproduction-isolation or manual-playtest pass. The report shows
that 96 Hare energy is two Fox maintenance budgets locally, but does not
support a universal ecological Fox:Hare ratio.

## Not checked / risk

The full Unity suite still has 12 failures outside a clean pass; review the
retained NUnit XML before release. Noesis board rendering and manual play have
not been inspected. The no-decay carcass persists until consumed; if later
scenarios accumulate stale food, decay needs a separate design decision. The
portable runner targeted .NET 8 but ran under installed .NET 10.0.4 with major
roll-forward; a native .NET 8 runtime was not available.
