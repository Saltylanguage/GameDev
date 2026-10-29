# Rabbit Genome: remaining work and Luna implementation plan

**Owner:** Josh. **Prepared:** October 21, 2026, using source inspection on
`codex/audio-direction-and-sourcing` at `6ffb7b10` plus the current uncommitted
work. **Status:** Implementation handoff plan. New mechanic definitions and
numbers below remain proposals until Josh accepts them. This document does not
change Trello cards, other developers' scope, or production balance.

## Current packet execution

RG-00 and RG-01 code and focused regression coverage are in the shared checkout.
The Field Data journey, shops, interventions and accounting are integrated.
Focused checks pass: the population-control EditMode class 16/16 and the route
through the first shop into Simulation 2 1/1. Commit `bc864346` fixes the paid
population-removal accounting defect; `7ac2a545` aligns the tests with the
wallet unlock rule. On October 29, Josh reported manually completing the journey
and said the flow made sense. This is qualitative feedback; exact settings,
choices and wallet receipts were not recorded. RG-02's route experience is
validated, while reproducible end-to-end receipt evidence remains open. The
[RG-02 route review](Research/2026-10-22-RG-02-normal-route-review.md) contains
the earlier observations. The required RG-00/RG-01 720p/1080p cycle-result
captures still need confirmation; see the
[RG-00/RG-01 handoff](handoffs/2026-10-21-1820-codex-rabbit-genome-rg00-rg01.md).

## Outcome

Make the current journey useful for testing decisions, then deliver the missing
Rabbit Genome mechanics in small, verifiable pieces. Keep a visible inventory of
6–10 candidate upgrades per branch while designing the unclear nodes. Reach a
playable, readable simulation with meaningful choices before trying to balance
the whole inventory.

Luna can implement the bounded packets below after Josh asks it to continue.
It should stop at the named design gates, bring back a concrete proposal, and
continue independent approved work where possible. A model switch in this chat
can use this document directly; no new chat or delegation is required.

## Established foundation: use it

- Profile wallets, Rabbit tree license, allocation receipts, reset/refund,
  save/load, the eight-point budget, and immutable per-species Genome launch
  snapshots work. The Gene Lab supports multiple selected/allocated nodes.
- The explicit POC catalog has **eight activatable nodes plus one Burrows
  preview**. Seven activatable nodes use stat/lifecycle effects and Fertile
  Droppings uses its existing explicit ability. The production map has Fertile
  Droppings only. Existing POC IDs must continue resolving in saved profiles.
- Implemented POC effects: Long Stride, Long Vision, Tough Hide, maximum litter,
  Efficient Digestion, +5 starting Hares and +10 refill capacity. Do not rebuild
  these effects or the selection bridge.
- Base refill capacity is **50**, plus **10 per active capacity node**. Starting
  Hares and refill capacity are separate. Births can exceed the refill limit;
  survivors are never culled to it. Unchecked Hares may exhaust food and starve.
- The local prototype has two connected six-round simulations, habitat
  conditions, Rest Stop and Seed Cache branches, and six population controls.
  The controls cost 10 Field Data each. Encounter quantities are provisional.
- Current reports use schema **41**, the stat registry is **v7**, and accepted
  population interventions have their own ordered records and checkpoint state.
- Last executed verification: 404 EditMode tests passed, new UI at 720p/1080p,
  existing Genome-pool and journey-reward integration checks, portable build and
  self-test. See the [latest feature handoff](handoffs/2026-10-21-1657-codex-population-decisions-food-feedback.md).
  This planning turn ran no tests, experiments, or gameplay changes.

## First implementation queue

