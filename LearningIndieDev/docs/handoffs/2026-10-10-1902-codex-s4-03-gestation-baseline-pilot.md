# S4-03 gestation baseline pilot

[Working state](../WORKING_STATE.md) | Status: in-progress

- Handoff schema: 1
- Handoff ID: 2026-10-10-1902-codex-s4-03-gestation-baseline-pilot
- Owner: Codex
- Branch: codex/s4-gestation-checkpoint-20261010
- Baseline commit: cea3c278d9e0e9984dbb4e1ddbb3fb5922d2093a
- Date: 2026-10-10
- Supersedes: none

## Summary

Ran an exploratory matched-seed S4 control/Gardeners pilot after the 48-tick animal gestation change. The approved 400/25/15 fixture gives little Gardeners exposure in these seeds. The existing 400/20/10 manual fixture allows all five seeds to reach the second choice and produces measurable seed renewal, while still showing frequent Hare extinction. A main screening fixture decision is pending; this is not strategy balance acceptance.

## Method

- Forest Edge, 36x20, 400 Plants, Hare focus, seeds 10101-10105, OpposedRoll combat, Natural attack opportunities, bev-experimental metrics, 0.1-second step, six continuous 100-tick phases.
- Control cumulative loadout: none in every phase. Gardeners: none at launch, Efficient Digestion at tick 100, Seed Dispersal at tick 200, then the same two skills through tick 600.
- Ran both arms at 400/25/15, then both at 400/20/10 without editing the production Forest Edge asset. Tick-zero counts and matching ruleset fingerprints were checked within each pair.
- Also ran seed 10100 at 400/25/15: Hares were extinct by tick 146 before the Seed Dispersal choice; that pair is not included in the five-seed table.
- Used direct Unity CLI because CellSim.ps1 currently references the missing Resolve-UnityExecutionLane helper. Each five-seed arm took about 23-27 seconds of CLI wall time on this host. Setup and review time were not separately measured.

## Five-seed results

| Fixture | Seed | Control stop tick / Hares | Gardeners stop tick / Hares | Gardeners drops / reserve spent |
| --- | ---: | ---: | ---: | ---: |
| 400/25/15 | 10101 | 234 / 0 | 234 / 0 | 0 / 0 |
| 400/25/15 | 10102 | 208 / 0 | 221 / 0 | 0 / 0 |
| 400/25/15 | 10103 | 152 / 0 | 152 / 0 | 0 / 0 |
| 400/25/15 | 10104 | 228 / 0 | 228 / 0 | 0 / 0 |
| 400/25/15 | 10105 | 600 / 1 | 600 / 1 | 1 / 1 |
| 400/20/10 | 10101 | 295 / 0 | 600 / 1 | 4 / 4 |
| 400/20/10 | 10102 | 458 / 0 | 600 / 18 | 23 / 23 |
| 400/20/10 | 10103 | 412 / 0 | 307 / 0 | 1 / 1 |
| 400/20/10 | 10104 | 267 / 0 | 271 / 0 | 1 / 1 |
| 400/20/10 | 10105 | 381 / 0 | 362 / 0 | 5 / 5 |

At 400/25/15, four of five controls reach tick 200 and four of five lose Hares by tick 234. Gardeners makes one successful drop across five seeds, creating 10 plant food and spending one stored food. At 400/20/10, all five controls reach tick 200 but all lose Hares by tick 458. Gardeners reaches tick 600 in two seeds and makes 34 successful drops, creating 340 plant food and spending 34 stored food. The other three Gardeners seeds lose Hares; two end sooner than their controls.

## Raw evidence and reproducibility

Raw JSON reports and compact summaries are retained locally under LearningIndieDev/artifacts/s4-gestation-pilot-20261010 in this managed worktree; artifacts are ignored by Git. The report schema is 30, and source code is checkpoint commit cea3c278. Each report's tick-zero population and paired ruleset fingerprint were checked.

| Local raw report | SHA-256 |
| --- | --- |
| control-seeds-10101-10105.json | 88C6B9683B92FF7B313C53618E685A05EB70F3736A295D0E610DFC2017ACF7BF |
| gardeners-seeds-10101-10105.json | FCD9986284A5FBDDF76A2A141F0949C606555DCCFDE2A8F494BBCB838F8DF118 |
| control-400-20-10-seeds-10101-10105.json | 798BA9D17FD11ADDD9A33325481B55F885E53F264D3589940530F47544E407B9 |
| gardeners-400-20-10-seeds-10101-10105.json | 4205073E66066B4F9DC76C1E3F7D89EB3E60257EF7DE81FD511819E971ABC9ED |

Reproduce with Unity CLI using the same branch, scenario, grid, populations, seed range, and schedule. Invoke SaltyGame.EditorTools.CellularSimulationExperimentRunner.RunFromCommandLine with -seedStart 10101 -seedCount 5 -scenarioPath Assets/Data/ProductionData/CellularSimulation/Scenarios/ForestEdge.asset -gridWidth 36 -gridHeight 20 -playerSpeciesId hare -stepIntervalSeconds 0.1 -phaseLengthTicks 100 -experimentalFeatures bev-experimental, plus -startingPopulations for the chosen fixture, -phaseUpgradeSchedule for the arm, and -outputPath for the report. The exact schedules are above.

## Interpretation and limits

- This was a small, exploratory pilot selected to check feasibility and choice reach. Five neighboring seeds are not a balance sample, and both population fixtures need a fresh predeclared screen before statistical claims.
- Different starting populations have different ruleset fingerprints by design; within each pair the ruleset fingerprint matches. Only the cumulative upgrade schedule differs within each matched pair.
- Successful seed drops confirm accounting, not a visible local patch network. Patch revisits, movement locality, and player comprehension still need telemetry and in-game review.
- The raw reports are local and ignored, so collaborators can read this summary or rerun the stated inputs; they cannot inspect the raw JSON from Git alone.

## Next useful step

Choose the main S4 screening fixture. Then run the predeclared 13-arm, 20-seed screen with the same control and scenario baseline, retain extinctions and choice reach, and follow with in-game review. Keep 400/25/15 as a stress comparison if 400/20/10 becomes the main screen. Repair the CellSim execution-lane helper before a larger automated checkpoint.
