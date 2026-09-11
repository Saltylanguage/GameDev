# GalapagOS Desktop Feature Set Plan

> Status: Draft feature contract — ready for UX screen planning  
> Date: 2026-09-05  
> Scope: What the player can do through the GalapagOS desktop

## Purpose

Turn the desktop app list into a concrete player-facing feature set before
designing individual screens. Each surface below has one job, a defined set of
player verbs, an information boundary, and explicit links to related surfaces.

The desktop is the player's between-expedition home base. It supports
preparation, collection, permanent progression, personalization, learning, and
reflection. It does not replace the Simulation scene, the Developer Lab, or
the domain systems that validate purchases and results.

## Authority boundaries

The desktop must make these four progression/evidence layers visibly distinct:

| Layer | Meaning | Owned by |
| --- | --- | --- |
| Species Genome | Permanently unlocked options plus a configurable active set that affects every population of one species | Gene Lab |
| Expedition Mutations | Nine temporary adaptations acquired during one continuous expedition | Simulation reward breaks |
| Environmental research | Permanent biome, terrain, resource, or scenario changes | Biome / Ecology Lab |
| Evidence and history | What happened in completed phases and expeditions | History / Data Record |

The desktop can preview or link to the other layers, but it must not silently
apply a Mutation, recreate an expedition, or turn a read-only record
into a progression purchase.

## Feature inventory

| ID | Surface | Player question | Priority |
| --- | --- | --- | --- |
| D-00 | Desktop Home / Overview | What changed, and what is useful to do next? | Core shell |
| D-01 | Settings | How do I make the game work for me? | Core |
| D-02 | Species Collection | What organisms have I discovered and what do they do? | Core |
| D-03 | Gene Lab | Which biological research can I unlock or pursue? | Core |
| D-04 | My PC | How do I make this desktop feel like mine? | Core |
| D-05 | Music Player | What should the desktop and expeditions sound like? | Core |
| D-06 | History / Data Record | What happened in my expeditions, and what patterns can I see? | Core |
| D-07 | Biome / Ecology Lab | How do environments work and what can I unlock for them? | Core |
| D-08 | Expedition Planner / Launchpad | What expedition do I want to prepare and launch? | Core |
| D-09 | Field Guide / Research Journal | What have I learned about this world? | Recommended |
| D-10 | Research Inbox / Bulletin | What new information or action needs my attention? | Recommended |
| D-11 | Habitat Gallery / Museum | What discoveries and accomplishments do I want to display? | Expansion |
| D-12 | Quick Search / Jump | Where is the thing I remember seeing? | Platform utility |

## Core surfaces

### D-00 — Desktop Home / Overview

**Purpose:** Orient the player when they return to the Lab.

**Player can:**

- see the profile name and persistent scientific-data balances;
- review the latest completed expedition or result summary;
- see a short “next useful action” recommendation without being forced into it;
- see newly discovered species, unlocked research, and pending notifications;
- launch Expedition Planner;
- jump directly to the selected recommendation, species, biome, or result.

**Owns:** desktop landing state and recent-summary presentation. It does not
own progression, purchase validation, or active-expedition continuation.

**Key links:** History, Species Collection, Gene Lab, Biome Lab, Expedition
Planner, Research Inbox.

**Design principle:** This is a welcoming home screen, not a wall of charts.
One prominent recent result and one clear next action should carry the page.

### D-01 — Settings

**Purpose:** Change how the game behaves and presents itself.

**Player can:**

- change display mode, resolution, window/fullscreen behavior, UI scale, and
  pixel-scaling preferences;
- configure keyboard and mouse controls;
- adjust audio mix, master volume, effects, music, and ambient volume;
- configure accessibility options, color support, text/readability options,
  and reduced motion;
- control notifications and desktop behavior;
- restore defaults with confirmation.

**Owns:** behavioral and accessibility preferences. Settings does not own
wallpapers, desktop themes, icon arrangement, or progression.

**Key links:** My PC for appearance, Music Player for content selection.

### D-02 — Species Collection

**Purpose:** Give the player a clear, collectible catalogue of organisms.

**Player can:**

- browse All, Plants, Herbivores, Carnivores, and future role categories;
- filter by Unlocked, Undiscovered, Mastery in Progress, and Favorites;
- inspect a species card showing its glyph, role, discovery state, primary
  biome, mastery progress, and short ecological identity;
- open a detailed record with behaviors, needs, known interactions, and useful
  observed metrics;
- favorite or pin a species for quick access;
- compare two discovered species when the record format supports it;
- open the selected species directly in Gene Lab, History, or Expedition
  Planner when those actions are valid.

**Owns:** collection presentation, discovery state, species identity, mastery
display, and favorites. It does not own species rules or purchase validation.

**Key links:** Gene Lab, History, Field Guide, Expedition Planner.

