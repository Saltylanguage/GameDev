# Hare purchase confirmation launch

October 8, 2026. Codex / GPT-6. Branch BevBranch; HEAD e7a6c602. Existing dirty
simulation/tooling work was preserved. Bevin approved the recommended first step
with "okay lets begin step by step". No production values changed in this step.

## Run and frozen scope

Root: `artifacts/cellsim-hare-confirmation-20261008-143725/`.
Launch: 18:37:36 UTC / 14:37:36 EDT; hidden PowerShell pipeline PID 62072.
Batch identity: `036565a25ed21c1853ad35fec8b3ee8c2474b8dc2114c21b0e29d65e3f067087`.

Four candidates (D4/S19, D4/S14, C2/S14, D5/S25), four paths (Skip all,
Trailblazer, Warren, Gardeners), six policies (none, late-five, each-one,
each-three, each-five, restore-toward-start), 1,000 matched seeds 60000-60999.
96,000 runs, 16 workers, 50-seed chunks, 1,920 chunks, horizon 600. All purchase,
wallet, placement and Mutation rules remain those of the completed screen.

Preparation verified the screen raw hash, all 96 exact argument sets, identical
snapshot and all copied runtime files; reran the portable self-test and dry-run.
Snapshot SHA256: `38bda62130a25b257434c3f4e685ca22e745b5f967444137db9d2cd82d954b01`.
Assembly SHA256: `50178631910b6d85d1705aed7b648f548f61395bec5c0582f4797c210159cb3f`.
Source hash: `8e61009504e4610c77a37132f47f8ef75da15333dd678a029b1bacc0310fe3b1`.
Retained screen validation includes eight Unity references, sixteen historical
controls, 128-arm smoke and two EditMode tests. Those were not rerun; identical
build reuse is the basis for their relevance. `validation/checks.json` records
fresh and retained evidence separately. Launcher checks frozen hashes before
running; pipeline/stdout/stderr/status files provide live progress.

First checkpoint: 16/1,920 validated chunks (800 runs), 16 active workers,
107 seconds elapsed, stderr empty. This is a launch checkpoint, not completion.
At screen throughput, approximately five hours plus analysis; actual duration
depends on run lengths and host load. No scheduler notification was created.

## Analysis and existing-data diagnostic

Automatic analysis compares purchases to no purchases on the same path, and
Mutation paths to Skip all under the same purchase policy. Retains all seeds,
paired uncertainty, applicability, observed durations, APS/AHS and spending.
Purchase report header now derives candidate/path/policy counts instead of
assuming eight policies. Expected window audit: 480,000 assigned rows.

`screen-diagnostics/` holds an analysis of the previous screen, not new results:
D5/S25 and C2/S14 Gardeners with none, late-five, restoration and each-five;
800 whole-run observations plus reached phases. Component sums reconcile to
APS/AHS within 1e-6. Whole-run comparisons retain all 100 assigned seeds/arm;
phase deltas require matching observed start/end ticks and report missing or
different windows. CSVs preserve both score terms and ecological counters.

D5/S25 Gardeners each-five versus none: APS delta -0.1050, consisting of RFS
-0.1280, predAVG +0.0001, starvation term +0.0230, crowding zero. AHS delta
-0.0566, predominantly Fox starvation term -0.0504. C2/S14 restoration shows
APS +0.0839 despite the earlier lower Hare/all-species survival, and AHS -0.0692.
Scores and survival must therefore remain separate criteria. Different whole-run
durations occurred in 70/100 D5 each-five pairs and 61/100 C2 restoration pairs;
these decompositions identify arithmetic contributions, not isolated causality.
Only 30 D5 and 24 C2 each-five pairs have common full phase-6 windows. Individual
purchased-Hare lineage was not tracked.

## Continue

Inspect the actual pipeline and batch status when checking completion. Success
requires the batch, paired analysis and purchase reports, plus frozen hashes and
96 x 1,000 run counts; review results before selecting new mechanics.
If interrupted, use `pwsh -NoProfile -File <root>/run.ps1 -RunRoot <root> -Resume`
with hidden launch and output logs after inspecting the recorded failure.
The approved protocol and preparer live in
`docs/Research/Experiments/S4-04-Hare-Purchase-Confirmation/`.
Threshold/cap changes, gameplay examples, a tick-700 tail and production tuning
remain proposed subsequent steps. No commit, push or external board update.
