# FD2 — Journey Settlement and Permanent Genome Progression

- **Document:** PDD-FD-002
- **Status:** Approved product direction; implementation workflow proposed
- **Prepared:** 2026-10-21
- **Product owner:** Bevin
- **Related baseline:** [Accepted Field Data journey](FIELD_DATA_PROCESS_DESIGN.md), [Genome direction](UPGRADE_SYSTEM_DIRECTION.md), [Studio balance guideline](Studio%20Guidelines/SG-005-UPGRADE-AND-ECOLOGY-BALANCE.md)
- **Technical companion:** [FD2 Solution Design](FIELD_DATA_PERMANENT_PROGRESSION_SOLUTION_DESIGN.md)
- **Ticket breakdown:** [FD2 implementation tickets](FIELD_DATA_PERMANENT_PROGRESSION_TICKET_DRAFT.md)

## 1. Purpose

FD1 established a playable five-node journey with a temporary Field Data (FD)
wallet. FD2 carries successful-journey value back to the Desktop and lets the
player spend permanent data in the Gene Lab. This document describes what the
player does and what the team needs to deliver. It does not claim that this
permanent-economy flow is implemented.

## 2. Player-facing outcome

After completing a journey, the player sees how its unspent FD becomes data for
the species selected for that journey, receives a fixed Research Data reward,
and returns to the Desktop. In the Gene Lab, the player can unlock that
species' Genome path, permanently buy nodes, choose which unlocked nodes are
active within capacity, or pay Research Data to reset that species' active
loadout. Shared conversion research is purchased with Research Data.

Failure forfeits the journey's unspent FD. It does not remove any permanent
wallet balance, path access, or Genome node already earned before the failed
journey.

## 3. End-to-end process

```text
Desktop → choose species and start journey
        → play the existing Field Data route
        → journey fails? show FD forfeited; retain permanent profile state
        → journey succeeds? settle once:
             unspent FD → selected species wallet
             fixed success bonus → Research Data
        → show settlement breakdown and before/after balances
        → return to Desktop → player opens Gene Lab when ready
        → use species data to unlock its path and permanently buy nodes
        → active nodes apply to the next simulation/journey launched
```

The journey outcome and the Genome Lab are separate screens. Success returns
the player to the Desktop; the player chooses when to open the Lab. A first
journey is allowed with no path license and no active Genome nodes.

## 4. Success settlement

Settlement happens automatically once when the full journey succeeds, before
the results screen is shown. Its input is the leftover FD balance after every
journey purchase. At the first-test baseline, every 10 FD converts to 1 data
unit in the selected species' wallet; fractions are rounded down and discarded.
The global conversion research level raises the species-data payout by 10%
multiplicatively per level. The current level is frozen for that settlement.

Every successful journey also grants a provisional fixed 10 Research Data,
including a success with zero leftover FD. The selected journey species is the
only species wallet credited. Research Data funds global research and Genome
reset costs; a species wallet funds that species' path license and nodes.

The result screen shows leftover FD, conversion rate and research level,
rounded species-data deposit, fixed Research Data bonus, and before-and-after
balances for both wallets. Leaving results returns to the Desktop. A completed
settlement cannot be repeated by reopening results, continuing twice, or
reloading the profile.

## 5. Failure

On journey failure, show how much FD was forfeited and state that permanent
balances and unlocks remain. Award no species data or Research Data for that
failed journey. Do not roll back prior profile purchases. A journey that has
already succeeded and settled remains settled even if the results screen is
reopened.

## 6. Gene Lab process

1. A fresh profile begins with zero in every species wallet, zero Research
   Data, and all Genome paths locked. Its first journey can still start.
2. A successful journey deposits data only into the wallet matching its
   selected Genome species. The player can then unlock that species' path.
3. A path license grants access to the tree; it has no gameplay effect by
   itself. The first implementation includes the Hare tree only. Hare path
   license cost is provisionally 10 Hare Data.
4. The player purchases eligible nodes using that species' data. Nodes remain
   permanently unlocked. Each purchase activates the node immediately if the
   species' active capacity permits; otherwise the purchase is blocked.
5. The player may arrange unlocked nodes within the existing eight-point
   capacity. Effects are captured for the next simulation/journey at launch;
   an in-progress simulation never changes.
6. One species' active loadout can be reset at a time for a fixed Research Data
   fee. The reset removes all active selections for that species, refunds no
   species data, and leaves all permanent unlocks intact. The player can then
   freely reselect unlocked nodes within capacity.
7. Shared conversion research appears in the Gene Lab's Research section. It
   costs Research Data, has a small fixed level cap, and its price rises with
   each level. Each purchased level affects later success settlements only.

For the first-test baseline, Genome nodes cost 5 matching species data each,
subject to their authored prerequisites. The node list, node effects, reset
fee, exact conversion-research cap, and its price schedule remain tuning or
content decisions to finalize during implementation. Do not invent effects or
prices to fill those gaps.

## 7. Wallets and migration

Permanent balances consist of separate species-data wallets and one shared
Research Data wallet. Journey FD remains temporary and separate. Failure clears
only journey-local value. A legacy profile's single generic currency balance
migrates to Research Data; existing species access and all unlocked and active
Genome nodes are preserved. Newly created profiles start with no permanent
currency or path licenses.

## 8. Delivery process and acceptance evidence

Implement profile storage and migration first, then deterministic settlement,
transaction retry protection, Genome purchases/loadouts and Lab UI. Finish with
end-to-end success, failure, migration and duplicate-entry checks, followed by
team review of the actual first playable values. Exact cap, fee and catalog
contents can be finalized as implementation tuning; they must be recorded
before the affected behavior is called complete.

The slice is ready for internal acceptance when a new profile can complete a
journey and receive the correct two deposits once; a failed journey loses only
its FD; existing profile progress survives migration; the player can unlock
the matching Hare path and purchase/configure eligible nodes without exceeding
capacity; reset costs Research Data and refunds no species data; and the UI
shows the balances and effects clearly. Focused tests must cover arithmetic,
retry/save behavior, and profile migration. A passing code test alone is not
player acceptance.

## 9. Out of scope

- Cashing out FD on journey failure.
- Converting or crediting a species other than the one selected for the journey.
- A separate permanent Mutation system; permanent mutations are Genome nodes
  for this slice.
- Fox, Plant, multispecies or biome trees; new node effects beyond the existing
  authored Genome catalog; and broad content/balance expansion.
- Research Data rewards on failure, additional event-based rewards, or any
  alternate conversion route.
- A profile reset or refund of permanent Genome node purchases.

These boundaries keep FD2 focused on the first permanent progression loop.