**First-slice emphasis:** Hare is the first hero species; fox provides a clear
pressure contrast; plant/resource presentation remains legible even before a
dedicated fern glyph is available.

### D-03 — Gene Lab

**Purpose:** Let the player understand and eventually purchase permanent Genome
options, then configure which unlocked options are active for the selected
species.

**Player can:**

- browse species, with Plant, Herbivore, and Carnivore as filters;
- select a species, beginning with Hare for the vertical slice;
- inspect Genome nodes, prerequisites, costs, species effects, ecological
  obligations, active-capacity use, and resulting choices;
- see locked, available, affordable, unaffordable, selected, unlocked, active,
  inactive, and newly available states;
- preview how a Genome change affects that species and its wider ecology in
  future expeditions;
- confirm a valid Genome purchase;
- turn unlocked nodes on or off between simulations without losing them;
- open the selected species record in Species Collection.

**Owns:** each species' permanent Genome unlocks, active configuration, and
purchase preview. It does not own temporary phase rewards or Mutations.

**Key links:** Species Collection, Expedition Planner, History, Research Inbox.

**Progression rule:** Unlocked nodes persist. The active Genome is frozen at
simulation launch and applies to every population of its species, including
when the species is not player-controlled. A Genome may improve stats or unlock
behaviours, but it must keep ecological costs and wider balance visible under
[`SG-005`](Studio%20Guidelines/SG-005-UPGRADE-AND-ECOLOGY-BALANCE.md).

### D-04 — My PC

**Purpose:** Make the home base feel personally owned.

**Player can:**

- choose unlocked wallpapers/background scenes;
- select desktop themes such as Meadow Morning, Sunset Field Notes, Rainy
  Study, or Spring Growth;
- arrange desktop icons and choose pinned taskbar apps;
- enable or disable optional widgets;
- select available icon/cursor treatments;
- review collected desktop decorations, badges, and unlockable ornaments;
- view profile identity and local personalization status.

**Owns:** appearance, arrangement, and personalization. It does not own actual
display settings, controls, audio mix, or profile creation.

**Key links:** Settings, Music Player, Habitat Gallery.

**Usability rule:** Personalization should be optional. The player must never
need to rearrange the desktop to reach core content.

### D-05 — Music Player

**Purpose:** Make the desktop and expeditions feel inhabited and personal.

**Player can:**

- choose Desktop, Simulation, Results, and Ambient playlists;
- select or unlock tracks;
- set shuffle, repeat, and mute behavior;
- see the current track in the taskbar;
- choose ambient layers such as meadow, forest, rain, or research-room tone.

**Owns:** playlist and track selection. Settings owns the audio mix, volume,
and accessibility behavior.

**Key links:** Settings, My PC, Habitat Gallery, Research Inbox for music
unlock notifications.

### D-06 — History / Data Record

**Purpose:** Help the player understand what happened and make better future
choices.

**Player can:**

- review completed Expedition Logs;
- inspect a Phase Timeline for each expedition;
- compare species performance across compatible records;
- inspect Biome Reports and scenario outcomes;
- review achievements and milestones;
- favorite or pin notable expeditions;
- open the related species, Mutation, Genome, biome, or scenario in its owning
  app.

**Record fields may include:** scenario ID, player species, seed, base ruleset
fingerprint, per-species Genome fingerprints, phase boundaries, ordered
Mutations, final result, population
history, births, food events, movement, combat, and mortality causes where the
metric is valid for that record.

**Owns:** read-only result history, comparison views, accomplishments, and
milestone display. It does not award data, purchase research, or reinterpret
invalid evidence as a score.

**Key links:** Species Collection, Gene Lab, Biome Lab, Field Guide.

**Important distinction:** The app must label phase versus expedition windows.
A ten-phase continuous expedition is not ten fresh runs, and a fresh research
window must not be presented as an equivalent gameplay expedition.

### D-07 — Biome / Ecology Lab

**Purpose:** Let the player understand and unlock environmental progression.

**Player can:**

- browse biomes and scenarios;
- inspect terrain, resources, movement costs, and environmental pressures;
- see species compatibility, food relationships, and notable ecological risks;
- inspect biome-specific research and upgrade nodes;
- purchase valid permanent environmental research;
- review unlocked scenarios and documented best attempts;
- open a selected biome directly in Expedition Planner.

**Owns:** environmental research, biome/scenario access, terrain/resource
knowledge, and ecology-specific unlocks.

**Boundary:** Gene Lab changes organism traits and species research. Biome Lab
changes terrain, resources, environmental rules, and scenario access.

### D-08 — Expedition Planner / Launchpad

**Purpose:** Turn the player's current knowledge and unlocks into a valid new
expedition.

**Player can:**

- choose an unlocked scenario and biome;
- choose an eligible player species;
- choose Species Simulation or Biome Simulation when both are available;
- review starting conditions and the frozen active Genome of every participating
  species;
