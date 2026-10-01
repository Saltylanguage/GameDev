# Sprint 4 — Build, Ecology Identity, and Local Profile (Draft)

> **Status:** Proposed; not committed or kicked off  
> **Forecast dates:** 2026-10-01–2026-10-14, assuming uninterrupted two-week cadence  
> **Cadence:** two weeks  
> **Planning capacity:** up to 32h feature work plus an 8h integration/review reserve (40h total); owner allocation TBD

This is a planning skeleton, not an approved sprint plan. Sprint 3 and M1
closed on 2026-09-29. Refine and confirm this plan's work, estimates, owners,
and acceptance checks before promoting anything into an active sprint; a
separate kickoff is still required.

## Proposed outcome

Trailblazer, Warren, and Gardeners produce distinct strategic decisions in
Forest Edge, supported by matched-seed evidence and an in-game review. The
player's local profile choices save and restore between sessions without
changing the deterministic run contract.

## Candidate work packages — not yet committed

**Sim review completed 2026-09-30:** Sim accepts 20h availability (17h feature
work plus 3h protected reserve), leads shared basic-skill integration and
matched evidence, and reviews profile launch inputs. The original per-card
hours remain starting estimates, with an early implementation/evidence
checkpoint and explicit feature trades before overruns. See the [accepted Sim
review](../handoffs/2026-09-30-2018-codex-s4-sim-review-and-stat-sheet-strategy-plan.md)
for six basic skills, the three strategy pairs, the 20-seed ordered screen,
separate 200-seed confirmation, slash-line evidence, and in-game acceptance.
Josh's confirmation and the P1-032 estimate/feature-capacity trade are still
required. S4 remains Proposed; this review does not schedule kickoff.

The candidate packages and sequence below preserve the September 29 proposal
for comparison; use the linked Sim review for his accepted refinements.

| Work package | Roadmap links | Readiness / open decision |
| --- | --- | --- |
| Co-design the selected builds, species identities, and Forest Edge pressures | F03, F07, F11 | Define what Trailblazer, Warren, and Gardeners represent and what evidence makes their strategies meaningfully distinct. |
| Validate the build differences | F03, F11 | Agree the matched-seed comparison, in-game review surface, and accept/revise gate. |
| Save and restore local profile choices | F13, F17 | Confirm the persisted fields, restart boundary, and recovery behavior; keep this separate from active-expedition resume and production progression saves. |
| Small design-only spikes, if capacity remains | F04, F09, F10, F19 | Optional only; select narrowly after the primary outcome and reserve are protected. |

## Planning checkpoint — Sim discussion before finalizing

This section records a proposal for the owner discussion. Draft cards are now
staged in `🎯 Upcoming Work` so they can be reviewed with Sim. The assignments
and estimates below are the original discussion draft. Sim's response is
recorded above; staging cards and completing this review do not finalize team
scope or start S4.

See the [Sim kickoff handoff](../handoffs/2026-09-29-1241-codex-s4-sim-kickoff-approvals.md)
for the requested capacity, feasibility, evidence, scope, and profile-boundary
decisions.

Sim's recent work provides a clear path into a bounded build-and-evidence
slice: he integrated purchased-population accounting into simulation reports
and validation tools (`1f4c7c8e`), made Hare Mutation offers readable and
eligibility-aware (`16f06ea7`), added phase-boundary Reinforcements
(`c310746d`), exposed reproduction telemetry (`cafaf6fd`), and maintained the
Forest Edge starting contract and balance (`7e6ed38c`, `f4fa9bf8`).

### Provisional Sim work sequence

1. **S4-01 — co-review the strategy matrix (2h in this draft).** Josh leads
   the player meaning and trade-offs for Trailblazer, Warren, and Gardeners;
   Sim checks whether each can be represented with supported rules and named
   inputs. Confirm whether the names describe species, builds, or loadouts.
2. **S4-02 — lead the three bounded configurations (8h in this draft).** If
   S4-01 is feasible, Sim implements only the approved differences using
   existing simulation and Mutation rules. Keep new mechanics out unless a
   separate scope trade is approved.
3. **S4-03 — lead the matched-seed evidence (4h in this draft).** Compare the
   three configurations with the same scenario, starting state, seed set, and
   windows. Preserve fingerprints and report limits; Josh leads the player
   review and accept/revise decision. Confirm the seed count and review surface
   with Sim before setting acceptance details.
