# Predictive AI research treatment — second-pass evidence review

**Audience:** Josh, Sim, and contributors working on the ecology laboratory
**Date:** 2026-09-03
**Analysis ID:** `DRT-P3-AI-ANALYSIS-V2`
**Status:** Needs Review
**Model:** `gpt-5.6-sol`, extra-high reasoning
**Method:** `deep-research-work:deep-research` v0.1.14 plus project
`correctness-first-engineering`
**Supersedes:** [AI analysis v1](ai-analysis-v1.md)
**Scope:** Current P3 predictive change-impact work, especially EX-007 through
EX-009
**Boundary:** Additive review only; canonical plans, code, artifacts, and human
decisions are not changed

**Evidence confidence:** High for repository observations and arithmetic;
medium-high for prioritization recommendations. Human acceptance remains
pending.

## Direct answer

This project has a credible early research harness, but it has not yet proven a
general predictive-AI capability. The strongest parts are deterministic seeded
runs, paired interventions, immutable evidence, independent validators,
fingerprints, and a human-owned promotion gate.

The second pass changes the immediate priority. The next major gain is not
another model or a larger report, and it is not finishing EX-009 as a full
ecology experiment. It is making four boundaries mechanically trustworthy:

1. **Input integrity:** the AI receives values generated from source artifacts,
   not hand-copied summaries.
2. **Information isolation:** the exact prompt, model, files, hashes, and task
   context are retained so “the AI did not see the answer” is auditable.
3. **Validation independence:** once a held-out panel has been viewed, it is
   retired from blind-confirmation duty.
4. **Forecast scoring:** every confidence value refers to one defined event;
   correlated outcomes are not counted as independent calibration trials.

The present order question should be protected by a small code-level invariant
test. The two upgrades change separate fields, both orderings produce the same
ruleset, and their existing matched runs are identical. A full order experiment
becomes valuable only if upgrade application later gains state, caps,
multipliers, intermediate effects, or another order-sensitive mechanism.

## What the first pass got right

- The EX-007 bounded loop is a real prediction exercise rather than a demo
  assembled entirely after seeing the results.
- The incorrect baseline `RFS`, ambiguous `PREY`, stale status page, bundle
  mismatch, and incomplete provenance are material evidence-hygiene problems.
- Interaction needs B/A/C/AC on the same seeds, with a pre-defined interaction
  contrast.
- Confidence should accumulate in a registry rather than being treated as
  persuasive prose.
- Autonomous experiment selection, surrogate modeling, and a dashboard remain
  premature.

## What the second pass corrected

### EX-009 is mostly an implementation question

`SpeciesUpgrade.Apply` adds Faster Movement only to `MovementSpeed` and
Crowding Tolerance only to `CrowdingTolerance`. `ApplyLoadout` applies every
upgrade before simulation and passes only the resulting data into the run. For
the existing 20-seed reports:

| Evidence | Forward order | Reverse order | Result |
|---|---|---|---|
| Loadout | faster, crowding | crowding, faster | Deliberately different |
| Ruleset fingerprint | `18190dd0…819acb` | `18190dd0…819acb` | Identical |
| Run provenance fingerprint | `00ca8195…0ab2e` | `a350cec3…ef69` | Different, as intended |
| 20 normalized run objects | 20 | 20 | Identical for every seed |

The provenance fingerprint includes ordered loadout, so its difference records
how the same final ruleset was requested. It does not show a simulation effect.

The proportionate closure is a unit test asserting that the operations commute
and yield identical final rules. One same-seed end-to-end smoke pair can verify
the command path. Treating the blocked five-seed run as a major P3 research
blocker spends experiment effort on a fact already implied by the current code
and corroborated by 20 deterministic runs.

This conclusion is deliberately narrow. It does not say every upgrade commutes
or that these upgrades will commute after future implementation changes.

### “Training” is the wrong term for seeds 1–20

