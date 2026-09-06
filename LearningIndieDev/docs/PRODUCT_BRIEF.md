# Cellular Automata Roguelike — Vertical-Slice Product Brief

## Promise

**Cellular automata as a roguelike:** the player guides one species through a short, deterministic ecosystem run by choosing upgrades to its rules, then watches those choices reshape survival, movement, feeding, reproduction, and competition on the board.

The slice succeeds when a new player can finish a run, describe what their upgrades changed, and identify the main cause of victory or defeat.

## Player experience and agency

The player selects the curated slice scenario and begins with a fixed player species and base ruleset. During simulation phases, the player observes the live board, pauses or changes speed, and inspects cells and species. The player does not directly move individual cells or edit raw parameters.

The primary decisions are upgrade choices. At each reward break, the player
chooses an eligible upgrade from the offered options or explicitly skips it.
Every option previews the affected rule, valid range, tradeoff, and expected
board consequence. The chosen upgrade becomes part of the expedition's ordered,
fingerprinted ruleset for all later phases. Skipping preserves the current build.

## Run and decision cadence

A run contains **ten simulation phases**. Each phase uses the configured phase length; 200 ticks is the current product target. Normal speed targets roughly one phase every two to three minutes, producing a **20–30 minute run** including inspection and choices.

The flow is:

1. Scenario briefing and starting ruleset.
2. Simulate the configured phase length, with 200 ticks as the current target,
   with pause, speed, and inspection controls available.
3. Show a short phase summary and offer three upgrades.
4. Apply one choice or record a skip, clearly preview any change, and continue
   the same ecosystem from its next tick.
5. After phase ten—or immediate extinction—show results, accomplishments, and earned unlocks.

There is no real-time decision timer. Simulation pauses automatically during upgrade selection. A player therefore makes nine build decisions per completed run, after phases one through nine.

Each phase preserves the board, creature/resource state, age, energy, cooldowns,
initial seed, absolute tick and accumulated history. A phase summary does not
restart the expedition. Explicitly ending or restarting the expedition is a
separate action. The controlled prototype now keeps the same world through its
phase decisions; telemetry and full product validation remain in the
[migration plan](CONTINUOUS_SIMULATION_FLOW_PLAN.md).

The prototype can tune phase length for testing; a configured 200-tick phase
takes about 20 seconds at normal speed.
The two-to-three-minute viewing target above remains a separate pacing decision;
the continuity migration does not silently change the configured step interval.

## Success, failure, and rewards

- **Victory:** the player species remains alive at the end of phase ten and finishes at or above the scenario's authored survival population threshold.
- **Narrow survival:** the species remains alive but finishes below the threshold. The run completes and records accomplishments, but does not grant the scenario-completion unlock.
- **Defeat:** the player species reaches zero population. The run ends immediately after the completed tick that caused extinction.

After every phase except the last, surviving players may choose one run upgrade
or skip. Phase summaries show that phase's population change, births, deaths by
cause, food consumed, movement, combat, and notable upgrade contributions.
Final results distinguish whole-expedition totals from individual phases.

The final results screen awards accomplishments for explicit feats such as victory, population recovery, efficient feeding, or surviving a named pressure. The first vertical slice grants **one predetermined meta-progression unlock on the first victory**. Unlock content may be a scenario, species, or eligible upgrade, but cannot provide raw permanent stat bonuses to the starting species.

## Persistence and replay

The slice saves settings, completed accomplishments, and versioned
meta-progression unlocks. A completed expedition records its seed, scenario ID,
base ruleset fingerprint, phase boundaries, ordered upgrades with acquisition
ticks and resolved values, and final result for reproduction and comparison.

An active expedition is **not saved to disk or restored after application exit**
in the initial slice. In-memory continuation across decision breaks is required.
Research checkpoints are a separate reproduction contract. Starting over is
explicit. Final results return the player to a next-expedition screen where the
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

New work belongs in the vertical slice only if it improves the upgrade decision, makes its simulation consequence easier to understand, enables the complete run-to-reward-to-next-run flow, or provides evidence needed to validate those outcomes. Everything else is deferred until the slice passes external comprehension and replay-intent testing.

