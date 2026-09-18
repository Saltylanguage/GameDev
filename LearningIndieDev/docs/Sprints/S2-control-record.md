# Sprint 2 Control Record — First Trustworthy Upgrade Loop

> Status: Closed | Start: 2026-09-03 | End: 2026-09-16 | Closed: 2026-09-17

> **Closed sprint archive.** Any proposed S3 acceptance or 10-phase/200-tick
> carry-forward text below reflects the plan at S2 close, not current scope.
> The active six-round player contract and S3 work are defined by
> [S3 control record](S3-control-record.md) and
> [S3-02 expedition contract](S3-02-expedition-contract.md).

This control record closes Sprint 2. The authoritative execution detail is
[`../NEXT_WORK_BUCKET_PLAN.md`](../NEXT_WORK_BUCKET_PLAN.md); this record fixes
the identity, capacity, ownership, board disposition, and exit gate.

## Control fields

| Field | Value |
| --- | --- |
| Sprint ID | S2 |
| Status | Closed |
| Start | 2026-09-03 |
| End | 2026-09-16 |
| Goal | A player can inspect, choose, and verify a temporary upgrade in a deterministic Forest Edge run. |
| Capacity | Josh 20h; Sim 20h; 40h committed. |
| Entry state | Sprint 1 is closed; its Windows build/review gate is accepted. |
| Exit criteria | A documented V1 catalog covers the supported numeric/additive and tradeoff effects; unsupported spatial and conditional effects are explicitly deferred; one temporary selection is applied reproducibly; the effective loadout/fingerprint and contribution evidence are reported; focused tests and a fixed-seed comparison pass; remaining balance questions are documented. |

## Board disposition

- The 2026-09-17 closeout verification found no cards in `Current Work`,
  `🛠️ In Progress`, or `⛔ Blocked`.
- Completed S2 work remains in `✅ Done`; the EX-010 schedule/approval card is
  synchronized as complete.
- Fox mating/eating telemetry is the sole carry-over. It is in `🎯 Upcoming
  Work` with `Sprint: S3 (carried from S2)`, owner Sim, reviewer Josh, and a 2h
  estimate.
- `Species roster and scenario co-design` remains outside S2 in the backlog.
- No Sprint 1 task was carried: cards 51–53, 62, and 72 remain complete.

### Register-to-board mapping

The repository register uses suffixes where Trello has separate cards with the
same S2 work-package prefix. Links below are the current board records.
The repository owner column remains authoritative for the local plan.
Historical board ownership differences on completed evidence cards are
preserved rather than silently reassigned during closeout.

