# Fertile Droppings scent first slice

[Working state](../WORKING_STATE.md) | Status: in progress

- Handoff schema: 1
- Handoff ID: 2026-10-17-1545-codex-fertile-droppings-scent-first-slice
- Owner: Codex
- Branch: ProjectMain
- Baseline commit: `6ffb7b10`
- Date: 2026-10-17

## Summary

An active `fertile-droppings` Hare Genome node now has a local simulation effect. It adds a provisional +0.10 seed-drop chance to the existing reserve-funded grass creation rule. A successful drop leaves a timed scent marker with the Hare's species and entity ID on its tile. Hungry carnivores with that Hare species as prey sample scent every 10 ticks: ordinary Hare scent on occupied and vacated movement tiles is detectable at the shorter range, while a dropping takes priority at the longer range. They move toward a detected dropping when no prey is visible and sniff it at one-cell range. Once a predator has sniffed the dropping, it samples only that Hare's freshest ordinary scent at longer range every 10 ticks; movement between samples uses the last known marker location. A Hare standing still refreshes its own tile's scent. The existing attack resolver is unchanged.

## Starting values and boundaries

| Rule | Starting value |
| --- | ---: |
| Scent sample interval | 10 ticks per predator |
| Hare ordinary scent lifetime / detection range | 20 ticks / 16 tiles |
| Fertile Droppings marker | 40 ticks / 24 tiles |
| Tracking range after sniff | 36 tiles |
| Memory without a fresh sample | Up to 20 ticks |
| Genome seed-drop chance bonus | +0.10, capped at 1.0 |

Josh confirmed 20/16 ordinary scent, 40/24 Fertile Droppings scent, and 36-tile post-sniff tracking as starting values on October 17. Fox production vision is 12 tiles. This first slice applies the 2× duration and 1.5× range to the Fertile Droppings marker relative to ordinary Hare scent, then applies another 1.5× range after sniffing. The +0.10 seed-drop bonus remains a provisional implementation value. A visible prey target takes priority over scent. Hungry predicates still use the species' existing foraging rules.

The marker is stored on the map cell and remains when its creature leaves, dies, or a plant changes that tile. The predator's sampled memory is stored on its creature cell and moves with it. Both survive grid copies and in-memory phase checkpoints. Each tile holds one marker; a dropping stays in preference to ordinary movement scent, and overlapping droppings do not stack. The current first slice uses point sources and follows actual scent marks rather than looking up a live Hare by ID. It is not yet a diffusing scent field or a visual trail.

## October 17 telemetry follow-on

The run and phase activity now count ordinary scent acquisitions, dropping acquisitions, dropping sniffs, successful named-scent samples, and scent-directed pursuit decisions by predator species. Exported report activity includes the same five fields. These are decision and perception counts, not evidence that scent caused a kill; compare them with existing combat and population outcomes on matched seeds.

CellSim now accepts `-RabbitGenomeNodeIds fertile-droppings` for simple experimental runs and records the selected node and Genome fingerprint in report schema 37. A [20-seed Forest Edge pilot](../Research/2026-10-17-fertile-droppings-pilot.md) compares that arm with no active Genome node. It found a measurable plant/Hare benefit and lower mean Fox population and kills at tick 300; it is screening evidence, not a balance pass.

## Verification

- `dotnet build SaltyGame.Runtime.csproj --no-restore`: passed after a local restore; seven existing obsolete-API warnings.
- `dotnet build SaltyGame.Tests.csproj --no-restore`: passed; existing obsolete-API warnings.
- Clean Unity EditMode `SaltyGame.Tests.SpeciesDomainTests`: **145/145 passed**. Evidence: `artifacts/unity-tests-20261017-155132/EditMode-results.xml`.
- The telemetry follow-on passed the same **145/145** fixture. Evidence: `artifacts/unity-tests-20261017-164909/EditMode-results.xml`.
- The phase-window telemetry assertion passed **145/145**. Evidence: `artifacts/unity-tests-20261017-165043/EditMode-results.xml`.
- The CellSim Genome option passed **18/18** focused Editor tests. Evidence: `artifacts/unity-tests-20261017-165316/EditMode-results.xml`. A one-seed smoke report and both 20-seed pilot arms completed through the clean Unity lane.
- The first one-seed CLI attempt failed before simulation because the wrapper forwarded a false `WrapEdges` switch positionally as a phase-schedule argument. The wrapper was fixed; the succeeding smoke report is `artifacts/cellular-experiment-20261017-165428/report.json`.
- `CellSim Validate` is a herbivore-stat-line validator and rejected both simple pilot reports because that optional stat line was not requested. The pilot checked report schema, Genome provenance, scenario fingerprint, seed pairing, and tick-zero populations directly.
- The first full fixture run had 142/143 passing. Its new scent test found an age-offset bug in the sampling phase. The code now uses the source tick's age; the later 145/145 run includes that fix, expiration while moving, ordinary range, dropping priority, and scent on stationary Hares.
- No PlayMode, player UI, balance seed panel, or human play review ran in this slice.

## Remaining work

- The Gene Lab is still a read-only catalog preview. There is no production Genome upgrade asset, configured purchase price/capacity cost, or purchase action to place this ID into a player's active snapshot. A synthetic active snapshot and the runner path are covered by tests.
- The broader sight-and-scent system still needs a designed diffusing field, terrain interaction, and player-readable visualization. Existing sight-acquired tracking uses separate legacy logic; this slice does not silently replace it.
- The new scent counters need a matched balance panel alongside existing kills and population outcomes. A full Gardener or two-simulation prediction has not been run, and these counters alone cannot attribute kills to scent.
- Josh and Sim should use the agreed scent starting values in the first balance comparison, then review the provisional +0.10 drop bonus and one-marker-per-cell rule before balance promotion.

## Next useful step

Review the [playtest offer preview](../Research/2026-10-17-fertile-droppings-playtest-offer-preview.md), agree its node placement and economy terms, then author the offer and Gene Lab action. Follow the 20-seed pilot with a larger frozen comparison that includes the playable journey gate and post-clear persistence.
