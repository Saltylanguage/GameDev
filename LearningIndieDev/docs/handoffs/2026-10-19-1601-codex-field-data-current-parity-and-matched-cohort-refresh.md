# Field Data current parity and matched cohort refresh

[Working state](../WORKING_STATE.md) | Status: complete

- Handoff schema: 1
- Handoff ID: 2026-10-19-1601-codex-field-data-current-parity-and-matched-cohort-refresh
- Owner: codex
- Branch: BevBranch
- Baseline commit: acc371a
- Date: 2026-10-19
- Supersedes: October 17 parity/cohort as evidence of current rules only; historical artifacts remain preserved.

## Summary

Bevin authorized the two recommended evidence refreshes. A fresh authored
snapshot passes direct Unity/headless parity across eight action fixtures,
including all six mutations owned before round 7. The current 200-seed × three
policy comparison completed and validated all 600 journeys. The
[report](../Research/Experiments/FD-Current-Loop-Refresh-20261019/REPORT.md)
records setup, results, reproduction, limits and local artifact paths.

## Changes

- Exported current Forest Edge and starter definition to
  `artifacts/field-data-t14/verified-20261019/`.
- Froze rebuilt runner/dependencies, snapshot, 200 seeds and v3 schedules under
  `artifacts/field-data-t15/verified-20261019/`. v3 removes population requests
  at node ends, preserving the three declared spending policies.
- Added the report and refreshed working-state, SDD, ticket-evidence and
  self-play pointers. No gameplay source, ecology values or prices changed.

## Decisions and assumptions

- Authored setup: 400 Plants / 55 Hares / 20 Foxes, 36×20 nonwrapping map,
  0.2-second interval, empty active Genome, 18 rounds of 100 ticks.
- Shop-only income, repeatable six-skill ordinary rewards and no node-end
  population controls are included in the frozen source/definition.
- HEAD `acc371a` is only an anchor for the dirty working tree. Current shared
  source hash: `476b1e8d8d5e06e084247976a21c7b2ed8c4ffa34f05431e85324356d991d21a`.
  Scenario/definition fingerprints are in the report and manifests.
- All three study paths skip every free reward. Their labels do not define
  ecological strength. Income offers are natural, never guaranteed or injected.

## Validation

- Portable Release build: zero warnings/errors; self-test passed.
- Fresh ordinary Unity reference, seed 10100: standalone verifier passed exact
  state/counters/choices/timing and existing derived-rate tolerance.
- Dedicated clean Unity EditMode parity: **1/1 passed, zero skips**, eight
  fixtures, at `artifacts/unity-tests-20261019-155907/`. Covers success/failure,
  population purchases/rejections, signed species effects, shop income,
  random/chosen reorder and all-six-owned round-7 mutation rewards.
- Repeat replay identical except measured runtime; all fixture hashes checked.
- **600/600 completed and validated**, each seed 10100–10299 once per policy.
  Source/scenario/definition identity, frozen hashes, payout counts, action
  charges, terminal state and complete wallet arithmetic passed.
- Remove Hare: 58 survive / 142 fail; Add Hare: 1 / 199; save for ×2: 52 / 148.
  First payout affords 10 FD in 180/200 seeds. All 58 removal survivors and
  51/52 saving survivors finish with one Hare. ×2 appeared in 28 of 78 reached
  second shops and was bought in 25. See report for matched comparisons.
- Cohort wall time: 181.668164 seconds, eight workers. No human decision times
  collected. Historical manifest/summary anchor hashes remained unchanged.

## Risks and incomplete work

- Later population collapse remains a design/balance issue. This comparison
  neither identifies its ecological cause nor approves tuning.
- T13 human acceptance and T16 recorded team accept/revise remain open. Bevin's
  informal play feedback remains distinct from the formal decision record.
- No new graphics, broad PlayMode suite or player build was required/run for
  this evidence-only refresh; previous rendered evidence retains its dates.
- Work remains local/uncommitted/unpushed. Trello statuses/comments were not
  changed; unrelated dirty files were preserved.

## Next useful step

Review these matched outcomes alongside direct play, especially one-Hare
finishes and repeated-addition failures, then record accept/revise and any
bounded next investigation. This does not authorize tuning or new content.
