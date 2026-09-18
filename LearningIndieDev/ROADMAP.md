# Cellular Automata Roguelike Roadmap v2.2

> **Status:** Active production roadmap | **Version:** 2.2 | **Updated:**
> 2026-09-17 | **Cadence:** Two weeks, approximately 20 hours per developer |
> **Product owner:** Josh

This is the product-level source of truth for what the team is trying to make,
what comes next, and what must be true before work moves forward. Feature briefs,
sprint control records, Trello cards, and technical plans provide execution
detail; they do not silently expand this roadmap.

## Product outcome

Deliver a run-based game built around **cellular automata as a roguelike**. The
player guides one species through six ten-second simulation rounds in one
continuing ecosystem. The player makes five temporary **Mutation** decisions
per run; permanent currency purchases in Gene Lab add **Genome Upgrades** to a
species' **Genome** skill tree. The long-term objective is a diverse, resilient
ecosystem rather than one dominant species.

The first vertical slice uses:

- **Scenario:** Forest Edge.
- **Player species:** Hare.
- **Opposing species:** Fox. Playable plant species are on hold and are not part
  of the current expedition contract.
- **Builds:** Trailblazer, Warren, and Gardeners.
- **Home base:** the GalapagOS Desktop. Lab features such as Gene Lab, Species
  Collection, History, and Expedition Planning live within that player-facing
  home. The standalone `Lab` scene remains a legacy/developer route until its
  retained tests and responsibilities are deliberately migrated.

## How to read this roadmap

Every item has one of three scheduling states:

- **Active:** inside the current sprint's approved capacity.
- **Proposed:** prepared for the next sprint but not committed until kickoff.
- **Horizon:** sequenced by dependency and given a planning range, but not yet a
  sprint commitment.

Roadmap effort is the expected team effort for the **first useful slice**, not
the lifetime cost of a feature:

| Band | First-slice planning range |
| --- | ---: |
| S | 1–4 hours |
| M | 5–10 hours |
| L | 11–20 hours |
| XL | More than 20 hours or deliberately split across sprints |

An effort band is not enough to start work. At sprint kickoff, selected work
must receive an owner, reviewer, hour estimate, dependencies, and acceptance
evidence. Keep 10–20% of sprint capacity for integration, defects, review, and
small evidence-producing corrections.

## Decisions that are already locked

- One expedition contains six ten-second simulation rounds in the same
  evolving world, for one minute of simulation time.
- Round 1 has no upgrade. After rounds 1–5, the player chooses one of three
  Mutations or Skip. Finishing round 6 goes to results without another choice.
- Mutations are temporary for the current run and exclusive to Species
  Simulations. Each is communicated with a small icon and keyword, not a full
  statistic breakdown; its expected impact should be clear in the following
  round.
- Pause is available. There is no Restart action. End abandons the run after a
  Yes confirmation and forfeits rewards if used before round 6.
- Victory means the player species survives through round 6. Extinction ends
  the run immediately as a failure with no rewards.
- Successful-run currency uses a performance measure now in development, not
  simply final population. A Skip bonus and additional bonus-event rewards are
  undecided and non-blocking; if a Skip bonus is adopted, it uses Genome
  Upgrade currency.
- Permanent **Genome Upgrades** are bought with currency in the Gene Lab
  application on the GalapagOS Desktop and fill a **Genome** skill tree.
- Forest Edge board size is deferred; playable plant species, including Fern,
  are on hold and are not contract requirements.
- Genome nodes remain unlocked permanently, while each species has a separate
  active Genome configuration that can change only between simulations.
- Every participating species receives its frozen active Genome at launch,
  including species that are not controlled by the player.
- The first Genome identity, local-profile, immutable snapshot, authoring-type,
  catalog, and Gene Lab display contracts are implemented. No production node
  effect, cost, purchase, activation action, migration, or approved map is
  implied by that foundation.
- The current GalapagOS Desktop acceptance route directly starts Forest
  Edge/Hare. The cause of the recorded PlayMode failures has not been verified
  from individual test results; preserving the current route does not waive the
  required profile and launch-request handoff.
- Species and Biome Simulations use separate success scorecards. A result from
  one mode is not sufficient approval evidence for the other.
- Active-run disk save/resume is separate from profile, settings, and settled
  progression saves.
- Upgrade, species, scenario, and balance work follows
  [`SG-005 — Upgrade and Ecology Balance`](docs/Studio%20Guidelines/SG-005-UPGRADE-AND-ECOLOGY-BALANCE.md).
