# Upgrade System Direction — Mutations and Genomes

Status: **approved product direction; the first Mutation contract exists, while
Genome progression and the scalable balance model remain planned work**.
Feature owner: **Josh**. Sim is not assigned to this feature.
Active concern record: [`Planning concerns/upgrade-system.md`](Planning%20Concerns/upgrade-system.md).
Balance guideline:
[`SG-005 — Upgrade and Ecology Balance`](Studio%20Guidelines/SG-005-UPGRADE-AND-ECOLOGY-BALANCE.md).

## Two upgrade systems

Every species has both systems. They share the species-rule foundation, but
their ownership and lifetime must remain visibly separate.

| System | Player meaning | Lifetime | Application |
| --- | --- | --- | --- |
| **Mutation** | An acute adaptation chosen during the current expedition | Resets when the expedition ends | Applies to the selected species and remains active for later phases |
| **Genome** | A permanently unlocked species option configured in the Gene Lab | The unlock persists; active choices can change between simulations | The active configuration applies to every population of that species, including when it is not player-controlled |

The effective rules used in a Species Simulation are:

```text
Natural species rules + active Genome + ordered expedition Mutations
```

The effective rules used in a Biome Simulation are:

```text
Natural species rules + active Genome
```

Scientific data connects both systems. The exact in-expedition Mutation cost
and permanent Genome economy remain open in
[`SCIENTIFIC_DATA_ECONOMY.md`](SCIENTIFIC_DATA_ECONOMY.md).

## Permanent unlocks and the active Genome

Each species has its own Genome tree. Purchasing a node unlocks that option
permanently; it does not require the node to remain active forever. Between
simulations, the player can turn unlocked nodes on or off and create an active
Genome for that species.

The active Genome applies wherever that species appears. Playing Hare does not
silently turn off the active Fox or Fern Genomes. Turning a node off does not
remove the permanent unlock or undo primary progress.

Genome improvements may change stats, unlock behaviours, improve efficiency,
or change how a species fills its ecological role. “Better” does not always
mean more damage, more offspring, or faster consumption. A mature Genome can
also improve restraint, recovery, sustainable feeding, migration, or response
to scarcity.

Genome purchases and configuration occur in the Gene Lab and take effect on the
next simulation. They do not rewrite a simulation already in progress. The
profile must store permanently unlocked node IDs separately from each species'
active configuration. A simulation receives an immutable active Genome snapshot
for every participating species at launch.

A reassignable point or capacity budget is the leading way to stop every
unlocked node from being active at once while preserving experimentation. The
exact capacity, whether different nodes consume different amounts, and whether
reallocation has a cost remain open decisions. The recommended starting bias is
to make reallocation available between simulations and freeze it during a run.

Capacity cost should represent the size and flexibility of the species-level
change, not whether the node is “good” for a biome. Biome impact is reported
separately through diversity, stability, resource pressure, recovery, and other
scenario goals. A powerful but destabilizing Genome node should not become
cheap merely because its Biome result is negative.

Plant, Herbivore, and Carnivore remain useful filters, data categories, and
balance roles. They are not substitutes for each species' individual Genome.
If broad role-wide research is added later, it is a separate progression
system and must not be silently folded into Genome behaviour.

## Two simulation modes

The two modes can reward different kinds of success without contradicting the
larger game.

### Species Simulation

The player guides one focal species through the current expedition loop. Every
species present uses its active Genome, and the selected species receives nine
temporary Mutation choices. Mutations end with that run and never carry into a
Biome Simulation.

It is valid for this mode to reward focal-species survival, reproduction,
resource use, or a species-specific objective. Those results should still be
readable in context, but they do not have to maximize whole-biome stability.

### Biome Simulation

The player configures active Genomes across several species and observes how
the community behaves. There are no Mutations. Success is judged by the
scenario's ecological goals: for example biodiversity, recovery, stability,
population pressure, or persistence of intended food relationships.

A Genome node is not required to be good for the biome. The interesting choice
comes from finding cross-species combinations that produce a desired ecology.
An unsuccessful configuration provides information and a reason to reallocate;
it does not revoke unlocked Genome nodes.

## Possible third track: Biome projects

The Biome Lab may eventually offer persistent environmental projects that make
a habitat more capable of supporting biodiversity or recovering from pressure.
Examples could include water retention, habitat corridors, nesting space, soil
recovery, or additional ecological niches.

This is a promising separate system, not a Genome effect. A limited baseline
restoration step could be required before advanced Biome progress if it creates
a clear learning sequence. These projects should expand or reshape the set of
viable ecosystem solutions rather than serve as automatic win buttons. Their
costs, toggling rules, progression role, and relationship to Genome capacity
remain open design work.

