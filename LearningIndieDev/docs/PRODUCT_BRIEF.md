# Cellular Automata Roguelike — Vertical-Slice Product Brief

> **Current M2 reading — 2026-11-14:** The [roadmap's ten-outcome M2 contract](../ROADMAP.md#m2-outcome-contract--consolidated-2026-11-14)
> and [S6 predator-first journey decision](Sprints/S6-control-record.md#november-9-predator-first-design-decision)
> govern the current slice. One six-round simulation is a node in the current
> map journey. The played species carries
> only its population count into a fresh next node; other species and resources
> reset to authored counts. Avoidance, survival, and faster reproduction are the
> three introductory responses to Fox pressure. The six-round node cadence is
> the accepted M1 baseline; the complete journey and its player acceptance are
> current M2 work. No balance value or M2 acceptance is implied.

> **Earlier journey decision — 2026-10-14:** The original six-round victory,
> failure, and immediate currency-award rules described the standalone M1 slice.
> That meeting established whole-journey failure with no cash-out and permanent
> unlocks surviving it. The later Field Data and Gene Lab implementations have
> advanced the settlement and spending contracts; their final balance and M2
> player acceptance are still open. See the [meeting decisions](Meeting%20Briefs/2026-10-14-Simulation-Focus-and-Journey-Economy-Notes.md#meeting-decisions)
> and [journey prototype](JOURNEY_PROTOTYPE.md).

## Promise

The [Field Data baseline](FIELD_DATA_JOURNEY_DESIGN.md) defined the five-node
economy test. The current implementation includes journey map, reward, shop,
settlement, and Gene Lab paths; the [S6 control](Sprints/S6-control-record.md)
records the current route and unresolved playability decisions.

**Cellular automata as a roguelike:** the player guides one species through a
short, deterministic ecosystem expedition by choosing temporary **Mutations**,
then watches those choices reshape survival, movement, feeding, reproduction,
and competition on the board.

The slice succeeds when a new player can complete and replay the representative
journey, describe what their Mutations changed, identify leading causes of
victory or defeat, and use one earned permanent unlock on the next journey.

The vertical slice proves a local Species Simulation journey. The larger game lets
the player permanently unlock species **Genome** options, configure which ones
are active, and use their interactions in Biome Simulations to establish a
diverse, resilient ecosystem. Dominating with one species or completing every
collection entry is not the only measure of progress.

The two simulation modes have different goals. Species Simulations focus on a
chosen species and include temporary Mutations. Biome Simulations contain no
Mutations; they test active Genome combinations across participating species
against authored biodiversity, stability, balance, and recovery goals. Genome
unlocks remain earned even when a particular active configuration performs
poorly.

## Player experience and agency

The player enters the curated Forest Edge/Hare journey through the Desktop
profile and launch flow. The scenario and species choice may be constrained,
but the player must understand that choice and the opening Fox pressure. During
simulation nodes, the player observes the live board, pauses or changes speed,
and inspects cells and species. The player does not directly move individual
cells or edit raw parameters.

The primary decisions are temporary Mutation choices during a simulation node,
followed by map, reward, and shop decisions between nodes. The introductory
choices should offer understandable avoidance, survival, and faster-reproduction
responses to Fox overhunting, subject to fresh-profile availability review.
Each choice needs a small icon and keyword, a visible effect and tradeoff, and
an honest explanation in the following phase and result. A Mutation lasts only
for the current journey.

## Run and decision cadence

A simulation node contains **six rounds**, each lasting **10 seconds of
simulation time**. The journey connects multiple nodes and decisions. No
real-time duration target is set; elapsed time varies with simulation speed,
pauses, and choices.

The flow is:

1. Select the profile and curated route, then read the scenario introduction.
2. Play six rounds in one continuing simulation node, with pause, speed, and
   inspection controls available.
3. At round boundaries, choose a Mutation or Skip; within the node, preserve
   the evolving ecosystem.
4. At node end, read the result and choose the next map, reward, or shop action.
   The next simulation node starts from authored data plus the played species'
   carried population count.
5. At journey end, settle any earned permanent reward and return to the Desktop
   or next-journey flow.

There is no real-time decision timer. Simulation pauses automatically during
Mutation selection. The accepted M1 node had five Mutation decision points,
after rounds one through five; the representative M2 journey must make its
actual decision cadence understandable without relying on that older total.

Each round within one node preserves the board, creature/resource state, age,
energy, cooldowns, seed, absolute tick, and history. At the next node, only the
played species' count carries; its individuals are recreated and other species
and resources return to authored counts. Restart is not a player action. End
abandons the journey after confirmation and forfeits unbanked Field Data and
temporary Mutations. Pause is available.

The round duration is simulation time, not wall-clock time. Do not infer a
player-facing real-time pacing target from the 10-second simulation duration.

## Success, failure, and rewards

- **Journey success:** complete the authored route and settle the permanent
  reward. Exact phase-clear and route-survival expectations remain under S6 review.
- **Failed journey:** player-species extinction ends the journey and awards no
  new permanent cash-out.
- **Player-ended journey:** confirming End abandons the active route and
  forfeits unbanked Field Data and temporary Mutations.

The current journey has round payouts, shops, and permanent success settlement.
Final results must distinguish journey totals from individual nodes and explain
the leading observed causes and supported Mutation contributions. Final payout
values and player-facing balance remain open.

The Gene Lab application on the GalapagOS Desktop uses permanent currency to
research **Genome Upgrades** for a species' **Genome** skill tree. M2 needs one
earned unlock to persist through a real application relaunch and apply on the
next journey. Owning, equipping, and journey eligibility must remain distinct.

## Persistence and replay

The slice saves settings, completed accomplishments, and versioned
meta-progression. Reproducible evidence records the route identity, seed,
starting rules and Genome state, ordered actions, and result; S6 owns the
comparison and review contract.

An active journey is **not saved to disk or restored after application exit**
in the initial slice. In-memory continuation across decisions is required.
Research checkpoints are a separate reproduction contract. After results, the
player can return to the Desktop and start the next journey; an earned unlock
must be visible and usable when applicable.

## Launch target

The initial commercial target is **Steam on 64-bit Windows PC**, with keyboard and mouse as the required input path. Common desktop resolutions and windowed/fullscreen play are required. Controller support, Steam Deck verification, macOS, Linux, consoles, and mobile are later validation or porting decisions rather than slice commitments.

The presentation target is readable at 1920×1080 and remains functional at 1280×720. Performance budgets will be set against the selected slice scenario before optimization claims are made.

## Explicit non-goals for the vertical slice

- Direct control of individual cells, action-game combat, or mid-tick rule editing.
- Balancing every authored species or shipping multiple full scenarios.
- A universal rule scripting, modifier, behavior-plugin, or event-bus framework.
- Player disk save/load of unfinished expeditions, online multiplayer,
  leaderboards, or live services.
- Steam Deck verification, console/mobile ports, or platform achievements.
- Large-scale procedural worlds, cave production, colony construction, ant tunnels, or beaver dams.
- Final-volume art/audio, localization, and broad accessibility coverage beyond
  the M2 baseline. M2 still needs an initial audio language, readable art,
  visible focus, text/contrast, mute, and essential reduced-motion feedback.

## Evaluation rule

New work belongs in M2 when it closes one of the roadmap's ten outcomes on the
representative player route or produces evidence needed to validate it. S6
concentrates on the journey's legitimate outcomes, three responses, causal
feedback, and player review. S7 is reforecast from that evidence and integrates
the menu-to-next-journey route for external comprehension and replay testing.

