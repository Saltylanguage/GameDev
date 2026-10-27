# FD2 First-Slice Tuning Sheet

- **Version:** 1.0 (approved first-test definition)
- **Updated:** 2026-10-24
- **Status:** Approved and authored in the isolated integration checkout; definition and compatibility checks pass. T01 marked complete in Trello Done with verified evidence on 2026-10-26; code remains local.
- **Owner:** Bevin
- **Related:** [Process Design](FIELD_DATA_PERMANENT_PROGRESSION_PROCESS_DESIGN.md) | [Solution Design](FIELD_DATA_PERMANENT_PROGRESSION_SOLUTION_DESIGN.md) | [T01 ticket](FIELD_DATA_PERMANENT_PROGRESSION_TICKET_DRAFT.md#fd2-t01--freeze-first-slice-progression-content-and-tuning)

## Purpose

The [executable starter definition](../Assets/Scripts/Game/Species/FieldDataPermanentProgressionDefinition.cs)
is the single source used by authoring and domain callers. This sheet records
its approved first-test values. Economy, stable
node IDs, prerequisites and capacity terms retain the recorded decisions. The
effect and display-name columns below were approved by Bevin on October 24 using
supported incoming mechanics. These are test settings, not
validated balance results. See the [T01 reconciliation review](FIELD_DATA_PERMANENT_PROGRESSION_T01_REVIEW.md)
for evidence, rule conflicts and the integration sequence.

## Journey settlement

| Item | First-test definition | Status |
|---|---|---|
| Leftover FD conversion | 10 FD = 1 selected-species data before research bonus | Provisional baseline |
| Research bonus | Each level multiplies the base species-data amount by 1.10 | Provisional baseline |
| Research cap | 3 purchased levels | Agreed first-test setting |
| Research prices | Level 1: 10 RD; Level 2: 20 RD; Level 3: 30 RD | Agreed first-test setting |
| Success bonus | 10 RD for every successful journey, including zero leftover FD | Provisional baseline |
| Failed journey | Lose run-earned FD; permanent balances and unlocks remain | Approved rule |

**Rounding rule:** First calculate `base species data = floor(leftover FD / 10)`. Then calculate `deposit = floor(base species data × 1.10^research level)`. Discard the remaining fraction. Use exact decimal or integer-rational arithmetic, not binary floating-point rounding.

| Leftover FD | Research level | Base data | Species-data deposit |
|---:|---:|---:|---:|
| 0 | 0 | 0 | 0 |
| 9 | 0 | 0 | 0 |
| 19 | 0 | 1 | 1 |
| 19 | 1 | 1 | 1 |
| 99 | 3 | 9 | 11 |
| 100 | 0 | 10 | 10 |
| 100 | 1 | 10 | 11 |
| 100 | 3 | 10 | 13 |

## Hare Genome license, nodes, and loadout

| Item | First-test definition | Status |
|---|---|---|
| Path license | 10 Hare Data; grants access to the tree only | Provisional baseline |
| Node purchase | 5 Hare Data per node | Provisional baseline |
| Active capacity | 8 points per Hare loadout | Existing Genome contract |
| Node capacity cost | 2 points for each first-slice node; at most four of five fit at once | Agreed first-test setting |
| Reset | 20 RD; clears the selected species' active loadout; refunds no Hare Data; permanent node unlocks remain | Agreed first-test setting |
| Replacement selection | Each replacement requires a new paid reset. Select the full owned loadout freely from empty, then apply once; identical selection is a no-op. Empty reset is rejected without a charge. | Bevin confirmed 2026-10-26 for T08 |
| Timing | Active loadout applies to journeys launched after the change | Approved rule |

The first-slice map retains all five existing IDs and their prerequisite links.
The approved display names describe the supported effect directly. Each effect
is one signed additive modifier; no burrowing, hearing-specific sensing, or new
ability is implied by the older placeholder name.

| Stable node ID (unchanged) | Approved display name | Owned prerequisites (IDs abbreviated) | Hare Data cost | Active capacity cost | Approved exact modifier |
|---|---|---|---:|---:|---|
| `hare.guarded-burrow` | Tough Hide | None | 5 | 2 | `combat.block` +1 |
| `hare.keen-hearing` | Long Vision | `guarded-burrow` | 5 | 2 | `awareness.vision-range` +2 tiles |
| `hare.swift-digging` | Long Stride | `guarded-burrow` | 5 | 2 | `movement.speed` +0.25 |
| `hare.safe-foraging` | Efficient Digestion | `keen-hearing`, `swift-digging` | 5 | 2 | `digestion.energy-bonus` +0.25 |
| `hare.warren-network` | Population Reserves | `guarded-burrow`, `safe-foraging` | 5 | 2 | `population.pool-capacity` +10 |

All abbreviated prerequisites above have the `hare.` prefix. Each node is owned
once and contributes its effect once while active. A prerequisite controls
permanent purchase eligibility, as in the FD2 SDD; it does not also consume
active capacity. With all five owned, a legal loadout can use up to four.
Requiring the entire prerequisite chain active would make Population Reserves
unusable: all five nodes would cost ten points. The incoming resolver and
purchase checks must use the FD2 ownership rule before these nodes can run in
the player flow. T01 supplies the owned-prerequisite freeze adapter; later
tickets wire purchases and launch. The shared POC catalog's rules stay strict.

Population Reserves raises the incoming ordinary replenishment limit from 50
to 60 while active. It does not add ten Hares, cap births, cull an above-limit
population, or replenish each round. Incoming POC values are source mechanics,
not approved FD2 balance. T01 authored the five approved assets under
`Assets/Data/TestData/Genome/HareGenome`, preserving their existing GUIDs.

Source counterparts at incoming `7ac2a54` are `ToughHide.asset`,
`LongVision.asset`, `LongStride.asset`, `EfficientDigestion.asset`, and
`PopulationPool.asset` under `Assets/Data/TestData/Genome/RabbitSelectionPOC`.
Copy their supported modifier semantics, not their `poc.*` IDs, POC prices,
prerequisite graph or ownership/refund behavior. Burrows Preview remains
unsupported. Fertile Droppings, Larger Litters and Founding Hares are supported
incoming content outside this five-node proposal; they remain in Salty's catalog.

## Rules that remain in force

- A first journey can start without a Genome license or active nodes.
- The successful journey credits species data only to its selected species wallet.
- The legacy generic profile currency migrates to Research Data; existing Genome ownership and active selections are preserved.
- Genome nodes are permanently unlocked when bought. Reset affects active selection only and gives no node-data refund.
- The 10 / 20 / 30 RD prices and 20 RD reset cost are first-test settings, not balance-validated values.

## T01 completion gate

**Implementation checkpoint (2026-10-24):** The approved five assets have been
authored through Unity in `codex/fd2-t01-integration`; existing node metadata/GUID
hashes are unchanged. The authoring/domain tests validate the asset fingerprint
against `field-data-permanent-v1`, execute the owned-prerequisite adapter, and
reject over-capacity, unowned/unknown and incompatible definitions. Clean
EditMode passes 497 tests with one dedicated parity-fixture skip. Both new
migration PlayMode tests pass. Full PlayMode has 51 passes, six graphics skips
and seven failures reproduced identically on unchanged incoming ProjectMain.
See the [integration handoff](handoffs/2026-10-24-2158-codex-fd2-t01-approved-definition-and-profile-integration.md).
T01 is technically ready for closeout; no full-gameplay or balance acceptance
is inferred. Settlement, purchase/reset UI and next-launch wiring remain their
later FD2 tickets.

**Approval (2026-10-24):** Bevin accepted all five effects and display names.
The earlier proposal notes below describe the review checkpoint. Authoring and
isolated integration evidence will be recorded in the next handoff.


**Original review state (superseded by approval above):** Five effects and truthful display names were
proposed above. Economy settings remain the existing FD2 baseline; the review
records the approved FD2 ownership/reset rule as a scoped exception to SG-005's
Rabbit reset paragraph. Approval of the new mappings/names and a validated
integrated definition remain open. The isolated
incoming portable contract check proves supported modifiers and reproduces the
capacity conflict; it does not validate a merged Unity implementation or balance.

**Dependency refresh (2026-10-24):** Fresh origin/ProjectMain at `7ac2a54`
includes Salty's Rabbit Genome mechanics (`be98b03`); the push is available
remotely but not merged into local BevBranch. Its POC node IDs and purchase/reset
values differ from this FD2 definition. Do not treat the incoming POC values as
an approved replacement: reconcile catalog, effects, prices and reset semantics,
then perform the SG-005 review before closing T01.

The numeric baseline and five-node topology are recorded. **FD2-T01 remains open** until Salty's core mechanics are pushed and each placeholder receives a supported, named effect with its cost and 2-point capacity assignment reviewed against SG-005. Do not close T01 or claim the Hare tree is gameplay-ready before that review.
