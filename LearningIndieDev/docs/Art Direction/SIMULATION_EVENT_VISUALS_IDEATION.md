# Simulation event visuals — first pass

> Status: The Fox/Hare emotional direction below is user-approved. The Fox hunting badge is implemented as a proof of concept; other cues remain design proposals for art review.
> Scope: The Forest Edge board, especially Hare, Fox, and plant interactions.

## Goal and tone

Let a player glance at the board and tell **what happened, where, and to whom**. Effects should be tiny, crisp pixel-art annotations that preserve the board's readability.

**Fox hunting: deadly but cute.** The Fox should look focused and capable. Give the hunt a quick, precise rhythm: perk, approach, snap. A little tail flick can keep its personality without making a kill feel like a celebration.

**Hare being hunted: sad but cute.** Show vulnerability in its reaction: a small ear tuck or quiver when threatened. If it dies, use a brief, quiet farewell such as a drooping ear silhouette and one falling petal before it disappears. If it escapes, let it recover with a tiny hop. Reserve the farewell for an actual death.

The [simulation view concept](Concepts/GalapagOS_Simulation_View_High_Fidelity_v1.md) sets the board-first layout. The [art style guide](../../ART_STYLE_GUIDE.md) sets crisp pixel clusters, flat species silhouettes, and stable role colors. Shape and motion must carry meaning along with color.

## Event vocabulary

| Event and trigger | Board flourish | What the player should read |
| --- | --- | --- |
| **Hunting:** a Fox starts seeking visible prey | A tiny pointed gaze mark, then two tight paw-print marks near the Fox. The motion is deliberate and quick. | “The Fox is hunting.” This is intent, not a hit or kill. |
| **Hare threatened:** a Hare detects a Fox | A small alert tick and a quick ear tuck or quiver. If no alternate Hare pose exists, use the alert mark alone. | “The Hare knows it is in danger.” |
| **Hare escapes:** a threatened Hare successfully gets away | Two short motion strokes and one small recovery hop. | “It got away.” No sad farewell appears. |
| **Attack:** an actual attempt resolves | A compact, sharp contact star between predator and target. A miss or block gets a hollow broken star; a hit gets a solid one. | “They clashed,” with a visible difference between hit and miss. |
| **Hare killed:** a Hare dies from combat | After the sharp contact, its ears sink for a beat if an alternate pose exists; one small petal falls as the sprite fades into a soft puff. | “The Hare was lost.” The moment is sad and gentle without hiding the Fox's danger. |
| **Eating plants:** a feeding action succeeds | Two leaf or seed pixels hop from the food cell toward the eater, then a tiny contented bounce mark. | “Food was consumed here.” Failed feeding produces no happy cue. |
| **Eating prey:** a predator gains food after a kill | One warm energy mote travels into the predator after the prey farewell. | “The kill also fed the predator.” This joins the kill sequence instead of becoming a second full effect. |
| **Mating and birth:** a creature successfully produces offspring with a mate requirement | A small heart pops above the parent and nearby mate area; each newborn gets a brief pollen sparkle or tiny ring at its actual cell. | “They reproduced, and these are the new offspring.” If no mate is required or no pair can be identified, center the cue on the parent instead. |
| **Plant propagation:** a new plant appears | A seed pops into the destination cell and opens into a two-leaf sprout. | “New growth appeared,” distinct from animal birth. |
| **Other death:** starvation, crowding, or wilt | A quiet downward curl and fade at the affected cell, with a small cause mark when selected or inspected. | “This was a loss, not a predator kill.” |

Second-wave candidates: a creature entering sleep gets one small leaf-shaped `z`; successful seed dispersal gets a seed arc toward its destination. Trigger these from real resolved events or state changes, never every tick.

### One combined Fox encounter

For a lethal Fox/Hare encounter, play **focused Fox mark → sharp contact → quiet Hare farewell → one nourishment mote into Fox**. A miss gets only the broken contact star and, if the Hare escapes, its recovery hop. The Fox's action feels dangerous; the Hare's loss gets a moment of tenderness. No feeding or kill flourish appears for a failed attempt.

## Rules for a readable board

- Keep each flourish within roughly one cell plus a small margin. Anchor it to event coordinates without hiding the species for more than an instant.
- Let one-shot cues last roughly 0.4–0.7 seconds of wall time as an initial target; tune at 1× and 4× simulation speed. Freeze active cues while paused.
- Use silhouette, motion, and symbol together; do not rely on role colors alone. A reduced-motion setting can show the same symbol briefly without travel or bounce.
- Prefer nearby or selected-species events when many happen together. Throttle repeated feeding and hunting cues. A selected creature can expose more detail in the Field Ledger.
- Show a heart only for an actual successful birth, not simply because a creature enters `Mating` state. Do not present a searching Fox as though an attack has landed.

## First prototype and review

1. Sketch a tiny sprite sheet for Fox focus, Hare alert/farewell, heart, newborn sparkle, leaf hop, contact stars, puff, and nourishment mote at the board's smallest useful zoom.
2. Prototype **birth, plant eating, attack/miss, and kill-with-feeding** on a recorded Forest Edge sequence. Add hunting and Hare threat cues once the outcome cues read clearly.
3. Review quiet and crowded boards at 1× and 4×. Ask a new viewer to identify mating/birth, eating, hunting, failed attack, escape, and kill without the Field Ledger. Check whether the Fox reads as “deadly but cute” and the Hare reads as “sad but cute.”

## Implementation boundary to resolve later

The current board renders snapshots in one batched Noesis control. Existing death records have cell coordinates and tick, but births and feeding are largely counted in metrics, and combat-roll records do not include contact coordinates. Reliable one-shot visuals will need small, positional **output events after a completed simulation tick**. They should describe resolved facts and remain separate from simulation rules, as described in the [project context](../PROJECT_CONTEXT.md). Do not infer a birth or kill from two board snapshots when multiple interactions can occur in one tick.

This is F14 visual-language exploration and preparation for F15 simulation feedback in the [roadmap](../../ROADMAP.md); it does not expand the current Sprint 3 polish scope.

## Fox hunting proof of concept (2026-09-21)

The shared [board view model](../../Assets/UI/HUD/Scripts/VM_SimulationBoard.cs) now recognizes when the simulation's tracked Fox enters `Hunting`. The [Noesis board](../../Assets/UI/HUD/Scripts/SpeciesSimulationBoard.cs) pops a dark-brown and gold paw badge near that Fox for about half a second. Both the GalapagOS Desktop and prototype simulation hosts display it. The badge stays inside the board at its edges.

This uses the existing tracked-behavior record, so it marks **one tracked Fox**, not every hunting Fox. It is an intent cue only: it does not indicate an attack, kill, or feeding. The badge is provisional art and uses wall time even if the player pauses mid-cue. The local visual acceptance capture is under `artifacts/visual-evidence-20260921-225005/`, with before and after images at 1280×720.