4. **S4-04/05 — keep profile ownership with Josh (3h from Sim in this draft,
   primarily review).** Sim can review that selected profile inputs are frozen
   at launch and do not break deterministic runs. Ask whether this support is
   needed; the contract and player save/restore flow should remain Josh-led.

This draft allocates Sim 17h of feature work and 3h of the protected reserve.
Sim has now confirmed availability and his contribution in the linked review.
Josh and the team must still confirm the final scope and capacity. This proposal
builds on his recent simulation, Mutation, and evidence work without assuming
he owns product decisions or profile persistence.

### Open planning decisions

- **P1-032 capacity:** The Desktop launch-context migration is scheduled before
  S4-01, but its estimate is still TBD and it is outside the five-card 32h
  feature baseline. Estimate it and name the capacity trade before kickoff.
- **Keep balance scope bounded:** `FUTURE_SPRINT_ROADMAP.md` describes a wider
  S4 balance lane (shared capability map, reference panel, Adaptation Value,
  and reachable Mutation-path analysis) than this 40h draft. Confirm with Sim
  which small comparison is necessary for the three-build outcome; defer the
  broader lane unless the team approves a scope and capacity trade.
- **Profile boundary:** Keep S4 to local player choices if retained. Clarify
  that versioned progression saves, migration, settlement, and Genome
  persistence stay in S6; confirm the minimal field set and restart behavior.
- **Optional Genome contract:** S4-06 costs 4h beyond the baseline. Keep it out
  unless Josh and Sim explicitly trade away other feature work.

### Questions for the Sim discussion

- Can the three build identities be expressed with supported species and
  Mutation rules? Which smallest differences would make their choices clear?
- What seed count, measurements, and report fields are enough to compare them
  without turning S4 into broad balance research?
- Are the proposed 8h for configurations and 4h for matched evidence realistic,
  and is 17h of feature work plus 3h reserve available?
- Does the Desktop launch-context migration or the profile field set create a
  dependency for the configuration/evidence work?
- Which parts of the wider capability-map, reference-panel, and Adaptation
  Value direction are needed now, and which can remain future work?

## Proposed task cards — baseline set

These draft cards are in `🎯 Upcoming Work` for planning review. IDs, owners,
and estimates remain proposals to refine with Sim. The five feature cards
total 32h; the separate 8h reserve brings the plan to 40h (Josh 20h, Sim 20h).

