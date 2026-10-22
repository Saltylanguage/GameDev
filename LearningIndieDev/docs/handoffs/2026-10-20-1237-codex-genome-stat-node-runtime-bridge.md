# genome-stat-node-runtime-bridge

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-10-20-1237-codex-genome-stat-node-runtime-bridge
- Owner: codex
- Branch: codex/audio-direction-and-sourcing
- Baseline commit: 6ffb7b10
- Date: 2026-10-20
- Supersedes: none

## Summary

Completed item 2 of the Rabbit Genome prerequisite sequence for existing stat
targets. Reused the agreed asset/catalog/snapshot design and signed-additive
species attribute registry. The canonical Desktop route now freezes values and
applies permanent Genome stats to all participating species before initial
population seeding and ordered temporary Mutations.

## Changes

- `GenomeUpgradeAsset` authors ordered stable attribute IDs and signed values.
  Immutable `GenomeUpgradeNodeSnapshot` validates/copies them and fingerprints
  their values and registry identity under catalog contract v3. Existing assets
  need no migration; empty lists remain valid authoring metadata.
- `GenomeSimulationSnapshot.Resolve(catalog)` freezes active node snapshots in
  allocation order and verifies node/species lookup and active prerequisites.
  `ApplyToNaturalRules` applies those ordered additions for all participating
  species. Unknown attributes, duplicate attributes, fractional integer
  modifiers, invalid resulting rule bounds, unresolved stat IDs, and selected
  metadata placeholders with no implemented runtime effect are rejected.
- `ProfileSessionSnapshot.CreateGenomeSnapshot(catalog)` adds an explicit
  resolution option without changing the saved profile schema. Desktop obtains
  the catalog through its existing serialized view-model/provider boundary.
- `SpeciesSimulationPreview` builds Genome-adjusted natural rules before
  progression and grid creation. Scenario data now carries every participating
  species' effective rules rather than only the selected player's rules.
- Ready-window reopening and new journeys rebuild from natural drafts. New
  journeys become editable before Desktop recaptures the current profile and
  catalog. Continuing a phase keeps effective rules and frozen values; it does
  not reapply Genome bonuses. Mutation provenance remains separate.
- Added domain, authoring and Desktop integration tests with in-memory nodes.
  Updated prerequisite checklist, canonical design, context, and concern progress.

## Decisions and assumptions

- No new production node, price, unlock, modifier operation, or balance value.
  Test modifier values are delivery fixtures. Fertile Droppings retains its
  existing explicit ability and approved playtest offer.
- V1 remains signed additions only, in allocation order and then authored
  modifier order. Bounds are checked after each addition by the existing
  registry/rule constructors, so authors must order dependent litter bounds
  appropriately. No implicit sorting or clamping is introduced.
- The old identity-only snapshot API remains for saved-profile transport and
  the explicitly supported Fertile Droppings ability. Arbitrary stat nodes
  require catalog resolution. The canonical Desktop route supplies it. Legacy
  Lab and CellSim's existing selector remain Fertile-Droppings-only delivery
  paths until their separate arbitrary-build work is scheduled.
- The working checkout began on ProjectMain and was later observed on
  `codex/audio-direction-and-sourcing` at the same HEAD `6ffb7b10`. This task
  neither switched branches nor committed/pushed. Preserve unrelated audio,
  art/meta, rendering, and prior uncommitted gameplay work.

## Validation

- Live Unity 6000.4.6f1 recompile passed with zero errors, including after final
  source edits. Used Unity MCP with the existing host Editor; the project
  wrapper's Pipeline-discovery limitation was established during item 1.
- Runtime assembly: **285/285 passed**, repeated after the final placeholder
  rejection check. Includes existing Mutation/registry behavior and new stat
  stacking, background species, inactive-node exclusion, bounds, frozen values,
  ordered identity, checkpoint preservation and rejection tests.
- Focused Genome checks: **23/23 passed**, covering authoring capture and
  refresh, production Fertile Droppings metadata, and Genome contracts.
- Population/scenario checks: **18/18 passed**.
- New Desktop integration test: **1/1 passed**. MainMenu profile route, ready
  reopen, actual initial Hare/Fox energy, frozen source edits, one phase
  continuation, next-journey recapture, and removing an active node.
- Full Lab PlayMode suite: **11/11 passed**, including the preceding payout,
  persistence, Gene Lab and legacy-launch regressions. The tool request timed
  out after 300 seconds; Unity's saved `Temp/pipeline_test_status.json` and
  `TestResults.xml` contain the complete 11-test result, confirmed by
  `TestResultCollector.RunFinished` in Editor.log. Copied JSON/XML to evidence;
  no Editor restart, lock deletion, or process termination was used.
- Initial fixture corrections: expected exact exception subtype; checkpoint
  must be captured at a continuous decision boundary; authored `hare` identity
  must not be confused with the legacy `herbivore` ID. These failed checks were
  corrected before their passing reruns.
- Retained evidence: `artifacts/unity-tests-20261020-genome-stats/` contains
  runtime, Genome, population, focused PlayMode and full PlayMode results,
  initial domain fixture failures, and compilation status. No ecology trial or
  human usability/balance review is implied.

## Risks and incomplete work

- Multiple-node selection/feedback in Gene Lab is still item 3. No production
  stat node is offered yet; the only production node remains Fertile Droppings.
- New abilities (burrows, migration, plant tiers, etc.) still need explicit
  mechanics. This bridge executes only existing registry targets and the
  already implemented Fertile Droppings ability.
- Arbitrary-build research selection/export and Biome presentation remain
  separate prerequisites. The shared per-species composition operation is
  available to those callers; do not treat an identity-only snapshot as an
  executable stat build or apply the bridge twice to effective rules.
- Whole-worktree whitespace checks report pre-existing art `.meta` whitespace;
  keep those unrelated files untouched. Scoped checks cover this task's files.

## Next useful step

Implement Gene Lab node selection/allocation against the existing catalog,
including prerequisites, capacity feedback, branch navigation and active-state
display. Use fixture catalogs for UI verification until additional production
nodes and values are agreed. The stat delivery path is ready for those nodes.
