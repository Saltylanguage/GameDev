# Handoff — Start-menu dismiss input fix

**Date:** 2026-09-05  
**Owner:** Codex  
**Branch:** `codex/simulation-window-production`  
**Status:** Local, uncommitted UI fix

## Observed issue

The full-surface Start-menu dismiss button used the default Noesis Button
template. Holding the pointer down therefore applied the default pressed visual
across the dismiss surface, producing a grey screen. The dismiss path also used
the toggle command, which made the close action ambiguous during the launch
click into another desktop view.

## Fix

- Added `GalapagOS.DismissLayer`, a Button style with a minimal transparent
  template and no pressed-state trigger.
- Added `CloseStartMenuCommand`, which explicitly closes the menu and leaves
  toggle behavior on the Start taskbar button.
- Put the dismiss layer below the taskbar and Start menu with explicit z-order
  values. The Start button remains clickable while the menu is open.

## Verification

- `GlobalResources.xaml` and the desktop XAML parse successfully as XML.
- `git diff --check` passes aside from normal LF/CRLF conversion warnings.
- Unity runtime verification remains pending while the existing Unity editor
  process is open.
