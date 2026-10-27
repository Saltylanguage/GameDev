# FD2 — Permanent Progression Ticket Draft

- **Prepared:** 2026-10-21
- **Status:** Published to Trello on 2026-10-21; T01 numeric baseline now captured in the [tuning sheet](FIELD_DATA_PERMANENT_PROGRESSION_TUNING_SHEET.md). Hare gameplay effects remain pending Salty's core-mechanics push. Breakdown derived from [PDD-FD-002](FIELD_DATA_PERMANENT_PROGRESSION_PROCESS_DESIGN.md) and [SDD-FD-002](FIELD_DATA_PERMANENT_PROGRESSION_SOLUTION_DESIGN.md)
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

**Live board closeout, 2026-10-26:** Bevin instructed that Trello be updated as
tickets finish. T01, T03 and T04 are now marked complete in Done with verification
comments posted and read back; T02 remains complete in Done. T04 code commit is
`ec30f70`, still local on `codex/fd2-t01-integration`. Earlier ticket paragraphs
below retain their dated implementation/approval history. Proof is
`artifacts/fd2-board-20261026/fd2-t01-t04-done.png`.

### FD2-T01 — Freeze first-slice progression content and tuning

**October 24 implementation verified:** Bevin approved the five effects and
display names. `FieldDataPermanentProgressionDefinition` now supplies the
versioned values; Unity authored the five existing assets with their GUIDs
preserved. Definition/adapter checks pass, along with 497 clean EditMode tests
(one dedicated-fixture skip). Both new migration PlayMode tests pass; seven
full-PlayMode failures reproduce identically on unchanged incoming ProjectMain.
T01 is technically ready for closeout in the isolated worktree. The live card
has not been changed in this implementation turn. See the
[integration handoff](handoffs/2026-10-24-2158-codex-fd2-t01-approved-definition-and-profile-integration.md).

**October 24 review prepared:** [Tuning sheet v0.2](FIELD_DATA_PERMANENT_PROGRESSION_TUNING_SHEET.md)
now proposes supported effects and truthful display names for all five existing
IDs. [The reconciliation review](FIELD_DATA_PERMANENT_PROGRESSION_T01_REVIEW.md)
records the 36-assertion isolated portable check, unreachable final-node issue
under incoming active prerequisites, differing purchase/reset rules and schema-v3
save shapes, and the integration sequence. Existing FD2 economy and permanent
ownership are retained. Effects/names still need approval; no asset authoring
or integrated runtime definition is claimed. T01 remains In Progress.

**Dependency refresh (2026-10-24):** Live card remains In Progress/incomplete.
Fresh origin/ProjectMain at `7ac2a54` includes Salty's Rabbit Genome mechanics
(`be98b03`), so the push is now available remotely. The five FD2 placeholder
effects are not yet reconciled with that catalog or reviewed against SG-005.
Incoming POC IDs, license/node prices and reset semantics differ from this
sheet. The incoming code is not merged into local BevBranch; T01 remains open.

**Outcome:** One reviewed, versioned first-test definition supplies every price,
cap, prerequisite and supported Hare node effect used by FD2.

**Trello status:** In Progress as of 2026-10-21. The numeric baseline and five-node
topology are recorded; effect mapping remains pending Salty's core-mechanics push.

**Work:** Freeze the recorded first-test baselines: Hare license 10 Hare Data;
node price 5 Hare Data each; all five placeholder node IDs with existing
prerequisites and 2 capacity points each; 8-point loadout capacity; 20 RD reset
fee; and three conversion-research levels priced at 10/20/30 RD. Preserve 10
FD → 1 Hare Data, +10% multiplicative payout per upgrade level and 10 Research
Data per successful journey as provisional test baselines. Keep node effects
undefined until Salty's core-mechanics changes are pushed, then map each node
to a supported effect and review against SG-005. See the [tuning sheet](FIELD_DATA_PERMANENT_PROGRESSION_TUNING_SHEET.md).

