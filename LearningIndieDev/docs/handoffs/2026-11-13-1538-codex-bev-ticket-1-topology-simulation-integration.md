# Bev Ticket 1 topology simulation integration

[Working state](../WORKING_STATE.md) | Status: in-progress

- Handoff schema: 1
- Owner: Codex
- Branch: codex/bev-ticket-1-map-wrapping
- Baseline commit: dac3bfa
- Date: 2026-11-13
- Publication: local, uncommitted, unpushed

## Summary

The runtime simulation now retains an explicit topology in CellularSimData and the initial Grid. Pattern queries preserve their origin through movement, perception, navigation, combat/blocking, reproduction, seed dropping, crowding and burrow interactions. This is a runtime integration foundation; production nonrectangular map definitions are still pending.

## Changes

- Existing rectangular behavior is supplied by RectangularGridTopology. Grid copies and steps preserve topology.
- Added integer-coordinate offset resolution and migrated simulation neighbor reads and destinations to origin-aware queries.
- Fixed gestation/birth-space checks that previously rejected wrapping destinations via IsInBounds.
- CellularSimData copies and rule/run updates retain topology. Resizing a custom map or toggling its wrapping cannot silently replace it with a rectangle.
- Scenario fingerprint version is now cellular-sim-data-v12 and includes the topology fingerprint. Existing exact-source replay manifests need regeneration for this source.
- PortableSnapshot rejects custom topology until it can serialize the complete map, preserving rectangular round trips.
- Custom topology may enter simulation data only while every storage coordinate is playable. Sparse layouts fail explicitly until all entity/resource/rendering loops support missing cells. Generic Grid sparse-hole fixtures remain supported.

## Validation

- Clean EditMode candidate: 190 total, 185 passed, 5 failed, zero skips: artifacts/unity-tests-20261113-153537/EditMode-results.xml.
- The five failures were rerun with every modified C# file temporarily restored to HEAD dac3bfa: 5/5 failed with identical assertion messages: artifacts/unity-tests-20261113-153716/EditMode-results.xml. They cover old Tough Hide/block values and upgrade rank/purchase expectations. Candidate source was restored byte-for-byte after this comparison.
- Final restored candidate focused check: 17/17 passed: artifacts/unity-tests-20261113-153828/EditMode-results.xml. Includes custom-seam perception/navigation, data topology retention/fingerprint, portable rectangular snapshot round trip, and wrapping/checkpoint tests.
- Follow-up after the previous checkpoint added playable-cell guards to 28 rectangular iteration passes. Focused grid/domain/burrow tests repeated at 185/190; the same five HEAD-reproduced upgrade/Tough Hide failures remain. This did not exercise sparse topology because CellularSimData still rejects it.
- Follow-up audit confirmed sparse gaps still cannot be enabled safely. Remaining full-area assumptions include burrow fingerprint/threat scans, board snapshot cell projection, population interventions, simulation-run state scans, and species metrics; board presentation currently allocates one snapshot per rectangular coordinate. The constructor guard remains the correct fail-closed behavior until these consumers handle absent cells consistently.
- No full-suite, PlayMode, visual, player-build, ecology-balance, or exact-source replay-parity acceptance claimed. The active user Editor is on BevBranch, separate from this isolated worktree.

## Risks and incomplete work

- Ticket 1 remains In Progress. No named-shape implementation or procedural seam generator is included.
- Audit and migrate every rectangular storage iteration/indexed scratch buffer before allowing sparse topology in CellularSimData. Confirmed remaining hotspots include HareBurrows, SimulationBoardSnapshot, PopulationIntervention, SimulationRunResult, and SpeciesSimulationMetrics, plus initial population placement, terrain/rendering, and persisted cell state.
- Supply a deterministic, immutable topology definition with validated seam pairings and fingerprints; the interface and test fixtures are not production map authoring.
- Add topology serialization to scenario assets, checkpoints/portable snapshots as required; prevent any map definition from being reconstructed using only width/height/wrapEdges.

## Next useful step

Complete missing-cell iteration across species, burrow, terrain, population, and board snapshot passes; then add a small authored irregular fixture with explicit paired boundaries. Exercise a real simulation step and presentation capture across those seams before generating named shapes or procedural maps.
