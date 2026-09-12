# current baseline before genome

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Owner: Codex
- Branch: codex/simulation-window-production
- Baseline commit: 39edc7ad
- Date: 2026-09-12

## Summary

This checkpoint packages the current GalapagOS/UI, project-context, and
diagnostics-tooling work before the first Genome contract begins. The player
home is now documented as the GalapagOS Desktop, while the standalone Lab and
the Genome UI remain explicitly bounded and representative.

## Changes

- Polished the Main Menu, GalapagOS Desktop, simulation shell, Lab/Expedition
  surfaces, focus and transition behavior, and their Play Mode coverage.
- Added the project-owned Noesis image-resource dictionary and the authored
  Main Menu background; concept images remain reference material only.
- Updated the roadmap, architecture/context documents, upgrade direction,
  Studio guideline index, and Sprint 2/Sprint 3 planning records.
- Added the artifact-retention, artifact-cleanup, Main Menu, and roadmap-v2
  handoff notes, plus the artifact-summarization and DirtyBoy project skills.

## Decisions and assumptions

- The first Genome contract is planned for S4; persistence-backed implementation
  remains an S6 slice. Genome runtime behavior is not included in this
  checkpoint.
- The next Genome slice must keep permanent unlocks separate from active
  per-species configuration, use the agreed 8-point capacity, freeze active
  Genomes at launch, and apply them by stable species identity to all matching
  populations.
- Named Genome loadouts and Species Mastery remain deferred/non-gating.
- Main Menu branding/generated art and the new concept art still require human
  review before promotion to final production direction.

## Validation

- `powershell -NoProfile -ExecutionPolicy Bypass -File
  LearningIndieDev/tools/Test-StudioPolicy.ps1` passed: 6 guidelines, 6 rules,
  alert-only mode.
- `git diff --check` was clean before staging the new files. Git reports normal
  LF-to-CRLF conversion warnings; Unity-generated `.meta` files and one
  handoff header retain standard trailing blank-field/Markdown whitespace.
- The current Unity batch test attempt did not run because Unity was already
  open (PID 4760); no new Unity result is claimed here.
- Earlier recorded evidence remains 212/212 EditMode, 21/22 general PlayMode
  with one intentional skip, and 22/22 graphics acceptance, as described in
  the dated handoff notes. Those results were not rerun in this checkpoint.

## Risks and incomplete work

- The Main Menu handoff remains Needs Review, and the current Genome profile,
  launch, persistence, economy, and Biome contracts are not implemented.
- The workspace DirtyBoy script was attempted but could not complete: normal
  execution was blocked by PowerShell policy, and the bypass run encountered a
  path-quoting bug while scanning changed files. Manual Git status and diff
  checks were used instead.
- The branch still needs the final handoff commit and push before it is shared.

## Next useful step

After human review of this baseline, begin the Genome contract slice with the
profile/launch snapshot boundary and focused EditMode tests. Do not start by
wiring the representative Gene Lab toggle into runtime state.
