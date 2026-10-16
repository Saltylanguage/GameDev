# Sprint 5 planning draft

> **Historical planning document, superseded at kickoff on 2026-10-16.**
> The [Active S5 control](S5-control-record.md) selects all 16 Field Data
> tickets for October 19-November 15, 2026. Dates, capacity and status below
> are retained proposal history, not the active sprint record.

Status: On hold at Bevin's request, 2026-10-15. No kickoff or Trello changes.

**Integration note, 2026-10-16:** Salty's
[S5 proposal](S5-proposal-2026-10-15.md) and the
[combined planning draft](../FIELD_DATA_AND_SIMULATION_ALIGNMENT.md) are the
next review inputs. S4 subsequently closed and P1-032 returned to Backlog.
The board snapshot and candidates below remain historical, not current status.

Bevin stopped this planning exercise and intends to leave sprint planning to
Salty. His immediate focus is the currency/journey loop and how the simulation
plays out; that work can be folded into Salty's planning. This records Bevin's
direction, not confirmation from Salty or a new sprint commitment. The dates,
capacity discussion, suggested reserve and candidate list below are retained
as historical planning material, not a schedule for Bevin's current work.

This is an initial discussion draft for Bevin, Salty and Chrono. The proposed
scope below is an assistant recommendation, not an accepted team commitment.
[Working State](../WORKING_STATE.md) | [Sprint workflow](../SPRINT_KICKOFF_WORKFLOW.md)

## Historical planning frame

- Dates: October 15–28, 2026. Bevin confirmed two weeks on October 15, consistent
  with the canonical workflow. The skill summary's one-week wording disagrees
  with that project workflow.
- Capacity: Bevin confirmed 10–15 hours per week in reply to the capacity question
  for both developers: Bevin/Sim 20–30h and Salty/Josh 20–30h over two weeks,
  40–60h combined. Chrono's art capacity is still TBD.
- Recommended commitment basis: use the lower 40h team availability and reserve
  20% (8h) for integration and review, leaving 32h for planned work. This reserve
  allocation is proposed. Any availability above 40h supports stretch work only
  after the committed outcome is secure; do not schedule against the full 60h.
- Selected work: none. Owners, reviewers and remaining-effort estimates for
  new work are TBD. No capacity-fit or readiness claim is possible yet.

## Verified starting point — October 15