## Temporary Mutation paths

During an expedition, Mutation choices form a visible decision tree rather
than an unrelated sequence of bonuses. Early choices establish a direction;
later choices deepen, modify, or occasionally hybridize that playstyle.

The ten-phase expedition creates nine Mutation decision points. The approved
direction is for a completed expedition to produce nine Mutations. The current
prototype also supports Skip; whether Skip remains a counted decision or is
removed from the final player flow must be resolved without changing the
nine-Mutation progression target silently.

A simple run structure is:

```text
Foundation choice
    -> specialization fork
        -> defining mechanic
            -> capstone or hybrid
```

Each node should communicate:

- its immediate rule change;
- the later nodes or branches it enables;
- its strength and tradeoff;
- the behavior and telemetry that will reveal its effect;
- whether it excludes an incompatible branch.

Mutations are stored in acquisition order with resolved values and effective
ticks. They survive phase summaries and disappear only after the expedition
ends. Choosing a Mutation or Skip continues the same world; neither creates a
new starting population.

## Example Hare Mutation branches

The current Hare vertical-slice builds provide the first working example:

- **Trailblazer:** movement and perception lead toward rapid migration and access to fresh food, at the cost of weaker grouping or protection.
- **Warren:** local defense and controlled reproduction lead toward stable breeding pockets, at the cost of mobility and greater local resource pressure.
- **Gardeners:** feeding efficiency and seed dispersal lead toward sustaining a food frontier, at the cost of delayed payoff and weak immediate predator defense.

These are intended build identities, not final node lists. The first implementation should prove these three paths with a small explicit catalog before creating a reusable tree-authoring framework.

### First Mutation catalog contract

The following six nodes are the initial path candidates. Their values are
starting hypotheses, not accepted balance. Each targets exactly one species, is
Hare-only in this first slice, is a temporary Mutation,
non-stackable in the first slice, and is recorded by stable ID in purchase
order. Cross-species upgrades are deferred. Direct mechanic tests must pass
before ecological trials.

| Stable ID | Build | Effect | Tradeoff | Required contribution evidence |
| --- | --- | --- | --- | --- |
| `trailblazer-long-stride` | Trailblazer | Movement speed +0.5 | Reproduction neighbor count +1 | Attributable movement, target arrivals, missing-mate blocks |
| `trailblazer-far-sight` | Trailblazer | Vision range +1 | Metabolism +1 | Targets detected beyond the old range, energy trajectory, starvation deaths |
| `warren-guarded-burrow` | Warren | Block +2 | Movement speed -0.25 | Blocks, prevented hits/damage, movement |
| `warren-room-to-breed` | Warren | Reproduction group limit +1 and crowding penalty -1, floored at zero | Metabolism +1 | Group-limit blocks, crowding penalties, births, local Fern depletion |
| `gardeners-seed-pouches` | Gardeners | Starting food reserve +2 | Starting energy -2 | Reserve consumed, seed attempts/successes, early starvation exposure |
| `gardeners-careful-sowing` | Gardeners | Seed-drop chance +0.10, capped at 1.0 | Movement speed -0.25 | Successful drops, new Fern cells, Fern population-time integral, movement |

`stronger-attack`, split attack-modifier, and damage upgrades are excluded from
the Hare catalog. Block above +2 and Fox attack cooldown remain diagnostic arms,
not player rewards. Same-build prerequisites and cross-build hybrids remain
future decisions until these nodes independently demonstrate their declared
mechanics and costs.

The launch snapshot and each subsequent Mutation record must include every
effect, tradeoff, ordered selection and effective tick. The current contract
supports an ordered list of immutable
snapshots; each snapshot still targets one species and uses signed additive
modifiers only. This preserves A→B versus B→A evidence without introducing
clamping, multiplication, or other V1 operations.

The existing Unity type names still use “Upgrade” for compatibility. In
player-facing design, these per-expedition assets are Mutations. The Unity
authoring adapter is `SpeciesUpgradeAsset`. Its inspector resolves
modifier attributes from `SpeciesAttributeRegistry`, validates the contract in
place, and keeps Scriptable Objects out of runtime state. The initial
production catalog now contains seven authored assets under
`Assets/Data/CellularSimulation/Upgrades/Production/`: the six path candidates
above plus `familial-bond-large-litters`. Their values remain starting
hypotheses until balance evidence promotes them.
The repeatable authoring workflow is documented in
[`UPGRADE_AUTHORING_GUIDE.md`](UPGRADE_AUTHORING_GUIDE.md). The launch boundary,
progression, run result, and report serializers now carry the ordered snapshot
metadata and fingerprints. Research runs can opt into the same authored values
with `-UpgradeAssetSequence`; the `SpeciesUpgradePredictionInputAdapter`
resolves a declared production or research catalog into ordered immutable
snapshots and records the catalog path, prediction input, and fingerprints in
the report. The older string-loadout arguments remain available for historical
experiments and diagnostic arms.
The row-by-row authoring contract and its current acceptance state are tracked
in [`UPGRADE_CATALOG_ACCEPTANCE_MATRIX.md`](UPGRADE_CATALOG_ACCEPTANCE_MATRIX.md).
The prototype Simulation scene now references the explicit production assets;
its reward panel presents authored options and applies snapshots, while the
legacy reward path remains available for the BEV experimental mode.

