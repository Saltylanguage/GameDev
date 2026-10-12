# Forest Edge confirmation and replay preset pack

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-10-12-1447-codex-forest-edge-confirmation-and-replay-preset-pack
- Owner: codex
- Branch: BevBranch
- Baseline commit: 612e65d5
- Date: 2026-10-12
- Supersedes: none

## Summary

Bevin asked to set up the fresh-seed confirmation and replay recommendations
from the October 12 review. Created five focused confirmation presets and six
small original-seed comparison presets, plus eleven validated saved Workbench
runs. No simulations started; the existing application's Run button remains
the execution step. No application rebuild or production tuning was needed.

## Changes

- Pack and readable loading guide:
  `artifacts/workbench-presets/Forest Edge confirmation 2026-10-12/README.md`.
- Five exact candidate worlds: 48x48 Hare/Fox gestation 4/24 and 8/32;
  64x48 24/8, 4/16 and 8/24. Each uses seeds 10001-10100, all four strategies
  and None/Each one/Each three/Restore toward start purchases: 1,600 runs each,
  8,000 total. All include matched Skip all and No purchases controls.
- Six original-seed worlds cover all seven no-purchase goal successes from the
  reviewed workbook: four strategies per preset, no purchases, one old seed,
  24 automatic replay runs total. Expected endpoints/state digests are saved.
- `prepared-runs.json` maps each preset to its already validated saved-run
  folder under `artifacts/workbench-runs/`; `validation.json` records checks.
- Presets have experiment questions, expectations and research criteria filled
  in; 55 Hares/35 Foxes/400 Plants, closed edges, 100 ticks/round and tick 600.
  Confirmation uses 16 workers and ten-seed chunks for bounded stop/resume work.

## Decisions and assumptions

- Existing comparison axes are Cartesian. Separate presets preserve the exact
  candidate worlds without introducing unrelated cross-combinations or a new
  application feature. There is no multi-preset automatic queue.
- 10001-10100 was outside every existing local Workbench experiment seed range
  at preparation time; no claim is made about external/standalone experiments.
- Uses a byte-identical copy of the reviewed experiment's frozen neutral
  snapshot and pinned runner, source `612e65d5+gestation-controls-working-tree`.
- The export lens of 5 Hares/5 Foxes/100 Plants at tick 600 is recorded for
  confirmation, not an approved production balance target. Existing export
  preferences are separate from simulation presets and must be selected there.

## Validation

- All eleven passed the real Workbench LoadPreset, Validate, RunCount, Prepare,
  LoadRun and runner Preflight methods. The helper used the exact DLL embedded
  byte-for-byte in the installed EXE; no mock preset parser was substituted.
- Independent readback: all eight frozen hashes verified per saved run, 104
  resolved conditions matched historical executed arguments, snapshot/runner
  hashes matched the source, all expected counts/seeds matched, and none had a
  `sweep/batch` directory. Original frozen inputs rechecked unchanged.
- Verification helpers and generation scripts are under
  `artifacts/results-review-20261012/`; they launch no simulation command.

## Risks and incomplete work

- Local, uncommitted and unpushed; artifact presets and saved runs are local.
- No new simulation, interactive Unity replay or result claim. Application
  methods were exercised; existing user windows/selections were untouched.
- The first verification harness needed fixes for the single-file EXE assembly
  location, PowerShell reflection argument wrapping and a nonpublic property.
  One prepared folder was retained and reused. Final checks pass for all eleven.
- Longer-horizon follow-up remains deferred until confirmation review. Changing
  ticks/round would also change decision timing and needs its own protocol.

## Next useful step

Use the pack README to load a preset and Validate, or Open saved run to load its
already frozen folder, then deliberately click Run. Review fresh-seed results
before approving tuning or preparing longer-horizon work.
