# S4-02 Trailblazer and Editor manual testing first slice

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-10-01-2216-codex-s4-02-trailblazer-and-editor-manual-testing-first-slice
- Owner: Codex
- Branch: BevBranch
- Baseline commit: 3eea4e8
- Date: 2026-10-01
- Supersedes: none

## Summary

Bevin explicitly authorized starting reversible implementation without waiting
for Salty's approval. Salty's review remains pending; this is Bevin-authorized
local work, not a recorded team signoff. The first slice implements Trailblazer's
two basics and the minimum Editor fixture/stepping tools. No commit or push
was performed.

## Changes

- General Movement (`faster-movement`): +0.5 movement per level, cap 10, included
  in both relevant Hare pools and displayed under the workbook name.
- Threat Avoidance: retains `threat-exposure` identity and legacy alias; +8
  percentage points of pre-contact avoidance per level, cap 10, no flee-speed
  application in progression or batch loadouts. Historical flee constants
  remain only for API compatibility. The old speed-override diagnostic fails
  clearly instead of silently becoming an avoidance override.
- Snapshot v2 stores `PreContactAvoidanceChanceBonus`, with validation and
  fingerprinting. Report schemas 10/28 and prediction-input v2 serialize it;
  the Play Mode Markdown logger describes the ability as an effect. Existing
  constructor callers default this value to zero. Historical artifacts are
  untouched; new snapshot fingerprints intentionally change.
- Phase offers keep an uncapped Movement/Avoidance partner available after a
  Trailblazer pick, with a rotating third choice. Players can still choose
  other skills or Skip. Initial offers remain seeded. This implementation
  choice is reversible and remains part of Salty's pending review.
- `SimulationManager.AdvanceOneTickWhilePaused` uses the normal runner tick,
  clears accumulated frame time, remains paused, and delivers the existing
  boundary/completion events. Helper and preview expose that operation.
- The existing preview custom Inspector shows effective setup, tick-zero
  populations, fingerprints and acquisitions. Runtime buttons use validated
  lifecycle APIs; in-memory fixture setup does not save assets or presets.
- Focused tests cover caps, mixed picks, aliases, correct ability provenance,
  real batch snapshot application, normal five-choice progression, and paused
  tick equivalence/boundaries. Source and test edits reuse existing files;
  no Unity GUIDs, scenes, assets or serialized fields changed.

## Decisions and assumptions

This slice follows the [S4-01 workbook](../Species%20Design/S4-01%20Hare%20Strategies%20and%20Upgrade%20Paths.xlsx)
and [preflight checklist](2026-10-01-1947-codex-s4-02-implementation-checklist-and-editor-testing-preflight.md).
Bevin's October 1 authorization supersedes the preflight's instruction to wait
for review before implementation. It does not count as Salty's acceptance or
authorize publishing, merging or changing Trello ownership/capacity.

Scope is Trailblazer and the first two proposed Editor tools. Crowding reduction,
Seed Dispersal, Warren/Gardeners verification, named evidence capture and the
full S4 screen remain separate work. No new sprint estimate or commitment was
made. The existing six-phase/five-decision contract is preserved.

The Editor fixture is a developer path with no active Genome after resetting
the preview. It does not load an active profile or implement Josh's P1-032.
Existing authored upgrade assets and their repeat-pick restrictions are not
migrated in this slice; the tested current Hare phase path uses the static
catalog. Prototype seeds and one-tick phases are diagnostic inputs only.

## Using the Editor tools

1. Enter Play Mode and select the object with `SpeciesSimulationPreview`.
   The prototype currently starts automatically, so use **Reset to Start**.
2. Set **Fixture Seed** and click **Apply S4 Fixture**. The preview must already
   list the production Forest Edge scenario. This selects Hare, 36x20,
   400 Plants/25 Hares/15 Foxes, 0.1-second steps and six 100-tick phases;
   seed randomization and coupled responses are off.
3. Check actual tick-zero counts and the Genome/rules fingerprints. Use seed 5
   to observe a first Movement offer; other seeds can start with other choices.
4. Click **Start**, then **Pause** and **Advance One Simulation Tick** to inspect
   local behavior. At a phase decision use the Game view's real choices or Skip.
   Resume returns to normal timed advancement. Reset discards the test session.

At the initial implementation checkpoint, the Inspector's rendered layout had
not been visually accepted by Bevin. The manual follow-up below records the
subsequent check and Bevin's decision to close that testing pass.

## Validation

- Unity 6000.4.6f1 compiled this slice. Final focused EditMode run: **180 passed,
  zero failed**. Covered SimulationManager, SpeciesDomain, SpeciesUpgradeContract,
  PopulationConfiguration, prediction-input adapter and authored catalog tests.
- Focused graphics-capable PlayMode run: **2 passed, zero failed**:
  `TrailblazerFivePickPathWorksThroughNormalPhaseOffersAndPausedSteps` and
  `ApplicableHerbivoreSkillsRetainLevelsAndAcquisitionsAcrossDecisionBoundaries`.
  The first uses seed 5 and six one-tick phases, preserves the same run, chooses
  Movement/Avoidance/Movement/Avoidance/Movement, verifies acquisition ticks,
  unchanged flee speed, +1.5 general speed and 16% avoidance, then completes.
- Initial EditMode attempt had two failures: an obsolete speed expectation and
  a new comparison fixture creating different entity IDs. Corrected the escape
  test to use Movement and gave the normal/stepped runs copies of the same grid.
  The retained initial result is evidence of that repair, not a current pass.
