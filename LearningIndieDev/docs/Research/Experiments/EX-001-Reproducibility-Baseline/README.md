# EX-001 - Reproducibility Baseline

**Experiment ID:** `EXP-001`  
**Status:** Provisional evidence captured; current-code rerun and replay gate pending  
**Owner:** Human design owner, with AI-assisted execution and analysis  
**Scenario:** `Assets/Data/CellularSimulation/Scenarios/ForestEdge.asset`

## Package contents

- [Experiment brief](EXP-001-brief.md)
- [Factual report](RPT-RUN-001-0001-0020.md)
- [AI analysis and suggested actions](ANL-RPT-RUN-001-0001-0020-v1.md)

## Current conclusion

Two historical ForestEdge runs over seeds `10100` through `10119` have matching
scenario metadata, ruleset fingerprint, per-seed run payloads, and final
population summaries. Their normalized run-payload SHA-256 is identical:

```text
8751175c405bbfb54f34089de182f65ca3f306fc92470e07b54a3b27f0c506e4
```

This is strong evidence that that historical pair was reproducible. It is not
yet a current-code pass: both artifacts use report schema `2`, while the
current experiment runner declares schema `4`, and no fresh replay was
captured for this package because the Unity project was open during execution.

## Human decision required

Choose one after reviewing the report and analysis:

1. **Accept provisional evidence and schedule a current-code rerun.**
2. **Require a fresh rerun before treating EX-001 as passed.**
3. **Record an instrument gap and revise the experiment.**

Do not begin EX-002 until this decision is recorded.