**Timing boundary:** EX-009 covered a complete loadout applied before a fresh
run. EX-010 now provides bounded evidence for ordered Mutations acquired during
one continuing expedition. Future Mutation types and Genome combinations still
need their own timing and interaction evidence.

**Catalog applicability:** `gardeners-seed-pouches` changes starting reserve and
energy. It has no grant to existing creatures at an upgrade break, and current
creature births do not inherit those starting values. Under the locked CF-0
contract it is eligible only at fresh launch; a live-state/newborn mechanic
would require a separate product decision. No asset or mechanic is changed by
this documentation review. A signed maximum-energy change preserves existing
energy at the boundary, including temporary above-cap values; later gains use
the authored cap rule.

## Effect families

Both systems use a shared player language, although their timing differs:

1. **Stat change:** adjusts a supported species value.
2. **One-time effect:** performs one explicit action at a legal boundary.
3. **Ability:** unlocks a new behaviour with its own costs, counters, and
   contribution evidence.

The current V1 Mutation contract supports signed additive stat changes only.
One-time effects, abilities, and Genome application are separate contract
packages. Do not disguise them as numeric modifiers or expand the V1 asset
format implicitly.

## Game-wide objective

The player is not trying to make one species dominate every simulation. The
long-term objective is to build a diverse, resilient ecosystem.

Improving one Genome can create a useful new imbalance: stronger Hare
reproduction may strain Fern resources; stronger Fern recovery may support too
many Hares; improved Fox pressure may restore control but threaten a weak Hare
population. These consequences should encourage the player to run and develop
several species.

Balance does not mean equal populations. Each role receives its own healthy
range and pressure cycle. When participating species have highly developed
Genome libraries, legal active configurations should include combinations that
support stability, recovery, and meaningful interaction. The game does not
require every node to be active or every configuration to be good for the
biome.

## Balance is part of feature development

Follow SG-005 from the first content sketch through production approval. An
upgrade is not finished merely because its asset validates or its local rule
works.

### UB-0 — Vocabulary and boundaries

**Status:** This direction and SG-005 establish the first approved baseline.

- Keep Mutation, Genome, natural rules, capability, ecological interaction,
  simulation mode, and outcome distinct.
- Keep permanent Genome unlocks separate from active Genome configurations.
- Keep the current immutable Mutation snapshot and stable-ID path intact while
  later contracts are added.
- Update player-facing plans and screens to use Mutation and Genome consistently.

**Gate:** A designer can explain what persists, what resets, who receives the
change, and when it becomes active.

### UB-1 — Shared capability map and reference panel

- Group the existing registered stats beneath a small set of player-meaningful
  capabilities such as escape, pursuit, detection, concealment, feeding,
  reproduction, resilience, and resource recovery.
- Declare a small reference panel of Species and Biome scenarios, species
  relationships, Genome unlock/allocation stages, and expedition windows.
- Give every capability local measurements and wider ecological outcomes.

**Gate:** Every current Hare Mutation maps to a supported rule, at least one
capability, a local measurement, and an ecosystem measurement without inventing
a second execution path.

### UB-2 — Adaptation Value calibration

- Create a versioned, non-runtime balance reference that estimates the value of
  supported stat changes at named starting values.
- Measure value at several starting levels so diminishing or runaway returns
  are visible.
- Define provisional Mutation and Genome tiers only after the reference panel
  exists.
- Record average impact and variation; do not hide the source outcomes inside
  one combined fitness score.

**Gate:** The first tier ranges and stat exchange estimates cite reproducible
reports and are clearly labelled as contextual estimates.

### UB-3 — Mutation paths and effect types

- Balance comparable choices at each of the nine acquisition points and compare
  full reachable paths rather than total tree size.
- Test early, middle, and late acquisition because the same Mutation can have a
  different value depending on how long it acts.
- Add one-time effects and abilities only through explicit contracts with
  timing, cost, counters, telemetry, and deterministic replay.
- Preserve authored order and test likely pairs and suspected synergies.

