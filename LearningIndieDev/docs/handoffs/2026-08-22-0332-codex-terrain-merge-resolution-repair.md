# Terrain merge resolution repair

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Owner: Codex
- Branch: codex/cellular-sprite-tiling
- Baseline commit: 45ca678b
- Date: 2026-08-22

## Summary

The branch was not inside an active Git merge: both merge commits had already
been completed and pushed. The remaining problem was a broken conflict
resolution in the terrain resolver tests. This forward repair preserves the
shared merge history and restores tests to the current dual-grid terrain API.

## Changes

- Replaced the deleted terrain-sheet assertion and removed calls to the retired
  cardinal-neighbor resolver API.
- Validate that every named Grass and Desert terrain texture can be loaded.
- Restored the missing test attribute on the invalid-corner-mask regression.

## Decisions and assumptions

- The 15-variant diagonal-aware resolver is authoritative. Do not restore the
  superseded cardinal-only compatibility API.
- Named source textures intentionally vary in dimensions; existence is the
  relevant contract because the preview stretches them into its display area.
- Repair the pushed merge with a new commit rather than rewriting shared history.

## Validation

- Unity MCP command compiled and executed successfully. It verified corner bits
  `1/2/4/8` and loaded all 30 named Grass/Desert textures.
- Unity Console contained zero errors, exceptions, or failed assertions after
  the repair.
- `git diff --check` found no whitespace errors (only the repository's expected
  LF-to-CRLF conversion warning).
- The full Unity Edit Mode suite was not run because the project was open in the
  Editor; the focused test was validated through the live Editor command above.

## Risks and incomplete work

- Source terrain dimensions are not standardized despite the folder name. No
  runtime defect was observed because the current preview uses stretch-to-fill.

## Next useful step

Review the focused test diff and run the full Edit Mode suite during the next
closed-Editor validation window.