- Research findings remain bounded to their declared scenario, values, seeds,
  windows, and review decision. EX-010 does not authorize a broader predictive
  or balance claim.

## Current production position

| Area | Status on 2026-09-17 | What remains |
| --- | --- | --- |
| Product definition | Complete for M0 | Revisit only when evidence changes the slice. |
| First Mutation catalog | Implemented: seven authored Hare candidates, stable application, previews, provenance, and focused tests | Player readability, Forest Edge reference-panel approval, and balance/promotion review. |
| Continuous expedition runtime | CF-0 through CF-5 implemented and verified; EX-010 accepted as bounded evidence | CF-6 duration and peak-memory measurement is stretch work, not an M1 closeout gate. |
| Player shell | Main Menu, GalapagOS Desktop, standalone Lab route, Simulation, and representative results surfaces exist; direct-start Forest Edge/Hare is the current Desktop test contract | S3 is proving the full game-state route, recovery, boundary choices, terminal outcomes, and return to the Lab; profile saving is scheduled for S4. |
| Graphics/build evidence | Target-resolution graphics acceptance and Windows development-player smoke complete | Feature-specific comprehension and presentation review. |
| Profile and progression | Local profile identity plus the first per-species Genome profile, immutable launch/run snapshot, metadata catalog, and Gene Lab display foundation exist | One approved executable Genome node, player buying/activation, versioned progression data, settlement, migration, reset, and corrupt-save behavior. |
| Research | P0–P3 and EX-010 are accepted within their approved bounds; EX-011 is accepted only as a bounded Open Range/Deer result for its tested ordered upgrade combination | No further experiment is selected; any new question requires a separate human-owned contract. |

## Milestones

| Milestone | Status | Exit gate |
| --- | --- | --- |
| **M0 — Production definition** | **Complete** | Product brief, slice roster, three builds, player/Dev Lab boundary, target platform, and non-goals are recorded. |
| **M1 — Playable upgrade loop** | **Active** | A player completes one six-round, one-minute-simulation Forest Edge expedition, makes five understandable Mutation decisions, sees the expected effects and result, and can reproduce the run in the developer evidence path without raw tuning fields. |
| **M2 — Vertical slice** | Horizon | Three builds are understandable; the full menu-to-expedition-to-settlement-to-next-expedition loop works; one Genome unlock persists and applies correctly; presentation and feedback are ready for external playtesting. |
| **M3 — Content alpha** | Hold until M2 | Several species and scenarios create distinct pressures; representative Genome configurations remain viable; feature scope, performance budgets, and save compatibility are controlled. |
| **M4 — Beta and release preparation** | Hold until M3 | Content is complete and work is focused on defects, balance, onboarding, accessibility, input, performance, compatibility, platform work, and release operations. |

## Delivery schedule

### Sprint 2 — First trustworthy upgrade loop

> **State:** Closed 2026-09-17 | **Dates:** 2026-09-03–2026-09-16 | **Capacity:** Josh
> 20h; Sim 20h

The bounded upgrade-loop goal and exit criteria were accepted at closeout.
Completed work remains in `✅ Done`; Fox telemetry is the sole carry-over and is
selected for S3 at 2h under Sim. Catalog readability, balance promotion, M1
integration, player-flow acceptance, and CF-6 remain explicit later work rather
than being treated as part of the S2 completion claim.

The authoritative execution detail remains
[`docs/NEXT_WORK_BUCKET_PLAN.md`](docs/NEXT_WORK_BUCKET_PLAN.md) and
[`docs/Sprints/S2-control-record.md`](docs/Sprints/S2-control-record.md).

### Sprint 3 — M1 closeout

> **State:** Active; kickoff verified 2026-09-17 | **Dates:**
> 2026-09-17–2026-09-30 | **Capacity:** Josh 20h; Sim 20h; 40h total

**Primary outcome:** the player can complete one six-round Forest Edge
expedition (ten seconds of simulation time per round), make and understand five
Mutation choices, recover from a bad game state, and return to the Lab without
stalls, errors, or soft-locks. The expedition screens receive a bounded visual
polish and UI integration pass.

Committed capacity allocation (profile saving remains in S4; performance
measurement is stretch work):

