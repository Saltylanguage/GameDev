# Second-pass delta

This note records what changed after a higher-effort review. It exists so the
treatment does not quietly overwrite its own earlier reasoning.

The full earlier interpretation is preserved in
[AI analysis v1](ai-analysis-v1.md); [the v2 report](report-source.md) is the
recommended current reading.

## Material corrections

### EX-009 was over-prioritized

The first pass recommended finishing the five-seed same-order comparison as a
priority research experiment. That was too expensive for the question.

In the current implementation:

- `faster-movement` adds only to `MovementSpeed`;
- `crowding-tolerance` adds only to `CrowdingTolerance`;
- all upgrades are applied before the simulation starts;
- both orders produce the same `rulesetFingerprint`; and
- the existing 20-seed forward and reverse reports contain identical run
  objects for every seed.

The different `runProvenanceFingerprint` values are expected because loadout
order is deliberately preserved in provenance. They are not evidence of a
different simulated ruleset.

**Revised recommendation:** encode this as a commutativity unit test. Keep one
same-seed end-to-end smoke test only if the team wants to verify command-line
wiring. A full ecological trial becomes justified if upgrades later gain
stateful application, caps, multipliers, unlock effects, or other order-sensitive
behavior.

### “Training seeds” conflicts with the project’s own plan

The research plan reserves “training” for an approved model-training design,
but EX-007 uses the term for seeds 1–20. No model was trained on those seeds.
Future material should call them the **development** or **calibration** panel.
Historical files should remain unchanged and receive a terminology note rather
than a rewrite.

### Previously viewed held-out seeds are no longer blind

Seeds 101–105 and 106–110 have now been inspected and used to shape follow-up
questions. They remain valid evidence for their original experiments, but they
should not be reused as unseen confirmation for a future claim. Future work
needs an explicit seed registry with development, limited-use validation, and
one-time blind-promotion panels.

### The confidence check uses the wrong unit

EX-007 assigns one confidence value to each arm, then informally compares it
with eight direction checks made from four correlated metrics across two
panels. Those are not eight independent probability forecasts, and the single
confidence value does not define which event it predicts.

Future records should either:

1. attach a probability to each pre-defined metric event; or
2. define one composite success event before execution and attach one
   probability to that event.

Only independent, completed forecast events should enter calibration summaries.

### The AI information boundary is asserted, not reproducible

The prediction note says that intervention results were unavailable, but the
record contains no model identifier, exact prompt, context manifest/hash,
invocation time, or evidence that the prediction ran in a fresh context. This
does **not** prove leakage occurred. It means isolation is unknown and cannot be
audited later.

## New interpretation of the five-seed panels

Five paired seeds are useful as a transfer smoke test and for inspecting exact
outcomes. They are weak as a final promotion panel. For example, even if all
five non-tied paired effects have the same sign, the smallest possible exact
two-sided sign-test probability under a no-direction null is `2 / 2^5 =
0.0625`. This is not an instruction to optimize around p-values; it is a clear
demonstration of the panel’s limited resolution.

## Boundary retained

This second pass still changes only files in `docs/DeepResearch_Treatement`.
It does not alter canonical plans, historical predictions, experiment reports,
simulation code, or human decisions.
