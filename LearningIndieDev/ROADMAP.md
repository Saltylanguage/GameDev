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

### Milestone 4 - Tools and specialization

- A crude axe is crafted at the campfire for 2 wood and 2 stone with [Q].
- Once crafted, the HUD shows the tool and future trees take 4 hits instead of 6 and yield 8 wood instead of 4.
- Manual Unity playtest passed: campfire craft, axe craft, day reset, and improved tree reward all worked.
- Scope remains deliberately local; no generic item, equipment, or crafting framework was introduced.

### Milestone 5 - Visual foundation

- Normal play no longer uses floating world-name labels; the F3 panel remains the explicit debug view.
- The world now uses authored pixel-art atlases, repeated beach/ocean/jungle tiles, a clearer shoreline, and world-Y depth sorting for characters and props.
- Manual Bootstrap playtests passed for the shoreline composition, time-of-day colors, camp/shelter landmarks, and label-free normal view.
- The jungle entrance now has a native 3x2 closed/open tile set built from the project canopy and beach pixel language. Its Play Mode visual acceptance remains pending.

### Milestone 6 - Compact jungle-edge zone

- Clearing it uses the existing wood-chopping activity: hands take 8 hits for 4 wood; the crude axe takes 4 hits for 8 wood.
- The interaction marker and context prompt have been manually verified near the jungle edge.
- Manual verification passed: the cleared edge returns on a new day.
- The mechanic swaps a closed 3x2 tile set for its matching open route after chopping; confirm the closed, opened, and next-day-reset states in Play Mode.

### Milestone 7 - One survivor with a routine and useful skill

- Mara moves between the berry bush in the morning, jungle edge in the afternoon, and camp at night.
- Once per day, [E] sends Mara scavenging instantly and returns one bonus berry without consuming the player's activity slot.
- Mara's no-time-cost assignment and bonus berry have been manually verified.
- Manual verification passed: Mara cycles through the starting point, jungle edge, and campsite; she can be used once per day and resets after rest.

## Immediate next step - Close the authored visual slice

- Run one clean Unity Play Mode check of the closed/open jungle tile states, their next-day reset, and the existing gameplay loop.
- Keep rejected entrance experiments under local-only `artifacts/`; do not add them to the Unity project.
- Commit the intentional HUD, world, interaction, test, documentation, and authored-art changes as one coherent slice.

## Later milestones

1. Author a proper jungle entrance as a small set of native tiles or a deliberately matched terrain transition, then connect it to the existing jungle-edge state swap
2. Autonomous camp assignments
3. Versioned persistence
4. Presentation and atmosphere polish
