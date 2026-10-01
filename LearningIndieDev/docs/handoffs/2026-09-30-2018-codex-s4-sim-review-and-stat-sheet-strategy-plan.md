# S4 Sim review and stat-sheet strategy plan

[Working state](../WORKING_STATE.md) | Status: ready-for-review

> **Planning correction, 2026-09-30:** Josh selected 400 Plants / 25 Hares /
> 15 Foxes for the S4 comparison. The scenario row below records the review's
> intended comparison values, but its phrase “Current production Forest Edge”
> is inaccurate: the checked-in production asset is 400 / 55 / 35 with a
> 0.2-second step. Use a separately identified 25/15 input and the explicit
> 0.1-second comparison step. See the current [S4 control record](../Sprints/S4-control-record.md)
> before running or interpreting the experiment. This note preserves Sim's
> reviewed proposal rather than rewriting it as a different historical review.

- Handoff schema: 1
- Handoff ID: 2026-09-30-2018-codex-s4-sim-review-and-stat-sheet-strategy-plan
- Owner: codex
- Branch: BevBranch
- Baseline commit: b049c35
- Date: 2026-09-30
- Supersedes: none

## Summary

Sim (Bevin) completed the guided review of the September 29 S4 proposal,
using the DARWIN OR DIE stat-sheet work and current simulation rules. He
accepts 20h availability: 17h feature work and 3h protected review/integration
reserve. Trailblazer, Warren, and Gardeners should emerge from combinations
of shared basic Hare skills that players can freely mix.

This records Sim's accepted position for Josh's review. It does not confirm
Josh's availability, finalize the team plan, approve ecology balance, or start
S4. P1-032 still needs an estimate and an explicit feature-capacity trade.

Sources: [original approval questions](2026-09-29-1241-codex-s4-sim-kickoff-approvals.md),
[S4 control record](../Sprints/S4-control-record.md), and the read-only workbook
`F:\Downloads\DARWIN OR DIE STAT SHEET (updated 2026-09-28)(1).xlsx`.

## Changes

- Added this review handoff and linked it from the S4 control record and working
  state. No gameplay, workbook, or experiment changes were made in this review.
- Refined the original configuration proposal into six shared basic skills,
  three recognizable combinations, and a bounded comparison plan.

## Decisions and assumptions

### Shared skills and strategy identities

| Strategy | Core basic skills | Intended player-visible behavior | Remaining definition work |
| --- | --- | --- | --- |
| Trailblazer | General movement + Threat Exposure | Escape predators and reach fresh food. | Add the existing general movement effect to shared Hare offers; confirm values, caps, and interaction with fleeing. |
| Warren | Tough Hide + Crowding Tolerance | Protect and sustain a dense breeding population. | Confirm values and costs; valid litter-size choices can support the identity after its core works. No literal burrow or territory mechanic is assumed. |
| Gardeners | Efficient Digestion + Seed Dispersal | Feed efficiently while actively renewing plant food. | Expose seed dispersal as a shared basic choice using the existing resolver; confirm chance, progression, and stored-food cost. |

The names describe recognizable strategies, not fixed classes or launch
packages. Players may mix the basic skills. Existing skills can be adjusted
where necessary to support these identities, with agreed numeric definitions
and bounds before evidence runs.

Current source inspection found general movement implemented but absent from
the Hare offer pools, and seed dispersal supported by an existing resolver.
Hare reproduction chance is already 1.0, so a chance increase is not a valid
breeding improvement. The phase and experimental Hare offer pools differ;
verify the intended player path rather than assuming every workbook choice
is available there.

Broader Fox retuning, exhaustive Mutation-path research, capability mapping,
Adaptation Value work, and new ecology mechanics require an explicit scope
trade. The optional Genome contract spike remains outside the baseline.

### Experiment contract

| Input | Accepted review position |
| --- | --- |
| Scenario | Current production Forest Edge, 36x20, 400 Plants / 25 Hares / 15 Foxes; zero starting probabilities. |
| Player and reporting species | Hare. |
| Duration | Six phases of 100 ticks, 600 ticks total. |
| Step interval | Explicit 0.1 seconds. The asset authors 0.2 seconds; preserve this distinction in reports. |
| Reinforcements | No purchases in these comparisons. |
| Screening | 20 matched seeds; exact seed IDs still to be fixed. |
| Confirmation | 200 separate matched seeds for selected strategies and a fresh control. Freeze configurations, schedules, response mode, and thresholds before confirmation. |
| Coupled Fox responses | OFF for initial screening, with Fox rules held fixed. Review the intended player mode separately; confirmation mode remains open. No gameplay-default change is approved here. |

Initial screening covers one control, six single skills, and each of the
three core pairs in both acquisition orders: 13 arms x 20 seeds = 260 runs.
Control skips all five choices. Singles acquire one level at tick 100 and
skip later choices. Pairs acquire A at tick 100 and B at tick 200, then skip;
the reverse arm acquires B then A. Both orders finish with one level of each.
Retain early extinctions and report how many runs reached each acquisition.
Ordered acquisition follows the existing UPG-C04 concern contract.

This is first-level screening. Repeated-level bounds and full representative
five-choice confirmation paths still need definition and correctness checks.
The 20-seed screen informs revisions and numeric acceptance thresholds;
confirmation must not become repeated tuning against the same seed panel.

### Evidence and acceptance

Always present the full Hare slash line, matched-control changes, and a
plain-language interpretation alongside diagnostic evidence. The slash line
helps express what players may perceive; an in-game review must check that
interpretation against visible behavior.

