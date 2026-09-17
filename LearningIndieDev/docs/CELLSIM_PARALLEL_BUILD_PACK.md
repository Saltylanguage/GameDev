# Parallel CellSim: agent build pack

Date: 2026-09-09  
Status: Draft for Bevin to give to an implementation agent; nothing provisioned  
Human decision owner: Bevin  
Implementation owner: assigned AI agent  
Companion: [copy-paste implementation prompt](CELLSIM_PARALLEL_AGENT_PROMPT.md)

## Outcome

Run independent CellSim skill experiments concurrently on one Docker host, collect the existing reports and full stat lines, and demonstrate a measured reduction in total testing time without changing simulation outcomes. Bevin should be able to request a skill/level/seed matrix and receive one validated artifact bundle through a documented command that an AI agent can also operate.

This pack specifies future implementation. Giving the companion prompt to an agent with an instruction to implement supplies the work assignment. This planning session does not install software, start experiments, change gameplay, or activate the existing remote worker.

## Current evidence and corrections to the initial proposal

Inspected local `BevBranch` at `9127195` on 2026-09-09. `SpeciesUpgrade.cs` and `SpeciesDomainTests.cs` have existing uncommitted changes. Recheck all of this at implementation time; HEAD alone does not identify those skill changes.

| Existing surface | Verified behavior | Consequence for this build |
|---|---|---|
| [Run-CellularExperiment.ps1](../tools/Run-CellularExperiment.ps1) | Runs an Editor method with `-batchmode -nographics`; creates manifests and invokes stat-line validation. | Existing entry point cannot simply be launched as a player executable. Reuse its report contract and validation. |
| [CellularSimulationExperimentRunner.cs](../Assets/Editor/SimulationTools/CellularSimulationExperimentRunner.cs) | Editor-only scenario loading with `AssetDatabase`, `EditorApplication.Exit`, schema 25 reports, serial seed loop, and continuous phase schedules. | Separate asset loading/Editor lifecycle from reusable simulation and reporting code. |
| [SimulationReportSerialization.cs](../Assets/Editor/SimulationTools/SimulationReportSerialization.cs) | Report helpers also live under `Assets/Editor`. | Audit their callers and types when extracting the minimum shared reporting surface. |
| [UnityTooling.ps1](../tools/UnityTooling.ps1) | Rejects any running Unity process; Windows licensing paths; cleanup discovers newly appearing helper processes by name. | Launching several copies on one host is not a safe parallel implementation. Preserve the legacy guard; use container ownership for worker lifecycle. |
| [Start-CellSimWorker.ps1](../tools/Start-CellSimWorker.ps1) | Serial loop copies Pending JSON to a temporary location; optional Git synchronization/publication. | Copying is not an exclusive claim. Multiple consumers can execute the same job. Use one coordinator for the first version. |
| [CellSim.ps1](../CellSim.ps1) | Defaults combat to `legacy-fixed-damage`; direct experiment script defaults to `opposed-roll`. | Resolve and record every scientific setting explicitly; do not inherit inconsistent wrapper defaults. |

The simulation already advances ticks in a tight loop. Twenty simulated seconds at a 0.1-second step means 200 ticks; it is not a required twenty-second wall-clock wait. Measure where time goes before predicting Docker speedup.

Docker installation, daemon access, host CPU/RAM, Linux build support, package compatibility, and a working Linux player have NOT been verified. The runtime assembly permits Unity engine references and references Input System; do not assume it is already an independent .NET executable. Noesis loading in a minimal simulation build needs a test, not an assumed failure.

## Recommended first version

Build a Linux headless simulation player once per immutable source/content snapshot using the existing licensed Unity installation and required Linux build module. Package that player in a local Docker image. One host-side PowerShell coordinator assigns explicit jobs to a bounded number of disposable worker containers and validates their outputs.

Each worker receives read-only inputs and one writable attempt directory. It executes its assigned seed chunk sequentially using the shared simulation code. Parallelize independent seeds/conditions; keep all ticks and consecutive phases of one expedition in the same worker.

The coordinator owns local job state and aggregation. Worker containers do not poll the existing Git queue, push results, mount the Docker socket, or need a Unity project/Library directory. Only the build step needs an Editor and import cache. An Editor-container fallback would require separate writable project/cache directories, a supported activation method, and its own parity proof.

