# Sprint 0 C2 — UX Contract and Low-Fidelity Layouts

> Status: Executed | Date: 2026-08-18 | Scope: Sprint 1 UI-only shell

This is the C2 contract for the first player-facing route. It narrows the
broader Main Menu/Lab delivery plan to the smallest demonstrable Sprint 1
flow:

```text
Launch -> Main Menu -> Lab Overview -> Research preview
                         ^             |
                         |------ Back -|
```

The UI uses representative data only. It does not spend currency, persist a
profile, launch a simulation, or expose Dev Lab controls.

## Screen contract

### Main Menu

Purpose: provide a calm entry point with one obvious next action.

| Element | State and behavior |
| --- | --- |
| Title and slice subtitle | Visible at both target resolutions. Subtitle identifies the Forest Edge research slice. |
| `Enter Lab` | Primary enabled action. Opens Lab Overview and gives focus to the Overview heading. |
| `Settings` | Visible but disabled and labeled `Prototype — Sprint 1`. No dead click. |
| `Credits` | Visible but disabled and labeled `Prototype — Sprint 1`. |
| `Quit` | Enabled desktop action. Opens a confirmation overlay before exiting. |

`Esc` on Main Menu opens the same quit confirmation. No data bar appears here;
currency is introduced in the Lab where it has a purpose.

### Lab Overview

Purpose: establish the Lab as the between-run home and point to the next useful
research action.

| Element | State and behavior |
| --- | --- |
| Persistent scientific-data bar | Shows Research, Plant, Herbivore, and Carnivore Data with text labels and numeric values. |
| Profile summary | `Forest Edge study · Hare focus · Representative data` |
| Last experiment summary | `No completed experiment in this fixture` with a clear empty state. |
| `Open Research` | Enabled. Opens Research preview with the first project selected. |
| `Species Archive` | Disabled and labeled `Prototype — later sprint`. |
| `Expedition Setup` | Disabled and labeled `Prototype — later sprint`. |
| `Back to Main Menu` | Enabled. Returns to Main Menu and restores focus to `Enter Lab`. |

The data bar remains visible on Research preview. The Lab does not show a
species-mastery balance in the global bar; that balance is contextual to a
future Species Archive surface.

### Research preview

Purpose: make one useful project and one blocked project understandable without
pretending that purchases are wired.

| Element | State and behavior |
| --- | --- |
| Research tabs | Plant, Herbivore, Carnivore. Herbivore is selected; the other tabs are visible and content-light. |
| Project list | Two fixture cards: one available/affordable and one locked/unaffordable. |
| Selected project panel | Shows type, cost, prerequisite, benefit, current balance, and explicit representative-data notice. |
| `Purchase` | Disabled and labeled `Prototype preview — no balance mutation`. |
| `Back to Lab` | Returns to Lab Overview and restores focus to `Open Research`. |

Selecting a project changes only the selected-project fixture state. It never
changes balances or unlocks a node.

## Representative data fixture

All values are deliberately static and must be marked `Representative data` in
the UI.

### Global balances

| Balance | Value | Display label |
| --- | ---: | --- |
| Research Data | 120 | Research |
| Plant Data | 36 | Plant |
| Herbivore Data | 48 | Herbivore |
| Carnivore Data | 12 | Carnivore |

### Projects

| Project | Type | Cost | Prerequisite | State | Benefit preview |
| --- | --- | --- | --- | --- | --- |
| Forage Route Mapping | Herbivore research | 10 Research + 20 Herbivore | None | Available and affordable | Reveals a food-search route preview for the Hare study. |
| Predator Avoidance Field Notes | Herbivore research | 160 Research + 60 Herbivore | Forage Route Mapping | Locked and unaffordable | Reveals a predator-pressure observation overlay. |

The locked project shows both reasons: the prerequisite is not completed and
the representative balances are insufficient. Cost, prerequisite, and benefit
are written as text and are never communicated through color alone.

## Low-fidelity layouts

These are structural wireframes, not visual-art direction. The same information
order is retained at both target resolutions.

### 1920×1080

