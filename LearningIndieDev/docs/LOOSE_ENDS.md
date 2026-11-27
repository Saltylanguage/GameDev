# Loose Ends

This ledger tracks unresolved gaps that are not already owned by a sprint,
roadmap, or hygiene ticket. Closed decisions remain in Git and dated handoffs.

## Current review — 2026-11-27

No P0 issue is verified. S5 is Closed and S6 is Active Track A; the sprint
control and Trello cards track remaining estimates, water topology, and
player-route work. Josh owns the active Track A cards and Sim may assist as
available. The complete Rabbit play
review was reforecast after the S6 technical checkpoint, not marked accepted.
The current [transition handoff](handoffs/2026-11-27-1436-codex-s5-closeout-s6-track-a-kickoff.md)
records the verified board and Git baseline. The two local-state gaps below
need owner disposition; they are not sprint completion claims.

## Open items

- **P1 — Uncommitted Forest Edge route inputs.** The main worktree has a local
  `ForestEdge.asset` edit changing the authored map from 34×19 to 40×25 and
  adding a seeded irregular layout with a hole and island. **Status:** new,
  unassigned. **Next:** Josh or the scenario editor identifies whether this is
  an experiment or the intended route, records the input identity, and
  validates it before comparing to prior route evidence. **Confidence:** high
  in the diff, low in its intent. Preserve the edit until its owner decides.
- **P2 — Detached Field Data retry worktree.** The old
  `D:/GameDev/CrossAgent/unity-retry-field-data-4` worktree has tracked test
  edits and Unity settings line-ending changes. The metrics correction appears
  on current `ProjectMain`, but the remaining test differences are unreviewed.
  **Status:** newly surfaced. **Next:** the original Field Data implementer or
  Sim compares its tests with main and decides whether to retain or retire the
  worktree. **Confidence:** high in the worktree diff, uncertain about unique
  test value.

## Historical review — 2026-10-16

No P0 issue is verified. S4 closed on 2026-10-15, with P1-032 returned to
Backlog for Sim's launch-input review and a real app save/exit/relaunch check;
S4-04/05 remain Backlog. These are owned by their Trello cards and the
[S4 control record](Sprints/S4-control-record.md). The
[Active S5 control](Sprints/S5-control-record.md) now owns the 16 Field Data
tickets for October 19-November 15, including explicit unassigned-role and
capacity exceptions. Josh's [S5 playability goalpost](Sprints/S5-playability-goalpost-2026-10-16.md)
sets the first two-simulation review gate; all 16 cards remain visible for
tonight's Sim priority meeting. The protected review reserve was removed;
the 57–95h Field Data range is 15h above the conservative 80h floor at its
upper end, before unestimated ecology work. Final priorities and owners are
tracked by the S5 control. Broader candidates remain in the historical
[proposal](Sprints/S5-proposal-2026-10-15.md). Forest Edge balance
iteration (P1-034) is covered by the
[future sprint roadmap](FUTURE_SPRINT_ROADMAP.md); deferred compatibility and
naming work (P2-007 and P2-014) retain their gates in the
[project hygiene tickets](PROJECT_HYGIENE_TICKET_SUMMARIES.md). The recent
retention and pruning decisions are recorded in the
[2026-10-01 handoff](handoffs/2026-10-01-0112-codex-loose-ends-retention-decisions.md).
P1-026, the remote worker lifecycle issue, was resolved on 2026-10-01; see its
[closeout handoff](handoffs/2026-10-01-0158-codex-p1-026-worker-lifecycle-closeout.md).

## Closed in this review — 2026-10-01

- **P1-026 — Remote worker lifecycle tooling:** resolved on the worker branch;
  see the [closeout handoff](handoffs/2026-10-01-0158-codex-p1-026-worker-lifecycle-closeout.md).
- **P1-029 — First production Genome node contract:** Josh accepted the
  Fertile Droppings node direction, Rabbit Genome progression rules, default
  stacking, player-copy standard, and food-web counterplay. This closes the
  design-contract gap only; implementation and balance evidence remain future
  work. See the [accepted contract](handoffs/2026-10-01-0154-codex-p1-029-first-hare-genome-contract.md).
