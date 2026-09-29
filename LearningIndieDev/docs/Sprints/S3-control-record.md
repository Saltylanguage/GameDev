# Sprint 3 Control Record — Safe Game Loop and M1 Closeout

> **Status:** Closed 2026-09-29 | **Dates:** 2026-09-17–2026-09-30 | **Cadence:** two weeks
> **Kicked off:** 2026-09-17 | **Roadmap baseline:** v2.2

> **Historical snapshot — 2026-09-21:** At that time, the kickoff and early
> execution snapshots below were current. The `ProjectMain` baseline was
> `ab56904b`; the latest retained clean run recorded EditMode 234/234 and
> no-graphics PlayMode 28/30 with 0 failures and 2 expected graphics-only skips.
> Phase-selection UI
> polish is committed, but its focused PlayMode validation remains pending;
> wrapper attempts were refused while the shared Editor returned to `playing`. An
> active Forest Edge balance pass is currently uncommitted and provisional.

> **Current-state update — 2026-09-29:** Josh merged `BevBranch` into
> `codex/forest-edge-visual-pass` at `51abd41d`. The approved cap fix and small
> integration repairs are preserved in pushed commit `974827f2`. Clean EditMode passed 263/263;
> Clean PlayMode passed 34 tests; two graphics-only tests were skipped as
> expected, and none failed. Invalid Reproductive Drive and capped coupled Fox
> Brood Drive choices are hidden. Josh accepts the current Forest Edge evidence
> for S3; later ecology work remains iterative. The bounded S3-06 pass is accepted,
> with later visual polish continuing iteratively. S3-08 telemetry is
> validated. Main Menu art direction is reviewed and Chrono is working on the
> art. Josh manually closed S3 on 2026-09-29; the committed S3 cards and control
> card are reconciled in Trello. M1 was separately accepted and closed on
> 2026-09-29 using the distributed evidence package; later changes may require
> retesting. S4 remains Proposed pending plan refinement and a separate kickoff.

> **Current-state update — 2026-09-27:** S3-04's three-choice/free/repeatable
> phase bridge is implemented. The first Tough Hide review used matched seeds
> 10100–10119 in a six-phase Forest Edge/Hare comparison; the directional
> player copy “Block more incoming attacks” and the existing generic A/B/C
> markers are accepted for this slice. Focused Unity EditMode and PlayMode
> checks passed. See the
> [Tough Hide handoff](../handoffs/2026-09-27-codex-s3-04-tough-hide.md).

Sprint 3 is about making the existing Forest Edge expedition safe to play from
start to finish, while making Mutations understandable and improving the look
and feel of the player interface. It does not repeat Sprint 2's Mutation
foundation or open a new research lane.

Sprint 2 is closed. This S3 plan was confirmed and kicked off on 2026-09-17.
Fox telemetry is the sole S2 carry-over. The current-sprint cards and stretch
work are listed below with their board locations.

## Control fields

| Field | Value |
| --- | --- |
| Sprint ID | S3 |
| Status | Closed — 2026-09-29 |
| Goal | Prove that a player can complete and understand one six-round Forest Edge expedition (10 seconds of simulation time per round), recover from bad states, and return to the Lab through the GalapagOS route. |
| Capacity | Josh 20h; Sim 20h; 40h planning capacity. |
| Entry state | S2 is closed; shared branches are consolidated at `cab5838d`; CF-0 through CF-5, EX-010, target-resolution graphics acceptance, and the Windows smoke are complete. The initial S3-01 validation shown here is historical; see the current-state note above for the latest retained clean result. |
| Primary outcome | The player can move through the Forest Edge expedition, its choices and results, and back to the Lab without stalls, errors, or becoming stranded. Mutations create visible, understandable effects on the simulation. |
| Carry-over | Fox mating/eating telemetry only — Sim, reviewed by Josh, 2h. |
| M2 relationship | This sprint prepares M2; species/build co-design moves to S4. |

## Already complete — do not schedule again

- The temporary Mutation contract, seven-item Hare catalog, deterministic
  application, previews, fingerprints, and contribution reporting.
- Same-world continuation through CF-0 to CF-5, including boundary checkpoints,
  phase/final Stat-Lines, and the headless schedule.
- EX-010 and EX-011 within their accepted evidence limits. S3 has no new
  predictive or balance experiment.
- Target-resolution graphics acceptance and the Windows development-player
  smoke.
- The Genome identity/profile/snapshot/catalog foundation. Production Genome
  content, effects, costs, actions, and persistence remain later work.

## Problems Sprint 3 must solve

