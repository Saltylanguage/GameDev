# S4-03 strategy-choice policy screen — 600/40/25 wrapped

**Status: Executed locally on 2026-10-06; Bevin approved the protocol before the run. Human balance review pending.**

## Decision and question

Bevin set a rough early-node clear-rate target of **40–50%** on 2026-10-06. This draft applies that target to each of the three intended Hare builds. The question is whether an offer-aware player who follows Trailblazer, Warren, or Gardeners choices can reach roughly that clear rate on the existing Forest Edge challenge.

This is a node-clear challenge, not a test of the later journey map or stable long-term ecology. The clear rule is unchanged: reach tick 600 with Plants > 0, Hares >= 5, and Foxes >= 5. Exactly five Hares or Foxes qualifies.

## Hypothesis

Using all five available Mutation decisions and choosing only from the actual three-card offer should produce at least one recognizable strategy near the 40–50% target. The test will also show whether all three paths are comparably viable under the same challenge.

## Method

- Four matched arms: skip-all control, Trailblazer, Warren, and Gardeners.
- 200 fresh common seeds per arm, proposed range 14200–14399; report matched clears as well as each arm's total clear rate and 95% Wilson interval.
- Production Forest Edge, 36x20, Wrap Edges on, 600 Plants / 40 Hares / 25 Foxes, Hare player, natural opposed-roll combat, 0.1-second steps, six continuous 100-tick rounds, and five choices after ticks 100, 200, 300, 400, and 500.
- Use the existing `bev-experimental` bundle so Threat Avoidance is legal and the human-readable Hare/Fox stat lines are emitted; keep coupled species responses off. Add no Genome or population reinforcements, and make no simulation value changes. Reuse the current checked-out rules only after recording exact source and scenario hashes.

## Proposed offer-aware policy

At each decision, generate the offer with the same `SpeciesUpgradeCatalog.CreateExperimentalHerbivoreMutationOffer` code used by the phase UI, using that run's seed, offer rotation, last selected skill, and current legal-level filter. Never apply a skill that was not offered. If neither target skill is offered, choose Skip. Skip advances offer rotation but does not change the last selected skill or Mutation levels.

| Build | Strategy skills | Tie-break / first-pick order |
| --- | --- | --- |
| Trailblazer | `faster-movement`, `threat-exposure` (player-facing Threat Avoidance) | `faster-movement` first, then alternate toward the lower-level skill |
| Warren | `tough-hide`, `crowding-tolerance` | `tough-hide` first, then alternate toward the lower-level skill |
| Gardeners | `efficient-digestion`, `seed-dispersal` | `efficient-digestion` first, then alternate toward the lower-level skill |

The route priorities follow the accepted five-pick paths in the [S4-01 handoff](../../../handoffs/2026-10-01-1711-codex-s4-01-hare-workbook-review.md). A selected route skill should cause the normal paired skill to appear in later offers where the production offer generator permits it. Log every offered card, selected card or Skip, current skill levels, and reason. This policy follows a build identity; it does not react to observed population, slashlines, or perceived threat, so it is not a prediction of adaptive human play.

## Measures and review rule

Primary measure: all-species clear rate at tick 600, with matched seed outcomes and uncertainty. Review each strategy against the 40–50% rough target; treat a rate above the band as possible over-strength and a rate below it as a tuning candidate, not as automatic approval/rejection. With 200 runs per strategy, intervals may span the target band; report that uncertainty plainly.

Also report Hares/Foxes/Plants at phase boundaries and at finish, all available Hare slashline metrics with validity and denominators, FPO reconciliation, mutation acquisition/offer history, seed-dispersal attribution, births and death causes, completed-to-horizon counts, and matched wins/losses versus skip-all. Do not call this a stable ecology or use one composite score as the decision.

## Execution and audit contract

Retain full per-seed run histories and offer/choice audit data under ignored `artifacts/`; validate seeds, inputs, all five choice windows, applied offered choices, final/phase population accounting, slashline formulas and statuses, and source/scenario hashes. Publish a factual `REPORT.md`, a separate evidence-linked `ANALYSIS.md`, and a handoff. Do not tune values from this screen; use the result to select a narrowly scoped follow-up and human review.

## Approval record and open boundaries

Bevin approved this protocol on 2026-10-06. The approved target was a rough 40–50% clear rate per strategy and the main panel was 200 matched seeds per arm at the specified setup. The policy was validated on diagnostic seeds, then the matched panel was run and independently audited. Results are in [REPORT.md](REPORT.md), with interpretation in [ANALYSIS.md](ANALYSIS.md) and execution context in the [handoff](../../../handoffs/2026-10-06-2326-codex-s4-03-strategy-choice-policy-screen.md). No balance values were tuned from this screen; use it to choose a narrowly scoped follow-up for human review.

This screen tested the currently implemented experimental Hare offer pool. It did not implement or test the broader journey progression, new Fox mutations, balancing changes, or population/default changes.