| Repository ID | Work package | Board card | List at review |
| --- | --- | --- | --- |
| S2.1 | Upgrade contract and boundary | [772a1U6k](https://trello.com/c/772a1U6k) | ✅ Done |
| S2.2A | Catalog design and acceptance matrix | [7e5KhUen](https://trello.com/c/7e5KhUen) | ✅ Done |
| S2.2B | First catalog slice implementation | [KhHBo2tf](https://trello.com/c/KhHBo2tf) | ✅ Done |
| S2.3A | Deterministic application and contribution evidence | [JORLMXG3](https://trello.com/c/JORLMXG3) | ✅ Done |
| S2.3B | Upgrade loadout report/stat-line integration | [pZ4qG2DM](https://trello.com/c/pZ4qG2DM) | ✅ Done |
| S2.4 | Review and balance evidence | [SSwZkrI1](https://trello.com/c/SSwZkrI1) | ✅ Done |
| EX-002 | Intervention surface and causal-gate preparation | [PVqz2g5n](https://trello.com/c/PVqz2g5n) | ✅ Done |
| S2-QA | Catalog fixtures and invalid-combination tests | [OLZYtNS9](https://trello.com/c/OLZYtNS9) | ✅ Done |
| S2-UI | Player-facing upgrade preview and result summary | [iKcmYkuy](https://trello.com/c/iKcmYkuy) | ✅ Done |
| S2-CORR-FOX | Fox mating/eating telemetry discrepancy | [BkJwxhkw](https://trello.com/c/BkJwxhkw) | 🎯 Upcoming Work — sole S3 carry-over |
| S2-CORR-BOARD | BoardSnapshot fixture repair | [Cy2TOMOh](https://trello.com/c/Cy2TOMOh) | ✅ Done |
| CF-1 | Continuous domain lifecycle parity foundation | [cPlWlTfr](https://trello.com/c/cPlWlTfr) | ✅ Done |
| EX-010-PREP | Continuation contract and prediction-input preparation | [qftxVtnX](https://trello.com/c/qftxVtnX) | ✅ Done |
| EX-010-SCHEDULE | Contract-specific schedule parity and human approval | [LyMlLztN](https://trello.com/c/LyMlLztN) | ✅ Done |

## Closeout verification

- **Operation:** `S2-CLOSEOUT-20260917-01`
- **Caller:** Josh
- **Confirmed:** 2026-09-17
- **Recorded:** 2026-09-17T14:34:53-04:00
- **Board snapshot:** 34 cards in `✅ Done`; zero in `Current Work`, `🛠️ In
  Progress`, and `⛔ Blocked`; one card in `🎯 Upcoming Work`.
- **Carry-over:** `S2-CORR-FOX` only, assigned to Sim for 2h in S3.
- **Decision at S2 closeout:** S2 is closed. S3 remained proposed pending its
  separate confirmation; the later kickoff is recorded below.
- **Exit assessment:** The S2 goal and bounded exit criteria are accepted.
  Remaining M1 integration, player-flow, performance, and Fox follow-up work is
  explicitly scheduled for S3 rather than treated as S2 completion debt.
- **Post-closeout documentation sync:** On 2026-09-17, the S2 plan, Roadmap,
  Project Context, GDD, TDD, Working State, and Loose Ends ledger were
  reconciled to this closure. The kickoff workflow and project skill now
  require the same sync at future sprint closeouts. This is the closeout-time
  snapshot; the subsequent, separately confirmed S3 kickoff is recorded below.

### Subsequent S3 kickoff board read

S3 kickoff `S3-KICKOFF-20260917-01` was later verified on 2026-09-17. At that
read, seven S3 cards were in `Current Work`; Upcoming Work, In Progress, and
Blocked were empty; Done contained zero cards; and 35 historical cards were in
Archived. No unfinished S2 work was present in the active workflow lists. The
Archived/Done difference from the S2 closeout snapshot above was recorded and
left unchanged; the archived history was not reopened or moved as part of the
S3 kickoff.

## Scope boundary

The 2026-09-04 [consecutive-phase review](../CONTINUOUS_SIMULATION_FLOW_PLAN.md)
identifies additional runtime and evidence work beyond the accepted launch-time
slice. It is proposed for explicit capacity planning, not automatically committed
to S2. Continuing a phase in memory is distinct from player file saves. The
[impact notice](../CONTINUOUS_SIMULATION_EVIDENCE_IMPACT.md) calls out required
Stat-Line/research retests. The S2.3B report/stat-line integration card is
Josh-owned and complete; final phase-aware Stat-Line meaning review remains in
Sim's active lane.

S2 is a temporary per-run upgrade slice. It does not implement file saves,
wallet mutation, permanent Lab research, Archive or Expedition flows, a
general modifier/plugin framework, broad roster expansion, or final art/audio
production.

## Review cadence

- **Mid-sprint:** verify contract/catalog decisions and one reproducible
  baseline-versus-upgrade run before expanding the catalog.
- **Final review:** run the fixed-seed comparison, record accepted behavior,
  tuning questions, and explicit carry-over. Font, layout, and visual-feedback
  review are a separate work track and are not part of S2.

## M1 delivery risk snapshot

M1 is not expected to close at the end of S2. The following items must be
reviewed explicitly rather than being treated as silently complete:

| Priority | Risk | S2 end-state needed |
| --- | --- | --- |
| P0 | CF-0 through CF-5, target-resolution graphics acceptance, and the Windows player smoke are complete, but full integrated M1 acceptance is still open. | Carry the remaining ten-phase duration/memory measurement and player-flow acceptance into the proposed S3 M1 closeout sprint. |
| P0 | Phase-aware Stat-Line meaning is approved for EX-010. | Keep fresh-window evidence separate from continued-world evidence; any new metric or broader claim needs a separate Sim/Josh review. |
| P1 | The current board lists more Sim work than the 20-hour S2 planning budget can hold. | Keep, split, or carry each Sim card during the S2 review; do not assume all can finish in this sprint. |
| P1 | The continuation checkpoint and EX-010 result are accepted only within the approved evidence envelope. | Keep the EX-010 decision linked during the S2 review; do not treat generic smoke runs or later schedules as the same evidence. |

The proposed extra sprint is recorded in
[`S3-control-record.md`](S3-control-record.md). The former species/build
co-design sprint moves to S4 in the product roadmap.