- The generated schema-10 Play Mode JSON contains the correct five ordered
  snapshots and explicit avoidance bonuses, with no flee modifier. Retained a
  copy under `artifacts/s4-trailblazer-20261001/trailblazer-smoke-report.json`.
- Raw logs/XML are ignored local evidence under
  `artifacts/s4-trailblazer-20261001/`: `EditMode-results.xml` (initial),
  `EditMode-fixed-results.xml` (179 passed before the final offer check),
  `EditMode-final-results.xml` (180 passed), and `PlayMode-results.xml` (2 passed).
  At that automated checkpoint, no full suite, 600-tick pilot, balance screen
  or human acceptance ran.

Direct CLI validation used the installed project version without terminating
existing Unity CLI services. Representative command (final results kept above):

```powershell
unity --no-banner --non-interactive test F:/ForkBin/GameDev/LearningIndieDev `
  --mode EditMode `
  --filter 'SaltyGame.Tests.SimulationManagerTests;SaltyGame.Tests.SpeciesDomainTests;SaltyGame.Tests.SpeciesUpgradeContractTests;SaltyGame.EditorTests.PopulationConfigurationTests;SaltyGame.EditorTests.SpeciesUpgradePredictionInputAdapterTests;SaltyGame.EditorTests.SpeciesUpgradeAssetCatalogTests' `
  --output F:/ForkBin/GameDev/LearningIndieDev/artifacts/s4-trailblazer-20261001/EditMode-final-results.xml `
  --editor-path F:/Editor/6000.4.6f1-x86_64/Editor/Unity.exe --timeout 300 `
  -- -nographics -logFile F:/ForkBin/GameDev/LearningIndieDev/artifacts/s4-trailblazer-20261001/EditMode-final.log
```

The PlayMode call used the two full test names above, `--mode PlayMode`, the same
Editor/timeout, `PlayMode-results.xml` and `PlayMode.log`, and omitted
`-nographics`. `git diff --check`, PowerShell syntax parsing for the help edit,
local handoff links and absence of placeholders passed. The workbook SHA256
remains `FFDFB58C4ABD8653F713D24BE845819404597D680EBBF4B575148D285364AC28`;
the final diff contains no scenario/scene/GUID changes. Those automated checks
did not establish visual acceptance.

## Risks and incomplete work

- `CellSim Test` currently fails because `Invoke-UnityTests.ps1` references
  undefined `Resolve-UnityExecutionLane`. A repository search found the calls
  but no definition. `CellSim Doctor` also mistakes the running `unity mcp` CLI
  service processes for open Editors. Direct `unity status` found no connected
  Editor, and direct `unity test` succeeded. Neither wrapper issue was repaired
  as part of this gameplay slice; resolve before the larger experiment run.
- Offer composition and snapshot v2 are review points. Old reports describe old
  behavior and must not be pooled with current results as one tuning revision.
- The clipped General Movement card footer and stale setup display described
  below remain UI follow-ups. Authored/catalog integration beyond the current
  path, the remaining four basics, full replay fidelity and balance evidence
  remain open. The copied six-tick report proves mechanics/provenance only.

## Manual follow-up and Bevin's disposition - 2026-10-01

Bevin explicitly said to call visual testing of this part complete. This closes
the first manual testing pass; it does not claim that every Trailblazer choice
was observed, that the known UI issues are fixed, or that the ecology is balanced.

- The Inspector showed the S4 fixture at seed 5, 36x20, 400 Plants / 25 Hares /
  15 Foxes, 0.1-second steps and six 100-tick phases.
- Screenshots showed Inspector/Game view agreement at Paused tick 36. Bevin
  reported the pause/single-step check working; the exact one-tick delta was
  not independently captured in a before/after screenshot pair.
- The first decision appeared at tick 100. General Movement was selected;
  a later Inspector screenshot showed Paused tick 108 and the acquisition
  recorded at tick 100. The expedition retained that upgrade in its results.
- The run ended at tick 180 with Hare extinction, before the second decision.
  Threat Avoidance and the complete five-pick path remain unobserved in this
  manual run. No cause breakdown or matched-control balance conclusion was made.
- General Movement's FREE footer was visibly clipped on the choice card.
  After applying the fixture, the Ready Game view initially showed the previous
  seed/populations while the Inspector showed the new setup; after starting,
  a screenshot showed both views using seed 5 and the same running tick.
  These are unresolved presentation observations, not diagnosed causes.
- A separate 10-tick-phase diagnostic was proposed to reach choices earlier.
  Bevin found the setting but did not report applying or completing that run.
  Bevin instead plans to explore starting populations. No new values have been
  selected or recorded as approved replacements for the S4 fixture or authored
  production scenario.
- In a subsequent October 1 follow-up, Bevin reported that 400 Plants / 20 Hares /
  10 Foxes feels better during his exploration. Record 400/20/10 as his preferred
  candidate for the next comparison. Its seed, phase length, upgrade choices and
  final outcome were not supplied with that preference; no controlled balance
  validation is claimed. The implemented fixture and authored defaults remain
  unchanged.

## Next useful step

With Bevin's first manual pass closed, continue Crowding Tolerance and Seed
Dispersal from the checklist and retain the two UI follow-ups. Incorporate
Salty's feedback when available. Bevin's population exploration does not yet
change the approved test fixture or authored defaults. Repair the
execution wrapper before attempting the matched 600-tick control/candidate
checkpoint. Commit/push and update Trello only when sharing is requested.