Keep the first release to one host and one controller per batch. A JSON plan, local files, Docker CLI, and existing report validators are enough. Defer cloud provisioning, multiple hosts, Kubernetes, queue databases, web dashboards, custom MCP servers, and containerized Editor CI. A native player benchmark may be useful if it can use the same runner; it is not a reason to build another simulator.

## Ordered goals

All goals start as Not Started. Complete each acceptance gate before depending on it. Record commands, evidence paths, and remaining limitations in the implementation handoff.

### G0 - Establish a reproducible starting point

**Work:** Inspect the checkout, active work, current runner callers, report consumers, scenario/upgrade assets, package lock, and relevant continuation tests. Record host CPU/RAM, Docker's Linux engine availability, resource allocation, Unity version and Linux build-module availability. Keep prerequisite checks read-only until implementation authority covers any missing installation.

Choose an isolated build snapshot. Include required current skill changes only through an intentional snapshot with file hashes/patch provenance; never silently benchmark HEAD while claiming to test uncommitted skills. Record resolved grid size, ticks, species, upgrade loadouts, scenario fingerprint, and modes. Prefer a small manifest of build inputs over an entire worktree copy full of artifacts.

**Deliver:** A short feasibility record and a frozen validation matrix. Reuse a historical baseline only when all source/content/configuration identities match; otherwise create a fresh serial reference during authorized validation.

**Pass:** The agent can explain the source-to-result flow, identify missing prerequisites, and reproduce the exact requested input identity. Choose the first build route from evidence. If the player route requires a broad engine/package rewrite, document the smallest working alternative and obtain direction on that scope change.

### G1 - Run the same experiment through a headless player

**Work:** Extract only the necessary experiment execution/reporting code into a player-compatible shared location. Keep existing Editor entry points as adapters. Trace all affected callers before moving any type; preserve GUIDs and serialized references. Reuse `SpeciesInitialGridFactory`, `SimulationRunState`, `SpeciesSimulationRunner`, existing upgrade application, and the existing serializers.

Resolve scenario and upgrade assets through a small explicit build input catalog or a lossless exported snapshot. Choose one after inspecting the existing asset API. Do not hand-code a second Forest Edge ruleset, rebuild upgrade formulas, or silently fall back to a default scenario. Preserve terrain, species order, upgrade order, fingerprints, and phase acquisition timing.

Add a minimal simulation-only player entry point with a structured job file, output path, explicit success/failure exit, and no presentation scene dependency. Use the existing Unity version and package lock. The build method must select its own scene list without rewriting the user's normal build settings. Shared code changes must compile in the normal game too.

**Required scope:** Forest Edge/Hare, `bev-experimental`, `opposed-roll`, natural opportunities, ordered launch upgrades including repeated levels, existing experimental overrides, and consecutive phase schedules. Inventory authored upgrades and diagnostic modes: either retain them through the shared path and validate them, or explicitly reject unsupported input. Never silently downgrade a requested mode.

**Pass:** One Linux player completes a bounded experiment with all required reports, and the existing Editor command still works. Fresh serial Editor/player runs match per seed under the parity contract below. Build/startup logs show the worker does not depend on initializing the game UI. Normal compilation and focused simulation/report/continuation regression tests pass.

### G2 - Package and verify one container

**Work:** Build a local image containing the player and its required runtime libraries/content. Pin the base image digest and record the resulting local image ID or registry digest where available. Keep the build input digest, Unity version, scripting backend, platform, package-lock hash, scenario/content hashes, and source identity with it. Rebuild after relevant code or content changes; a cached image must never masquerade as a new skill revision.

Give workers a read-only job mount, their own output mount, an explicit CPU/memory limit, and no runtime network access unless a demonstrated runtime dependency requires it. Run without Editor activation credentials in the worker image. Verify output permissions on the actual Windows/WSL host. Keep activation material out of images, job JSON, reports, and source control.

**Pass:** A container runs a known fixture, exits, and produces host-readable logs and reports. Its scientific output matches the direct player reference. A bad scenario or malformed job fails clearly and cannot write outside its assigned output path. A killed container leaves diagnosable partial evidence.

### G3 - Dispatch a bounded matrix in parallel

