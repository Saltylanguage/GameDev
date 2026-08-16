# predictive-ai-research-ex001b

[Working state](../WORKING_STATE.md) | Status: accepted bounded extension

- Owner: codex
- Branch: `Tooling/AI_Workflows`
- Date: 2026-08-15

## Summary

EX-001B extends the accepted ForestEdge reproducibility baseline across the
currently authored scenario library. ForestEdge, OpenRange, Wetland, and
BaselineParity each completed two current schema-4 20-seed runs with matching
normalized outcomes. The human design owner accepted the bounded reproducibility
result on 2026-08-15.

## Evidence

- Factual report: `RPT-RUN-001B-0001-0003.md`
- Analysis: `ANL-RPT-RUN-001B-0001-0003-v1.md`
- Raw reports: `artifacts/cellular-experiment-20260815-164921` through
  `artifacts/cellular-experiment-20260815-165312`
- Seed range: `10100`–`10119`
- Completed pairs: ForestEdge (`hare`, 32 x 32), OpenRange (`deer`, 32 x 20),
  Wetland (`snail`, 32 x 20), BaselineParity (`herbivore`, 32 x 20)

## Findings and limits

- All four completed scenario pairs matched complete run payloads and final
  summaries after generated metadata was excluded.
- This supports reproducibility across the four tested authored scenarios only.
- It does not prove that every cellular automaton is deterministic or that
  ecological findings transfer between scenarios.
- An invalid OpenRange player species was rejected before simulation and was
  corrected to the authored `deer` species.
- The first Wetland batch wrote a complete matching report despite a nonzero
  wrapper status; the environment/license anomaly is preserved in the report.

## Relay handling

The one-shot `CellSim Run` wrapper starts a Unity batch process per invocation.
The final BaselineParity repetition ran sequentially after confirming no Unity
process was open; the process exited and was verified absent. Do not launch
parallel relays. Prefer an existing open session when the tooling supports it,
then verify the process is closed.

## Next safe action

Proceed to EX-002 scoped to the accepted ForestEdge baseline. Preserve the
EX-001B scope boundary: this is reproducibility evidence for four authored
scenarios, not a universal cellular-automata or ecological-transfer claim.
