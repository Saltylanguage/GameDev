# GalapagOS Desktop App Ecosystem Plan

> Status: Draft product and UX plan — concept baseline approved, app scope open  
> Date: 2026-09-05  
> Scope: Player-facing desktop apps inside the GalapagOS Lab

> The detailed player-facing feature contract is in
> [`GALAPAGOS_DESKTOP_FEATURE_SET_PLAN.md`](GALAPAGOS_DESKTOP_FEATURE_SET_PLAN.md).

## Purpose

Define a lively but useful desktop environment around the existing Lab
contract. The desktop should give the player a memorable home base, quick paths
between related systems, and meaningful personalization without turning every
feature into a separate layer of menu friction.

The desktop is a player-facing presentation and navigation surface. The
developer Lab remains separate and must not leak tuning, debug, or authoring
tools into these apps.

## Desktop model

The approved visual direction is the light pastel eco-desktop shown in
[`ART_STYLE_GUIDE.md`](../ART_STYLE_GUIDE.md): Meadow Desktop as the shell,
Lab Notebook as the information-dense surface, and controlled Classic Eco OS
window behavior for selected utilities.

The desktop has four persistent anchors:

1. A bright ecology background or unlockable desktop scene.
2. A small set of labeled app icons arranged in groups.
3. A bottom taskbar for pinned/recent apps and quick status.
4. A profile/data area that stays available without dominating the screen.

Apps can open as controlled windows with a title, icon, close/back behavior,
tabs, and a clear primary action. Freeform overlapping windows are a visual
option, not a requirement for every app. The player should always be able to
reach an important destination in one or two actions.

## App groups

```text
OBSERVE       Species Collection · History / Data Record · Field Guide
DEVELOP       Gene Lab · Biome / Ecology Lab
PREPARE       Expedition Planner / Launchpad
PERSONALIZE   My PC · Music Player · Settings
```

The first desktop can show all core apps as icons. Later apps, widgets, and
themes may unlock into the same space without changing the core navigation.

## Core apps

### 1. Settings

**Purpose:** Change how the game behaves and presents itself.

Suggested sections:

- audio volume and mix;
- display mode, resolution, UI scale, and pixel scaling;
- controls and keyboard remapping;
- accessibility hints, color support, text options, and reduced motion;
- notification and desktop behavior.

Settings owns behavior and accessibility. It does not own wallpapers, icon
arrangement, themes, or desktop decoration; those belong to My PC.

Useful links: My PC for appearance, Music Player for playlist selection.

### 2. Species Collection

**Purpose:** Browse every discovered organism and understand its role at a
glance.

Suggested structure:

- tabs or filters for All, Plants, Herbivores, Carnivores, and future roles;
- secondary filters for Unlocked, Undiscovered, Mastery in progress, and
  Favorites;
- compact specimen cards showing glyph, name, role, discovery state, mastery,
  primary biome, and one-line ecological identity;
- a selected-species detail sheet with behavior, known interactions, observed
  statistics, and unlock guidance;
- a direct **Open in Gene Lab** action for the selected species;
- optional Compare and Pin/Favorite actions once the core record is stable.

Collection is primarily read-only. Favoriting and pinning are personalization,
not progression purchases.

### 3. Gene Lab

**Purpose:** Browse and eventually purchase organism-focused permanent research
and upgrade trees.

Suggested structure:

- species picker shared with Species Collection;
- Plant, Herbivore, and Carnivore research tabs;
- a central branching tree with locked, available, affordable, selected,
  purchased, and newly available states;
- a detail panel showing prerequisites, data cost, effect, and what becomes
  available next;
- a direct **Open in Species Collection** action;
- a **Preview in Expedition Setup** action for choices that will affect a
  future launch.

The Gene Lab must clearly distinguish permanent research from temporary
per-expedition upgrades. Phase reward choices remain in the Simulation scene;
the Gene Lab prepares the persistent choices available to future expeditions.

### 4. My PC

**Purpose:** Personalize the player's desktop identity and appearance.

Suggested sections:

- profile card and player name;
- wallpaper/background selection using unlocked biome scenes, field sketches,
  and expedition snapshots;
- theme presets such as Meadow Morning, Sunset Field Notes, Rainy Study, and
  Spring Growth;
- app icon arrangement, pinned taskbar apps, and optional desktop widgets;
- cursor/icon style choices where useful;
- display of collected desktop decorations, badges, and unlockable ornaments.

My PC should make the home base feel owned by the player. It should not become
a second Settings screen or require the player to rearrange the desktop to use
the game.

### 5. Music Player

**Purpose:** Choose the audio identity of the desktop and expeditions.

Suggested features:

- Desktop, Simulation, Results, and ambient playlist categories;
- currently playing track and compact taskbar control;
- volume, mute, shuffle, and repeat;
- track unlocks tied to biomes, achievements, or discoveries;
- optional ambient layers such as meadow, forest, rain, and research-room tone.

The Music Player chooses content. Settings owns the global audio mix and
accessibility behavior.

### 6. History / Data Record

**Purpose:** Let the player understand what happened across expeditions and
identify patterns worth trying again.