| Packet | Deliverable | Readiness / dependency |
| --- | --- | --- |
| RG-00 | Include the new population code in portable provenance | Ready; start here |
| RG-01 | Make population controls available on the first cycle's result | Ready; follows RG-00 |
| RG-02 | Review the normal-length route and reconcile its shop/session path | Shop/session integrated; automated route passes and Josh reports the manual journey made sense; exact run configuration, choices and receipts are not recorded |
| RG-03 | Complete the agreed stat-only POC offers | Existing targets are ready; new values, prerequisites and costs need Josh's approval |
| RG-04 | Add a food model that supports distinct edible Plants | Design gate G1; core Gardener dependency |
| RG-05 | Deliver Gardener abilities one at a time | RG-04 plus node-specific decisions |
| RG-06 | Add Trailblazer and Warren signature mechanics | Design gates G2–G4; independently sequenced when agreed |
| RG-07 | Finalize the production trees, economy and save migration | Implemented mechanics and reviewed play evidence |
| RG-08 | Confirm comparisons and S5 playability | Approved frozen experiment and human review; track FD-T14–16 obligations |

### RG-00 — Close the portable source-fingerprint gap

**Observed gap:** `PopulationIntervention.cs` is a `SharedSource` in
`tools/CellSim.Batch/CellSim.Batch.csproj`, but it is absent from
`PortableSourceFiles` in `Assets/Editor/SimulationTools/CellSimPortable.cs`.
The build can succeed while a later change to this file escapes the portable
source hash. Treat existing source-hash claims with that limitation. This is the
recorded UPG-C03 provenance concern, with a small concrete mitigation.

**Implementation:**

1. Add the file to the existing portable source list. Inspect export/hash callers
   for another explicit inclusion list that needs the same correction.
2. Add a focused regression check that relevant shared executable sources in the
   batch project are covered by the provenance list. Compare the independently
   used build inputs and manifest; account explicitly for any legitimate exclusion.
3. Verify that changing only PopulationIntervention source changes the hash,
   using the existing injected source-reader seam. Keep real source files intact.
4. Build the batch project and run its existing self-test. Use focused Editor
   tests for the manifest/hash check. No ecology panel or UI recapture is needed.

**Files:** `CellSimPortable.cs`, the batch `.csproj`, relevant Editor tests.
**Done:** the shared implementation is fingerprinted and omission fails a test.
Do not rewrite prior immutable reports or claim they were generated by a new hash.

### RG-01 — Finish the nonterminal round-6 control window

**Accepted requirement:** FD-T06 includes repeat population purchases on
nonterminal round-6 results before a shop. The current coordinator and Noesis
visibility allow only `PhaseDecision`, so the first cycle's `JourneyDecision`
currently has no population controls. The new Rest Stop is an alternate encounter;
it does not fulfill the paid population-control window.

**Implementation:**

1. Centralize the existing legal-window check. Keep ordinary Mutation boundaries,
   and allow the first cycle's frozen `JourneyDecision` while the next simulation
   has not been committed. Use existing cycle constants and run status.
2. Keep controls unavailable during running, ordinary pause, the pending encounter
   popup, after continuation, after terminal success, and after failure/early end.
   After an encounter is claimed, its result may still be managed before travel.
3. Reuse the same population controls in the cycle result. Extract a small XAML
   template if needed so labels/commands/eligibility stay shared. Show the round-6
   controls alongside its travel action; keep round Mutation offers in their
   existing window. Preserve paid atomicity and the 50/+10 pool rules.
4. Preserve the settled closing population and income. Post-settlement changes
   belong to the intervention log and next phase's opening world.
5. Add one focused interaction test: reach cycle 1's result, buy a valid action,
   verify exact wallet/population change and unchanged time/settled result,
   claim a refresh, buy again if eligible, continue, then reject further boundary
   purchases. Verify terminal/failure rejection and full-pool/no-funds rejection.
6. Inspect the affected cycle-result state at 720p/1080p after functional tests
   pass. Reuse existing domain and Desktop helpers.

**Files:** `SpeciesSimulationPreview.cs`, `VM_SimulationShell.cs`,
`V_Window_CellSimulation.xaml`, `LabPlayModeTests.cs`, and an existing domain test
only if the authoritative transaction changes.
**Done:** that result has useful paid controls and preserves world, wallet and
settlement. This packet closes a specific gap; FD-T06's complete retry/idempotency
and full shop-session acceptance still follows its original checklist.

### RG-02 — Check whether the player can influence a normal run

Use the actual Desktop route and normal durations/settings. Record the exact
scenario, configured/effective starting counts, seed, Genome, Mutation choices,
actions, wallet, outcomes and source identity. Distinguish the live prototype
from an integrated Field Data shop journey.

