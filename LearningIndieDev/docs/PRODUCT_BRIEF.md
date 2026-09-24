# Cellular Automata Roguelike — Vertical-Slice Product Brief

## Promise

**Cellular automata as a roguelike:** the player guides one species through a
short, deterministic ecosystem expedition by choosing temporary **Mutations**,
then watches those choices reshape survival, movement, feeding, reproduction,
and competition on the board.

The slice succeeds when a new player can finish an expedition, describe what
their Mutations changed, and identify the main cause of victory or defeat.

The vertical slice proves one local Species Simulation. The larger game lets
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

The player selects the curated slice scenario and begins with a fixed player species and base ruleset. During simulation rounds, the player observes the live board, pauses or changes speed, and inspects cells and species. The player does not directly move individual cells or edit raw parameters.

The primary decisions are Mutation choices. After rounds one through five, the
player sees three Mutations and chooses one or Skip. Round one starts without a
Mutation choice; finishing round six leads to results without another choice.
Each Mutation is presented with a small icon and keyword (for example,
“hunting +1”), not a full statistic breakdown. Its effect should be predictable
at a glance and apparent during the following round. A Mutation is temporary
and lasts only for its current run.

In the active experimental offer, the third choice is always Reinforcements:
it adds one individual of the selected species to the following round at a
deterministically selected unoccupied, passable cell. It can be selected again
at later phase decisions, subject to available space and the population limit.

## Run and decision cadence

A run contains **six simulation rounds**, each lasting **10 seconds of
simulation time**—one minute of simulation time in total. No real-time duration
target is set; actual elapsed time varies with simulation speed and pauses.

The flow is:

1. Scenario briefing and starting ruleset.
2. Simulate round one for 10 seconds of simulation time, with pause, speed, and
   inspection controls available.
3. After rounds one through five, pause and offer three Mutations or Skip.
   Continue the same ecosystem into the next round.
4. After round six, show results and currency earned from the performance
   measure under development. Additional bonus-event rewards are possible but
   not decided.

There is no real-time decision timer. Simulation pauses automatically during
Mutation selection. A player has five Mutation decision points per run, after
rounds one through five. A possible Skip bonus is undecided and non-blocking;
if adopted, it uses the same currency as Genome Upgrades. Its amount and limits
remain open.

Each round preserves the board, creature/resource state, age, energy, cooldowns,
initial seed, absolute tick and accumulated history. A round summary does not
restart the expedition. Restart is not a player action. End abandons the run
after the player confirms Yes; ending before completing round six forfeits all
rewards. Pause is
available. The controlled prototype keeps the same world through its round
decisions; telemetry and full product validation remain in the
[migration plan](CONTINUOUS_SIMULATION_FLOW_PLAN.md).

The round duration is simulation time, not wall-clock time. Do not infer a
player-facing real-time pacing target from the 10-second simulation duration.

## Success, failure, and rewards

- **Victory:** the player species is not extinct at the end of round six. This
  survival rule applies in any biome.
- **Failed run:** the player species goes extinct. The run ends immediately and
  awards no rewards.
- **Player-ended run:** confirming End abandons the run; if used before round
  six, it awards no rewards.

The successful-run currency award is based on the performance measure currently
in development, not simply final population. Possible bonus events may award
additional currency, but that is not decided. Final results distinguish
whole-expedition totals from individual rounds.

The Gene Lab application, opened from the GalapagOS Desktop, uses currency to
buy permanent **Genome Upgrades** that fill a species' **Genome** skill tree.
The exact performance-to-currency calculation is linked to the measure now in
development. Additional bonus-event rewards remain optional.

## Persistence and replay

The slice saves settings, completed accomplishments, and versioned
meta-progression. A completed expedition records its seed, scenario ID, base
ruleset fingerprint, round boundaries, ordered Mutations with acquisition
ticks and resolved values, and final result for reproduction and comparison.

An active expedition is **not saved to disk or restored after application exit**
in the initial slice. In-memory continuation across decision breaks is required.
Research checkpoints are a separate reproduction contract. Starting a new run
is a separate action after results. Final results return the player to a next-expedition screen where the
earned unlock is visible and usable when applicable.

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
- Final-volume art, music, sound effects, localization, or accessibility coverage before the slice direction is validated.

## Evaluation rule

New work belongs in the vertical slice only if it improves the Mutation
decision, makes its simulation consequence easier to understand, enables the
complete expedition-to-reward-to-next-expedition flow, or provides evidence
needed to validate those outcomes. Everything else is deferred until the slice
passes external comprehension and replay-intent testing.

