# Vertical-Slice Scenario, Roster, and Builds

Status: **initial selection for validation**. This is the working content target for Sprint 0; fixed-seed experiments and playtests may tune values or reject a build without expanding the roster.

## Curated scenario: Forest Edge

Use the existing `ForestEdge` scenario as the vertical-slice foundation. It is the smallest authored ecosystem that contains a complete and immediately readable pressure chain:

```text
fern -> hare -> fox
 food    player   predator
```

The player develops the **hare** species. Ferns are the supporting resource species and foxes are the opposing predator species. No fourth species enters the initial slice.

This choice keeps causal explanations legible: hare population changes should primarily trace to food access, reproduction/crowding, or fox pressure. `OpenRange` and `Wetland` remain useful later scenarios, but their five-species rosters add interactions before the core upgrade loop has been proven.

## Roster roles

| Species | Slice role | Pressure created | Why it earns a place |
| --- | --- | --- | --- |
| Fern | Supporting food/resource | Patch depletion and recovery constrain where hares can thrive. | Makes movement, food efficiency, and seed dispersal visibly valuable. |
| Hare | Player species | Must balance feeding, reproduction, crowding, and predator avoidance. | Its fast movement and dependence on both food and mates support several understandable build directions. |
| Fox | Opposing predator | Converts dense or exposed hare populations into direct losses. | Prevents population growth from being the only concern and gives defensive/spatial upgrades a clear purpose. |

## Three intended build styles

### 1. Trailblazer

**Fantasy:** a mobile population that reaches fresh fern patches and escapes local danger.

- Prioritizes movement speed, food-search reach, and efficient movement patterns.
- Visible strength: spreads into available food sooner and recovers from local resource collapse.
- Visible weakness: rapid dispersal makes mate contact less reliable and does not directly protect a caught hare.
- Different choice pressure: favors mobility and perception over reproduction rate or direct protection.

### 2. Warren

**Fantasy:** a dense, resilient population that survives pressure through local protection and controlled growth.

- Prioritizes block/protection effects, crowding tolerance, and compact reproduction patterns.
- Visible strength: preserves breeding pockets during fox pressure and stabilizes after losses.
- Visible weakness: consumes local fern patches quickly and is vulnerable when forced to relocate.
- Different choice pressure: favors defense and population stability over movement and resource reach.

### 3. Gardeners

**Fantasy:** hares sustain their own food frontier through feeding efficiency and seed dispersal.

- Prioritizes food value/energy efficiency, carried seed reserve, and seed-drop chance or pattern.
- Visible strength: creates renewable fern patches and supports longer population recovery.
- Visible weakness: develops slowly and has no immediate answer to concentrated fox attacks.
- Different choice pressure: favors ecosystem investment over short-term mobility or protection.

## Upgrade/catalog implications

The current explicit upgrade implementation covers movement speed, attack amount, and block amount. That is enough to prototype part of Trailblazer and Warren, but it does **not** yet support all three intended builds:

- Hare has no attack pattern, so an attack-amount reward currently provides no meaningful hare build identity and should not appear in the slice reward pool without a corresponding rule change.
- Warren needs a measurable protection behavior and at least one crowding or reproduction-control choice.
- Gardeners needs upgrades that affect feeding efficiency and seed dispersal.
- Upgrade contribution telemetry must show movement attributable to mobility, prevented losses attributable to protection, and food/fern creation attributable to ecosystem upgrades.

These gaps should feed the separate Sprint 0 upgrade inventory task. They are not authorization to build a generalized modifier framework.

## First validation pass

Use the existing Forest Edge data as a baseline, then evaluate the same fixed seed set for the base hare and one minimal prototype of each build.

Record:

- hare extinction frequency and final population;
- fern minimum/final population and food consumed;
- hare births, starvation deaths, predator deaths, and crowding deaths;
- movement, successful seed drops, and protection activations;
- whether each build changes the visible population shape or survival story, not merely the final number.

Keep the selection only if players can distinguish the three strategies from board behavior and results evidence. If one build cannot be made legible with a small explicit upgrade catalog, replace that build before adding species.

## Exit decision

The vertical-slice working target is therefore:

- **Scenario:** Forest Edge
- **Player:** Hare
- **Support:** Fern
- **Opponent:** Fox
- **Builds:** Trailblazer, Warren, Gardeners