- Observe opening losses and the time until the first usable decision. If a run
  collapses before agency is available, report that timing and the causal evidence.
- Try different responses to visible food/hunting pressure and record the actual
  result. A human records promising, confusing, tedious and unfair moments.
- Identify the active FD-T04 journey/session and FD-T09/12 shop implementation.
  Preserve its domain and any existing session code. If it lives in another
  checkout/branch, report the integration dependency to Josh before overlapping
  it. Do not build a second journey/session/economy to make this prototype appear
  to complete the S5 two-simulation/one-shop gate.
- Preserve the published full FD route/card scope. Rest Stop/Seed Cache are local
  prototype branches; neither is an implemented shop or full five-node route.
- Use existing run/report exporters where available. If actual Desktop choices
  lack export, identify the smallest missing integration with FD-T14. It must
  retain wallet receipts, ordered Mutations, encounter outcomes including leave,
  resolved values, and actual population interventions. A signed-delta list alone
  does not reproduce the entire journey/economy or player decision sequence.

**Done:** a reproducible route review, concrete blockers ranked by impact, and
Josh's next tuning/design decision. Automated survival is not human enjoyment.
Production feeding-threshold/Fox-ramp changes remain unapproved; the earlier
comparison does not authorize promoting its candidate settings.

### RG-03 — Finish stat-only offers before adding structural mechanics

Reuse the registry and frozen Genome bridge. The remaining straightforward
candidates are faster gestation, increased minimum litter, an explicitly chosen
crowding effect, and an explicitly chosen avoidance effect. Maximum litter,
movement, vision, defense and digestion already have playable POC offers.

For each candidate, settle its exact target, signed value, price, capacity,
prerequisites and stacking before authoring a selectable node. A minimal draft
for discussion is **gestation -8 ticks** and **minimum litter +1** against current
natural 48 / 2–5 values; these numbers are recommendations, not approved content.
Use the existing provisional POC economics only after Josh accepts that extension.
Do not choose whether “high crowding” changes tolerance or energy cost on his behalf.

Tests must cover safe lower bounds, minimum <= maximum in every legal active
allocation, prerequisite ordering, one-time application, save/reset and frozen
next-journey delivery. Preserve acquisition order; do not silently clamp invalid
builds. Keep new fixtures under the explicit TestData catalog. A stat proxy does
not implement burrows, migration or a general Hare scent ability.

## Food and ability implementation contracts

### G1 / RG-04 — Define distinct food before the remaining Gardener nodes

**Decision needed:** approve the food identities/tiers, energy per consumed unit,
reserve capacity, growth timing, edible set, and seed costs/selection. Start with
three distinguishable test plants representing tiers 1–3 if Josh agrees. Exact
values and species names remain open.

**Recommended structure:** reuse stable Plant `SpeciesId` values, immutable
species/food definitions and the existing resource layer. Resolve edible targets
through explicit supported food data. Add per-cell state only for effects that
really vary per plant, such as Nature's accelerated growth. Keep a one-target
compatibility path for existing species. Use a closed implementation for each
ability and freeze its resolved values with the Genome.

**Important current assumptions to remove deliberately:**

- `SpeciesRules.DietTargetId` and `SpeciesPerception` currently use one diet target.
- `SpeciesSimulation` supplies `GetFirstPlantEnergyValue` to feeding helpers.
- Seed dropping currently selects the first registered Plant.
- Resource growth uses terrain definitions and cell growth timing; applying a
  global grass multiplier would affect untouched plants as well.

**Acceptance:** a no-Genome Hare can eat every approved food; each target uses
its own identity/energy/reserve; two foods on a controlled map have distinguishable
results; seed selection is deterministic and registration-order independent;
creature/resource layers survive consumption/removal; wrap edges, search limits,
checkpoint restore, portable exports and versioned report inputs are covered.
The existing single-Plant baseline retains its behavior when new abilities are off.

### RG-05 — Gardener delivery order