```text
+--------------------------------------------------------------------------------+
| [Bio Os]  Forest Edge study                         [Profile] [Quit]            |
+--------------------------------------------------------------------------------+
|                                                                                |
|                         MAIN MENU                                              |
|                    Cellular automata as a roguelike                           |
|                                                                                |
|                         [ ENTER LAB ]                                          |
|                   [ Settings — Prototype ]                                    |
|                   [ Credits  — Prototype ]                                    |
|                         [ Quit ]                                               |
|                                                                                |
+--------------------------------------------------------------------------------+
```

```text
+--------------------------------------------------------------------------------+
| [Back] Lab Overview | Research 120 | Plant 36 | Herbivore 48 | Carnivore 12   |
+--------------------------------------------------------------------------------+
| Forest Edge study · Hare focus                 | NEXT RESEARCH                |
| Representative data                            | [ Open Research ]            |
|                                                | Archive [Prototype]          |
| Last experiment: none in this fixture          | Expedition [Prototype]       |
|                                                |                              |
|                                                | [ Back to Main Menu ]        |
+--------------------------------------------------------------------------------+
```

```text
+--------------------------------------------------------------------------------+
| [Back] Research | Research 120 | Plant 36 | Herbivore 48 | Carnivore 12        |
+--------------------------------------------------------------------------------+
| [Plant] [HERBIVORE] [Carnivore]                | SELECTED PROJECT             |
|                                                | Forage Route Mapping         |
| [Available] Forage Route Mapping               | Cost: 10 Research + 20       |
| [Locked] Predator Avoidance Field Notes       | Herbivore Data                |
|                                                | Prerequisite: none            |
|                                                | Benefit: route preview        |
|                                                | [Purchase — Prototype]        |
|                                                | Representative data           |
+--------------------------------------------------------------------------------+
```

### 1280×720

At 1280×720 the top bar stays single-line, the data bar uses compact labels
(`Research`, `Plant`, `Herbivore`, `Carnivore`) with no loss of text meaning,
and the Lab content becomes a stacked layout:

```text
+------------------------------------------------------------------------+
| [Back] Lab Overview   Research 120  Plant 36  Herbivore 48  Carnivore 12 |
+------------------------------------------------------------------------+
| Forest Edge study · Hare focus                                         |
| Representative data                                                    |
| Last experiment: none in this fixture                                  |
|                                                                        |
| [ Open Research ]                                                       |
| Archive [Prototype]   Expedition [Prototype]                           |
|                                                                        |
| [ Back to Main Menu ]                                                   |
+------------------------------------------------------------------------+
```

Research preview stacks the project list above the selected-project panel;
the `Purchase — Prototype` action remains in the initial keyboard tab order
but is disabled with its reason visible. No horizontal scroll, clipped cost,
or hidden Back action is acceptable.

## Navigation, Back, and focus contract

1. Launch focuses `Enter Lab`.
2. Main Menu → Lab Overview focuses the Overview heading, then `Open Research`
   is the first actionable control.
3. Lab Overview → Research preview focuses the selected project card.
4. `Back` always returns one level: Research → Lab Overview → Main Menu.
5. Overlays close before a page-level Back action is considered.
6. `Esc` follows the same rule as Back; on Main Menu it opens quit confirmation.
7. Focus is visible as a high-contrast outline plus a text/position change; a
   color change alone is insufficient.
8. Keyboard order follows reading order. Mouse activation and keyboard
   activation invoke the same command.
9. Disabled controls remain reachable for explanation, but cannot mutate state.
10. Selecting an available or locked project never changes the fixture balances.

## Acceptance evidence

- The Sprint 1 route is fixed to Main Menu → Lab Overview → Research preview.
- Every active Sprint 1 action has a destination and a Back path.
- Disabled, locked, prerequisite, and unaffordable states have explicit text.
- Currency type, cost, prerequisite, and benefit are readable without color
  alone.
- The 1920×1080 and 1280×720 structures preserve the same information order.
- Player Lab responsibilities are separated from Dev Lab authoring controls.
- The fixture is bounded and deterministic; it is not a persistence or wallet
  implementation.

## Deferred decisions

- Final typography, color palette, art motifs, and audio feedback.
- Actual research purchase behavior and data settlement.
- Full Species Archive, Expedition Setup, profiles, persistence, and saves.
- General navigation framework or reusable UI component library.
