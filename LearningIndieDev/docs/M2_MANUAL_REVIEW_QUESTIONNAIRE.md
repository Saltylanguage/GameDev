# M2 manual review questionnaire

- **Status:** Historical independent-review questionnaire. Q5 was retired from the active M2 gate on November 18, 2026; preserve the original answers.
- **Target gameplay baseline:** `codex/s6-predator-first` at `71c0c7b7`. A later documentation-only questionnaire commit does not change gameplay. Record the exact revision or build used.
- **Scope:** The ten [M2 outcomes](../ROADMAP.md#m2-outcome-contract--consolidated-2026-11-14), using the [current handoff](handoffs/2026-11-16-1055-codex-m2-05-art-and-m2-review-handoff.md).
- **Shared procedure:** Use the [independent review packet](M2_INDEPENDENT_REVIEW_PACKET.md) so both reviewers answer these same question IDs under comparable conditions.

## How we will use this

Ask **one numbered question at a time**, skipping retired Q5 in any new review. Answer **Yes** (you saw it and are happy with it), **Needs change** (you saw a problem), **Blocked** (you could not reach or judge it), or **Not tested**. Add one sentence about what you saw when the answer is not Yes. A seed, screenshot, or exact screen name helps reproduce a problem, but is optional while you play. Pause at any point and resume from the last question number. Keep Josh's and Sim's original answers separate.

These are observations, not automatic approvals of a whole M2 item. After the review, we will summarize each item as **Accept**, **Revise**, or **Inconclusive** from your answers and the existing test evidence. M2 stays open until all ten outcomes have their required evidence and a human closeout decision.

Use the normal player route and a fresh profile where a question asks for one. **1920×1080** remains the current M2 visual acceptance target; 1280×720 is outside the supported target. A real exit/relaunch means closing the game process, not merely returning to the menu or reloading a Unity scene. The authored Rabbit route has a known automated failure before node one ends. If your route stops early, record it and mark later unreachable questions **Blocked**; a controlled test fixture can check UI mechanics but cannot stand in for a successful predator route.

## Set up once

**Q0 — Test identity.** Which exact revision or build are you testing, in the Unity Editor or in a built game, and have you worked on this game before? This identifies the run and reviewer familiarity; it is not an acceptance question. Record resolution, input, and seed when they matter below.

## Start and choose

**Q1 — M2-01 / M2-10, normal entry.** Starting from the normal menu with a fresh profile, can you reach the journey without developer setup, and can you tell which profile is active?

**Q2 — M2-01, launch meaning.** Before you start, does the game make it clear that this slice launches **Forest Edge as a Rabbit**, what your immediate goal is, and what to do first?

**Q3 — M2-02, map direction.** On the journey map, can you find your current position, an available next node, and the action that starts it without help?

**Q4 — M2-04, opening responses.** With a fresh profile, are **Escape Instinct**, **Tough Hide**, and **Reproductive Drive** available at the opening choice, and can you say in plain words how each is meant to help?

## Watch and control the run

**Q5 — Retired November 18, 2026.** The 1280×720 check is no longer part of M2 acceptance. Retain Josh's and Sim's original Q5 answers as historical review evidence; do not renumber the remaining questions.

**Q6 — M2-02 / M2-03, 1920×1080.** At 1920×1080, can you distinguish the same three roles, read the objective, and find the next action?

**Q7 — M2-02, pointer inspection.** After panning and zooming, does clicking a board cell show information for the cell you actually selected?

**Q8 — M2-02 / M2-10, keyboard.** Without using the mouse, can you reach the important controls, see where keyboard focus is, and inspect a board cell?

**Q9 — M2-02, speed.** When you switch between 1× and 4×, does the simulation visibly change pace while the controls and objective remain usable?

**Q10 — M2-05, visual readability.** At 1920×1080, are the menu/Desktop, journey map, choices, live board, and any result you reach readable and visually coherent? Name any screen you could not reach.

**Q11 — M2-05, selected art.** Are you happy to keep the current Hero background, Rabbit art, and green/blue reward icon as the review direction? The [other delivered originals](Art/ChronoReview-20261116/README.md) are available for comparison; name a specific replacement or change if needed.

**Q12 — M2-06, sound at 1×.** At normal speed, can you tell important UI, Mutation choice, danger, and outcome cues apart, and do the ambience and music feel restrained enough to keep playing? Name any cue you could not reach.

**Q13 — M2-06 / M2-10, sound at 4×.** At 4×, do repeated sounds stay comfortable, and do mute and volume controls work as you expect?

**Q14 — M2-07, busy-run feedback.** During a crowded or fast stretch, do visual and audio cues match the important events without flooding you, and can you understand the summary of routine events that were grouped or suppressed?

## Reach an outcome and return

**Q15 — M2-03, legitimate route.** Using the authored Forest Edge/Rabbit route with Fox pressure and normal player choices, can you complete the representative journey? If it ends early, record the seed, last visible moment, and apparent cause; answer **Blocked** for completion.

**Q16 — M2-04, strategy differences.** After trying each opening response on comparable legitimate attempts, can you point to one observed benefit **and** one cost or limitation for each? If the route ends before the effects can be judged, answer **Blocked**.

**Q17 — M2-08, result explanation.** At a legitimate result, can you explain the leading observed reason for success or failure from the screen, and can you identify a **measured** Mutation effect rather than just a list of equipped Mutations?

**Q18 — M2-09, earn and buy.** Can you earn enough Field Data to buy one permanent unlock, and can you tell whether it is merely owned or currently equipped/active?

**Q19 — M2-01 / M2-09, real relaunch.** After a full game-process exit and relaunch, does the correct profile return with that unlock still owned, and can you use it on the next journey?

**Q20 — M2-10, reduced motion.** With reduced motion enabled, can you still perceive every essential danger, choice, and outcome that you could perceive with normal motion?

**Q21 — M2-10, independent replay.** Without coaching, can you understand how to start another attempt, read the key text and contrast, and complete the return-to-map or new-run flow?

**Q22 — S6 playability reflection.** Would you choose to try another attempt? What single moment most helped or hurt that decision? This is player feedback, not a stand-alone M2 pass.

## Separate evidence decision

**Q23 — M2-04 factual report only.** After reading the [48-run factual report](Research/Experiments/M2-04-Three-Response-20261115/REPORT.md), do you agree that it accurately records the approved comparison, including **0 of 12 Rabbits alive at node-one close in each of four arms** and its stated limits? Answer **Agree**, **Request a specific correction**, or **Still reviewing**. Sim's answer is independent review input; Josh, the decision owner, will make a separate explicit accept/revise decision after comparison. Agreement does **not** approve a balance value, a new experiment, M2-03/M2-04 closeout, or the M2 milestone.

## After Q23

Compare the two independent responses by question ID, then make a short item-by-item decision record: what each reviewer observed, where they disagree, which questions were blocked, and what implementation or validation remains. Any change to the Rabbit route needs a separate, concrete balance decision under [SG-005](Studio%20Guidelines/SG-005-UPGRADE-AND-ECOLOGY-BALANCE.md). Answers will not be filled in or inferred from automated screenshots.
