# Sprint 4 — Build, Ecology Identity, and Local Profile (Draft)

> **Status:** Proposed; not committed or kicked off  
> **Forecast dates:** 2026-10-01–2026-10-14, assuming uninterrupted two-week cadence  
> **Cadence:** two weeks  
> **Planning capacity:** up to 32h feature work plus an 8h integration/review reserve (40h total); owner allocation TBD

This is a planning skeleton, not an approved sprint plan. Revisit it after S3
closeout, then refine and confirm the work, estimates, owners, and acceptance
checks before promoting anything into the active sprint.

## Proposed outcome

Trailblazer, Warren, and Gardeners produce distinct strategic decisions in
Forest Edge, supported by matched-seed evidence and an in-game review. The
player's local profile choices save and restore between sessions without
changing the deterministic run contract.

## Candidate work packages — not yet committed

| Work package | Roadmap links | Readiness / open decision |
| --- | --- | --- |
| Co-design the selected builds, species identities, and Forest Edge pressures | F03, F07, F11 | Define what Trailblazer, Warren, and Gardeners represent and what evidence makes their strategies meaningfully distinct. |
| Validate the build differences | F03, F11 | Agree the matched-seed comparison, in-game review surface, and accept/revise gate. |
| Save and restore local profile choices | F13, F17 | Confirm the persisted fields, restart boundary, and recovery behavior; keep this separate from active-expedition resume and production progression saves. |
| Small design-only spikes, if capacity remains | F04, F09, F10, F19 | Optional only; select narrowly after the primary outcome and reserve are protected. |

## Proposed task cards — baseline set

These are draft card definitions, not Trello cards and not yet selected for
`🎯 Upcoming Work`. IDs, owners, and estimates are proposals to refine after S3
closeout. The five feature cards total 32h; the separate 8h reserve brings the
plan to 40h (Josh 20h, Sim 20h).

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

**Baseline allocation:** Josh 15h feature work + 5h reserve; Sim 17h feature
work + 3h reserve. Feature work is capped at 32h; total planned capacity is
40h.

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
- S4 remains Proposed until S3 is closed and a separate kickoff is confirmed.

## Before promotion to `🎯 Upcoming Work`

- Reconcile S3 closeout and carry-over decisions.
- Define each candidate card's problem/outcome, stable task ID, owner, reviewer,
  estimate, dependencies/risks, and observable acceptance check.
- Confirm profile data and restart/recovery boundaries.
- Confirm matched-seed evidence and the in-game review gate.
- Allocate no more than 32h to feature work and retain 8h for integration,
  defects, and review.

## Planning basis

- [Roadmap v2.2](../../ROADMAP.md), Candidate horizon after Sprint 3 and feature
  allocation ledger.
- [S3 control record](S3-control-record.md), Next sprint after S3.
- [Sprint kickoff workflow](../SPRINT_KICKOFF_WORKFLOW.md).
