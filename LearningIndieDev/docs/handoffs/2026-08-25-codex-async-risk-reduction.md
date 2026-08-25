# Async risk-reduction audit and fixes

Date: 2026-08-25  
Status: bounded fixes complete; Unity verification blocked by open Editor

## Outcome

Parallel read-only audits covered experiment provenance, upgrade direction,
tests, performance groundwork, documentation consistency, repository hygiene,
retained prototypes, and Noesis Editor analytics. Island Survivor was recorded
as deprecated and the Discord bridge as on hold. Active terrain, HUD, Figma, and
sprint-planning changes were not modified.

## Implemented

- Completed `CellularSimDataFingerprint` coverage for attack modifier, damage,
  maximum energy, and litter bounds; bumped the contract to version 6 and added
  focused inequality assertions.
- Guarded the optional `F:\Editor` Unity probe so a missing drive no longer
  crashes editor discovery.
- Corrected the stale claim that no Play Mode test assembly exists.
- Added a proposed Windows performance baseline protocol without adding tools or
  dependencies.
- Added a six-node experimental Hare upgrade catalog contract. Values remain
  hypotheses, not approved balance.
- Recorded an evidence-backed Noesis Editor analytics decision brief without
  modifying the embedded vendor package or copying its credential.
- Confirmed repository-wide Unity asset/meta parity: zero missing asset metas and
  zero orphan metas at audit time.
- Classified Cave/Life domain code as retained, their two unreferenced preview
  components as focused cleanup candidates, and Alpha offspring as dormant.

## Material evidence limitations

- Combat mode, attack-opportunity mode, experimental feature ID, and Fox attack
  cooldown are execution inputs, not `CellularSimData`. They remain separate
  report provenance and are not covered by the scenario-data fingerprint.
- Sampled ignored experiment artifact bundles cited by historical handoffs are
  absent from this checkout. Their committed summaries remain, but raw reports
  cannot be independently rechecked here.
- Historical full-suite evidence records two graphics/Noesis Play Mode failures;
  there is no current clean seven-test Play Mode baseline.

## Verification

- `tools/Test-StudioPolicy.ps1`: passed (2 guidelines, 6 rules, alert-only).
- `git diff --check`: passed; line-ending warnings only.
- `tools/Test-UnityPreflight.ps1`: editor discovery progressed after the probe
  fix, then correctly stopped because Unity was already running (PID 17788).
- Unity tests were not claimed or run. The focused fingerprint test and orphan
  preview removal remain gated on a saved, closed Editor.

## Next safe actions

1. Close Unity, rerun preflight, then run the focused fingerprint test and full
   Edit Mode suite.
2. Coordinate ownership before changing the actively modified Noesis/HUD code
   implicated in the two graphics Play Mode failures.
3. After test availability, remove `CavePreview.cs`/`.meta` and
   `LifeSimulationPreview.cs`/`.meta` as separate focused cleanup.
4. Before new ecology runs, add a durable run manifest/checksum that combines
   scenario fingerprint with execution options and reconciliation verdict.
5. Ask the Noesis vendor/license owner for a supported analytics opt-out or
   consent-gated package before modifying the embedded package.
