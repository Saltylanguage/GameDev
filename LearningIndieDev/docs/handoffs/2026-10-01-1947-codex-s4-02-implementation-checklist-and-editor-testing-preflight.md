# S4-02 implementation checklist and Editor testing preflight

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-10-01-1947-codex-s4-02-implementation-checklist-and-editor-testing-preflight
- Owner: Codex
- Branch: BevBranch
- Baseline commit: 3eea4e8
- Date: 2026-10-01
- Supersedes: none

## Summary

The six proposed Hare basics map to existing simulation rules. General Movement
needs offer/cap integration; Threat Avoidance needs its speed bonus removed;
Crowding Tolerance needs a different energy rule; Seed Dispersal needs a selectable
upgrade and attributable results. Tough Hide and Efficient Digestion largely
need verification and presentation alignment.

Bevin expects to do manual testing inside the Unity Editor. The smallest useful
tooling proposal is to extend the existing preview Inspector with an explicit
S4 fixture and one-tick stepping, then retain a named evidence package. These
tools are recommendations, not implemented or added to sprint commitments.

## Changes

- Added this implementation checklist and linked it from working state.
- No runtime code, Unity assets, workbook values, or Trello cards changed.
- This note remains local until separately committed and pushed.

## Decisions and assumptions

The [S4-01 workbook](../Species%20Design/S4-01%20Hare%20Strategies%20and%20Upgrade%20Paths.xlsx)
is Bevin's accepted design draft. Read `S4 Basic Skills!A6:F11` for the six
contracts, `S4 Strategies!A7:G9` for identity and evidence, and `S4 Upgrade Paths`
for the agreed examples and screen. [Salty's review](2026-10-01-1711-codex-s4-01-hare-workbook-review.md)
remains pending; this preflight does not record team acceptance.

All six basics start with a level cap of 10. Players can mix them; Trailblazer,
Warren and Gardeners describe recognizable combinations. Keep the existing six
rounds and five decisions. Twenty-pick examples do not expand the expedition.
Do not add resource penalties beyond one stored food per successful planting.

The S4 experiment fixture is Forest Edge, 36x20, Plants/Hares/Foxes 400/25/15,
0.1-second steps, 600 ticks, no reinforcements, and coupled Fox responses OFF.
Production defaults remain 400/55/35 at 0.2 seconds. Freeze and record every
participating species' Genome snapshot; do not silently substitute profile data.
S4-02 ownership remains Sim 8h / Josh 2h; S4-03 remains Sim 4h / Josh 2h.
Tool estimates and allocation remain undecided. Preserve the S4 reserve.

## Implementation checklist

Paths below are relative to the Unity project root. These are proposed edits
after design review; inspected behavior describes baseline `3eea4e8`.

| Basic skill | Existing implementation and required change | Focused verification and manual observation |
|---|---|---|
| General Movement | `Assets/Scripts/Game/Species/SpeciesUpgrade.cs`: `faster-movement` already adds +0.5 MovementSpeed. Add to both relevant Hare offer pools and give it cap 10. Align display with General Movement. | Levels 1/5/10 add 0.5/2.5/5.0 to the baseline; an eleventh pick changes nothing and spends nothing. Verify fractional movement and actual offer availability. Observe travel toward fresh food and escape behavior, not only final population. |
| Threat Avoidance | Keep stable `threat-exposure` identity and legacy alias unless a deliberate migration is approved. `SpeciesProgression.ApplyLegacyUpgrade` and Editor experiment runner `ApplyLoadout` both apply the first-level flee bonus. Remove that effect from both paths and their snapshots/descriptions; retain +8 percentage points avoidance per level. | Update `ThreatExposureProgressionGrantsSpeedAndCumulativeAvoidanceThroughLevelTen`. At all levels flee speed stays at its baseline; avoidance reaches 80% at L10 and rejects L11. Verify preview/batch agreement and alias normalization. Read raw attack/encounter counts alongside eAVI/pAVI: avoided attacks do not become recorded encounters. |
| Tough Hide | `tough-hide` already adds +2 BlockAmount with cap 10. Preserve the opposed block mechanic; align offer text and resolved snapshot. | Verify +2/+10/+20 at L1/5/10 and rejected L11. Seeded combat checks must distinguish avoided attacks, winning blocks and health loss. Observe blocked attacks and dense-population survival. |
| Crowding Tolerance | Current `crowding-tolerance` adds +1 to the local threshold. Replace that upgrade effect with equal reductions of 10% of the original extra energy surcharge per level. `SpeciesSimulation.ResolveCrowdingMetabolism` currently deducts integer `Metabolism * (CrowdingMetabolismMultiplier - 1)`. Keep the ordinary metabolism pass and crowding eligibility unchanged. | Test 90%/50%/0% of the original surcharge at L1/5/10, with ordinary metabolism unchanged, and reject L11. Accumulate fractional losses deterministically: low levels must work over repeated eligible ticks. Do not reuse feeding's `EnergyRemainder` without proving losses and gains cannot interfere. Retain birth-gate tests; add direct surcharge diagnostics. `cAVI` cannot prove this benefit because energy deaths remain starvation deaths. |
| Efficient Digestion | Existing +0.1 DigestionEnergyBonus, cap 10 and fractional feeding accumulation already match the proposal. Preserve `SpeciesSimulation` feeding behavior and energy cap. | Reuse `EfficientDigestionAddsEnergyWithoutConsumingMorePlantFood`, `EfficientDigestionAccumulatesFractionalEnergyDeterministically`, and the cap test. Check successful feeding, repeated fractions, maximum-energy clipping and unchanged food consumed. Observe starvation and food availability. |
| Seed Dispersal | `SpeciesRules.SeedDropChance` and the registry target `resource.seed-drop-chance` already exist. Add a stable catalog ID, +0.01 chance per level, cap 10 and Hare offer integration. `SpeciesSimulation.ResolveSeedDrops` already checks stored food, searches nearby free cells, creates a full starting plant patch and spends one reserve only on success. | Extend `FedHerbivoresCanDropSeedsIntoEmptyTiles`: no reserve, full neighborhood, failed chance, successful plant, initial plant food amount, cost exactly one and L11 rejection. Specify eligibility/RNG ordering so the chance is per eligible simulation tick. Add direct attempts/successes/planted-food/reserve-spent attribution; aggregate plant births also include ordinary growth. |

Shared integration work:

- [ ] Use explicit attribute registry/applier targets and immutable rule copies.
  A new crowding reduction value must pass through validation, authoring/runtime
  conversion, copies, fingerprinting, upgrade snapshots and reports. Preserve
  Unity GUIDs and serialized compatibility. Seed chance already has a target.
- [ ] Confirm all callers before changing a catalog helper: preview progression,
  batch loadouts and scheduled phase choices must resolve the same effective
  rules/options for the same ordered acquisitions.
- [ ] Verify both actual offer routes. `PrepareRewardOptions` uses authored
  `SpeciesUpgradeAsset` snapshots when experimental features are off, and static
  catalog offers when on. Updating a static pool alone does not update the
  canonical player route. Authored `TryApplyRunUpgrade` currently rejects repeat
  IDs except population reinforcement; resolve that integration explicitly if
  this is the selected route. Do not introduce a parallel gameplay system.
- [ ] Check free phase decisions as well as currency purchases. L11 must not
  change rules, level, currency, loadout or acquisition history. Capped choices
  must disappear safely; Skip must work when no legal option remains.
- [ ] Keep Mutation order, acquisition tick/phase, resolved effect values,
  fingerprints and descriptions consistent. Do not reinterpret historical
  reports as evidence for the changed mechanics.
- [ ] Keep Genome inputs frozen and separate from the Mutation schedule. Do
  not expand Josh's P1-032 launch/profile work to implement an Editor fixture.

Useful test locations: `Assets/Tests/Runtime/SpeciesDomainTests.cs`,
`Assets/Tests/Editor/SpeciesUpgradeAssetCatalogTests.cs`,
`Assets/Tests/Editor/PopulationConfigurationTests.cs`, and
`Assets/Tests/PlayMode/SpeciesPresentationPlayModeTests.cs`. Add focused checks
for changed contracts; retain existing eligibility and progression regressions.

## Editor manual-testing tools

Reuse first: `Salty Game/Simulation/Species Catalog` inspects/edits authored
species and roster data; `Salty Game/Upgrades/Catalog Validator` validates
production upgrade assets. The preview already exposes start, pause, resume,
reset and phase decisions. Completed Play Mode runs automatically write
`artifacts/playmode-last-run.json` and `.md` through
`PlayModeSimulationResultLogger`. These files are overwritten by the next run.

The custom `SpeciesSimulationPreviewEditor` currently draws serialized fields
and a scenario picker. It does not expose runtime action buttons or single-tick
stepping. Extend this existing Inspector rather than creating another Lab or
general test dashboard.

| Priority | Proposed addition | Concrete use and completion check |
|---|---|---|
| First | **S4 fixture setup and effective-settings readout** in the preview Inspector. Apply scenario/seed and the existing `TryApplyGlobalSettingsForTicksWithStartingPopulations` API before starting. Show actual tick-zero populations, step, phase length, seed randomization, experiment flags, Genome fingerprint and ordered acquisitions. | Prevent accidentally testing 400/55/35 at 0.2 seconds. Use in-memory overrides and existing validation, target the explicitly selected preview, and reject setup changes after launch. Starting a fixture must leave the scenario asset and saved developer preset unchanged. Keep any scripted schedule explicitly a developer path; ordinary player observation still checks real phase offers and decisions. |
| Second | **Advance one simulation tick while paused**, plus Inspector buttons wired to existing lifecycle APIs. | Inspect a feeding, crowding-loss or planting tick. `SimulationManager.Advance` and `SpeciesSimulationRunner.AdvanceOneTick` currently refuse paused runs, so a button alone is insufficient. Add the smallest shared stepping API using the normal tick resolver, metrics and boundary/completion notifications. One click means exactly one tick, no wall-clock backlog or extra random draws; stay paused unless a decision or completion is reached. Never auto-skip a phase choice. Verify equivalent cells/metrics/acquisition state against normal advancement at the same seed. Unity's frame-step button is not a simulation-tick guarantee. |
| Third, if needed for the pilot | **Save a named evidence package** by reusing the completed-run logger and existing screenshot workflow. | Retain control and candidate runs without overwriting them. Store effective fixture inputs, source revision, Genome and Mutation provenance, full slash line with counts/denominators, and offer/board/summary images at 1280x720. Existing local metrics may be displayed read-only; new seed/crowding counters belong in their domain implementations, not inferred by an Editor window. |

Replay limitation: `tools/Invoke-UnityVisualEvidence.ps1` currently forwards only
scenario, player species, seed and grid to `CavePreviewPlayModeTests`. Its
manifest copies the source fingerprint but does not enforce a matching replay;
population overrides, step interval, ordered phase schedule, experimental
options and Genome are not restored through those replay parameters. Use this
for visual checks only until those inputs are restored and validated. A named
manual run with an explicitly checked configuration is enough for the first
pilot; do not build a generic replay framework for this task.

Defer arbitrary cell editing/spawning, a custom scenario builder, whole-run
automation in the normal player UI, a new Lab screen, and build-only debug UI.
The current need is observing the proposed basics inside the Editor.

## Order of work and review evidence

1. Salty/agent reviews the S4-01 workbook and this mapping as Accept, Revise or
   Questions. Confirm the selected offer route and shared effect contracts.
2. Integrate caps/offers and Threat Avoidance; implement crowding reduction and
   Seed Dispersal with direct diagnostics. Verify the existing digestion/hide
   behavior. Check preview/batch agreement before claiming any balance result.
3. Build the smallest Editor controls needed for the early checkpoint; record
   actual effort and revisit scope if the protected reserve is threatened.
4. Take one skip-all control and one candidate through setup, simulation,
   report analysis, slash-line validation and in-game observation. Match seed,
   scenario, populations, step, Genome and acquisition schedule. Label a
   developer launch if the canonical P1-032 route is not ready.
5. Review whether the player can explain the change and tradeoff from offers,
   board behavior and phase summary. Diagnostic fixtures with forced successes
   prove mechanics, not balance. Then run the agreed 13-arm x 20-seed screen;
   freeze paths/thresholds before the separate 200-seed confirmation.

Use the full DARWIN OR DIE slash line, ADD-aware populations, raw counts,
denominators, validity flags and variation across seeds. Trailblazer emphasizes
eAVI/pAVI/predAVG plus travel/food access; Warren uses pAVI/sAVI/bAVG/RFS/FPO
plus block and surcharge events; Gardeners uses sAVI/bAVG/RFS/FPO plus
successful planting, planted food and reserve spend. APS supports comparison.

## Validation

- Inspected the live workbook read-only and traced catalog, progression,
  immutable rules, resolver, preview offer selection, batch loadout application,
  tests, result logger and visual replay inputs at baseline `3eea4e8`.
- Working tree was clean before this documentation work.
- `git diff --check` passed for tracked edits; the new handoff's local links,
  code/test paths and absence of placeholders passed a Python check. Workbook
  SHA256 remains `FFDFB58C4ABD8653F713D24BE845819404597D680EBBF4B575148D285364AC28`.
- No Unity compilation, tests, simulation or rendered observation ran for this
  planning-only task. Existing test names identify future verification, not
  fresh passes. No current S4 balance or human-readability result is claimed.

## Risks and incomplete work

Team design review, gameplay implementation, proposed tools and the pilot remain
open. Static catalog and authored offers have different repeat-pick contracts;
Editor/batch Threat Avoidance application is duplicated; the existing replay
cannot reproduce the full S4 fixture. These are scoped integration findings,
not evidence that a proposed strategy is balanced.

## Next useful step

Review this checklist with the S4-01 workbook. After the design review, implement
the shared effect contracts and the minimum fixture/stepping controls required
for the early S4-02/03 checkpoint. Do not launch the full screen first.
