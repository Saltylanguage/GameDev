# S4-01 Hare workbook review

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-10-01-1711-codex-s4-01-hare-workbook-review
- Owner: codex
- Branch: BevBranch
- Baseline commit: a389355
- Date: 2026-10-01
- Supersedes: none

## Summary

Codex prepared the S4-01 Hare strategy workbook with Bevin, who approved all
remaining design recommendations on October 1. This shares his starting design
for Salty (Josh) and his agent to review. It records intended behavior and test
values; it does not claim implementation, team acceptance or validated balance.

Open the [workbook](../Species%20Design/S4-01%20Hare%20Strategies%20and%20Upgrade%20Paths.xlsx)
and read its first three tabs in order: S4 Strategies, S4 Upgrade Paths, then
S4 Basic Skills. The original five DARWIN OR DIE stat-sheet tabs remain intact
as dated references. Read this alongside the
[September 30 Sim review](2026-09-30-2018-codex-s4-sim-review-and-stat-sheet-strategy-plan.md)
and [S4 control record](../Sprints/S4-control-record.md).

## Changes

- Added the approved workbook under `docs/Species Design/`.
- Added this review handoff and its link in the working-state index.
- The workbook distinguishes current implementation from approved starting
  design. Accepted statuses refer to Bevin's design approval.
- Renamed the draft's player-facing Threat Exposure skill to Threat Avoidance,
  retaining the existing name as an implementation reference.
- Recorded the accepted five-pick paths and longer alternating examples. No
  gameplay code, serialized assets or experiment results are part of this change.

## Decisions and assumptions

All six shared basic skills start capped at level 10. These are provisional
starting values for evidence runs, not final balance tuning.

| Basic skill | Approved starting behavior per level |
| --- | --- |
| General Movement | Add 0.5 general movement speed. |
| Threat Avoidance | Add 8 percentage points of attack avoidance. Remove the current first-pick flee-speed bonus; General Movement supplies speed. |
| Tough Hide | Add 2 BlockAmount. |
| Crowding Tolerance | Remove 10% of the ORIGINAL extra crowding energy cost. Subtract equal amounts from that surcharge, not 10% of the remaining cost. At level 10 the surcharge is zero; normal metabolism remains. |
| Efficient Digestion | Add 0.1 energy per successful feeding. Fractions accumulate; the maximum energy still applies. |
| Seed Dispersal | Add 1 percentage point of planting chance per eligible simulation step. A successful planting creates a plant with its normal starting food supply and consumes 1 stored-food unit. Failed attempts are free. Stored food and suitable nearby space are required. |

Each pick forgoes another upgrade. Add no further energy or food penalties in
the initial design. The strategy names describe combinations of freely mixable
basics, not fixed classes or mandatory packages.

| Strategy | Goal | Accepted first five picks |
| --- | --- | --- |
| Trailblazer | Escape predators and reach fresh food. | Movement, Avoidance, Movement, Avoidance, Movement. |
| Warren | Sustain a dense breeding population. | Tough Hide, Crowding Tolerance, Tough Hide, Crowding Tolerance, Tough Hide. |
| Gardeners | Feed efficiently and renew plant food. | Efficient Digestion, Seed Dispersal, Efficient Digestion, Seed Dispersal, Efficient Digestion. |

The longer examples continue alternating to level 10 in both core skills.
They describe future priorities beyond today's five Mutation decisions;
they do not extend the expedition or approve future choice timing. Players
can mix other basics. Optional Warren litter changes stay deferred until the
core works; no literal burrow mechanic is assumed.

The initial screen remains 13 arms x 20 matched seeds = 260 runs: skip-all,
six single skills, and each pair in both acquisition orders. Singles acquire
one level at tick 100; pairs acquire one level each at ticks 100 and 200;
all later choices are skipped. Retain early extinctions and acquisition reach.

The proposed test fixture is Forest Edge, 36x20, 400 Plants / 25 Hares /
15 Foxes, 600 ticks, explicit 0.1-second steps and no reinforcement purchases.
Coupled Fox responses remain OFF, with Fox rules fixed, for both the screen
and first confirmation. Confirmation uses 200 separate matched seeds and a
fresh control. Select its paths and freeze numeric success thresholds after
screening, before confirmation; do not tune repeatedly on the confirmation panel.

