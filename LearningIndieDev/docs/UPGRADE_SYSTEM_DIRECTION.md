# Upgrade System Direction — Future Work

Status: **approved product direction; detailed trees and balance remain future design work**.
Feature owner: **Josh**. Sim is not assigned to this feature.
Active concern record: [`Planning concerns/upgrade-system.md`](Planning%20Concerns/upgrade-system.md).

## Two progression layers

The upgrade system has two distinct layers:

1. **Species-type research** is permanent progression purchased at the Lab for Plants, Herbivores, and Carnivores.
2. **Run evolution** is a temporary branching build assembled during one
   expedition and reset when that expedition ends. Its consecutive simulation
   phases share the same evolving ecosystem and acquired upgrades.

Scientific data funds both layers, creating a choice between immediate adaptation and long-term research. See [`SCIENTIFIC_DATA_ECONOMY.md`](SCIENTIFIC_DATA_ECONOMY.md).

## Permanent species-type research

The Lab contains one research tree for each broad species type:

- **Plants:** propagation, resource production, resilience, and environmental influence.
- **Herbivores:** movement, foraging, herd behavior, reproduction, and predator avoidance.
- **Carnivores:** hunting, pursuit, territory or pack behavior, and prey efficiency.

These trees apply to every species of that type, while species mastery supplies narrower species-specific unlocks.

Permanent research should primarily expand choices instead of stacking unlimited power. Suitable rewards include:

- unlocking new run-upgrade branches or node families;
- adding an alternate starting trait or loadout choice;
- improving upgrade previews and exposing relevant telemetry;
- allowing one reroll, branch preview, or controlled starting option;
- unlocking type-specific mechanics that still require investment during a run.

Small permanent statistical bonuses may be tested as occasional milestones, but should remain bounded. A new profile must still be able to complete early content, and a developed profile must still face meaningful ecological pressure.

## Temporary branching run evolution

During a run, upgrade choices form a visible decision tree rather than an unrelated sequence of bonuses. Early choices establish a direction; later choices deepen, modify, or occasionally hybridize that playstyle.

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

Run upgrades are stored in acquisition order with resolved values and ticks.
They survive phase summaries and disappear only after the expedition ends.
Buying an upgrade or skipping it must continue the same world; neither creates
a new starting population. Runtime migration is pending under the
[consecutive simulation plan](CONTINUOUS_SIMULATION_FLOW_PLAN.md).

## Example herbivore branches

The current Hare vertical-slice builds provide the first working example:

- **Trailblazer:** movement and perception lead toward rapid migration and access to fresh food, at the cost of weaker grouping or protection.
- **Warren:** local defense and controlled reproduction lead toward stable breeding pockets, at the cost of mobility and greater local resource pressure.
- **Gardeners:** feeding efficiency and seed dispersal lead toward sustaining a food frontier, at the cost of delayed payoff and weak immediate predator defense.

These are intended build identities, not final node lists. The first implementation should prove these three paths with a small explicit catalog before creating a reusable tree-authoring framework.

### First experimental catalog contract

The following six nodes are the initial path candidates. Their values are
starting hypotheses, not accepted balance. Each targets exactly one species, is
Hare-only in this first slice, is temporary for one run,
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

The launch snapshot and each subsequent acquisition record must include every
effect, tradeoff, ordered selection and effective tick. The current contract
supports an ordered list of immutable
snapshots; each snapshot still targets one species and uses signed additive
modifiers only. This preserves A→B versus B→A evidence without introducing
clamping, multiplication, or other V1 operations.

The Unity authoring adapter is `SpeciesUpgradeAsset`. Its inspector resolves
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
The prototype Simulation scene now references the explicit production assets;
its reward panel presents authored options and applies snapshots, while the
legacy reward path remains available for the BEV experimental mode.

**Timing boundary:** EX-009 verified only a complete loadout applied before a
fresh run starts. Continuation is required game direction and its runtime work
is now [planned](CONTINUOUS_SIMULATION_FLOW_PLAN.md). Its timing/state evidence
remains untested under proposed
[EX-010](Research/Experiments/EX-010-Sequential-Upgrade-Continuation/README.md).

**Catalog applicability:** `gardeners-seed-pouches` changes starting reserve and
energy. It has no grant to existing creatures at an upgrade break, and current
creature births do not inherit those starting values. Under the locked CF-0
contract it is eligible only at fresh launch; a live-state/newborn mechanic
would require a separate product decision. No asset or mechanic is changed by
this documentation review. A signed maximum-energy change preserves existing
energy at the boundary, including temporary above-cap values; later gains use
the authored cap rule.

## Relationship to species mastery

Species-type research answers, “What can members of this ecological type learn?” Species mastery answers, “What is special about this particular species?”

For example, Herbivore research might unlock a general migration branch, while Hare mastery adds a hare-specific evasive node within that branch. Species mastery should enrich type trees without requiring a sep arate full permanent tree for every species.

## Guardrails

- Permanent progress must not remove the need to make consequential choices during a run.
- A run path needs a visible weakness; selecting every benefit is not a build.
- Branch prerequisites and exclusions should be understandable before purchase.
- Upgrades that do not change visible behavior or measurable outcomes should be revised or removed.
- Avoid randomized offers that make a chosen branch impossible to continue; randomness may vary options within the committed direction.
- Do not create a universal modifier, node-graph, or scripting framework until the first explicit tree proves what authoring features are actually needed.

## Questions for later design

- Does permanent research unlock nodes, improve their starting availability, or both?
- Can permanent tree choices be respecced, and at what cost?
- How many run decisions are needed for a branch to feel developed?
- When can a player hybridize two branches, and what is sacrificed?
- Are run nodes purchased directly with data, offered at phase breaks, or both?
- How are newly unlocked nodes introduced without diluting a reliable build path?

