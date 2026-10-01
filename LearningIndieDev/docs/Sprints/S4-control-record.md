# Sprint 4 — Build Strategy Identity and Evidence

> **Status:** Active; kickoff verified 2026-10-01 ([Trello control card](https://trello.com/c/Zn4UpBYc/118-sprint-4-control-record-build-identity-and-evidence))
> **Dates:** 2026-10-01–2026-10-14
> **Cadence:** two weeks  
> **Capacity:** Josh 20h; Sim 20h; 28h scheduled feature work, 8h protected integration/review reserve, and 4h unallocated

This is the active S4 control record. Sprint 3 and M1 closed on 2026-09-29.
The kickoff selected five work cards, left S4-04/05 deferred in Backlog, and
created the Trello control card linked above.
See the [S4 kickoff handoff](../handoffs/2026-10-01-codex-s4-kickoff.md) for
the board verification and next steps.

## Sprint outcome

Trailblazer, Warren, and Gardeners produce distinct strategic decisions in
Forest Edge, supported by matched-seed evidence and an in-game review. The
canonical Desktop route starts from the active profile and current setup
defaults. Local profile-choice save/restore work is deferred outside S4.

## Decisions and kickoff record — 2026-10-01

- Josh confirmed his S4 responsibilities and 20h availability. Sim's 20h
  availability is allocated as 14h feature work, 3h reserve, and 3h unallocated.
- P1-032 uses the active profile and existing setup defaults: Forest Edge,
  Hare, seed 10100, and the profile's frozen Genome snapshot. Do not add a new
  setup-choice screen or imply that `SimulationLaunchRequest` carries a
  selected upgrade schedule; normal in-game phase choices remain available.
- The planning estimate for P1-032 is 6h for profile connection, request
  application, and a focused Desktop handoff check. Josh accepts this estimate
  as the planning basis. A missing/invalid profile must fail closed with a
  visible recovery path to profile selection.
- S4-04/05 local profile-choice save/restore is deferred and is outside S4's
  outcome, exit gate, and capacity. Existing active-profile loading remains
  usable by P1-032.
- Josh approved the 400 Plants / 25 Hares / 15 Foxes comparison variant with
  a 0.1-second step. Keep the authored production asset at 400 / 55 / 35 and
  0.2 seconds; the difference is understood and does not block planning.
- The first control/candidate pilot was attempted on 2026-10-01 but Unity's
  licensing/global-mutex and Package Manager IPC failed before simulation.
  No experiment report or S4-02/03 re-estimate exists. The failure is not game
  or balance evidence.
- Scheduled allocation: Josh 14h feature + 5h reserve; Sim 14h feature +
  3h reserve. That is 28h feature work, the full 8h protected reserve, and 4h
  unallocated. Josh accepts the S4-02/03 estimates as planning assumptions.

**Kickoff status:** S4 is Active. The pilot is an early effort checkpoint
before the full 13-arm × 20-seed screen. If measured effort materially changes
the work or estimate, review the capacity trade while preserving the 8h
reserve.

## Selected work packages

**Sim review completed 2026-09-30; Josh decisions recorded 2026-10-01:** Sim
accepts 20h availability (14h feature work, 3h reserve, and 3h unallocated),
leads shared basic-skill integration and matched evidence, and reviews profile
launch inputs. Josh confirmed his S4 responsibilities and availability,
deferred S4-04/05, selected active profile + current defaults for P1-032, and
accepted the estimates as planning assumptions. See the
[accepted Sim review](../handoffs/2026-09-30-2018-codex-s4-sim-review-and-stat-sheet-strategy-plan.md)
for six basic skills, the three strategy pairs, the 20-seed ordered screen,
separate 200-seed confirmation, slash-line evidence, and in-game acceptance.
S4 kickoff was confirmed and verified on 2026-10-01.

**Josh's S4 comparison input decision, 2026-09-30:** Use 400 Plants / 25 Hares /
15 Foxes for the S4 strategy comparisons. This is an explicit experiment
variant: the checked-in production `ForestEdge.asset` still starts 400 / 55 /
35 with a 0.2-second step. The approved comparison uses a 0.1-second step.
Create or select a separately identified 25/15 scenario input and record its
source revision/fingerprint before any screening run; do not label the variant
as the current production asset or silently change the player baseline. The
in-game review must use the same inputs as the evidence it judges, or name and
test the transfer to the production route separately.

### S4 source-to-player preflight — 2026-09-30

This is a source inspection at `a1bd51ae` for planning, not implementation
acceptance. The phase-boundary player offer currently comes from
`SpeciesUpgradeCatalog.CreateExperimentalHerbivoreMutationOffer`; runtime
tests exist for several effects, but no S4 strategy comparison has run.

| Basic Hare skill | Current source path | Gap before S4 evidence can support player meaning |
| --- | --- | --- |
| General movement (`faster-movement`) | Catalog creates a +0.5 MovementSpeed upgrade; movement resolution and focused runtime tests exist. | Not in the phase Herbivore offer pool. Confirm the intended offer values/cap and prove it is selectable in the actual player route. |
| Threat Exposure | In the phase pool; +0.75 flee speed per level and cumulative avoidance are implemented and tested. | Confirm the selected level and coupled-response mode in each run; inspect escapes and food access in game rather than inferring them from eAVI alone. |
| Tough Hide | In the phase pool; +2 BlockAmount and focused block tests exist. | Verify the intended protection and cost in the approved configuration; inspect blocked contacts and visible survival, not only final population. |
| Crowding Tolerance | In the phase pool; +1 tolerance and level-bound tests exist. | The crowding effect is energy pressure, so cAVI alone cannot prove it. Review local density, starvation, births, and player-visible behavior together. |
| Efficient Digestion | In the phase pool; +0.1 DigestionEnergyBonus and deterministic feeding tests exist. | Check food consumed, energy/starvation, and the visible result; a survival difference alone is not attributable evidence. |
| Seed Dispersal | `SeedDropChance` exists in the attribute registry and resolver; a runtime test proves fed Hares can drop seeds. Production Hare chance is zero. | No catalog ID or phase offer is present. Add an approved chance/cap and direct successful seed-drop plus stored-food-cost attribution before claiming Gardeners renew plants. |

**Early effort checkpoint:** Before the 13-arm × 20-seed screen, complete one
control and one candidate through the intended variant input, schedule,
fingerprint, validation, comparison, and in-game observation. Record the actual
setup, run, analysis, and review time. The accepted S4-02/03 estimates are the
planning baseline; if this checkpoint shows a material effort difference,
review a feature-capacity trade while protecting the 8h integration/review
reserve. Freeze seed IDs, thresholds, response mode, skill values/levels, and
full confirmation paths before the separate 200-seed panel. The comparison and
player observation should answer whether someone can see and explain each
strategy's strength and tradeoff, not just whether APS changes.

**Player observation contract for that pilot:** Launch the same scenario
variant, seed, step, and skill schedule used by the paired report through the
available player route; record the route used and whether P1-032 has connected
the canonical Desktop launch. Capture the offer, a relevant board moment, and
the following phase/result summary at 1280×720. Ask a reviewer who has not
seen the raw report what changed, what caused it, and what tradeoff they expect.
Record their answer, confusing cues, and Josh's accept/revise/defer decision
beside the paired report reference. A developer-only route may diagnose the
presentation but must be labeled as such.

The candidate packages and sequence below preserve the September 29 proposal
for comparison; use the linked Sim review for his accepted refinements.

| Work package | Roadmap links | Readiness / open decision |
| --- | --- | --- |
| Co-design the selected builds, species identities, and Forest Edge pressures | F03, F07, F11 | Define what Trailblazer, Warren, and Gardeners represent and what evidence makes their strategies meaningfully distinct. |
| Validate the build differences | F03, F11 | Agree the matched-seed comparison, in-game review surface, and accept/revise gate. |
| Save and restore local profile choices (deferred) | F13, F17 | Josh deferred this work outside S4 on 2026-10-01; cards are in Backlog. |
| Small design-only spikes, if capacity remains | F04, F09, F10, F19 | Optional only; select narrowly after the primary outcome and reserve are protected. |

## Planning basis — Sim review and Josh decisions

This section records the planning basis. Selected S4 cards are in
`Current Work`; deferred profile cards remain in Backlog. The estimates are
accepted planning assumptions.

See the [Sim kickoff handoff](../handoffs/2026-09-29-1241-codex-s4-sim-kickoff-approvals.md)
for the requested capacity, feasibility, evidence, scope, and profile-boundary
decisions.

Sim's recent work provides a clear path into a bounded build-and-evidence
slice: he integrated purchased-population accounting into simulation reports
and validation tools (`1f4c7c8e`), made Hare Mutation offers readable and
eligibility-aware (`16f06ea7`), added phase-boundary Reinforcements
(`c310746d`), exposed reproduction telemetry (`cafaf6fd`), and maintained the
Forest Edge starting contract and balance (`7e6ed38c`, `f4fa9bf8`).

### Sim work sequence

1. **S4-01 — co-review the strategy matrix (Sim 2h).** Josh leads
   the player meaning and trade-offs for Trailblazer, Warren, and Gardeners;
   Sim checks whether each can be represented with supported rules and named
   inputs. Confirm whether the names describe species, builds, or loadouts.
2. **S4-02 — lead the three bounded configurations (Sim 8h).** If
   S4-01 is feasible, Sim implements only the approved differences using
   existing simulation and Mutation rules. Keep new mechanics out unless a
   separate scope trade is approved.
3. **S4-03 — lead the matched-seed evidence (Sim 4h).** Compare the
   three configurations with the same scenario, starting state, seed set, and
   windows. Preserve fingerprints and report limits; Josh leads the player
   review and accept/revise decision. Confirm the seed count and review surface
   with Sim before setting acceptance details.
4. **S4-04/05 — deferred.** Local profile-choice save/restore is out of S4.
   Sim's launch-input review remains part of P1-032 and the S4 evidence work.

The active plan allocates Sim 14h of feature work and 3h of the protected
reserve. Sim confirmed availability and his contribution in the linked review;
Josh confirmed his 20h availability and S4 responsibilities on 2026-10-01.
This proposal builds on Sim's recent simulation, Mutation, and evidence work
without assigning him product decisions or profile persistence.

### Scope constraints and in-sprint decisions

- **P1-032:** Josh owns the Desktop launch-context migration at the accepted 6h
  planning estimate. Sim reviews the launch inputs. Missing/invalid profile
  context must fail closed with visible recovery to profile selection.
- **Keep balance scope bounded:** `FUTURE_SPRINT_ROADMAP.md` describes a wider
  S4 balance lane (shared capability map, reference panel, Adaptation Value,
  and reachable Mutation-path analysis) than this 40h draft. The baseline
  covers only the approved three-build comparison; defer the broader lane
  unless the team approves a scope and capacity trade.
- **Profile boundary:** Local profile-choice save/restore is deferred. P1-032
  uses the existing active profile and its frozen Genome snapshot; no new
  profile persistence is implied. Versioned progression saves, migration,
  settlement, and production Genome persistence stay out of S4.
- **Optional Genome contract:** S4-06 costs 4h beyond the baseline. Keep it out
  unless Josh and Sim explicitly trade away other feature work.

### Decisions to complete during S4

- The strategy identity and supported skill values are the purpose of S4-01;
  movement offer/cap and Seed Dispersal value/cap are not yet authored.
- Freeze exact confirmation seeds, arms, response mode, skill values, and
  thresholds before running the separate confirmation panel.
- Keep the wider capability-map, reference-panel, and Adaptation Value lane out
  unless Josh and Sim make an explicit capacity trade.

## Scheduled task cards — S4 baseline

The five selected cards are in `Current Work`; S4-04/05 remain deferred in
Backlog. Current owners, availability, and planning estimates are accepted.
P1-032 plus S4-01/02/03 total 28h of feature estimates; the protected 8h
reserve brings the plan to 36h, leaving 4h unallocated. Run the control and
candidate pilot before scaling to the full screen and revisit the plan only if
its measured effort materially changes the estimate.

| Task | Trello card |
| --- | --- |
| P1-032 — Desktop launch-context migration (Josh, 6h accepted estimate) | [Open card](https://trello.com/c/Enm8Avxu/111-p1-032-desktop-launch-context-migration) |
| S4-01 — Build identities and counterplay | [Open card](https://trello.com/c/74VBPME5/112-s4-01-define-forest-edge-build-identities-and-counterplay) |
| S4-02 — Three bounded configurations | [Open card](https://trello.com/c/j22jDrAu/113-s4-02-implement-three-bounded-forest-edge-build-configurations) |
| S4-03 — Matched-seed comparisons and in-game review | [Open card](https://trello.com/c/xyDDiiRC/114-s4-03-run-matched-seed-comparisons-and-complete-in-game-review) |
| S4-04 — Local profile save/restore contract (deferred to Backlog) | [Open card](https://trello.com/c/JiVmlHhh/115-s4-04-define-the-local-profile-save-restore-contract) |
| S4-05 — Save and restore approved profile fields (deferred to Backlog) | [Open card](https://trello.com/c/78M83hds/116-s4-05-save-and-restore-approved-local-profile-fields) |
| S4-07 — Integration, defects, and review reserve | [Open card](https://trello.com/c/e5xd0K7c/117-s4-07-integration-defects-and-review-reserve) |

S4-06, the optional Genome contract spike, remains out of the baseline and has
no Trello card. Owner and reviewer names are in the card descriptions; Trello
member assignment fields are empty.

### Priority work — P1-032 Desktop launch-context migration

- **Scheduled slot:** First S4 work item, before S4-01.
- **Owner / reviewer:** Josh / Sim (launch-input review).
- **Estimate:** 6h, accepted as the planning estimate. Allocate from feature
  capacity, not the protected 8h integration/review reserve.
- **Outcome:** Replace the Desktop's local Forest Edge/Hare preview start with
  an active-profile launch using current defaults through the immutable
  `SimulationLaunchRequest` contract.
- **Acceptance:** Apply a request built from the active profile's frozen Genome
  snapshot and current defaults (Forest Edge, Hare, seed 10100) before starting.
  A focused PlayMode handoff test proves those values arrive unchanged. The
  request currently has no upgrade-schedule field; normal in-game phase choices
  remain. Missing/invalid profile context must visibly fail and provide a
  recovery route to profile selection instead of silently starting a local run.
  Keep the standalone developer preview isolated. Profile persistence, reward
  settlement, and cloud saves remain out of scope.
- **Capacity gate:** The 6h estimate and safe recovery behavior are accepted
  for planning. Do not exceed the 32h feature cap or draw from the 8h reserve.

### S4-01 — Define Forest Edge build identities and counterplay

- **Features:** F03, F07, F11
- **Owner / reviewer:** Josh / Sim
- **Estimate:** Josh 4h; Sim 2h (6h total)
- **Outcome:** Turn Trailblazer, Warren, and Gardeners into three testable
  Forest Edge strategies. Resolve whether each name means a species, build, or
  loadout; record the expected pressure, choices, strengths, and trade-offs.
- **Acceptance:** Josh and Sim approve one concise strategy matrix; each entry
  has concrete setup inputs and an observable expected difference; the matrix
  defines matched-seed conditions and what evidence would make a strategy
  distinct. No new species roster or broad ecology system is implied.
- **Depends on:** S3's accepted expedition and Mutation contracts.
- **Why S4:** This contract makes the sprint's build-identity outcome
  implementable and reviewable.

### S4-02 — Implement the three bounded Forest Edge build configurations

- **Features:** F03, F07, F11
- **Owner / reviewer:** Sim / Josh
- **Estimate:** Josh 2h; Sim 8h (10h total)
- **Outcome:** Configure only the minimum differences approved in S4-01,
  reusing supported species and Mutation paths.
- **Acceptance:** All three configurations can be selected and launched; focused
  tests prove the intended inputs differ and remain deterministic; unsupported
  effects are not approximated with hidden behavior. No general framework,
  production Genome effects, or broad balance expansion.
- **Depends on:** S4-01 and the S3 rules baseline.
- **Why S4:** It realizes the three strategies without pulling later Genome
  implementation into the build-identity work.

### S4-03 — Run matched-seed comparisons and complete the in-game review

- **Features:** F03, F11
- **Owner / reviewer:** Sim / Josh
- **Estimate:** Josh 2h; Sim 4h (6h total)
- **Outcome:** Compare the approved configurations under the same Forest Edge
  inputs and make an accept/revise decision using the existing player flow.
- **Acceptance:** Retained evidence records the seed, configuration/loadout
  fingerprint, relevant outcomes, observed decisions, and limitations for each
  comparison; the in-game review explains the intended differences; Josh and
  Sim record an accept/revise decision. Claims stay within the tested seeds.
- **Depends on:** S4-02.
- **Why S4:** Matched evidence and player review are part of the stated S4
  outcome, not optional broad balance research.

### S4-04 — Define the local profile save/restore contract (deferred)

- **Features:** F13, F17
- **Proposed owner / reviewer:** Josh / Sim
- **Estimate:** Josh 2h; Sim 1h (3h total)
- **Outcome:** Decide which player choices the first local save preserves, when
  it saves/loads, where it is stored, and what the Lab does when no usable
  profile is present.
- **Acceptance:** A short contract names the persisted fields and default/read/
  write behavior, plus focused checks. It excludes active-expedition resume,
  cloud sync, settlement, progression migration, and production Genome
  persistence. If platform-specific storage assumptions remain, name the
  reviewer needed before implementation.
- **Depends on:** S3's safe Lab-to-expedition-to-Lab route.
- **Why deferred:** Josh chose to keep local profile-choice persistence out of
  S4 on 2026-10-01; retain the card for a later planning pass.

### S4-05 — Save and restore the approved local profile fields (deferred)

- **Features:** F13, F17
- **Proposed owner / reviewer:** Josh / Sim
- **Estimate:** Josh 5h; Sim 2h (7h total)
- **Outcome:** Persist exactly the fields approved in S4-04 through an
  application restart.
- **Acceptance:** A focused test and a player-flow check save, exit, relaunch,
  and restore the expected choices; missing or failed storage follows the
  documented safe default without breaking the Lab; an already-started run is
  unaffected. No active-run resume, cloud, migration, settlement, or production
  Genome save/load.
- **Depends on:** S4-04 and the accepted S3 player route.
- **Why deferred:** Josh chose to keep local profile-choice persistence out of
  S4 on 2026-10-01; retain the card for a later planning pass.

### S4-07 — Integration, defects, and review reserve

- **Features:** Shared integration
- **Owner / reviewer:** Josh + Sim / joint review
- **Estimate:** Josh 5h; Sim 3h (8h reserve)
- **Outcome:** Keep capacity available for defects found while integrating the
  S4 outcome and verifying its acceptance checks.
- **Acceptance:** Reserve use is tied to a named S4 blocker or failed gate; the
  integrated build and focused tests are reviewed; unused reserve stays
  unused rather than becoming new feature scope.
- **Why S4:** The forecast explicitly protects 8h for integration and review.

**S4 allocation:** Josh 14h feature work + 5h reserve; Sim 14h
feature work + 3h reserve. Feature work totals 28h against a 32h cap; total
planned capacity is 36h against 40h, leaving 4h unallocated.

## Optional card requiring a capacity trade

### S4-06 — Define the first production Genome node contract (design only)

- **Feature:** F16
- **Proposed owner / reviewer:** Josh / Sim
- **Estimate:** 2h each (4h total; not included in the baseline above)
- **Outcome:** Specify one candidate node's identity, player meaning, cost and
  prerequisites, activation boundary, and evidence needed for a later
  implementation decision.
- **Acceptance:** The contract distinguishes permanent unlock from active
  state, defines species-keyed application and launch-time freezing, and
  records a validation plan. No production asset, runtime effect, purchase
  action, or persistence is implemented in this card.
- **Depends on:** The existing Genome contract and the `UPG-C07` species-
  identity constraint.
- **Why optional:** The roadmap feature ledger calls for an S4 contract, but
  the S4 capacity row does not allocate hours to F16. Include this only after
  an explicit 4h trade from baseline feature work; do not consume the reserve.

## Exit gate

- The three named builds create distinct decisions in Forest Edge, demonstrated
  with matched-seed evidence and reviewed in-game.
- The canonical Desktop starts from the active profile and current setup
  defaults without changing deterministic simulation behavior.
- The result is reviewed and either accepted or returned with a short,
  prioritized revision list.

## Scope boundaries to preserve

- This sprint does not claim the M2 vertical slice is complete.
- Local profile-choice save/restore is deferred; active-run resume, cloud sync,
  settlement, versioned progression migration, and production Genome
  persistence remain outside S4.
- Broad species/scenario expansion and full reactive-ecology implementation
  remain out of scope. Any design spike must be small and must not displace the
  primary outcome or the 8h reserve.
- S4 is Active. Complete the pilot before the full screen; revisit capacity if
  its measured effort materially changes the accepted estimates.

## Kickoff record — 2026-10-01

- [x] Reconcile S3 closeout: committed work is in Trello Done; S3-05 remains
  uncommitted stretch work in Backlog.
- [x] Promote the five selected cards to `Current Work`; keep deferred profile
  cards in Backlog.
- [x] Confirm Josh and Sim responsibilities and 20h availability each.
- [x] Defer S4-04/05 profile-choice persistence outside S4.
- [x] Choose active profile + current defaults for P1-032; accept its 6h
  planning estimate within feature capacity.
- [x] Agree that P1-032 fails closed to a visible profile-selection recovery
  path; Sim reviews its launch inputs.
- [x] Accept the current S4-02/03 estimates as planning assumptions; run the
  pilot before scaling to the full screen and revisit the capacity plan if
  measured effort materially changes the estimates.
- [x] Create the Active S4 control card in `🧭 Roadmap & Milestones` and verify
  the board state by read-back.
- Freeze confirmation seeds, arms, response mode, values, and thresholds before
  running the separate confirmation panel during S4.
- Keep feature work at or below 32h and retain 8h for integration, defects,
  and review.

## Planning basis

- [Roadmap v2.2](../../ROADMAP.md), Candidate horizon after Sprint 3 and feature
  allocation ledger.
- [S3 control record](S3-control-record.md), Next sprint after S3.
- [Sprint kickoff workflow](../SPRINT_KICKOFF_WORKFLOW.md).
