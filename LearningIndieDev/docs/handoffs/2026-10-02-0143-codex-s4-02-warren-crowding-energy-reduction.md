# S4-02 Warren crowding energy reduction

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-10-02-0143-codex-s4-02-warren-crowding-energy-reduction
- Owner: Codex
- Branch: BevBranch
- Baseline commit: 3eea4e8
- Date: 2026-10-02
- Supersedes: none

## Summary

Crowding Tolerance now reduces the extra crowding energy cost by 10 percentage
points of its original value per level, with a cap of 10. Warren's five choices
are reachable through the current player phase offers: Tough Hide, Crowding,
Tough Hide, Crowding, Tough Hide. Work remains local on BevBranch.

## Changes

- Added `CrowdingEnergyReduction` (finite fraction 0..1, default zero) and
  registry target `crowding.energy-reduction`. The static catalog's existing
  `crowding-tolerance` ID now adds 0.1 of this reduction per level.
- Preserved ordinary metabolism, its interval, the multiplier and crowd/birth
  eligibility. Eligible ticks deduct only the remaining original surcharge.
  A separate decimal loss remainder follows each animal through movement and
  immutable copies, survives upgrades, and clears on replacement/removal.
  Feeding's fractional gain remainder also now survives both movement routes.
- Kept historical `crowding.tolerance` threshold modifiers and the old enum
  value intact. Research fixtures retain their original meaning. The current
  S4 player phase route uses the static catalog; this does not migrate old
  authored research upgrades.
- Carried the new rule through authoring conversion, runtime draft/copies,
  registry application, snapshots, prediction inputs and fingerprints.
  Versions: attribute registry v5, scenario fingerprint v10, experiment report
  schema 29 and Play Mode report schema 11. Generic snapshot modifiers reuse
  the existing v2 contract.
- Added `crowdingMetabolismTicks` and `crowdingEnergyLost` to activity reports,
  snapshots and phase windows. The Inspector displays original extra-cost
  percentage remaining and these cumulative counters. Animal-ticks include
  eligible ticks at L10; energy lost is actual deducted energy, clipped at zero.
  These are not a counterfactual measure of energy saved.
- Added focused regressions in existing domain/contract/Editor test files;
  updated old static-catalog threshold expectations. Added no dependency,
  manager, scene wiring or authored population changes.

## Decisions and assumptions

Bevin explicitly selected this next implementation step after accepting the
slashline display. Standing authorization allows reversible implementation
before Salty's design review; this does not record team acceptance.

L1/L5/L10 leave 90%/50%/0% of the original extra surcharge. Reductions add,
not multiply. Free phase choices and paid upgrades share the cap and resolved
effect. The Warren path gives Tough Hide L3 (+6 Block) and Crowding L2 (20%
reduction). Pending fractional loss is retained across choices; L10 adds no
new loss. No extra RNG draws were introduced.

The S4 fixture remains 400/25/15, 36x20, 0.1-second steps and six 100-tick
phases; production population defaults remain 400/55/35. Bevin's preferred
400/20/10 remains a candidate for later matched comparisons.

## Validation

- Unity 6000.4.6f1 compiled the final scripts without errors. Live Console
  ground truth reported zero errors and one warning; the warning was present
  before this slice's live checks.
- **25/25 focused synchronous NUnit assertion cases passed via in-place
  reflection invocation**, not a standard NUnit suite run. Saved harness and
  results: `artifacts/s4-crowding-20261002/focused-assertions.cs` and
  `focused-assertions-results.json`. Checks cover L1/5/10, repeated fractional
  loss, metabolism/interval preservation, below-threshold behavior, caps and
  currency, feeding independence, movement/replacement, birth restrictions,
  starvation classification, Warren/Trailblazer offers, snapshot/batch parity,
  rule copies, authoring conversions, fingerprints, nonzero report counters
  and phase-window subtraction. A first authoring-copy test fixture omitted
  the Hare's Plant diet target; corrected the fixture to carry the whole roster
  and reran all focused cases successfully.
- The standard EditMode runner was started, then cancelled after its
  `SaveModifiedSceneTask` opened a modified-scene dialog. Bevin closed the
  dialog. No standard-suite pass is claimed. The in-place harness initially
  looked for behavior tests in the first of two fixture classes in the same
  file; corrected the harness class mapping. The test assembly was current.
- **Live player-flow check passed**: Forest Edge seed 0, 400/25/15, six
  one-tick diagnostic phases, coupled responses off. Used actual phase offers
  and `PurchaseReward`, retained state/currency, acquired the five ordered
  Warren choices and reached tick 6 with +6 Block/20% reduction. Report counters
  matched the domain and phase sums. This short run had zero crowding events;
  it proves choice flow, not energy benefit or balance. The crowded-cell
  assertion cases separately prove energy loss. Saved `live-warren.cs`,
  `live-warren-results.json` and `warren-playmode-report.json` in that folder.
- Inspected `warren-offer.png` at 1280x720: the Crowding offer reads
  "Cut extra crowding cost by 10%." and its FREE label fits. This is Codex's
  rendered check, not Bevin's acceptance of the new skill or all resolutions.
- Feature-file `git diff --check` passed. The full-tree check reports trailing
  spaces in the separately saved prototype scene's Unity YAML `m_Name` lines.
  The workbook and production scenario/species assets were not edited.

## Risks and incomplete work

No new balance result, matched-seed screen, full-suite pass or full-expedition
manual Warren observation is claimed. Salty review remains pending. Seed
Dispersal and the larger experiment checkpoint remain open, including the
pre-existing `Resolve-UnityExecutionLane` wrapper problem.

The prototype scene was dirty before testing. After Bevin closed the dialog,
Unity reported it clean and `CellularAutomataPrototype.unity` had a new local
diff (serialized preview fields and component ordering). That change was
preserved separately; Codex did not revert or edit it. Its SHA256 before and
after live Play Mode checks stayed
`0720CED6F17A6D6FB29FE64D6F3D28106A99F1F6A994F923B14038D1306A3879`.
The Editor is stopped with that same scene open. Review this scene diff before
the next commit; do not conflate it with the Crowding feature's authored data.

## Next useful step

Implement Seed Dispersal (+1 percentage point planting chance per level, cap
10, one stored food per successful planting) with direct attribution. Then run
matched control/strategy comparisons, including the 400/20/10 candidate,
using the restored slashline plus raw crowding/planting events. Preserve the
distinction between mechanics, presentation, manual acceptance and balance.