**Work:** Add one coordinator command with dry-run, maximum workers, seeds per chunk, batch ID, and output root. Dry-run prints the complete resolved matrix and expected run count without launching Unity or Docker workers. Use a single-controller lock with ownership metadata so two controllers cannot mutate the same batch.

Assign jobs explicitly through the Docker CLI. Validate supported keys/types/ranges and reject unknown scientific options. Keep scientific configuration immutable, and store mutable execution status separately. Use unique experiment/condition/job/attempt IDs and container IDs. Limit concurrent containers and queued work; do not start one Editor or image build per seed.

**Pass:** Two workers demonstrably overlap in time, process disjoint jobs, and produce exactly the expected condition/seed coverage. Serial and parallel runs of the same image have identical scientific outputs. Starting a second coordinator for the same batch is refused without affecting the first. Logs/status are readable by an agent through ordinary commands.

### G4 - Recover failures and aggregate trustworthy results

**Work:** Persist job states such as Pending, Running, Completed, Failed, and Cancelled using atomic replacement. Mark Completed only after the container has exited successfully, required artifacts exist, hashes/identities agree, and report validation passes. Keep the original validation classification, including `VALIDATED_WITH_LIMITATIONS`.

Support status, cancellation, and resume. On restart, reconcile saved container IDs against Docker before launching replacements; a still-running attempt must not be duplicated. Retry failed/interrupted chunks into new attempt directories, with a bounded retry count. Restarting a chunk from its seeds is sufficient; do not add mid-tick checkpointing. Publish at most one validated attempt for each job, retaining all failed-attempt evidence.

Aggregate completed chunks in stable condition/seed order, retaining provenance and existing full stat-line columns. Verify exact expected seed sets, no duplicates, configuration compatibility, and per-row reconciliation. Recompute summaries from raw rows rather than averaging chunk averages. Fail the complete-batch claim if any required job is missing or invalid; a partial preview must say it is partial.

**Pass:** Focused checks cover a worker crash, coordinator restart while a worker lives, timeout, cancellation, a truncated report, hash mismatch, and duplicate/missing rows. Cancellation targets only recorded containers in this batch. Retry/resume reaches complete coverage without duplicate results. Existing raw outputs remain unchanged.

### G5 - Measure throughput and choose a worker limit

**Question:** Does this implementation reduce total time to a validated, equivalent skill sweep on the selected host?

**Proposed fixture:** One existing skill, frozen at G0, Levels 0-10, seeds 1-40 at each level: 440 independent runs. Use Forest Edge/Hare, 200 ticks, a 0.1-second step, `bev-experimental`, opposed-roll combat, and natural opportunities. Record resolved grid dimensions. Retain a separate consecutive-phase parity fixture; do not reinterpret this fresh-run sweep as expedition evidence.

Measure the existing serial Editor workflow and the same player image with 1 and 2 workers; test 4 only if measured memory/CPU headroom permits. Use three repetitions per configuration, rotate order, and record other host workload. Include launch, execution, report validation, and aggregation in elapsed time. Record build/import/image preparation separately, plus the time for the first complete sweep including preparation.

**Primary metric:** Median elapsed time to a complete validated bundle. Report current-workflow speedup and parallel-only speedup separately: `Editor serial / N containers` and `1 container / N containers`. Also report peak memory, failures, and runs per second. Repeated samples are timing replicates, not extra scientific seeds.

**Proposed performance gate:** At least 1.5x warm end-to-end speedup over the current serial workflow, with zero scientific mismatches, duplicate rows, missing rows, or unhandled failures. This target is a draft for Bevin's acceptance, not a promised result. Report the lowest worker count that meets it. If it fails, report the bottleneck and stop claiming a testing speedup; functional operation alone does not satisfy the performance goal.

**Pass:** Measured results and exact commands support the selected worker limit. Then complete one 2,200-run demonstration (Levels 0-10, seeds 1-200) at that limit with full coverage and validation. The frozen fixtures and criteria should be accepted with the implementation assignment under SG-001; ask once for any unresolved material experiment choice before running it.

### G6 - Make operation self-service

**Work:** Deliver one runbook with verified PowerShell commands for prerequisites, building/rebuilding the image, dry-run, a small smoke run, a level sweep, status, logs, cancellation, resume, validation, paired deltas, and opening the final artifacts. State the working directory and real parameter names. A planned command is not a verified command until executed.

