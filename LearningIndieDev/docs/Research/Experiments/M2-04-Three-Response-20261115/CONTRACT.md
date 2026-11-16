# M2-04 three-response matched diagnostic — contract

- **Experiment ID:** M2-04-THREE-20261115
- **Decision owner and approval:** Josh explicitly approved the exact 12-seed, four-arm scope in the November 15 Codex chat. This contract records that approval before execution.
- **Question:** Under matched current Forest Edge inputs, does one early Escape Instinct, Tough Hide, or Reproductive Drive Mutation produce its intended measurable local effect, and how do the three arms differ in Hare persistence and route outcome from no intervention?
- **Hypotheses:** Escape Instinct prevents some pre-contact Fox encounters; Tough Hide permits some full-health Hares to survive a base Fox hit; Reproductive Drive changes some otherwise eligible reproduction rolls. These local effects may or may not improve survival through the four-node route.

## Frozen inputs

| Field | Value |
| --- | --- |
| Source | `codex/s6-predator-first` at `f4c1ee34e5174224f2dfe1161083b53839f4129f`, plus the dirty M2 implementation recorded in the validation handoff. The relevant source diff and status will be copied to the artifact bundle before the first run and checked after the last. |
| Scenario | `Assets/Data/ProductionData/CellularSimulation/Scenarios/ForestEdge.asset`, SHA-256 `0ece03f19616942f052b16f07fe171d9bd73663cc1deea26604c75438024f69e`. |
| Ecology | 36×20 closed map; opening 400 Plants, 55 Hares, 20 Foxes; neutral active Genome; opposed-roll combat; `bev-experimental`; ordinary Fox movement scent remains at its default off state. |
| Seeds | 10100–10111 inclusive, paired across all arms. |
| Route window | Four continuous nodes, six 100-tick phases each, 2400-tick horizon. The played Hare population carries across nodes; nonplayer species/resources reset to the authored scenario at each node. A run ends early on Hare extinction. |
| Actions | Control uses no Mutations or later interventions. Each other arm chooses exactly one rank of its named Mutation after phase one (effective phase two) and skips all later choices, shops, and population actions. |
| Arms | `none`; `threat-exposure` (Escape Instinct); `tough-hide`; `reproductive-drive`. The schedule is a mechanics proxy rather than a guarantee that a player would see an offer in a particular seed. Fresh-profile offer access has separate Unity tests. |

## Success, failure, and report contract

Execution succeeds if all four arms complete with reports, the seed list and fixed input/ruleset fingerprints agree, the recorded acquisition effective tick matches the schedule, and all assigned seeds have an explicit terminal status. Report per-seed node reach and terminal tick, Hare/Fox/Plant trajectories, Fox encounters/kills and Hare combat/starvation deaths, births/reproduction opportunities, direct Mutation-effect counters, and missing or invalid telemetry. Compare matched seeds, not only totals.

The local hypotheses are supported only where the corresponding direct counter is positive with a relevant opportunity and the matched control lacks that added effect. A run with no relevant opportunity is inconclusive for that mechanism. Distinct local effects do not by themselves establish four-node viability, player understanding, or a final balance value. No complete Hare route in an arm, absent effect despite opportunities, incomplete reports, or incompatible fixed inputs is a review blocker or comparability failure, not permission to tune the scenario.

Retain exact commands, source/scenario hashes, report paths and hashes, warnings, completed/early-stop statuses, a factual report, and separate AI analysis. Josh's accept/revise decision remains pending and does not follow automatically from this diagnostic.