| Work | Features | Josh | Sim | Acceptance result |
| --- | --- | ---: | ---: | --- |
| Consolidated baseline and launch-contract acceptance | F13 | 2h | 2h | Direct-start Forest Edge/Hare is the accepted contract, obsolete fixtures are updated, and retained EditMode/PlayMode results have no unexpected failure. |
| Expedition rules and acceptance inputs | F01, F06 | 4h | 2h | Six rounds of ten seconds' simulation time; five three-option Mutation/Skip decisions; Pause, confirmed End/no rewards before round 6, no Restart; survival victory at round 6, immediate extinction failure/no rewards, and performance-based currency are recorded. Board size and playable plants remain explicitly deferred. |
| Complete game-state flow and recovery | F02, F05, F13 | 8h | — | The player can start and finish an expedition, recover from reset or other bad states, and return to the Lab without stalls, errors, or soft-locks. Profile saving remains S4 work. |
| Mutation readability and Forest Edge evidence follow-up | F03 | 2h | 6h | Five existing experimental Hare Mutations provide three distinct free choices or Skip at each boundary; later repeat selections increase level and stack the defined effect. Evidence-backed qualitative copy and one bounded Forest Edge/Hare review produce a recorded decision. |
| Visual polish and UI integration | F13, F14 | 2h | 2h | A bounded polish pass improves the expedition screens and integrates them with the player flow. |
| Integration, defect, and review reserve | Shared | 2h | 4h | Capacity remains available for discovered acceptance failures. |
| Fox telemetry carry-over | Supporting evidence | 0h | 2h | Close the mating/eating telemetry discrepancy with a regression assertion and a recorded decision. |
| **Committed total** |  | **20h** | **18h** | **2h of Sim capacity remains uncommitted.** |

**Stretch only — CF-6 duration and memory measurement:** Josh 3h / Sim 2h,
outside committed totals. If taken on, measure the six-round, one-minute-
simulation player session. It is not an M1 closeout gate and cannot start under
current allocations without a scope trade or additional capacity.

The 4-hour visual polish and UI integration split is 2h Josh / 2h Sim. Josh's
capacity is fully allocated; the remaining 2h of Sim capacity is uncommitted.
S3 kickoff operation `S3-KICKOFF-20260917-01` is recorded in the control record
and Trello control card. S3-01, S3-02, S3-03, S3-04, S3-06, S3-07, and S3-08
are in `Current Work`; S3-05 remains in Backlog as uncommitted stretch work.

During S3, any scope or allocation change needs an explicit capacity review.
Preserve the 40-hour planning limit, the M1 outcome, and the integration
reserve; do not replace the reserve with new feature scope.

### Candidate horizon after Sprint 3

Dates below assume uninterrupted two-week cadence. They are forecast windows,
not commitments.

| Sprint | Forecast dates | Primary outcome | Feature capacity | Reserve | Exit test |
| --- | --- | --- | ---: | ---: | --- |
| **S4 — Build, ecology identity, and local profile** | 2026-10-01–2026-10-14 | Trailblazer, Warren, and Gardeners create distinct decisions in Forest Edge; local profile choices save and restore between sessions. | Up to 32h across F03, F07, F11, F13, and F17; small design-only slices for F04, F09, F10, and F19 | 8h | Three builds create distinct decisions, and a local profile survives restart without changing the deterministic run contract. Playable plant species remain on hold unless separately approved. |
| **S5 — Readable presentation and feedback** | 2026-10-15–2026-10-28 | A new player can read the board, understand the important change, and explain the outcome. | Up to 32h across F05, F14, and F15; preparation only for F04 and F12 | 8h | A comprehension playtest identifies species roles, pressure, selected Mutation effect, and outcome cause. |
| **S6 — Persistence and first Genome** | 2026-10-29–2026-11-11 | One completed expedition changes a later expedition through settled, versioned progression. | Up to 32h across F06, F13, F16, and the local-profile portion of F17 | 8h | One Hare Genome unlock survives restart, can be activated or deactivated between simulations, and applies to all Hare populations only when active. |
| **S7 — Vertical-slice integration** | 2026-11-12–2026-11-25 | An external player completes and replays the slice without developer help. | Up to 32h for integration and validation; only the accepted focused slices of F08, F09, and F16 may enter | 8h | M2 passes or the evidence produces a short prioritized revision plan. |

If S6's ready work cannot fit inside 32 feature hours, split the persistence and
Genome implementation before moving S7. Do not compress migration, recovery,
or acceptance testing to preserve the forecast date.

## Feature allocation ledger

The order below is dependency order, not a promise to implement every feature
before M2.

