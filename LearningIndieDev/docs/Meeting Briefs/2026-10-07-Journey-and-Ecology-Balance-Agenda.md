# October 7: journey and ecology balance

Prepared by Codex for Bevin before the team meeting. These are discussion notes; unchecked decisions and proposed assignments are not team approval.

**Meeting completed:** Bevin supplied the decisions below on October 7. They supersede the agenda's proposals where they differ. The original agenda and experiment results are retained as historical context.

## Meeting decisions

1. **Success:** survive to tick 600 with an ecology viable enough to survive to tick 700. The exact meaning of viability remains to be defined; no new population threshold, prediction formula, or runtime gate is approved yet.
2. **Upgrade continuity:** upgrades acquired during a run persist into the next simulation run in the journey. This approves carryover between successive journey simulations. It does not decide persistence into a newly started journey or permanent account progression.
3. **Population and event pacing:** retain 600 Plants / 40 Hares / 25 Foxes as the historical research fixture. Discover a new starting population that makes kills, births, and other events happen at a more personal pace. No replacement population is selected yet; the old fixture's outcomes are not validation of the new pacing goal.
4. **Ownership:** leave the next ecology balance and population-search work to SimMasterBev.
5. **Deferred:** do not pursue the DNA reward economy / secondary long-running simulator at this point.
6. **Shared context:** add meeting updates to Trello comments, preserving existing card descriptions and historical evidence.

The next definition needed is the tick-700 viability criterion: which species must remain viable, what floors or recovery behavior count, whether it is measured by an actual 100-tick continuation or estimated at tick 600, and what interventions or next-node conditions apply during that window. These are open questions, not adopted mechanics. The earlier Plants > 0 / Hares >= 5 / Foxes >= 5 rule remains the label for historical six-phase reports, not a complete definition of this new goal.

See the [meeting handoff](../handoffs/2026-10-07-2239-codex-journey-meeting-decisions-and-population-pacing.md) for ownership and the next work boundary.

## Opening statement

"I've pushed our accumulated S4 implementation and balance research to BevBranch, merged your latest journey work, and pushed that too. The focused integration checks passed. The map gives us the longer-run structure we wanted; the ecology still needs work. I'd like us to agree what counts as clearing a node and which upgrades persist, then choose one controlled balance experiment."

## What is shared now

