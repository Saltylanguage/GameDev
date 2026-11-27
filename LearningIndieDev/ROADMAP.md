# Cellular Automata Roguelike Roadmap v2.9

> **Status:** Active production roadmap | **Version:** 2.9 | **Updated:**
> 2026-11-27 | **Cadence:** Two weeks, approximately 20 hours per developer |
> **Product owner:** Josh

This is the product-level source of truth for what the team is trying to make,
what comes next, and what must be true before work moves forward. Feature briefs,
sprint control records, Trello cards, and technical plans provide execution
detail; they do not silently expand this roadmap.

**November 18 visual target decision:** Josh removed 1280×720 from the supported
M2 acceptance target and retired questionnaire Q5. Review the current player
route at 1920×1080; no replacement minimum resolution has been specified.
Earlier 1280×720 captures remain historical evidence, not a current gate.

**November 16 meeting direction, recorded November 17:** Josh listed
wrapping, ponds/rivers, a verifiable ScriptableObject Trinket skeleton,
Mutation cards, terrain upgrades, a cozier creature-scale simulation and
additional playable species into technical Track A before balance work. Josh
will help Sim with Track A. Visual polish and player feedback across UI, VFX,
Field Report and audio form deferred Track B; the reward-screen prototype is
the polish benchmark for all screens. The [meeting handoff](docs/handoffs/2026-11-17-0741-codex-m2-vertical-slice-meeting-tracks.md)
links the Trello work and open decisions. Reconcile this added scope with the
ten-outcome M2 gate below and the M3 content boundary before marking new work
accepted. No balance value or milestone closeout was approved by this update.

**November 27 sprint transition:** [S5](docs/Sprints/S5-control-record.md) is
Closed: all 16 Field Data and six GLJP cards are Done, while WTR-01 was carried
forward unfinished. [S6 — Track A technical foundations](docs/Sprints/S6-control-record.md)
is Active for the November 16–29 window, formalized after work began. Its
selected subset is VS-A1, WTR-01, VS-A3, VS-A6 and VS-A7. Josh owns these
cards for now; Sim may assist when available. Remaining-hour estimates and
specific review assignments are open, so no capacity-fit claim is made.
November 29 is a technical review and carry-over
checkpoint. The complete Rabbit play review and three-response accept/revise
gate are reforecast after Track A, before M2 can close; S7 dates and scope
must be reset from that evidence. This supersedes the older S6 Fun Pass sprint
schedule, not its player outcome or the permanent progression already built.

## Product outcome

Deliver a run-based game built around **cellular automata as a roguelike**. The
player guides one species through a journey of simulation nodes and decisions
on a map. Each simulation node has six ten-second rounds in one continuing
ecosystem. The current implementation carries the played species' population
count into the next simulation node and recreates individuals. The November 18
design target is to carry surviving Rabbits' identity, state, and event history
across nodes. Nonplayed populations still reset from authored data. Temporary
**Mutations** shape the journey. Successful settlement
can fund permanent Gene Lab research and **Genome Upgrades** for future
journeys. The long-term objective is a diverse, resilient ecosystem rather
than one dominant species.

The first vertical slice uses:

- **Scenario:** Forest Edge.
- **Player species:** Rabbit.
- **Opposing species:** Fox. Playable plant species are on hold and are not part
  of the current expedition contract.
- **Introductory Mutation responses to Fox pressure:** avoidance, survival,
  and faster reproduction. Trailblazer, Warren, and Gardeners remain the
  original build vocabulary, not three accepted playable M2 strategies.
- **Home base:** the GalapagOS Desktop. Lab features such as Gene Lab, Species
  Collection, History, and Expedition Planning live within that player-facing
  home. The standalone `Lab` scene remains a legacy/developer route until its
  retained tests and responsibilities are deliberately migrated.

Josh confirmed Rabbit, Fox, Owl, Deer, and Wolf as the planned playable test
roster on November 18. VS-A7 brings them into normal selection and simulation
for mechanics testing before balance; the M2 review route above remains pinned
to Rabbit. Rabbit uses the canonical species ID `rabbit`.