1. The consolidated branch is not yet a trusted player baseline: the fresh
   full suites are red. Several PlayMode tests still assume the scene begins
   Ready/configurable, but the current host auto-starts the canonical route.
2. Game states do not yet have a proven, uninterrupted route through setup,
   simulation, boundary choices, results, recovery, and return to the Lab.
3. The earlier 10-phase/200-tick product language is superseded by the S3-02
   contract: six rounds, 10 seconds of simulation time per round, and five
   Mutation decision moments. Forest Edge's production default is 36×20 as of
   2026-09-26; playable plants remain deferred.
4. The agreed player-facing choices must be made clear in the interface:
   three glanceable Mutations or Skip after rounds 1–5, with pause and confirmed
   End but no Restart. The three-choice phase bridge is implemented and
   focused-tested; S3-04 copy/card-marker review is accepted for this slice.
5. The current player interface needs visual polish and integration with the
   simulation flow to move toward a shippable presentation.
6. Fox telemetry is the only S2 carry-over and needs to remain visible in the
   S3 plan.

## Committed capacity allocation

This allocation puts the game-state flow first, keeps Mutation
readability as a major M1 closeout task, and adds a small visual polish and UI
integration package. The 6-hour reserve remains protected. Performance
measurement is optional stretch work and is not included in the committed total.

| ID | Work and player outcome | Features | Owner / reviewer | Josh | Sim | Acceptance check |
| --- | --- | --- | --- | ---: | ---: | --- |
| S3-01 | Consolidated baseline and launch-contract acceptance | F13 | Josh / Sim | 2h | 2h | Direct-start Forest Edge/Hare is documented as canonical; recorded PlayMode failures are individually triaged and confirmed defects corrected; focused UI coverage and the full EditMode/PlayMode suites produce retained results with no unexpected failure or Noesis binding error. |
| S3-02 | Expedition rules and acceptance inputs | F01, F06 | Josh / Sim | 4h | 2h | Six 10-second simulation rounds; five three-option Mutation/Skip decisions; Pause, confirmed End/no rewards before round 6, no Restart; survival victory at round 6, immediate extinction failure/no rewards, and performance-based currency. Forest Edge defaults to 36×20; playable plants remain deferred. |
| S3-03 | Complete game-state flow and recovery | F02, F05, F13 | Josh / Sim | 8h | 0h | From the Lab, the player can start and finish an expedition, use its choices, reach a clear result, recover from reset or other bad states, and return to the Lab. No known route leaves the player stalled, in error, or unable to continue. Profile saving remains S4 work. |
| S3-04 | Mutation readability and bounded Forest Edge review | F03 | Josh product / Sim evidence | 2h | 6h | Each phase boundary offers three distinct applicable free Mutations from the existing five-Mutation Hare pool, or Skip. Invalid choices are hidden, including effects blocked by a capped coupled response. Repeats stack their defined effects. Repeatable Hare Reinforcements are a separate purchase. Evidence-backed qualitative copy and the phase summary remain player-readable. Current evidence is accepted for S3 closeout; no broader balance claim or catalog expansion is required. |
| S3-05 | Expedition duration and memory measurement — stretch | CF-6 stretch | Josh / Sim | 3h | 2h | Not included in committed capacity or the M1 closeout gate. If the plan is rebalanced or capacity added, measure the six-round, one-minute-simulation Forest Edge/Hare session. |
| S3-06 | Visual polish and UI integration | F13, F14 | Josh / Sim | 2h | 2h | Apply a bounded polish pass to the expedition screens and integrate them with the player flow; capture a reviewable result at target resolutions. |
| S3-07 | Integration, defect, and review reserve | Shared | Josh + Sim | 2h | 4h | Reserved for failures discovered while proving the S3 outcome; unused time does not become new feature scope. |
| S3-08 | Fox telemetry carry-over | Supporting evidence | Sim / Josh | 0h | 2h | Close the named mating/eating telemetry discrepancy with a regression assertion and a recorded decision; do not expand into broad balance tuning. |
| **Committed total** |  |  |  | **20h** | **18h** | **2h of Sim capacity remains uncommitted.** |

The 4-hour S3-06 split is 2h Josh and 2h Sim. Josh owns the eight-hour S3-03
flow and recovery work. S3-05 is outside these committed totals and needs a
scope trade or added capacity before it can start.

S3-04's working execution plan is in
[S3-04-mutation-readability-plan.md](S3-04-mutation-readability-plan.md). It
preserves the allocated Josh 2h / Sim 6h split and limits the review to existing
candidates and one bounded Forest Edge/Hare evidence slice. The current V1
bridge uses three eligible choices from the five-effect pool, hides effects
that cannot apply, supports repeat offers with level/stack progression, and
keeps Reinforcements separate from the Mutation choice.

