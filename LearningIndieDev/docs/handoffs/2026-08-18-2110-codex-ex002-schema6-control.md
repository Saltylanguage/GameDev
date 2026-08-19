# EX-002 schema-6 BaselineParity control

[Working state](../WORKING_STATE.md) | Status: control complete; intervention
matrix remains open

- Owner: codex
- Branch: `codex/cellular-sprite-tiling`
- Control range: BaselineParity, `herbivore`, seeds `10100`–`10119`
- Date: 2026-08-18

## Result

The schema-6 control was run twice:

- `artifacts/cellular-experiment-20260818-210354/report.json`
- `artifacts/cellular-experiment-20260818-210443/report.json`

Both reports have schema 6, the same ruleset fingerprint, identical run
histories, identical death events, and identical summaries. All 20 runs contain
death events. Creature death-event counts reconcile with aggregate activity
for every species/run; resource events remain separately represented.

Final herbivore population is minimum 0, maximum 37, average 10.25, with one
extinct run. The herbivore reproduction funnel averages 1290.45 candidates per
run: 612.6 energy blocks, 375.05 mate blocks, 64.95 group-limit blocks, 227.75
chance failures, 0 no-location blocks, and 10.1 successful attempts. No
intervention or balance value was changed.

## Interpretation boundary

This establishes the deterministic schema-6 control and the report/event
contract. It does not establish that starvation is the root cause, that the
rules are unbalanced, or that any intervention improves outcomes.

## Next actions

1. Predeclare the collapse endpoint and approved intervention surface.
2. Run the matched-seed intervention matrix.
3. Run the held-out seed check.
4. Write the causal analysis only after those comparisons are complete.