## How to read this roadmap

Every item has one of three scheduling states:

- **Active:** explicitly selected for the current sprint; any capacity exception is recorded in its control.
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

- The accepted M1 expedition contains six ten-second rounds in one world. The
  current Field Data journey connects multiple such simulation nodes through
  map, reward, and shop decisions. Freeze the representative M2 route at the
  reforecast Rabbit play review; do not describe one node as the whole journey.
- The M1 node starts without a Mutation and has decisions after rounds 1–5.
  The current journey also has node-end reward and shop decisions; terminal
  results occur when the journey succeeds, fails, or is ended.
- Mutations are temporary for the current run and exclusive to Species
  Simulations. Each is communicated with a small icon and keyword, not a full
  statistic breakdown; its expected impact should be clear in the following
  round.
- Pause is available. There is no player Restart of an active journey. End
  abandons it after confirmation, forfeiting unbanked Field Data and temporary
  Mutations; already permanent balances and unlocks remain.
- The M1 one-node victory rule is historical acceptance. For the M2 journey,
  extinction ends the whole journey without cash-out. The exact authored
  phase-clear and route-survival expectations remain under the reforecast
  Rabbit play review.
- The Field Data journey has implemented round payouts, shops, and permanent
  success settlement. Their player-facing balance and M2 result explanation
  still need review; older proposed performance and Skip-bonus formulas do not
  override the current economy.
- Permanent **Genome Upgrades** are bought with currency in the Gene Lab
  application on the GalapagOS Desktop and fill a **Genome** skill tree.
- Forest Edge board size is deferred; playable plant species, including Fern,
  are on hold and are not contract requirements.
- Genome nodes remain unlocked permanently, while each species has a separate
  active Genome configuration that can change only between simulations.
- Every participating species receives its frozen active Genome at launch,
  including species that are not controlled by the player.
- Versioned local profiles, settlement, Gene Lab purchase/equipment paths,
  and researched journey-Mutation eligibility now exist. Final catalog prices,
  some effects, player acceptance, and a real exit/relaunch check remain open;
  current code is not approval of provisional values.
- The canonical GalapagOS Desktop route now uses the active profile and a
  frozen Forest Edge/Rabbit launch request at seed 10100. Focused Main Menu
  PlayMode checks pass; Sim's launch-input review and a real application
  exit/relaunch save check remain open.
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

| Area | Status on 2026-09-29 | What remains |
| --- | --- | --- |
| Product definition | Complete for M0 | Revisit only when evidence changes the slice. |
| First Mutation catalog | Implemented: seven authored Rabbit candidates, stable application, previews, provenance, and focused tests | S3 V1 readability and bounded Forest Edge review are accepted; wider balance and promotion review remain iterative follow-up. |
| Continuous expedition runtime | CF-0 through CF-5 implemented and verified; EX-010 accepted as bounded evidence; S3 six-round route and recovery accepted | M1 exit gate accepted and closed 2026-09-29; later changes may need retesting. CF-6 duration and peak-memory measurement remains stretch work. |
| Player shell | Main Menu, GalapagOS Desktop, standalone Lab route, Simulation, and representative results surfaces exist; the Desktop launches from the active profile | S3 route and UI acceptance remain recorded. Sim's launch-input review and a real exit/relaunch profile check remain open; selected Main Menu art is in production. |
| Graphics/build evidence | Target-resolution graphics acceptance, Windows development-player smoke, and bounded S3 interface review complete | Further visual polish and feature-specific comprehension checks continue iteratively. |
| Profile and progression | Versioned local multi-profile save/restore now holds currency, unlock IDs, Genome state, simulation ticks, and playtime; the Desktop freezes the selected Genome at launch | A real exit/relaunch check, Sim review, approved permanent wallets and settlement, the first playable Genome node, and purchase/activation rules. Corrupt records have a recovery copy, but broader migration and reset UX remain later work. |
| Research | P0–P3 and EX-010 are accepted within their approved bounds; EX-011 is accepted only as a bounded Open Range/Deer result for its tested ordered upgrade combination | No further experiment is selected; any new question requires a separate human-owned contract. |

