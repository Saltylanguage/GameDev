# S4 paired-seed development screen - 2026-10-03

Source: `6a0ab08e155b1a8a2962d19a6c5eca1bc8233118` plus the local scheduled-avoidance fix (dirty tree; source.diff and source SHA256 in experiment-contract.json). Report schema 31. Forest Edge 36x20; 400 Plants / 20 Hares / 10 Foxes; 0.1-second steps; six 100-tick phases; coupled responses OFF; no Genome additions or reinforcements. Seeds 10100-10119 shared by all 13 arms. Choices at ticks 100 and 200; later choices skipped.

## At a glance

All 260 corrected runs completed and passed the declared seed, starting-state, acquisition and accounting checks. The skip-all control retained Hares at tick 600 in 7/20 runs; Crowding Tolerance alone did so in 9/20; the six pairs ranged from 2/20 to 5/20. Every arm's median final Hare population was zero. This supports further diagnosis, not strategy acceptance or a balance verdict.

**Experiment:** S4-03-20261003-400-20-10-verified. **Owners:** Bevin and Salty. **Decision:** pending. **Lifecycle:** fresh tick-zero grid per seed/arm, then a continuous evolved world through six phases; extinction ends the run early. **Metric contract:** cellsim-experiment-metrics v1, report schema 31. Scenario base rules are used without an additional profile Genome allocation. The top-level avoidance chance describes the launch configuration (zero here); acquired snapshot bonuses define later scheduled chance.

Scheduled avoidance options, the obsolete flee-speed guard, and process-wide ID hashing were corrected before this batch. Earlier full/partial runs remain diagnostic/superseded, with raw files retained. See the [handoff](../../../handoffs/2026-10-03-1200-codex-s4-paired-seed-development-screen.md).

## Observed outcomes

| Arm | Survived / 20 | Mean FPO | Median FPO | Mean paired FPO change | Better / equal / worse FPO | Picks reached 100 / 200 |
|---|---:|---:|---:|---:|---:|---:|
| Skip-all control | 7 | 3.00 | 0.0 | +0.00 | 0 / 20 / 0 | 0 / 0 |
| General Movement | 4 | 0.50 | 0.0 | -2.50 | 2 / 13 / 5 | 20 / 0 |
| Threat Avoidance | 5 | 3.65 | 0.0 | +0.65 | 2 / 15 / 3 | 20 / 0 |
| Tough Hide | 3 | 9.55 | 0.0 | +6.55 | 2 / 13 / 5 | 20 / 0 |
| Crowding Tolerance | 9 | 2.90 | 0.0 | -0.10 | 2 / 17 / 1 | 20 / 0 |
| Efficient Digestion | 7 | 3.55 | 0.0 | +0.55 | 2 / 17 / 1 | 20 / 0 |
| Seed Dispersal | 3 | 0.65 | 0.0 | -2.35 | 2 / 13 / 5 | 20 / 0 |
| Trailblazer: Movement then Avoidance | 2 | 0.10 | 0.0 | -2.90 | 1 / 13 / 6 | 20 / 17 |
| Trailblazer: Avoidance then Movement | 5 | 0.50 | 0.0 | -2.50 | 2 / 12 / 6 | 20 / 18 |
| Warren: Hide then Crowding | 4 | 9.60 | 0.0 | +6.60 | 3 / 12 / 5 | 20 / 18 |
| Warren: Crowding then Hide | 5 | 2.95 | 0.0 | -0.05 | 3 / 13 / 4 | 20 / 19 |
| Gardeners: Digestion then Dispersal | 2 | 8.85 | 0.0 | +5.85 | 1 / 13 / 6 | 20 / 19 |
| Gardeners: Dispersal then Digestion | 3 | 0.15 | 0.0 | -2.85 | 1 / 13 / 6 | 20 / 18 |

## Evidence and limits

- `validation.json`: provenance, initial-state, schedule, accounting and report hashes.
- `final-slashlines-and-diagnostics.csv`: all raw counts, ratios, validity statuses, direct skill counters and Fox/Plant final populations.
- `phase-slashlines.csv`: actual observed windows; missing later phases remain absent rather than zero.
- `paired-control-deltas.csv`: per-seed candidate/control metrics and valid deltas; incomparable values remain N/A.
- `arm-summary.csv`: means with valid denominators, ranges, median, acquisition reach and seed variation.
- `resolved-skills.json`: exact resolved values and registry provenance for acquired skills.
- Each arm folder retains command request, raw JSON/CSV, CLI result and timing.

These are development-screen observations, not accepted tuning or whole-ecosystem balance. The two-pick pairs are not full five-pick builds. Pair/single differences include acquisition timing; they are not a clean isolated synergy estimate. Raw counts must be read alongside exposure and extinction windows. APS is shown in the CSV with its components, not used as a lone ranking. The pre-fix control replay matched all non-ID fields; raw entity IDs retain a process-wide counter offset.

Human strategy review, real offer/board observation, and separately frozen 200-seed confirmation remain open.

## Slashline and direct counters

Means below are valid per-run means over all 20 seeds, not pooled encounter probabilities. Whole-run exposure windows differ on extinction. pAVI measures post-contact survival, eAVI encounter avoidance, sAVI starvation survival, and bAVG births per mating candidate.