| Task | Trello card |
| --- | --- |
| P1-032 — Desktop launch-context migration (pre-S4; estimate/trade TBD) | [Open card](https://trello.com/c/Enm8Avxu/111-p1-032-desktop-launch-context-migration) |
| S4-01 — Build identities and counterplay | [Open card](https://trello.com/c/74VBPME5/112-s4-01-define-forest-edge-build-identities-and-counterplay) |
| S4-02 — Three bounded configurations | [Open card](https://trello.com/c/j22jDrAu/113-s4-02-implement-three-bounded-forest-edge-build-configurations) |
| S4-03 — Matched-seed comparisons and in-game review | [Open card](https://trello.com/c/xyDDiiRC/114-s4-03-run-matched-seed-comparisons-and-complete-in-game-review) |
| S4-04 — Local profile save/restore contract | [Open card](https://trello.com/c/JiVmlHhh/115-s4-04-define-the-local-profile-save-restore-contract) |
| S4-05 — Save and restore approved profile fields | [Open card](https://trello.com/c/78M83hds/116-s4-05-save-and-restore-approved-local-profile-fields) |
| S4-07 — Integration, defects, and review reserve | [Open card](https://trello.com/c/e5xd0K7c/117-s4-07-integration-defects-and-review-reserve) |

S4-06, the optional Genome contract spike, remains out of the baseline and has
no Trello card. The descriptions identify owners and estimates as provisional;
they are not assigned to Trello members yet.

### Priority carry-over — P1-032 Desktop launch-context migration

- **Scheduled slot:** First work item in the next post-S3 work block, before
  S4-01. The S4 forecast begins 2026-10-01, but S4 is still a draft and this
  item needs an estimate and a capacity trade at kickoff before work starts.
- **Proposed owner / reviewer:** Josh / UI-runtime owner.
- **Estimate:** TBD; allocate from feature capacity, not the protected 8h
  integration/review reserve.
- **Outcome:** Replace the Desktop's local Forest Edge/Hare preview with the
  profile- and Expedition-Setup-driven launch through the existing immutable
  `SimulationLaunchRequest` contract.
- **Acceptance:** The Desktop route passes the selected scenario, species,
  seed/schedule, and frozen profile/Genome inputs into the simulation run; a
  focused PlayMode handoff test proves those values arrive unchanged. The
  normal Desktop route no longer chooses hardcoded local-preview defaults, and
  a missing/invalid request returns to setup with a clear failure instead of
  silently starting a local run. Keep the standalone developer preview
  isolated. Profile persistence, reward settlement, and cloud saves remain out
  of scope.
- **Capacity gate:** At S4 kickoff, add the approved estimate and trade against
  baseline feature work before promoting the card. Do not silently exceed the
  32h feature cap or draw from the 8h reserve.

### S4-01 — Define Forest Edge build identities and counterplay

- **Features:** F03, F07, F11
- **Proposed owner / reviewer:** Josh / Sim
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
- **Proposed owner / reviewer:** Sim / Josh
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
- **Proposed owner / reviewer:** Sim / Josh
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

### S4-04 — Define the local profile save/restore contract

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
- **Why S4:** The roadmap schedules local profile choices for this sprint while
  keeping progression persistence later.

### S4-05 — Save and restore the approved local profile fields

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
- **Why S4:** It completes the planned local-profile outcome without claiming
  the later progression/save system.

### S4-07 — Integration, defects, and review reserve

- **Features:** Shared integration
- **Proposed owner / reviewer:** Josh + Sim / joint review
- **Estimate:** Josh 5h; Sim 3h (8h reserve)
- **Outcome:** Keep capacity available for defects found while integrating the
  S4 outcome and verifying its acceptance checks.
- **Acceptance:** Reserve use is tied to a named S4 blocker or failed gate; the
  integrated build and focused tests are reviewed; unused reserve stays
  unused rather than becoming new feature scope.
- **Why S4:** The forecast explicitly protects 8h for integration and review.

**Draft allocation to validate:** Josh 15h feature work + 5h reserve; Sim 17h
feature work + 3h reserve. Feature work is capped at 32h; total planned
capacity is 40h.

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

## Proposed exit gate

- The three named builds create distinct decisions in Forest Edge, demonstrated
  with matched-seed evidence and reviewed in-game.
- A local profile survives an application restart and restores the intended
  player choices without changing deterministic simulation behavior.
- The result is reviewed and either accepted or returned with a short,
  prioritized revision list.

## Scope boundaries to preserve

- This sprint does not claim the M2 vertical slice is complete.
- Local profile save/restore is in scope for consideration; active-run resume,
  cloud sync, settlement, versioned progression migration, and production
  Genome persistence remain separate work unless explicitly re-planned.
- Broad species/scenario expansion and full reactive-ecology implementation
  remain out of scope. Any design spike must be small and must not displace the
  primary outcome or the 8h reserve.
- S4 remains Proposed until Sim reviews the proposed ownership and capacity,
  this plan is refined, and a separate kickoff is confirmed.

## Before final approval and kickoff

- [x] Reconcile S3 closeout: committed work is in Trello Done; S3-05 remains
  uncommitted stretch work in Backlog.
- [x] Stage the draft task cards in `🎯 Upcoming Work` for planning review.
- Confirm the proposed Sim work sequence, estimates, and available capacity
  with Sim before finalizing the plan.
- Confirm each draft card's problem/outcome, task ID, owner, reviewer, estimate,
  dependencies/risks, and observable acceptance check with the team.
- Confirm profile data and restart/recovery boundaries.
- Confirm matched-seed evidence and the in-game review gate.
- Allocate no more than 32h to feature work and retain 8h for integration,
  defects, and review.

## Planning basis

- [Roadmap v2.2](../../ROADMAP.md), Candidate horizon after Sprint 3 and feature
  allocation ledger.
- [S3 control record](S3-control-record.md), Next sprint after S3.
- [Sprint kickoff workflow](../SPRINT_KICKOFF_WORKFLOW.md).
