---
name: artifact-summarization
description: "Compress large Unity CellSim JSON reports into dense Markdown and machine-readable summaries that preserve balance, prediction, provenance, and phase evidence."
metadata:
  short-description: "Compress Unity reports into decision evidence"
---

# Artifact Summarization

Use this skill when a Unity CellSim `report.json` is too large to load directly
or when several experiment arms need a compact, comparable evidence packet.

Run one report at a time:

```powershell
python .agents/skills/artifact-summarization/scripts/summarize_cell_sim.py `
  --report artifacts/cellular-experiment-.../report.json
```

For a paired comparison, provide a same-seed baseline:

```powershell
python .agents/skills/artifact-summarization/scripts/summarize_cell_sim.py `
  --report artifacts/arm/report.json `
  --baseline artifacts/control/report.json
```

The V1 writes `report-summary.json` and `report-summary.md` beside the source
report unless `--output-directory` is supplied. The compact JSON keeps exact
experiment identity, catalog and snapshot provenance, per-seed outcomes,
phase windows, all activity/stat-line counters, population exposure summaries,
behavior-state totals, transition counts, death causes, combat aggregates,
opportunity controls, and a baseline delta section when requested. The Markdown
view is intentionally smaller: it uses cross-seed phase averages and points
back to the JSON for exact per-seed phase detail.

Raw event arrays are intentionally omitted after their decision-useful counts
and distributions are retained. The Markdown explains what was retained and
what was compressed, so a later model does not mistake a summary for a raw
replay. Never call a summary proof of causality by itself; keep the source
artifact and report limitations in the handoff. Legacy reports with only
trailing-comma JSON damage are recovered narrowly and marked with a parse
warning; those warnings require human review before raw deletion.

For Unity runtime/test evidence, build one compact diagnostics index without
touching the raw logs or NUnit XML:

```powershell
python .agents/skills/artifact-summarization/scripts/summarize_unity_diagnostics.py `
  --artifact-root LearningIndieDev/artifacts `
  --output LearningIndieDev/artifacts/unity-diagnostics-summary-v1.json
```

That index keeps per-file hashes, byte/line counts, Unity/version/date/command
identity, process-exit evidence, bounded redacted signal samples, memory-leak
totals, and failed test cases. It omits raw event order and full stack/log
replay. Treat it as a triage/context layer, not a replacement for raw evidence
or proof of causality; review it before archiving any log.