Document disk location/retention and cleanup of only named batch containers and disposable build outputs. Include fixes for daemon unavailable, missing build module, stale input image, invalid job, output permission failure, OOM, timeout, partial artifacts, and parity mismatch. Preserve completed evidence by default.

**Pass:** An agent following the runbook can launch a small batch, inspect real progress, cancel/resume it, and locate the validated result without undocumented knowledge. Provide a final handoff with source/build identity, changed files, actual test and benchmark results, known limitations, and Bevin's pending acceptance. Keep publishing and remote-worker activation outside the local build unless separately assigned.

## Input, output, and parity contracts

Use one versioned JSON batch specification. Required information is experiment ID, frozen source/build/content identity, explicit scenario and player species, ticks/step/grid values, modes, seeds, conditions with ordered upgrades, and optional complete phase schedules. Keep worker limits, timeout, chunk size, and output locations as execution settings. The existing queue's nested `parameters` object can be adapted; do not assume its current fields cover the new contract. Reject seed overflow, invalid dimensions, conflicting duration/tick settings, unsupported modes, and paths escaping allowed roots.

Expected outputs per attempt: resolved input, `report.json`, `report.csv`, Hare `statline.csv` when applicable, validator output, `manifest.json`, and process/container logs. Batch outputs add a status manifest, coverage/validation summary, consolidated raw rows, and existing paired-delta/summary formats where applicable. Keep analysis and human acceptance separate from factual reports.

Parity compares canonical scientific values keyed by condition, seed, species, and phase where applicable. Include resolved rules, fingerprints, ordered loadouts, final state or an equivalent complete deterministic state digest, populations, raw counters, phase boundaries, derived metrics, and simulation termination status. Ignore only explicitly listed execution metadata such as timestamps, paths, host/container IDs, and durations. Do not compare whole report hashes across runs whose metadata intentionally differs; retain hashes for integrity.

First require exact scientific equality across repeated runs of the same platform/image at different chunk sizes and worker counts. Also compare Editor and Linux player for fresh-run control, a skill at Levels 1 and 10, and at least one two-phase continuation with a boundary upgrade. Use seeds 1-20 for each fixture. Check the snapshot-to-runtime round trip where export is used. A cross-platform mismatch is a diagnostic result: localize it before adopting the new runner, and never hide it with a broad float tolerance or aggregate-average comparison.

## Concrete failure modes to handle

| Trigger | Required response |
|---|---|
| Build omits current dirty skill changes | Reject the claimed identity; produce an intentional frozen snapshot. |
| Multiple legacy workers copy the same Pending file | Do not scale that consumer; use the single coordinator and its batch lock. |
| Windows process-name cleanup used for concurrent jobs | Retain legacy serial behavior; scope worker cleanup to container IDs. |
| Player import/stripping omits scenario or upgrade content | Fail input resolution and fix the explicit catalog/export; no defaults. |
| Parallel run changes RNG order or splits a continuing world | Keep each seed/expedition sequential and investigate parity failure. |
| Higher worker counts cause memory pressure or slowdowns | Reduce the cap using measurements; container count is not a speedup metric. |

These are implementation acceptance requirements, not a separately accepted Feature Concern Guard ledger.

## Sources and evidence boundaries

- Repository source pointers above were inspected on 2026-09-09; refresh them before implementation.
- [Unity 6.4 headless player documentation](https://docs.unity3d.com/6000.4/Documentation/Manual/desktop-headless-mode.html): a desktop player supports `-batchmode -nographics`. Dedicated Server is a separate build option; networking is not needed for this experiment tool.
- [Unity 6.4 Dedicated Server build documentation](https://docs.unity3d.com/6000.4/Documentation/Manual/dedicated-server-build.html): reference if that target is justified by the build spike.
- [Docker Desktop Windows installation requirements](https://docs.docker.com/desktop/setup/install/windows-install/): verify the actual host and Linux engine setup before choosing resources or installation steps.
- [SG-001](Studio%20Guidelines/AI_GENERATED_REPORTS.md), [SG-002](Studio%20Guidelines/AI_ASSISTED_DEVELOPMENT.md), and [existing remote-worker documentation](CELLSIM_REMOTE_WORKER.md) remain applicable.

No container build, Unity test, benchmark, or simulation was executed while drafting this pack. Proposed Linux viability and speedup remain unverified.
