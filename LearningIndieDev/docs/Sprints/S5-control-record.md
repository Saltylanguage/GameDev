# Sprint 5 control record - Field Data journey implementation

- **Sprint:** S5
- **Status:** Active; kickoff verified 2026-10-16
- **Work window:** October 19-November 15, 2026, inclusive (four weeks)
- **Trello control:** [Sprint 5 Control Record - Field Data journey implementation](https://trello.com/c/hqQczLQ9/135-sprint-5-control-record-field-data-journey-implementation)
- **Execution state at kickoff:** 16 cards in Current Work; none started; 79 acceptance items unchecked
- **Owner / reviewer allocation:** Unassigned on all 16, explicitly requested by Bevin for team allocation
- **Carry-over:** None. S4 remains Closed; P1-032 and S4-04/05 remain Backlog.

## Goal and scope

Deliver and internally evaluate Simulation -> Shop -> Simulation -> Shop ->
Simulation: 18 rounds, two shops, one retained ecosystem and journey wallet.
Implement population controls, ordered income, signed Hare shop effects,
free rewards, paid reorders, Noesis screens and reproducible headless evidence.
Use the [reviewed design](../FIELD_DATA_JOURNEY_DESIGN.md),
[PDD](../FIELD_DATA_PROCESS_DESIGN.md), [SDD](../FIELD_DATA_SOLUTION_DESIGN.md)
and [ticket specification](../FIELD_DATA_TICKET_DRAFT.md).

Permanent currency/Genome content, broader ecology corrections, new species,
random-event gambling, active-run save/resume and final art are separate work.
The existing scrolling content map and standalone six-round path remain usable;
FD-T01 resolves the explicit test-route entry and compatibility review.

## Capacity and user-approved exceptions

Bevin selected all 16 tickets after the capacity warning and requested four
weeks from October 19. Both developers' planning availability is 10-15h/week:
40-60h each, 80-120h combined. Use the conservative 80h floor and protect 16h
(20%) for integration/review, leaving 64h of feature capacity at that floor.
The tickets total **57-95 active team-hours**, excluding unknown machine time
and a new tuning iteration. The upper estimate exceeds that conservative
feature allowance by **31h**. Re-estimate openly; do not assume maximum
availability or consume reserve silently.

This is an approved four-week exception for S5; the normal two-week cadence
is unchanged. Owners/reviewers remain unassigned by explicit user instruction.
Selection records intended work, not completion or a guarantee of the upper
estimate fitting available capacity. S6/S7 dates need reforecasting after S5.

## Selected work

**October 17 delivery update:** implementation evidence is complete through
FD-T12 plus FD-T14/15. The five-node Desktop route is playable; FD-T13 retains
its independent human visual-acceptance criterion and FD-T16 needs actual
internal self-play and accept/revise. This is not Sprint 5 or balance acceptance.
See the [current handoff](../handoffs/2026-10-17-0202-codex-field-data-rendering-parity-and-cohort.md)
and [self-play record](../FIELD_DATA_SELF_PLAY_REVIEW.md). Changes remain local
and unpushed; original kickoff estimates and ownership exceptions remain.

Every linked card has Sprint ID S5, its stable FD-T task ID, work dates,
estimate, dependencies, outcome, source links and a native acceptance checklist.
All work is Not started as of the October 16 kickoff audit.

| Task / Trello | Outcome | Active hours | Depends on | Owner / reviewer |
| --- | --- | ---: | --- | --- |
| [FD-T01](https://trello.com/c/kl9F3o2l/119-fd-t01-agree-the-test-route-entry-and-first-implementation-boundaries) | Agree the test-route entry and first implementation boundaries | 1–2 | Team review | Unassigned |
| [FD-T02](https://trello.com/c/3ZTf7hmC/120-fd-t02-freeze-and-validate-the-starter-journey-definition) | Freeze and validate the starter journey definition | 3–5 | FD-T01 | Unassigned |
| [FD-T03](https://trello.com/c/kdncn6mH/121-fd-t03-calculate-ordered-income-with-a-complete-breakdown) | Calculate ordered income with a complete breakdown | 3–5 | FD-T02 | Unassigned |
| [FD-T04](https://trello.com/c/LKzm9j7r/122-fd-t04-run-all-18-rounds-in-one-retained-journey-session) | Run all 18 rounds in one retained journey session | 4–7 | FD-T02 | Unassigned |
| [FD-T05](https://trello.com/c/3cGXubBh/123-fd-t05-settle-awards-once-and-enforce-the-journey-wallet-lifecycle) | Settle awards once and enforce the journey wallet lifecycle | 4–6 | FD-T03, FD-T04 | Unassigned |
| [FD-T06](https://trello.com/c/CMld0U0x/124-fd-t06-add-the-six-atomic-population-controls) | Add the six atomic population controls | 4–7 | FD-T05 | Unassigned |
| [FD-T07](https://trello.com/c/LFKRoQpN/125-fd-t07-apply-the-six-signed-hare-shop-effect-families) | Apply the six signed Hare shop-effect families | 5–8 | FD-T02, FD-T04 | Unassigned |
| [FD-T08](https://trello.com/c/XB4jwxit/126-fd-t08-offer-and-resolve-free-round-node-rewards) | Offer and resolve free round/node rewards | 3–5 | FD-T05 | Unassigned |
| [FD-T09](https://trello.com/c/Hrl7GNAo/127-fd-t09-generate-and-purchase-deterministic-shop-offers) | Generate and purchase deterministic shop offers | 4–7 | FD-T05, FD-T07 | Unassigned |
| [FD-T10](https://trello.com/c/C9D3evir/128-fd-t10-buy-random-or-chosen-income-reorders) | Buy random or chosen income reorders | 3–5 | FD-T09 | Unassigned |
| [FD-T11](https://trello.com/c/GZFJ6QPo/129-fd-t11-show-round-income-rewards-and-population-actions-in-noesis) | Show round income, rewards and population actions in Noesis | 4–6 | FD-T06, FD-T08 | Unassigned |
| [FD-T12](https://trello.com/c/J2EqiGv3/130-fd-t12-show-shops-and-reorder-controls-in-noesis) | Show shops and reorder controls in Noesis | 4–7 | FD-T10, FD-T11 | Unassigned |
| [FD-T13](https://trello.com/c/FGFDVXcQ/131-fd-t13-author-the-test-route-and-verify-the-complete-desktop-flow) | Author the test route and verify the complete Desktop flow | 3–5 | FD-T01, FD-T02, FD-T11, FD-T12 | Unassigned |
| [FD-T14](https://trello.com/c/XB3Yc2j7/132-fd-t14-run-the-same-journey-domain-headlessly-and-export-replay-evidence) | Run the same journey domain headlessly and export replay evidence | 6–10 | FD-T13 | Unassigned |
| [FD-T15](https://trello.com/c/nnujHAoK/133-fd-t15-freeze-and-run-the-matched-200-seed-comparison) | Freeze and run the matched 200-seed comparison | 3–5 | FD-T14 | Unassigned |
| [FD-T16](https://trello.com/c/osmzcZVf/134-fd-t16-review-internal-self-play-and-record-accept-revise) | Review internal self-play and record accept/revise | 3–5 | FD-T15 | Unassigned |

Follow the dependency fields. FD-T03/04 can follow FD-T02 independently;
agree ownership of shared files when allocating work. At kickoff, the next
step was team allocation and FD-T01's entry/compatibility review; the review
is now complete and FD-T02 has also been implemented.

## Exit evidence

- Focused correctness checks accompany implementation; awards/purchases settle once and reject invalid changes atomically.
- Complete the full 18-round Desktop flow and both shops, retaining world/wallet/upgrades and showing readable ordered payouts and purchase effects.
- Verify journey resets/failure and results-exit behavior without changing previously banked Desktop currency, unlocks or feature availability.
- Produce versioned action/receipt evidence and bounded Unity/headless replay parity.
- Freeze the scenario/revision and 200 matched seeds per comparison arm; compare weaker/stronger paths and record internal self-play accept/revise.
- Record failed, skipped and unperformed checks explicitly. No numeric ecology gate or external-player research requirement is added.

## Verified kickoff evidence - 2026-10-16

**Later October 16 progress:** FD-T01 is accepted and complete with all four
criteria closed under Bevin's explicit review exception. Bevin selected larger-map entry followed by explicit
five-node test selection. [Source review](../FIELD_DATA_ENTRY_REVIEW.md) and
PDD/SDD updates are accepted; Bevin waived further review. No Salty review is
claimed. FD-T02 may proceed. Other 15 cards remain
Current Work. The counts below retain the original kickoff snapshot.

The live board was read back after publication: Current Work 16, Upcoming Work
0, In Progress 0, Blocked 0, Done 13 and Backlog 26. Each card's description,
Current Work placement and exact unchecked acceptance criteria were verified;
the total is 79. The control reads Active with the approved dates/exceptions.
Local UI audit files/screenshots are under ignored
`artifacts/s5-kickoff-20261016/`; those are publication evidence, not gameplay
or balance evidence. No gameplay implementation or new Unity/seed run was
performed during kickoff.

## Current execution checkpoint - 2026-10-16

After FD-T04 and FD-T05 moved from Current Work to In Progress, the board
reads 16 Done, 2 In Progress, 11 Current Work, 0 Upcoming, 0 Blocked, and 26
Backlog (derived from the verified post-T04 snapshot and verified T05 move).
All criteria for T01 (4), T02 (5), and T03 (5) are checked; T04's five and
T05's five criteria remain open, leaving 65 acceptance items open. Focused
Unity EditMode verification now passes 24/24 Field Data tests; this does not
close T04 or T05, whose remaining integration and player-flow criteria are
still open. The kickoff
count above is retained as its historical snapshot. The T02 definition,
T03 ordered-income, T04 retained-session, and T05 wallet-ledger records are in
the [T02 checkpoint](../FIELD_DATA_SOLUTION_DESIGN.md#15-fd-t02-implementation-checkpoint---2026-10-16),
[T03 checkpoint](../FIELD_DATA_SOLUTION_DESIGN.md#16-fd-t03-ordered-income-checkpoint---2026-10-16),
and [T04 checkpoint](../FIELD_DATA_SOLUTION_DESIGN.md#17-fd-t04-retained-journey-session---2026-10-16),
and [T05 checkpoint](../FIELD_DATA_SOLUTION_DESIGN.md#18-fd-t05-wallet-settlement---2026-10-16).
