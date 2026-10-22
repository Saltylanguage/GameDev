# FD2 — Journey Settlement and Permanent Genome Progression

- **Document:** SDD-FD-002
- **Status:** Proposed technical design for approved product direction
- **Prepared:** 2026-10-21
- **PDD:** [FD2 Process Design](FIELD_DATA_PERMANENT_PROGRESSION_PROCESS_DESIGN.md)
- **Ticket breakdown:** [FD2 implementation tickets](FIELD_DATA_PERMANENT_PROGRESSION_TICKET_DRAFT.md)
- **Source baseline inspected:** `BevBranch`, 2026-10-21

## 1. Design intent and current source

FD2 adds permanent wallet settlement and a working first Genome progression
loop on top of the accepted FD1 journey. Keep run-local FD isolated from
profile currency. Reuse the existing profile and Genome contracts where they
fit; avoid introducing a second competing source of truth.

Current source has a single `currency` field in `Helper_ProfileSession`'s
version-2 profile record and a single `Currency` value in
`ProfileSessionSnapshot`. `SpeciesGenomeProfile` already keeps permanent
`unlockedNodeIds` separate from `activeNodeIds`, and `GenomeContract` sets
`ActiveCapacity` to 8. Existing profile methods include generic currency
mutation, Genome path unlock, and Genome configuration. `GenomeCatalogProvider`
and the Desktop view model already supply a Gene Lab presentation seam; parts
of the XAML remain preview-only. These facts describe the source baseline, not
FD2 implementation.

Related source: `Assets/Scripts/Game/Profile/Helper_ProfileSession.cs`,
`ProfileSessionSnapshot.cs`, `Assets/Scripts/Game/Species/SpeciesGenomeContract.cs`,
`GenomeCatalogProvider.cs`, `SpeciesProgression.cs`,
`Assets/UI/GalapagOS/Scripts/VM_GalapagOS_Desktop.cs`, and
`Assets/UI/GalapagOS/XAML/Panels/V_Panel_GalapagOS_Desktop.xaml`.

## 2. Ownership boundaries

| State or calculation | Owner | Rule |
|---|---|---|
| Journey FD | Existing FD journey session | Run-local only; never stored in a permanent species wallet |
| Species wallets, Research Data, licenses and Genome node ownership/loadout | Profile session snapshot and its serialized record | One persistent profile source of truth; all edits produce and persist a new validated snapshot |
| Settlement math | Pure permanent-settlement calculator | Integer/rational arithmetic, deterministic inputs, no UI dependency |
| Settlement idempotency | Profile-persisted settlement receipt keyed by profile and unique journey ID | A replay returns the original receipt and does not credit twice |
| Genome catalog/prerequisites/capacity | Existing Genome catalog/contract with an explicit cost/effect extension | Definitions are data; purchasing and validation stay in the domain/profile boundary |
| Rendering and commands | Gene Lab ViewModel / Noesis view | Display snapshots and submit intent; do not own wallet math or persistence |

Do not put permanent conversion on round or node settlement. It is a single
terminal success operation after FD1 has completed. The failure terminal path
does not call it.

## 3. Profile model and migration

Replace the authoritative generic wallet with a Research Data balance plus
species-keyed balances. Preserve a read-only compatibility projection only if
existing callers require it during migration; migrate every caller to explicit
currency intent before removing the generic API. Validate nonnegative balances
and checked additions/subtractions before committing.

The serialized record needs:

- `researchData`;
- stable species IDs paired with species-data balances;
- permanent path-license species IDs or a validated equivalent;
- existing per-species unlocked and active node IDs;
- a bounded list/map of unique journey settlement receipts, each with profile
  identity, journey identity, selected species, source leftover FD, conversion
  research level/rate, species deposit, Research bonus, pre/post balances, and
  schema/design version.

Migrate profile store version 2 to the new version once, copying legacy
`currency` to `researchData`, setting species balances to zero, and preserving
all other profile values. Migration must be repeat-safe: if save interruption
causes the old record to load again, the conversion still produces one
Research Data balance and no duplicate currency. Retain the project's backup
and recovery behavior. A new profile starts at zero for all balances and with
no Genome licenses.

Use stable `SpeciesId` values; do not key currencies by display name or enum
position. Extend snapshot copy/with methods to preserve new fields, and update
serialization, equality/change notifications, and tests together.

## 4. Success settlement

Expose one domain operation that receives a completed journey identity,
selected Genome species, leftover FD, and the conversion research level frozen
for the settlement. Validate success, nonnegative FD, known species, and
absence/presence of a receipt before applying changes.

Provisional baseline formula:

```text
base species data = floor(leftover FD / 10)
species deposit = floor(base species data × 1.10^research level)
Research Data bonus = 10
```

The 10-FD-per-species-data exchange and 10 Research Data bonus are provisional
first-test settings. To avoid floating point, represent the multiplier as a
bounded rational or checked integer numerator/denominator and floor only at the
specified final result. Confirm with product whether flooring occurs before or
after the percentage multiplier; the default design above floors once after
the multiplier. The settlement ticket must record the exact resolution before
shipping that calculator. Check overflow and both wallet additions before
changing either wallet or recording the receipt. Commit both deposits and the
receipt as one profile update.