| Order | Ability | Contract that must be accepted before implementation | Evidence required |
| --- | --- | --- | --- |
| 1 | Picky Eater | How nutrient quality is ranked against reachable distance; sensing range and tie-breaks | Reachable alternatives, chosen quality, rejected unreachable choices, ordinary-Hare comparison |
| 2 | Tier-2 then tier-3 Droppings | Unlocked seed set, eligibility/selection, reserve cost, caps and how tier-3 depends on tier-2 | Attempts, eligible drops, selected tier, success, reserve spent and food created |
| 3 | Favorite Food | Named edible target, exact 2x benefit and scarcity tradeoff | Target-specific meals and actual extra energy; other foods retain base yield |
| 4 | At One with Nature | Preserve chance; whose energy “50%” describes; what remains; 2x growth duration/cap | Trigger opportunities, preserves, energy received, growth effect and its expiry |
| 5 | Pass Through Bushes | Meaningful bush terrain; Hare/Fox traversal; sensing/feeding through or around it | Path reachability and actual movement agree; no crossing forbidden terrain |
| Design decision | Patrols | Job, route, stop/return conditions, energy cost and Fox exposure | A player-visible behavior distinct from wandering, foraging and migration |

Existing Fertile Droppings and Efficient Digestion remain the first playable
Gardener pair. Add one ability per packet with its trigger denominator and outcome
telemetry. Keep ordinary behavior off-path unchanged and test combinations with
Fertile Droppings. Balance renewal against reserve expenditure and food demand;
food exhaustion must remain possible.

A conditional 2x yield/growth effect is a named ability, not an unapproved generic
multiply operation inserted into the signed-additive stat registry (UPG-C02).
Extend the existing explicit effect contract only as each ability requires.

### G2–G4 / RG-06 — Trailblazer and Warren signature mechanics

| Design gate | Minimum decisions | Implementation boundary |
| --- | --- | --- |
| G2: Wanderlust | Chunk size, wrap behavior, crowding measure, threshold accumulation, stay/migrate roll, destination, energy/mating consequences | Density observation and deterministic migration state; local dispersal remains distinct |
| G3: Burrows | Construction/maintenance, occupancy, access, mating/hiding safety, crowding penalty, predator counterplay, death persistence, offspring use | Run-owned persistent structure state with stable IDs, checkpoint/report support and explicit animal behavior |
| G4: Hare scent | Whether food, mates, predators or a subset are detected; precedence and stale-target behavior | Shared scent sampling at the approved 10-tick cadence, longer-range bounded perception; attack still follows its accepted sight requirements |
| Additional decisions | What 2x interactions doubles; what skip-a-node skips; what burrow speed accelerates; how minimum-litter x n crowding works | Implement only the accepted closed rule with measurable opportunities and costs |

Preserve sight's current-state response and scent's slower samples. Existing Fox
Droppings tracking does not give Hares a functioning “good scent” upgrade.
For litter-linked crowding, specify whether it uses natural, Genome-adjusted or
Mutation-adjusted minimum litter; do not turn it into a fixed additive proxy.
Structures and migration state belong to the simulation, not scene objects.

## Candidate inventory: keep every idea visible

These counts include proposed slots, not promises that every concept is ready.
The two added population nodes currently sit in Warren in the POC.

| Branch | Candidate slots | Current implemented POC portion | Remaining mechanics / definition |
| --- | ---: | --- | --- |
| Trailblazer | 8 | Movement, long vision | Avoidance, fast gestation, chosen crowding effect, wanderlust, 2x interactions, skip a node |
| Warren | 10 | Tough hide, maximum litter, +5 starting Hares, +10 pool | Burrows, Hare scent, minimum litter, litter-linked crowding, burrow speed, offspring/inherited burrows |
| Gardener | 9 | Fertile Droppings first slice, Efficient Digestion | Bush traversal, Picky Eater, tier-2 drops, tier-3 drops, Favorite Food, At One with Nature, Patrols |

**Total: 27 candidate slots.** This fits the requested 6–10 range per tree.
Josh chooses whether unresolved concepts stay, combine or are replaced; do not
silently delete them to hit a sprint estimate. Additional capacity nodes would
need separate authored identities/allocation rules; no node-level system is implied.

## RG-07 / RG-08 — Production and playability finish

