# Sprint 1 Control Record — Main Menu and Lab Foundation

> Status: Active | Start: 2026-08-17 | End: 2026-08-23

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
| End | 2026-08-23 |
| Goal | Launch → Main Menu → Lab Overview → Research preview using representative UI data. |
| Capacity | Josh 10h; Sim 10h; 20h total including reserve. |
| Exit criteria | Windows development-build review route works with keyboard/mouse, visible focus, deterministic Back behavior, representative balances/projects, both target resolutions, and recorded smoke evidence. |

## Committed task register

| Task | Owner | Reviewer | Effort | Dependency | Acceptance evidence |
| --- | --- | --- | ---: | --- | --- |
| S1.1 Main Menu and Lab shell (Trello 51) | Sim (6h) | Josh (4h) | 10h | C2 contract; C3 bounded MainMenu repair | Main Menu → Lab → Research route works at both target resolutions; existing scene GUIDs remain intact; no wallet/persistence/simulation logic. |
| S1.2 Herbivore research preview (Trello 52) | Sim (2h) | Josh (2h) | 4h | S1.1 | Four balances, one affordable project, one locked/unaffordable project, cost/prerequisite/benefit text, disabled prototype purchase, no mutation/persistence. |
| S1.3 Verification and review (Trello 53) | Josh (2h) | Sim (1h) | 3h | S1.1 and S1.2 | Back/focus/mouse/keyboard and both resolutions checked; Play Mode and Windows smoke attempted; tests and diff check recorded. |
| Integration reserve (Trello 54) | Josh (2h) | Sim (1h) | 3h | S1.1/S1.2 findings | Used only for bounded Unity/Noesis repair, defects, review changes, or coordination; hours and evidence logged; never stretch-feature capacity. |

**Total:** Josh 10h, Sim 10h, 20h committed including reserve.

## Review cadence

- **Midweek checkpoint:** Thursday, 2026-08-20. Demonstrate Main Menu → Lab
  Overview, compare actual hours with remaining shell work, and apply the
  documented cut order if the route is unstable.
- **Final review:** Sunday, 2026-08-23. Run the full review route, record
  accepted work, cuts, defects, and unresolved follow-up.
- **Cut order:** secondary Main Menu actions; second research project; Lab
  content beyond the data bar and Research entry; Build Settings promotion
  while retaining a direct-scene smoke check. Do not cut Back, visible focus,
  currency clarity, target resolutions, or basic verification.

## Board disposition

- The four cards above are the complete Sprint 1 commitment and remain in
  `🎯 Upcoming Work` until kickoff promotes them to the active execution list.
- `Sprint 1 - Separate the player UI from the Dev Lab scene` (Trello 30) is
  retained but moved to `🗂️ Backlog & Ideas`: it is explicitly deferred by
  `SPRINT_1_PLAN.md` and lacks the metadata required for sprint commitment.
- The Sprint 0 C4 card is closed only after this record and the matching board
  control card are visible and the 20-hour total is verified.

## Known blockers

- Unity Editor/headless licensing still prevents the Play Mode and Windows
  smoke checks. This is tracked as an explicit integration/verification risk,
  not hidden inside the reserve.
- The Main Menu scene needs the bounded player-shell XAML/ViewModel repair
  identified by C3 before Build Settings promotion.