The operation must be idempotent. Same profile + journey ID returns the
existing receipt without another deposit. A different journey ID settles
normally. Persist the receipt with the balances in one saved snapshot. If the
save fails, retain a pending success result and retry the same transaction;
never show an uncommitted deposit as final and never create a second receipt.
Receipts must survive app restart and results-screen reentry. Define a safe
retention policy so duplicate protection is not lost while old journeys can
still be reopened.

Failure only clears/disposes the journey-local FD session and presents the
lost-FD amount. It must not touch Research Data, species wallets, licenses or
node state.

## 5. Conversion research

Add one shared conversion-research record to profile state: current level,
small fixed maximum level, and stable purchase receipts or a sufficient
transaction identity. Purchase costs Research Data and increase with each
level. Each level multiplies species deposit by 1.10; a level purchased after a
settled journey cannot retroactively alter that receipt.

The exact cap and cost schedule are intentionally unresolved product tuning.
Keep them in a small validated definition, not scattered constants. Until the
values are agreed, show the feature as design-in-progress rather than silently
choosing a progression curve. A purchase is atomic: check level cap and
balance, deduct, then increment and persist together.

## 6. Genome license, permanent node purchases, active loadout

Treat the existing `unlockedNodeIds` as permanent node ownership and
`activeNodeIds` as the current allocation. Add/extend a validated catalog for
path license cost, node cost, prerequisite IDs, and active capacity cost. The
initial catalog includes Hare only. Provisional prices are 10 Hare Data for
the Hare path license and 5 Hare Data per eligible node; the actual small node
set, effects, and any per-node capacity cost need finalization against authored
catalog data and SG-005 before implementation is called done.

Path license grants access to inspect/buy from the tree and has no gameplay
effect itself. A node purchase requires the path license, an owned species
wallet with enough data, all authored prerequisites already permanently
unlocked, and enough active capacity for immediate activation. If any condition
fails, make no balance or profile change. On success, deduct species data,
add permanent ownership, and add it to active configuration in one profile
update. At full capacity, block purchase; do not silently unlock but leave
inactive.

Reset takes a selected species ID and fixed Research Data fee. It clears that
species' active selections, leaves its licenses and all permanently unlocked
nodes intact, and refunds no Species Data. After one reset the player may freely
select any owned nodes within the 8-point capacity. Reset of an already-empty
loadout should be rejected or clearly defined at implementation time; avoid
charging for a no-op without an explicit product decision.

Every launched simulation receives the validated active Genome snapshot. Lab
changes affect only a later launch. For FD2, implement the Hare catalog using
existing authored supported effects; do not invent new stat behaviors. If the
existing catalog has no sufficiently defined effects, make selecting the
first implementation subset a prerequisite ticket rather than fabricating
effects.

## 7. UI and player flow

Use the existing GalapagOS Desktop's Gene Lab window, its view-model, and
Noesis bindings. Add a shared Research section and species wallet display;
show a Hare tree/license state and locked/owned/active/capacity states; expose
purchase, activate/deactivate, reset, and conversion-research commands with
cost and resulting balance. Keep commands unavailable when prerequisites,
capacity, or wallet checks fail. ViewModels format receipts but do not compute
deposits or edit profile records directly.

The successful journey result displays its immutable receipt before the player
returns to the Desktop. The failure result displays the forfeited FD and
retained permanent balances/unlocks. Starting another journey must not
accidentally re-settle a closed one.

## 8. Verification

Add focused domain tests for:

- 0, below-rate, exact-rate, remainder, and large FD conversion;
- level 0 and multiple 10% levels, rounding, overflow, and invalid input;
- zero-leftover success still granting 10 Research Data;
- selected-species-only deposit and no deposit on failure;
- same journey replay, results reentry, reload, and retry after simulated save
  failure paying exactly once;
- v2 profile migration, preserving Genome unlocks/loadouts, repeat-safe load,
  and fresh-profile zero state;
- Genome license purchase, node price/prerequisite/capacity gates, atomic
  success, permanent unlock persistence, no-refund reset and per-species scope;
- conversion-research cost curve, cap and non-retroactivity once values are set;
- UI command-state and balance refresh after successful/rejected operations.

Then validate a complete success flow, a failed flow, and a migrated profile
through the actual Desktop/Gene Lab screens. Record what tests ran and what
human acceptance remains; unit tests do not establish player clarity or balance.

## 9. Risks and decision points

- Save atomicity: the two wallets and the receipt must not diverge across a
  failed save or app restart.
- Legacy currency meaning: migration assigns all generic currency to Research
  Data, so every legacy UI and purchase path must be accounted for.
- Rounding: layering the 10% upgrade on top of the base floor needs one agreed
  whole-number rule and explicit examples.
- Existing Genome data is metadata-first and some UI remains preview-only;
  node effects/costs may not yet be purchase-ready.
- The Research upgrade cap, cost curve, reset fee, exact Hare node selection,
  and specific node effects are not decided. Keep these as explicit content
  tuning, not hidden engineering assumptions.
- Settlement receipt retention must preserve exactly-once behavior across
  profile backup/recovery and journey history retention.

## 10. Not included

No implementation is implied by this design. Fox/Plant/multispecies trees,
separate mutation progression, biome currencies, failure rewards, refunds for
permanent node ownership, and balancing every Genome configuration are later
work. Product choices above are approved direction; provisional rates/prices
and unresolved catalog details are not final balance approval.
