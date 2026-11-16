# M2 implementation readiness before player validation

[Working state](../WORKING_STATE.md) | Status: in-progress

- Handoff schema: 1
- Handoff ID: 2026-11-15-1907-codex-m2-implementation-readiness
- Owner: Codex
- Branch: `codex/s6-predator-first`
- Baseline commit: `f4c1ee34`
- Date: 2026-11-15, America/Toronto
- Supersedes: none

## Summary

The local implementation audit continues the [November 15 validation batch](2026-11-15-0940-codex-m2-validation-batch.md). It closed the headless direct-effect reporting gap for M2-04/M2-08, wired the already imported Chrono reward icon into journey image resources, and repaired stale test fixtures. The clean EditMode suite now has zero failures. These changes prepare the implemented M2 slices for Josh's evening validation; they do not close any of the ten M2 outcomes.

The central remaining implementation decision is M2-03. The authored Forest Edge/Hare route still loses the played Hares before node one ends in seeds 10100 and 10101. The approved 48-run M2-04 diagnostic found 0/12 Hares at node-one close in each arm. Josh has the [factual report](../Research/Experiments/M2-04-Three-Response-20261115/REPORT.md) under [Needs Review](../Research/Experiments/M2-04-Three-Response-20261115/HUMAN_DECISION.md). No new ecology experiment, scenario value, or balance implementation was approved here.

## Changes

- `SimulationReportSerialization` now emits the existing `MutationEffects` counters (`avoidedContacts`, `decisiveFullHealthHits`, `changedReproductionRolls`) for each closed phase. CellSim run records emit the same totals under schema 42; the PlayMode JSON and Markdown logger emits them under schema 13. The older schema-41 matched reports remain immutable and do not gain these counters retroactively.
- A focused serialization test uses nonzero run and phase-window counters; the schema assertion was updated. A one-seed, two-phase CellSim smoke export confirms the fields exist in actual schema-42 JSON. Its four simulated ticks produced zero effects, so it tests export shape, not an effect-rate claim.
- `reward_v1.png` is now referenced by the semantic `GalapagOS.Image.Journey.Node.Reward` key and the reward-node image. This is a provisional use of an already imported original. The scrolled route capture did not show a reward marker, so appearance of this specific icon remains for player/art review. The separately downloaded `reward_v2.png` remains only in ignored local staging and was not selected.
- Eight previously failing EditMode checks were diagnosed as stale or unsuitable fixtures: reward availability no longer depends on purchasing an unaffordable shop item or an exploding Hare fixture; population-control tests retain scenario data across nodes, compare natural-event counters before/after the paid action, and set the exact shared plant layers at the decision under test; Tough Hide checks current/new Hare health independent of movement; the Warren test checks the new maximum-health rule rather than obsolete block amount. No gameplay rule or authored scenario value was changed for these test repairs.

## Decisions and assumptions

- Josh's answer about the M2-04 factual report was “I need to review it first.” Its human decision stays `Needs Review`; no strategy, balance, or M2 acceptance is inferred.
- The selected reward icon variant remains a human art choice. The source PNG is local and imported; other Chrono originals remain unavailable.

## Validation

| Check | Result | Artifacts |
| --- | --- | --- |
| Full Unity EditMode, Clean, after fixture repairs | 778 passed, 0 failed, 1 skipped / 779 total | [result](../../artifacts/unity-tests-20261115-190046/EditMode-results.xml) |
| Population accounting after a final readability edit | 6 passed, 0 failed | [result](../../artifacts/unity-tests-20261115-190659/EditMode-results.xml) |
| Focused telemetry tests and schema check | 4/4 and 1/1 passed | `artifacts/unity-tests-20261115-151631` and `artifacts/unity-tests-20261115-151801` |
| CellSim actual JSON export | schema 42, 1 seed, 2 phases, run and phase `mutationEffects` objects present; `VALIDATED_WITH_LIMITATIONS` | [report](../../artifacts/cellular-experiment-20261115-184541/report.json) and [manifest](../../artifacts/cellular-experiment-20261115-184541/manifest.json) |
| Journey map visual, Clean, 1280×720 and 1920×1080 | 1/1 passed at each size; map captures inspected | [1280 capture](../../artifacts/visual-evidence-20261115-184809/09-cycle-one-route-choice.png) and [1920 capture](../../artifacts/visual-evidence-20261115-190404/09-cycle-one-route-choice.png) |
| Reward-scrolled map visual, 1280×720 | 1/1 passed; capture has no visible reward marker at that scroll position | [capture](../../artifacts/visual-evidence-20261115-190521/12-map-reward-icon.png) |
| Diff whitespace | `git diff --check` passed; Git emitted only existing LF/CRLF warnings for unrelated authored assets and roadmap HTML | local checkout |
| Handoff lint | Repository-wide lint reports 64 historical handoff issues; this new handoff is not among them | `tools/Test-Handoffs.ps1` |

The preceding [validation batch](2026-11-15-0940-codex-m2-validation-batch.md) remains the PlayMode baseline: 101 passed, 1 failed, 7 skipped, with authored Hare seed 10101 failing before node one. The separate authored 10100 visual also fails. These failures were not made green by altering balance or replacing the route test with the stable fixture.

## M2 closeout state

| Item | Work before acceptance |
| --- | --- |
| M2-01 launch | Real exit/relaunch and unassisted constrained-launch explanation; fix observed defects. |
| M2-02 live UI | Hands-on pointer/zoom/keyboard inspection, focus, and actual 1×/4× cadence at both target resolutions. |
| M2-03 route/roster | Design and implement a legitimately viable Hare route with visible Fox pressure, then prove a full player journey. This is the known implementation blocker and requires a new, separately approved SG-001/SG-005 balance contract before tuning. |
| M2-04 strategies | Direct effect export is implemented. Josh must review the old matched report; a design-approved viable route, new matched comparison with schema-42 direct counters, and player-observed tradeoffs remain. |
| M2-05 art | The reward image is wired provisionally. Missing Chrono UI/animal/latest Hero source originals still need retrieval or an approved replacement and integration, followed by human art/readability review at both sizes. |
| M2-06 audio | Existing procedural palette is provisional; human listening at 1×/4× must decide whether production audio work remains. |
| M2-07 feedback | Crowded 1×/4× player check for truthful bounded cues and suppressed-event summary. |
| M2-08 results | Direct effect export is implemented. Player causal explanation on a legitimate result and isolation of any further claimed upgrade effects remain. |
| M2-09 progression | Real process exit/relaunch after earning and buying an unlock; verify owned/equipped state and next-journey use. |
| M2-10 onboarding/access | First-time unassisted replay, visible keyboard focus, contrast/readability, mute, and reduced-motion equivalence. |

## Risks and incomplete work

- The checkout is shared and dirty. These changes are local, uncommitted, and unpushed; no Trello card was closed or updated. Preserve other contributors' changes and Unity `.meta` files when integrating.
- Chrono originals are still unavailable locally beyond imported `simulation.png` and `reward_v1.png`; direct Trello browser/download attempts stalled or failed. The selected reward variant remains a human art choice.

## Next useful step

Josh can run the planned hands-on validation and record responses against the ten M2 outcomes above. Separately, after reviewing the existing M2-04 report, approve or revise a concrete route-balance experiment contract before any M2-03 ecology tuning. Obtain the missing Chrono originals or explicitly choose replacement assets for M2-05.