| ID | Feature | Priority | First useful slice | Effort | Landing window | Owner / reviewer | Scheduling state |
| --- | --- | --- | --- | ---: | --- | --- | --- |
| F01 | Expedition decision loop | Now | Implement six 10-second simulation-time rounds, rewards, confirmed End/no-Restart, and terminal outcomes | M | S3 | Josh / Sim | Active |
| F02 | Phase decision moment | Now | Five readable frozen-boundary choices after rounds 1–5: one of three Mutations or Skip, with End and Continue | M | S3 | Josh / Sim | Active |
| F03 | Mutation grammar and build identity | Now → Next | Close readability review, then prove three distinct Hare paths | XL | S2 closeout; S3 review; S4 depth | Josh / Sim | Active / horizon |
| F04 | Mutation and Genome tree presentation | Next | Author the first approved Genome content and make temporary/permanent tree language understandable before functional Genome actions | L | S4 design; S5 presentation; S6 functionality | Josh / UI reviewer | Horizon |
| F05 | Cause-and-effect phase summary | Now → Next | Explain one phase and one expedition outcome in player language | M | S3; refine in S5 | Josh / Sim | Active |
| F06 | Economy and currency loop | Now → Next | Approve phase rewards, final settlement, temporary spending, and banked data | L | S3 contract; S6 implementation | Josh / Sim | Active contract / horizon |
| F07 | Species identity and counterplay | Next | Hare and Fox create different pressures and invite different responses; playable plants remain deferred | L | S4 | Josh / Sim | Horizon |
| F08 | Risk/reward events and adaptive pressure | Next → Later | Define one readable event or pressure response without hidden rubber-banding | L | S4 design; S7 only if accepted; otherwise post-M2 | Josh / Sim | Horizon |
| F09 | Species simulation and ecology depth | Next, foundational | Implement one focused predator/prey/plant interaction slice | XL | S4 design; staged validation; broad portfolio post-M2 | Josh / Sim | Horizon |
| F10 | Biome mechanics expansion | Next → Later | Prove one biome pressure that changes viable plans | XL | S4 design; implementation post-M2 unless required by the slice | Josh / Sim | Horizon |
| F11 | Scenario pressure | Next | Make Forest Edge support multiple readable responses | L | S4 | Josh / Sim | Horizon |
| F12 | Collection and Field Notes | Next | Define seen, discovered, owned, researched, and secret states | L | S5 taxonomy; full surface post-M2 | Josh / UI reviewer | Horizon |
| F13 | Expedition setup and home-base context | Next | Complete a safe Lab-to-expedition-to-Lab route, then save and restore the local player profile | L | S3 safe game route; S4 profile save/restore; S6 progression integration | Josh / UI reviewer | Active / scheduled |
| F14 | Readable visual language | Next | Lock board-scale hierarchy for terrain, roles, danger, selection, and effects | L | Current exploration; S5 acceptance | Josh / art reviewer | Active exploration / horizon |
| F15 | Audio and simulation feedback | Next | One rate-limited UI/simulation feedback palette | M | S5 | Josh / audio reviewer TBD | Horizon |
| F16 | Species Genome progression | Later, contract early | One versioned Hare node with separate unlock and active state | XL | S4 contract; S6 implementation; S7 validation | Josh / Sim | Horizon |
| F17 | Profile, save/load, and cloud sync | Later, design early | Save and restore the local player profile in S4; versioned progression, migration, reset, and corrupt-save recovery remain later; cloud is separate | XL | Local profile save/restore S4; versioned progression S6; cloud post-M2 | Josh / platform reviewer TBD | Scheduled / horizon |
| F18 | Additional species, scenarios, and builds | Later | Add one item only when it creates a new decision pattern | XL | S7 selection decision; production post-M2 | Josh / Sim | Hold |
| F19 | Reactive ecology and adaptive counterplay research | Research → Later | One bounded paired-counter design spike | L | S4 spike; implementation post-M2 | Josh / Sim | Horizon / hold |
| F20 | Predictive AI as design support | Research | EX-011's bounded decision is recorded; select another frozen question only when the core loop and review capacity justify it | XL | After core loop, economy, and metric meaning are stable | Josh / Sim | Hold; no next experiment selected |

## Dependency path

```text
S2 trustworthy Mutation foundation
  -> S3 expedition decision loop + M1 closeout
       -> S4 build, species, and scenario identity
            -> S5 readable presentation and feedback
                 -> S6 local profile, settlement, and first Genome
                      -> S7 vertical-slice integration and M2 decision

Tooling supports the path when repeated friction justifies it.
It does not become a feature prerequisite by default.
```