The first Tough Hide candidate is implemented and measured in a bounded
20-seed matched panel. Its handoff records exact phase-local combat counts,
population observations, and report provenance. The current evidence is
accepted for S3 closeout; it does not mark the other four descriptions or the
Forest Edge ecology as broadly balance-tested.

## Delivery order

1. **Keep evidence honest:** Josh closed S3-01 on 2026-09-18 after integration.
   The latest merged-baseline Clean run passed EditMode 263/263 and PlayMode
   34 tests, with two expected graphics-only skips and no failures. The focused
   phase-decision regression passed 1/1 on 2026-09-27. Josh completed
   the P1-031 Main Menu art-direction review on 2026-09-29; Chrono is working
   on the selected art, which remains iterative follow-up work.
2. **Lock the product contract:** S3-02 is complete. Its explicit deferrals do
   not block implementation; the final Trello acceptance wording was reconciled
   during closeout.
3. **Make the flow safe:** S3-03 is implemented and validated against the
   approved contract; the Lab route and recovery paths pass PlayMode coverage.
4. **Close the M1 loop:** make Mutation effects understandable in S3-04 and
   apply the bounded interface polish in S3-06. S3-08 can proceed in parallel.
5. **Use stretch time only if available:** take on S3-05 after committed S3 work
   is on track. Protect S3-07 for integration defects and review findings.

## Kickoff verification

- Operation: `S3-KICKOFF-20260917-01`.
- Caller and confirmation: Josh explicitly confirmed the plan and kickoff on
  2026-09-17.
- Verification time: 2026-09-17 16:13 America/Toronto.
- This kickoff board snapshot is historical. Later execution notes record
  S3-01, S3-02, and S3-03 as complete; do not use the old `no cards in Done`
  sentence below as the current board state.
- S3-05 is in `🗂️ Backlog` as stretch work and is not committed.
- Trello records owners and estimates in each card description. The card-write
  connection did not set Trello member assignments.
