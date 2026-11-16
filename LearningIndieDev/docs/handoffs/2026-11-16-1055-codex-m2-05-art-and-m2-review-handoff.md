# M2-05 art and M2 review handoff

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-11-16-1055-codex-m2-05-art-and-m2-review-handoff
- Owner: Codex
- Branch: `codex/s6-predator-first`
- Baseline commit: `f4c1ee34`
- Date: 2026-11-16
- Supersedes: none

## Summary

The M2-05 production-art gap is prepared for Josh and Sim's runtime review. Chrono's latest Hero and Rabbit originals now appear on the existing desktop/Main Menu and Hare board paths. Their Unity GUIDs and Noesis resource keys did not change. All other missing UI and animal originals are included in a [small review set](../Art/ChronoReview-20261116/README.md), without inventing additional UI roles or playable species. M2-05 remains open until human art/readability acceptance at both target resolutions.

This handoff also packages the current M2 implementation and [November 15 validation evidence](2026-11-15-1907-codex-m2-implementation-readiness.md) for Sim's review. It records known blockers and decisions separately; it does not close any M2 outcome.

## Changes

- Retrieved the original 64×64 PNGs from the [UI Icons card](https://trello.com/c/32Y2b0C2/51-ui-icons) and [Animal Icons card](https://trello.com/c/KWadDn89/40-animal-icons), plus the latest 1920×1080 Hero from the [Backgrounds card](https://trello.com/c/0uBTZQMI/39-backgrounds-wallpapers). Local copy sizes and SHA-256 hashes match the downloaded originals. The Fox original is byte-identical to the existing source and was not duplicated.
- Replaced only the image bytes at `Assets/UI/GalapagOS/Art/Journey/WoodlandSummer.png` and `Assets/Art/Species/Animals/Chrono_Rabbit_Tile_Face.png`, preserving their `.meta` files and GUIDs. `ChronoSpeciesArt.Rebuild()` regenerated the 32, 64, 128, and legacy 1024 Rabbit exports through the established editor pipeline. It also rewrote the Fox exports to identical bytes; they have no Git diff.
- Added the thirteen as-yet-unassigned UI and Deer/Owl/Wolf originals under `docs/Art/ChronoReview-20261116/` for joint review. These are source candidates, not imported Unity assets. Existing `simulation.png` and `reward_v1.png` remain the runtime journey-node icons through `Assets/UI/ImageResources.xaml`.
- The other local M2 code, data, tests, reports, and handoffs on this branch are part of the same shareable review package. They are summarized in the [implementation readiness handoff](2026-11-15-1907-codex-m2-implementation-readiness.md) and [validation batch](2026-11-15-0940-codex-m2-validation-batch.md).

## Decisions and assumptions

- The green/blue `reward_v1.png` remains the current runtime candidate. The red/blue `reward_v2.png` is available beside the other originals. The delivered final-challenge files are frames without a clear finale symbol, so the current map star remains. Any further icon mapping is a design choice for Josh and Sim.
- The latest Hero and Rabbit are review candidates integrated into the runtime, not accepted final art. Terrain art and a playable Deer/Owl/Wolf roster were not added.
- Josh's M2-04 report decision remains **Needs Review**. The [factual report](../Research/Experiments/M2-04-Three-Response-20261115/REPORT.md) and [exact metrics](../Research/Experiments/M2-04-Three-Response-20261115/METRICS.json) are shareable; the four raw experiment bundles total about 300 MB under ignored `artifacts/` and are local to this checkout. Acceptance of the report would acknowledge its factual record only. It would not approve balance values or close M2-03/M2-04.

## Validation

| Check | Observed result |
| --- | --- |
| Unity `ChronoSpeciesArt.Rebuild()` with host permissions | Completed; Rabbit 32/64/128/1024 exports and atlas import settings processed, no `.meta` diff. Unity's licensing log included recoverable entitlement lookup messages, but the editor method completed with exit 0. |
| Focused `GalapagOSDesktopAndSimulationCaptureGameViewEvidence`, Clean, 1280×720 | 1 passed, 0 failed. Inspected [desktop](../../artifacts/visual-evidence-20261116-105355/01-galapagos-desktop-home.png) and [live board](../../artifacts/visual-evidence-20261116-105355/03-galapagos-simulation.png) captures. |
| Same test, Clean, 1920×1080 | 1 passed, 0 failed. Inspected [desktop](../../artifacts/visual-evidence-20261116-105446/01-galapagos-desktop-home.png) and [live board](../../artifacts/visual-evidence-20261116-105446/03-galapagos-simulation.png) captures. |
| Previous full EditMode suite after M2 implementation | 778 passed, 0 failed, 1 skipped, as recorded in the [readiness handoff](2026-11-15-1907-codex-m2-implementation-readiness.md). |
| Previous full PlayMode suite | 101 passed, 1 failed, 7 skipped. The authored Hare route fails before node one; it was not changed by the art import. |
| Staged whitespace check | All code, data, XAML, and documentation paths pass. Strict `git diff --cached --check` flags trailing spaces in empty fields of the two Unity-generated new icon `.meta` files; those serialized files were left unedited to preserve Unity's output. |

The focused visual test captures the Desktop and active simulation board at both resolutions. It does not independently capture the legacy Main Menu or prove subjective readability, comfort, or art approval.

## M2 review agenda

| Item | Review or work still required |
| --- | --- |
| M2-01 launch | Josh's real exit/relaunch and unassisted constrained-launch explanation. |
| M2-02 live UI | Hands-on pointer, zoom, keyboard focus, and 1×/4× inspection at both resolutions. |
| M2-03 route/roster | **Known implementation blocker:** authored Hare route dies before node one on tested seeds. Agree a concrete SG-001/SG-005 balance contract, implement it, then prove a legitimate full journey with visible Fox pressure. No balance value is approved yet. |
| M2-04 strategies | Josh reviews the 48-run factual report. After a viable route is agreed, run a new matched comparison with schema-42 direct-effect counters and assess player-observed tradeoffs. |
| M2-05 art | Josh and Sim compare latest/previous Hero and Rabbit, reward v1/v2, and applicable review-set icons; accept or request a specific change after runtime review at both sizes. |
| M2-06 audio | Listen at 1×/4× and decide whether the provisional procedural palette needs production work. |
| M2-07 feedback | Crowded 1×/4× player check for truthful bounded cues and the suppressed-event summary. |
| M2-08 results | Explain observed causes on a legitimate result and isolate any further claimed Mutation effects. |
| M2-09 progression | Real process exit/relaunch after an earned and purchased unlock; verify owned/equipped state on the next journey. |
| M2-10 onboarding/access | Unassisted first-time replay, focus, contrast, mute, and reduced-motion equivalence. |

## Risks and incomplete work

- M2-03 is the known production implementation blocker; M2-04 and M2-08 depend on a legitimate route. Human visual, playability, audio, and accessibility acceptance remains open for all applicable items.
- The raw M2-04 experiment bundles and today's visual screenshots are ignored local artifacts. Sim receives the committed factual report, seed metrics, code, and this handoff, but will need a separate transfer or rerun to inspect raw bundles/screenshots outside this machine.
- A temporary `Assets/InitTestScene*.unity` pair was produced by Unity and is excluded from the shared commit.

## Next useful step

Josh can share the pushed branch and this handoff with Sim, review the M2-04 factual report, and run the planned acceptance questionnaire. Record explicit art and route decisions from the meeting before changing balance values or marking M2 items complete.
