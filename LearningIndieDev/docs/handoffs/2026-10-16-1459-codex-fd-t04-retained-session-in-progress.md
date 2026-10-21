# FD-T04 retained session implementation — in progress

**Date:** 2026-10-16 14:59 America/New_York  
**Branch:** `BevBranch`  
**Ticket:** [FD-T04](https://trello.com/c/LKzm9j7r/122-fd-t04-run-all-18-rounds-in-one-retained-journey-session)

## Implemented so far

- Added `FieldDataJourneySession` around one existing `SimulationManager`, its
  prepared `SpeciesSimulationRunner` and `SimulationRunState`, the approved
  definition snapshot, and one retained `SpeciesProgression` reference.
- The session accepts only a ready continuous run configured for 100-tick
  rounds and 1,800 total ticks. It transitions through `Running`,
  `RoundDecision`, `Shop`, `Succeeded`, `Failed` and `Closed`.
- Manager boundary events pause the flow at every 100 ticks and label 600 and
  1,200 as shops. Explicit `Continue` resumes one boundary exactly once;
  decision states themselves never advance the clock.
- `SpeciesSimulationPreview.TryConfigureFieldDataJourney` provides an explicit
  pre-run opt-in for a resolved definition, matching the selected scenario and
  player species. It prepares 18 rounds and binds the session to the preview's
  existing manager (also through `Helper_Simulation`). The preview's `Update`
  remains the sole clock forwarder. In this mode its legacy boundary and
  completion reward handlers are bypassed, and `ContinueWithoutUpgrade` routes
  decision/shop continuation to the retained session. Helper rebinding is
  rejected while that session is attached.
- Completion succeeds only at the target tick with a living selected species.
  Early completion and explicit abandonment fail. The session checks both
  runner and run identity before transitions and unsubscribes on disposal.
- Added Editor fixtures covering the 18-round path, decision/shop pauses,
  progress history, same-run and progression references, no time advancement
  while paused, wrong run configuration, abandonment, disposal and runner
  replacement.

## Validation and blocker

The first focused Unity test call did not discover the new test because its
hand-written `.meta` files lacked their final newline; that metadata issue was
fixed. The next batch compile found four static-constant qualification errors;
those were fixed. A following Unity attempt was blocked before launch because
`Temp/UnityLockfile` remained while Unity Pipeline was unreachable. The lock
was created by the failed test attempt at 2026-10-16 14:55:32; no Unity Editor
process is running. Per project tooling guidance, the lock was not removed.

Using the Unity installation's C# compiler and the generated Bee response
files, the `SaltyGame.Runtime` and `SaltyGame.Editor.Tests` assemblies both
compiled successfully after the fixes (existing obsolete API warnings only).
That does not count as Unity test execution. No test count is claimed.

## Remaining before FD-T04 can close

- Run `FieldDataJourneySessionTests` in Unity and fix any failures.
- Complete and verify explicit selected-mode composition with
  `SpeciesSimulationPreview`/`Helper_Simulation` in Unity. Source composition
  is now present, but the preview and UI lifecycle have not been runtime tested.
- Verify terminal tick-1,800 completion, lifecycle cleanup/rebind behavior and
  legacy route compatibility, then close criteria only with the corresponding
  evidence.

FD-T04 remains In Progress; all acceptance boxes remain open. No commit or
push was made. Existing local work, including the ten unrelated terrain
metadata edits, was preserved.
