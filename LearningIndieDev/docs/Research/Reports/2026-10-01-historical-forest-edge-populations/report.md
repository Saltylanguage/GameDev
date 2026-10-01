# Historical Forest Edge population evidence

**Packaged:** 2026-10-01 by Codex. **Status:** historical evidence for review.
This is a compact extraction of existing September 29 local runs (UTC reports
are dated September 30). No experiment was rerun to create this package.
The current S4 basic-skill proposal still requires its own pilot and evidence.

## Recorded setup and outcomes

All three panels started 400 Plants / 25 Hares / 15 Foxes on a 36x20 grid,
with opposed-roll combat, natural attack opportunities, coupled responses off,
`bev-experimental`, a 600-tick limit and six 100-tick phases.
The baseline covers seeds 10100-10299; each pilot arm covers 10100-10119.

| Panel | Runs | Step seconds | Player species | Observed Hare extinctions | Median first-zero tick | Mean terminal Hare | Runs reaching 600 |
| --- | ---: | ---: | --- | ---: | ---: | ---: | ---: |
| baseline-200 | 200 | 0.1 | fox | 178 | 211.0 | 1.635 | 4 |
| control-20 | 20 | 0.2 | hare | 2 | 177.0 | 119.050 | 18 |
| guarded-burrow-20 | 20 | 0.2 | hare | 0 | N/A | 137.100 | 20 |

The baseline observed Fox extinction in 196/200 runs, with median first-zero
tick 351. Its Hare median first-zero tick was 211 among the 178 observed
extinctions. Baseline runs stopped at Fox extinction; Hare outcomes with no
observed extinction can be censored. The pilot arms stopped at Hare extinction.
Terminal populations are measured at each run's ending tick, not necessarily 600.
The baseline and pilot use different step intervals and player termination rules;
they are not one matched comparison. The two pilot arms match each other.

Guarded Burrow was acquired after phase 1 and retained thereafter. Its effect
was block +2 and movement speed -0.25. This composite Mutation is different from
the proposed S4 basic Hide skill, which has no movement penalty. Pilot mean APS
was 1.35887 for control and 1.56838 for Guarded Burrow.

## Shareable measurements and limits

[Per-seed measurements and Hare slash lines](per-seed.csv) contains 240 rows
extracted from the raw JSON reports, including seed, run-ending tick, starting
and terminal populations, observed first-zero ticks, and all recorded Hare
slash-line fields. A blank first-zero tick means no extinction was observed
before that run ended. Baseline slash-line cells are blank because that report
selected Fox and did not record the Hare slash line.

The archived independent pilot validators each checked 20 seeds: all 180 metric
comparisons per arm passed and all FPO counts reconciled. Their status was
`VALIDATED_WITH_LIMITATIONS`: raw per-step HPS/EHS/ECN event lists were absent.
These are archived validation results, not fresh Unity tests or a balance approval.

Baseline metadata records source commit
`b049c35592f7878c41ede25b08fcf851e6a15215`, a dirty working tree, and Unity
6000.4.6f1. The source commit for each pilot was not captured in the retained
request/report pair; its fingerprints are preserved below. This extraction
supports inspection of the reported numbers, but is not a full replay bundle.
The original requests, event histories and validator JSON remain local artifacts.
The earlier 400/55/5 comparison is also historical and is not packaged here.

## Source identities

### baseline-200

- Original report: `artifacts/forest-edge-baseline-400-25-15-20260929/report.json` (local, ignored; raw report is not bundled here).
- Report UTC: `2026-09-30T03:05:56.9607430Z`.
- Raw report SHA-256: `49251a9068814f197d427f6e1212750afb2074a61778d86c578767b7f2b3b3ed`.
- Ruleset fingerprint: `ec53a16960c9c68179f46c9e2768c94cf964b904adab3e84437bf4bc8be47da4`.
- Run provenance fingerprint: `dbb401c79866176f1d15a48c45c52223c404d317840ed97986db541d56245fa4`.
- Phase upgrade schedule: `none;none;none;none;none;none`.

### control-20

- Original report: `artifacts/forest-edge-warren-guarded-burrow-200-20260930/control-20-report.json` (local, ignored; raw report is not bundled here).
- Report UTC: `2026-09-30T03:48:22.0237294Z`.
- Raw report SHA-256: `73722676a03cdc19a1214383ffea7466e97dd9f1ca3776cb85f5f6103092e04b`.
- Ruleset fingerprint: `b7d466d3eb4a1657c2f11942cbda93b02faec4973e41f6c91ad2e08dfbb05a3a`.
- Run provenance fingerprint: `69fafc663bf81c16edb98e94b5fb27f22c6e891ba1f8925ba21656841d96ab45`.
- Phase upgrade schedule: `none;none;none;none;none;none`.

### guarded-burrow-20

- Original report: `artifacts/forest-edge-warren-guarded-burrow-200-20260930/guarded-burrow-20-report.json` (local, ignored; raw report is not bundled here).
- Report UTC: `2026-09-30T03:49:39.5499997Z`.
- Raw report SHA-256: `8d4937bb4484efc6e92455522c7c7a00c79b814fedc06297eafedf97a24b1a19`.
- Ruleset fingerprint: `b7d466d3eb4a1657c2f11942cbda93b02faec4973e41f6c91ad2e08dfbb05a3a`.
- Run provenance fingerprint: `b55fdcb1a9c0aabea35b6095d2764ae3bcb46af1fe266fe884787f73ac5034a1`.
- Phase upgrade schedule: `none;warren-guarded-burrow;warren-guarded-burrow;warren-guarded-burrow;warren-guarded-burrow;warren-guarded-burrow`.
