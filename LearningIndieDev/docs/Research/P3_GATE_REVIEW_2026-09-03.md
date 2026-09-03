# P3 gate review — Bound AI discovery

**Date:** 2026-09-03 03:35
**Decision:** Not passed; carry the phase forward as an evidence-gated continuation.
**Decision owner:** Josh + Sim

## Gate being reviewed

The research plan defines P3's exit gate as EX-003/EX-007 meeting held-out
prediction and false-cause requirements. P3 is not complete merely because the
prediction contract or tooling exists; the training runs, held-out runs,
prediction scoring, and human decision must be recorded.

## Evidence state

- EX-001 and EX-001B have accepted bounded reproducibility results.
- EX-002 has a bounded BaselineParity intervention result with its stated
  interpretation limits.
- EX-003 has no execution package in the repository; its validity measures are
  therefore untested.
- EX-007 has a prepared contract and templates, but `REPORT.md`,
  `AI_ANALYSIS.md`, and `HUMAN_DECISION.md` remain pending.
- The evidence-quality tooling is implemented in commits `f315d3d6`, `03011357`,
  and `022ea769`. New bundles must contain `report.json`, `report.csv`,
  `statline.csv`, `manifest.json`, and `unity.log`, and must pass the strict
  artifact validator.

## Queue correction

The first two jobs submitted during the packaging check are retained as
diagnostics, not P3 evidence: they omitted `ScenarioPath` and used the wrapper's
legacy combat default. They must not be described as Forest Edge or EX-007
runs.

Corrected EX-007 baseline jobs are now queued with `ForestEdge.asset`, Hare,
`opposed-roll`, natural attack opportunity, `bev-experimental`, and no upgrade:

- Training seeds `1-20`: `20260903-033218-3b7607ba`
- Held-out seeds `101-105`: `20260903-033240-b1b43c58`

No worker pickup or completed bundle exists yet, so no P3 result can be scored.

## Carry-forward plan

1. Process and strictly validate the two corrected baseline bundles.
2. Generate the bounded AI input and pre-register the prediction before showing
   the analyst any intervention result.
3. Run S1 (`faster-movement`) and J1
   (`faster-movement,crowding-tolerance`) on training and held-out seeds.
4. Produce the factual report, score direction/effect-band accuracy, false
   causes, missed effects, and confidence calibration, then record the human
   decision.

Until those steps are complete, P3 remains a protocol plus queued work, not a
validated AI-discovery capability. P4 design translation may be prepared as
non-promotional planning, but no upgrade, balance, or player-facing rule should
be promoted from this phase.
