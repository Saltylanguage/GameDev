# Experiment packages

The [AI-Assisted Ecology Laboratory Research Plan](../AI_ASSISTED_ECOLOGY_LAB_RESEARCH_PLAN.md)
is the canonical definition of this protocol. These packages are execution
records, not a second research plan.

Each package follows the studio evidence protocol:

```text
Experiment -> Run -> Report -> Analysis -> Human Decision
```

Packages contain the approved question and method, references to raw run
artifacts, a factual report, a separate AI analysis, and the decision state.
Generated JSON, CSV, logs, and visual captures remain under the ignored
`LearningIndieDev/artifacts/` directory. The package records their paths and
provenance so a reviewer can locate the evidence without copying large files
into the documentation tree.

Each completed experiment report begins with an at-a-glance section that says,
in plain language, what was tested, what happened, what the evidence supports,
and what remains unproven. If a package has no separate factual report, that
section appears in the package README. Failed execution attempts use the same
structure and must clearly state when no research conclusion is possible.

## Packages

**Applicability overlay:** [continued simulation impact](../../CONTINUOUS_SIMULATION_EVIDENCE_IMPACT.md)
records which existing claims do not transfer to phases in an evolved world.
It does not rewrite these packages' observations or decisions. EX-010 now
provides the first accepted continued-world comparison under its own contract;
its result remains bounded to that scenario, schedule and upgrade set. See the
[P3 cohesive report](P3-Predictive-AI-Cohesive-Report.md) and
[P3 gate decision](../P3_GATE_DECISION_2026-09-06.md) for the current synthesis.

| Package | Current disposition |
| --- | --- |
| [EX-001 - Reproducibility Baseline](EX-001-Reproducibility-Baseline/README.md) | Accepted current-code ForestEdge reproducibility baseline. |
| [EX-001B - Cross-Scenario Determinism](EX-001B-Cross-Scenario-Determinism/README.md) | Accepted bounded four-scenario reproducibility result. |
| [EX-002 - Herbivore Collapse Attribution](EX-002-Herbivore-Collapse-Attribution/README.md) | Bounded schema-6 matrix and held-out check complete. Two cited raw control bundles are absent from this checkout, so current raw re-audit is limited to the retained records. |
| [EX-007 - Predictive Statline Interventions](EX-007-Predictive-Statline-Interventions/README.md) | Accepted bounded model-scoped prediction result. |
| [EX-008 - Reversed Upgrade-Order Follow-up](EX-008-Reversed-Order-Followup/README.md) | Complete exploratory support; incorporated into the P3 synthesis. |
| [EX-009 - Same-Held-Out-Seed Upgrade-Order Comparison](EX-009-Same-Heldout-Order-Comparison/README.md) | Accepted bounded launch-time commutativity result. |
| [EX-010 - Sequential Upgrade Continuation](EX-010-Sequential-Upgrade-Continuation/README.md) | Accepted bounded continued-world timing/order result. |
| [EX-011 - Cross-Biome, Cross-Species Transfer](EX-011-Cross-Biome-Species-Transfer/README.md) | Bounded finding accepted: the tested Faster Movement → Crowding Tolerance combination increased final Deer population in Open Range/Deer; no individual-upgrade or production-balance claim. |

EX-003 is deferred without an execution package. EX-004 through EX-006 remain
portfolio proposals and do not have experiment packages yet.
