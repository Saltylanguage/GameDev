# Sprint 6 control record — The Fun Pass

- **Sprint ID:** S6
- **Status:** Planned; direction approved November 9, 2026. Kickoff and task allocation are pending.
- **Work window:** November 16–29, 2026, inclusive (normal two-week cadence).
- **Milestone:** M2 — Vertical Slice.
- **Product owner:** Josh. **Technical collaborator/reviewer:** Sim. Assign implementation owners at kickoff.
- **Primary outcome:** Demonstrate and evaluate one complete Hare journey with consequential choices, readable ecological pressure, distinct strategies, and a reason to replay.
- **Capacity:** Reconfirm availability and estimate the selected tasks at kickoff. The normal planning envelope is about 20 hours per developer over two weeks; this plan does not pre-commit all five work packages or borrow unestimated time.

This is the approved next-sprint direction and execution gate, not a claim that S6 has started or that the gameplay exit gate has passed. [S5 remains active through November 15](S5-control-record.md); its remaining GLJP work and closeout are not silently moved or closed by this plan. The older [future-sprint horizon](../FUTURE_SPRINT_ROADMAP.md) is historical; this record and the [active roadmap](../../ROADMAP.md) supersede its S6 persistence/Genome forecast.

## November 9 evidence and integration boundary

