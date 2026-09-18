# Sprint 3 Control Record — Safe Game Loop and M1 Closeout

> **Status:** Active | **Dates:** 2026-09-17–2026-09-30 | **Cadence:** two weeks
> **Kicked off:** 2026-09-17 | **Roadmap baseline:** v2.2

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
| Status | Active |
| Goal | Prove that a player can complete and understand one six-round Forest Edge expedition (10 seconds of simulation time per round), recover from bad states, and return to the Lab through the GalapagOS route. |
| Capacity | Josh 20h; Sim 20h; 40h planning capacity. |
| Entry state | S2 is closed; shared branches are consolidated at `cab5838d`; CF-0 through CF-5, EX-010, target-resolution graphics acceptance, and the Windows smoke are complete. The initial S3-01 baseline had two terrain failures; the latest retained rerun is green (EditMode 251/251, no-graphics PlayMode 28 passed with two expected skips, graphics-capable PlayMode 30/30). Trello card status still awaits Josh's review. |
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
   Mutation decision moments. Board size and playable plants are deferred.
4. The agreed player-facing choices must be made clear in the interface:
   three glanceable Mutations or Skip after rounds 1–5, with pause and confirmed
   End but no Restart.
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
| S3-02 | Expedition rules and acceptance inputs | F01, F06 | Josh / Sim | 4h | 2h | Six 10-second simulation rounds; five three-option Mutation/Skip decisions; Pause, confirmed End/no rewards before round 6, no Restart; survival victory at round 6, immediate extinction failure/no rewards, and performance-based currency. Board size and playable plants are explicitly deferred. |
| S3-03 | Complete game-state flow and recovery | F02, F05, F13 | Josh / Sim | 8h | 0h | From the Lab, the player can start and finish an expedition, use its choices, reach a clear result, recover from reset or other bad states, and return to the Lab. No known route leaves the player stalled, in error, or unable to continue. Profile saving remains S4 work. |
| S3-04 | Mutation readability and bounded Forest Edge review | F03 | Josh product / Sim evidence | 2h | 6h | The current Mutation offer and phase summary show the affected rule, timing, tradeoff, and observed consequence; one bounded review records an accept/revise decision. No catalog expansion is required. |
| S3-05 | Expedition duration and memory measurement — stretch | M1 technical gate | Josh / Sim | 3h | 2h | Not included in committed capacity. If the plan is rebalanced or capacity added, measure the six-round, one-minute-simulation Forest Edge/Hare session. |
| S3-06 | Visual polish and UI integration | F13, F14 | Josh / Sim | 2h | 2h | Apply a bounded polish pass to the expedition screens and integrate them with the player flow; capture a reviewable result at target resolutions. |
| S3-07 | Integration, defect, and review reserve | Shared | Josh + Sim | 2h | 4h | Reserved for failures discovered while proving the S3 outcome; unused time does not become new feature scope. |
| S3-08 | Fox telemetry carry-over | Supporting evidence | Sim / Josh | 0h | 2h | Close the named mating/eating telemetry discrepancy with a regression assertion and a recorded decision; do not expand into broad balance tuning. |
| **Committed total** |  |  |  | **20h** | **18h** | **2h of Sim capacity remains uncommitted.** |

The 4-hour S3-06 split is 2h Josh and 2h Sim. Josh owns the eight-hour S3-03
flow and recovery work. S3-05 is outside these committed totals and needs a
scope trade or added capacity before it can start.

## Delivery order

1. **Trust the baseline:** finish S3-01 before using the consolidated player
   route as acceptance evidence.
2. **Lock the product contract:** complete S3-02 before implementing behavior
   that depends on phase timing, board identity, rewards, or terminal meaning.
3. **Make the flow safe:** implement S3-03 against the approved contract, then
   test each route back to the Lab and each recovery path.
4. **Close the M1 loop:** make Mutation effects understandable in S3-04 and
   apply the bounded interface polish in S3-06. S3-08 can proceed in parallel.
5. **Use stretch time only if available:** take on S3-05 after committed S3 work
   is on track. Protect S3-07 for integration defects and review findings.

## Kickoff verification

- Operation: `S3-KICKOFF-20260917-01`.
- Caller and confirmation: Josh explicitly confirmed the plan and kickoff on
  2026-09-17.
- Verification time: 2026-09-17 16:13 America/Toronto.
- The seven committed work cards are in `Current Work`; `🎯 Upcoming Work`,
  `🛠️ In Progress`, and `⛔ Blocked` are empty.
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
- The board currently has no cards in `✅ Done` and 35 cards in its `Archived`
  list. This differs from the earlier S2 closeout snapshot; no unfinished S2
  cards were in the active workflow lists, and the archived history was left
  unchanged.

## S3-01 execution status

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
- The latest retained rerun is `artifacts/unity-tests-20260917-220612/`:
  EditMode 251/251 passed; no-graphics PlayMode 28 passed with two expected
  graphics-only skips; the additional graphics-capable PlayMode run passed
  30/30. No failures were reported. This resolves the test gate locally; the
  Trello card status remains unchanged pending Josh's review.
- Next: review/accept the retained results and reconcile the Trello card status.

## Risk register

| Priority | Risk | Owner | Exit evidence |
| --- | --- | --- | --- |
| P1 | The latest integrated test rerun is green locally, but the S3-01 Trello card status has not yet been reconciled after review. | Josh | Review retained artifacts and update the card/control status consistently. |
| P0 | A player can become stranded by a broken state transition, error, or reset path. | Josh | Tests cover the main route and recovery paths back to the Lab; no known soft-lock remains. |
| P1 | The successful-run currency measure and conversion are still being developed; optional Skip and bonus-event rewards are unsettled. | Josh + Sim | Link the approved performance measure when ready; this does not block S3-02. |
| P1 | Flow and UI work could expand into profile saving, settlement, or production Genome actions. | Josh | S3-03 ends at a safe expedition and return route; profile saving remains in S4. |
| P1 | Concurrent terrain and Genome reconciliation changes could blur the baseline. | Work-block owners | Each work block reaches a reviewable checkpoint before shared integration evidence is claimed. |
| P2 | Fox follow-up could become open-ended balance research. | Sim + Josh | S3-08 closes the named telemetry discrepancy without broad balance tuning. |

## Acceptance gate

Sprint 3 is ready to close when:

- the consolidated EditMode and PlayMode baseline is trustworthy;
- the player can move from the Lab through setup, simulation, choices, and
  results, recover from a bad state, and return to the Lab;
- a six-round Forest Edge expedition advances for 10 seconds of simulation
  time per round, pauses after rounds 1–5 for three Mutation options or Skip,
  continues the same world, and reaches results after round 6;
- victory means the player species survives through round 6; extinction ends
  immediately as a failed run with no rewards; confirmed End before round 6
  abandons the run with no rewards; no Restart action exists;
- phase and expedition summaries explain the main change without raw developer
  fields, and the player can return to the Desktop;
- the expedition interface has a reviewable visual polish and integration pass;
- Fox telemetry has an explicit disposition;
- the M1 gate matrix has no unresolved P0 item; and
- active documents, retained test evidence, handoffs, and board cards agree.

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
