# Sprint 1 Control Record — Main Menu and Lab Foundation

> Status: Closed | Start: 2026-08-17 | End: 2026-08-30 | Closed: 2026-09-03

This is the durable Sprint 1 control record produced by S0.C4. The
authoritative execution detail remains in
[`../SPRINT_1_PLAN.md`](../SPRINT_1_PLAN.md); this record fixes the sprint
identity, capacity, ownership, checkpoints, and board disposition.

## Control fields

| Field | Value |
| --- | --- |
| Sprint ID | S1 |
| Status | Active |
| Start | 2026-08-17 |
| End | 2026-08-30 |
| Goal | Launch → Main Menu → Lab Overview → Research preview using representative UI data. |
| Capacity | Josh 20h; Sim 20h; 40h two-week capacity. The committed S1 scope remains 20h including reserve; extra capacity is uncommitted. |
| Exit criteria | Windows development-build review route works with keyboard/mouse, visible focus, deterministic Back behavior, representative balances/projects, both target resolutions, and recorded smoke evidence. |

## Closeout — 2026-09-03

Sprint 1 closed with its bounded outcome accepted: Main Menu → Lab Overview →
Research preview, representative research data, and the first usable species
stat-line slice. The acceptance record is deliberately narrow and does not
promote later player-loop work into this sprint.

- Route smoke passed at 1280×720 and 1920×1080; graphics-capable PlayMode
  evidence passed 6/6.
- The Windows development build succeeded and the player stayed alive through
  the recorded 15-second startup smoke.
- The v1 stat line preserves raw counts, exposes audited rates/denominators,
  and has deterministic validation recorded in
  `docs/HERBIVORE_STATISTICS_VALIDATION.md`.
- Trello cards 51–53 and 72 carry their acceptance evidence. The planned
  integration-reserve card (54) has no live Trello record; no reserve work is
  claimed.

Deferred work remains explicitly outside S1: Player UI/Dev Lab separation,
persistence and economy, functional research purchases/mastery, Archive and
Expedition, branching upgrades, and final art/audio/polish.

## Committed task register

| Task | Owner | Reviewer | Effort | Dependency | Acceptance evidence |
| --- | --- | --- | ---: | --- | --- |
| S1.1 Main Menu and Lab shell (Trello 51) | Sim (6h) | Josh (4h) | 10h | C2 contract; C3 bounded MainMenu repair | Main Menu → Lab → Research route works at both target resolutions; existing scene GUIDs remain intact; no wallet/persistence/simulation logic. |
| S1.2 Herbivore research preview (Trello 52) | Sim (2h) | Josh (2h) | 4h | S1.1 | Four balances, one affordable project, one locked/unaffordable project, cost/prerequisite/benefit text, disabled prototype purchase, no mutation/persistence. |
| S1.3 Verification and review (Trello 53) | Josh (2h) | Sim (1h) | 3h | S1.1 and S1.2 | Back/focus/mouse/keyboard and both resolutions checked; Play Mode and Windows smoke attempted; tests and diff check recorded. |
| Integration reserve (Trello 54) | Josh (2h) | Sim (1h) | 3h | S1.1/S1.2 findings | Used only for bounded Unity/Noesis repair, defects, review changes, or coordination; hours and evidence logged; never stretch-feature capacity. |

**Committed total:** Josh 10h, Sim 10h, 20h including reserve. The two-week
planning capacity is Josh 20h and Sim 20h; the additional capacity is not
automatically committed to new scope.

## Review cadence

- **Mid-sprint checkpoint:** Thursday, 2026-08-27. Demonstrate Main Menu → Lab
  Overview, compare actual hours with remaining shell work, and apply the
  documented cut order if the route is unstable.
- **Final review:** Sunday, 2026-08-30. Run the full review route, record
  accepted work, cuts, defects, and unresolved follow-up.
- **Cut order:** secondary Main Menu actions; second research project; Lab
  content beyond the data bar and Research entry; Build Settings promotion
  while retaining a direct-scene smoke check. Do not cut Back, visible focus,
  currency clarity, target resolutions, or basic verification.

## Board disposition

- The four cards above are the complete Sprint 1 commitment and were promoted
  from `🎯 Upcoming Work` to `🛠️ In Progress` at the 2026-08-18 kickoff. Their
  owners, estimates, acceptance evidence, and Aug 30 due dates are unchanged.
- `Sprint 1 - Separate the player UI from the Dev Lab scene` (Trello 4) is
  retained but moved to `🗂️ Backlog & Ideas`: it is explicitly deferred by
  `SPRINT_1_PLAN.md` and lacks the metadata required for sprint commitment.
- The Sprint 0 C4 card is closed only after this record and the matching board
  control card are visible and the 20-hour total is verified.

## Current gate

- Graphics-capable PlayMode evidence passes 6/6, while the generic nographics
  failures are a known Noesis native-texture boundary. Current-head EditMode
  passes 140/140 after the shared upgrade catalog and experiment-runner
  plumbing change.
- Build Settings put `MainMenu.unity` first. The current-head Windows
  development build succeeded and the player stayed alive for a 15-second
  startup smoke; artifacts are under
  `artifacts/audit-windows-build-current-20260820-101211/`.
- The shared preflight now bounds the licensing handshake and preserves a probe
  log. Keep the exact build/launch evidence on Trello card 53; do not reopen
  the completed Main Menu repair because of the prior environment failure.
- The current schema-7 Forest Edge control completed over seeds `10100`–`10119`
  with player species `hare`, authored grid `32 x 32`, and 20-second runs.
  Report: `artifacts/cellular-experiment-20260820-123724/report.json`;
  analysis: `artifacts/cellular-experiment-20260820-123724/analysis.md`.
  All 20 runs reconciled food-action attempts as successes plus failures.
- The held-out control completed over seeds `10120`–`10124` under the same
  protocol with zero food-action reconciliation mismatches. Report:
  `artifacts/cellular-experiment-20260820-154509/report.json`; analysis:
  `artifacts/cellular-experiment-20260820-154509/analysis.md`. Its Hare,
  Fox, and Plant final-population ranges remained within the 20-seed control
  envelope; the next evidence gate is the matched upgrade/control arm.
