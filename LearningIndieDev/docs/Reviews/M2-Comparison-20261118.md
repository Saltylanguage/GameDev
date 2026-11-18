# M2 independent review comparison — November 18, 2026

This records Josh's clarifications after comparing his November 16 Editor
review with [Sim's published independent response](https://github.com/Saltylanguage/GameDev/blob/3f9f726e16e5bfa366d8a79aff3111305d44f206/LearningIndieDev/docs/Reviews/M2-Sim-20261116/README.md).
Josh's original response remains in the local ignored artifact
`artifacts/m2-manual-review-20261116-josh.md`. Preserve both original sets;
the decisions below do not replace either reviewer's observations. Sim's
latest answers are the comparison input. His earlier startup-fix history is
not a reviewer disagreement.

## Shared observations

- Both reviewers found the journey map, opening Mutation availability,
  1920×1080 board readability, pointer inspection, speed switching, broad
  screen readability, provisional art direction and failure-result explanation
  understandable.
- Both heard no simulation audio, reached Hare extinction during phase five
  of the first authored journey, could not validate a permanent purchase and
  relaunch through that route, and would not replay it now.
- Both agreed the M2-04 48-run factual report accurately states the comparison
  and its limits. That accuracy agreement is not the separate human report
  accept/revise decision, a balance decision, or M2 closeout.

## Josh's post-comparison adjudications

| Question | Recorded direction | Effect on current work |
| --- | --- | --- |
| Q1 | Active-profile information outside entry can wait for Track B visual polish. | Carry into [VS-B1](https://trello.com/c/3QjBofCv); do not reopen the normal-menu/profile-entry implementation on this point. |
| Q5 | Retire the 1280×720 question. Josh is not supporting that resolution as an M2 target. | Preserve both historical Q5 answers, remove the check from active M2 acceptance, and keep Q6 at 1920×1080. A new lower supported bound has not been chosen. |
| Q14 | The reviewers interpreted the question differently but agree on the prescription. Use Sim's **Needs change** answer for the joint board-event readability finding. | Important events need clearer board-level cues. Josh's observation that grouped ledger text was understandable remains intact. Track B [VF-01](https://trello.com/c/RWRw4cjb), [VF-02](https://trello.com/c/AVJ0KXrJ) and [M2-07](https://trello.com/c/bqa1KUTK) hold the feedback work. |
| Q20 | Josh found Reduced Motion but saw no apparent effect and does not know when or why it was added. Sim could not find it. | Treat the effect and discoverability as unverified, rather than a human pass. Source inspection shows the toggle was added in `71c0c7b7` for M2 accessibility: it reads a saved preference, suppresses animated Fox-hunt and mating cues, and substitutes a static ring for the selected map pulse. Those limited effects may not be visible in an ordinary stretch; no human equivalence check has been established. |
| Q21 | Investigate Sim's inability to return from the Field Data test route to normal Journey. | Code inspection on `58e927aa` finds a visible test-route button in the Ready journey map even outside Developer Mode. Selecting it stores a Field Data definition; Back closes the map but does not clear that definition, and `ResetToStart` retains it. No in-place return command is exposed. This is a code-path finding, not a fresh runtime reproduction. Decide whether the test route should be hidden from normal players or have an explicit return path before implementing a fix. |

Q13 and Q16 used different labels in the two responses, but neither reviewer
validated audible 4× behavior or isolated benefit and cost for all three
opening strategies. Q2's selection and Q12–Q13's listener fixes exist on
`codex/m2-species-scenario-select` and still need human review. The authored
route, strategy comparison, permanent progression and external-player gate
remain open. Track A technical work may proceed in parallel; this comparison
does not approve balance tuning or close any M2 outcome.

## Follow-up

1. Carry Q1 and Q14 into the deferred Track B presentation and feedback work.
2. Retest Reduced Motion with an event that actually triggers its supported
   paths, and decide whether the current effect is meaningful and discoverable.
3. Resolve Q21's player versus developer route contract, then test the chosen
   navigation behavior from a normal fresh-profile entry.
4. Record Josh's separate M2-04 report and milestone decisions only when he
   explicitly makes them.
