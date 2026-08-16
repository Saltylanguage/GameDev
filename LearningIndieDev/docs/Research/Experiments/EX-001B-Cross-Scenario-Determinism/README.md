# EX-001B - Cross-Scenario Determinism

**Experiment ID:** `EXP-001B`  
**Status:** Accepted bounded cross-scenario reproducibility result; all four scenario pairs match  
**Parent:** `EXP-001 - Reproducibility Baseline`

This is a bounded extension of EX-001. It asks whether the current shared
simulation engine reproduces the same outcomes for each currently authored
scenario when that scenario is repeated with identical inputs.

## Package contents

- [EXP-001B brief](EXP-001B-brief.md)
- [Factual paired report](RPT-RUN-001B-0001-0003.md)
- [Separate analysis](ANL-RPT-RUN-001B-0001-0003-v1.md)
- Human decision: Accept (`DEC-EXP-001B-0001`)

Generated JSON, CSV, and Unity log artifacts remain under the ignored
`LearningIndieDev/artifacts/` directory. The final report records their paths,
raw hashes, and normalized comparison results without copying generated output
into the documentation tree.

## Interpretation boundary

A pass supports the statement that the shared engine is reproducible across the
tested authored scenarios and input range. It does not prove that every
cellular automaton is deterministic, that one scenario's ecological findings
apply to another, or that the model is correct or balanced.
