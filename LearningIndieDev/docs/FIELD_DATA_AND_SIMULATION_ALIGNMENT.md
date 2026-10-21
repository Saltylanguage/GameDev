# Field Data and simulation planning alignment

- **Prepared:** 2026-10-16
- **Status:** combined planning reference; Field Data tickets selected in Active S5 (October 19-November 15, 2026)
- **Integrated sources:** Bevin's planning publication `d178a62` and Salty's
  `ProjectMain` through `341cc702`, merged on `BevBranch` at `0928359`
- **Planning ownership:** Salty/Josh retains sprint planning and the existing
  upgrade-system ownership. Implementation/review assignments remain to be agreed.

## Shared outcome

Make the Forest Edge/Hare journey useful to play and evaluate: the simulation
creates understandable pressures, the player can spend or save Field Data,
and temporary upgrades produce visible tradeoffs. Preserve the current Desktop
and profile foundations instead of rebuilding them as new features.

This document brings the two plans together without treating every idea as
one sprint. It identifies completed foundations, the defined Field Data slice,
related simulation work and remaining integration decisions. All 16 Field Data
tickets are now selected; see the [Active S5 control](Sprints/S5-control-record.md).

## Which document answers which question?

| Question | Source to use |
| --- | --- |
| What is the first Field Data slice? | [Field Data Journey Design v1.0](FIELD_DATA_JOURNEY_DESIGN.md), Bevin's reviewed rules and provisional values |
| How would we deliver and implement it? | [Process Design](FIELD_DATA_PROCESS_DESIGN.md) and [Solution Design](FIELD_DATA_SOLUTION_DESIGN.md), technical/process drafts |
| What broader simulation/upgrade work does Salty propose? | [S5 simulation and upgrades proposal](Sprints/S5-proposal-2026-10-15.md), historical proposal; broader candidates not selected by the Field Data kickoff |
| What is implemented today? | Merged code/tests, [journey prototype](JOURNEY_PROTOTYPE.md) and dated implementation handoffs |
| What was discussed earlier? | Both October 14 meeting records and the historical sections of the economy and sprint documents |
| What is scheduled? | [Active S5 control](Sprints/S5-control-record.md) and the [published ticket index](FIELD_DATA_TICKET_DRAFT.md); kickoff verified October 16 |

The earlier [Bevin-side sprint draft](Sprints/S5-planning-draft.md) remains on
hold. It is historical capacity/candidate discussion, not a second active S5.
The Step 1–5 checklist retains its 124 IDs; the FD-I01–FD-I07 implementation
stages remain a delivery breakdown, not sprint identifiers or estimates.

## What the merge already brings in

| Foundation | Source evidence | What still needs work |
| --- | --- | --- |
| Journey map and artwork contract | Scrollable 29-marker/35-connection map, three opening choices, wheel/hover behavior and normalized 5760 x 1080 panorama target | Later encounters remain placeholders; the five-node Field Data test route and shops are not implemented by the map |
| Journey reset boundary | `SpeciesSimulationPreview.PlayNextSimulation` clears carried currency for a new journey while preserving the standalone carryover path | Complete first-slice failure, terminal settlement and results-exit behavior still need their own checks |
| Permanent feature availability | Profile schema v2 stores `UnlockedFeatureIds` alongside existing species/Genome state | Accomplishment triggers, feature gates, permanent reward wallets and Genome purchases are not implemented by storage alone |
| Sprint administration | S4 is Closed in the incoming closeout record; P1-032 returned to Backlog; S4-04/05 remain Backlog | Launch-input review and actual application exit/relaunch acceptance remain open; S5 Field Data work is Active |

These are source/handoff observations. Fresh merge-test results belong in the
integration handoff; previous pass counts do not become current evidence merely
because the commits were merged. Do not reschedule these foundations as though
their implementations are missing.

## Overlap and the combined treatment

| Topic | Bevin's defined Field Data slice | Salty's related plan | Combined treatment |
| --- | --- | --- | --- |
| Earn, spend and save | Population income every 100 ticks; six 10-FD controls; three ordered income modifiers; shops and reorders | Define first costs/limits and meaningful temporary choices | Use the existing Field Data baseline; do not run a duplicate economy-design task |
| Passing a node | Six completed rounds with selected-species survival, no extra metric gate | Bevin guides provisional clear judgments; older viability tests are context | Use survival for this slice; broader scenario gates remain a later design decision |
| Continuous world | Three simulations and two shops, 18 rounds, same board/state/progression | Existing two-cycle map with more visual encounters | Reuse the current map/UI foundations, with an explicit test route; do not make every map marker playable to deliver this slice |
| Temporary upgrades | Ordinary free choices plus six signed Hare shop families; independent shop caps | Tune Trailblazer, Warren and Gardeners using freely mixed shared skills | Implement the defined mechanics, then judge their ecological effects using those build lenses |
| Currency and permanence | Journey FD is lost on failure; success displays/discards leftovers; no permanent payout in this slice | Separate species/biome wallets, success-only deposits and first permanent upgrade content | Keep permanent economy as a separate future package; do not reinterpret the 10-FD node bonus as a permanent reward |
| Unlock availability | Protect existing banked Desktop currency/unlocks during FD operations | Permanent availability can survive failure, including future accomplishment unlocks | Preserve schema-v2 state; this slice adds no accomplishment writer or unlock trigger. Future permanent availability is separate from run-earned currency |
| Evidence | 200 matched seeds per comparison arm, internal self-play, ordered receipts | Freeze inputs, measure simulation behavior and record accept/revise | Share input/provenance and reporting infrastructure; keep economy and ecology comparisons identifiable |
| Presentation | Show payouts, choices, prices and effective order clearly | Keep only UI/tooling needed to understand the simulation | Extend the canonical Noesis screen and accepted scrolling map; final art and broad Desktop polish remain separate |

