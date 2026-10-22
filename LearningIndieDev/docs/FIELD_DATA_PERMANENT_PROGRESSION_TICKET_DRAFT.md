# FD2 — Permanent Progression Ticket Draft

- **Prepared:** 2026-10-21
- **Status:** Published to Trello on 2026-10-21; actionable breakdown derived from [PDD-FD-002](FIELD_DATA_PERMANENT_PROGRESSION_PROCESS_DESIGN.md) and [SDD-FD-002](FIELD_DATA_PERMANENT_PROGRESSION_SOLUTION_DESIGN.md)
- **Owners/reviewers:** Bevin (Trello account: Nick Nevins)
- **Sprint placement:** Not assigned here; sprint planning belongs to Salty
- **Estimates:** Omitted; size/capacity has not been planned

## Ticket index

| ID | Actionable outcome | Depends on |
|---|---|---|
| FD2-T01 | Freeze the first permanent progression content and tuning sheet | Product direction approved |
| FD2-T02 | Migrate profiles to separate permanent wallets | FD2-T01 |
| FD2-T03 | Calculate the success settlement receipt | FD2-T01, T02 |
| FD2-T04 | Persist settlement once and retry safely | FD2-T02, T03 |
| FD2-T05 | Apply the conversion research level to future settlements | FD2-T01, T02, T03 |
| FD2-T06 | Purchase and persist the Hare Genome path license | FD2-T01, T02 |
| FD2-T07 | Purchase permanent Hare Genome nodes within prerequisites and capacity | FD2-T01, T02, T06 |
| FD2-T08 | Reset and rearrange a species' active Genome loadout | FD2-T02, T07 |
| FD2-T09 | Apply the active Genome snapshot to the next launch | FD2-T07, T08 |
| FD2-T10 | Present settlement and failure results | FD2-T03, T04 |
| FD2-T11 | Build the Gene Lab wallet, Research and Hare-tree interactions | FD2-T05–T09 |
| FD2-T12 | Verify migration and the complete permanent progression loop | FD2-T02–T11 |

## Ticket details

### FD2-T01 — Freeze first-slice progression content and tuning

**Outcome:** One reviewed, versioned first-test definition supplies every price,
cap, prerequisite and supported Hare node effect used by FD2.

**Work:** Confirm the existing Hare node subset/effects against the Genome
catalog and SG-005. Record the Hare license baseline (10 Hare Data), node price
(5 Hare Data each), Research reset fee, conversion-upgrade maximum level and
per-level Research Data costs. Confirm conversion rounding with examples.
Preserve 10 FD → 1 Hare Data, +10% multiplicative payout per upgrade level and
10 Research Data per successful journey as provisional test baselines unless
the implementation review changes them.

**Acceptance:** All consumed values are in one validated definition; every
node has stable ID, supported effect, prerequisite, cost and capacity cost;
Research cap/fees/costs and rounding examples are explicit; unresolved values
are not silently embedded in code.

### FD2-T02 — Migrate profiles to separate permanent wallets

**Outcome:** Profiles store Research Data and species-data balances separately
while retaining their existing Genome progress.

**Work:** Upgrade the version-2 profile schema. Copy legacy generic currency to
Research Data; initialize species balances to zero; preserve profile identity,
species unlocks, Genome path access, unlocked/active nodes, and other profile
data. Ensure new profiles start with zero permanent balances and locked paths.

**Acceptance:** A version-2 profile migrates once with the full generic balance
in Research Data; all Genome state survives; reloading/retrying migration does
not duplicate value; new profiles have all-zero wallets; focused serialization,
backup/recovery and snapshot-copy tests pass.

### FD2-T03 — Calculate the success settlement receipt

**Outcome:** A deterministic calculator converts leftover FD and produces a
complete immutable settlement breakdown.

**Work:** Use only leftover FD after journey purchases; credit only the
selected species wallet; grant fixed 10 Research Data on every success,
including zero leftover; round species data down using the finalized T01 rule.
Failure must not produce a settlement receipt.

**Acceptance:** Receipt includes journey ID, selected species, leftover FD,
conversion/rate level, rounded deposit, bonus, and before/after balances.
Domain tests cover 0/remainders/exact rate, multiple upgrade levels, overflow,
invalid inputs and species destination.

