# GLJP-06 route diagnostic preflight

November 7, 2026. [GLJP-06](https://trello.com/c/eo0iqoRL/154-gljp-06-diagnose-route-seed-survival-mismatch-before-further-development) covers seed 10100, which stopped completing the authored route, and seed 10108, which stopped serving as a natural failure fixture. Bevin approved the [bounded scope](../Research/Experiments/GLJP-Route-Seed-Diagnostic-20261106.md). The same-snapshot comparison stopped at its compatibility check. Both seeds still need comparable evidence before assigning a cause.

## Preflight result

- Built CellSim.Batch Release from the dirty `BevBranch` worktree at `8659a1b77b0f18ca01c4a31e744d90c5fbdb63e3`. Build reported zero warnings and zero errors. The frozen runner's 55 embedded source files match the live worktree and reproduce source hash `156fcb8921bc8b00dfbfd02b83571d69c0e07e3d75d6a21d64721bf052609804`. That is also the source hash of the retained November snapshot.
- The October snapshot and runner DLL file hashes match the original parity manifest. The historical snapshot has source hash `476b1e8d8d5e06e084247976a21c7b2ed8c4ffa34f05431e85324356d991d21a`. The fresh runner's `journey-replay` command exits 1 with `Worker source differs from export. Rebuild and re-export together.` The failure JSON says `executionStatus: crashed`. The stack trace places the rejection in `LoadSnapshot`, before snapshot restore or grid creation. The check applies to both seeds because it occurs before seed-dependent work. Zero simulations ran.
- Evidence is under `artifacts/gljp06-preflight-20261107-213929/`. `preflight.json` records source and binary hashes. `worktree-status.txt` discloses local changes. `guard-check-command.json`, `guard-check.log`, `must-not-exist.json`, and `guard-check-result.json` record the attempted load. Despite its filename, `must-not-exist.json` is the runner's intentional error record. The shell assertion initially mistook that file for a completed replay. The follow-up classified it by `executionStatus` and stack trace.

## What the retained reports show

| Seed and retained route | Tick 100 Hares / Foxes / Plants | Tick 200 Hares / Foxes / Plants | Observed outcome |
| --- | --- | --- | --- |
| 10100, October no action | 25 / 24 / 465 | 27 / 22 / 564 | Successful exit at 1800 |
| 10100, November no action | 16 / 24 / 490 | 2 / 25 / 618 | Failed at 237 |
| 10108, October scout | 17 / 25 / 435 | 8 / 28 / 551 | Failed at 236 |
| 10108, November Unity failure fixture | No comparable replay | No comparable replay | Reached Shop |

The two 10100 replay manifests point to the same empty schedule, with SHA-256 `8f45675393efa6a7b7051a82ed0d0ff5ec1b387ccb0361cadcd1cbeb6cc104f0`. The first payout was 25 Field Data in October and 16 in November. Both reports record the pre-choice populations at tick 100, so the population divergence was already present before an upgrade could be selected. The restored ecology payloads match in the retained snapshots, while their source hashes, journey definition fingerprints, and reward catalogs differ. The earliest observed population difference is tick 100. Earlier placement, births, deaths, feeding, and predation are unobserved in the retained journey reports.

The recovered October and live November `SpeciesInitialGridFactory.cs` files match after newline normalization. `SpeciesCell.cs`, `SpeciesSimulationRunner.cs`, and `SpeciesSimulation.cs` differ. Equal authored inputs and unchanged factory text do not establish the same initial board across builds. The separate 1,800-tick reference runs start with equal counts, have no acquired upgrades, and have population snapshots only at ticks 0 and 1800. Their aggregate scent and activity totals cannot locate the first journey divergence. The continuous reference and the journey replay also enter the simulation through different code paths.

Retained evidence locations are in the [November 6 evidence comparison](2026-11-06-gljp-route-evidence-comparison.md). Seed 10108's current `Shop` result is in `artifacts/gljp-finite-pool-unity-20261106/Natural-seed-check-results.xml`.

## Preparation for the proposed early ecology comparison

- Both frozen runners accepted their own retained exports through the normal `LoadSnapshot` guard. Both returned header-only reports for seeds 10100 and 10108 with the same ruleset fingerprint, 36×20 nonwrapping grid, 100 ticks, 0.2 seconds per tick, Hare focal species, Opposed Roll combat, Natural opportunities, empty upgrades, and no experimental feature preset or phase policy. The November header also confirms zero active Genome nodes. Report schemas are October 36 and November 41. Both header commands returned empty `runs` arrays.
- A tick-zero probe on a common .NET 10.0.14 PowerShell host matched the initial factory output. A follow-up probe on the runners' target runtime, **.NET 8.0.29**, used each binary's original snapshot guard, applied the proposed ecology settings, and constructed its run state and simulation runner. Both versions reported `Ready` at tick 0 with a 100-tick target and zero upgrades. For both seeds, all 29 shared `SpeciesCell` instance fields matched at all 720 coordinates, including species placement, energy, and plant growth counters. The November-only scent marker and scent memory fields had zero active values. Each board has 400 Plants, 55 Hares, 20 Foxes, and 245 empty cells. This verifies standalone ecology initialization. The Field Data journey session and Unity UI use separate entry points.
- Both runners accepted the four one-seed batch plans with `--dry-run`. These plans explicitly override the retained exports' `bev-experimental` argument with an empty value. The dry runs returned before creating output directories or launching workers. There were zero simulation ticks across the header, grid, and dry-run checks.
- `artifacts/gljp06-preflight-20261107-213929/matched-input-preparation/` contains the arguments, four plan files, command vectors, headers, `header-comparison.json`, grid probes, and dry-run logs. `native-probe/` contains the .NET 8 probe source, command, input hashes, and `result.json`. Each plan references the original snapshot by path and has a separate new output directory.

## Approved follow-up scope

Bevin approved this comparison on November 7, 2026. The approval covers the two named seeds, the two frozen standalone runners and their respective snapshots, and the first 100 ticks under the prepared ecology settings.

**Question.** For 10100 and 10108, where does the first shared population or event divergence occur in ticks 0–100 when each version loads its own pinned export and runs the same ecology settings? The observed route divergence predates the first choice, so the initial hypothesis is an early simulation difference. Scent is one candidate among several code changes.

**Method.** For each of the four verified plans, run `dotnet <runner.dll> batch <plan.json>` to retain the compact run and provenance, then `dotnet <runner.dll> replay-batch <plan-output> early-ecology <seed> <new-detail.json>` for the same plan and seed. This is two seeds across two runner versions, with one compact and one detailed execution per combination. The detailed path keeps tick-by-tick population and recorded event arrays. Both arms use natural ecology without a journey economy, purchases, reward choices, or Genome effects. Keep the October and November snapshot hashes and binary hashes in the result bundle.

**Decision criteria.** Verify all four completed runs and their detailed counterparts agree within each runner on the final digest and shared counters. Then compare the common per-tick populations, birth and death totals, food and predation counters, and any event fields with the same meaning in both schemas. Report the first observed difference, or no observed difference through tick 100. Record early extinction if an arm stops before tick 100. Compare per-tick records only through the common observed window. Do not compare native digests across versions because the reflected `SpeciesCell` field set changed. If comparable inputs or outcomes cannot be verified, report the failure and preserve the raw records.

This comparison isolates early ecology from route economy and choice handling. Establishing the contribution of scent or the cause of the authored journey failure requires separate evidence.

## November 7 execution and review

Both frozen versions accepted their own snapshots. All four detailed replays reached tick 100 with 101 board snapshots each. All four compact batch jobs failed twice with `Species/phase population accounting failed` before publishing a row. The planned batch validation therefore failed. The detailed replay command can emit a trace from a pinned batch manifest even when its compact job failed. Each raw detail file, the batch failure records, the pinned command vectors, and the independent comparison are in `artifacts/gljp06-preflight-20261107-213929/matched-input-preparation/`. `compare-detailed.py` writes `comparison.json` and checks the seed, tick coverage, starting counts, empty upgrades, manifest linkage, and Hare count arithmetic per tick.

| Seed | First difference in a species count | First Hare count difference | Hares at tick 100, October → November | Hare combat deaths / starvation deaths, October → November |
| --- | --- | --- | --- | --- |
| 10100 | Tick 17, Hares | Tick 17, 44 → 45 | 25 → 16 | 29 / 1 → 30 / 9 |
| 10108 | Tick 41, Plants, 479 → 481 | Tick 51, 35 → 36 | 17 → 13 | 33 / 5 → 35 / 7 |

The independent check verified every tick-to-tick Hare count using the combat, starvation, and crowding death events. There were no Hare births in these traces. October's earlier wilt events at tick 10 for 10100 and tick 14 for 10108 mislabel plants on animal tiles as creature deaths. November labels those same position/tick events as plant wilt. The [known October reporting bug](../UNITY_SIMULATION_TOOLING.md#mortality-on-shared-plantanimal-tiles-october-21) explains these early event-label differences. The first differing combat rolls were at tick 17 and tick 48, respectively. The November traces include 93 and 94 Fox ordinary scent detections, but multiple source paths changed. The trace supports the timing of the count changes and the death breakdown. It does not establish scent as the cause.

The batch failure is in reporting. The approved no-preset configuration leaves the experimental Hare `PREY` count at zero, while portable reporting always emits the species stat lines and the worker requires them to reconcile. In all four traces, the gap between expected and actual final Hare counts is exactly the number of combat deaths recorded by the general event stream. A compact and detailed digest comparison was therefore unavailable. Enabling the experimental preset would also change combat behavior, so the evidence retains the approved configuration and its original validation failures.

Seed 10100's detailed tick-100 counts match the two retained no-action journey reports at that boundary. That corroborates a difference in early ecology before any choice. The comparison has no current full journey counterpart for 10108. Leave the route root cause open and retain the existing acceptance expectations. A potential follow-up is to fix or scope the standalone validator to the metrics collected in the no-preset mode, then rerun the same pinned comparison. Isolating a gameplay cause requires a separate experiment.

## Approved November Fox scent comparison

Bevin approved the next isolated diagnostic on November 7, 2026. Compare ordinary Fox pursuit of Hare movement scent enabled and disabled for seeds 10100 and 10108 in the November standalone build through tick 100. The hypothesis is that Fox movement scent acquisition contributes to the early population and feeding differences. The two arms use the same source, scenario, starting grid, simulation mode, and empty Genome. Only the runtime switch on ordinary Fox movement scent acquisition changes. The diagnostic keeps dropping scent and all other behavior paths available. The source copy also records Hare food intake, successful feeding actions, starvation deaths, and Fox kills per tick in both arms. The diagnostic snapshot derives from the pinned November export with the isolated build's source hash and previously approved ecology arguments. The runner validates the hash through its ordinary loader.

Require the enabled arm to reproduce both previous November detailed reports, including final state digest and per-tick populations. Require the disabled arm to show zero ordinary Fox detections and verify the complete tick range, feeding totals, and Hare deaths against the detailed reports. Treat any divergence as an effect of disabling that behavior on these specific seeds. Keep the full authored journey and other revisions outside this comparison.

All four detailed replays finished through tick 100. The enabled arm reproduced both pinned November detailed reports byte for byte. The disabled arm recorded zero Fox ordinary detections and zero Fox scent pursuit decisions. Dropping scent detections were zero in all four runs. The disabled arm matched the October Hare, Fox, and Plant populations at **every tick from 0 through 100 for both seeds**. It also matched the October tick-100 Hare food intake, starvation deaths, and Fox kill totals. Independent checks reconciled the Hare count at every tick against creature death events and tied the instrumented feeding and death counts to each full report.

| Seed | Hares at tick 100, on → off | Successful Hare feeds, on → off | Hare starvation deaths, on → off | Fox kills, on → off | First difference in cumulative Hare food |
| --- | --- | --- | --- | --- | --- |
| 10100 | 16 → 25 | 205 → 240 | 9 → 1 | 30 → 29 | Tick 49 |
| 10108 | 13 → 17 | 167 → 253 | 7 → 5 | 35 → 33 | Tick 57 |

For 10100, the first starvation death moved from tick 64 with scent to tick 96 without it. The first population differences from disabling scent were tick 17 for 10100 and tick 41 for 10108, matching the first October/November differences. This isolates ordinary Fox scent pursuit as the cause of the measured early population divergence under these two configurations. The early first food difference can favor either arm at a particular tick, and the table reports cumulative outcomes over the entire window. This result does not establish whether the November behavior is desirable or whether turning it off would restore completion of the full authored journey.

Raw reports, per-tick telemetry, commands, snapshot and binary provenance, the isolated source copy, and independent `comparison.json` are in `artifacts/gljp06-scent-diagnostic-20261107-v2/`. The first scratch build failed because the diagnostic referenced a Fox constant scoped to another type. The corrected build used the stable Fox species ID and passed with zero warnings and errors. Both arms used the same binary and snapshot. The previously observed compact batch accounting failure remains a separate tooling issue.

## November 8 default decision and route result

Bevin changed the product direction on November 8: keep ordinary Fox pursuit of Hare movement scent **off by default**. This supersedes the previous paragraph's default-on decision. It does not remove Hare movement scent markers, nor the separate dropping-scent behavior. The new runtime option is independent of the other experimental feature bundle, defaults false, can be explicitly enabled in the preview's serialized settings, persists through runner shop-boundary option reconstruction, and is saved with preview defaults. The runtime test now checks ordinary Fox scent is absent by default and returns when the option is explicitly enabled; the dropping-scent tests remain on their existing path.

The already prepared full-route scratch A/B completed with the same isolated November runner, snapshot and empty action schedule. Seed 10100 changed from **Failed at tick 237 with ordinary scent on** to **Closed at tick 1800 with scent off**. Seed 10108 changed in the other direction: **Closed at tick 1800 with scent on** and **Failed at tick 236 with scent off**. Thus disabling scent changed both route outcomes, but it does not guarantee route completion across these two seeds. Both enabled reports retained their original reproduction expectations; all four reports completed at the authored horizon or terminal failure, and the same runner, snapshot and schedule hashes appear in each command record. The per-seed outcomes are in `artifacts/gljp06-scent-diagnostic-20261107-v2/journey-execution.json`; the earlier 100-tick population and feeding comparison remains in `comparison.json`.

This route A/B uses the retained isolated diagnostic runner built before the current worktree changes. It is evidence about those pinned inputs, not proof that the newly wired production default has completed an equivalent Unity journey. It also shows ordinary scent is a seed-sensitive mechanic; leave balance and route acceptance open. The production default follows Bevin's decision and is not inferred from which seed completes.

On November 8, the current Unity PlayMode suite completed **96 passed / 1 failed / 2 skipped**. Its only failure was the four-simulation test route, seed 10100, at tick 1130. A focused rerun reproduced the failure (`artifacts/unity-tests-20261108-102318/PlayMode-results.json`). The failure diagnostic records plants=0 and foxes=0 from at least tick 1119; Hares fall from two at tick 1126 to one at tick 1127 and zero at tick 1130. The run ends as expected when the player species is extinct. This confirms an ecological survival failure in that test scenario, not a journey shop or map-state transition failure. The focused test now includes the last twelve ticks of species counts in its failure message to retain useful evidence on future runs. No population or combat values were changed.

The later November 8 full PlayMode run on the Burrowing checkout reproduced seed 10100 extinction at tick 1210, with Plants at zero, Hares at zero, and three Foxes remaining (`artifacts/unity-tests-20261108-210949/PlayMode-results.json`). The previously reported tick 1130 was observed on an earlier checkout. The latest run confirms extinction after the second shop and leaves route survival open. The separate wallet forfeiture fixture passed with ordinary Fox scent explicitly enabled. No ecology values changed in this pass.

## November 9 direction — November is the canonical build

Bevin directed that November is the current build and that October-value testing stop. Do not run additional cross-build value comparisons or use October values to guide present tuning. Keep the completed October evidence above as historical only. GLJP-06 now focuses on the current November route failure and route behavior. This direction does not establish the cause of the failure or approve balance changes. The Editor later recovered and the current November route failure reproduced.

## November seed 10100 resource depletion

### Observation

On November 8 at 11:16 PM Eastern, the live Unity visual fixture `FieldDataAuthoredRouteCapturesRoundShopReorderAndSuccessEvidence` ran seed 10100 on the November worktree (dirty `BevBranch` at `8659a1b`, including the earlier ancestry digest change). It used the authored Forest Edge scenario, a fresh profile, and a test route of four simulations with six 100-step rounds each. The fixture skips Mutations and exercises income purchases and reorders. It applies no population interventions. Ordinary Fox movement scent is off by default. The only change for this check was exporting existing resource death events at the unexpected stop. Unity recompiled cleanly.

Command: `powershell -NoProfile -ExecutionPolicy Bypass -File .\CellSim.ps1 -Command Visuals -Execution Live -TestFilter FieldDataAuthoredRouteCapturesRoundShopReorderAndSuccessEvidence`.

The fixture failed its completion assertion at tick 1210 with journey state Failed. The retained captures show shop 1 at tick 600 and shop 2 at tick 1200. The journey ended ten ticks into simulation 3, before shop 3 at tick 1800. The population and Hare death CSVs are byte identical to the previous November visual run in `artifacts/visual-evidence-20261108-211940`. The new raw evidence is in `artifacts/visual-evidence-20261108-231609`.

| Ticks | Plants at start → end | Recorded consumption | Recorded wilt | Inferred Plant additions |
| --- | ---: | ---: | ---: | ---: |
| 1–600 | 400 → 690 | 205 | 1,120 | 1,615 |
| 601–914 | 690 → 1 | 1,019 | 422 | 752 |
| 915–1210 | 1 → 0 | 1 | 0 | 0 |

Additions are inferred from net population change plus recorded resource deaths. The event export contains direct removal records and no positional Plant birth records. The same accounting yields a nonnegative inferred addition count at every tick.

The last plant at (22, 14) was consumed at tick 915. The run reached 279 Hares with zero plants at tick 925. From ticks 915 to 1200, 357 Hare deaths were recorded as starvation and eight as combat. The remaining Hare died in combat at tick 1210. The combat event identifies the victim and location. It does not identify the attacker.

### Interpretation

Before shop 1, Plant additions outpaced losses. During simulation 2, consumption alone exceeded inferred additions. The final consumption removed the last source of ordinary Plant spread. The runtime removes the resource layer when a bite depletes it, and ordinary spread requires a live resource parent. The fresh-profile fixture skips Mutations and adds no plants, so the Plant count stays at zero for the rest of the run. This evidence explains the observed food collapse and ensuing starvation. It does not establish which upstream ecology setting made this seed vulnerable or whether the no-intervention route should meet a survival target.

The remaining GLJP-06 decision is whether this authored seed should be required to finish without population actions. If it is a success gate, the next ecology experiment needs a scoped November baseline, a defined player policy and survival criteria. If completion is a route-flow check, use a controlled completion fixture and retain the authored seed as a separate ecology observation. Keep the current failure evidence available for either choice.

## November 9 controlled route completion

The November Editor compiled cleanly and ran two PlayMode cases through the same desktop journey commands. The stable fixture uses a test-only population of 300 Plants, 200 Hares and zero Foxes, with Hare movement and reproduction disabled and metabolism set to zero.

- `FieldDataTestRouteReachesResultsAndRestartsWithStableHares` passed. It visited three shops, reached successful results at tick 2400, returned to the completed map, and started a new journey at zero balance. Repeated Continue commands did not duplicate payouts. The fixture preserves the existing profile values when restarting. Results are in `artifacts/unity-tests-20261109-110111/PlayMode-results.json`.
- `FieldDataTestRouteCompletesThroughAllShopsOnFixedSeed` still failed with authored Forest Edge and seed 10100. The final Hare died at tick 1210 with zero Plants and three Foxes. Results are in `artifacts/unity-tests-20261109-110139/PlayMode-results.json`.

The controlled case verifies the completion and restart path when the focal population survives. The authored survival expectation and ecology follow-up remain open. [Trello evidence](https://trello.com/c/eo0iqoRL#comment-6af1ee9ba19ab0a5ea199cfa)
