# BevBranch Salty gestation integration and testing impact

[Working state](../WORKING_STATE.md) | Status: shared

- Handoff schema: 1
- Handoff ID: 2026-10-10-2004-codex-bevbranch-salty-gestation-integration-and-testing-impact
- Owner: Codex
- Branch: BevBranch
- Baseline commit: 6ee2c799
- Date: 2026-10-10
- Supersedes: none

## Summary

Pushed our dropdown and comparison-preview fixes, merged Salty's latest animal
gestation checkpoint into BevBranch, and repaired a portable-snapshot integration
gap found during review. Gestation changes population timing and requires fresh
snapshots and matched runs before drawing balance conclusions.

## Changes

- Our Workbench fixes: `a82d1cd47578ea40591e9651d76dc3e36bb8a71f`, pushed to
  `origin/BevBranch` before merging.
- Incoming branch: `origin/codex/s4-gestation-checkpoint-20261010`, tip
  `65e47386c3769143c5cb89bae2fd59a8facfbd74`. Its two commits add animal gestation
  (`cea3c278`) and document an exploratory S4 baseline pilot (`65e47386`).
- Merge: `10c13f4`. The only conflict was WORKING_STATE.md; both contributors'
  current-focus entries were retained. Production code merged without conflicts.
- Integration repair: `6ee2c799`. Shared `CellSimPortable.RuleData` now saves and
  restores `gestationTicks`. Before the repair, a 48-tick snapshot failed its
  round-trip fingerprint check. A runner self-test reproduces that failure and
  verifies the repair and preservation through an unrelated vision override.

- Parity repair: `e36ef25` with Unity assembly-boundary follow-up `5514e24`.
  The prior reflective checksum reduced the internal gestation struct to its
  type name, omitting ticks, litter size, mate identity, and reserved offspring
  energy. It now includes all four and normalizes mate identity to live grid
  position. Regression checks fail before the repair and pass afterward,
  including invariance when process-wide entity labels change.

## Decisions and assumptions

- Production Hare and Fox both use a provisional **48 simulation ticks** of
  gestation. The constructor's zero default retains immediate reproduction for
  rules that explicitly use zero. Plants keep their existing reproduction path.
- A successful conception charges mating energy and starts a cooldown. The
  pending litter follows the carrying parent through movement. Birth happens
  after the timer, provided enough cells are free for the whole conceived litter.
  If the carrying parent dies, its pending litter disappears. The conceived
  litter is already limited by available space and energy at conception.
- Successful reproduction attempts are recorded at conception; births and birth
  events are recorded at delivery. Those counts can diverge across phase
  boundaries, the end of a run, or parent deaths. Treat them as separate stages.
- The new birth pass runs after movement, starvation, seed drops, and wilt.
  Plants/food, available space, and parent survival therefore matter during the
  wait. At 100 ticks per round, 48 ticks is nearly half a round.
- The ruleset fingerprint advances from v10 to v11 and includes gestation. The
  worker source hash changes too. Do not edit historical fingerprints/source
  hashes to make old snapshots pass a current runner.
- The completed 77,760-run Forest-Edge-Review--Hare-Vision--Fox-Vision--Map batch
  predates gestation. Its frozen runner and original evidence remain unchanged.
  It is valid evidence for the earlier model, not a current 48-tick baseline.
- Workbench and worksheet formats remain intact. Gestation is preserved in the
  frozen snapshot, but the current comparison dropdown/stat registry do not yet
  expose Hare/Fox gestation controls. No new GUI controls were added in this merge.
- The checkpoint's Trailblazer/Warren/Gardeners descriptions clarify design
  intent. Burrows, stronger crowding rules, and per-movement energy charges are
  proposed follow-ups, not mechanics implemented by this branch.

## Validation

- Standalone runner and Workbench Release builds: passed, zero warnings/errors.
- Runner self-test: failed before the serializer repair with a snapshot
  fingerprint mismatch; passed afterward, including gestation round-trip,
  override preservation, deterministic compact/detailed runs, invalid-input
  rejection, and the existing eight purchase-policy checks.
