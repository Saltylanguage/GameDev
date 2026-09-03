# DeepResearch Treatment — Predictive AI research

**Date:** 2026-09-03
**Scope:** Current AI-Assisted Ecology Laboratory / P3 predictive change-impact work
**Mode:** Non-destructive review and improvement treatment, second pass

This directory is an additive treatment package. It does not replace the
canonical research plan, rewrite experiment evidence, or promote a design
decision. It records what is working, what currently weakens the evidence, and
the smallest practical changes that would make the next P3 experiments more
reliable.

## Start here

1. [Full research report, v2](report-source.md) — the complete second-pass
   review,
   conclusions, sources, and limitations.
2. [Second-pass delta](SECOND_PASS_DELTA.md) — what the deeper review changed,
   including one first-pass priority that was wrong.
3. [Current-state audit](CURRENT_STATE_AUDIT.md) — evidence-backed findings
   ranked by urgency.
4. [Revised P3 protocol](REVISED_PROTOCOL.md) — a tighter execution and
   scoring loop for future predictions.
5. [Implementation backlog](IMPLEMENTATION_BACKLOG.md) — additive, bounded
   work items with acceptance criteria.

The original interpretation is preserved as
[AI analysis v1](ai-analysis-v1.md). It is superseded for prioritization, not
deleted or rewritten.

## Executive conclusion

The project already has an unusually strong evidence spine for an early game
research effort: deterministic seeded runs, ruleset fingerprints, machine-
readable reports, independent StatLine validation, held-out seeds, explicit
causal-status language, and a human promotion gate.

It is not yet a validated general predictive-AI capability. The next gains are
mostly in evidence hygiene and scoring rather than a larger AI model:

- correct and lock the bounded input data before every prediction;
- run each confirmatory prediction in a sealed, auditable AI context;
- retire a seed panel from blind-validation duty once its results are viewed;
- give every metric a stable definition and aggregation rule;
- make provenance and report packaging complete for local and worker runs;
- attach confidence to a defined forecast event and score it properly; and
- use a same-seed four-cell design before calling an interaction robust.

The second pass also changes one earlier recommendation. EX-009 should not be
treated as a major ecological experiment under the current implementation.
`faster-movement` and `crowding-tolerance` alter different rule fields, both
orders already produce the same final ruleset, and their 20 matched run objects
are identical. A focused commutativity unit test plus an optional end-to-end
smoke test is the proportionate way to protect that invariant.

## Non-destructive boundary

No existing report, experiment package, source file, scene, serialized asset, or
decision record is modified by this treatment. Recommendations are deliberately
written as reviewable work items. A human owner should accept each change before
it is applied to the canonical plan or tooling.
