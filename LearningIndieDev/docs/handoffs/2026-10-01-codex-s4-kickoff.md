# Sprint 4 kickoff — Build identity and evidence

**Date:** 2026-10-01
**Status:** Active
**Sprint dates:** 2026-10-01–2026-10-14
**Trello control card:** [Sprint 4 Control Record — Build Identity and Evidence](https://trello.com/c/Zn4UpBYc/118-sprint-4-control-record-build-identity-and-evidence)
**Project control record:** [S4 control record](../Sprints/S4-control-record.md)

## Goal

Make Trailblazer, Warren, and Gardeners distinct strategic choices in Forest
Edge, supported by matched-seed evidence and an in-game review. Connect the
canonical Desktop route to the active profile and current setup defaults.

## Decisions carried into S4

- P1-032 uses the active profile, Forest Edge, Hare, seed 10100, and the
  profile's frozen Genome snapshot. No new setup-choice screen is in scope.
- Missing or invalid profile context must fail closed and show a recovery path
  to profile selection; it must never start a hardcoded preview run.
- P1-032 is Josh's first task at 6h. Sim reviews the launch inputs.
- The accepted comparison input is Forest Edge with 400 Plants, 25 Hares, and
  15 Foxes at a 0.1-second step. The production asset remains 400 / 55 / 35 at
  0.2 seconds.
- S4-04/05 local profile-choice save/restore are deferred and remain in
  Backlog. S3-05 remains uncommitted stretch work in Backlog.

## Work and capacity

| Work | Josh | Sim | Total |
| --- | ---: | ---: | ---: |
| Feature estimates | 14h | 14h | 28h |
| Protected integration/review reserve | 5h | 3h | 8h |
| Planned | 19h | 17h | 36h |
| Unallocated | 1h | 3h | 4h |

The five selected Trello cards are in `Current Work`: P1-032, S4-01, S4-02,
S4-03, and S4-07. S4-04/05 remain in `Backlog & Ideas`. Names, owners, and
reviewers are written in each card description; Trello member assignments are
empty.

## Evidence checkpoint

The control/candidate pilot attempted on 2026-10-01 stopped before simulation
because Unity licensing/global-mutex and Package Manager IPC failed. It
produced no experiment result and is not balance evidence. Retry one matched
control and one candidate through setup, run, analysis, and in-game observation
before the full 13-arm × 20-seed screen. The estimates are accepted planning
assumptions; revisit them only if measured effort materially changes the
capacity plan. Keep the 8h reserve protected.

## Exit gate

- The three strategies create distinct, explainable decisions under matched
  conditions.
- Retain run inputs, fingerprints, outcome evidence, limits, and in-game
  observations; keep claims within the tested runs and seeds.
- Josh and Sim record an accept/revise decision after in-game review.
- Verify that Desktop launches from the active profile and current defaults,
  preserving deterministic inputs and safe failure recovery.

## Verification

After the Trello writes, the board was reread. It contains one S4 control card
in `🧭 Roadmap & Milestones`, five selected cards in `Current Work`, no cards
in `🎯 Upcoming Work`, `🛠️ In Progress`, or `⛔ Blocked`, and the deferred
S4-04/05 cards remain in Backlog. The existing Done cards and S3-05 backlog
item were left in place.

## Next step

Begin with P1-032 and S4-01. Run the small control/candidate effort checkpoint
before scaling S4-03 to the full screen. Record actual work and any capacity
change in the control record before using reserve or changing scope.
