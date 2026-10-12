# Forest Edge confirmation review — October 12, 2026

Status: Needs Review. These are historical research findings, not approved
production tuning or player acceptance.

This folder preserves a compact copy of the read-only review of Bevin's five
cumulative Excel exports. The final export covers 8,000 runs, 80 conditions and
100 shared seed IDs per condition. Do not add the five workbook totals together.

- [Full analysis](ai-analysis-v1.md), including method, provenance, uncertainty,
  seed suggestions and recommendations.
- [Overview chart](confirmation-overview.png).
- [Verification record](verification.json).
- [All condition results](condition-results.csv).
- [Matched choice effects](choice-effects.csv).
- [Review handoff](../../../handoffs/2026-10-12-1609-codex-forest-edge-fresh-seed-confirmation-results-review.md).

The source workbooks, raw reports, frozen runners and preset pack remain in the
local ignored `artifacts/` folders referenced by the analysis. This compact
record does not replace the raw evidence or make those local files available
through Git. The copied report retains its original analysis-time scope and
timestamp; this preservation step does not rerun or modify the experiments.

## Integration boundary

These runs used source hash
`62d30f34dc7c3b531a710f26234b8091c4d0bfbc858b8dcb7adfb3eead7ea08d`,
before merging Salty's checkpoint `876f3cf7`. That checkpoint changes scheduled
Threat Avoidance behavior and the first Hare path offer, and adds spatial
telemetry. Retain the original frozen runner when inspecting these results.
Use a newly built runner and matched controls for claims about the merged code;
do not relabel these results as validation of the new behavior.
