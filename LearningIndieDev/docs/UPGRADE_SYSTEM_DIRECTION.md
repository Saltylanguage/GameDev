# Upgrade System Direction — Future Work

Status: **approved product direction; detailed trees and balance remain future design work**.

## Two progression layers

The upgrade system has two distinct layers:

1. **Species-type research** is permanent progression purchased at the Lab for Plants, Herbivores, and Carnivores.
2. **Run evolution** is a temporary branching build assembled during one simulation and reset when that run ends.

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

Run upgrades are stored as an ordered part of the deterministic effective ruleset and disappear after results are resolved.

## Example herbivore branches

The current Hare vertical-slice builds provide the first working example:

- **Trailblazer:** movement and perception lead toward rapid migration and access to fresh food, at the cost of weaker grouping or protection.
- **Warren:** local defense and controlled reproduction lead toward stable breeding pockets, at the cost of mobility and greater local resource pressure.
- **Gardeners:** feeding efficiency and seed dispersal lead toward sustaining a food frontier, at the cost of delayed payoff and weak immediate predator defense.

These are intended build identities, not final node lists. The first implementation should prove these three paths with a small explicit catalog before creating a reusable tree-authoring framework.

## Relationship to species mastery

Species-type research answers, “What can members of this ecological type learn?” Species mastery answers, “What is special about this particular species?”

For example, Herbivore research might unlock a general migration branch, while Hare mastery adds a hare-specific evasive node within that branch. Species mastery should enrich type trees without requiring a separate full permanent tree for every species.

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

