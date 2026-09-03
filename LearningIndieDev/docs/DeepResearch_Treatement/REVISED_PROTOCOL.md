# Proposed P3 protocol vNext

This is a review proposal, not an automatic change to the canonical research
plan. It preserves **Experiment → Run → Report → Analysis → Human Decision**
and adds the minimum controls needed for a trustworthy predictive claim.

## Stage 0 — integrity and seed preflight

Before accepting a prediction:

1. Confirm Unity preflight, source revision, scenario, species, modes, schema,
   observation window, and exact intervention values.
2. Generate the bounded baseline summary directly from its report and reject
   any checked-in value that disagrees.
3. Record branch, commit, dirty state, Unity version, package/project
   fingerprint, wrapper revision, host, command hash, and validator versions.
4. Register every seed panel with one role:
   - **Development:** reusable for exploration, diagnosis, and effect-band work.
   - **Validation:** limited-use transfer check; disclosure is recorded.
   - **Blind promotion:** one-time final check; results remain unavailable until
     the contract and prediction are sealed.
5. Reject overlap between a blind panel and any previously disclosed panel.

A failed preflight may produce a failure record, never an experiment result.

## Stage 1 — contract and forecast objects

The experiment record must state:

- question, hypothesis, and human decision owner;
- confirmatory or exploratory status;
- baseline, interventions, controls, permitted range, and observation window;
- one primary endpoint and its material threshold;
- a small secondary endpoint family;
- exploratory telemetry that cannot determine success;
- development, validation, and optional blind-promotion panels;
- success, failure, abstention, and missing-data rules; and
- the exact evidence bundle the AI may receive.

Each confidence value must belong to a defined forecast event. Valid forms are:

```text
Metric event:
  P(FPO delta >= +3 Hares on blind panel) = 0.62

Composite event:
  P(primary succeeds AND no registered harm threshold is crossed) = 0.58
```

Do not assign one confidence value to an arm and later compare it with an
arbitrary number of correlated metric/panel checks.

If the feature or outcome is not represented and instrumented, return **Not
currently testable**. If the design cannot distinguish plausible outcomes,
return **Underdetermined**. These are successful safety outcomes, not failures.

## Stage 2 — sealed AI prediction

Confirmatory predictions should run in a fresh task or equivalent isolated
context containing only the approved bundle. Retain:

```text
model ID and reasoning setting
exact system/task prompt or prompt hash
ordered context-file manifest and SHA-256 hashes
invocation ID and timestamp
prediction JSON and SHA-256 hash
explicit statement that intervention results were not in context
```

This makes the information boundary auditable. A prose statement alone records
intent but cannot prove what context was available.

## Stage 3 — execution and packaging

Run the approved arms without changing the contract. Compare interventions
with the same seeds and stochastic configuration. This is a common-random-
numbers design: it usually makes paired differences more informative, but the
per-seed pairing must be preserved in analysis and does not guarantee variance
reduction in every simulation.

Every arm must produce the same bundle:

```text
report.json
report.csv
statline.csv
manifest.json
unity.log
```

The local and worker commands must call the same exporter and validators. A
bundle is not analysis-ready until both validators pass.

## Stage 4 — normalization and scoring

For every paired seed and metric:

```text
d_i = intervention_i - baseline_i
```

Report the exact deltas plus mean, median, interquartile range, minimum,
maximum, and sign-agreement fraction. Keep development and validation results
separate.

Score each pre-defined forecast event separately:

- direction hit;
- material-threshold hit;
- effect-band coverage;
- registered harm-threshold breach;
- correct abstention or unresolved call; and
- probability score, when a probability was registered.

For a binary event with forecast probability `p` and outcome `y` in `{0,1}`,
store Brier loss `(p - y)^2`. Do not claim calibration from one trial, and do
not count correlated metrics or the same prediction on development and
validation panels as independent calibration cases.

### Five-seed interpretation

A five-seed panel is a **transfer smoke test**. Show every paired result and
avoid precise generalization language. Even five same-sign non-tied effects
have a minimum exact two-sided sign-test probability of `0.0625`; therefore
sign consistency alone cannot satisfy a conventional two-sided 5% rule.

This is a resolution warning, not a requirement to use null-hypothesis testing.
Promotion may instead use a pre-registered practical threshold, but a strong
claim still needs a larger fresh panel or a second scenario.

## Stage 5 — causal, interaction, and order analysis

Use the weakest language supported by the design:

```text
Observed association
    -> Mechanistically consistent
    -> Causal evidence supported within model scope
    -> Robust causal relationship within validated range
```

For a two-factor interaction, run all four cells on the same seeds:

```text
B   baseline
A   factor A only
C   factor C only
AC  both factors
```

Predefine:

```text
interaction = outcome(AC) - outcome(A) - outcome(C) + outcome(B)
```

### Order questions start in code

Before spending simulation budget on `A→C` versus `C→A`:

1. inspect whether the operations touch separate fields or share state;
2. test whether both orders produce the same final ruleset fingerprint/data;
3. add a commutativity unit test for operations expected to commute; and
4. use one same-seed runtime smoke test only to verify end-to-end wiring.

Use a full runtime order experiment when application has state, caps,
multipliers, unlocks, intermediate side effects, or another mechanism that can
make the operations non-commutative. Under the current additive implementation,
EX-009 is primarily an invariant test, not an ecological discovery experiment.

## Stage 6 — human decision and registry update

The human record must include:

```text
Decision: Accept | Reject | Revise and rerun | Inconclusive | Archive
Key Observation: one sentence
Evidence References: report, analysis, replay, or playtest IDs
Scope: what this authorizes and does not authorize
Review Time: minutes
Follow-up: next bounded test or explicit none
```

After that decision, add the forecast event to an append-only registry with its
context hash, probability, observed outcome, scoring result, tested range,
scenario coverage, causal status, and stale/superseded rule. The AI never
promotes its own conclusion.

## Metric dictionary minimum

| Field | Example |
|---|---|
| Stable ID | `PREY` |
| Plain-language label | Hare deaths caused by carnivores |
| Unit | deaths per run |
| Source | `deathEvents` with cause `Combat` |
| Aggregation | per-seed total; report mean across seeds |
| Direction | higher is more deaths/worse survival |
| Validity statuses | `Valid`, `Unavailable`, `Limited` |
| Known limitations | no per-step encounter denominator in current schema |

The dictionary is versioned with the report schema, hashed into the prediction
context, and acknowledged by the forecast record.

## Range and distribution policy

An effect at one catalog value is a point result, not evidence of smooth
scaling. A range claim needs multiple values or explicitly bounded regimes. If
the sign changes across a range or scenario family, report a piecewise regime.
If coverage is insufficient, use **Unresolved**.

## Minimal next increment

1. Add generated bounded-input summaries and a metric-dictionary hash.
2. Add sealed-context provenance and forecast-event fields.
3. Register seeds 1–20 as development and mark 101–110 as disclosed/consumed.
4. Replace EX-009's research priority with a commutativity unit test; retain an
   optional single runtime smoke pair after Unity preflight is healthy.
5. Run one clean B/A/C/AC design only if interaction is a current product
   question, using development seeds first and a fresh blind panel only after
   the contract is frozen.
6. Complete the human EX-007 decision and start the append-only forecast
   registry.

This improves validity without adding autonomous experiment selection, a
surrogate model, or a research dashboard.
