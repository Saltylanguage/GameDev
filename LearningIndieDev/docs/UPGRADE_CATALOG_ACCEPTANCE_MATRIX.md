# Species Per-Run Upgrade Acceptance Matrix

**Status:** Contract and authoring gate complete; gameplay and balance evidence
remain open.

**Owner:** Josh
**Scope:** The seven Hare upgrades in
`Assets/Data/CellularSimulation/Upgrades/Production/`.

This matrix answers two different questions that should not be mixed together:

1. **Did we author the upgrade correctly?** This is the gate we can close now.
2. **Does the upgrade create a good, intended playstyle?** That needs matched
   simulation evidence and a later human balance decision.

An asset can pass the first question while remaining provisional for the
second. The values below are starting hypotheses, not approved balance.

## Scope reconciliation

The original S2.2 card asked for examples covering numeric, spatial,
conditional, and tradeoff effects. The approved V1 upgrade contract supports
finite signed additive values for registered species attributes only. The
current matrix therefore covers the supported numeric and tradeoff effects;
spatial or conditional operations are not missing rows.

Adding spatial targets or conditional rules would expand the contract and the
research evidence model. That is a separate design package and must not be
introduced just to satisfy this first catalog or to prepare EX-010.

## Catalog contract gate

The production catalog passes this gate when every row below has:

- one unique, stable ID and one valid target species;
- a non-negative cost, a player-readable description, and at least one
  modifier;
- finite signed additive modifiers, with each attribute listed only once;
- no unresolved prerequisite or exclusion relationship;
- a deterministic runtime snapshot, registry fingerprint, and snapshot
  fingerprint;
- a clear launch-only or live-boundary applicability decision.

The editor fixture test
[`SpeciesUpgradeAssetCatalogTests.cs`](../Assets/Tests/Editor/SpeciesUpgradeAssetCatalogTests.cs)
checks these points against the exact rows in this matrix. The general runtime
and adapter tests cover the same V1 rules for non-asset and research inputs.

Latest verification: Unity EditMode **210/210 passed** on 2026-09-05. The
artifact is [`EditMode-results.xml`](../artifacts/unity-tests-20260905-064231/EditMode-results.xml).

## Production rows

| Stable ID | Player-facing idea | Cost | Modifiers (in authored order) | After run starts? | Evidence needed before balance approval | Contract status | Balance status |
| --- | --- | ---: | --- | --- | --- | --- | --- |
| `trailblazer-long-stride` | Move farther, at the cost of needing more nearby mates | 5 | `movement.speed +0.5`; `reproduction.neighbor-count +1` | Yes | Movement attempts and arrivals; reproduction blocks caused by the added partner requirement | Accepted | Provisional |
| `trailblazer-far-sight` | See farther, but spend energy faster | 8 | `awareness.vision-range +1`; `energy.metabolism +1` | Yes | Newly visible targets; energy trend; starvation deaths | Accepted | Provisional |
| `warren-guarded-burrow` | Block more hits, but move more slowly | 7 | `combat.block +2`; `movement.speed -0.25` | Yes | Blocks and prevented damage; movement attempts and arrivals | Accepted | Provisional |
| `warren-room-to-breed` | Support larger groups and ease crowding, but spend energy faster | 9 | `reproduction.group-size +1`; `crowding.energy-penalty -1`; `energy.metabolism +1` | Yes | Group-limit blocks; crowding penalties; births; local Fern depletion | Accepted | Provisional |
| `gardeners-seed-pouches` | Start with more stored food, but less starting energy | 6 | `resource.starting-food-reserve +2`; `energy.starting -2` | **No — launch only** | Starting reserve and energy; reserve use; early starvation exposure | Accepted | Provisional |
| `gardeners-careful-sowing` | Spread seeds more reliably, but move more slowly | 8 | `resource.seed-drop-chance +0.1`; `movement.speed -0.25` | Yes | Successful drops; new Fern cells; Fern population over time; movement | Accepted | Provisional |
| `familial-bond-large-litters` | Tolerate more crowding | 10 | `crowding.tolerance +3` | Yes | Crowding deaths, births, and local population density | Accepted | Provisional |

All seven rows target `hare`, have an empty prerequisite list and an empty
exclusion list, and use `PerRun` scope. The empty relationship lists are
intentional for this first catalog slice; branch prerequisites and exclusions
remain a later design decision.

## Research and continuation use

- Production research inputs must resolve these stable IDs through
  `SpeciesUpgradePredictionInputAdapter`, not by copying modifier values into
  a second fixture.
- The adapter records the ordered IDs, resolved values, catalog path, contract
  version, registry fingerprint, and snapshot fingerprints. This is the parity
  evidence for the game and research inputs.
- `gardeners-seed-pouches` is not a mid-run offer because its effects describe
  initial seeding. Making it affect existing creatures or newborns would be a
  separate product decision and a new matrix row or lifecycle rule.
- EX-010 currently uses its own research fixtures (`faster-movement` and
  `crowding-tolerance`) so that the approved experiment can preserve its
  historical values. That does not change or replace this production matrix.

## What remains open

The following are deliberately not closed by authoring this matrix:

1. **Mechanic evidence:** a deterministic matched run must show that each
   modifier changes the intended simulation measure.
2. **Balance evidence:** matched baseline and upgrade runs, including held-out
   seeds where appropriate, must support any balance or promotion claim.
3. **Branch design:** prerequisites, exclusions, and hybrid paths need a human
   design decision before they are added to the assets.
4. **Spatial/conditional contract:** decide whether and how those unsupported
   effect types should be added in a future package.
5. **EX-010 approval:** the experiment still needs its contract-specific
   schedule, phase/stat-line review, parity gate, and human approval before it
   is run.

M1 closeout remains a separate final review and is intentionally deferred until
the current pre-EX-010 work block is complete.
