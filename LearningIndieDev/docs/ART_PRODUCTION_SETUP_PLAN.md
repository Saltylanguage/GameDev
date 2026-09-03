# Art Production Setup Plan — Trello Department Board

> Status: Proposed setup plan  
> Owner: Artist / Art Director  
> Contributors: Producer, gameplay developers  
> Scope: Forest Edge vertical slice and the art department workflow that supports it

## Purpose

Create one Trello board that serves as the art department's working home: art
direction reference, requests, briefs, visual development, production,
feedback, technical handoff, in-game review, approval, and history.

The board is designed for a small team with one artist/art director, two
developers, and one producer. The artist owns visual direction and consistency;
developers own implementation and technical integrity; the producer owns scope,
priority, and review cadence.

This plan records the board structure. It does not create the Trello board or
promote any proposed visual direction to a final aesthetic decision.

## Project context the board must support

- The current product direction is a cellular-automata roguelike.
- The first slice is Forest Edge: fern as support/resource, hare as the player
  species, and fox as opposition.
- The player develops the hare through Trailblazer, Warren, and Gardeners build
  directions.
- The board must make the simulation readable: species, terrain, food,
  selection, danger, upgrade effects, and causes of population change.
- The current visual baseline is compact geometric silhouettes, flat fills,
  high contrast, and role colours: plants green, herbivores/player blue, and
  carnivores/threats red.
- Animal exports currently exist in standardized 32, 64, and 128 variants.
- Terrain uses a presentation-only normalized 47-mask eight-neighbor blob
  smart-tiling system.
- Grass is authored; bare terrain currently uses a temporary desert-family
  mapping.
- A dedicated authored fern presentation is still required.
- The board is rendered through a custom Noesis control and player-facing UI is
  Noesis/XAML.
- The presentation target is 1920×1080 and remains functional at 1280×720.
- Existing menu and Lab contracts define information structure but leave final
  palette, typography, motifs, animation, and audio open.
- The retained island, shoreline, and jungle prototype art is not an automatic
  visual baseline for the cellular-automata slice.

## Board columns

The board uses six columns. The leftmost column is a shared home for permanent
direction, incoming requests, and deferred ideas. Divider cards and title
prefixes keep those groups easy to scan without adding extra workflow columns.

### 1. Art Direction HQ / Requests / Future

This leftmost column contains three card groups.

#### Art Direction HQ

Permanent reference cards live here and do not move through production.

Initial cards:

- Visual North Star.
- Gold Standard Gallery.
- Current Art Inventory and Asset Status.
- Palette and Role Colours.
- Pixel-Art and Animation Rules.
- Visual Hierarchy.
- Species Silhouette Language.
- Terrain Smart-Tiling Contract.
- UI and Lab Visual Language.
- Unity/Noesis Art Handoff Guide.
- Art Review and Approval Rules.

These cards are the living replacement for a large static art bible.

#### Art Requests

Every new visual request begins here. Anyone may submit a request, but a
request is not yet approved work.

The requester describes the player-facing problem or need. The requester does
not prescribe the visual solution unless that is itself an approved constraint.

#### Backburner and Future Art

Valid ideas outside current slice scope live here. This includes additional
species, biomes, seasons, marketing art, broad animation, controller-specific
work, and revisions to retained prototype art. Future cards are preserved
without being treated as commitments.

### 2. Ready for Art

Prioritized and approved art work. A card only enters this list when its purpose,
states, references, dependencies, technical constraints, owner, and acceptance
criteria are clear.

The artist controls work-in-progress limits.

### 3. In Progress

Active visual-development and production work: references, thumbnails,
direction exploration, pixel production, UI design, animation, VFX, correction,
and export preparation.

Work-in-progress images stay attached to the card.

### 4. Art Review / Needs Brief or Decision

This column handles both review and blocked decisions. Cards move here when a
specific approval is needed or when the work cannot proceed without a brief,
design, technical, or scope decision.

For review cards, state what is being reviewed: direction, silhouette, palette,
readability, animation, scope, final art, or full-screen composition.

For blocked cards, name the missing decision, responsible person, and next
action. Typical blockers include unresolved gameplay behaviour, missing
references, unclear visual states, technical uncertainty, or a possible scope
multiplier.

Review outcomes are recorded as Approved for Production, Approved for
Integration, Changes Requested, Direction Rejected, or Inconclusive.

### 5. Integration and In-Game Review

Finished or test-ready art is imported, atlas-wired, connected to Noesis or the
runtime renderer, and tested in the real composition at both target resolutions
and normal simulation speed.

Technical success does not equal visual acceptance. The asset returns to In
Progress when it works in Unity but fails in context.

### 6. Approved and Done

Cards enter here only after art approval and technical integration approval.
This list is also the production history and source of approved precedents.

## Labels

Labels identify discipline, not workflow status.

### Discipline labels

- Visual Development
- Species
- Terrain
- UI/UX
- Icons
- Animation
- VFX/Feedback
- Technical Art
- Marketing
- Documentation

### Attention labels

- Needs Art Direction
- Needs Gameplay Decision
- Needs Developer
- Needs Producer
- Readability Risk
- Scope Multiplier
- Temporary Asset
- External Contribution

Columns already communicate workflow status, so duplicate status labels should
be avoided.

## Custom fields