**Acceptance:** All consumed values are in one validated definition; every
every node has stable ID, prerequisite, cost and capacity cost; conversion
research prices/cap, reset fee and rounding examples are explicit; and no
unsupported effect is invented. The ticket remains open until core mechanics
are available and supported node effects are reviewed.

### FD2-T02 — Migrate profiles to separate permanent wallets

**Approved board closeout (2026-10-24):** Bevin approved closure after the 26
focused Unity passes. Live Trello card is complete in Done, and the additive
verification comment was posted and read back. Proof:
`artifacts/fd2-t02-20261024/trello-t02-done.png`. Source remains local and
uncommitted/unpushed. Incoming ProjectMain also changes profile schema v3;
integration and save-format reconciliation remain future work, and the local
test evidence does not validate that unmerged code.

**Outcome:** Profiles store Research Data and species-data balances separately
while retaining their existing Genome progress.

**Verified locally (2026-10-24):** Startup resolved; current Unity assemblies
compiled. Focused Live EditMode GenomeContractTests pass 17/17 and Main Menu
PlayMode tests pass 9/9, with zero failures/skips. This supersedes the pending
verification checkpoints below. T02 is ready for review/board closeout; source
is uncommitted/unpushed and no Trello update occurred. See the
[verified handoff](handoffs/2026-10-24-2040-codex-fd2-t02-verified-wallet-migration.md).

**Local status (2026-10-22):** Implementation is in the working tree on
`BevBranch`. Verification is pending because the live Unity test command
discovered zero tests and the existing PlayMode assembly predates the change.
Do not close the ticket or publish until the updated tests compile and pass.

**Continuation (2026-10-24):** Recovery is flushed before replacement and Currency
projects ResearchData directly. Six EditMode cases now cover migration replay,
existing recovery preservation, snapshot copies and invalid wallets. A .NET check
passes 30 snapshot assertions; Unity serialization and the focused Unity suites
remain pending behind the Editor's "Recovering Scene Backups" window. Live card
is still in Upcoming Work; no board change, commit or push occurred. See the
[handoff](handoffs/2026-10-24-2025-codex-fd2-t02-migration-recovery-and-validation.md).

**Work:** Migrate the current version-2 profile store to version 3. Copy legacy
generic currency to Research Data; retain the raw v2 JSON when no recovery
copy exists; initialize species balances to zero; preserve profile identity,
species unlocks, Genome path access, unlocked/active nodes, and other profile
data. Ensure new profiles start with zero permanent balances and locked paths.

**Acceptance:** A version-2 profile migrates once with the full generic balance
in Research Data; all Genome state survives; reloading/retrying migration does
not duplicate value; the original v2 JSON remains recoverable; new profiles
have all-zero wallets; focused serialization, backup/recovery and snapshot-copy
tests pass.

### FD2-T03 — Calculate the success settlement receipt

**October 26 implementation verified locally:** `FieldDataSettlement` produces
the immutable breakdown using the canonical definition and frozen level; it
changes no wallet. All 37 focused tests pass. Full clean EditMode passes 534,
fails zero and skips one parity-manifest fixture; portable build/self-test pass.
The existing profile test fixture now isolates its inputs and restores both
stored preferences. T03 is technically ready for scoped review/closeout; no
live card update occurred. See the
[T03 handoff](handoffs/2026-10-26-1229-codex-fd2-t03-deterministic-success-settlement.md).

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

**October 26 implementation verified locally:** Profile schema v5 retains frozen
pending inputs and lifetime receipts. Payment saves both deposits and the receipt
together; same profile/journey returns the prior receipt without writing. Save
failure and uncertain-save retry, profile switches, corrupted journals and old
formats have focused coverage. Full EditMode: 557 passed, zero failed, one parity
skip (23 T04 cases). Two distinct-process checks and 3/3 focused PlayMode cases
pass. No push or player-results wiring is implied. See the
[T04 handoff](handoffs/2026-10-26-1254-codex-fd2-t04-exactly-once-settlement-persistence.md).

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

