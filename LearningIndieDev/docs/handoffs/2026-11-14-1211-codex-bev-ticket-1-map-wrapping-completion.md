# Bev-Ticket-1 - mandatory map wrapping and topology foundation

[Working state](../WORKING_STATE.md) | Status: complete within ticket scope

- Handoff schema: 1
- Owner: Codex
- Branch: `codex/bev-ticket-1-map-wrapping`
- Baseline: `dac3bfa` (ProjectMain integration)
- Date: 2026-11-14
- Ticket: https://trello.com/c/tkOVt9XS/159-bev-ticket-1-map-wrapping
- Scope: completes the November 13 topology foundation in this existing worktree. Publication is local until explicitly pushed.

## Behavior and implementation

- Scenario maps wrap across both axes. The preview and Workbench expose no optional map-wrap toggle. The old data/experiment argument remains source-compatible but cannot disable scenario wrapping; generic grids retain bounded construction for cave tools and isolated fixtures.
- Preserve the existing `IGridTopology` and `RectangularGridTopology` API. `MapTopology` is the concrete immutable playable-cell definition, with deterministic sorted identity, generation-input provenance and reciprocal boundary connections. Outer row/column endpoints receive mandatory opposite-edge connections. Interior gaps have no implicit edge, cannot be spawned into, and cannot be crossed by a longer offset. Explicit shape seam pairings validate endpoints, reverse directions and conflicts.
- `MapTopologyDefinition` serializes author-provided cells, connections and generation inputs on both scenario authoring assets. Named shape selection and procedural irregular controls remain tickets 2 and 3; ticket 1 tests use geometry fixtures for square, circle, oval, diamond, cross, rectangle and an irregular layout with a hole.
- Movement, navigation, perception, attack/block direction, reproduction/conception/gestation, resources, scent, crowding and burrow neighbors use the same origin-aware topology. Rectangular and shaped gestation seam tests cover previously bounded birth checks.
- Sparse cell scans, explicit/random population placement, interventions, node resets, metrics, run snapshots and portable digests skip missing cells. `Grid.Count` remains playable capacity; `StorageCount` sizes indexed scratch arrays and backing projections. Presentation snapshots expose `IsPlayable` and tile drawing skips missing entries.
- Data copies and checkpoints retain topology. A mismatched checkpoint topology is rejected. Resizing custom layouts cannot silently replace their topology with a rectangle.
- Scenario fingerprint v12 includes topology identity. Portable snapshot schema 2 retains cells, effective reciprocal connections and generation inputs. Portable surface/replay digests include topology. Old frozen snapshots/manifests must be regenerated for these changed sources and semantics.
- Runtime validation exposed an existing saved-preview-draft migration defect: old saved drafts deserialize the newer `MaximumHealth` field as zero. The rule adapter now maps only zero to the legacy one-health default, preserving authored positive values and negative-value validation. A focused regression covers zero and positive values. No authored health assets or balance tuning changed.

## Evidence and baseline separation

- Both standalone Release builds and the expanded runner self-test pass. The self-test includes sparse shape JSON round-trip, connected seams, absent cells, generation inputs and the existing 18-round replay/hidden-burrow checks.
- Initial clean candidate full suites: `artifacts/unity-tests-20261114-115926/`: EditMode 782 passed / 16 failed / 1 skipped; PlayMode 15 passed / 81 failed / 7 skipped. All new map-topology tests passed.
- Untouched `dac3bfa` failing-case comparison: `artifacts/unity-tests-20261114-120926/`: EditMode 5 passed / 15 failed. Those 15 assertion/error messages match the candidate exactly; baseline Cave Preview PlayMode reproduces the identical maximum-health-zero exception. The one additional candidate failure was a legacy bounded checkpoint fixture; it now uses wrapping to agree with its scenario.
- Candidate files were restored byte-for-byte after the baseline comparison. SHA-256 restoration receipt and exact failure comparisons are retained in `artifacts/baseline-inplace-20261114/`.
- Focused corrected topology/checkpoint set: 34/34 EditMode in `artifacts/unity-tests-20261114-120623/`. After the preview-draft adapter fix, the original Cave Preview runtime reproduction passes 1/1 in `artifacts/unity-tests-20261114-121339/`; that intermediate run also exposed and led to correction of a reflection flag in the new regression fixture.
- Final clean full suites, `artifacts/unity-tests-20261114-121541/`: **EditMode 789 passed / 12 failed / 1 skipped (802 total)**; **PlayMode 92 passed / 4 failed / 7 skipped (103 total)**. All 14 topology tests, legacy-draft cases, preview wrapping, portable provenance and checkpoint tests pass. Every remaining EditMode failure message matches untouched `dac3bfa` exactly; receipt: `artifacts/baseline-inplace-20261114/final-comparison.json`.
- The remaining PlayMode cases were rerun on `dac3bfa` with only the zero-health draft adapter fix, without topology changes (`artifacts/unity-tests-20261114-122101/`, 0 passed / 4 failed). Three `quick-escape` display errors match exactly. The authored seed-10101 route fails on both versions: baseline tick 468, wrapping tick 288. This is an observed topology-dependent ecology change, not a balance approval. No population/health/upgrade asset tuning was made. Candidate bytes were restored and verified again; receipts: `artifacts/baseline-play-comparison-20261114/`.
- No new unresolved ticket regression remains in the final suites. Existing broader failures and the parity/graphics skips remain visible and are not described as a green full suite.

## Boundaries and handoff

- No new terrain/border art, shape picker, procedural generation controls, balance sweep, player build, or human visual acceptance is claimed.
- Remaining full-suite failures concern existing population-control expectations, upgrade ranks/health, three quick-escape display errors and the authored-route ecology expectation; compare the retained baseline messages and route ticks. The exact-source Field Data replay parity fixture requires a new retained manifest.
- The original `F:/ForkBin/GameDev` checkout remains on `BevBranch`, clean. An exploratory implementation there was restored after discovering this existing ticket worktree; its patch is retained under `artifacts/nov14-prototype/`. The unrelated pre-existing ProBuilder settings edit in this worktree is preserved outside ticket commits.
- Use this worktree for follow-on map work. Keep this implementation local until publication is requested.