### FD2-T04 — Persist success settlement exactly once

**Outcome:** Repeated callbacks, results entry, app reload and save retry cannot
pay the same journey twice.

**Work:** Persist a unique profile/journey receipt atomically with both wallet
deposits. Return the prior receipt for a repeated journey ID. On save failure,
keep success pending and retry the same operation; do not show a final paid
result before persistence succeeds.

**Acceptance:** Same ID replay never changes balances twice; a new ID pays once;
simulated failure/retry and process reload preserve one receipt and one deposit;
the failure path leaves permanent balances unchanged.

### FD2-T05 — Add conversion research purchases

**Outcome:** A shared Genome Lab research upgrade raises later species-wallet
settlements.

**Work:** Purchase one level with Research Data using the T01 cost curve/cap.
Each level adds a 10% multiplicative payout bonus. Validate funds/cap and commit
deduction and level together. Freeze the research level used in each receipt.

**Acceptance:** Insufficient balance/cap/rejected purchase changes nothing;
successful purchase persists; prior receipts stay unchanged; later success
uses the new level and the defined integer rounding; tests cover costs, cap and
non-retroactivity.

### FD2-T06 — Unlock the Hare Genome path

**Outcome:** A player can use Hare Data to permanently license the Hare Genome
tree after earning it through a Hare-selected successful journey.

**Work:** Charge the provisional 10 Hare Data once. The license reveals tree
access but grants no node effect. Prevent duplicate purchase and path unlock
with other species' data.

**Acceptance:** Exact matching-wallet purchase succeeds once; insufficient
balance, duplicate license and wrong wallet leave state unchanged; license
survives restart; a journey remains launchable with no license.

### FD2-T07 — Purchase permanent Hare nodes

**Outcome:** Eligible Hare Genome nodes become permanently owned and active
without exceeding capacity.

**Work:** Use the T01 Hare catalog, prerequisite graph and provisional 5 Hare
Data/node cost. Require path license, prerequisites, sufficient wallet and
room under the existing capacity of 8. Deduct, unlock and activate in one
validated profile update.

**Acceptance:** Valid purchase atomically deducts, permanently unlocks and
activates; missing prerequisite/license, insufficient data, duplicate purchase
and capacity overflow change nothing; state survives reload; supported effects
match catalog records.

### FD2-T08 — Reset and arrange an active loadout

**Outcome:** A player can rebuild one species' active configuration without
losing purchased nodes.

**Work:** Charge the fixed Research Data fee for a reset. Clear that species'
active selections only, refund no species data, retain all permanent unlocks,
then allow free arrangement within capacity 8.

**Acceptance:** Reset affects only selected species; fee is deducted once;
unlocked nodes and species wallet remain; no refund occurs; over-capacity or
locked-node configurations are rejected atomically; reset fee behavior for an
empty loadout follows the T01 definition.

### FD2-T09 — Apply Genome loadout to the next launch

**Outcome:** The current active Genome configuration is frozen into the next
simulation/journey and does not mutate a run already in progress.

**Work:** Pass the validated per-species snapshot through the existing launch
composition path. Confirm Hare node effects use supported authored simulation
rules and affect every relevant population of that species.

**Acceptance:** Active nodes appear in the new run's immutable snapshot;
changes after launch do not alter the running simulation; reset/rearrangement
is reflected only on a later launch; integration test covers the initial Hare
catalog.

### FD2-T10 — Show permanent settlement and failure results

**Outcome:** Journey results explain the permanent economy outcome accurately.

**Work:** Success view shows leftover FD, rate/research level, species deposit,
Research bonus, and before/after balances. Failure view shows forfeited FD and
confirms permanent balances/unlocks remain. Success returns to Desktop only
after settlement persistence succeeds.

**Acceptance:** Display matches the persisted receipt; zero-leftover success
still shows bonus; failure shows no deposit; retry-pending state cannot imply
payment; result reentry does not pay again.

### FD2-T11 — Wire the Gene Lab permanent economy UI