**Fixture distinction:** the committed scenario at baseline `a389355` still
authors 400 Plants / 55 Hares / 35 Foxes and a 0.2-second step. Bevin's local
scenario edits are excluded from this push. Review the intended test profile
explicitly rather than assuming a fresh checkout reproduces the workbook
fixture. No gameplay-default change is approved by this handoff.

Show the full Hare slash line, matched-control changes and plain-language
interpretation, together with direct diagnostics:

- Trailblazer: eAVI, pAVI and predAVG; support with sAVI/FPO and inspect actual
  travel, food access and escape behavior.
- Warren: pAVI, sAVI, bAVG, RFS and FPO; inspect blocked attacks, crowding
  energy loss and sustained breeding. cAVI cannot establish that energy benefit.
- Gardeners: sAVI, bAVG, RFS and FPO; attribute successful seed placements,
  created plant food and stored food spent. Aggregate plant births are insufficient.
- Keep APS and its components visible as supporting evidence. Do not substitute
  a higher composite score for recognizable strategy behavior. Preserve ADD-aware
  population accounting, raw counts, denominators, validity and seed variation.

## Validation

- Workbook creation/edit checks passed: affected formulas recalculated;
  changing a path pick updated its final-level summary and was then restored;
  longer paths reach level 10 in both skills; no formula errors were found in
  the S4 tabs. Changed views were rendered and visually checked.
- Read-only saved-file checks confirmed the original five source tabs retain
  their values, styles, merges and native tables. Dropdowns and the 13-arm
  screen remain present.
- The repository copy is byte-identical to the approved output. SHA-256:
  `FFDFB58C4ABD8653F713D24BE845819404597D680EBBF4B575148D285364AC28`.
- The handoff validator found no schema or local-link errors in this note.
  The full journal still fails on 17 errors in older notes, with historical
  artifact warnings. Those unrelated notes were left unchanged.
- No native Excel UI test, Unity compilation, simulation, or in-game strategy
  acceptance run was performed for this design draft.

## Risks and incomplete work

- Movement offer integration, selectable Seed Dispersal, the avoidance bonus
  change, cap enforcement and the crowding redesign still require implementation.
- The current crowding threshold benefit saturates at Hare level 2 under the
  inspected local rules. The new surcharge design needs suitable fractional
  energy handling; it must not round the intended low-level benefit away.
- Seed chance is checked per eligible simulation STEP, not per feeding or
  real-time second. Review resource creation carefully: one stored-food unit
  currently produces a full starting plant patch. This is an approved test
  value whose ecological effect remains unmeasured.
- Original stat-sheet tabs are preserved historical references, including
  stale notes. The S4 tabs identify important differences; do not treat the
  original expected outcomes as current measured results.
- Planning freshness differs: the committed S4 control record still says
  Proposed, while the live [S4-01 card](https://trello.com/c/74VBPME5)
  inspected October 1 is in Current Work and says S4 Active, October 1-14,
  with Josh's ownership confirmed and Josh 4h + Sim 2h accepted estimates.
  Preserve the newer card state. This handoff does not reconcile the broader
  sprint documents or independently establish every dependency's completion.
- The live card says Josh-approved existing authored values remain as written.
  Salty should explicitly review the first-pick flee bonus removal and crowding
  redesign, in addition to the newly specified movement and seed choices.
- Seven pre-existing local modifications were excluded from the sharing commit,
  including the scenario and older local evidence in WORKING_STATE.

## Next useful step

Salty and his agent should review player meaning, implementation feasibility
and the comparison plan before S4-02 implementation. Return Accept, Revise or
Questions for each strategy, with concrete proposed workbook cell changes and
the code or guideline evidence supporting technical concerns. Identify which
issues affect this draft and which belong to later implementation or balance work.

Salty can give his agent this prompt:

> Review the S4-01 Hare design on origin/BevBranch. Fetch the branch and read
> this handoff, the linked workbook's first three S4 tabs, the September 30
> Sim review and SG-005. Preserve local work and inspect the actual committed
> upgrade pools, effects, feeding, crowding and seed resolver. Check that each
> strategy's goal, first five picks, increments, cap, costs and slash-line
> evidence agree. Treat Bevin's accepted values as starting design, separate
> from implemented behavior and validated balance. Flag the committed fixture
> difference and the longer paths' five-choice limit. Return Accept, Revise or
> Questions per strategy, cite concrete evidence, and propose exact cell edits
> where needed. Review only: do not implement gameplay, run experiments or
> change Trello. Record your findings in a new handoff for Salty to approve;
> after approval, share the review through the normal repository workflow.