- Board card list: S3-01 [MVDAAQqH](https://trello.com/c/MVDAAQqH), S3-02
  [c3i7HO09](https://trello.com/c/c3i7HO09), S3-03
  [QGZR0hdj](https://trello.com/c/QGZR0hdj), S3-04
  [O1j4WKHw](https://trello.com/c/O1j4WKHw), S3-06
  [UfyzMAGt](https://trello.com/c/UfyzMAGt), S3-07
  [TCzSvO7Z](https://trello.com/c/TCzSvO7Z), and S3-08
  [BkJwxhkw](https://trello.com/c/BkJwxhkw). The S3-05 stretch card is
  [M1Icx6FY](https://trello.com/c/M1Icx6FY). The S3 control card is
  [zfzJkUnj](https://trello.com/c/zfzJkUnj).
- The kickoff snapshot recorded no cards in `✅ Done` and 35 cards in its
  `Archived` list. That is historical board state; later execution notes below
  record S3-01, S3-02, and S3-03 as complete.

## S3-01 execution status

- **Current sprint snapshot (2026-09-21):** Josh closed S3-01 after
  integration. The latest retained clean run after the Island Survivor
  retirement recorded EditMode 234/234 and no-graphics PlayMode 28/30 (0
  failures, 2 expected graphics-only skips). Bare-cell resolver and snapshot
  regressions passed. See the current evidence
  in [WORKING_STATE](../WORKING_STATE.md) and P1-031. The Trello card is in `✅ Done`, marked
  complete, and retains this caveat in its description. S3-02 is complete as a
  product contract; see [S3-02](S3-02-expedition-contract.md). Josh confirms
  S3-03 is complete; its Trello card is in `✅ Done` and marked complete. The
  pushed baseline now includes the 64px
  Grass art/atlas, Bare-cell neighbor-mask resolution, regression coverage, and
  the simulation shell/UI integration (commits `fe56660d`, `a5a47e0f`,
  `a1b355e2`, and `52ce0430`). S3-01 closure remains the owner's scope
  decision; post-fix verification is now recorded above. S3-02 is complete on
  the Trello board; any
  remaining board-size/Fern acceptance wording is a non-blocking cleanup noted
  in P1-033.
- S3-03 aligns the runtime to six rounds, adds a confirming End dialog that
  pauses safely and resumes on cancel only when it was previously running,
  ends extinct player species immediately, and clears unspent data when an
  expedition is abandoned or lost. A new expedition resets run-scoped
  Mutations and carries awarded field data forward. Both simulation hosts
  start one new run on Results re-entry. The prototype scene now points to the
  simulation `UserControl` instead of its resource dictionary, and End-command
  availability refreshes as run state changes. The focused End/cancel test
  passed 1/1; the full no-graphics PlayMode suite passed 31/33 with 0 failures
  and two expected graphics-only skips. Unity imported and compiled the UI
  host in an isolated copy while the working project editors were open. Reports
  are retained under `artifacts/s3-03-test-results-20260918/`. The separate
  At that 2026-09-18 handoff, the three-offer gap remained open in S3-04; the
  later 2026-09-27 handoff records its implementation and focused validation.
  This closes S3-03 flow/recovery, not the Sprint 3 acceptance gate.

- Started 2026-09-17. After Unity was closed, fresh full-suite runs completed
  and retained XML/logs:
- `artifacts/unity-tests-20260917-174307/EditMode-results.xml`: 247 passed,
  2 failed, 0 skipped (249 total).
- `artifacts/unity-tests-20260917-174422/PlayMode-results.xml`: 28 passed,
  0 failed, 1 skipped (29 total).
- The profile-session persistence and Genome asset-change tests now pass.
  Both remaining EditMode failures were terrain contract checks against the
  absent Blob/64 tile root and atlas. The terrain integration now has focused
  EditMode coverage passing 3/3 and prototype-scene PlayMode coverage passing
  1/1; this resolves the recorded terrain-check failures locally.
- PlayMode passes the serialized Forest Edge/Hare direct-start route, Lab
  launch/return case, and rejected-settings preservation check. The
  graphics-only sprite test is skipped by the nographics runner and must be
  run in a graphics-capable player. No Noesis binding errors were found.
- The settings rejection path now leaves the active run intact: global values
  are validated before commit, and experimental setup no longer rebuilds a
  pending run before all setup fields are applied.
- The latest complete retained bundle is `artifacts/unity-tests-20260917-222442/`:
  EditMode 251/251 passed; no-graphics PlayMode 28 passed with two expected
  graphics-only skips. After the final ForestEdge population fix, the focused
  graphics-capable scene/visual test passed 1/1 and captured setup, running,
  rewards, and results under `artifacts/visual-evidence-20260917-222658/`.
  A separate attempt to run the full graphics-capable suite exited during
  Unity startup without producing test results, so that attempt is
  inconclusive. The earlier 30/30 graphics run predates the population fix.
  Josh closed S3-01 and moved its Trello card to Done on 2026-09-18; the card
  description records that post-fix validation remains outstanding.
- The terrain correction changes mask selection for Bare cells and adds
  resolver/snapshot regression tests so Grass patches fill empty dirt cells.
  The focused Bare-cell regressions passed in the later clean validation. The
  phase-decision UI, Settings, and My Collection PlayMode checks now pass. At
  this earlier snapshot, the remaining P1-031 item was human review of Main
  Menu branding/generated art; Josh completed it on 2026-09-29 (see the
  current-state update above). Do not reopen S3-01 without a specific defect.

## S3-02 completion status

- Complete: the six-round, 10-seconds-of-simulation-time-per-round cadence;
  five Mutation-or-Skip choices after rounds 1–5; Pause and confirmed End with
  no Restart; survival victory after round 6; extinction failure without
  rewards; and performance-based currency are recorded in the accepted
  contract.
- The Skip bonus, playable plant species, and exact performance-to-currency
  formula remain deferred; Forest Edge's production grid default is 36×20.
  These do not block S3-02.
- Josh marked the Trello card complete on 2026-09-18. During the 2026-09-29
  closeout, its description was confirmed to match the accepted contract and
  deferred decisions.

## Risk register

| Priority | Risk | Owner | Exit evidence |
| --- | --- | --- | --- |
| P2 | Main Menu art production and further UI polish continue after the bounded S3 pass. | Chrono + UI owner | Direction is reviewed; keep remaining art and polish work in its iterative follow-up list. |
| P0 | A player can become stranded by a broken state transition, error, or reset path. | Josh | Tests cover the main route and recovery paths back to the Lab; no known soft-lock remains. |
| P1 | The successful-run currency measure and conversion are still being developed; optional Skip and bonus-event rewards are unsettled. | Josh + Sim | Link the approved performance measure when ready; this does not block S3-02. |
| P1 | Flow and UI work could expand into profile saving, settlement, or production Genome actions. | Josh | S3-03 ends at a safe expedition and return route; profile saving remains in S4. |
| P1 | Concurrent terrain and Genome reconciliation changes could blur the baseline. | Work-block owners | Each work block reaches a reviewable checkpoint before shared integration evidence is claimed. |
| P2 | Fox follow-up could become open-ended balance research. | Sim + Josh | S3-08 closes the named telemetry discrepancy without broad balance tuning. |

## Closeout review — 2026-09-29

- **Code and evidence:** The merged `BevBranch` baseline plus the current diff
  passes the Clean Unity suites. EditMode passed 263/263; PlayMode passed 34,
  with two expected graphics-only skips and no failures. The local cap fix
  hides Reproductive Drive and the capped Fox Brood Drive offer, and the
  integration regressions are included in the run.
- **Product decisions:** Josh accepts the current Forest Edge evidence for S3.
  The bounded S3-06 visual pass is accepted, and further polish and art
  production remain iterative. The Main Menu direction review is complete;
  Chrono is working on the art. S3-08 telemetry is validated.
- **Remaining scope:** S3-05 duration/memory measurement is uncommitted stretch
  work, not a committed acceptance item. Later Forest Edge balance and Mutation
  changes are planned iterations, not blockers for this S3 V1.
- **Closeout processing:** Josh confirmed S3 closeout on 2026-09-29 after
  reviewing the current conversation and retained evidence. Trello now shows
  every committed S3 card and the control card in Done; S3-05 remains in Backlog
  as uncommitted stretch work. S3-01's validation caveat and S3-03/04's stale
  offer/integration notes are reconciled. S3-02 already matched the accepted
  contract. Owner and reviewer names remain in each card description; the
  Trello member-assignment fields are empty, as in the kickoff board convention.
  M1 is closed under its separate 2026-09-29 gate review; see the
  [M1 closeout handoff](../handoffs/2026-09-29-codex-m1-closeout.md).

## Sprint 3 closeout decision — 2026-09-29

Josh confirmed each exit criterion after reviewing the recorded evidence:

- [x] The consolidated EditMode and PlayMode baseline is trustworthy.
- [x] The player can move from the Lab through setup, simulation, choices, and
  results, recover from a bad state, and return to the Lab;
- [x] A six-round Forest Edge expedition advances for 10 seconds of simulation
  time per round, pauses after rounds 1–5 for three Mutation options or Skip,
  continues the same world, and reaches results after round 6;
- [x] Victory means the player species survives through round 6; extinction ends
  immediately as a failed run with no rewards; confirmed End before round 6
  abandons the run with no rewards; no Restart action exists;
- [x] Phase and expedition summaries explain the main change without raw developer
  fields, and the player can return to the Desktop;
- [x] The expedition interface has a reviewable visual polish and integration pass.
- [x] Fox telemetry has an explicit disposition.
- [x] No unresolved P0 item blocks S3 closeout.
- [x] Current documents, retained test evidence, handoffs, and board cards agree.

S3 was closed before the separate M1 gate review and did not itself close the
milestone. Josh subsequently accepted M1 on 2026-09-29; see the
[M1 closeout handoff](../handoffs/2026-09-29-codex-m1-closeout.md). S4 remains
Proposed and has not been kicked off.

## Out of scope

- Repeating S2 implementation or reopening EX-010/EX-011 without a new contract.
- Permanent Genome content, effects, costs, buying/activation actions, migration,
  or recovery.
- Wallet settlement, production persistence, and profile saving; profile saving
  remains planned for S4.
- Broad species or scenario expansion.
- Full reactive-ecology or rubber-banding implementation.
- Terrain-art delivery, final art/audio production, and broad visual redesign.
- New predictive calibration or generalized AI recommendation validation.
- Broad refactors or a generalized modifier, evolution, or plugin framework.

## Next sprint after S3

S4 carries forward the species/build co-design plan and adds local profile save
and restore, deferred from S3. Its outcome remains that Trailblazer, Warren,
and Gardeners produce distinct strategies in Forest Edge, supported by
matched-seed evidence and an in-game review.

Related evidence and plans: [ROADMAP.md](../../ROADMAP.md),
[WORKING_STATE.md](../WORKING_STATE.md),
[PRODUCT_BRIEF.md](../PRODUCT_BRIEF.md),
[GAME_FEATURE_ROADMAP_TRIAGE.md](../GAME_FEATURE_ROADMAP_TRIAGE.md),
[branch integration handoff](../handoffs/2026-09-13-0137-codex-branch-integration.md),
[Genome reconciliation handoff](../handoffs/2026-09-17-codex-genome-fixture-and-doc-reconciliation.md),
and [CONTINUOUS_SIMULATION_FLOW_PLAN.md](../CONTINUOUS_SIMULATION_FLOW_PLAN.md).
