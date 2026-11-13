# S6 predator-first Hare diagnostic — factual report

- **Experiment:** `S6-PF-DIAG-20261109`
- **Report:** `S6-PF-DIAG-R1`
- **Feature:** Forest Edge introductory Hare journey
- **Task:** Current-route baseline and three first-choice Mutation mechanics
- **Status:** Complete; awaiting Josh's design decision
- **Generated:** November 9, 2026
- **Contract:** [CONTRACT.md](CONTRACT.md)
- **Per-seed machine-readable summary:** [paired-summary.json](paired-summary.json)

## Evidence and provenance

| Evidence | Local artifact under `LearningIndieDev/artifacts/s6-pf-diag-20261109/` | Identity |
| --- | --- | --- |
| Unity export | `forest-edge-v2.json`, `.reference.jsonl`, `.unity.log` | Snapshot SHA-256 `429ea327fd8a6275cec833da710e04696bc64f01e94a6062b25fd38098daf455`; shared-source hash `6fcc6913b758717217150094b7bdd9c8afca85cb96eb96484ab31669f3db3990` |
| No-action journeys | `no-actions.json`, `journey-v2-10100.json` through `journey-v2-10111.json` | Empty schedule SHA-256 `25bf7ae31846fd2f26ae6ad383ab38a92838a588f31e357e4fec2b129d940410` |
| Mechanics batch | `mechanics-plan.json`, `mechanics-12-seed/batch.json`, `runs.jsonl`, `summary.json` | 48/48 validated; batch identity `74dfcc5627b501285f596f690c01b8ab24620c63b776c823e7ba75b09c923034`; raw JSONL SHA-256 `51f8e7ba130e1c84a86498cf333b35fc9229a947342d78c4aef01597a77383e0` |
| Focused Unity route test | `../unity-tests-20261109-195344/PlayMode-results.xml` | One test ran and failed at tick 236, as the assertion expected a complete route |
| Offered-choice replays | `choice-*.json`, `journey-choice-*.json` | Three first-choice selections succeeded; route/proxy boundary populations matched |

The export recorded base Git commit `6b6972dbe0216ff27ab968cad9be105b2cedc830` with a disclosed, then-uncommitted portable replay option-wiring fix in `CellSimPortable.cs` and `CellSim.Batch/Program.cs`. Simulation rules and authored assets were unchanged. Scenario fingerprint: `e89a6ed32cdf87a9314dd86349ce8cca8607bb266908e9553f8dd88ad5d0d616`. Field Data definition fingerprint: `ab50a72c5ec22ebaa4a4c35856eba0f8d43a0bddd4f891cec380a47f999ae4aa`. Unity 6000.4.6f1 exported the snapshot; the .NET 8 target worker ran with installed .NET 9 via `DOTNET_ROLL_FORWARD=Major` on Windows.

The first snapshot and pre-fix journey replay (`forest-edge.json`, `journey-10100.json`) are retained as rejected comparability evidence. That replay did not apply the snapshot's experimental feature options. The fixed export and worker are the inputs for all results below.

## Controlled method

Forest Edge used a 36×20 nonwrapping grid, 400 Plants, 55 Hares, 20 Foxes, opposed-roll combat, `bev-experimental`, no active Genome, and default ordinary Fox movement scent off. Seeds were 10100–10111. The authored journey has four 600-tick simulation nodes, with shops at ticks 600, 1200, and 1800. The no-action schedule skipped every offered reward and took no shop or population action.

The mechanics batch ran the same continuous world for 24 phases of 100 ticks. Phase 1 was identical in all arms. At tick 100, the three candidate arms respectively installed one `threat-exposure`, `tough-hide`, or `larger-minimum-litter` Mutation, then held that loadout through tick 2400. The control stayed at `none`. Batch ecology observation continued after Hare extinction; the journey stopped at extinction. Extinction ticks and shared decision-boundary populations were compared directly.

Commands, from the project directory, were `tools/Export-CellSimSnapshot.ps1` with the authored Field Data definition, `CellSim.Batch journey-replay <snapshot> <seed> <schedule> <output>`, `CellSim.Batch batch <mechanics-plan.json>`, and `CellSim.ps1 -Command Test -Mode PlayMode -Execution Clean -TestFilter FieldDataTestRouteCompletesThroughAllShopsOnFixedSeed -FilterType TestName`. The exact batch arguments and seeds are in the retained plan and manifest.

