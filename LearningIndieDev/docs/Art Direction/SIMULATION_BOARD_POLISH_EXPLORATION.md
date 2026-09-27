# Simulation board polish exploration

> Status: Design proposals for review, 2026-09-27. No new terrain, character, event, or gameplay rule is approved by this note.
> Scope: Forest Edge simulation board, its Field Ledger, phase feedback, and associated sound. Use this review method later on other player screens.

## The experience to build

The board should let the player notice a small ecological story: **a Fox begins a hunt, a Hare narrowly escapes or is lost, a pair has offspring, a food patch recovers, and the next phase begins.** The happy moments should land because the quieter board leaves room for them. Each effect earns its place by answering *what happened, where, to whom, and why it matters*.

The [approved simulation-view concept](Concepts/GalapagOS_Simulation_View_High_Fidelity_v1.md) supplies the board-first composition and biome ambition. The [earlier event vocabulary](SIMULATION_EVENT_VISUALS_IDEATION.md) supplies the Fox/Hare tone and the requirement that outcome cues report real events. The latest retained [playable capture](../../artifacts/visual-evidence-20260921-232618/03-galapagos-simulation.png) shows a readable shell and species, but a comparatively flat grass/bare grid. It is evidence from 2026-09-21, not proof of the current live Editor state. Current code draws a Fox hunt badge and a heart/sparkle birth cue; the board view model exposes only one recent hunt cue and one recent birth cue at a time. Those are useful prototypes, not yet a complete event language.

## First questions for every proposed flourish

1. What *resolved* event or useful state change triggers it? For intent, what exactly is the creature intending?
2. What should a player infer without opening the Field Ledger? Can this cue be confused with another outcome?
3. How often will it occur on a quiet board, a crowded board, and at 4× speed?
4. Does it still read at the smallest normal zoom and at 1280×720?
5. What is its still, muted, and reduced-motion form?

If a cue has no clear answer, keep it as environmental texture or leave it out of the first pass.

## Environment: make Forest Edge a place

| Proposal | Player-facing value | Boundary to settle before production art |
| --- | --- | --- |
| Clustered trees and shrubs, with sparse clearings | A visible forest edge, cover, and natural routes; fewer uniformly repeated cells | If trees block movement or provide shelter, make that a real tile state with matching rules. A decorative tree must not appear to block a traversable cell. |
| A narrow, continuous stream with one or two legible crossings | A recognizable landmark that helps players follow movement and remember events | Water, bank, crossing, and passability must agree. River behavior is a gameplay/terrain design decision, not an art-only addition. |
| Rocks, fallen logs, and small flower or fern clusters | Material contrast and local visual landmarks | Keep silhouettes lower contrast than animals and important cues; decide whether each occupies a cell or is only background detail. |
| Patches of grass in visibly different growth stages | The food landscape can visibly recover or thin out | Show real food/plant state. Do not imply a playable resource from random decorative density. |
| A few subtle ambient motions, such as one leaf drifting at an edge | A living atmosphere during quiet beats | Use sparse, low-contrast loops away from active encounters; avoid animating every tile. |

Start with a **small authored Forest Edge composition**: one coherent tree border, one rock cluster, and a path or clearing through the existing grid. Then decide whether a stream is worth its terrain and movement contract. Follow the [tile authoring guide](../TILE_AUTHORING_GUIDE.md) for new interactive terrain. The concept image is a target for density and charm, not a texture to place over live cells.

## Events: anticipation, outcome, aftermath

| Moment | Suggested treatment | Trigger and restraint |
| --- | --- | --- |
| Fox stalks prey | Two to four staggered paw prints along its *actual traversed cells*, alternating slightly across the travel direction. Older prints lose opacity over several simulation ticks. | Only when an identified Fox is moving while hunting. The existing badge marks a hunting transition, not a path; a trail needs per-Fox movement history. Freeze ageing during pause. Never draw tracks ahead of the Fox. |
| Hare senses danger or survives a near miss | Ear tuck or single alert mark; then a short hop and two motion pixels for a real escape. | Show only from a known threat/escape event. The recovery beat is more rewarding when the danger was legible first. |
| Successful animal birth | Keep the current heart. At each actual newborn cell, add a brief soft **pollen poof** behind the existing sparkle and a small arrival bounce. | Birth event coordinates already exist. The poof and sparkle are one composed event, with one sound, not competing effects. A phase Mutation that adds an individual should use a different arrival cue. |
| Feeding and new plant growth | Two leaf pixels move into an eater after successful feeding; a two-frame sprout opens on new plant growth. | Throttle ordinary feeding. Growth should be a quiet sign of ecosystem recovery, distinct from animal birth. |
| Attack and consequence | Sharp contact for a hit, hollow contact for a miss; a brief gentle farewell for a Hare death, followed by one food mote into the Fox when feeding succeeds. | Do not celebrate a kill with the birth palette or a cheerful sound. The sequence should preserve both the Fox's competence and the Hare's loss. |
| Population recovery or collapse | One restrained pulse in the Field Ledger and a short field note when a threshold or meaningful reversal is crossed. | Compare real phase/run state; avoid pulsing the ledger on every count change. A selected species may reveal more detail. |

