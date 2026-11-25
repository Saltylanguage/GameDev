# Rabbit and M2 integration

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-11-24-1105-codex-rabbit-and-m2-integration
- Owner: Codex
- Branch: codex/rabbit-identity-rename
- Baseline commit: c99b1141
- Date: 2026-11-24
- Supersedes: none

## Summary

The regular checkout moved to `codex/rabbit-identity-rename` from Bev-Ticket-3.
The branch already contained `origin/ProjectMain` at `663b5c2a`, including
GLJP and the three map tickets. This local integration adds the five commits
from `origin/codex/m2-species-scenario-select` through `7bb56547`.

## Changes

- Merged the authored Rabbit launch introduction, Desktop audio listener,
  M2 review adjudications, developer-only Field Data test-route button, and
  Reduced Motion explanation. The explicit Expedition Planner launch skips
  the introductory screen and retains its chosen test species and scenario.
- Refreshed `ROADMAP.html` from source. M2's current visual review target
  is 1920×1080.
- Fixed two issues found by Unity. Population counting visits playable
  cells on shaped maps. The Desktop Genome catalog points to the Fox Genome
  map asset GUID. The Fox folder and asset both retain their original GUIDs.

## Decisions and assumptions

- The Forest Edge irregular-layout playtest edit stays in the verified local
  stash `20a3012689ffa261462819c878c2811422314b4c`. The branch uses its
  committed 34×19 wrapped Forest Edge with 400 Plants, 3 Rabbits, and 2 Foxes.
  Rabbit's authored cap is 15.
- Tracked-creature inspection is already implemented. Keeping individuals,
  histories, and lineage across simulation nodes is still the design target.

## Validation

- Roadmap generation check, XAML parse and silhouette references, and both
  portable Release builds passed. The Batch self-test passed.
- The first Live run exposed the shaped-map exception and invalid Fox catalog
  reference. After both fixes, EditMode passed 826, failed 4, and skipped 1.
  PlayMode passed 92, failed 21, and skipped 2 before the final test fixture
  adjustments. The full suite still needs reconciliation with the Rabbit cap,
  current scenario settings, and Gene Lab catalog. Results are retained in
  `artifacts/unity-tests-20261124-110936` and
  `artifacts/unity-tests-20261124-110713`.
- Fixed-shape and detached-island PlayMode passed, including tracking the
  same animal across an island seam. The Desktop listener, Rabbit
  introduction, and authored seed-10101 Field Data first shop and next-node
  reset passed in the broad run.
- Final focused checks passed for the developer-only route button, Owl launch
  through the Five Species Sandbox, and the 1920×1080 Desktop through
  Simulation capture. Results are in `artifacts/unity-tests-20261124-111248`,
  `artifacts/unity-tests-20261124-111257`, and
  `artifacts/visual-evidence-20261124-111358`.
- The handoff validator still reports older metadata and unavailable
  archives. It reported no error on this handoff.

## Risks and incomplete work

- M2 completion remains open. The current authored seed passed the Field
  Data first-shop check. A complete normal-player Journey, natural permanent
  progression, relaunch, and replay still need evidence.
- GitHub [PR #8](https://github.com/Saltylanguage/GameDev/pull/8) is a draft
  against `codex/s6-predator-first`. This integration is local.

## Next useful step

Review the authored opening, then prove a complete run and an earned Gene Lab
unlock that works after a real relaunch. Use the
[Track A plan](2026-11-17-0741-codex-m2-vertical-slice-meeting-tracks.md) to
assign the remaining technical foundations and the
[M2 comparison](../Reviews/M2-Comparison-20261118.md) for human retests.
