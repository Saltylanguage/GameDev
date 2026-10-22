# Rabbit Genome RG-00 and RG-01 implementation handoff

[Working state](../WORKING_STATE.md) | Status: implementation complete; Unity verification pending

- Handoff schema: 1
- Handoff ID: 2026-10-21-1820-codex-rabbit-genome-rg00-rg01
- Owner: codex
- Branch: codex/audio-direction-and-sourcing
- Baseline commit: 6ffb7b10 plus the existing uncommitted work
- Date: 2026-10-21
- Supersedes: 2026-10-21-1720-codex-rabbit-genome-implementation-plan

## Summary

Implemented RG-00 and RG-01 in the existing shared checkout. Preserved the
existing dirty work; did not start RG-02, change balance values, touch Trello,
commit, or push.

## RG-00 — portable source fingerprint

- Added `PopulationIntervention.cs` to `PortableSourceFiles`.
- Confirmed `CellSim.Batch.csproj` is the other relevant explicit shared-source
  list; `Program.cs` computes verification using the same portable hash list.
- Added `CellSimPortableProvenanceTests`: one test checks every batch
  `SharedSource` is fingerprinted, and one injected-reader regression proves a
  change to only `PopulationIntervention.cs` changes the hash.

## RG-01 — first cycle result controls

- Centralized boundary eligibility in `IsPopulationControlWindowOpen`. It keeps
  existing Mutation decisions and opens the first six-round journey result only
  while its continuation is pending. Pending encounter popups, committed travel,
  early end and terminal outcomes are closed.
- Extracted the existing Noesis controls into one shared template and placed it
  in both the Mutation decision and cycle-result view.
- Extended `PopulationControlsRefreshAndSeedCacheUseTheLiveJourney` to exercise
  an actual result button, check wallet and live population, preserve settled
  phase income/population and tick, reject controls during a pending encounter,
  allow them again after the encounter is claimed, and reject after travel.
  Added early-end/failure/success rejection assertions to the journey reward test.

## Verification actually executed

- `dotnet build tools/CellSim.Batch/CellSim.Batch.csproj --no-restore`: passed,
  0 warnings and 0 errors.
- Portable CellSim `self-test`: passed all listed checks.
- XAML and batch project XML parse: passed.
- Scoped `git diff --check` for the edited tracked files: passed.
- The first two focused filters returned **zero tests**; they were not counted as
  evidence. A full live EditMode request reported 311/311 passed, but its build
  log exposed that the Editor test assembly had failed compilation, so Editor
  tests were not included. The initial errors were a forbidden direct reference
  from the test asmdef to `Assembly-CSharp-Editor`; the test now resolves the
  runner reflectively. The same build log exposed invalid test assertions against
  a nonexistent `SimulationPhaseResult.CurrencyEarned`; those now compare the
  frozen phase reward summary instead.
- Unity verification and the 720p/1080p captures are still **pending**. After
  the source fixes, the wrapper reports a project lock with Pipeline
  unreachable, and no Unity process is visible. The live Editor was not killed,
  the lock was not deleted, and Clean was not forced around the project-state
  guard. The Editor must recover/reopen so Unity can compile the corrected
  assembly and run the checks.

## Next

After the project Editor is available and reports Ready, run:

```powershell
./CellSim.ps1 -Command Test -Execution Auto -Mode EditMode -TestFilter CellSimPortableProvenanceTests
./tools/Invoke-UnityVisualEvidence.ps1 -Execution Auto -TestFilter PopulationControlsRefreshAndSeedCacheUseTheLiveJourney -ScreenWidth 1280 -ScreenHeight 720
./tools/Invoke-UnityVisualEvidence.ps1 -Execution Auto -TestFilter PopulationControlsRefreshAndSeedCacheUseTheLiveJourney -ScreenWidth 1920 -ScreenHeight 1080
```

Inspect the cycle-result captures and fix any Unity compile, Noesis template,
test or layout issue before considering RG-00/RG-01 done. Do not begin RG-02
until that verification gate passes. No ecological experiment was run.