| Arm | pAVI | eAVI | sAVI | bAVG | Crowded animal-ticks | Crowding energy lost | Plants created |
|---|---:|---:|---:|---:|---:|---:|---:|
| Skip-all control | 0.417 | 0.700 | 0.057 | 0.01117 | 18.10 | 18.00 | 0.00 |
| General Movement | 0.419 | 0.673 | 0.028 | 0.01111 | 22.10 | 21.95 | 0.00 |
| Threat Avoidance | 0.415 | 0.697 | 0.036 | 0.01098 | 27.05 | 26.70 | 0.00 |
| Tough Hide | 0.493 | 0.669 | 0.054 | 0.01122 | 24.05 | 23.95 | 0.00 |
| Crowding Tolerance | 0.420 | 0.688 | 0.059 | 0.01109 | 23.25 | 14.05 | 0.00 |
| Efficient Digestion | 0.412 | 0.688 | 0.052 | 0.01139 | 25.10 | 24.95 | 0.00 |
| Seed Dispersal | 0.424 | 0.681 | 0.055 | 0.01074 | 22.60 | 22.55 | 24.75 |
| Trailblazer: Movement then Avoidance | 0.408 | 0.678 | 0.019 | 0.01111 | 36.20 | 36.05 | 0.00 |
| Trailblazer: Avoidance then Movement | 0.420 | 0.684 | 0.034 | 0.01109 | 17.90 | 17.75 | 0.00 |
| Warren: Hide then Crowding | 0.495 | 0.682 | 0.056 | 0.01112 | 26.45 | 16.80 | 0.00 |
| Warren: Crowding then Hide | 0.435 | 0.686 | 0.024 | 0.01134 | 17.80 | 9.65 | 0.00 |
| Gardeners: Digestion then Dispersal | 0.416 | 0.674 | 0.028 | 0.01152 | 31.75 | 31.55 | 33.20 |
| Gardeners: Dispersal then Digestion | 0.428 | 0.688 | 0.039 | 0.01074 | 30.05 | 29.90 | 25.05 |

Whole-run and phase exports retain original validity statuses; valid denominators are included per metric in arm-summary.csv, and incomparable paired deltas remain N/A. The 2,080 whole-run ratio entries include 1,877 Valid and 203 N/A values; no INVALID values were observed. Every valid cAVI value was 1.0; 201/260 cAVI values were N/A. It measures direct crowding deaths, not energy saved. sAVI had 258 Valid values and 2 N/A; pAVI/eAVI/bAVG were Valid in all 260 runs. Seed planting attribution uses successes and food-spend accounting, rather than total final plants.

## Verification

- 260 final rows and 1,171 actual phase windows validated; identical 400/20/10 start and first phase before choices, ordered reachable acquisitions, exact snapshots, FPO reconciliation and zero reinforcements.
- Three focused assertion methods executed directly in the live Editor, including 384 fresh-grid avoidance comparisons. This is not a standard NUnit runner or full-suite result.
- Seed 10100 avoidance replay matches all run fields except process-scoped `entityId` labels. Control and the nine other arms without Avoidance also match the original diagnostic batch under that exclusion; this is not a separate clean-process replay.
- Four changed source/test files match their recorded SHA256; all five authored ProductionData simulation assets match their pre-run SHA256.
- Compilation passed; Console errors zero. All 181 native Console warnings were classified: 180 CS0618 obsolete-API compiler warnings from existing code/tests, plus one Unity AI Toolkit account-access warning. The CLI's capped log buffer had evicted warnings; native inspection was retained. Nothing was suppressed or cleared.
- `git diff --check` passed.

## Evidence files

Raw requests, JSON/CSV and command results remain under ignored `artifacts/s4-paired-screen-20261003-400-20-10-verified/`. They are local evidence, not published to another checkout. Full report hashes, source hashes and registry fingerprints are retained there. The usual CellSim wrapper still references missing helpers; this screen used the existing registered `cellsim_run` directly.

- [Contract and source hashes](../../../../artifacts/s4-paired-screen-20261003-400-20-10-verified/experiment-contract.json)
- [Validation and report hashes](../../../../artifacts/s4-paired-screen-20261003-400-20-10-verified/validation.json)
- [Replay, mechanism and unchanged-asset checks](../../../../artifacts/s4-paired-screen-20261003-400-20-10-verified/mechanism-and-replay-validation.json)
- [All final slashlines and diagnostics](../../../../artifacts/s4-paired-screen-20261003-400-20-10-verified/final-slashlines-and-diagnostics.csv)
- [Actual phase windows and slashlines](../../../../artifacts/s4-paired-screen-20261003-400-20-10-verified/phase-slashlines.csv)
- [Paired seed deltas](../../../../artifacts/s4-paired-screen-20261003-400-20-10-verified/paired-control-deltas.csv)
- [Summary, ranges and valid denominators](../../../../artifacts/s4-paired-screen-20261003-400-20-10-verified/arm-summary.csv)

**Human decision:** pending, Bevin/Salty. No tuning is approved. Five-pick paths, repeat levels, longer horizons and independent confirmation remain outside this screen.