1. Agree production IDs/tree links, per-node effects, prices/capacity, reset fees
   and any milestone gates. The “100 rabbits unlock Burrow” example stays
   hypothetical. Preserve permanent tree access versus resettable allocations.
2. Plan migration for saved active POC IDs before replacing/removing their map.
   Validate the full authored tree and display specific blockers in Gene Lab.
3. Freeze a playable reference after agreed fixes. Record source hashes, natural
   rules, starting counts, world dimensions/wrapping, tick/round durations, active
   Genomes, Mutation/action scripts, shop/economy rules and exact seed IDs.
4. Before each new ecology comparison, write the prediction and its expected
   causal story, choose the controlled arms and metrics, and obtain approval of
   that exact scope under SG-001/SG-005. Reuse the existing prediction-input
   adapter/evidence workflow; flag unsupported mechanisms as not yet testable.
   Identify AI forecasts as forecasts and report calibration evidence before
   attaching numerical confidence or claiming a trained predictor was used. Preserve raw outcomes separately from
   interpretation. A small diagnostic screen does not satisfy FD-T15's approved
   200-matched-seed-per-arm requirement. Do not invent a numerical success target
   or promote the earlier panel's conditions into the current baseline.
5. Judge focal-species survival, resource pressure/renewal, births/death causes,
   predator feeding/starvation and economy separately. Compare population actions
   separately from Genome effects before testing combinations. Record when a
   choice is unavailable because a run has already failed.
6. Bevin guides provisional phase/viability judgment. Named human players review
   two different attempts through the actual route, explaining promising,
   confusing, tedious and unfair moments. Record accept/revise independently of
   passing tests. Coordinate the actual shop/full-route work and original card
   requirements before declaring S5 complete.

## How Luna should work without spending budget on rediscovery

- Start with this plan and the latest feature handoff. Read just the packet's
  code, callers and relevant instructions; do not rescan historical reports.
- Implement **one packet at a time**. Ask Josh at its design gate with a concrete
  proposal; do not request the same already-granted approval again.
- Preserve the dirty checkout, GUIDs, metas, existing POC IDs and evidence. Work
  locally unless Josh requests sharing. Do not move cards or message other chats.
- Reuse the runner/registry/profile/Noesis paths. No second simulator, generalized
  modifier framework, event bus, or parallel journey/economy infrastructure.
- Write the smallest meaningful regression test for the changed risk. Compile
  first, test focused behavior, then capture the final affected UI once at
  720p/1080p. Inspect captures. Run the broad suite only for shared-contract risk
  or an explicit gate. Avoid repeated whole-suite or screenshot runs for unchanged
  states; a zero-test filter result is not evidence.
- Use `CellSim.ps1` Auto/Live/Clean and approved elevated host access. The current
  Live lane accepts a single name filter; semicolon-name filters selected zero
  tests. Do not edit C# while a Unity test run is active.
- Update the packet result and a concise handoff: changes, exact executed tests,
  artifacts, limits, next decision. Unit/fixture checks are not balance runs.
- Start with **RG-00, then RG-01**, and return the result before broadening scope.
  Josh explicitly authorized the RG-02 review before RG-00/RG-01's pending
  Unity verification; that review is now delivered. Do not begin RG-03 offers or
  values until the starting-count and first-spendable-Field-Data decisions above
  are resolved.

## Sources

- [Current prerequisite checklist](Research/2026-10-17-Rabbit-Genome-prerequisite-checklist.md)
- [Genome architecture and accepted product direction](UPGRADE_SYSTEM_DIRECTION.md)
- [POC contents and save compatibility](Research/2026-10-20-Gene-Lab-selection-POC.md)
- [Population-pool contract](Research/2026-10-21-Hare-population-pool-and-Genome-nodes.md)
- [Population controls/encounters](POPULATION_DECISIONS_PLAYTEST.md)
- [Original Field Data ticket acceptance](FIELD_DATA_TICKET_DRAFT.md)
- [S5 playability goal](Sprints/S5-playability-goalpost-2026-10-16.md)
- [Active upgrade concerns](Planning%20Concerns/upgrade-system.md)