- in Species Simulation setup, inspect which Mutations may be available to the
  selected species;
- in Biome Simulation setup, see that Mutations are not part of the mode;
- review the launch summary, seed policy, and starting ruleset;
- launch the expedition or cancel back to the desktop;
- open related species, Gene Lab, or Biome Lab surfaces before committing.

**Owns:** launch preparation and immutable launch-request composition. It does
not purchase Genome research, grant Mutations, or resume a destroyed
Lab scene.

**Boundary:** Mutation selection begins in Simulation. Phase reward decisions remain
inside the same continuous expedition and do not route back through the Lab.
This reward flow exists only in Species Simulations.

## Recommended supporting surfaces

### D-09 — Field Guide / Research Journal

**Purpose:** Teach and reward understanding without turning the analytics app
into a textbook.

**Player can:**

- read species facts and ecology terms;
- review discovered behaviors and interactions;
- browse biome notes, glossary entries, and short system explanations;
- revisit tutorial guidance;
- open the relevant species or biome record.

**Owns:** qualitative knowledge entries and discovery explanations. It does not
own raw metrics or progression purchases.

### D-10 — Research Inbox / Bulletin

**Purpose:** Surface meaningful changes without making the player hunt through
apps.

**Player can:**

- review new species, biome, research, music, or desktop unlock notices;
- jump to the app that can act on a notice;
- mark notices read or dismiss them;
- review pending result and accomplishment summaries.

**Owns:** notification/read state only. It must not create a second reward
wallet or silently claim progression.

### D-11 — Habitat Gallery / Museum

**Purpose:** Give long-term discoveries a place to remain visible.

**Player can:**

- display species sets, biome discoveries, achievement plaques, and favorite
  expedition snapshots;
- preview and equip unlockable wallpapers or desktop ornaments;
- browse completed collection milestones;
- jump to the originating species, biome, or history record.

**Owns:** display and collection presentation. It does not become a second
research tree or achievement rules engine.

### D-12 — Quick Search / Jump

**Purpose:** Make a growing desktop easy to navigate.

**Player can:**

- search species, biomes, research projects, achievements, music, and history;
- filter results by app or category;
- open the result in its owning app with context preserved.

**Owns:** no content. It is a navigation utility that prevents app count from
becoming menu friction.

## Desktop-level features

These are platform features shared by the apps rather than separate apps:

- app icons, labels, pinned taskbar apps, and recent-app state;
- controlled window open/close/focus behavior;
- contextual deep links between apps;
- persistent profile and scientific-data summary;
- notification badges and Research Inbox routing;
- optional widgets for next research goal, latest result, current music, and
  collection progress;
- unlockable wallpapers, themes, icon treatments, and ambient desktop scenes;
- clear empty, locked, unavailable, representative-data, and error states.

## Proposed first feature contract

Before detailed UI design, the following should be treated as the first solid
feature inventory:

### Core desktop loop

```text
Desktop Home
    -> inspect latest result or next objective
    -> Species Collection / Gene Lab / Biome Lab
    -> Expedition Planner
    -> Simulation
    -> Results
    -> Desktop Home
```

### First complete player loop

1. Return to Desktop Home after a completed expedition.
2. Read the result and newly available data/unlocks.
3. Open Species Collection to review the player species.
4. Open Gene Lab or Biome Lab to inspect an available permanent option.
5. Confirm a valid permanent purchase when the economy is connected.
6. Open Expedition Planner and prepare the next expedition.
7. Launch the new expedition.

### First personalization loop

1. Open Research Inbox after a discovery or accomplishment.
2. Open My PC and equip an unlocked wallpaper or theme.
3. Pin Species Collection or Expedition Planner to the taskbar.
4. Choose a desktop playlist in Music Player.

## Recommended delivery order

1. Desktop Home, app shell, Settings, My PC, and Expedition Planner.
2. Species Collection and Gene Lab with the Hare vertical slice.
3. Biome / Ecology Lab with Forest Edge.
4. History / Data Record with phase-versus-expedition labeling.
5. Field Guide and Research Inbox.
6. Music unlocks, Habitat Gallery, widgets, and richer personalization.
7. Quick Search once the app/content count justifies it.

Each surface should receive a one-page screen contract and representative data
states before detailed visual design begins.

## Open decisions before UI design

1. Which apps are visible on the first desktop and which unlock later?
2. Is Desktop Home a full app window, a permanent desktop surface, or both?
3. Does the first Gene Lab expose only Herbivore/Hare research, with other tabs
   visible but content-light?
4. Which first Biome Lab actions are real progression and which are read-only
   scenario information?
5. Do the first Biome projects provide optional habitat tools or a small
   restoration baseline required before advanced Biome challenges?
6. Which History metrics are player-facing in the vertical slice versus later
   evidence expansion?
7. Are achievements displayed primarily in History, Habitat Gallery, or both?
7. Which personalization features are worth producing for the first slice?