**Gate:** At least three Hare paths remain understandable and viable across the
reference panel, no path is universally correct, and the player can see each
path's strength and weakness.

### UB-4 — Genome foundation

- Define the versioned profile and immutable launch snapshot for one species'
  Genome.
- Store permanent node unlocks separately from the active allocation.
- Freeze and apply each active Genome to every population of its species,
  whether or not it is selected by the player.
- Test natural, early, middle, and highly developed Genome libraries using
  legal active configurations, with representative Mutation paths only in
  Species Simulations.
- Keep broad role categories out of the Genome execution contract unless a
  separate role-wide progression feature is approved.

**Gate:** One Hare Genome node unlock survives restart, can be turned on or off
between simulations, changes all Hare populations only when active, is recorded
in run provenance, and does not alter a simulation already in progress.

### UB-5 — Matchup and ecosystem review

- Keep separate Species and Biome scorecards. Compare species, builds, and
  environments through representative matrices without demanding equal
  outcomes everywhere.
- Flag universal dominance, missing counters, repeated extinctions, runaway
  resource use, failed recovery, and choices that rarely matter.
- Test legal cross-species active Genome configurations at uneven and highly
  developed unlock stages.

**Gate:** The selected ecosystem has more than one useful response, retains its
intended species and resource relationships, and can recover from its declared
pressures across matched and held-out seeds.

### UB-6 — Targeted automation and scale-up

- Extend the existing `CellSim` report path with simulation mode, Genome unlock
  and active-allocation provenance, capability groups, reference-panel
  comparisons, marginal-value sweeps, and targeted pair/path analysis.
- Add choice-dominance analysis when bot/search or enough player-choice data
  exists.
- Build a dashboard or scheduled broad sweep only after the underlying reports
  and repeated review questions are stable.

**Gate:** Automated warnings identify their inputs, expected range, observed
result, variation, and source report. They nominate a human review; they never
silently change production values.

### UB-7 — Content expansion gate

- Use the first Hare / Fox / plant ecology to prove the workflow.
- Add species and upgrade families only after the shared capability, authoring,
  evidence, and human-decision paths work end to end.
- Review the reference panel whenever a new species, biome, ability family, or
  Genome stage introduces a genuinely new interaction.

**Gate:** Adding another species is primarily content and evidence work, not a
new balance architecture or another hidden rules system.

### Delivery order

```text
UB-0 vocabulary and boundaries
  -> UB-1 capability map and reference panel
       -> UB-2 Adaptation Value calibration
            -> UB-3 Mutation paths
            -> UB-4 Genome foundation
                 -> UB-5 matchup and ecosystem review
                      -> UB-6 targeted automation
                           -> UB-7 broader content
```

Automation may support earlier packages, but it does not block a manual,
evidence-backed design decision. Package owners and estimates must be assigned
when a package is selected for a sprint; this direction does not silently add
them to the completed Sprint 2 scope.

## Guardrails

- Genome progress must not remove the need to make consequential Mutation
  choices during an expedition.
- Mutations never enter Biome Simulations.
- A Genome unlock is permanent; its activation is configurable and frozen only
  for the current simulation.
- Do not use Species Simulation success as the sole measure of Genome or Biome
  health, or use Biome health to invalidate a Mutation outside that mode.
- A Mutation path needs a visible weakness; selecting every benefit is not a
  build.
- Branch prerequisites and exclusions should be understandable before purchase.
- Upgrades that do not change visible behavior or measurable outcomes should be revised or removed.
- Avoid randomized offers that make a chosen branch impossible to continue; randomness may vary options within the committed direction.
- Do not create a universal modifier, node-graph, or scripting framework until the first explicit tree proves what authoring features are actually needed.
- Do not use Adaptation Value as a hidden universal fitness score or as a
  substitute for ecological evidence.
- Review every effect with the scorecard for the modes where it can appear.

## Questions for later design

- Does Skip remain part of the final nine-decision flow, or does every completed
  expedition select exactly nine Mutations?
- How much active Genome capacity does each species receive, how do nodes
  consume it, and is reallocation free between simulations?
- Does Species Mastery remain a separate progress measure, or does it become
  the way Genome nodes are revealed?
- When can a player hybridize two branches, and what is sacrificed?
- Are Mutations purchased with in-expedition data, granted at phase breaks, or
  both?
- Which ecosystem measures define healthy ranges for the first Forest Edge
  reference panel?
- Which current stats belong under each shared capability?
- Which one-time Mutation and ability should be the first contract expansion
  after signed stat changes?
- Do Biome projects become a separate permanent progression track, and should a
  small restoration baseline gate advanced Biome challenges?