Shared dependencies that must be settled before their consumer starts:

| Dependency | Required before |
| --- | --- |
| Normal phase duration, decision rhythm, and terminal outcomes | F01–F06, F08, F13, F16–F20 |
| Forest Edge reference-panel approval and shared capability map | F03, F07–F11, F16, F18–F20 |
| Reward and settlement contract | F04, F06, F12–F13, F16–F17 |
| Profile ownership and stable IDs | F12–F13, F16–F17 |
| Focused ecology behavior contract and performance budget | F07–F11, F14–F15, F18–F19 |
| Approved player-facing cause-and-effect language | F02, F05, F07–F11, F14–F15 |

## Explicit holds and non-goals

The following work is not part of the initial vertical slice unless a sprint
review deliberately promotes a smaller slice:

- broad species, biome, scenario, or Genome-tree production;
- cloud sync, multiple profiles, or active-expedition disk resume;
- a generalized modifier, scripting, rule-plugin, event-bus, or navigation
  framework;
- full reactive ecology, hidden rubber-banding, or a large random-event catalog;
- predictive-model promotion or a new research program without a frozen
  question and human review capacity;
- final-volume art or audio production before comprehension evidence approves
  the direction;
- colony construction, ant tunnels, beaver dams, or other geometry-directed
  mechanics outside a separately approved experiment.

## Roadmap review rule

At every sprint review, record:

1. The playable outcome demonstrated and its evidence.
2. Planned versus actual effort by owner.
3. Acceptance failures, defects, and changed product assumptions.
4. Work completed, cut, carried, or returned to the horizon.
5. The next sprint's single primary outcome, task-level estimates, reviewer, and
   exit test.

Update this roadmap only when milestone scope, dependency order, effort band,
landing window, or product direction materially changes. Implementation status
belongs in the sprint control record and task board.

## Changes from roadmap v1 and v2.1

- Marks M0 complete and M1 active instead of leaving milestone state implicit.
- Replaces the old Lab-first route with the GalapagOS Desktop as the canonical
  player home while retaining the standalone Lab as a legacy/developer route.
- Removes completed EX-010, graphics-acceptance, and Windows-smoke work from the
  list of unresolved gates; CF-6 now means only the outer duration/memory run.
- Allocates the proposed S3 forty-hour capacity and preserves a six-hour reserve.
- Gives every named feature a stable ID, first-slice effort band, owner/reviewer,
  landing window, and scheduling state.
- Resolves the Genome “ASAP versus S6” ambiguity: the foundational identity and
  snapshot contract landed early; approved effect/content work remains staged
  from S4 and production persistence remains in S6.
- Replaces the former ten-phase, 200-tick player target with the approved
  six-round, ten-seconds-of-simulation-time cadence and five Mutation decisions.
  This does not rewrite historical research schedules or their results.
- Records the S3 player-facing End, Pause, victory, and extinction rules;
  defers board size and playable plant species; and keeps the Skip currency
  bonus undecided and non-blocking.
- Separates local profile persistence from later cloud sync and keeps
  active-expedition resume out of scope.
- Moves broad ecology, biome, collection, content expansion, cloud, and
  predictive work behind focused slice gates instead of implying that all of it
  fits into S4–S7.

## Supporting plans

- [Product brief](docs/PRODUCT_BRIEF.md)
- [Feature rationale and dependency triage](docs/GAME_FEATURE_ROADMAP_TRIAGE.md)
- [Sprint 2 plan](docs/NEXT_WORK_BUCKET_PLAN.md)
- [Active Sprint 3 control record](docs/Sprints/S3-control-record.md)
- [Mutation and Genome direction](docs/UPGRADE_SYSTEM_DIRECTION.md)
- [Scientific data economy](docs/SCIENTIFIC_DATA_ECONOMY.md)
- [Main Menu and home-base delivery](docs/MAIN_MENU_LAB_DELIVERY_PLAN.md)
- [GalapagOS Desktop feature set](docs/GALAPAGOS_DESKTOP_FEATURE_SET_PLAN.md)
- [Continuous expedition plan](docs/CONTINUOUS_SIMULATION_FLOW_PLAN.md)
- [Vertical-slice selection](docs/VERTICAL_SLICE_SELECTION.md)
- [Current collaboration state](docs/WORKING_STATE.md)
