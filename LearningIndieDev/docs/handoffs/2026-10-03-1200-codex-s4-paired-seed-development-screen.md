# S4 Hare paired-seed development screen

[Working state](../WORKING_STATE.md) | Status: ready-for-review (local)

- Handoff schema: 1
- Handoff ID: 2026-10-03-1200-codex-s4-paired-seed-development-screen
- Owner: Codex; experiment decision owners Bevin and Salty
- Branch: BevBranch
- Baseline commit: 6a0ab08e155b1a8a2962d19a6c5eca1bc8233118
- Date: 2026-10-03
- Sharing: local source changes and documentation; no commit or push requested in this work block

## Work and experiment scope

Bevin requested paired-seed testing and explicitly selected 400 Plants / 20 Hares / 10 Foxes. Run the agreed 13-arm screen: skip-all control, six single skills, and both acquisition orders of Trailblazer (Movement/Avoidance), Warren (Hide/Crowding), and Gardeners (Digestion/Dispersal). Each arm uses seeds 10100-10119, ForestEdge 36x20, 600 ticks, six 100-tick phases, 0.1-second steps, opposed-roll natural combat, experimental features on, coupled Fox responses off, and no reinforcements. Singles acquire L1 at tick 100; pairs acquire first L1 at 100 and second L1 at 200; later choices are skipped.

This tests the existing phase simulator. Salty owns planning for the branching journey, DNA rewards and secondary long-running ecology simulator; none of that is implemented here.

## Corrections required before balance comparison

1. Scheduled experiments kept the initial experimental options even after acquiring avoidance. Resolve chance from the active snapshots at launch and each boundary, covering legacy and authored schedules.
2. The shared combat path still required positive flee-speed bonus before avoidance could occur. Remove that obsolete condition to honor the approved speed-free Threat Avoidance skill.
3. Avoidance hashing used process-wide entity IDs. Use the per-step seed and encounter coordinates so repeated runs are independent of previously allocated IDs.

Report schema is 31. Skill values, scenario assets, production population defaults, normal movement and upgrade prices were not tuned. The source diff and SHA256 of the four changed source/test files are retained with the final batch contract. Earlier runs remain diagnostic evidence, clearly superseded, rather than silently overwritten.

## Validation

- Unity compilation passed.
- Three focused checks executed directly in the live Editor through its existing ephemeral `run_script` command: scheduled options, Trailblazer snapshot/progression parity without flee speed, and avoidance behavior/replay across fresh entity IDs (384 fresh-grid comparisons).
- These are direct assertion checks, not a standard NUnit suite run. They do not change or save the open scene.
- Corrected full screen completed: 260 runs and 1,171 actual phase windows validated for seed/input parity, identical pre-choice phase, exact snapshots, reachable acquisition order, no reinforcements and population accounting.
- Seed 10100 Avoidance replay matches all run fields after excluding process-scoped entityId labels. All ten arms without Avoidance match their original diagnostic runs under the same exclusion.
- All five authored simulation assets and all four source/test file hashes remained unchanged during the corrected batch.
- Console errors zero. Native Console inspection classified 180 existing CS0618 obsolete-API compiler warnings plus one Unity AI Toolkit account-access warning; the CLI log buffer had evicted them. Nothing was cleared or suppressed.
- `git diff --check` passed.

## Evidence and reproduction

[Experiment package](../Research/Experiments/S4-03-Hare-Paired-Seed-Screen/README.md).

Raw requests, JSON/CSV, ordered acquisition snapshots, command results and timings: `artifacts/s4-paired-screen-20261003-400-20-10-verified/` (ignored, local only). Earlier full diagnostic batch: `artifacts/s4-paired-screen-20261003-400-20-10/`; partial scheduled-options-only diagnostic: `artifacts/s4-paired-screen-20261003-400-20-10-fixed/`.

The usual CellSim PowerShell wrapper still references missing `Resolve-UnityExecutionLane` / `Invoke-UnityLiveCommand` helpers. This work uses the already registered `cellsim_run` command directly, through the installed Unity CLI. Each arm's `request.json` retains exact inputs. Run requests sequentially; the command requires the Unity main thread. Never overwrite an existing report: use a new output path for a new run.

```powershell
& 'C:/Users/bevin/AppData/Local/Unity/bin/unity.exe' command cellsim_run `
  --request_path '<absolute new request.json>' `
  --project-path 'F:/ForkBin/GameDev/LearningIndieDev' --timeout 120 --json
```

`analyze-screen.py` uses only Python's standard library, checks the common starting phase, seed/input parity, ordered reachable acquisitions, exact snapshots, FPO accounting, and slashline validity statuses. Top-level prediction input is empty for this legacy schedule lane; full acquired snapshots and registry fingerprints are retained instead. Runtime scenario base rules are used without an additional profile Genome allocation.

## Observed result

Control survives in 7/20; Crowding Tolerance alone in 9/20. Trailblazer Movement-first is 2/20 versus 5/20 Avoidance-first; Warren Hide-first is 4/20 versus 5/20 Crowding-first; Gardeners Digestion-first is 2/20 versus 3/20 Dispersal-first. Every median final Hare population is zero. High mean final populations for Hide/Warren/Gardeners are driven by a few large survivors. See the separate [factual report](../Research/Experiments/S4-03-Hare-Paired-Seed-Screen/REPORT.md) and [interpretation](../Research/Experiments/S4-03-Hare-Paired-Seed-Screen/ANALYSIS.md).

No pair exceeded the control survival rate in this development panel. The suggested next diagnostic is Movement's contact/food/energy tradeoff using matched phase windows, followed by human review before tuning or confirmation.

## Remaining decisions

The 20-seed screen is exploratory. Human review, a separately frozen 200-seed confirmation, full five-pick paths/repeats, real offer availability and Clean-lane suite acceptance remain open. This does not approve scenario balance or a tuning change.
