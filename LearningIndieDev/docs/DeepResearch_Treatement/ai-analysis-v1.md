# Predictive AI research treatment — evidence review and improvement plan

**Analysis ID:** `DRT-P3-AI-ANALYSIS-V1`
**Status:** Superseded by [v2](report-source.md)
**Model:** Codex; exact deployment identifier was not recorded in the first pass
**Date:** 2026-09-03
**Method:** `deep-research-work:deep-research` v0.1.14
**Preservation note:** This is the original first-pass interpretation restored
after the second pass. Its EX-009 priority is intentionally retained so the
reasoning change remains reviewable.
**Audience:** Josh, Sim, and contributors working on the ecology laboratory
**Scope:** The current predictive-AI research program, especially P3, EX-007,
EX-008, and the blocked EX-009 continuation
**Assumptions:** The checked-out `UI/ControlLibrary` branch and the repository
records available on this date are the source of truth. Generated artifacts are
evidence, not editable design intent.

## Direct answer

The project is conceptually ahead of its measurement discipline. The core idea
is sound and the EX-007 loop has now demonstrated a real bounded prediction
exercise. The main risk is not lack of sophistication; it is allowing small
provenance, metric-definition, and scoring gaps to make the evidence look more
certain or more comparable than it is.

The best improvement is a correctness layer around the existing runner:

1. freeze a metric dictionary and validate the bounded input against it;
2. make every local and worker bundle identical and provenance-complete;
3. score per-seed distributions and effect bands, not only mean direction;
4. use a same-seed factorial design for interaction claims; and
5. treat confidence as a calibration record accumulated over many predictions,
   not as a persuasive number attached to one answer.

No new model-training framework is justified yet.

## What is already strong

- The canonical plan separates **Experiment → Run → Report → Analysis → Human
  Decision** and prohibits silent post-run rewriting.
- EX-007 has a bounded input, a retained pre-registration, 20 training seeds,
  five original held-out seeds, independent StatLine validation, and a
  plain-language report.
- EX-008 correctly added the missing crowding-only arm and documented that its
  different held-out seeds cannot establish an order effect.
- EX-009 is a good correction: it locks both orderings to the same held-out
  seeds and refuses to bypass Unity preflight. Its current forward arm is
  blocked before simulation rather than being misreported as evidence.
- The runner emits ruleset and run-provenance fingerprints. The bundle validator
  and independent StatLine validator make boring failures visible.
- The research plan keeps ecological evidence separate from human judgments
  about balance, fun, quality, or promotion. That boundary should remain.

These are meaningful foundations. The treatment below tightens them without
expanding the project into an autonomous research platform.

## Evidence-backed findings

### 1. There is a concrete input-integrity error

The pre-registered AI input reports baseline mean `RFS` as **0.63** in
`EX-007.../AI_INPUT-EXP-007-0001.md:22`. Recomputing that value from the
validated baseline report gives **0.400402** across the 20 StatLines. `APS` is
also independently recomputable as **0.906114**. The prediction JSON is kept
unchanged for research integrity, but future bounded inputs must be generated
from the report rather than typed by hand.

This is exactly the kind of small discrepancy that can make an otherwise
careful prediction untrustworthy. It is a data-integrity defect, not evidence
that the model or the prediction was bad.

### 2. The evidence state is split across current and stale records

`docs/Research/P3_GATE_REVIEW_2026-09-03.md` still describes EX-007 as
contract-only and says no completed bundle exists. The canonical research plan
now records EX-007 as complete through analysis, EX-008 as a follow-up, and
EX-009 as the blocked same-held-out comparison. The old gate review is a useful
historical snapshot, but it is unsafe as a current status page.

The EX-007 README also has a completed status while retaining the boilerplate
sentence “Until steps 3-6 are complete…” at the end. Contributors can resolve
the contradiction only by reading several files. A current status index would
remove that ambiguity without altering historical records.

### 3. The artifact contract is stronger than the direct local command

The research contract requires `report.json`, `report.csv`, `statline.csv`,
`manifest.json`, and `unity.log`. `Run-CellularExperiment.ps1` directly creates
the first, second, fourth, and fifth files; `Start-CellSimWorker.ps1` adds
`statline.csv` while packaging worker results. Direct local runs therefore need
an extra derived-export step and can look complete before they meet the full
contract.