## Milestones

| Milestone | Status | Exit gate |
| --- | --- | --- |
| **M0 — Production definition** | **Complete** | Product brief, slice roster, three builds, player/Dev Lab boundary, target platform, and non-goals are recorded. |
| **M1 — Playable upgrade loop** | **Complete — 2026-09-29** | A player completes one six-round, one-minute-simulation Forest Edge expedition, makes five understandable Mutation decisions, sees the expected effects and result, and can reproduce the run in the developer evidence path without raw tuning fields. |
| **M2 — Vertical slice** | Horizon | All ten M2 outcomes below pass on the same representative Forest Edge/Rabbit journey, including three understandable strategies, a persistent unlock, and an unassisted next journey. |
| **M3 — Content alpha** | Hold until M2 | Several species and scenarios create distinct pressures; representative Genome configurations remain viable; feature scope, performance budgets, and save compatibility are controlled. |
| **M4 — Beta and release preparation** | Hold until M3 | Content is complete and work is focused on defects, balance, broader onboarding and accessibility coverage, input, performance, compatibility, platform work, and release operations. The M2 onboarding/accessibility baseline is earlier. |

### M2 outcome contract — consolidated 2026-11-14

These are the ten required outcomes, not ten claims of completion. Existing
implementation, design decisions, and tests are starting evidence; M2 closes
only after one representative, legitimate player route passes all rows. S6
reviews Track A technical foundations. The complete Rabbit journey and its
three responses move to the next planned play review, before S7 integrates the
menu-to-next-journey experience and obtains external-player evidence. S7's
dates and scope must be reforecast from the S6 checkpoint. The
[Trello M2 milestone](https://trello.com/c/PoxE1gpz/19-m2-vertical-slice) links
one ticket per outcome with provisional remaining-effort ranges; refine those
ranges and assign owners and reviewers at kickoff rather than treating them as
sprint commitments.

| Required outcome | Current foundation / open acceptance | Planned gate |
| --- | --- | --- |
| Main menu, profile/new/continue, scenario/species selection, run introduction | Menu and versioned profiles exist. Desktop Simulation must open Species & Scenario Selection with Rabbit and Forest Edge selected, Rabbit alone available, and other shown species as locked silhouettes; Continue then opens Journey, first reward, and first simulation. Verify real exit/relaunch and launch inputs. | S7 integration |
| Complete live-run UI: readable board, inspection, objectives, upgrades, feedback | Board, Field Ledger, controls, choices, and map exist. A player must find the Fox pressure, inspect relevant state, understand the current objective and next action at 1920×1080. | Post-Track A comprehension; S7 visual acceptance |
| One curated scenario and small visually distinct species roster | Forest Edge with Rabbit, Fox, and Plant resources is authored. Keep Fern as a resource, not a promised playable species; validate the chosen route and visual identities rather than expanding content. | Post-Track A route review; S7 visual acceptance |
| Three understandable strategies using the proven upgrade grammar | Current introductory answers are avoidance, survival, and faster reproduction. Verify each is available to a fresh profile, has an observable effect and tradeoff, and can be described after legitimate play. Preserve older Trailblazer/Warren/Gardeners research as history. | Post-Track A Rabbit play gate |
| Selected art direction on board and core UI | Pastel eco-OS and board-first direction are approved; production art and event cues are partial. Review actual board, menu, choices, results, and imported art at 1920×1080; do not substitute concept images for runtime art. | S7 presentation gate |
| Initial UI, upgrade, danger, outcome, ambience, and music audio language | Menu and birth chimes exist; the wider palette and sourced ambience/music remain open. Validate distinct, restrained cues, mute/volume behavior, and 4× fatigue without requiring final-volume production. | S7 presentation gate |
| Aggregated, rate-limited dense-activity feedback | Birth grouping, a chime limit, and prioritized reactions are partial. Test crowded and 4× play for truthful, bounded visual/audio cues and a readable summary of suppressed routine events. | Post-Track A feedback; S7 acceptance |
| Results identify primary causes and upgrade contributions | Settlement, population, stat lines, and loadouts exist. Explain leading observed causes and show directly measured Mutation effects or a clearly identified matched comparison; do not claim causal contribution from a loadout list alone. | Post-Track A causal read; S7 result gate |
| Versioned meta-progression unlocks at least one scenario, species, or upgrade option | Versioned profile, settlement, Gene Lab research, and journey-Mutation eligibility are implemented. A fresh profile must earn, buy, save, relaunch, and use one permanent unlock on the next journey; verify active versus owned state. | S7 persistence gate |
| Onboarding, accessibility baseline, clean next-run flow | Results can return to map or start again; some focus and volume controls exist. A first-time player must reach and replay the route without developer help, with visible keyboard focus, readable text/contrast, mute, and a reduced-motion equivalent for essential feedback. | S7 external-player gate |

The November 10 node reset and November 11–13 carcass findings are constraints
on this gate, not balance approvals. The 2:1 energy ledger did not predict
natural survival, and no Fox count or production ratio was selected. See the
[S6 control](docs/Sprints/S6-control-record.md) and the
[carcass report](docs/Research/Experiments/S6-Fox-Carcass-Value-20261111/REPORT.md)
before changing the introductory scenario.

## M1 closeout — 2026-09-29

Josh accepted the M1 exit gate using the distributed evidence recorded in the
[M1 closeout handoff](docs/handoffs/2026-09-29-codex-m1-closeout.md). The Clean
suite passed EditMode 263/263 and PlayMode 34 passed, 2 expected graphics-only
skips, 0 failed. The cap fix and related integration changes are preserved in
commit `974827f2` on `codex/forest-edge-visual-pass`, pushed to `origin`.
Josh accepts the evidence package without a single artifact pairing a complete
player run with its developer report; later changes may require retesting. At
M1 closeout, S4 remained Proposed; its kickoff was verified on 2026-10-01.

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

> **State:** Closed 2026-09-29; kickoff verified 2026-09-17 | **Dates:**
> 2026-09-17–2026-09-30 | **Capacity:** Josh 20h; Sim 20h; 40h total

**Primary outcome:** the player can complete one six-round Forest Edge
expedition (ten seconds of simulation time per round), make and understand five
Mutation choices, recover from a bad game state, and return to the Lab without
stalls, errors, or soft-locks. The expedition screens receive a bounded visual
polish and UI integration pass.

S3 closed on 2026-09-29 after manual review. M1 was separately accepted and
closed on 2026-09-29. S4 started on 2026-10-01 after planning decisions were
resolved and the Trello kickoff was verified.

Committed capacity allocation (local profile-choice save/restore was later
deferred beyond S4 on 2026-10-01; performance measurement remains stretch work):

| Work | Features | Josh | Sim | Acceptance result |
| --- | --- | ---: | ---: | --- |
| Consolidated baseline and launch-contract acceptance | F13 | 2h | 2h | Direct-start Forest Edge/Rabbit is the accepted contract, obsolete fixtures are updated, and retained EditMode/PlayMode results have no unexpected failure. |
| Expedition rules and acceptance inputs | F01, F06 | 4h | 2h | Six rounds of ten seconds' simulation time; five three-option Mutation/Skip decisions; Pause, confirmed End/no rewards before round 6, no Restart; survival victory at round 6, immediate extinction failure/no rewards, and performance-based currency are recorded. Forest Edge defaults to 36×20; playable plants remain deferred. |
| Complete game-state flow and recovery | F02, F05, F13 | 8h | — | The player can start and finish an expedition, recover from reset or other bad states, and return to the Lab without stalls, errors, or soft-locks. Profile saving was not part of S3; Josh later deferred local profile-choice persistence beyond S4 on 2026-10-01. |
| Mutation readability and Forest Edge evidence follow-up | F03 | 2h | 6h | Each boundary offers three distinct applicable free Mutations from the existing five-Mutation Rabbit pool, or Skip. Invalid choices are hidden, repeats stack their defined effects, and Reinforcements remain a separate purchase. Evidence-backed qualitative copy and the bounded Forest Edge/Rabbit review are accepted for S3 V1. |
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
and Trello control card. S3 closed on 2026-09-29: committed cards S3-01 through
S3-04, S3-06 through S3-08, and the control card are in `✅ Done`; S3-05 remains
in Backlog as uncommitted stretch work. M1 closed on 2026-09-29 after Josh
accepted the distributed exit-gate evidence. S4 was Proposed at that closeout
and was activated after plan decisions were resolved on 2026-10-01.

During S3, any scope or allocation change needs an explicit capacity review.
Preserve the 40-hour planning limit, the M1 outcome, and the integration
reserve; do not replace the reserve with new feature scope.

### Delivery schedule after Sprint 3

S4 closed on 2026-10-15 with the research and Desktop foundation published.
Its original in-game strategy acceptance remains open. P1-032 returned to
Backlog for Sim's launch-input review and a real save/exit/relaunch check;
S4-04/05 remain Backlog. S5 closed administratively November 27 after its
November 15 execution end; Field Data and GLJP are Done and WTR-01 carried into
S6. S6 is Active Track A for November 16–29, with a technical review at its
end. S7 and the full Rabbit play review must be reforecast after that checkpoint.

| Sprint | Forecast dates | Primary outcome | Feature capacity | Reserve | Exit test |
| --- | --- | --- | ---: | ---: | --- |
| **S4 — Build identity and evidence (Closed 2026-10-15)** | 2026-10-01–2026-10-14 | Trailblazer, Warren, and Gardeners create distinct decisions in Forest Edge; the canonical Desktop starts from the active profile and current defaults. | Historical plan: 28h scheduled feature work across P1-032, F03, F07, and F11; local profile-choice save/restore and optional design spikes deferred | 8h | Research and Desktop foundation published; four selected cards Done. The original in-game strategy acceptance and independent review remain open, and P1-032 was returned to Backlog. See the [closeout](docs/Sprints/S4-control-record.md#closeout--2026-10-15). |
| **S5 — Field Data and playability (Closed 2026-11-27)** | 2026-10-19–2026-11-15 | Deliver and review the Field Data and GLJP foundations; FD-T01–FD-T16 and GLJP-01–06 are Done. | Historical 57–95h Field Data range; actual remaining hours were not reconstructed | No protected reserve under the October 16 exception | WTR-01 carried into S6 unfinished. Closeout does not claim Rabbit route, balance or M2 acceptance. See the [S5 control](docs/Sprints/S5-control-record.md). |
| **S6 — Track A technical foundations (Active)** | 2026-11-16–2026-11-29 | Reconcile water/wrapping and prove the smallest useful Trinket, creature-scale Rabbit and five-species sandbox foundations before balance. | Josh owns VS-A1, WTR-01, VS-A3, VS-A6 and VS-A7; Sim may assist. Remaining-hour estimates are open and capacity fit is unverified | Set at technical review | Record topology decision or blocker, verified prototype state, estimate/reviewer reconciliation and deliberate carry-over. Full Rabbit Fun Pass play review is reforecast after Track A. M2 remains open. See the [S6 control](docs/Sprints/S6-control-record.md). |
| **S7 — Vertical-slice integration (Proposed)** | TBD; reforecast after S6 | Plan the Rabbit play-review gate first, then select a realistic S7 integration scope for the ten M2 outcomes and external-player review. The play review is not automatically committed to one sprint. | Select and size after S6 evidence; do not treat all open work as pre-committed | Reforecast | Every M2 outcome passes on the representative route, or the evidence produces a short prioritized revision plan and M2 stays open. |

Do not treat every Track A card as committed merely because S6 is Active.
Assign remaining estimates and review responsibility for the selected cards
before claiming capacity fit. The November source is published on `ProjectMain` at
`49c81f91`; the procedural-water prototype remains separate. The former S6
persistence/Genome forecast is superseded as a sprint schedule, while its
remaining M2 acceptance remains open.

## Feature allocation ledger

The order below is dependency order, not a promise to implement every feature
before M2.

| ID | Feature | Priority | First useful slice | Effort | Landing window | Owner / reviewer | Scheduling state |
| --- | --- | --- | --- | ---: | --- | --- | --- |
| F01 | Expedition decision loop | Now | Preserve six 10-second rounds per simulation node, map/node transitions, confirmed End, and whole-journey terminal outcomes | M | M1 foundation; S5 Field Data; post-Track A route review | Josh / Sim | M1 and Field Data foundations delivered; current route acceptance open |
| F02 | Phase decision moment | Now | Keep readable frozen round choices, node-end rewards, shop decisions, and a clear next map action | M | M1 foundation; S5 Field Data; post-Track A comprehension | Josh / Sim | Foundations delivered; player comprehension open |
| F03 | Mutation grammar and build identity | Now → Next | Prove fresh-profile avoidance, survival, and faster-reproduction responses to Fox pressure; retain original build research | XL | S4 research; post-Track A Rabbit play review | Josh / Sim | Grammar and bounded research delivered; three-response player acceptance open |
| F04 | Mutation and Genome tree presentation | Next | Author the first approved Genome content and make temporary/permanent tree language understandable before functional Genome actions | L | content not selected in S5 kickoff; functional actions after their contract | Josh / UI reviewer | Proposed content / horizon |
| F05 | Cause-and-effect phase summary | Now → Next | Explain observed Fox, food, and reproduction pressure and directly supported Mutation effects in phase and journey results | M | S3 foundation; post-Track A causal review; S7 integrated result | Josh / Sim | Metrics and results foundation; player explanation open |
| F06 | Economy and currency loop | Now → Next | Preserve the accepted Field Data loop and verify its contribution to playable decisions; reconcile permanent progression against the current branch | XL | S5 Field Data accepted; post-Track A play review | Assign next-review owner | FD workstream Done; November integration and player review remain separate |
| F07 | Species identity and counterplay | Next | Rabbit and Fox create different pressures and invite different responses; playable plants remain deferred | L | not selected in Field Data S5 kickoff | Josh / Sim | Proposed / horizon |
| F08 | Risk/reward events and adaptive pressure | Next → Later | Define one readable event or pressure response without hidden rubber-banding | L | S4 design; S7 only if accepted; otherwise post-M2 | Josh / Sim | Horizon |
| F09 | Species simulation and ecology depth | Next, foundational | Implement one focused predator/prey/plant interaction slice | XL | S4 design; staged validation; broad portfolio post-M2 | Josh / Sim | Horizon |
| F10 | Biome mechanics expansion | Next → Later | Prove one biome pressure that changes viable plans | XL | S4 design; implementation post-M2 unless required by the slice | Josh / Sim | Horizon |
| F11 | Scenario pressure | Next | Make Forest Edge support multiple readable responses | L | not selected in Field Data S5 kickoff | Josh / Sim | Proposed / horizon |
| F12 | Collection and Field Notes | Next | Define seen, discovered, owned, researched, and secret states | L | taxonomy unscheduled; full surface post-M2 | Josh / UI reviewer | Horizon |
| F13 | Expedition setup and home-base context | Next | Verify canonical profile/menu-to-Desktop-to-journey-to-results-to-next-journey route, constrained selection, and real exit/relaunch | L | S3/S4 foundation; S7 integration | Josh / Sim for launch inputs | Foundation published; full route acceptance open |
| F14 | Readable visual language | Next | Apply approved pastel eco-OS and board-first direction to menu, board, choices, and results at the 1920×1080 acceptance target | L | Current exploration; S7 presentation | Josh / art reviewer | Partial production assets and visual evidence; review open |
| F15 | Audio and simulation feedback | Next | Initial UI, upgrade, danger, outcome, ambience, and music palette with truthful dense-event limits and mute/reduced-motion equivalents | M | Post-Track A feedback; S7 presentation | Josh / audio reviewer TBD | Cozy Pond integrated; full palette and listening acceptance open |
| F16 | Species Genome progression | Now → Later | Preserve the integrated Gene Lab research, owned/equipped distinction, journey eligibility, and permanent settlement; prove one earned unlock applies on a later journey | XL | S5 GLJP review; S7 persistence gate | Josh / Sim | Functional paths and focused tests exist; final catalog choices, player review, and relaunch acceptance open |
| F17 | Profile, save/load, and cloud sync | Now → Later | Local versioned multi-profile save/restore and recovery copy exist; verify real exit/relaunch and the earned-unlock loop. Active-run resume and cloud remain separate. | XL | Local foundation published; S7 acceptance; cloud post-M2 | Josh / platform reviewer TBD | Foundation ready for real relaunch review |
| F18 | Additional species, scenarios, and builds | Next | Bring the confirmed Rabbit/Fox/Owl/Deer/Wolf roster into a test scenario and selection flow with provisional rules and working test upgrades | XL | VS-A7 technical expansion; balance and production acceptance later | Josh / Sim | Roster confirmed; implementation and estimate pending |
| F19 | Reactive ecology and adaptive counterplay research | Research → Later | One bounded paired-counter design spike | L | S4 spike; implementation post-M2 | Josh / Sim | Horizon / hold |
| F20 | Predictive AI as design support | Research | EX-011's bounded decision is recorded; select another frozen question only when the core loop and review capacity justify it | XL | After core loop, economy, and metric meaning are stable | Josh / Sim | Hold; no next experiment selected |

## Dependency path

```text
S2 trustworthy Mutation foundation
  -> S3 expedition decision loop + M1 closeout
       -> S4 build, species, and scenario identity
            -> S5 Field Data journey and GLJP work (Closed Nov 27)
                 -> S6 Track A technical foundations (Active Nov 16–29)
                      -> Rabbit play review, S7 integration and M2 decision (reforecast)

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
- cloud sync or active-expedition disk resume;
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
- The earlier S6 persistence/Genome forecast is superseded by the November 9
  playability decision. Existing implementation and pending acceptance must be
  reconciled at kickoff; no completed work is re-scoped by this forecast.
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
- [Closed Sprint 3 control record](docs/Sprints/S3-control-record.md)
- [Closed Sprint 4 control record](docs/Sprints/S4-control-record.md)
- [Closed Sprint 5 control](docs/Sprints/S5-control-record.md)
- [Active Sprint 6 control — Track A technical foundations](docs/Sprints/S6-control-record.md)
- [Historical S5 simulation and upgrades proposal](docs/Sprints/S5-proposal-2026-10-15.md)
- [Sprint 4 kickoff handoff](docs/handoffs/2026-10-01-codex-s4-kickoff.md)
- [Mutation and Genome direction](docs/UPGRADE_SYSTEM_DIRECTION.md)
- [Scientific data economy](docs/SCIENTIFIC_DATA_ECONOMY.md)
- [Main Menu and home-base delivery](docs/MAIN_MENU_LAB_DELIVERY_PLAN.md)
- [GalapagOS Desktop feature set](docs/GALAPAGOS_DESKTOP_FEATURE_SET_PLAN.md)
- [Continuous expedition plan](docs/CONTINUOUS_SIMULATION_FLOW_PLAN.md)
- [Vertical-slice selection](docs/VERTICAL_SLICE_SELECTION.md)
- [Current collaboration state](docs/WORKING_STATE.md)
