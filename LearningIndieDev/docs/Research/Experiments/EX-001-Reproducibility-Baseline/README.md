# EX-001 - Reproducibility Baseline

**Experiment ID:** `EXP-001`  
**Status:** Accepted current-code reproducibility baseline; authored 32 x 32 configuration recorded  
**Owner:** Human design owner, with AI-assisted execution and analysis  
**Scenario:** `Assets/Data/CellularSimulation/Scenarios/ForestEdge.asset`

## Package contents

- [Experiment brief](EXP-001-brief.md)
- [Current-code execution attempt](RPT-RUN-001-0003-0020.md)
- [Current-code paired evidence](RPT-RUN-001-0004-0020.md)
- [Current-code gate analysis](ANL-RPT-RUN-001-0003-0020-v1.md)
- [Current-code paired-evidence analysis](ANL-RPT-RUN-001-0004-0020-v1.md)

## Current conclusion

The superseded schema-2 ForestEdge pair over seeds `10100` through `10119`
matched scenario metadata, ruleset fingerprint, per-seed run payloads, and
final population summaries. Its normalized run-payload SHA-256 was:

```text
8751175c405bbfb54f34089de182f65ca3f306fc92470e07b54a3b27f0c506e4
```

This historical fact is retained for provenance, but the source report and
analysis artifacts were removed after the current-code evidence superseded
them. The first current-code attempt on 2026-08-15 failed during Unity batch startup, but a
subsequent elevated run completed the current schema-4 matrix twice. The paired
reports and selected replays now satisfy the current-code gate. The superseded
schema-2 report used 32 x 20; the authored ForestEdge asset and current
experiment brief now consistently define 32 x 32.

## Current decision state

**Decision:** Accept  
**Key observation:** The current schema-4 matrix and selected replays match the
authored ForestEdge 32 x 32 configuration. The superseded 32 x 20 report's
normalized hashes are retained above for provenance only.
**Evidence:** [RPT-RUN-001-0004-0020](RPT-RUN-001-0004-0020.md)  
**Scope:** Accepts EX-001 as a reproducibility baseline only; no balance claim,
causal claim, or EX-002 promotion is implied.

**Decision ID:** `DEC-EXP-001-0002`  
**Decision owner:** Human design owner  
**Decision date:** 2026-08-15

EX-001 is complete within the recorded configuration. EX-002 may now be drafted,
but it must use its own controlled intervention design and human decision.