## Related simulation work to retain separately

Salty's proposal includes the four-neighbor crowding surcharge, per-move energy
charges and whole-litter retry behavior, followed by shared-skill/build tuning.
Those directions are recorded in the [S4 control history](Sprints/S4-control-record.md)
and the [S5 proposal](Sprints/S5-proposal-2026-10-15.md). They are not Field Data
transactions and the merge does not implement them.

These changes can alter population-based income substantially. Field Data coding
can proceed against a declared current ecology fixture. If the team elects to
change ecology first, validate that change and freeze the resulting revision
before the 200-seed economy comparison. Do not compare an old-ecology control
with a new-ecology purchase path and attribute the difference to spending.

Within an economy comparison, related prices/effects may change together as a
documented bundle, as Bevin approved. That does not authorize silently changing
the underlying ecology between matched arms.

## Suggested work order for ticket planning

This dependency order informs the selected S5 Field Data tickets. Broader follow-on
content remains separate; owner/reviewer allocation is deliberately unassigned.

1. **Confirm the combined slice and inputs.** Select the Field Data route's
   entry point in the current map, identify the ecology revision used for its
   first test and keep unresolved permanent content out of that scope.
2. **Deliver the Field Data domain.** Use FD-I01–FD-I04 for definitions,
   retained-world settlement, interventions, rewards, signed effects and
   deterministic offers/reorders. Reuse the reset/profile work already merged.
3. **Integrate the player route.** Use FD-I05 for Noesis commands, results,
   shops, clear purchase effects and full five-node continuity. Preserve the
   accepted map and panorama contract.
4. **Make the evidence reproducible.** Use FD-I06 for the shared headless
   journey/action path, versioned receipts and bounded Unity/headless parity.
5. **Review the loop on matched inputs.** Use FD-I07 for the 200-seed arms and
   internal self-play, including intervention-versus-saving tradeoffs.
6. **Select follow-on content from the findings.** Schedule simulation-pressure
   changes/build tuning, permanent upgrades or other additions deliberately.
   A separately selected ecology correction may precede item 5 under the
   frozen-input rule above; none is silently mandatory for domain coding.

P1-032 and deferred profile acceptance may become separate bounded review work
if selected. Their remaining effort is not the original implementation estimate.

## Selection decisions and remaining implementation review

| Decision | Why it matters | Current state |
| --- | --- | --- |
| Which package is first: the Field Data loop, selected simulation corrections, or a bounded combination? | Determines dependencies and whether the work fits available time | Field Data FD-T01–FD-T16 selected for S5; broader ecology corrections remain separate |
| How is the explicit five-node test route reached from the current journey UI? | Prevents the 29-marker content map from silently replacing or expanding the agreed acceptance route | Bevin selected larger-map entry then explicit test-route selection on October 16; [FD-T01 review](FIELD_DATA_ENTRY_REVIEW.md) records the accepted composition boundary; Bevin waived further review on October 16 |
| Which ecology/scenario/build is frozen for the comparison? | Keeps population/income results comparable | Choose during preparation; no production population change approved here |
| How should roadmap terminology be aligned? | The [roadmap](../ROADMAP.md) still describes one six-round expedition/five choices, while this test journey contains three six-round simulations plus shops/node choices | Owner review needed before declaring a revised overall product contract; keep the first-test scope explicit |
| Is permanent Genome/reward content part of the selected work block? | Salty proposes it; Bevin's first FD slice defers it | Separate package unless explicitly selected; no invented wallets, prices or accomplishment thresholds |
| Owners, reviewers, estimates and reserve | Needed to turn packages into scheduled work | Four-week S5 approved; 57–95h selected, 16h protected reserve; owners/reviewers unassigned by request; see control for capacity risk |

## Ticket preparation rules

The [October 16 ticket draft](FIELD_DATA_TICKET_DRAFT.md) now contains 16
Field Data work items derived from the PDD/SDD. It retains unassigned owners/
reviewers and planning estimates; all 16 are published and selected for S5.

Start from the numbered Field Data checklist and FD-I stages rather than making
parallel tickets for the same requirement. Each selected ticket should state a
single reviewable outcome, concrete scope, dependencies, owner/reviewer,
remaining estimate and observable acceptance evidence. Label implemented
foundation, remaining acceptance, new implementation and later design distinctly.

Keep product choices separate from technical tasks that can proceed within an
agreed boundary. Do not mark implementation, balance, runtime review or a
200-seed comparison complete because a document or map marker exists.

## Not selected by this alignment

The October 16 kickoff selected the 16 Field Data tickets and four-week S5
window. New ecology values, permanent reward amounts, Genome node effects,
active-run resume, additional species, random-event gambling, final panorama
delivery and a generic modifier framework remain outside those tickets.
The next action is team allocation and FD-T01's bounded integration review.
