# S6 predator-first Hare diagnostic — AI analysis v1

- **Analysis ID:** `S6-PF-DIAG-A1`
- **Source:** [Factual report](REPORT.md) and [per-seed summary](paired-summary.json)
- **Analyst / method / timestamp:** Codex (GPT-6); declared-input, matched-seed interpretation against [S6-P1–P5](../../2026-11-09-S6-predator-first-forecast.md); November 9, 2026. No trained predictive model was used.
- **Decision state:** Recommendation only; Josh has not accepted a balance change.

## Executive summary

The published `feature/s6-fun-pass` Forest Edge scenario already creates a clear early predator problem. Hares suffer 700 combat deaths and 105 starvation deaths across the first 600 ticks of the 12-seed panel; Plants never reach zero in that window. The first node is too hard for the agreed introduction: six seeds lose all Hares before its tick-600 boundary. Seven are extinct by tick 1200, numerically close to the desired roughly 60% second-node failure, but six of those losses happened in node 1. The target shape is therefore absent.

One Mutation at tick 100 changes outcomes, but none yet demonstrates a reliable, easily read answer across this panel. `tough-hide` has the strongest second-node count (5/12 extinct versus 7/12 control) and a lower aggregate Fox kill share per recorded attack (294/619 versus 313/553 after tick 100). Those are different attack populations, so this is a mechanism clue, not a paired attack-level causal estimate. `threat-exposure` reduces recorded Fox attack opportunities (468 versus 553 after tick 100), but its extinction time moves earlier on six seeds and later on four. `larger-minimum-litter` produces fewer Hare births from ticks 101–600 (196 versus 313) and leaves first-node failures at 6/12, rescuing one seed and losing another. The reproduction strategy remains a design goal; this specific mutation and acquisition time have not met its introductory signal requirement.

## Forecast scorecard

| Forecast | Evidence | Assessment |
| --- | --- | --- |
| S6-P1, Fox pressure versus food | Combat deaths dominate the first node; zero Plant extinctions by tick 600. Four control seeds exhaust Plants by tick 1200. | Early predator-first pressure supported. The forecast's *increase Fox pressure* intervention was not run, and later failure has mixed causes. |
| S6-P2, improved Hare persistence may strain food | Four control and four avoidance seeds reach zero Plants by tick 1200; three survival and two reproduction seeds do. | No consistent plant-collapse increase from these candidate arms; the directional concern is unresolved and regime-dependent. |
| S6-P3, avoidance reduces predation | `threat-exposure` is active from phase 2, with fewer Fox attacks and kills after tick 100 (468/280 versus 553/313). Pre-contact avoidance triggers are not counted. | Contact reduction is consistent with the rule, but the predicted per-relevant-encounter effect and route benefit are not established. |
| S6-P4, defensive survival helps attacks | `tough-hide` reduces the aggregate kill share among recorded attacks while attacks increase. Route extinction improves on five seeds, worsens on five, and is unchanged on two. | Local survival signal is plausible; a dependable route strategy is unproven. |
| S6-P5, faster reproduction visibly replaces losses | `larger-minimum-litter` is active from phase 2, but births 101–600 fall from 313 to 196 and first-node failures stay 6/12. | This tested Mutation fails the intended early observable-birth screen. It does not falsify faster reproduction as a strategy. |

## Why the reproduction result needs a different candidate

The authored Hare has a 48-tick gestation and a natural litter range of 2–5; `larger-minimum-litter` raises the minimum to 3. The code fixes litter size when gestation starts and delays birth until the parent survives and enough adjacent birth cells are vacant. That makes a larger litter a possible *capacity* improvement, not necessarily a faster replacement of Hares under heavy early hunting. The panel's birth totals are consistent with that limitation, though the aggregate alone cannot isolate which missed births came from parent deaths, space, food, or changed random trajectories. A shorter gestation or other genuinely faster birth mechanism merits a separate focused test before this is offered as the introductory reproduction answer.

## Player-choice gap

The mechanics arms force a chosen upgrade after tick 100. In the portable offer stream, the exact tested avoidance, survival, and reproduction choices were offered in 4, 5, and 6 of 12 seeds respectively; all three strategy families appeared together on only 3 seeds. The actual fresh-profile path also applies Gene Lab research eligibility. Its current UI can show “Continue without a Mutation” when nothing is researched. A scenario cannot teach three responses if the first decision frequently omits one or more, or if the new player has no unlocked Mutation. The introductory offer/access rule needs a deliberate design decision independently of Genome build paths.

## Next decisions and smallest discriminating work

1. **Josh / scenario balance:** Keep Fox overhunting as the first pressure, but set a first-node safety target explicitly before tuning. Run a new matched-seed contract that varies only starting Fox count (for example 16, 18, and current 20), records first-node combat deaths, Plant stock, and extinction at 600/1200, then validates a promising candidate on disjoint seeds. If lowering the starting count also removes second-node pressure, test a visible later Fox escalation in a separate contract. Do not accept a value from this 12-seed panel alone.
2. **Josh and Sim / Mutation design:** Verify `tough-hide` on a larger paired panel with attack-level telemetry. Add a direct pre-contact avoidance opportunity/success count before claiming that `threat-exposure`'s local action is observable. Test a genuinely faster reproduction candidate against `larger-minimum-litter`, including conception, gestation completion, litter size, and birth-site blocks in the first post-choice node.
3. **Josh / onboarding and offers:** Decide how the first journey grants trial access to Mutations and whether its first offer guarantees one readable option from avoidance, survival, and reproduction. Confirm the resulting choice flow in a fresh-profile Unity playtest. Keep this separate from permanent Genome allocation.

The historical tick-1210 seed 10100 result was recorded on a different, dirty November worktree. The current published checkout's Unity fixture and frozen portable route both fail at tick 236. That source-state difference should be tracked in the handoff; it is not evidence that a balance rule changed during this diagnostic.

**Confidence:** High that current source has severe early Fox pressure and that the 12-seed panel fails the opening-safety goal; medium for the local `tough-hide` and contact signals; low for population-level Mutation rankings or a 60% failure estimate. This diagnostic supports tuning priorities, not a final difficulty or fun claim.