The canonical plan says “training” is appropriate only after a separate
model-training design has been approved. EX-007 nevertheless calls seeds 1–20
the training panel. No model was fitted to those seeds. Future material should
use **development** or **calibration**. Historical files remain evidence and
should receive a terminology note rather than silent rewriting.

### The held-out panels have now been consumed

Seeds 101–105 and 106–110 were legitimate held-out evidence for their original
questions. Their results have since been inspected, discussed, and used to
shape follow-ups. That makes them development evidence for future adaptive
work, not fresh confirmation.

This is the ordinary adaptivity problem: repeatedly choosing new analyses after
seeing the same holdout can overfit decisions to that holdout even when no
model weights are trained. Dwork et al. formalize why adaptive reuse needs
special controls rather than ordinary holdout reasoning
([Science paper via PubMed](https://pubmed.ncbi.nlm.nih.gov/26250683/),
[technical treatment](https://arxiv.org/abs/1411.2664)).

The project does not need a sophisticated reusable-holdout algorithm now. It
needs a simple prospective rule:

- development panels may be reused freely and labelled as such;
- validation panels have limited, logged disclosures; and
- blind-promotion panels are opened once, after the contract and prediction
  are frozen, then retired.

### The confidence check does not measure calibration

EX-007 assigns one confidence value to S1 and one to J1. The analysis then
counts direction correctness for four outcomes across development and held-out
panels, producing eight checks per arm.

That is useful descriptive bookkeeping, but it is not a valid calibration
sample:

- the probability was not attached to any one of those eight events;
- metrics from the same run are correlated;
- development and held-out evaluations resolve the same forecast, rather than
  creating independent forecasts; and
- one arm-level confidence cannot be retrospectively split into eight
  probability claims.

Proper scoring rules require a probabilistic forecast and a defined event or
quantity that later materializes. They are designed to reward honest
probability assessment rather than rhetorical certainty
([Gneiting and Raftery, 2007](https://doi.org/10.1198/016214506000001437),
[author-hosted PDF](https://sites.stat.washington.edu/people/raftery/Research/PDF/Gneiting2007jasa.pdf)).

For the next experiment, either register a probability for each metric event,
such as `P(mean held-out FPO delta >= +3)`, or define one composite success
event before execution. Score each event once. A Brier loss is sufficient for
binary events; calibration plots wait until the registry contains enough
independent events.

### The AI isolation boundary is asserted, not reproducible

The prediction says intervention reports were unavailable, but it lacks the
model ID, exact prompt, ordered context manifest and hashes, invocation ID, and
evidence that the run occurred in a fresh context. The responsible conclusion
is **unknown**, not “contaminated.” Nothing in the repository proves leakage;
nothing lets a later reviewer reproduce the information boundary either.

A sealed prediction task should contain only the allowed baseline, contract,
parameter range, telemetry dictionary, and output schema. The resulting record
should hash every input and the final prediction.

## Evidence-backed findings retained from the first pass

### Baseline input integrity is already broken once

The bounded input reports baseline mean `RFS` as **0.63**. Recomputing its
referenced `report.json` gives **0.400402** across 20 seeds. `APS` independently
recomputes as **0.906114**. The historical input and prediction should remain
unchanged, but all future summaries should be generated and checked from the
source report.

This is not proof that the AI answer was bad. It is proof that the supposedly
bounded evidence package was not internally exact.

### `PREY` exposed a missing metric dictionary

The prediction interpreted `PREY` as a resource or food event. In the current
StatLine it means Hare deaths caused by carnivores. The post-run erratum is
honest and the metric was excluded from causal scoring, but the same class of
mistake will recur until metric ID, label, unit, source, aggregation, direction,
validity, and limitations travel with every prediction.

### Direct and worker runs have different completion contracts

The expected evidence bundle contains `report.json`, `report.csv`,
`statline.csv`, `manifest.json`, and `unity.log`. The direct runner path does not
automatically produce `statline.csv`; the worker packaging path does. The fix is
one shared exporter and validator path, not another reporting system.

### Provenance is good but incomplete

The current manifest records source commit, dirty state, scenario path/GUID,
Unity executable, and command arguments. It does not normalize branch, Unity
version, package/project fingerprint, wrapper revision, host, or prediction
provenance. These gaps do not erase the evidence, but they weaken future
comparisons when tools and branches change.

### Current status and formal gates disagree

The dated P3 gate snapshot says EX-007 is contract-only, while newer records
show completed EX-007 evidence, EX-008 follow-up, and blocked EX-009. The EX-007
human decision remains pending. The phase table also names EX-003 in the formal
exit gate, but no EX-003 package exists. A new status page should supersede the
old snapshot without rewriting it, and a human should either execute EX-003 or
formally revise the gate.

## How to interpret the sample sizes

Same-seed comparison is the right default because it preserves paired
counterfactual structure. In simulation literature this is a common-random-
numbers design. It often reduces noise in differences, but it is not magic:
depending on how random streams and system responses interact, it can fail to
reduce variance or even increase it
([Wright and Ramsay, 1979](https://doi.org/10.1287/mnsc.25.7.649)). The project
should therefore retain exact paired deltas and check the observed pairing
behavior rather than assuming a benefit.

Five held-out seeds are valuable for a transfer smoke test, especially when
all exact values are shown. They have limited inferential resolution. Under an
exact paired sign test, signs follow a binomial distribution with probability
0.5 under the no-direction null
([NIST sign-test reference](https://www.itl.nist.gov/div898/software/dataplot/refman1/auxillar/signtest.htm)).
With five non-tied pairs, even 5/5 in one direction gives a smallest possible
two-sided probability of `2 × (1/2)^5 = 0.0625`.

That calculation is not a demand to turn P3 into a p-value exercise. It is a
plain demonstration that five seeds cannot, by sign consistency alone, support
a strong conventional two-sided claim. Use them to catch reversal and inspect
mechanisms; use a larger fresh panel or second scenario when promotion depends
on stable effect size.

## Interaction design

The simultaneous J1 arm is not by itself an interaction test. A clean design
needs the same seeds for:

```text
B   baseline
A   faster movement only
C   crowding tolerance only
AC  both upgrades
```

and a pre-defined contrast:

```text
interaction = outcome(AC) - outcome(A) - outcome(C) + outcome(B)
```

NIST guidance emphasizes matching experimental design to the question and
using factorial designs when estimating interactions between a small number of
factors
([design selection](https://www.itl.nist.gov/div898/handbook/pri/section3/pri33.htm),
[interaction effects](https://itl.nist.gov/div898/handbook/pri/section5/pri594.htm)).

The contract should name one primary endpoint. A small secondary family may
test anticipated side effects. Everything else remains exploratory telemetry.
This keeps the rich StatLine useful without letting a post-run search select
whichever outcome tells the best story.

## Recommended treatment

### Priority 0 — before another confirmatory forecast

1. Generate and validate the bounded baseline summary.
2. Version and hash the metric dictionary.
3. Seal the AI task and retain complete model/prompt/context provenance.
4. Define probability-bearing forecast events and their scoring rules.
5. Register seed roles; mark 101–110 disclosed and unavailable for future
   blind promotion.
6. Complete the EX-007 human decision.

### Priority 1 — make the evidence portable

1. Unify direct and worker bundle creation.
2. Complete run provenance separately from prediction provenance.
3. Emit machine-readable paired deltas and event-level scores.
4. Add primary/secondary/exploratory endpoint labels.
5. Publish one current P3 status page and decide the EX-003 gate explicitly.

### Priority 2 — answer the next product question

1. Replace EX-009's full research priority with a commutativity unit test and
   optional end-to-end smoke pair.
2. If interaction matters to a design decision, run one clean B/A/C/AC package
   on development seeds, freeze the interpretation, then open a fresh panel.
3. Add spatial or time-window telemetry only for a named hypothesis that
   aggregate StatLines cannot answer.

## Proposed promotion gate

A future predictive claim is eligible for human review only when:

- the bounded input is generated and agrees with its source artifact;
- the metric dictionary and context manifest are hashed;
- the forecast runs in a fresh sealed context and names resolvable events;
- the contract distinguishes primary, secondary, and exploratory outcomes;
- paired runs produce identical required bundle shapes and pass validators;
- validation uses a panel not disclosed during hypothesis formation;
- exact seed-level effects and uncertainty limits are reported;
- misses, false causes, abstentions, and review time are retained; and
- a human records an explicit decision and scope.

That would demonstrate a disciplined bounded prediction loop. It would still
not, by itself, prove general prediction across species, scenarios, parameter
ranges, or future ruleset versions.

## Deferred on purpose

Conformal prediction can provide coverage guarantees under a suitable
calibration set and exchangeability assumptions
([Angelopoulos and Bates](https://arxiv.org/abs/2107.07511)). It remains a
reasonable later option for effect bands, but the current tiny, consumed panels
and incomplete context provenance do not support it yet.

Likewise, autonomous experiment selection, learned calibration, Morris
screening, surrogate models, and a generalized dashboard should wait until the
simple forecast registry contains enough clean, independently resolved events
to reveal an actual need.

## Claim-to-source ledger

| Claim | Source | Application and limit |
|---|---|---|
| Adaptive reuse can erode ordinary holdout validity. | [Dwork et al., *The Reusable Holdout*](https://pubmed.ncbi.nlm.nih.gov/26250683/); [technical paper](https://arxiv.org/abs/1411.2664) | Supports retiring disclosed panels prospectively. It does not invalidate their original use. |
| Probabilistic confidence needs a defined outcome and proper score. | [Gneiting and Raftery, 2007](https://doi.org/10.1198/016214506000001437) | Supports event-level probability records and Brier-style scoring. It does not determine the project's success threshold. |
| A paired sign test is discrete/binomial. | [NIST sign-test reference](https://www.itl.nist.gov/div898/software/dataplot/refman1/auxillar/signtest.htm) | Supports the exact `0.0625` resolution calculation for five non-tied pairs. It does not require NHST as the project decision rule. |
| Common random numbers often help paired comparisons but are not guaranteed to. | [Wright and Ramsay, 1979](https://doi.org/10.1287/mnsc.25.7.649) | Supports preserving paired seeds and empirically checking variance behavior. |
| Interaction estimates require a design that can separate factors. | [NIST design selection](https://www.itl.nist.gov/div898/handbook/pri/section3/pri33.htm); [NIST interactions](https://itl.nist.gov/div898/handbook/pri/section5/pri594.htm) | Supports B/A/C/AC for the current two-factor question. |
| Preregistration distinguishes planned confirmation from later exploration. | [OSF guidance](https://help.osf.io/article/626-simplifying-the-preregistration-process) | Supports visible confirmatory/exploratory status and frozen decision rules. |
| Project-specific facts and constraints. | [Canonical research plan](../Research/AI_ASSISTED_ECOLOGY_LAB_RESEARCH_PLAN.md); [EX-007 report](../Research/Experiments/EX-007-Predictive-Statline-Interventions/REPORT.md); [EX-009 package](../Research/Experiments/EX-009-Same-Heldout-Order-Comparison/README.md) | Establish current terminology, evidence, gates, and experiment state. |

## Uncertainties and stopping rationale

- The audit can show missing isolation provenance; it cannot determine what the
  original AI session actually saw.
- The code and artifacts establish commutativity for these two upgrades under
  the current path. They do not prove future or universal order independence.
- No power calculation is appropriate without a declared primary outcome,
  practical threshold, and variance estimate. The proposed protocol creates
  those prerequisites first.
- No claim is made that Brier score alone solves calibration; it is the smallest
  auditable starting point for defined binary events.

The review stopped when repository evidence and primary methodology sources
converged on the same bounded controls. Further literature would not change the
immediate implementation order. A new deep-research pass becomes worthwhile
after the project has a sealed forecast registry, a declared primary endpoint,
and several fresh independently resolved predictions.