| Field | Use |
| --- | --- |
| Asset ID | Stable reference such as `ART-SPEC-001` |
| Milestone | Foundation, Vertical Slice, or Post-Slice |
| Target | Board, Lab, Research, Upgrade, Results, or Marketing |
| Asset Type | Species, terrain, UI, icon, animation, VFX, or concept |
| Art Owner | Artist producing or directing the work |
| Integration Owner | Developer implementing it |
| Reviewer | Person providing final acceptance |
| Priority | P0, P1, or P2 |
| Estimate | Small, medium, or large |
| Decision Status | Proposed, approved, rejected, or temporary |

Do not create a custom workflow-status field; the card's list already provides
that information.

## Standard card template

### Title

```text
[AREA] Deliverable — State or Variant
```

Examples: `[SPECIES] Hare — gameplay-scale identity`, `[TERRAIN] Bare Ground —
47-mask blob family`, and `[UI] Upgrade Card — selected state`.

### Description sections

```markdown
## Player-facing purpose

What must the player understand or feel?

## Art brief

What is being created? What visual states are required?

## Context

Where does it appear, at what scale, and at what simulation speed?

## Art-direction guidance

Relevant pillars, approved references, role colours, and Gold Standards.

## Required states

Every state, variant, orientation, or animation.

## Technical constraints

Dimensions, atlas, naming, pivot, transparency, Noesis/Unity requirements,
and target resolutions.

## Dependencies

Gameplay, design, technical, or other art dependencies.

## Deliverables

Source files, exports, animations, previews, and screenshots.

## Acceptance criteria

Observable requirements for art approval and integration.

## Open questions

Decisions still required before final approval.
```

### Checklists

Brief Ready:

- Player-facing purpose is clear.
- Required states are listed.
- References are attached.
- Technical constraints are confirmed.
- Dependencies are resolved.
- Acceptance criteria are written.
- Priority is confirmed.

Art Direction:

- References gathered.
- Initial concepts attached.
- Gameplay-scale preview created.
- Direction reviewed.
- Chosen direction recorded.
- Rejected alternatives labeled.

Production:

- Final artwork complete.
- Required variants complete.
- Animation complete where applicable.
- Native-scale review complete.
- Source file attached or linked.
- Exports prepared.

Technical Handoff:

- Naming verified.
- Dimensions verified.
- Pivot verified.
- Atlas destination recorded.
- Import settings recorded.
- `.meta` present.
- Integration owner notified.

In-Game Acceptance:

- Asset resolves correctly.
- Tested in the real composition.
- Tested at 1920×1080.
- Tested at 1280×720.
- Readable at normal simulation speed.
- Relevant overlays tested.
- Artist approved.
- Developer approved technical integration.
- Gold Standard Gallery updated when appropriate.

## Initial Art Direction HQ cards

1. Visual North Star — Cellular Automata Roguelike.
2. Gold Standard Gallery — Current Approved Work.
3. Current Art Inventory and Asset Status.
4. Role Colours and Palette.
5. Pixel Scale and Texture-Density Rules.
6. Species Silhouette Language.
7. Terrain Smart-Tiling Contract.
8. Board Visual Hierarchy.
9. UI and Lab Visual Language.
10. Art Review and Approval Process.
11. Unity/Noesis Art Handoff Guide.

## Initial production cards

### Foundation

- Forest Edge — gold-standard board target.
- Hare — resolve Rabbit/Hare identity.
- Fox — gameplay-scale gold standard.
- Fern — dedicated visual language.
- Grass — noise and readability review.
- Bare Ground — replace or approve the desert substitute.
- Animal atlas — runtime validation.
- Terrain atlas — all 16 masks visual validation.
- Board layering — terrain/resource/creature/feedback.
- Selection and focus — gameplay-scale treatment.

### Upgrade language

- Spatial-pattern visual grammar.
- Trailblazer — visual identity and feedback.
- Warren — visual identity and feedback.
- Gardeners — visual identity and feedback.
- Feeding and fern depletion.
- Seed drop and fern regrowth.
- Protection activation.
- Fox pursuit and predation.
- Starvation, crowding, and extinction differentiation.

### Player-facing UI

- Main Menu — final visual direction.
- Lab Overview — final visual direction.
- Research Project Card — state family.
- Persistent data bar — icon and colour language.
- Upgrade choice card.
- Phase summary.
- Results and accomplishment presentation.
- Typography hierarchy.
- Keyboard focus and selected states.

Existing non-slice animals remain inventory/backburner items rather than automatic
production work. Additional biomes, seasons, final marketing art, broad
environmental animation, and retained-prototype revisions remain deferred.

## Operating rules

1. One card represents one reviewable visual deliverable.
2. No unbriefed request enters Ready for Art.
3. The requester explains the problem; the artist determines the visual solution.
4. Direction is approved before expensive production work.
5. Review work in the real composition whenever possible.
6. Batch routine reviews into predictable sessions.
7. Required changes and optional suggestions are separated.
8. Temporary assets are labeled with a replacement condition or acceptance decision.
9. New mechanics include their board, UI, animation, feedback, summary, and
   results art cost.
10. The artist controls visual consistency; the producer controls scope;
    developers control implementation integrity.
11. Done means approved in-game, not merely exported.
12. Accepted precedents are promoted into Art Direction HQ.

## Setup definition of done

- The board exists with the six columns in this plan.
- The leftmost column contains clearly separated HQ, Requests, and Future card
  groups.
- Permanent Art Direction HQ cards are created.
- Labels, custom fields, and card templates are configured.
- Initial foundation, upgrade-language, and player-facing UI cards are added.
- Each active card has an owner, reviewer, priority, dependency status, and
  acceptance criteria.
- The artist can manage a request from intake through in-game approval without
  relying on undocumented side conversations.
- The producer and developers can see what they owe the art department and what
  is waiting on them.