**Outcome:** The existing Gene Lab provides usable Research and Hare Genome
wallet/purchase/loadout controls.

**Work:** Replace preview-only parts required for this slice. Display Research
Data, Hare Data, conversion research, path license, node prerequisites,
ownership/active state and capacity. Expose valid purchase, activation and
reset intents through the existing view-model/Noesis boundary.

**Acceptance:** UI is sourced from profile/catalog snapshots; all costs and
resulting balances are clear; disabled/rejected actions explain the reason;
success refreshes state; no wallet or settlement calculation lives in XAML or
the ViewModel.

### FD2-T12 — Verify migration and end-to-end permanent progression

**Outcome:** The team can complete and inspect a new and migrated profile's
permanent progression loop.

**Work:** Run focused profile, calculator, idempotency, Genome and UI checks.
Exercise new-profile success, failure, migrated profile success, Lab purchases,
reset, reload, and launch snapshot. Record the actual tested settings and
remaining balance/content questions.

**Acceptance:** All focused checks pass; no duplicate settlement or lost
profile progress; human review confirms result and Gene Lab clarity; evidence
distinguishes automated checks from human acceptance and balance tuning.

## Planning notes

All published tickets assign Bevin (Nick Nevins) and record Bevin as owner and
reviewer. Estimates remain omitted and no sprint placement was made. The cards
are upcoming work; this publication does not claim implementation has started.
T01 is a content/configuration gate; T02 can begin profile migration after its
wallet mapping is finalized. T03/T04 establish settlement before UI. T06–T09
build Genome progression after the first catalog is approved. T12 closes the
slice after integration and human review.


## Published Trello cards (2026-10-21)

All 12 cards were read back in **🎯 Upcoming Work** after publication. Trello member assignment is Bevin's account, Nick Nevins; every description records `Owner: Bevin | Reviewer: Bevin`. No sprint was assigned.

| ID | Card | Trello |
|---|---|---|
| FD2-T01 | Freeze first-slice progression content and tuning | [Open card](https://trello.com/c/QgVMHyWR/137-fd2-t01-freeze-first-slice-progression-content-and-tuning) |
| FD2-T02 | Migrate profiles to separate permanent wallets | [Open card](https://trello.com/c/AyAGpBx6/138-fd2-t02-migrate-profiles-to-separate-permanent-wallets) |
| FD2-T03 | Calculate the success settlement receipt | [Open card](https://trello.com/c/LqK96MhS/139-fd2-t03-calculate-the-success-settlement-receipt) |
| FD2-T04 | Persist success settlement exactly once | [Open card](https://trello.com/c/YhaRwN6I/140-fd2-t04-persist-success-settlement-exactly-once) |
| FD2-T05 | Add conversion research purchases | [Open card](https://trello.com/c/KM9iNvJF/141-fd2-t05-add-conversion-research-purchases) |
| FD2-T06 | Unlock the Hare Genome path | [Open card](https://trello.com/c/YNr1HuDw/142-fd2-t06-unlock-the-hare-genome-path) |
| FD2-T07 | Purchase permanent Hare nodes | [Open card](https://trello.com/c/bYWmUPYV/143-fd2-t07-purchase-permanent-hare-nodes) |
| FD2-T08 | Reset and arrange an active Genome loadout | [Open card](https://trello.com/c/TZe1J7oQ/144-fd2-t08-reset-and-arrange-an-active-genome-loadout) |
| FD2-T09 | Apply Genome loadout to the next launch | [Open card](https://trello.com/c/4vwCZxSH/145-fd2-t09-apply-genome-loadout-to-the-next-launch) |
| FD2-T10 | Show permanent settlement and failure results | [Open card](https://trello.com/c/Cqfr2WyU/146-fd2-t10-show-permanent-settlement-and-failure-results) |
| FD2-T11 | Wire the Gene Lab permanent economy UI | [Open card](https://trello.com/c/YJwJtQ2e/147-fd2-t11-wire-the-gene-lab-permanent-economy-ui) |
| FD2-T12 | Verify migration and end-to-end permanent progression | [Open card](https://trello.com/c/wxVgh3Q6/148-fd2-t12-verify-migration-and-end-to-end-permanent-progression) |