Two further candidates for later review: a short selected-creature movement pose (Hare hop, Fox tail settle) to soften tick-to-tick teleportation, and a subtle forage trail that makes a successful food search understandable. Both risk becoming constant motion if applied to every creature.

## Sound, portrait, and phase feedback

**Sound palette.** Start with a quiet field ambience bed and four event families: a soft birth chime/chuff, a dry paw or grass rustle for a nearby hunt, a leaf/seed pluck for a meaningful feed or sprout, and a restrained contact/escape sound. A completed phase gets its own short, recognizable phrase. Pitch or sample variation can keep repeated cues natural. Cap simultaneous sounds, give birth/escape/phase priority over routine feeding, and check 4× playback for fatigue. Sound should reinforce a visible event, while the event remains understandable muted.

**Field Ledger character moments.** Reserve a compact portrait and one or two lines of text for authored observations: a surprising litter, a close escape, a population comeback, or a phase boundary. A 16-bit-style Hare face could blink, tuck its ears, or smile in two to three frames. “Darwin” could be a narrator candidate, but the project has not established that character or voice; settle who speaks and whether lines report facts, offer advice, or add personality before writing dialogue. Never let a portrait displace phase, population, or next-action information during live observation.

**Phase progress.** Fill the path across the existing six-phase timeline, including continuous travel between the current phase's node and the next. The thumb can be a small Hare that changes pose or hops briefly at meaningful milestones, rather than hopping every update. Progress follows authoritative tick/phase state, freezes on pause and at the Mutation decision, and resumes into the same world. The fill can ease between ticks visually without claiming more simulation progress than has occurred. Phase completion should feel like a small reward beat, with a clear handoff to the decision panel.

## Attention budget and implementation boundary

- Preserve a hierarchy: terrain is quiet; ordinary actions are small; decisive outcomes get the strongest local cue; phase transitions may briefly use the UI. Avoid whole-board flashes, camera shakes, or automatic camera moves.
- If many events occur together, favor events near the selected creature, within the visible board, or that affect the player's species. Summarize excess in the Field Ledger rather than stack effects. Record which events were suppressed so testing can detect lost meaning.
- Keep simulation logic UI-agnostic. Add the smallest positional output records needed after a completed tick, with tick, entity identity, event kind, coordinates, and outcome. Presentation owns animation, selection, and sound. Existing birth records are a good starting point; a multi-Fox paw trail and attack/feeding cues need more precise source events.
- Keep visual randomness separate from simulation randomness so effects never change reproducible ecology. Clear stale cues on restart; preserve continuity across phase breaks. Tick-based marks stop ageing on pause, while short one-shot effects should also freeze on pause.
- At 4× speed, fewer cues may be shown, but an event must never be represented as a different outcome. Offer reduced motion and effect intensity without removing essential state labels. Avoid repetitive high-contrast flashes; see [Xbox's photosensitivity guidance](https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/118).

## Suggested order of work

1. **Finish one happy event:** birth pollen poof, the existing heart/sparkle, one sound, and an optional Field Ledger sentence. This is the smallest complete board + sound + UI example and uses an existing positional event.
2. **Make one tense encounter readable:** Fox paw trail, Hare alert, and a clearly different escape versus kill resolution. This exposes missing event data and tests the attention budget.
3. **Give the board a home:** one small Forest Edge terrain composition using truthful tree/rock states; scope the stream separately after deciding passability and crossing behavior.
4. **Add phase movement and portrait moments:** animate authoritative progress, then use a portrait for one rare, authored observation. Keep the right panel readable during all six phases.

This is a learning order, not a sprint commitment. A rough event-data estimate and art/audio asset list should precede scheduling.

### Pilot checkpoint — 2026-09-27

The first visual slice is in progress on `codex/simulation-board-birth-pilot`. The board now draws a soft, two-tone pixel poof behind the newborn at the existing positional birth cue, while the heart and sparkle remain visible above it. Its timing freezes during the simulation's Paused state. This uses the current single-newborn cue path; showing a full litter still needs a multi-birth presentation decision. No active simulation sound assets or mixer path were found, so this checkpoint has no birth sound yet. The live view still needs visual review before the effect can be accepted as production art.

## Reusable review workflow for this and later screens

1. **Capture the real state:** record branch/commit, seed, phase/tick, speed, zoom, and screenshots or video at 1280×720 and 1920×1080. Compare it with the approved concept; name the largest gap.
2. **Write an event truth table:** trigger, player inference, board position, competing cues, sound, and reduced-motion form. Mark what simulation data exists and what must be added.
3. **Prototype one complete moment:** real event → board cue → sound → optional Field Ledger response. Keep asset and code scope bounded.
4. **Review contrast:** inspect quiet and crowded sequences at 1× and 4×, paused and muted, at minimum useful zoom. Ask a viewer unfamiliar with the code to say what happened, where, and whether the outcome was good or bad. Record misreads, not just preferences.
5. **Decide with evidence:** accept, revise, or drop the treatment; record capture links, asset provenance, event-data gaps, performance observations, and the next candidate. Promote only accepted visual rules into the reusable art language.

The team should preserve the successful pattern from the current heart/sparkle cue: a real event produces a compact, charming reaction at the correct cells. The main risks to test are false implied mechanics from scenery, cues lost during crowded/high-speed play, fatigue from repeated sounds or animation, and a Field Ledger that becomes a second competing show.
