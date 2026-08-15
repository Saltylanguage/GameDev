# predictive-ai-research-ex001

[Working state](../WORKING_STATE.md) | Status: accepted baseline; follow-on research open

- Owner: codex
- Branch: Tooling/AI_Workflows
- Baseline commit: ff59da90
- Sharing commit: bce281e9
- Date: 2026-08-15

## Summary

The Predictive AI research project has started as a documented, human-governed
change-impact program. Its first instrument-trust experiment, EX-001, is
complete and accepted as a current-code reproducibility baseline. The live
ForestEdge source of truth is 32 x 32; the older 32 x 20 schema-2 artifact is
retained as archival evidence only.

## Changes

- Added the umbrella Predictive AI research paper, research-plan/source-reading
  support, report templates, and research index links.
- Completed EX-001 with two current schema-4 runs over seeds 10100–10119.
- Recorded matching ruleset fingerprint, normalized run payloads, population
  histories, final summaries, and replay evidence.
- Replayed representative seed 10102 and boundary seed 10116; each Play Mode
  evidence suite passed 4/4 and retained machine-readable results.
- Revised the EX-001 brief and package to use the authored 32 x 32 configuration.
- Added the accepted human decision `DEC-EXP-001-0002` and resolved the EX-001
  Loose Ends entry.

## Decisions and assumptions

- EX-001 accepts reproducibility only; it does not establish balance, causation,
  or authorize EX-002.
- Current evidence uses fingerprint
  `c794841a72be34c241c7f811b848fe540914e5a8a9f55f5ee6406ae44f093fc4`.
- The initial Unity batch startup failed because Unity could not access its
  per-user cache; the successful reruns required elevated process permissions.
- Generated JSON, CSV, logs, and visual evidence remain ignored local artifacts;
  the repository stores their paths and factual summaries.

## Validation

- Two schema-4 `CellSim Run` executions matched across metadata, all 20 run
  payloads, histories, and final summaries.
- `CellSim Report` and `CellSim Compare` completed for the paired reports.
- Replay seeds 10102 and 10116 each completed `4/4` Play Mode tests with matching
  source player populations.
- The 32 x 32 asset/brief/report/replay configuration was cross-checked.

## Risks and incomplete work

- The research paper is a draft; predictive accuracy, calibration, causal
  diagnosis, held-out range validation, and workflow-value measurement remain
  untested.
- The historical 32 x 20 report must not be mixed into current 32 x 32 baselines.
- Existing unrelated/uncommitted local work, including `.agents/`, remains
  outside this handoff unless deliberately staged.

## Next useful step

Design EX-002 as a single-variable, same-seed intervention study using the
accepted EX-001 schema-4 baseline and the authored 32 x 32 ForestEdge scenario.