## Observed no-action route

| Measure | Observation on 12 seeds |
| --- | ---: |
| Hare extinction by tick 600 | 6/12 |
| Hare extinction by tick 1200 | 7/12 total |
| Hare extinction by tick 2400 | 10/12 total |
| Hare combat deaths, ticks 1–600 | 700 |
| Hare starvation deaths, ticks 1–600 | 105 |
| Seeds with zero Plants by tick 600 | 0/12 |
| Minimum Plant population at tick 600 | 646 |
| Seeds with zero Plants by tick 1200 | 4/12 |

The current checkout's focused Unity fixture stopped seed 10100 at tick 236, with 653 Plants, zero Hares, and 17 Foxes. The no-action portable journey also stopped at tick 236. Across all 12 seeds, no-action journey and batch had identical Hare extinction ticks and zero population mismatches at every shared 100-tick decision boundary. The two journeys that reached tick 2400 also had identical final board digests. This establishes ecology parity for these inputs; it does not validate the fresh-profile Mutation research gate.

| Seed | No-action Hare extinction tick | Outcome at tick 1200 |
| ---: | ---: | --- |
| 10100 | 236 | Failed in node 1 |
| 10101 | 1257 | Survived node 2 |
| 10102 | none by 2400 | Survived node 2 |
| 10103 | 231 | Failed in node 1 |
| 10104 | 1164 | Failed in node 2 |
| 10105 | 304 | Failed in node 1 |
| 10106 | 1346 | Survived node 2 |
| 10107 | none by 2400 | Survived node 2 |
| 10108 | 262 | Failed in node 1 |
| 10109 | 283 | Failed in node 1 |
| 10110 | 1343 | Survived node 2 |
| 10111 | 283 | Failed in node 1 |

## Matched Mutation observations

| Arm, one Mutation from tick 100 | Hare extinction by 600 | By 1200 | By 2400 | Hare combat deaths 1–600 | Hare starvation deaths 1–600 | Fox attacks 101–600 | Hare births 101–600 |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| No Mutation | 6 | 7 | 10 | 700 | 105 | 553 | 313 |
| Avoidance: `threat-exposure` | 5 | 6 | 9 | 667 | 102 | 468 | 274 |
| Survival: `tough-hide` | 5 | 5 | 9 | 681 | 107 | 619 | 214 |
| Reproduction: `larger-minimum-litter` | 6 | 6 | 8 | 704 | 102 | 538 | 196 |

The no-action and candidate arms had identical first-phase populations and ruleset fingerprints on every seed. Every candidate arm showed its intended upgrade in the phase-2 effective loadout. Relative to no action, Hare extinction moved later in 4/12 avoidance seeds, 5/12 survival seeds, and 4/12 reproduction seeds; it moved earlier in 6/12, 5/12, and 2/12 respectively. Six reproduction seeds had the same extinction tick as control. Per-seed values are in the linked JSON.

At the first decision, the exact tested Mutation was offered on 4/12 seeds for avoidance, 5/12 for survival, and 6/12 for reproduction. An offer from each of the three broader strategy families appeared together on 3/12 seeds. Three offer-aware journey replays selected the actual first offer: `tough-hide` on seed 10100, `larger-minimum-litter` on seed 10100, and `threat-exposure` on seed 10101. All three selections succeeded; their route and mechanics boundary populations matched, and the two successful journeys matched final board digests.

## Completion and limits

The batch completed 48/48 runs, self-test passed after routing temporary files into project artifacts, and the focused Unity test produced a normal ecological failure rather than a crash. The 12 consecutive seeds include known diagnostic seeds and are not a random or held-out sample. They are too few to calibrate a 60% failure probability or declare a Mutation effective. The forced mechanics arms bypass offer selection and the fresh-profile research gate. The portable report does not count pre-contact avoidance triggers directly. Detailed scene visuals and player comprehension were not tested here. Raw artifacts remain local and ignored by Git; this report and the compact per-seed summary are versioned.

**Human decision:** Pending; Josh owns the opening difficulty and Mutation viability decision.
