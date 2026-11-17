# M2 selection and Desktop audio correction

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-11-16-1939-codex-m2-selection-audio-listener
- Owner: Codex
- Branch: codex/m2-species-scenario-select
- Baseline commit: fda77319
- Date: 2026-11-16
- Supersedes: none

## Summary

This isolated branch prepares two corrections found during M2 validation.
Desktop Simulation now presents a constrained species/scenario screen before
the Journey map. The Desktop also retains an audio listener after the Main
Menu's temporary chime listener is destroyed. Neither correction is in the
pinned fda77319 independent-review checkout.

## Changes

- The new Noesis screen selects Rabbit/Hare and Forest Edge, shows Fox, Deer,
  and Wolf as locked silhouettes using existing art, explains Fox pressure,
  and offers Back to Desktop or Continue to Journey. The existing Journey,
  first reward, and first simulation sequence remains domain-owned.
- The Desktop host opens this screen only for the canonical Forest Edge/Hare
  setup. Developer setups using another scenario or species keep the existing
  entry behavior; no new species or scenario becomes playable.
- The Desktop host waits for the Main Menu chime listener to finish, then
  enables its own listener. That listener stays active when the view switches
  from Desktop to Simulation.
- Updated the roadmap, product brief, and project context with the requested
  launch flow.

## Decisions and assumptions

- The fixed locked-card roster is a visual M2 presentation of currently
  unavailable animal art. Profile-backed, dynamic cards are deferred until
  another player species is genuinely playable.
- No balance, unlock, or save contract changed. Art remains a review direction,
  not a final art approval.
- The pinned review baseline and private reviewer answers were not modified by
  this branch. Reviewers must compare answers from the same pinned revision
  before retesting changes here.

## Validation

- Clean PlayMode audio-listener test: 1/1 passed,
  artifacts/unity-tests-20261116-193341. It covers the real Main Menu
  transition, the end of the chime, one active listener, and the Simulation bed.
- Clean PlayMode journey report and reopen tests: 1/1 each,
  artifacts/unity-tests-20261116-193708 and
  artifacts/unity-tests-20261116-193755.
- Clean visual capture test: 1/1 at 1920×1080 and 1/1 at 1280×720,
  artifacts/visual-evidence-20261116-193508 and
  artifacts/visual-evidence-20261116-193602. The selection captures were
  inspected at both sizes, including silhouette masks and button labels; the
  test exercises Back, reopen, Continue, Journey, and first reward.
- A focused Fox developer-setup PlayMode test passed 1/1 at
  artifacts/unity-tests-20261116-194427, confirming that the canonical
  Rabbit selection is not shown for that setup.
- XAML parsed as XML and git diff --check found no whitespace errors.

## Risks and incomplete work

- Human listening and visual acceptance of this new branch remain open.
  Automated tests establish listener/source presence, not perceived sound.
- The earlier independent baseline displayed repeated Unity Console warnings
  that no AudioListener was present. The isolated fix needs a fresh manual
  listening check after the reviewers finish their baseline comparison.
- A developer-only scenario asset at index 1 contains an empty species
  definition; a first attempt to test that route failed in scenario setup.
  The valid Fox developer setup was used for the passing fallback check.
- Unity's clean run touched three ProjectSettings files only by line-ending
  conversion. They are incidental and must not be included in the commit.

## Next useful step

Finish independent reviewer responses on fda77319, compare by question ID,
then review this branch and retest the corrected Q2 and audio routes. Record
human acceptance separately; keep the M2 outcome rows open until their
individual requirements pass.
