# M2 focused validation batch — November 15

[Working state](../WORKING_STATE.md) | Status: automated validation and approved M2-04 diagnostic recorded; M2 acceptance open

- Handoff schema: 1
- Handoff ID: 2026-11-15-0940-codex-m2-validation-batch
- Owner: codex; human decision owner: Josh
- Source: `codex/s6-predator-first` at `f4c1ee34`, with the disclosed shared dirty M2 implementation and art files. No commit or push was made.
- Scope: the ten [M2 outcome rows](../../ROADMAP.md#m2-outcome-contract--consolidated-2026-11-14), not a milestone acceptance decision.

## Execution and retained evidence

Unity Doctor was healthy with a valid license. The final Clean suite is in
[`artifacts/unity-tests-20261115-091405`](../../artifacts/unity-tests-20261115-091405):

| Mode | Passed | Failed | Skipped | Comparison with November 14 |
| --- | ---: | ---: | ---: | --- |
| EditMode | 769 | 8 | 1 | All eight failures have the same test names as the retained baseline. Five additional tests pass. |
| PlayMode | 101 | 1 | 7 | The sole failure is the same authored seed-10101 route case as the baseline. The prior five Quick Escape failures are gone. |

The focused graphics class produced 45 PNGs in
[`visual-evidence-20261115-092050`](../../artifacts/visual-evidence-20261115-092050).
It passed 18/19 tests; `FieldDataAuthoredRouteCapturesRoundShopReorderAndSuccessEvidence`
failed because authored seed 10100 lost its last Hare at tick 383 before round
four. Its [failure screen](../../artifacts/visual-evidence-20261115-092050/gljp-authored-route-unexpected-stop.png)
records 38 Fox kills and 51 starvation deaths, with 606 Plants remaining. This
is a separate real route failure from seed 10101. The [success capture](../../artifacts/visual-evidence-20261115-092050/gljp-income-route-success.png)
uses a controlled income fixture with no Fox encounters; it does not prove the
authored predator route clears.

The presentation graphics class produced
[`visual-evidence-20261115-093013`](../../artifacts/visual-evidence-20261115-093013):
34/37 passed. The same seed-10101 authored route failed. Sprite initialization
and four Burrow marker captures passed. Two terrain screenshot cases could not
run in Clean batch mode because `WaitForEndOfFrame` is not invoked there. Their
focused [live Editor rerun](../../artifacts/unity-tests-20261115-093342/PlayMode-results.json)
passed 2/2. The Editor opened for that rerun was closed afterward.

The final targeted Desktop, Settings, map, first choice, live board, and cue
captures passed at [1280×720](../../artifacts/visual-evidence-20261115-092639)
and [1920×1080](../../artifacts/visual-evidence-20261115-092811), 1/1 each.
The 1920 [cycle result and next-map capture](../../artifacts/visual-evidence-20261115-093641)
also passed 1/1. We inspected the actual images. The board, objective, three
opening response cards, Settings controls, and all four speed labels fit at
both resolutions. The result observations are dense at 1280 and still need a
player readability judgment. These captures are state fixtures; they do not
certify a full representative journey or input accessibility.

`git diff --check` found no whitespace errors; the three edited Noesis XAML
files parse as XML. The final XAML-only label/padding changes were imported by
the passing targeted visual runs after the full suite.

## Per-outcome result and remaining gate

| Outcome | Focused evidence now | Still required for acceptance |
| --- | --- | --- |
| M2-01 launch | Fresh-profile opening choice, menu/profile, and immutable launch PlayMode cases pass. Map copy appears in both resolution captures. | Real exit/relaunch and an unassisted explanation of the constrained Forest Edge/Hare launch. |
| M2-02 live UI | `LiveBoardInspectionAndSpeedControlsUseCurrentRun` passes. Live board and all four speed labels render at both resolutions. | Hands-on pointer location after zoom/pan, keyboard inspection and focus, actual cadence at 1×/4×. |
| M2-03 route/roster | Hare/Fox/Plant visuals and authored sprite checks pass. | A legitimate complete predator-route journey. Authored seeds 10100 and 10101 currently fail before the first node ends in the visual and PlayMode checks. No balance value was changed to force a pass. |
| M2-04 strategies | Fresh-profile access/stacking and direct avoidance, Tough Hide, and reproduction effect tests pass. The three opening descriptions render. | A newly approved current-source matched-seed comparison and complete player routes for each response, with observed tradeoffs. |
| M2-05 art | Board, Desktop, map, choices, result, Gene Lab, terrain sprites, and Burrow marker were captured or checked; live terrain tests pass. | Human art/readability decision at both resolutions; missing Chrono source originals remain a production-art gap. |
| M2-06 audio | Audio palette clip/loop structure and persisted mute/reduced-motion Settings tests pass. | Human listening at normal and 4× cadence, including fatigue, mute and volume; procedural palette is provisional. |
| M2-07 feedback | `RunFeedbackGroupsSkippedTicksAndPrioritizesRecordedHareLosses` passes. Fox hunt and birth cue screens were captured. | Crowded 1×/4× play for truthful, bounded visual/audio feedback and readable suppressed-event summary. |
| M2-08 results | Phase-backed cause and direct Mutation-effect counters pass; cycle, success-fixture and extinction screens were captured. | A player's causal explanation after a legitimate success/failure; non-introductory upgrade effects remain unisolated. |
| M2-09 progression | The fresh earn/purchase/reset/scene-reload/next-launch PlayMode fixture passes, as do migration and settlement tests. | A real process exit/relaunch with owned versus equipped state and use on the next journey. |
| M2-10 onboarding/access | Settings persist mute/reduced motion; map and replay route pass focused checks. Settings and result screens render at both resolutions. | Direct reduced-motion equivalence, first-time keyboard/focus, contrast/readability, and unassisted replay review. |

The EditMode replay-parity test remains skipped because it needs a retained
manifest. Five graphics-only PlayMode skips were exercised in the presentation
graphics and live terrain runs. No human player feedback or real built-player
exit/relaunch was represented by these tests.

## Fixes made during validation

- Corrected the audio test's cross-assembly reflection lookup so Unity compiles.
- Set the board's view model before the root Noesis context in both hosts. This
  removed the inspection binding error that initially failed 64 PlayMode cases.
- Scoped the curated first offer to the Forest Edge journey, preserving the
  older experimental path starters elsewhere. The older five-pick path tests
  now pass while the fresh three-response opening remains available.
- Allowed the wallet test one frame to observe the refreshed results view.
- Captured Settings in the visual fixture, shortened its clipped reduced-motion
  button label, and reduced speed-button padding so `0.5x` is visible.

## Decision boundary and questionnaire

All ten outcomes remain open for human accept/revise decisions. The immediate
route blocker is reproducible authored Hare extinction before the first node
in two fixed-seed checks. A controlled balance or strategy comparison needs a
new exact experiment contract under SG-001. The November 9 approved diagnostic
used an older reproduction Mutation and source, so its approval does not cover
the current three-response comparison. Proposed scope for Josh's approval:
current dirty-source snapshot; Forest Edge 36×20 with 400 Plants, 55 Hares,
20 Foxes, neutral Genome, opposed-roll combat and `bev-experimental`; matched
seeds 10100–10111; no-action control versus one Escape Instinct, Tough Hide,
or Reproductive Drive acquired after phase one, with later choices skipped;
four continuous six-phase nodes with authored nonplayer resets. Report each
arm's completed status, provenance/fingerprints, Hare/Fox/Plant histories,
predator contacts/kills, direct Mutation counters, births, starvation, and
node/terminal outcomes. A complete comparable report and an observed local
effect would support further review; missing provenance, no relevant effect
despite opportunities, or no legitimate route clear would block viability.
The panel is diagnostic, not a production balance approval.

The acceptance questionnaire should ask Josh or a named first-time player to
try the route and record, for each M2 row, **accept / revise / inconclusive**
plus the observed moment or screenshot. Priority questions are: Can the player
state the Fox problem and next action; which of the three responses visibly
changes play and at what cost; can they explain the leading loss and measured
effect; can they read the board/result at both resolutions; do cues tire them
at 4×; can they operate keyboard, mute and reduced motion; can they earn, buy,
exit, relaunch, and use a permanent unlock; and do they want another attempt?
Do not invent the player's answers or close M2 from automated results.

## November 15 addendum — approved M2-04 diagnostic completed

Josh approved the exact current-source, four-arm, 12-seed scope above. The
[factual report](../Research/Experiments/M2-04-Three-Response-20261115/REPORT.md),
[separate analysis](../Research/Experiments/M2-04-Three-Response-20261115/AI_ANALYSIS.md),
and [per-seed metrics](../Research/Experiments/M2-04-Three-Response-20261115/METRICS.json)
record the result. All 48 runs completed with matching fixed inputs, report
hashes, and expected tick-100 acquisitions. Control, Escape Instinct, Tough
Hide, and Reproductive Drive each had **0/12 Hares alive at node-one close**.
The mean first zero-Hare ticks were 444.7, 444.6, 425.8, and 441.3,
respectively. Tough Hide recorded 338 Fox nonlethal hits across its panel;
the other arms recorded zero. CellSim does not export the three direct
Mutation-effect counters, so these outcome counts are not a complete
mechanism audit. The runner continues to tick 2400 after Hare extinction;
first zero-Hare ticks, not report duration, define the player-route outcome.

The experiment directly informs **M2-04 strategy comparison** but does not
close it: none of these one-rank, no-later-action schedules supplies a complete
route, and actual player understanding and tradeoffs are untested. It
reinforces **M2-03** as an implementation/route-validation blocker. It gives
**M2-08** cause counts for review, while direct per-arm contribution telemetry
and a player's explanation remain missing. It does not validate the displayed
or audible **M2-07** feedback. No other M2 row changes acceptance status.

| M2 outcome | Remaining closeout path after this batch |
| --- | --- |
| M2-01 | Real profile exit/relaunch and unassisted constrained-launch explanation; fix only observed defects. |
| M2-02 | Hands-on pointer/zoom/keyboard inspection, focus and actual 1×/4× cadence at both target resolutions; fix any failed behavior. |
| M2-03 | Design and implement a legitimate Hare route with visible Fox pressure, then prove a full player journey on current source. A new ecology/balance experiment needs its own approved SG-001/SG-005 contract. |
| M2-04 | Expose direct effect counts in CellSim, test a design-approved viable route with each response and player-observed tradeoffs; retain fresh-profile availability checks. |
| M2-05 | Obtain or replace the missing Chrono source originals, integrate selected production art, and obtain human runtime art/readability review at both resolutions. |
| M2-06 | Complete the provisional audio language where needed and listen at 1× and 4× for distinctness, fatigue, mute, and volume. |
| M2-07 | Crowded 1×/4× play to judge truthful bounded cues and the suppressed-event summary; fix observed overload or incorrect feedback. |
| M2-08 | Export direct Mutation effects for matched experiments, then ask a player to explain leading loss and measured contribution on a legitimate result; isolate any further claimed upgrade effects. |
| M2-09 | Real process exit/relaunch after earning and buying one permanent unlock; verify owned versus equipped state and use on the next journey. |
| M2-10 | First-time unassisted replay, visible keyboard focus, contrast/readability, mute, and reduced-motion equivalence; fix any failed access path. |

The proposed diagnostic scope in the earlier section is now historical. This
addendum records its execution. Josh's [evidence decision](../Research/Experiments/M2-04-Three-Response-20261115/HUMAN_DECISION.md)
is `Needs Review`; he has not accepted the report or any M2 row.
