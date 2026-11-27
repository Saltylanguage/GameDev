# Sprint 6 control record — Track A technical foundations

- **Sprint ID:** S6
- **Status:** Active; Track A work began November 16 and formal kickoff was recorded November 27, 2026.
- **Work window:** November 16–29, 2026, inclusive (normal two-week cadence).
- **Milestone:** M2 — Vertical Slice.
- **Product and implementation owner:** Josh for the selected Track A cards. **Technical collaborator:** Sim as available. Remaining estimates and review assignments are open.
- **Primary outcome:** Establish and review the Track A technical foundations needed before the representative Rabbit balance and play review.
- **Capacity:** The normal envelope is about 20 hours per developer over two weeks. Size the selected work against Josh's availability and confirm Sim's time separately if he can assist.

Josh approved this correction to the sprint that was already under way.
[S5 closed November 27](S5-control-record.md#closeout--2026-11-27), after its
October 19–November 15 execution window. S6's November 29 date is a technical
checkpoint and deliberate carry-over decision, not a promise that every Track A
card or the full Rabbit play review will finish. The older [future-sprint
horizon](../FUTURE_SPRINT_ROADMAP.md) is historical.

## Current Track A scope and November 29 checkpoint

| Selected card | Current board state | Evidence and next gate |
| --- | --- | --- |
| [VS-A1](https://trello.com/c/wsmjVY20) | In Progress; moved by Josh November 27 | Reconcile authored wrapping with ponds, rivers, islands and water continuity before integration. |
| [WTR-01](https://trello.com/c/AhPFXox7) | In Progress; carried from S5 | Seeded generator prototype remains isolated; scenario terrain, replay, shore art and in-game checks are not integrated. |
| [VS-A3](https://trello.com/c/wf9cwfhD) | In Progress; moved by Josh November 27 | Prove the smallest authorable Trinket skeleton and dummy behavior; the player flow remains to be decided. |
| [VS-A6](https://trello.com/c/YVqjvxYn) | In Progress | The low-population Rabbit setup and code foundation are published; identity, lineage, events, inspection and player-feel review remain. |
| [VS-A7](https://trello.com/c/IiCOUGOz) | In Progress | The five-species sandbox is published; a full non-Rabbit journey and correct species presentation remain. |

Josh owns the selected cards for now and Sim may assist when available. The
remaining-hour estimates and specific review assignments are still open. Size the next slice
before treating it as a capacity commitment. [VS-A4](https://trello.com/c/IKjreLWW),
[VS-A5](https://trello.com/c/5QZIUMoD), and
[VF-03](https://trello.com/c/rx9JnIcc) are uncommitted Backlog candidates;
Track B visual polish remains deferred. Other M2 outcome tickets remain in
Backlog. The November 29 review records the topology choice or named blocker,
verified prototype state, remaining estimates, review assignments and deliberate
carry-over. The complete Rabbit Fun Pass play review is reforecast after this
technical checkpoint; S7 timing and scope must be reset from the evidence.
M2 remains open. See the [transition handoff](../handoffs/2026-11-27-1436-codex-s5-closeout-s6-track-a-kickoff.md).

## M2 alignment — November 14 vision consolidation

The [ten M2 outcomes](../../ROADMAP.md#m2-outcome-contract--consolidated-2026-11-14)
are the milestone exit contract. The reforecast Rabbit play review concentrates
on the representative journey: clarify the route's legitimate success and failure, make avoidance,
survival, and faster reproduction available and distinct, and show players the
Fox pressure, their choice's observable effect, and the result's leading
causes. Reuse the current menu, map, Field Data, Gene Lab, board, and report
foundations. Track A may surface defects in any of them, but does not claim all ten
outcomes or pull the full presentation, persistence, onboarding, and
accessibility integration gate forward from S7.

At the Rabbit play review, preserve every accepted behavior and every unresolved finding.
Record the exact source, authored route, seed/action schedule, Genome state,
Mutation access, terminal outcome, test results, and player observations. A
controlled fixture that removes Foxes or disables metabolism proves route
machinery only. A failed no-action seed can be intended pressure; do not tune
it solely to turn a test green. The November node-reset contract and carcass
results are current inputs, while the older retained-world sweeps and
Trailblazer/Warren/Gardeners labels remain historical evidence. No production
Fox count, calorie ratio, or final Mutation price is approved by this record.

**November 14 checkout baseline, before this documentation update:** On
`codex/s6-predator-first`, Unity Doctor was healthy and a Clean full suite
completed with EditMode **764 passed / 8 failed / 1 skipped** and PlayMode
**90 passed / 6 failed / 7 skipped**. Five PlayMode failures share an unhandled
`quick-escape` player-description exception; one is the authored seed-10101
route ending at tick 442 before the first shop. The EditMode failures involve
reward continuation, population-control fixtures, Warren snapshot expectations,
and Tough Hide health placement. Triage the controlling code and dated test
assumptions before using these failures to change ecology or to assert M2
readiness. Retained XML/logs: `artifacts/unity-tests-20261114-111056/` (local,
ignored). This baseline is not an S6 kickoff, a balance decision, or player
acceptance.

## November 27 continuity for the selected species

Josh clarified that continuity applies to whichever creature species the
player selects for the journey. A new node places survivors on the seeded
starting tiles with their existing identity and individual state, including
health, energy, age, cooldowns and pending litters. Family relationships
recorded for that species and existing run events retain the same creature IDs.
The new board resets the other species and resources to their authored counts.
Burrow occupants join the survivors on the surface because the burrows belong
to the old board. Tracking targets and scent memories tied to the previous
board are cleared on transfer.

This implements the continuity rule for VS-A6. The player facing event history,
inheritance and goals are separate work.

## November 18 cozy individual-led simulation direction

Josh selected the low-population, character-led direction as the current design
target, accepting the added architecture risk. Fox overhunting remains the
opening pressure, and avoidance, survival, and faster reproduction remain the
species-level responses. Rabbits and Foxes act autonomously; the player makes
indirect choices and inspects animals, tiles, and other entities for useful
state and event information. Direct control of individual animals is out of
scope for this direction.

For the played Hare species, a survivor keeps its identity, individual state,
and event history across the run's simulation nodes. New Rabbits have their own
identities and may inherit a tunable share of parental stats and event-derived
effects. The exact inheritance rule and which effects can be inherited remain
open. This supersedes the November 10 count-only Hare carryover rule as the
design target; Fox and other nonplayed-species persistence is unchanged unless
separately decided.

The Rabbit life cycle is an observable sequence: reach breeding eligibility,
encounter a suitable mate, mate successfully, produce a litter, and see offspring
survive and mature. Rabbit inspection includes contextual goals across three
scopes: individual goals (for example, reach speed 5), lineage goals (at least
five living Rabbits in the lineage), and run goals (for example, acquire the
Burrow Mutation). Goals should guide players toward viable strategies without
requiring one specific Mutation or issuing commands to the animal.

An intelligent event-tracking and presentation system, with useful tile and
animal inspection, is necessary work for this direction. It should connect
events to affected animals, life stages, lineages, goal progress, and visible
outcomes in an engaging and visually appealing way. Its data model, inheritance
details, goal lifecycle, and presentation are still open design work.

This direction supersedes the November 14 deferral of individual-state
persistence and focused inspection/goals as design requirements, and reopens
the earlier exclusion of major simulation and UI work. It does not silently
add implementation tasks to the existing S6 estimate: revise the sprint scope
and capacity before scheduling the work.

## November 19 first-pass Rabbit population ceiling and crowding

Josh set the initial cozy population values to 3 Rabbits and 2 Foxes, with a
hard Rabbit ceiling of 15. The ceiling includes living Rabbits, hidden burrow
occupants, and pending litter reserved for birth. Rabbit population purchases,
starting counts, and reproduction must respect that ceiling. The overall
`maxPopulation` remains disabled so the Rabbit limit does not cap Plants.

For capped species, the crowding metabolism surcharge scales with current living
population divided by the species ceiling. With the current multiplier of 2,
the Rabbit surcharge starts at 20% of its maximum at 3/15 and reaches 100% at
15/15. Uncapped species retain the existing local-group crowding rule. The
first-pass authored setup also uses a 34×19 map and 600 ticks; Rabbit starts
with 24 energy, 2 health, and 9 vision, while Fox attack is 1 and vision is 8.
The Fox energy values remain 48 starting / 96 maximum, with 2 energy lost per
25 ticks and a forage threshold of 72. These are test inputs, not measured
survival or death-rate targets; no balance result has been collected for them.

## November 9 predator-first design decision

Josh selected **Fox overhunting of Hares** as the introductory scenario's primary, immediately understandable pressure. Plant overgrazing may remain a gentle background pressure, but it must not obscure the opening problem or make food renewal the only viable answer. Use the existing Hare/Fox matchup before considering another predator species or terrain system.

The player's three intended **temporary Mutation strategies** are avoidance (notice or escape a hunt), survival (live through an attack), and faster reproduction (replace losses). Each must be an obtainable response in the introductory scenario, produce an observable effect, and have a credible path to success. Mutations are not per-run Genome-tree upgrades: research and equipped Genome state remain separate, although Genome choices may synergize with Mutations. The current fresh-profile offer gate and exact introductory access to these choices need review; this decision does not silently unlock them.

For matched seeds under a defined **no-intervention** schedule, the first simulation node should be very unlikely to fail, roughly 60% of seeds should fail by the end of the second node, and later unassisted success should become rare. The working control skips Mutations and shop purchases and takes no population actions; freeze its Genome state with the experiment inputs. These are proposed design targets, not measured current rates or a universal survival gate. The team must define the seed panel, terminal outcome, and action schedule before using a percentage as evidence. A player should be able to state the Fox problem, predict how a chosen Mutation helps, and see its effect before the next major decision.

This decision supersedes the resource-sustainability candidate in FP-02 and focuses FP-01, FP-03, FP-04, and FP-05 on predator pressure and the three responses. Balance iteration for this introductory scenario should test those hypotheses first; food, population, and stat tuning should support that experience.

## November 11 carcass feeding and Fox energy first pass

Josh approved a shared carcass model for the low-population Hare/Fox matchup:
a lethal attack replaces the living Hare with a meat resource carrying the
Hare's authored energy value. Multiple Foxes may eat from the same carcass;
each takes only enough energy to fill its current stomach, leaving the rest for
another feeder. This makes the death consequential to Fox survival without
removing the Hare from the board and granting all food to the attacking Fox at
once.

For the agreed comparison window, **600 ticks represents 48 hours**. The Hare
currently yields 96 energy. The first-pass Fox values are start energy 48,
maximum energy 96, and upkeep of 2 energy every 25 ticks (48 per node). The
calorie-only prediction is one Hare per two Foxes. Foxes forage below the
effective 72-energy threshold; their current 25% mating threshold and 15% mating
cost fractions remain in force. Fox offspring begin at 48 energy. These are
provisional values and a ledger calculation, not a measured survival ratio.

The portable mechanics check confirms the local energy path: one 96-energy
Hare carcass fills two Foxes from 48 to 96, and both finish a no-reproduction
600-tick fixture alive at 48. The natural ecology result can differ through
encounter timing, missed attacks, reproduction, and spatial access. The
[five-arm, 20-seed-per-arm diagnostic contract](../Research/Experiments/S6-Fox-Carcass-Value-20261111/CONTRACT-DRAFT.md)
was approved by Josh on November 12. The initial panel and the contract's
conditional scale extension completed on November 13. The three 2:1 starting
arms recorded at least one Fox starvation death in 47/100, 82/100, and 100/100
seeds as both populations scaled from 2/1 to 4/2 to 6/3. Hares went extinct in
all 300 of those seeds. The fixed-six-Fox controls remained at 20 seeds each;
the 6/6 arm recorded Fox starvation in 6/20 seeds and retained Hares in 5/20.
This shows the 96/48 energy arithmetic is not itself an ecological survival
ratio. No production ratio or energy value is approved from this diagnostic.
See the [factual report](../Research/Experiments/S6-Fox-Carcass-Value-20261111/REPORT.md)
and [separate AI analysis](../Research/Experiments/S6-Fox-Carcass-Value-20261111/AI_ANALYSIS.md);
Josh's balance decision remains pending.

The separate [15-condition starting-population failure-rate draft](../Research/Experiments/S6-Population-Curve-20261111/CONTRACT-DRAFT.md)
was superseded before execution after Josh clarified the question. Its
generated dry-run plan and pre-carcass snapshot are historical preparation and
must not be reused. The map-size follow-up should be designed after the
carcass-value results identify a useful count region.

## November 10 node-boundary design correction

Josh clarified the population contract for this and future playable species: each simulation node has static, trackable starting populations; only the population count of the species being played carries into the next node. All individuals are recreated with default state. All other species and resources reset to their authored starting populations at the node boundary. For the Hare route, Hares carry over as a count while Foxes and Plants reset. This rule must work with more than two species. Rounds within a node continue normally; the reset happens between simulation nodes.

Before this correction, the Field Data journey and its portable replay retained one world across all nodes, as required by the earlier S5 continuous-world implementation. The [original Fox-start sweep](../Research/Experiments/S6-Fox-Start-Sweep-20261110/REPORT.md) tested that retained-world behavior. Its first-node observations remain relevant, but its node-two balance conclusions and proposed Fox escalation/Plant tuning do not apply to the clarified design; see [analysis v2](../Research/Experiments/S6-Fox-Start-Sweep-20261110/AI_ANALYSIS_V2.md). This correction supersedes the cross-node retained-world assumption for future S6 work, not the immutable evidence of the earlier run.

**Implementation and corrected measurement, November 10:** The Field Data and legacy journey transitions now create a fresh grid at each simulation node, carrying only the played species' count and restoring fixed nonplayed counts. The portable worker uses the same transition and records reset deltas at the boundary. Focused Unity route tests and Unity-to-portable parity passed. The [revised Fox-count sweep](../Research/Experiments/S6-Fox-Node-Reset-20261110/REPORT.md) completed 336/336 runs with every node opening validated. On the 100-seed confirmation panel, first-node Hare failures were 19%, 15%, and 20% for 16, 18, and 20 Foxes; cumulative node-two failures were 55%, 51%, and 61%. All three miss the opening safety screen despite meeting the node-two pressure band. No authored balance value changed; see the [separate analysis](../Research/Experiments/S6-Fox-Node-Reset-20261110/AI_ANALYSIS.md) before choosing the next experiment or tuning.

**Lower-count diagnostic, November 10:** A follow-up [12-seed-per-arm screen](../Research/Experiments/S6-Fox-Soft-Opening-20261110/REPORT.md) tested 8, 10, and 12 fixed Foxes per node. Each arm had 2/12 Hare failures by node one and 4/2/3 cumulative failures by node two. The intervals are wide, and no arm met the preregistered trigger for a 100-seed confirmation. One 10-Fox run depleted Plants by node-two close. This does not select a balance value; the separate [analysis](../Research/Experiments/S6-Fox-Soft-Opening-20261110/AI_ANALYSIS.md) recommends evaluating distinct static Fox counts per node if a gentler opening and stronger node-two pressure remain the target.

## November 9 evidence and integration boundary

The current [GLJP-06 record](https://trello.com/c/eo0iqoRL/154-gljp-06-diagnose-route-seed-survival-mismatch-before-further-development) reports the November authored seed-10100 route ending at tick 1210, before shop 3 at tick 1800. Plants reached zero after the last was consumed at tick 915; the final Hare's recorded death was combat. A separate stable-Hare fixture traversed four simulation nodes and three shops through successful results at tick 2400. That fixture disables reproduction and metabolism and has no Foxes, so it verifies route machinery, not authored ecology or fun.

Bevin's November 9 direction makes the **November implementation canonical for current investigation**. October comparisons remain historical; the current runner rejects the October snapshot's source hash, so those inputs cannot be called a matched comparison. The cause of the authored resource collapse and the requirement for a no-action run to survive are still undecided. A failure without intervention is not automatically a defect.

At the original plan publication, `ProjectMain` was at October 29 commit `4f0ace88` and did not include the November GLJP work described on Trello. That publication-time note is superseded for the S6 feature branch by the merge update below. Do not copy historical tuning into the November build.

**November 9 merge update.** `feature/s6-fun-pass` fast-forwarded to `BevBranch` tip `a2c38b33`, so the November GLJP source and handoffs are now in the S6 feature branch. The [GLJP handoff](../handoffs/2026-11-09-gljp-salty-handoff.md) records the Unity results and remaining design and route decisions. Raw test artifacts remain local to the original worktree unless separately published; recheck source and input identity before repeatable comparisons.

Other open work at the boundary:

- [GLJP-01](https://trello.com/c/dR2z6ENf/149-gljp-01-replace-hare-genome-stat-nodes-with-researched-journey-unlocks): capability changes have focused validation; minimum-litter rank cap, Faster Gestation effect, prices/prerequisites, and player acceptance remain open.
- [GLJP-04](https://trello.com/c/DQwFKLSF/152-gljp-04-implement-burrowing-family-hiding-and-burrow-upgrades): Burrowing and Born Burrower have November focused checks; final prices and player review remain open.
- [GLJP-05](https://trello.com/c/Htt7PJBY/153-gljp-05-integrate-gene-lab-and-journey-upgrade-ui-tests-and-captures): November UI and route checks exist; player review and catalog decisions remain open. Its full PlayMode rerun still reports the authored seed-10100 failure under GLJP-06.
- [WTR-01](https://trello.com/c/AhPFXox7/136-wtr-01-procedural-water-maps-and-gameplay-shorelines): isolated procedural-water work remains separate from the S6 playable outcome.

These are November 9 evidence statements, not current card status. At the
November 27 transition, GLJP-01 through GLJP-06 were Done, WTR-01 carried into
S6, and ProjectMain was verified at `49c81f91`; see the current control above.

## Reforecast Fun Pass work packages, in order

### FP-01 — Agree on the route-survival contract

Use the November authored scenario and seed to distinguish intentional Fox pressure from an invalid setup, regression, or unintended extinction. Decide how to keep plant depletion secondary during the introductory challenge. Record the scenario, source revision, seed, action schedule, expected player-facing behavior, and evidence limits. Preserve the November plant-consumption finding and do not infer a survival rate from the stable fixture or one authored seed.

**Done when:** Josh and Sim record the intended survival/failure behavior and identify the smallest representative route to play. Any unresolved cause remains named rather than hidden by tuning.

### FP-02 — Make three Mutation responses viable

Test avoidance, survival, and faster reproduction against Fox overhunting. Use legitimate temporary Mutation choices and explain each response's hypothesis, eligibility, expected visible effect, and tradeoff. The introductory survival response must not depend on Burrowing or terrain safety. Reconcile the fresh-profile research gate before calling any choice obtainable by a new player. Genome loadouts may provide synergy but do not define or gate the three Mutation strategies.

**Done when:** All three responses can be tried on the agreed route and their differences can be described from observed play and evidence.

### FP-03 — Make comparisons cheap and reproducible

Use the existing deterministic runner and reports. Begin with a small diagnostic set of matched **November** seeds and explicit player-action schedules, including a no-intervention control and one response at a time. Record source/scenario/ruleset/seed/loadout identity, node reach and terminal tick, Hare/Fox/Plant trajectories, Fox encounters and kills, avoidance and blocked attacks, births and replacement timing, feeding/starvation, and Mutation acquisition timing. Report missing telemetry or incompatible inputs explicitly. Expand the seed set only for a named hypothesis.

**Done when:** A same-input comparison can be repeated, important divergences are visible without manual log trawling, and the report does not confuse a control failure with a design verdict. Follow [SG-005](../Studio%20Guidelines/SG-005-UPGRADE-AND-ECOLOGY-BALANCE.md) and the [report protocol](../Studio%20Guidelines/AI_GENERATED_REPORTS.md) for new experiments.

### FP-04 — Improve player-facing feedback

Use existing journey, Mutation, shop, and results surfaces to show Fox hunting, the immediate effect of avoidance or survival, births replacing losses, recovery or deterioration, and meaningful tradeoffs. Keep plant-pressure feedback available without making it compete with the introductory problem. Prefer the few signals players need during the route over a new presentation system or event flood.

**Done when:** A player can recognize the major pressure, connect at least one choice to its outcome, and understand the run's result without developer diagnostics.

### FP-05 — Play, observe, decide

Play representative complete journeys using the three Mutation responses. For each, record the most engaging and confusing moments, most consequential choice, fairness of failure, and what the player wants to try next. Separate observed behavior from interpretation and combine the play notes with the controlled comparisons.

**Done when:** At least one complete legitimate Hare journey and representative evaluations of all three responses are recorded, with an explicit accept/revise decision and one prioritized next iteration. Automated tests alone do not establish this gate. Record which of the ten M2 outcomes this evidence supports and hand remaining integration and external-player checks to S7.

## Balance loop and scope boundary

For each change: state one gameplay hypothesis, predict its observable effect, run a controlled comparison, inspect the difference, play representative cases, then retain, revise, or reject. Favor authored scenario constraints and distinct decisions over broad global number tuning. A no-action failure may be intentional; its meaning follows FP-01.

The original Fun Pass scope excluded new biomes/species, procedural water and
major simulation/UI work. The November Track A direction supersedes that sprint
boundary for the selected technical foundations above. It does not approve a
broad ecology rebalance, final species content, or an unestimated completion
commitment. Revisit the playable route after the foundation review.

## Exit gate and handoff to design

The reforecast Rabbit play-review gate passes only if the team can demonstrate
a complete Rabbit journey using legitimate decisions; three visibly different
approaches; at least one meaningful tradeoff; recognizable causes of
deterioration and recovery; a repeatable low-effort comparison; and playtest
feedback that identifies both friction and a reason to replay. If the
fun/replay gate fails, record a prioritized revision plan instead of expanding
content. This gate is not claimed by the S6 Track A checkpoint.

Before the reforecast play review, the design discussion should decide the
Rabbit journey's beginning/middle/end emotional arc, the most exciting player
decision, fair failure, feedback and spectacle, and the smallest experiment
set. S5 closeout and current source were verified at the November 27
transition. Task estimates, review assignments, and the exact playable route
remain open. GLJP cards are Done from their separate November closeout.
