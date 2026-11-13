# FD2-T01: Hare content and integration review

- Date: 2026-10-24
- Status: Content approved by Bevin; isolated authoring and integration validation underway
- Owner/reviewer: Bevin
- Local baseline: BevBranch `9f9ed11` plus the preserved local T02 changes
- Incoming baseline: origin/ProjectMain `7ac2a5456bf0c1f6b8e3c106533cd555f78ba5dc`
- Definition: [FD2 tuning sheet v1.0](FIELD_DATA_PERMANENT_PROGRESSION_TUNING_SHEET.md)
- Context: [PDD](FIELD_DATA_PERMANENT_PROGRESSION_PROCESS_DESIGN.md), [SDD](FIELD_DATA_PERMANENT_PROGRESSION_SOLUTION_DESIGN.md), [T01 ticket](FIELD_DATA_PERMANENT_PROGRESSION_TICKET_DRAFT.md#fd2-t01--freeze-first-slice-progression-content-and-tuning)

## Outcome

**Approval (2026-10-24):** Bevin accepted the five mapped effects and display
names. Implementation has started in the managed isolated checkout on
`codex/fd2-t01-integration` at
`C:\Users\bevin\.codex\worktrees\fd2-t01-integration\GameDev`.
The proposal/review text below is retained as the prior checkpoint.


Use Salty's existing modifier runtime for the five FD2 nodes. Keep FD2's stable
IDs, prerequisite graph, permanent ownership, economy and eight-point capacity.
The tuning sheet records the approved effects and clearer display names. No
new simulation ability is needed. The review findings below describe the
original source comparison; current implementation evidence belongs in the
approval/integration handoff. No balance acceptance is implied.

## What can be reused

Incoming `be98b03` supplies a registry-backed Genome modifier path, immutable
resolved nodes, catalog/registry fingerprints, all-species rule composition,
and separate starting-population/pool handling. `GenomeUpgradeAsset` creates
validated immutable snapshots. This is the existing implementation to extend;
there is no need for a second modifier framework.

The proposed five effects are supported by the incoming registry and runtime.
An isolated portable check constructs the proposed catalog and executes each
effect against default rules (pool capacity through its separate accessor).
It is not a Forest Edge balance run. The source snapshot also includes tests
for frozen values, all-species application before Mutations, invalid modifiers,
and checkpoint retention; these test sources were inspected, not executed here.

Burrows Preview explicitly says safe mating/hiding is unimplemented and cannot
be activated. Do not make Guarded Burrow a burrow ability. Proposed display
names instead say what the existing modifiers do. Preserve all five stable IDs
and their Unity GUIDs when authoring is later approved.

## Rule differences to resolve before integration

| Area | Incoming Rabbit implementation | FD2 rule to retain / adaptation needed |
|---|---|---|
| Currency | Separate generic Currency, Rabbit Data and Research Data | Species-keyed wallets; old generic currency migrates to Research Data |
| Success payout | Fixed +1 Rabbit Data and +1 RD | Selected-species leftover-FD conversion and the tuning sheet's fixed RD award; one terminal receipt, no duplicate payout hook |
| Lab access | License purchase checks three completed runs | FD2 describes access through the species wallet/license, with no new three-run gate; do not import that gate silently |
| License/node/reset prices | POC constants and node prices of 1 | Use the FD2 tuning sheet; do not copy POC prices |
| Node ownership | Paid active allocations are refundable and do not become permanent node unlocks | Purchase adds permanent ownership once; reset clears active choices, refunds no species data, and keeps ownership/license |
| Prerequisites | Must be active at purchase and at snapshot resolution | FD2 purchase prerequisites are permanent ownership; validate the ownership graph before freezing the chosen active effects |
| Catalog | POC nodes and a separate Fertile Droppings ability | Explicit five-node FD2 catalog, retaining FD2 IDs/graph; no implicit TestData discovery |
| Loadout reset | Clears allocations and refunds recorded species-data spend | Retain approved FD2 reset fee/no-refund behavior; paid reset vs subsequent selection commands need one coherent implementation in T08 |

The no-refund/permanent-node rule is already approved in the FD2 PDD/SDD.
SG-005 section 1 describes a different approved Rabbit allocation contract.
For this work, the approved FD2 contract is a documented scoped exception to
that ownership/refund paragraph. General Rabbit/POC policy is not rewritten,
and the FD2 decision is preserved. Mutation/Genome separation, stable registry IDs,
all-species application and frozen launch state still apply.

## Capacity conflict reproduced

The original prerequisite graph leads through all five nodes. At two points
each, activating the complete prerequisite chain requires ten points. The
incoming capacity is eight. Exhaustively checking all 32 subsets against the
incoming `SpeciesGenomeProfile` and `GenomeSimulationSnapshot.Resolve` yields
six legal configurations, none containing `hare.warren-network`.

After all five nodes are permanently owned, the FD2 ownership-based proposal
has 31 subsets within capacity, including 15 containing Warren Network. Those
counts enumerate capacity eligibility only; they do not claim a patched
resolver or purchase flow has been implemented. Keep eight points and the
existing graph; adapt the FD2 prerequisite check rather than raising capacity,
dropping a node or silently bypassing validation for every catalog.

## SG-005 review of the proposed content

| Proposed effect | Capability / ecological pressure | Evidence still needed |
|---|---|---|
| Block increase | Reduces combat damage; can shift losses toward starvation and prey/resource pressure | Actual combat deaths/damage and full-route survival with the authored rules |
| Vision increase | Changes awareness radius; does not add hearing or adaptive intelligence | Encounter/contact changes, threat response, movement and food access |
| Movement increase | Changes travel/contact opportunities and movement cost exposure | Distance, energy spent, pursuit/escape, feeding access |
| Digestion bonus | Changes energy gained through feeding; can increase persistence and consumption | Food consumed, energy gained, starvation, downstream resource depletion |
| Pool-capacity increase | Allows larger ordinary replenishment; does not spawn Hares by itself | Actual grants, FD spending, later food pressure and losses |

Every node is Hare-specific, a single signed additive modifier, applied once
while active and frozen at launch before ordered expedition Mutations. None
uses reflection, conditional modifier syntax or a live asset as run authority.
Capacity costs remain provisional first-test terms. AV/tier estimates are
unestimated: no approved numeric AV reference panel/rates were established by
this review. Focal-species and ecosystem outcomes need separate measurements.
No balance panel, viability threshold or production-balance approval is added.

Existing concern checks: UPG-C01 requires registry/applier/telemetry/test parity
before an asset enters a run; incoming support is established only for the
five proposed IDs/values. UPG-C03 and C07 require frozen resolved values for
every participating species. UPG-C05 keeps POC/TestData discovery explicit.
This review authors no runtime asset and triggers no unsafe production action.
Recheck those constraints during integration and do not claim them satisfied
by the document alone.

## Save formats and the integration sequence

Both branches call their profile store version 3, but the serialized fields
mean different things. Local T02 writes speciesDataWallets/researchData and
projects Currency as RD; incoming writes currency/rabbitData/researchData,
completed runs/IDs and paid allocations. Its v2 fixture retains old Currency
and starts the new wallets at zero. Replacing either file with the other can
lose value or progression. A version-number comparison alone cannot select
the right migration.

An isolated three-way text preview finds one conflict block in each profile
class and in GenomeContractTests; MainMenuPlayModeTests merges cleanly as text.
The previews do not change checkout files, the index, branches or user saves.

Recommended sequence:

1. Review the five mappings/names in the tuning sheet; retain prior FD2 economy
   and ownership decisions. Keep T01 open until the resulting definition is
   approved and validated.
2. Preserve a recoverable local T02 checkpoint and integrate incoming work in
   an isolated checkout before touching the active Editor checkout.
3. Define an unambiguous next save schema (proposed version 4) that recognizes
   v2, local-v3 species wallets, and incoming-v3 Rabbit wallets. Preserve all
   raw originals, balances, licenses, legacy unlocks, allocation records and
   completion IDs. Reject ambiguous mixed shapes for recovery; do not guess or
   add two balances merely because both fields are present. Confirm how old
   generic Currency and incoming Research Data combine without loss/double
   counting using explicit fixtures before the migration writes anything.
4. Retain the incoming modifier/fingerprint/all-species runtime. Adapt profile
   purchases, ownership-based FD2 prerequisites, reset and success settlement
   to FD2. Preserve the old POC catalog's own semantics where needed; no global
   relaxation of snapshot validation.
5. Author the reviewed five-node definition through the existing asset path,
   preserve GUIDs, use one explicit catalog and validate all consumed prices,
   costs, prerequisites and modifiers together. Wire one terminal success
   settlement instead of allowing both incoming fixed awards and FD2 conversion.
6. Run migration fixtures for all three inputs and repeated/recovery loads;
   ownership/reset/prerequisite/capacity/launch-freeze checks; both branches'
   focused tests and required clean integration checks. T02's prior 26 local
   passes remain evidence for its own source, not for a merged version.

## Evidence and limits

The source archive, manifest (Git blob IDs and extracted-content hashes),
isolated build, contract check, result JSON and merge previews are under
`artifacts/fd2-t01-reconciliation-20261024/`. Exact incoming commit is recorded
above; no source checkout or user profile was modified by these checks.

- Incoming portable Release build: passed, zero warnings/errors.
- Contract review: 36 assertions passed; all five effect applications,
  catalog terms, the exhaustive capacity/prerequisite conflict, and all eight
  canonical tuning-sheet rounding examples checked. Rounding is checked
  against the mathematical specification, not a new payout implementation.
- Isolated merge preview: three files have one text conflict each; local
  source hashes unchanged. Text merge success is not semantic compatibility.
- No new Unity compile/test, scene/asset write, balance simulation, player
  build, merge, commit, push or Trello mutation was performed for this review.

Bevin approved the five effects/display names on October 24. Existing FD2
prices and no-refund ownership remain preserved. Integration validation and
the final authored-definition evidence are recorded in the next handoff.
