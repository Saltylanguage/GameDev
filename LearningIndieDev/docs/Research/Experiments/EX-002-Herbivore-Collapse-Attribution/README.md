# EX-002 - Herbivore collapse attribution

**Experiment ID:** `EXP-002`  
**Status:** Schema-5 death telemetry integrated; execution blocked by Unity batch startup failure  
**Decision owner:** Human design owner  
**Reference scenario:** `Assets/Data/CellularSimulation/Scenarios/BaselineParity.asset`

EX-002 investigates how a simulation-specific growth rule can identify and test
a cell type's collapse state. The accepted ForestEdge result remains the
instrument-trust baseline, but ForestEdge's player species is `hare`; it is not
the correct reference for an herbivore-specific attribution claim. BaselineParity
is the first scenario adapter because it defines `herbivore` and produced one
extinction in its pre-telemetry-extension 20-seed EX-001B pair.

## Package contents

- [Experiment brief](EXP-002-brief.md)

The factual report and separate analysis must be added only after the Unity
batch failure is resolved and the controlled intervention matrix is approved.

## Current interpretation boundary

The existing evidence supports a proximate starvation signal in BaselineParity,
not a complete causal explanation or a universal collapse detector. The prior
schema-4 report contains aggregate death counts only; it cannot be retroactively
converted into per-entity death events. The first schema-5 rerun must verify the
event stream before the baseline is treated as the EX-002 instrumented result.
Nothing currently establishes that starvation is the root cause, that the rules
are unbalanced, or that the result transfers to other scenarios.