The [development board](https://trello.com/b/Z14AeSnU) has Current Work 1,
Done 12, Upcoming Work 0, In Progress 0, Blocked 0 and Backlog & Ideas 25.
All list/card titles were inventoried; this is not a full audit of every
backlog description. No S5 control card is visible among the seven roadmap
cards. The [S4 control](https://trello.com/c/Zn4UpBYc) still explicitly says
Active, October 1–14. Closing four work cards did not close that control.

| Existing work | Observed status | Decision needed before kickoff |
| --- | --- | --- |
| [P1-032 — active-profile launch](https://trello.com/c/Enm8Avxu) | Only Current Work card. Owner Josh; reviewer Sim; original accepted estimate 6h. Implementation evidence exists in the October 14 integration, but the card remains incomplete. | Reconcile acceptance: close after the required review, carry the remaining work, or return it to Backlog. Obtain a remaining-effort estimate; do not reuse the original 6h as remaining work. |
| [S4-04 — saved-fields contract](https://trello.com/c/JiVmlHhh) | Backlog. Card records published profile foundation; manual exit/relaunch and Sim review remain open. Original plan Josh 2h + Sim 1h. | Review delivered scope and remaining acceptance. Do not schedule the already implemented foundation again. |
| [S4-05 — save/restore](https://trello.com/c/78M83hds) | Backlog. Card records focused tests and published persistence; manual restart/player-flow check and Sim review remain open. Original plan Josh 5h + Sim 2h. | Estimate and schedule only the remaining review/fixes if selected. |
| S4-01/02/03/07 | Complete in Done; refreshed closeouts and publication comments. | Leave in Done. Any continuing balance, crowding or strategy work needs separately bounded follow-ups. |

No active blocked card was visible. New candidates remain unassigned and
unestimated. Broad reference cards are not automatically ready for sprint work.
The [motion/feedback review](https://trello.com/c/xqXLWakH) explicitly has no
sprint assignment or estimate. The broad progression card appeared in the
board inventory but its detail page returned Board not found; its description
was not verified in this pass.

## Recommended outcome for discussion

**Make the journey's progress, failure and currency rules playable and clear.**

This builds on the October 14 meeting decisions. Start by checking which parts
already work, then choose a small coherent slice that fits confirmed capacity.
The alternative is a sprint centered on ecology/maps or strategy balance; the
team has not selected among these outcomes yet.

| Candidate | First useful result / proposed acceptance evidence | Refinement needed |
| --- | --- | --- |
| Journey failure and banked progress | Demonstrate that phase failure ends the journey without cash-out, loses only that journey's earnings, and retains previously banked Desktop progress. | Inspect current behavior, define when state is committed/discarded, and scope any required correction. |
| Field Data across phases | Demonstrate that Field Data survives a phase transition within the same journey and remains separate from permanent Desktop rewards. | Audit existing carryover, agree the visible presentation and handling on journey end. |
| Tunable simulation speed | Let the team vary speed and compare readable play at candidate defaults. The same tick-based simulation inputs must retain the same outcomes. | Separate simulation cadence from visual smoothness; changing a tick interval alone may not address the reported choppiness. Agree controls/range and assess current code before estimating. |
| One bounded spending improvement | Add one agreed useful purchase or small purchase set, with visible cost/effect and defined insufficient-funds behavior. | Pick from removing Hares, adding/removing Foxes/Plants or a random event. Agree prices/limits first. Random-event behavior is still undefined. |
| Small journey-art integration | Review/integrate a selected ready asset into the relevant journey screen, with a clear visual acceptance check. | Chrono's capacity, delivery selection, node container shape/size and integration effort need agreement. Use existing placeholders if art is not ready. |

These are candidate slices, not five committed tickets. All need an owner,
reviewer, remaining estimate, dependencies and stable task ID before selection.
The profile acceptance work above also consumes capacity if carried into S5.

## Art planning

The [live Art HQ board](https://trello.com/b/ykbpLTaf/ecos-art-hq) still shows
nine Ready for Art cards, zero In Progress, zero Unapproved, eight Approved and
Done, zero Technical Integration and sixteen Art Direction HQ cards. This pass
refreshed the list/title inventory; detailed specifications below come from
the reviewed October 14 meeting record, not a fresh review of every art brief.

- Journey node icons: Simulation, Final Challenge, Reward, Shop and Restore.
  The reviewed briefs request eleven 64px variants across these five cards.
- Species icons: Deer, Wolf and Owl. Artwork delivery does not imply new
  species simulation rules are implemented or included in S5.
- Journey panorama: one 5760×1080 Forest Edge background.

Suggested priority if the journey outcome is chosen: select node icons and a
bounded integration first; fit the panorama and species artwork to Chrono's
confirmed availability. No art owner, estimate or delivery date is assigned
by this draft.

## Other approved directions to refine separately

Wrapping and species-specific bushes, nonrectangular maps, lakes/rivers,
functional plants/mushrooms, richer income modifiers and additional purchases
remain valid meeting direction. They are not automatically S5 scope. Rock
behavior, permanent reward currency names/formulas and exact settlement rules
are still open. Initial playtesting pass criteria remain provisional under
Bevin's guidance; do not invent a fixed population or viability gate.

Continuing crowding/strategy/balance work also remains separate. The October 14
source review found the approved four-neighbor crowding surcharge absent;
this planning pass did not re-audit that implementation or run experiments.

## Decisions for the next planning step

- [x] Confirm two weeks and developer capacity: October 15–28; 10–15h/week each.
- [ ] Confirm Chrono's art capacity and agree the integration/review reserve.
- [ ] Select one main sprint outcome.
- [ ] Decide P1-032 acceptance/carryover and profile-review scope.
- [ ] Audit selected behavior against current code before estimating.
- [ ] Agree owners/reviewers, estimates, dependencies and observable checks.
- [ ] Select a bounded Chrono art delivery and integration target.
- [ ] Preview exact Trello updates and confirm them before applying changes.
- [ ] Reconcile whole-S4 closure and current roadmap wording with the meeting
  decisions. The October 1 roadmap still contains older fixed run-success
  wording; preserve it as history while updating current direction in the
  eventual approved documentation sync.

## Sources and limits

- [Reviewed October 14 meeting record](../Meeting%20Briefs/TEAM%20MEETING%20%E2%80%94%20OCTOBER%2014,%202026,%20AT%209%20PM.docx)
- [Meeting publication](../handoffs/2026-10-14-2337-codex-october-14-meeting-brief-review.md)
- [S4 control record](S4-control-record.md)
- [Profile foundation evidence](../handoffs/2026-10-14-1424-codex-profile-persistence-foundation.md)
- [Merged integration evidence](../handoffs/2026-10-14-1707-codex-projectmain-bevbranch-integration.md)

Read-only board inspection and documentation planning only. No Trello edits,
kickoff, source/asset changes, Unity tests, builds or balance experiments were
performed. Existing ten flourish metadata edits remain outside this work.
