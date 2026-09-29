# Merge latest Salty roadmap HTML into BevBranch

[Working state](../WORKING_STATE.md) | Status: shared

- Handoff schema: 1
- Handoff ID: 2026-09-28-1123-codex-merge-latest-salty-roadmap-html-into-bevbranch
- Owner: codex
- Branch: BevBranch
- Baseline commit: 1f4c7c8
- Date: 2026-09-28
- Supersedes: none

## Summary

Merged the newest `origin/codex/forest-edge-visual-pass` tip, `6f41fd8`, into
`BevBranch` after first pushing our Forest Edge starting populations and Hare
purchase accounting as `1f4c7c8`.

## Changes

- Salty's new commit refreshes only generated `ROADMAP.html`: the S3-04
  acceptance wording and source SHA-256 footer.
- Updated `WORKING_STATE.md` to mark the earlier Forest Edge and stat-line
  changes as shared on BevBranch.

## Decisions and assumptions

- `origin/ProjectMain` is an ancestor of BevBranch, so this integration targets
  the newer `origin/codex/forest-edge-visual-pass` branch rather than replaying
  older ProjectMain commits.
- The two pre-existing ProjectSettings worktree modifications remain uncommitted.

## Validation

- Fetched current remote refs before both pushes and confirmed the Salty tip
  against `git ls-remote`; the merge had no conflicts.
- `git diff --check` found no whitespace errors. The generated HTML footer's
  SHA-256 matches the Git-normalized `ROADMAP.md` source (`80302844...`).
- No Unity test suite or gameplay run was requested for this documentation-only
  merge. The earlier `1f4c7c8` gameplay changes compiled in the directly
  installed Unity 6000.4.6f1 Editor with zero Console errors.

## Risks and incomplete work

- The 400/55/35 starting ecology and live `ADD=1` Hare purchase behavior
  remain unverified by a gameplay run.

## Next useful step

Run the live Hare purchase check when gameplay validation is requested, then
review the starting ecology separately. Keep the matching Trello card current
with the pushed state and remaining runtime check.