- Workbench native self-check: **14 groups passed**, including the oversized third
  comparison recovery and dropdown readability checks.
- Worksheet regressions: **21 tests passed**.
- Unity EditMode: **323 passed, zero failed/skipped** on the merged checkout.
- Unity PlayMode: **40 passed, zero failed, two graphics skips** with `-nographics`.
  A focused graphics-enabled run then passed **both skipped sprite tests**.
- After the final checksum correction, Unity export succeeded and all **323
  EditMode tests passed again**, zero failed/skipped. A fresh snapshot confirms
  Hare=48, Fox=48, Plant=0 gestation ticks.
- Unity/standalone parity: **three seeds (10101-10103), 200 ticks each**, matching
  final state digest, counters, choices and timing; derived report rates agree
  within the existing 1e-6 tolerance. These runs exercised Fox births on all
  three seeds and Hare births on 10102. This is a bounded compatibility check,
  not a balance screen.
- Final fresh snapshot: `artifacts/cellsim-gestation-merge-check/verified/snapshot.json`.
  Source commit `5514e2411d6fe933a19a870644505df1fa9eb90f`, source hash
  `1c594f1122fc9f301a5bbef28daa93dda6b82926aa7712734928d9f563f8778b`.
  The initial export and initial worker outputs were retained separately;
  validation did not overwrite frozen evidence.
- Validation caught and corrected an intermediate checksum compile error caused
  by direct access to an internal runtime struct from the Editor assembly. No
  runtime API was widened. The final regression checks pass in .NET.
- The first neutral export command incorrectly included a phase length without
  a phase schedule. Validation rejected it before writing a snapshot; the neutral
  export was rerun with that argument removed.
- Local validation artifacts: `artifacts/cellsim-gestation-merge-check/`.
- `CellSim.ps1 -Command Doctor` rejected idle CLI MCP processes as running Unity;
  `UnityTooling.ps1` also lacks `Resolve-UnityExecutionLane`, which the current
  test wrapper calls. Direct Unity CLI is used for validation. These are existing
  tooling issues, also identified in Salty's pilot; this merge does not rewrite
  the execution wrappers or close those MCP processes.

## Testing impact and next useful step

1. Establish a fresh gestation baseline with the same map, populations, policies,
   seeds, and horizon as each intended comparison. Earlier successful tuning
   combinations may no longer have the same outcomes.
2. Add independent Hare and Fox gestation options to the existing shared stat
   registry, portable mapping, Workbench comparison catalogue, and export
   provenance. A small first screen could compare 0/24/48/96 ticks while holding
   vision and plant energy fixed. Those are proposed diagnostic values, not
   approved tuning. This task preserves the new authored rule without adding
   that separate interface feature.
3. Measure first-birth timing, extinction tick, final populations, and reach of
   the five choice windows alongside total births. Inspect deaths during the
   gestation interval. Pending-litter/conception-loss telemetry is not currently
   exposed as a dedicated workbook metric; do not infer its count from births.
4. Recheck Gardeners' renewal timing: a population can die before the tick-200
   Seed Dispersal choice becomes observable. Salty's five-seed pilot reports
   limited choice reach at 400/25/15 and more seed-drop activity at 400/20/10, with
   frequent extinction in both fixtures. The pilot is exploratory; raw reports
   are local to Salty's checkout and were not inspected here.
5. Restart the Workbench through `Open-CellSim-Workbench.cmd` to rebuild its
   current runner before selecting a new gestation snapshot. Existing prepared
   runs keep their own pinned runner. The running Workbench window was not closed
   or overwritten during this task.

## Risks and incomplete work

- No new balance matrix, manual player acceptance, or gameplay build acceptance
  is claimed by this integration check.
- The 48-tick value is provisional; do not label it balanced from passing tests.
- The existing 10,000-condition and 1,000,000-run budgets remain unchanged.
- Trello follow-up is required by the collaboration workflow but no Trello
  connector is available in this session; no board update is claimed.
