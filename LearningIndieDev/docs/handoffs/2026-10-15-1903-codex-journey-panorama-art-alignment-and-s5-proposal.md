# journey panorama art alignment and S5 proposal

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-10-15-1903-codex-journey-panorama-art-alignment-and-s5-proposal
- Owner: Codex
- Branch: ProjectMain
- Baseline commit: 4d3d7734
- Date: 2026-10-15
- Supersedes: none

## Summary

Published the scrollable journey map on `ProjectMain` at `4d3d7734`, then
aligned its scroll surface to Chrono's 5,760 × 1,080 art brief. Prepared a
proposed S5 outcome centered on Forest Edge simulation and upgrade content,
while the S4 Trello closeout decision remains pending.

## Changes

- Changed the map's logical width from 3133.3333 to 3840, keeping the 720
  logical height and normalized node positions. At the project's 1.5 scale,
  the new surface matches 5,760 × 1,080 artwork without horizontal distortion.
- Updated `docs/JOURNEY_PROTOTYPE.md` with the final-art pixel coordinate
  conversion and temporary-placeholder behavior.
- Drafted `docs/Sprints/S5-proposal-2026-10-15.md` for the next simulation and
  upgrade work block. It is a proposal, not an activated sprint.
- Reconciled `ROADMAP.md` v2.3 and `docs/PROJECT_CONTEXT.md` with Josh's new
  S5 priority while leaving S4 Active and S5 Proposed pending Trello decisions.

## Decisions and assumptions

- The existing Art HQ card specifies 5,760 × 1,080. Until Josh responds to
  the size clarification, that art brief is the working canvas contract. The
  current 4,700 × 1,080 placeholder is intentionally stretched and will be
  replaced by Chrono's work.
- The three opening routes, graph topology, gameplay availability, and
  authored node positions are unchanged. Later nodes still await content.

## Validation

- Before resizing, Clean Editor journey graph 1/1, Clean journey PlayMode 5/5,
  and focused visual acceptance 1/1 passed; see the published map handoff.
- A Windows `StandaloneWindows64` player build at `4d3d7734` completed with
  Unity's `Build Finished, Result: Success` verdict. Output:
  `artifacts/windows-profile-journey-20261015/LearningIndieDev.exe`.
  This was a dirty-tree build because unrelated Unity importer/settings files
  were present; the source map code was committed.
- After resizing, Clean 1920 × 1080 visual acceptance passed 1/1
  (`artifacts/visual-evidence-20261015-190208/PlayMode-results.xml`). Twelve
  screenshots were retained in
  `artifacts/journey-map-5760-contract-20261015/`; opening, middle, right,
  and hover images were inspected.
- Scoped `git diff --check` passed for the map alignment and proposal files.

## Risks and incomplete work

- The hidden player launch reached engine initialization in its log, but its
  waiting shell did not return a trustworthy exit verdict; it was interrupted.
  This is not an application exit/relaunch profile smoke or a P1-032 acceptance
  claim.
- Sim's launch-input review, Josh's map visual approval, Chrono's final art,
  the permanent-wallet and journey-settlement content contract, and S4 closeout
  are still open.

## Next useful step

Apply Josh's canvas-size and S4-closeout decisions. If 5,760 is accepted,
give Chrono the pixel placement formula in `JOURNEY_PROTOTYPE.md`. Keep S5
Proposed until its own kickoff is confirmed and verified.