The fix should be one shared StatLine-export path, not a new reporting system.

### 4. Provenance is sufficient for a replay seed, not yet for a research claim

The current manifest records source commit, dirty state, scenario path/GUID,
Unity executable, and command arguments. It does not record the branch, Unity
editor version as a normalized field, package-lock or package state, host
identity, wrapper/tool revision, or AI model/prompt/context provenance.

The plan explicitly requires build, branch, commit, tool versions, environment,
and analysis provenance. A dirty-tree flag is an important warning, but it is
not a substitute for those fields.

### 5. EX-007 is a useful pilot, not a calibrated predictor

The analysis reports 5/8 and 7/8 direction checks and calls them early
calibration evidence. That wording is appropriately cautious, but there is no
registry or scoring tool that aggregates predictions over time. One experiment
cannot establish that 56% or 62% confidence means what it says.

Calibration research shows that confidence estimates can be systematically
misaligned with correctness and should be evaluated against observed outcomes;
temperature scaling is one practical post-processing method for some model
classes ([Guo et al., 2017](https://proceedings.mlr.press/v70/guo17a.html)). For
this project, the immediate requirement is simpler: retain each prediction,
score it after held-out validation, and delay any calibration claim until there
are enough independent predictions to inspect reliability.

### 6. Interaction and order questions need different designs

EX-007's simultaneous joint arm was not a complete two-factor interaction
design. EX-008 improved interpretation by adding crowding tolerance alone, but
its held-out seeds differed. EX-009 correctly locks both orderings to seeds
106–110, but the forward arm is blocked before execution.

NIST's experimental-design guidance distinguishes comparison, screening, and
response-surface objectives and recommends full factorial designs when a small
number of factors and their interactions are the question
([NIST design selection](https://www.itl.nist.gov/div898/handbook/pri/section3/pri33.htm)).
Its interaction guidance also warns that the ability to estimate an interaction
depends on the design and run count, not merely on the fact that two factors
were changed ([NIST interaction effects](https://itl.nist.gov/div898/handbook/pri/section5/pri594.htm)).

The practical rule here is: use B, A, C, and AC on the same seeds for an
interaction claim; use AB versus BA on the same seeds for an order claim. Do
not substitute different-seed averages for either.

### 7. Pre-registration and exploratory follow-ups are being mixed clearly

EX-007's prediction was recorded before its intervention results. EX-008 was
properly labelled an exploratory follow-up after the fact, but the distinction
is easy to miss when reports are read out of order. OSF describes
preregistration as recording the questions, methods, data, analysis, and
evidence rules before collection so the resulting conclusions can be compared
with the original plan
([OSF guidance](https://help.osf.io/article/626-simplifying-the-preregistration-process)).

The project should keep a visible `confirmatory` or `exploratory` field on every
experiment and every derived analysis. That preserves useful follow-ups without
pretending they were pre-registered.

## Recommended treatment

### Priority 0 — before another predictive claim

1. Correct the RFS value in any future regenerated bounded input and add an
   automated baseline-summary check. Do not edit the historical prediction.
2. Add a stable metric dictionary with plain-language meaning, unit, source
   field, aggregation rule, valid statuses, and known limitations. `PREY` must
   remain explicitly “Hares killed by carnivores.”
3. Publish a new current P3 status note that supersedes the stale gate snapshot,
   while leaving the historical note untouched.
4. Complete the human decision records for EX-007 and EX-009. Until then,
   findings remain evidence, not accepted design guidance.
5. Finish EX-009 only after Unity preflight passes. Its same-seed comparison is
   the correct answer to the order question.

### Priority 1 — make the evidence repeatable by other developers

1. Move StatLine export into one shared command path so direct and worker runs
   produce the same required bundle.
2. Add normalized manifest fields: branch, Unity version, project/package
   fingerprint, wrapper revision, host, and explicit source-tree status.
3. Add a machine-readable prediction score containing per-seed deltas, mean,
   median, quantiles, range, direction hit, effect-band hit, and unresolved
   status. Keep the human-readable explanation separate.
4. Add a report-integrity check that compares every bounded-input summary value
   with the referenced baseline artifact before a prediction can be marked
   preregistered.
5. Track `confirmatory` versus `exploratory` at the package and analysis level.

### Priority 2 — improve statistical usefulness without overbuilding

1. For each metric, report mean, median, interquartile range, per-seed range,
   and the fraction of seeds moving in the predicted direction.
2. Use bootstrap intervals only when the sample size and independence justify
   them. For five held-out seeds, prefer the exact seed-level values and label
   uncertainty as unresolved rather than printing a fragile interval.
3. Add a small full-factorial contract for two upgrades. Define the interaction
   contrast before execution:

   `interaction = outcome(AC) - outcome(A) - outcome(C) + outcome(B)`

4. Maintain a prediction registry with model/prompt provenance and enough
   completed cases to inspect reliability, Brier-style probability error, and
   selective abstention. Do not introduce a learned calibration model yet.
5. Add spatial or time-window metrics only when a named question needs them;
   final population alone can hide displacement, confinement, or late collapse.

### Deferred on purpose

Conformal prediction can provide distribution-free prediction sets with
explicit coverage guarantees when a suitable calibration set and
exchangeability story exist
([Angelopoulos and Bates, 2023](https://arxiv.org/abs/2107.07511)). It is a
promising later option for effect intervals or risk bands, but the current
five-seed held-out panels and unresolved provenance are not enough to justify
adding it now.

Likewise, Morris screening, surrogate models, autonomous experiment selection,
and a generalized dashboard should wait until the simpler paired evidence path
is complete and useful.

## Proposed success test for the next P3 increment

The next increment should be considered successful only if all of the following
are true:

- a fresh bounded input is generated and verified from the baseline artifact;
- the prediction records its model/prompt/context provenance and confirmatory
  status;
- local and worker runs produce identical required bundle shapes;
- the same-seed factorial or order comparison completes with valid provenance;
- the scorer reports distributional effects and explicit unresolved cases; and
- a human records a decision with scope, evidence references, and follow-up.

That is a stronger and more useful milestone than adding a larger model.

## Claim-to-source ledger

| Claim used in this treatment | Source | Why it applies |
|---|---|---|
| Confidence should be checked against observed correctness rather than trusted as a raw score. | [Guo et al., *On Calibration of Modern Neural Networks*](https://proceedings.mlr.press/v70/guo17a.html) | Primary calibration study; supports measuring reliability and treating confidence as fallible. |
| Experimental design should match the question; interactions require an appropriate design. | [NIST design selection](https://www.itl.nist.gov/div898/handbook/pri/section3/pri33.htm); [NIST interaction effects](https://itl.nist.gov/div898/handbook/pri/section5/pri594.htm) | First-party engineering-statistics guidance; supports factorial/order design rules. |
| Preregistration separates planned analysis from later conclusions. | [OSF, Simplifying the Preregistration Process](https://help.osf.io/article/626-simplifying-the-preregistration-process) | First-party guidance; supports explicit confirmatory/exploratory labels. |
| Prediction sets can provide explicit distribution-free coverage under stated conditions. | [Angelopoulos and Bates, *A Gentle Introduction to Conformal Prediction*](https://arxiv.org/abs/2107.07511) | Primary/authoritative technical tutorial; supports deferring conformal methods until calibration data and assumptions are adequate. |
| Existing project protocol, metric and gate observations. | [AI-Assisted Ecology Laboratory Research Plan](../Research/AI_ASSISTED_ECOLOGY_LAB_RESEARCH_PLAN.md); [EX-007 report](../Research/Experiments/EX-007-Predictive-Statline-Interventions/REPORT.md); [EX-009 package](../Research/Experiments/EX-009-Same-Heldout-Order-Comparison/README.md) | Repository sources; establish the current contract and evidence state. |

## Stopping rationale

The review stopped after the project documents, current artifacts, runner/
worker packaging, and a focused set of primary methodology sources converged on
the same small set of improvements. More literature would repeat the same
recommendations without changing the immediate priorities. A later research
pass is justified when the project has a larger prediction registry, a clean
EX-009 result, or a concrete need for range uncertainty and surrogate models.