| Strategy | Slash-line emphasis and supporting evidence |
| --- | --- |
| Trailblazer | eAVI, pAVI, predAVG, with sAVI and FPO; inspect escaping and food access. |
| Warren | pAVI, sAVI, bAVG, RFS, FPO; inspect protection, crowding pressure, and sustained breeding. |
| Gardeners | sAVI, bAVG, RFS, FPO plus attributable plant renewal and its food cost. |
| All | Keep APS visible, with its components, raw counts, denominators, validity status, shared reporting windows, acquisition reach, and seed variation. |

Do not treat unavailable values as zero or rely only on an average. Current
eAVI uses full-species encounter-tick exposure EHS/HPS, not merely contacted
individuals. MAT represents mating candidate checks. Crowding applies energy
pressure without necessarily producing direct CRWD deaths, so cAVI alone
cannot establish crowding tolerance. Preserve ADD-aware population accounting
and reconcile the report against the current implementation; older workbook
notes about omitted ADD are stale.

Plant births aggregate multiple causes. Gardeners needs separate successful
seed-drop and stored-food-cost attribution to demonstrate renewal from the
skill. Any diagnostic addition must preserve simulation ordering and RNG use.

Acceptance requires all three checks:

1. Correct effects, level bounds, acquisition order, deterministic
   reproducibility, and reconciled reports.
2. Matched evidence supporting the intended identity, strengths, tradeoffs,
   and variation, with thresholds fixed before confirmation.
3. An in-game Sim/Josh review connecting visible behavior to the strategy
   description and slash-line interpretation.

A higher APS alone does not establish a strategy's identity. Historical
baselines and the earlier composite Guarded Burrow pilot are context, not
validation of these six basic definitions or the new pair matrix.

### Ownership, estimates, and profile boundary

| Sim contribution | Starting estimate |
| --- | --- |
| S4-01: co-review strategy definitions and feasibility | 2h |
| S4-02: lead basic-skill integration and approved configurations | 8h |
| S4-03: lead matched evidence and report interpretation | 4h |
| S4-04: review local-profile contract at the simulation boundary | 1h |
| S4-05: review profile launch-input integration | 2h |
| Protected integration/review reserve | 3h |
| Total availability accepted by Sim | 20h |

These are starting estimates, not guarantees that the clarified work fits
8h implementation plus 4h evidence. Check integration, attribution, run and
report effort early; agree a feature scope or capacity trade before overruns.
Keep Sim's 3h and the team's 8h reserve protected.

Josh remains the proposed owner of player meaning, UI/profile flow, exact
persisted fields, and recovery behavior, subject to his confirmation. Sim
reviews correct inputs at launch, frozen active-run inputs, reproducibility,
and safe fallback. Scope is approved local player choices for future launches.
Active-expedition resume, cloud sync, settlement, progression migration, and
expanded production Genome persistence remain separate.

P1-032 Desktop launch-context migration precedes S4-01. Josh must estimate it
and confirm ownership; the team must name the feature-capacity trade before
kickoff because it is currently outside the 40h baseline. Preserve Sim's
accepted 20h unless he agrees otherwise. Acceptance must prove the actual
player launch path supplies the intended inputs.

## Validation

- Reviewed the existing S4 proposal, current authored scenario, Hare offers,
  upgrade effects, seed resolver, coupled response mappings, metric formulas,
  and workbook contents during the guided discussion.
- This is planning evidence only. No Unity compilation, gameplay tests,
  matched experiments, or player acceptance runs were performed in this review.
- The new handoff passed the repository handoff validator with zero warnings;
  the planning-file whitespace check passed. The full historical handoff scan
  still fails on 17 errors in older notes (missing local artifacts and invalid
  status text), with additional historical artifact warnings. None identifies
  this new handoff; those older issues were left outside this review.

## Risks and incomplete work

- Numeric skill definitions, repeat coverage, seed IDs, confirmation paths,
  confirmation response mode, and thresholds are still open.
- Josh's ownership/capacity confirmation and the P1-032 estimate/capacity trade
  remain required before final scope and separate kickoff.
- Existing unrelated checkout changes include scenario/editor/presentation/
  project settings and earlier local evidence in WORKING_STATE. Preserve them;
  sharing this review must include only its planning changes.
- S4 remains Proposed. No Trello edits, commit, or push occurred while preparing
  this review.

## Next useful step

Bevin explicitly authorized committing/pushing these planning changes to
BevBranch and applying the matching Trello updates on 2026-09-30. Sharing does
not approve implementation or kickoff. The approved card-update scope is:

| Card | Update to reflect this review |
| --- | --- |
| P1-032 | Josh to confirm ownership, estimate migration, and name a feature-capacity trade; preserve reserve and verify the actual player launch inputs. |
| S4-01 | Record the six shared basic skills and three emergent strategies; Sim co-reviews feasibility and Josh confirms player meaning. |
| S4-02 | Define/integrate the approved basic skills and bounds, including movement offers and seed dispersal; keep 8h as a starting estimate. |
| S4-03 | Record the 13-arm ordered 20-seed screen, separate 200-seed confirmation, full slash line plus diagnostics, and three-part acceptance gate. |
| S4-04 | Josh defines local-choice fields/recovery; Sim reviews launch-input freezing and reproducibility. |
| S4-05 | Keep save/restore implementation within the approved profile boundary and include Sim's launch-path review. |
| S4-07 | Protect the 8h team reserve, including Sim's 3h; require feature trades after the early estimate checkpoint. |

Keep cards in planning; do not imply team approval or move them into an active
sprint from this review alone.

Josh reviews this response and confirms his responsibilities. Resolve the
P1-032 estimate and feature trade, then agree the final card scope and early
estimate checkpoint. Share the planning-only change and matching card updates
under the recorded authorization. Freeze remaining experiment details before runs;
schedule a separate sprint kickoff only after the team plan is approved.
