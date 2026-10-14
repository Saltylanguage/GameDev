# profile persistence foundation

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-10-14-1424-codex-profile-persistence-foundation
- Owner: Codex
- Branch: ProjectMain
- Baseline commit: fea55926
- Date: 2026-10-14
- Supersedes: none

## Summary

Local profile persistence foundation for the Main Menu, canonical Desktop, and
standalone Lab/Simulation route. A player can create and select multiple
profiles; each carries its own currency balance, progression IDs, simulation
time, and active playtime across scene loads and restarts.

## Changes

- Versioned the existing PlayerPrefs JSON store. Existing unversioned profiles
  load with their Genome node state and initial Hare species access. Unreadable
  stores are copied to a recovery key, and partially malformed entries raise a
  visible Main Menu warning while valid profiles continue to load.
- Added profile currency add/spend operations, species and whole-Genome unlock
  IDs, and retained per-species unlocked/active Genome node IDs. This is a
  storage skeleton; purchase and completion rules are not wired yet.
- Count cumulative simulation ticks and focused, unpaused wall-clock playtime.
  The profile helper saves time in 30-second intervals and on scene teardown,
  focus loss, or pause. Profile launches on the canonical Desktop now apply the
  selected profile's frozen Genome configuration before Simulation initializes.
- Kept direct scene opens as developer previews, without crediting a profile.
  Missing-profile Simulation launches return to Profile Selection. Added
  focused storage, Main Menu/Desktop, and Lab PlayMode coverage. Profile tests
  now preserve any pre-existing Editor PlayerPrefs save around each test.

## Decisions and assumptions

- `SimulationTicks` is the cumulative in-game time unit. No game calendar or
  tick-to-year conversion is assumed. `HoursPlayed` is derived from persisted
  active wall-clock milliseconds.
- `Currency` is a generic balance for now. Research, Rabbit, and other data
  categories may require a wallet migration once the economy is approved.
- Profile unlock IDs preserve existing Genome state. They do not imply a
  purchase, biome-completion, or reset rule.

## Validation

- Unity EditMode `GenomeContractTests`: 11/11 passed, including multi-profile
  isolation, reload, legacy migration, and whole/partial corrupt-store recovery.
- Unity PlayMode `MainMenuPlayModeTests`: 7/7 passed, including selected-profile
  Desktop launch, simulation tick persistence, playtime reload, missing
  Simulation profile recovery, and a visible partial-save warning.
- Unity PlayMode `LabPlayModeTests`: 8/8 passed. Focused Desktop direct-open
  check: 1/1 passed before the final recovery and test-fixture changes.
- `git diff --check`: passed (Git emitted only line-ending conversion warnings
  for unrelated existing files).
- The repository handoff validator reports 11 pre-existing errors in older
  notes (missing historical artifacts and unsupported old status text); it
  reports no error for this note.

## Risks and incomplete work

- Run currency is still a prototype. Phase transfer, final settlement,
  extinction loss, and purchase rules remain open in
  [Scientific Data Economy](../SCIENTIFIC_DATA_ECONOMY.md); no run reward is
  deposited automatically into the new profile balance.
- The recovery copy is a raw PlayerPrefs value. There is a warning, but no
  in-game restore/export UI yet. Autosave may lose up to 30 seconds on a hard
  process crash. A full test suite, production build, and visual UI pass were
  not run.
- Earlier test runs used fixtures that cleared the Editor PlayerPrefs profile
  key. Any pre-existing local Editor profile save may have been affected before
  the fixtures were changed to restore both save keys. No prior value was
  captured, so its earlier state cannot be confirmed from these test results.
- The shared working tree already held substantial unrelated S4 and journey
  edits. This handoff covers only the profile commit; do not attribute the
  other changes to it.

## Next useful step

Review the per-profile save contract and decide currency categories and the
run-to-profile settlement boundary. Then wire actual progress triggers and
display balances/progress in the player UI.
