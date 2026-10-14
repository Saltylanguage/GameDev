# Optional wrapping grid: S4 development screen

Experiment ID: S4-03-20261004-wrap-400-20-10. Owners: Bevin and Salty. Status: implementation and local development screen complete; visual review and default-boundary decision pending.

Bevin proposed connecting opposite edges because Hares appeared trapped against the border, and authorized implementing an Inspector toggle. The hypothesis was that extra escape routes could help Hares. This is a topology comparison with unchanged skill values, not a balance acceptance test.

Read [observations](REPORT.md) and [separate interpretation](ANALYSIS.md), then the [handoff](../../../handoffs/2026-10-04-0022-codex-s4-wrapping-grid-experiment.md). The earlier [13-arm screen](../S4-03-Hare-Paired-Seed-Screen/README.md) remains its own evidence record.

## Method and evidence

- Forest Edge, 36x20, 400 Plants / 20 Hares / 10 Foxes, 0.1-second steps, six 100-tick phases, 600-tick horizon. Natural opposed-roll combat; Bev experimental features enabled; coupled responses off; no additional Genome allocation or reinforcements.
- Seeds 10100-10119, matched between bounded and wrapping grids. Each arm starts a fresh world, then carries the evolved world through phases; Hare extinction ends a run early.
- Two paths per topology: skip all choices, or acquire General Movement at tick 100 and Threat Avoidance at tick 200 if still alive. Later choices skipped. These are L1 pairs, not full five-pick builds.
- No thresholds for accepting wrapping as the default were approved. These reused development seeds do not constitute independent confirmation. No production scenario or species asset changed.
- Local ignored evidence: `artifacts/s4-wrap-screen-20261004-400-20-10/`. Includes four raw JSON/CSV reports, requests, execution records, source diff/SHA256, authored-asset hashes, final/phase slashlines with validity statuses, paired deltas, six focused assertion checks and one wrapped replay.
- Reproduce on the recorded source: use the installed Unity CLI `command cellsim_run --request_path <arm/request.json> --project-path <project> --timeout 120 --json` sequentially. The only topology argument is `-wrapEdges true` or `false`; omission defaults to false. Preserve old raw outputs; change output paths for another batch. The usual CellSim shell wrapper's previously recorded missing live-lane helpers remain a separate issue.
- Report schema 32 adds `wrapEdges` to JSON/CSV. Data fingerprint v11 includes topology; earlier fingerprint strings change even when bounded behavior is unchanged. `analyze.py` preserves process-independent result fields when comparing prior bounded runs, excluding only `entityId` and the versioned `rulesetFingerprint`.

## Human decision

Authorized: optional wrapping implementation and Inspector control. Pending: visual acceptance and whether any authored scenario should use wrapping. Bounded remains the default. No approval of revised skill values, whole-ecosystem balance, or Salty's separate journey/DNA work is implied.

## October 4 visual follow-up

Bevin reported that wrapping feels much better, despite the continuing failures,
and authorized searching a large set of starting populations with it enabled.
This closes the initial visual preference check; it does not validate ecology
balance or change the authored boundary default. See the separate
[population sweep](../S4-03-Wrapped-Population-Sweep/README.md). The original
80-run observations above remain unchanged.