**October 26 implementation verified locally:** Schema v6 retains conversion
research level; purchases atomically save the 10/20/30 RD deduction and next
level, with a cap of three and profile/expected-level duplicate protection.
Fresh settlements use owned research; pending/prior receipts are unchanged.
Focused research 25/25, full EditMode 582 passed/zero failed/one parity skip,
distinct-process write/read and focused PlayMode 4/4 pass. Player controls are
still T11. Code commit `e32367b` is local; the live card is now complete in Done
with verification comment posted and read back on October 26. See the
[T05 handoff](handoffs/2026-10-26-1351-codex-fd2-t05-conversion-research-purchases.md).

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

**October 26 implementation verified locally:** `TryPurchaseHareGenomeLicense`
charges exactly 10 Hare Data and saves access together, rejects duplicate/wrong
profile/insufficient/reentrant purchases and preserves other data. No node effect
is granted; the existing license flag uses schema v6. Focused license16/16,
full EditMode598 passed/zero failed/one parity skip, distinct-process write/read
and focused PlayMode6/6 pass, including unlicensed journey launch. Visible
Gene Lab controls remain T11. See the
[T06 handoff](handoffs/2026-10-26-2050-codex-fd2-t06-hare-genome-path-license.md).

Code commit `2e6fcd8` remains local. T06 is marked complete in live Trello Done
with verification comment posted and read back on October 26.

**Outcome:** A player can use Hare Data to permanently license the Hare Genome
tree after earning it through a Hare-selected successful journey.

**Work:** Charge the provisional 10 Hare Data once. The license reveals tree
access but grants no node effect. Prevent duplicate purchase and path unlock
with other species' data.

**Acceptance:** Exact matching-wallet purchase succeeds once; insufficient
balance, duplicate license and wrong wallet leave state unchanged; license
survives restart; a journey remains launchable with no license.

### FD2-T07 — Purchase permanent Hare nodes

**October 26 implementation and closeout:** Code commit `c58f7b5` remains local;
T07 is marked complete in live Done with verification comment
`#comment-6adffac4ab95c50adf0bc641` posted and read back. All32 new node cases,
full EditMode630 passed/zero failed/one parity skip, independent process checks
and focused PlayMode7/7 pass. `TryPurchaseHareGenomeNode` validates the owning
profile, canonical node/price, license, permanently owned ancestors and full
catalog capacity. It saves 5 Hare Data deduction, permanent ownership and
activation together with the existing atomic profile commit path. No refundable
POC allocation or schema change is added. Owned inactive duplicates, other
wallets, invalid IDs, capacity overflow and failed saves cannot publish a
purchase. Player controls remain T11; T08/T11 use catalog capacity rather than
the POC profile's one-point fallback. Validation/board closeout is recorded in
the T07 handoff. Board proof: `artifacts/fd2-board-20261026/t07-done.png`.

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

**October 26 decision and closeout:** Code commit `d90c2f5` is local; T08 is
complete in live Done with verification comment
`#comment-6ae013d687320fb2409a962e` posted and read back. All36 new cases,
full EditMode666 passed/zero failed/one parity skip, independent-process checks
and focused PlayMode8/8 pass. Proof: `artifacts/fd2-board-20261026/t08-done.png`.
Bevin selected a 20 RD reset for each replacement
loadout. Free selection applies from an empty loadout; after applying a nonempty
selection, a different selection needs another paid clear. Identical selection
is a no-op, and an empty reset is rejected without a fee. The existing POC
refund flow remains separate. Implementation and verification are recorded in
the T08 handoff.

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

**October 27 implementation:** Permanent Hare loadouts use the canonical FD2
owned-prerequisite resolver; other/POC profiles keep their existing catalog rules.
The Desktop binds a profile-owned first-start capture, so direct next-journey
commands also refresh the saved configuration. Opening Mutations are retained,
all participating species receive their own resolved effects, and active
journeys keep their frozen values through phases/checkpoints/results. Field Data
test-route configuration retains its prepared Genome. Detailed validation and
board closeout are recorded in the T09 handoff.

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
