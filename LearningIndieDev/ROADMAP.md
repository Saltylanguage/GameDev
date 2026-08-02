# Island survival roadmap

## Completed

### Milestone 1 - Prepare for Night

- Gather wood and stone.
- Build a persistent campfire.
- Track the first daily objective and reset renewable resources on a new day.

### Milestone 2 - Eat and Rest

- Hunger and energy are explicit, recoverable player needs.
- Every activity uses one shared, tunable hunger/energy cost model.
- The campfire cooks two berries into one cooked meal.
- Eating cooked meals restores more hunger than raw berries.
- Sleeping at the campfire is the normal route to a new morning; hungry sleep reduces energy recovery.
- Night now waits for player sleep instead of silently starting a new day.
- Runtime coverage proves sleeping resets renewable targets and begins the next morning.
- Play Mode coverage proves Bootstrap can complete the build, cook, eat, and sleep loop in Unity's frame loop.

## Current limitations

- Needs are intentionally forgiving and have no game-over state.
- One campfire recipe exists; there is no general recipe system.
- Campfire/sleep presentation remains placeholder-quality.
- Save/load does not exist yet.

### Milestone 3 - Weather the Storm

- Building the campfire advances the objective to a Day 2 storm forecast.
- The player can build a 4-wood shelter beside the campfire.
- The shelter marker is deliberately withheld until Night on Day 2, when it blinks above the site as an urgent preparation cue.
- Sleeping on Day 2 resolves the authored storm: shelter prevents exposure; an open camp loses 25 energy and gains 20 hunger, but remains recoverable.
- The F3 diagnostics panel now starts hidden, keeping the default game view player-facing.
- Verification: Edit Mode 17/17 and Play Mode 3/3, including the Bootstrap shelter/storm path.

## Next milestone - Tools and specialization

Add one simple tool upgrade that changes a demonstrated activity decision. Do not introduce generic item, equipment, or crafting frameworks until there is more than one real consumer.

## Later milestones

1. Explore one compact jungle-edge zone
2. Add one survivor with a routine and useful skill
3. Autonomous camp assignments
4. Versioned persistence
5. Presentation and atmosphere polish