The current [GLJP-06 record](https://trello.com/c/eo0iqoRL/154-gljp-06-diagnose-route-seed-survival-mismatch-before-further-development) reports the November authored seed-10100 route ending at tick 1210, before shop 3 at tick 1800. Plants reached zero after the last was consumed at tick 915; the final Hare's recorded death was combat. A separate stable-Hare fixture traversed four simulation nodes and three shops through successful results at tick 2400. That fixture disables reproduction and metabolism and has no Foxes, so it verifies route machinery, not authored ecology or fun.

Bevin's November 9 direction makes the **November implementation canonical for current investigation**. October comparisons remain historical; the current runner rejects the October snapshot's source hash, so those inputs cannot be called a matched comparison. The cause of the authored resource collapse and the requirement for a no-action run to survive are still undecided. A failure without intervention is not automatically a defect.

As of this plan, `ProjectMain` is at October 29 commit `4f0ace88`; its published documentation and code do not include the November GLJP work described on Trello. Sim's November handoffs and artifacts named on the cards are local/unmerged evidence until their source, inputs, and publication state are verified. The new S6 branch must reconcile that work before implementation or repeatable comparisons. Do not copy historical tuning into the November build or claim the local evidence is present on `ProjectMain`.

**November 9 handoff update.** The November GLJP implementation is available on `BevBranch` in `de26d3b`, with the current Sprint 6 plan incorporated into that branch. The [GLJP handoff](../handoffs/2026-11-09-gljp-salty-handoff.md) records the Unity results and the remaining design and route decisions. The Sprint 6 working branch still needs to take that implementation.

Other open work at the boundary:

- [GLJP-01](https://trello.com/c/dR2z6ENf/149-gljp-01-replace-hare-genome-stat-nodes-with-researched-journey-unlocks): capability changes have focused validation; minimum-litter rank cap, Faster Gestation effect, prices/prerequisites, and player acceptance remain open.
- [GLJP-04](https://trello.com/c/DQwFKLSF/152-gljp-04-implement-burrowing-family-hiding-and-burrow-upgrades): Burrowing and Born Burrower have November focused checks; final prices and player review remain open.
- [GLJP-05](https://trello.com/c/Htt7PJBY/153-gljp-05-integrate-gene-lab-and-journey-upgrade-ui-tests-and-captures): November UI and route checks exist; player review and catalog decisions remain open. Its full PlayMode rerun still reports the authored seed-10100 failure under GLJP-06.
- [WTR-01](https://trello.com/c/AhPFXox7/136-wtr-01-procedural-water-maps-and-gameplay-shorelines): isolated procedural-water work remains separate from the S6 playable outcome.

These are dated evidence statements, not S5 closeout or acceptance decisions. Recheck the cards and source at kickoff.

## Work packages, in order

### FP-01 — Agree on the route-survival contract

Use the November authored scenario and seed to distinguish intentional pressure from an invalid setup, regression, or unintended extinction. Decide whether a selected journey requires player intervention to survive. Record the scenario, source revision, seed, action schedule, expected player-facing behavior, and evidence limits. Preserve the November plant-consumption finding and do not infer a universal survival target from the stable fixture.

**Done when:** Josh and Sim record the intended survival/failure behavior and identify the smallest representative route to play. Any unresolved cause remains named rather than hidden by tuning.

### FP-02 — Select three contrasting Hare builds

Test reproduction, defensive survival (including Burrowing where obtainable), and resource sustainability as candidate approaches. Use available, legitimate player choices and explain each build's hypothesis, obtainable path, expected effect, and tradeoff. Replace an archetype if the current catalog cannot support it; do not create a major subsystem just to fill a slot.

**Done when:** Three builds can be tried on the agreed route and their differences can be described from observed play and evidence.

### FP-03 — Make comparisons cheap and reproducible

Use the existing deterministic runner and reports. Begin with a small diagnostic set of matched **November** seeds and explicit player-action schedules, including a no-action control. Record source/scenario/ruleset/seed/loadout identity, completion or failure and terminal tick, Hare/Fox/Plant trajectories, feeding/starvation/predation/reproduction, and the effects of selected upgrades. Report missing telemetry or incompatible inputs explicitly. Expand the seed set only for a named hypothesis.

**Done when:** A same-input comparison can be repeated, important divergences are visible without manual log trawling, and the report does not confuse a control failure with a design verdict. Follow [SG-005](../Studio%20Guidelines/SG-005-UPGRADE-AND-ECOLOGY-BALANCE.md) and the [report protocol](../Studio%20Guidelines/AI_GENERATED_REPORTS.md) for new experiments.

### FP-04 — Improve player-facing feedback

Use existing journey, Mutation, shop, and results surfaces to show resource shortage, predator pressure, intervention effects, recovery or deterioration, and meaningful tradeoffs. Prefer the few signals players need during the route over a new presentation system or event flood.

**Done when:** A player can recognize the major pressure, connect at least one choice to its outcome, and understand the run's result without developer diagnostics.

### FP-05 — Play, observe, decide

Play representative complete journeys using the selected builds. For each, record the most engaging and confusing moments, most consequential choice, fairness of failure, and what the player wants to try next. Separate observed behavior from interpretation and combine the play notes with the controlled comparisons.

**Done when:** At least one complete legitimate Hare journey and representative evaluations of all three builds are recorded, with an explicit accept/revise decision and one prioritized next iteration. Automated tests alone do not establish this gate.

## Balance loop and scope boundary

For each change: state one gameplay hypothesis, predict its observable effect, run a controlled comparison, inspect the difference, play representative cases, then retain, revise, or reject. Favor authored scenario constraints and distinct decisions over broad global number tuning. A no-action failure may be intentional; its meaning follows FP-01.

The S6 primary scope excludes new biomes/species, unrelated Genome branches, procedural-water integration without a demonstrated dependency, comprehensive ecology rebalance, a large UI redesign, new economies, major simulation rewrites, and an optimization framework. A blocker may enter only with evidence that it directly prevents the playable outcome and a capacity decision at kickoff or review.

## Exit gate and handoff to design

The sprint passes only if the team can demonstrate a complete Hare journey using legitimate decisions; three visibly different approaches; at least one meaningful tradeoff; recognizable causes of deterioration and recovery; a repeatable low-effort comparison; and playtest feedback that identifies both friction and a reason to replay. If the fun/replay gate fails, record a prioritized revision plan instead of expanding content.

Before gameplay implementation, the design discussion should decide the Hare journey's beginning/middle/end emotional arc, the most exciting player decision, fair failure, feedback and spectacle, and the smallest experiment set. Kickoff must also verify S5 closeout, current source and November evidence, owner availability, task estimates, and the exact playable route. No GLJP ticket is closed by publishing this plan.