Suggested tabs:

- Expedition Log;
- Phase Timeline;
- Species Performance;
- Biome Reports;
- Achievements and Milestones;
- Saved comparisons or favorite runs.

Useful metrics include scenario, player species, seed, expedition/phase
identity, final population, survival, data earned, upgrades acquired, births,
food events, combat, and mortality causes where the evidence is valid.

The screen should provide direct links to Species Collection, Gene Lab, and
Biome Lab for the selected species, upgrade, or scenario. Expedition history
must distinguish an ongoing multi-phase expedition from a new expedition and
must not collapse incomparable phase and expedition windows into one number.

### 7. Biome / Ecology Lab

**Purpose:** Browse biome-specific progression and understand how environment
changes the ecosystem.

Suggested structure:

- biome/scenario tabs;
- terrain and resource profile;
- ecological pressures and compatible species;
- biome-specific research and upgrade tree;
- unlocked scenarios, field notes, and best documented runs;
- direct **Open in Expedition Setup** action.

The boundary is simple:

- Gene Lab changes organism traits, species options, and biological research.
- Biome / Ecology Lab changes terrain, resources, environmental rules, and
  scenario access.

### 8. Expedition Planner / Launchpad

**Purpose:** Represent the existing Expedition Setup contract as a first-class
desktop app.

Suggested features:

- scenario and biome selection;
- player species selection;
- starting options and unlocked persistent choices;
- a compact expedition summary;
- launch, back, and confirmation behavior;
- shortcuts from Species Collection, Gene Lab, Biome Lab, and History.

This should be the most action-oriented app and the easiest route from desktop
to a new expedition.

## Recommended additional apps

### 9. Field Guide / Research Journal

A qualitative discovery log for species facts, ecology terms, field sketches,
observed interactions, and short explanations of systems. History answers
“what happened?”; the Field Guide answers “what have I learned?”

This is a strong place for tutorial entries and discoveries without making the
History screen feel like a textbook.

### 10. Research Inbox / Bulletin

A lightweight notification app for meaningful events:

- new species discovered;
- new biome or upgrade unlocked;
- expedition result ready to review;
- achievement completed;
- a previously locked research path becoming available.

It should collect actionable notifications, not manufacture busywork. A small
unread marker on the desktop or taskbar is enough when nothing requires action.

### 11. Habitat Gallery / Museum

An optional long-term collection app for displaying completed species sets,
biome discoveries, achievement plaques, unlocked wallpapers, and favorite
expedition snapshots. This supports personalization and gives discoveries a
place to remain visible after their first unlock.

The Museum should be a display and reflection surface, not a second progression
tree.

## Desktop liveliness and personalization

Useful, low-friction ways to make the desktop feel alive:

- time-of-day changes in the background scene;
- small ambient wildlife or plant motion behind windows;
- taskbar notifications for discoveries and completed research;
- unlockable wallpapers from biomes and expedition results;
- themed icon sets tied to plant, herbivore, carnivore, or biome collections;
- optional widgets for current expedition, next research goal, weather/biome,
  and music;
- app pinning and icon rearrangement through My PC;
- a small desktop pet or ambient creature only if it remains optional and does
  not compete with readable app icons.

## Cross-app quality-of-life links

The most valuable convenience behavior is contextual linking:

| Source | Direct destination |
| --- | --- |
| Species Collection | Gene Lab, History, Expedition Planner |
| Gene Lab | Species Collection, Expedition Planner |
| Biome / Ecology Lab | Expedition Planner, History, Field Guide |
| History / Data Record | Species Collection, Gene Lab, Biome Lab |
| Field Guide | Species Collection, Biome Lab |
| Research Inbox | The app that can act on the notification |
| My PC | Settings, Music Player, Habitat Gallery |

No link should silently spend data, mutate a run, or launch an expedition.
Links should open the relevant app in the correct context and leave the player
one explicit action away from the next decision.

## Delivery order

### Desktop foundation

- Meadow Desktop background and icon/taskbar shell;
- Settings;
- My PC;
- Music Player;
- Expedition Planner / Launchpad.

### Progression and collection

- Species Collection;
- Gene Lab;
- Biome / Ecology Lab;
- cross-app links and selected-species context.

### Evidence and long-term identity

- History / Data Record;
- Field Guide / Research Journal;
- Research Inbox / Bulletin;
- achievements and meaningful notification states.

### Optional expansion

- Habitat Gallery / Museum;
- richer widgets, desktop pets, and additional unlockable personalization.

Each app should first receive a low-fidelity screen contract and representative
states before receiving bespoke pixel art. The desktop shell should be proven
with a small set of apps before the full ecosystem is produced.

## Open decisions

1. Which apps appear on the first desktop versus unlock after progression?
2. Does History own achievements entirely, or does Museum become their primary
   display surface later?
3. Are desktop backgrounds static unlockable scenes, animated scenes, or both?
4. Which personalization options are meaningful for the vertical slice without
   becoming a production sink?
5. What is the first permanent Gene Lab catalog, and which upgrades remain
   exclusively in the Simulation reward flow?
