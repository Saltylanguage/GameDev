# Standalone CellSim research evidence

Packaged October 8, 2026 by Codex / GPT-6 for Bevin's requested Git publication.
This shares completed analysis and immutable configurations without modifying
the active 96,000-run confirmation. Production balance decisions remain open.

| Study | Runs | Shared evidence |
|---|---:|---|
| First expanded grid | 32,400 | Completion review, all metric distributions and paired deltas |
| Quarter-million grid | 250,000 | Completion review, all-path comparisons with average APS/AHS, all metric distributions and paired deltas |
| Hare purchase screen | 12,800 | Purchase/path comparisons, average APS/AHS, spending and all 64,000 assigned-window diagnostics |
| Confirmation | 96,000 planned; still running when packaged | Frozen matrix/inputs/validation plus earlier-screen component diagnostics; no confirmation results |

Start with these readable reports:

- [First grid review](cellsim-large-sweep-20261008-012921/completion-review.md).
- [Quarter-million review](cellsim-250k-sweep-20261008-030316/completion-review.md).
- [All-path benefit tables with APS/AHS](cellsim-250k-sweep-20261008-030316/all-path-benefit/report-with-statlines.md).
- [Hare purchase focused review](cellsim-hare-purchase-20261008-132317/purchase-analysis/focused-review.md).
- [All Hare purchase tables](cellsim-hare-purchase-20261008-132317/purchase-analysis/report.md).
- [Confirmation testing matrix](cellsim-hare-confirmation-20261008-143725/testing-matrix.md).
- [Earlier-screen score decomposition](cellsim-hare-confirmation-20261008-143725/screen-diagnostics/report.md).

The three ZIPs contain each completed study's snapshot, scientific specification,
expanded plan/case matrix, original completion provenance, full metric/paired
CSV tables and derived analysis. The quarter-million archive is approximately
86 MB compressed; extract it to access the larger tables. Readable reports are
also copied outside the archives. Original relative links may refer to files
inside the corresponding ZIP or the original local artifact directory.

`manifest.json` records source paths, original run identities, preserved raw
hashes, archive SHA256 and per-entry hashes. Every copied file and extracted
archive entry was byte-checked; archive CRCs and analysis output hashes passed.
Raw hashes are recorded prior validated evidence, not a new full rehash of
all raw runs during packaging. Source commit/dirty-tree provenance is retained
verbatim and is not relabeled as the publication commit.

Validation files include the earlier 259-test EditMode XML, the purchase screen's
two-test XML (inside its ZIP), portable guard/recovery evidence and a fresh
sweep-analysis regression. A fresh isolated Release build and self-test passed
before publication; its DLL is byte-identical to the active confirmation's copy.
These checks do not claim a new Unity test run or full PlayMode validation.

Raw per-run/chunk output, duplicate SQLite tables, build binaries/caches, large
logs and active output remain local under `LearningIndieDev/artifacts/`.
The archived snapshots preserve historical source hashes; rebuilding current
sources does not automatically reproduce earlier pre-purchase snapshots.
Preserve original local frozen tools or use the matching source revision/export
when replaying historical evidence. A clone of these summaries alone cannot
regenerate omitted per-seed raw output or full event traces.
