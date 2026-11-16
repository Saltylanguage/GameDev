# M2 prevalidation implementation handoff

[Working state](../WORKING_STATE.md) | Status: implementation ready for validation, acceptance open

- Handoff schema: 1
- Handoff ID: 2026-11-15-0152-codex-m2-prevalidation-implementation
- Owner: codex
- Branch: codex/s6-predator-first
- Baseline commit: f4c1ee34
- Date: 2026-11-15
- Supersedes: none; extends the M2-02, M2-04, M2-07, and M2-08 handoffs

## Scope and current state

The local checkout has implementation for all ten M2 outcome rows available for
the November 16 validation batch. This is not an M2 acceptance decision. The
player route, Unity import/compile, visual, seeded, audio, persistence, and
human checks remain unrun for these new changes. All changes are local and
uncommitted in a shared dirty checkout; existing scene, art, and planning edits
from other tasks were preserved.

| Outcome | Implementation available to validate | Main remaining evidence |
| --- | --- | --- |
| M2-01 launch | Active-profile menu/Desktop route already fixes Forest Edge/Hare; map now says Foxes hunt Hares, explains six rounds, Mutation breaks, extinction loss, and the first trailhead action. Fresh-profile route assertion added. | Real exit/relaunch, immutable launch inputs, first-time understanding. |
| M2-02 live UI | Cell inspection, live objective, zoom/pan selection, and four working playback speeds are in the [M2-02 handoff](2026-11-15-0120-codex-m2-02-live-run-ui-implementation.md). Keyboard arrows now select cells after tab focus. | Pointer coordinates after pan/zoom, speed cadence, two resolutions, player read. |
| M2-03 route/roster | Authored Forest Edge Hare/Fox/resource route and identities remain in place. | Legitimate representative run, role distinction, seeded outcome, two resolutions. |
| M2-04 strategies | Fresh Hare route offers stackable Escape Instinct, Tough Hide, and Reproductive Drive under existing caps; see [M2-04 handoff](2026-11-14-1204-codex-m2-04-hare-response-implementation.md). | Matched seeds, tradeoffs, viable full journeys, player descriptions. |
| M2-05 art | Approved pastel eco-OS, board sprites, and imported Simulation journey icon are present. Existing [art handoff](2026-11-13-1850-codex-chrono-art-integration.md) lists missing Chrono originals; grass and desert were explicitly deferred. | Inspect actual board, menu, choices, results, and imported art at both resolutions. Source originals remain a production-art gap. |
| M2-06 audio | The existing menu and birth chimes now sit beside distinct selection, Mutation, danger, success, and failure cues plus quiet looping wind and music. Simulation hosts start/stop the bed; accepted danger reactions and choice/result transitions fire rate-limited cues. Settings reuses master volume and mute, including menu reload. | Listen at normal/4x cadence and with mute/volume; judge fatigue and the provisional procedural palette. |
| M2-07 dense feedback | Grouped, prioritized activity and capped visuals remain as in the [M2-07 handoff](2026-11-14-2041-codex-m2-07-feedback-implementation.md). | Crowded 1x/4x, pause/reset, suppression clarity, player read. |
| M2-08 causes/contributions | Prior phase-backed whole-expedition causes remain as in the [first-slice handoff](2026-11-15-0112-codex-m2-08-results-first-slice.md). The new metrics count three directly resolved Mutation effects across closed phases and show them on results. | Focused tests, successful/extinct/player-ended results, player causal explanation. |
| M2-09 persistence | Versioned profile, Gene Lab purchase/equip/eligibility, and next-journey application remain in place. | Fresh earn/buy/save/exit/relaunch/use route; migration/recovery and owned-vs-equipped read. |
| M2-10 onboarding/access | Opening/failure copy, keyboard board inspection, visible board/button focus, saved mute/volume, saved reduced-motion preference, a static current-node ring in place of the map pulse, and text reactions during suppressed board animation are available. Existing results return to map/new run. | Keyboard route, contrast/layout, reduced-motion equivalence, unassisted replay. |

## M2-08 measurement boundaries

- Escape Instinct counts a predator contact only when the deterministic
  pre-contact roll passes with the Mutation's added avoidance chance and fails
  with that chance removed while other effects stay fixed.
- Tough Hide counts only a hit on a creature at its current full maximum health
  when its extra maximum health changes that hit from lethal to survivable.
  This conservatively omits later hits and does not estimate lifetime saves.
- Reproductive Drive counts an otherwise eligible reproduction chance roll
  that passes with the Mutation's added chance and fails without it. Later
  gestation or placement can still prevent an offspring. The panel says this.
- `SpeciesSimulationRunner` configures the effect bonuses from the current
  per-run loadout before each tick. `SpeciesSimulationMetrics` retains counters
  in the closed phase window, which the results panel sums across nodes and
  checkpoint restores. Non-introductory upgrade effects remain unisolated.

## Static checks and focused tests to run

- `git diff --check` passed. The edited Noesis XAML files parse as XML. The
  authored Unity tests were not run; the shared project's known November 14
  baseline had unrelated full-suite failures, so compare any new failures
  against that baseline before attributing them to this work.
- Run `SpeciesDomainTests.ToughHideLetsAHareSurviveOneBaseFoxAttack`,
  `PreContactAvoidanceWorksWithoutFleeSpeedAndReplaysAcrossFreshEntityIds`,
  `ReproductiveDriveCountsOnlyRollsItsChanceChanged`, and
  `MutationEffectCountsAreRetainedInTheClosedPhaseWindow`.
- Run `SpeciesPresentationPlayModeTests.JourneyOutcomeAggregatesPhaseEvidenceAndLeadingCause`
  and its canonical success, natural-failure, and manual-end result fixtures.
  Run `GalapagOSVisualAcceptanceTests.LiveBoardInspectionAndSpeedControlsUseCurrentRun`,
  `SettingsPersistMuteAndReducedMotion`,
  `SimulationAudioPaletteCreatesDistinctCuesAndTwoLoopingBeds`, and the
  fresh-profile route fixture.
- Capture the menu, map, live board at 1x/4x, Mutation choice, results for
  success/extinction/player-end, Settings, and a next journey at 1280×720 and
  1920×1080. Test click/drag/zoom/keyboard selection, focus, pause, mute,
  reduced motion, and long result text. Listen for repeated-event fatigue.
- Run legitimate and matched Hare seeds without tuning the authored Fox count
  or carcass ratio to force a pass. Have a first-time player explain Fox
  pressure, each Mutation, the leading result cause, and how to replay. Verify
  one permanent unlock survives a real exit/relaunch and is usable next run.

## Limits

The M2 gate stays open until its route and player evidence exists. The new
audio is a deliberately small procedural first palette, not selected final
recordings. Chrono source originals listed in the art handoff are still
missing; this pass did not substitute concept images or alter deferred terrain.