- S4 checkpoint: `64a8e3b5` - optional grid wrapping, deterministic Threat Avoidance fixes, population override handling, runner improvements, focused tests, and seven research packages.
- Integrated Salty's ProjectMain `fea55926` in merge `01449281`. One Working State document conflict was resolved by preserving both entries; the shared simulation preview merged automatically.
- Fresh merged-code validation: 158 focused EditMode tests passed; one PlayMode test passed with wrapping enabled through the same-biome journey transition. Five changed XAML files and both changed PowerShell scripts parsed successfully. No fresh graphics review, full suite, player build, or journey balance batch.
- Start with [Working State](../WORKING_STATE.md), the [publication/integration handoff](../handoffs/2026-10-07-2049-codex-s4-balance-publication.md), and [Salty's current journey contract](../JOURNEY_PROTOTYPE.md). Raw experiment JSON/logs are local ignored artifacts; protocols, reports, and interpretations are in Git.

## Proposed discussion order (25 minutes)

### 1. Confirm the playable journey and upgrade rule (5 minutes)

Salty's current prototype begins with First Discovery, then six simulation phases, a report/map transition, a Seedfall or Fox Tracks condition, and another six phases in the same ecosystem. The current loadout persists across cycles. Six of eighteen authored nodes are playable; later nodes remain placeholders.

- Should phase Mutations persist through the journey, reset at each simulation, or carry through a limited inheritance choice?
- What is the name for an upgrade that persists within one journey, and how does it differ from a Genome upgrade between runs?
- Does the opening reward become part of the intended balance contract? Our latest offer-aware study starts its five choices at ticks 100-500; it does not model this opening reward and twelve-phase route.

**Decision to record:** temporary prototype behavior we keep for now, and the intended persistence rule for the next implementation slice.

### 2. Define ecological success and difficulty (5 minutes)

Bevin's current direction is to keep all species present, with fewer than five Hares or Foxes treated as failure. The research clear rule is completion at tick 600 with Plants > 0, Hares >= 5, and Foxes >= 5. Bevin wants collapse pressure without player intervention and approximately 40-50% clears with useful intervention.

- Is 40-50% the target for the first node, a selected strategy, a typical player, or the entire journey? It must not silently become all four.
- Do we check ecological viability only at the node boundary, or also during a node? A temporary dip and an end-state failure are different rules.
- Is Plants > 0 enough, or should plant abundance/recovery have a viability floor too?
- At a failed ecological gate, does the journey end, offer a recovery choice, or allow a voluntary cash-out?

**Decision to record:** one exact node gate and one difficulty target with its scope. Keep experimental success distinct from the current runtime's completion/report flow until reconciled.

### 3. Review the balance evidence and its tradeoff (7 minutes)

Latest comparable policy screen: wrapped Forest Edge, 600 Plants / 40 Hares / 25 Foxes, 36x20, six 100-tick phases, coupled responses off, no reinforcements or added Genomes, 200 shared seeds per arm. Policies select from the real offers; ideal forced schedules belong to earlier studies.

| Offer-aware policy | Strict ecological clears | Rate |
| --- | ---: | ---: |
| Skip all | 32/200 | 16.0% |
| Trailblazer | 30/200 | 15.0% |
| Warren | 34/200 | 17.0% |
| Gardeners | 51/200 | 25.5% |

Gardeners is the strongest current signal: +9.5 percentage points over Skip on matched seeds, with an exploratory 95% interval of +3.2 to +15.8. It remains below the desired target. These are frozen six-phase results, not accepted journey balance.

Moving Trailblazer's five choices earlier produced 28/200 clears versus 30/200: -1 percentage point, 95% interval -7.3 to +5.3, exact McNemar p=.878. We should keep current production timing based on this result. The opening median Hare population improved from 21 to 26, but early Hare-extinction runs increased from 79/200 to 98/200. Later Fox feeding improved and starvation fell in the early arm; that coexistence is a warning against blindly improving Fox persistence.

**Talking point:** "We need enough Foxes left to count as a balanced ecology, but helping Foxes can wipe out the Hares. I think we should diagnose both failure modes together before choosing a buff."

See the [policy report](../Research/Experiments/S4-03-Strategy-Choice-Policy-Screen-600-40-25/REPORT.md) and [timing/failure interpretation](../Research/Experiments/S4-03-Trailblazer-Early-Choice-Timing-600-40-25/ANALYSIS.md).

### 4. Choose the next bounded experiment and owners (5 minutes)

Recommended next step: classify matched seeds by early Hare extinction and by which species miss the final threshold. Compare Hare combat/starvation with Fox kills/starvation/reproduction by phase and normalized exposure. Include partial terminal windows explicitly. Then select one reversible parameter change and confirm it on held-out seeds.

- Keep population and wrapping fixed while diagnosing one mechanism.
- Fox mating eligibility is a possible candidate, not an approved change: current threshold is 25% of maximum energy (60/240), with a mating cost of 36 per parent. Blocked-candidate counts do not contain exact energy at each event, so they cannot prove that a threshold reduction will help.
- Reconfirm the merged revision before extending old evidence to the journey; include its opening reward, persistent upgrades, and condition choice explicitly.
- Proposed ownership: Bevin/Codex handles ecology diagnostics; Salty owns journey rules and node implementation; agree shared acceptance before integrating new tuning.

**Decision to record:** diagnostic protocol, candidate-selection gate, owner, and next review date. No new tuning is approved merely by this agenda.

### 5. Park the larger reward economy with a clear question (3 minutes)

Our earlier direction was that deeper journey progress earns DNA for a separate long-running ecology-building simulator. The current journey uses field-data rewards and is a prototype, so agree which reward belongs within a run and which carries between runs. Salty owns planning this direction; avoid expanding Bevin's immediate balance task into implementing that system.

**Decision to record:** whether that secondary simulator remains the destination, and whether DNA is paid on node completion, cash-out, journey finish, or failure.

## Capture before ending the meeting

- [ ] Node clear/failure rule and target scope.
- [ ] Within-journey upgrade persistence and vocabulary.
- [ ] Starting population/wrapping contract for the next study.
- [ ] One next diagnostic, owner, and review date.
- [ ] Reward/DNA questions assigned to Salty's planning work.
- [ ] Any approved changes copied into the relevant Trello comments and a dated handoff.
