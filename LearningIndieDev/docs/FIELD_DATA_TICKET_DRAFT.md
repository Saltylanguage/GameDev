# Field Data implementation tickets - Sprint 5

- **Prepared:** 2026-10-16
- **October21 acceptance:** Bevin explicitly accepts the initial five-node slice as a good starting point and confirms APS fixed in play. The local T13 human presentation criterion and T16 keep-loop decision are satisfied; missing timing/matched-human-path metadata is preserved as unrecorded, not invented or made into another acceptance gate. Further content remains unspecified. This records human acceptance without claiming a fresh Trello audit, all16 external cards closed or publication. See the [acceptance record](FIELD_DATA_SELF_PLAY_REVIEW.md).
- **October20 human playthrough:** Bevin completed the whole route and said it felt good, with near population misses on both animal sides. Positive keep-loop feedback is recorded in the [self-play record](FIELD_DATA_SELF_PLAY_REVIEW.md); APS reporting was the specific defect, now corrected with159 focused EditMode/4 PlayMode passes (2 graphics skips). Missing human timing/matched-path details are not fabricated, and no live Trello criteria/status changes are claimed. See the [handoff](handoffs/2026-10-20-0007-codex-field-data-human-playthrough-and-aps-accounting-fix.md).
- **October 19 evidence refresh:** T14 now has eight current-rule action fixtures passing direct Unity/headless parity (1/1 dedicated test, zero skips). T15 has 600/600 validated current-rule journeys: Remove Hare 58/200 survive, Add Hare 1/200, save for ×2 52/200. Current evidence and limits are in the [report](Research/Experiments/FD-Current-Loop-Refresh-20261019/REPORT.md). Earlier dated execution entries remain history. No card status, tuning, commit or push changed; T13/T16 human work remains open.
- **October 18 node-navigation revision:** Bevin removed population controls from all node-end settlements. Simulation settlement now returns to the map before explicit shop entry; leaving a shop returns to the map before explicit simulation entry. Final success also offers the completed map. Earlier checked pre-shop round-6 purchase criteria below are superseded by this decision; ordinary rounds 1–5 retain their controls. See the current PDD/SDD and node-navigation handoff.
- **Status, October 17:** 14 Done (T01–T12, T14, T15); T13 In Progress 4/5 awaiting independent human visual acceptance; T16 In Progress 0/5 awaiting team self-play and accept/revise. All 16 remain selected for Active S5, October 19–November 15, 2026. Implementation is local and unpushed.
- **October 17 correction:** route selection now previews the five-node path before a separate Start action; active Field Data screens can reopen that route map. Income modifiers are shop-only and Hare free rewards exclude generic attack upgrades. October 18 verification adds visible route cards and panel Map buttons: full suites passed 399 EditMode / 49 PlayMode with dedicated skips, followed by final graphics passes of 6/6 at 1280x720 and 1/1 at 1920x1080. See the [validation handoff](handoffs/2026-10-18-fd-route-and-reward-validation.md). Human acceptance/self-play remain open; earlier T08/T15 panel evidence remains historical.
- **October16 incoming planning checkpoint (historical):** published to Trello; all 16 retained for Active S5 priority review, October 19-November 15, 2026. At the later October 16 board check FD-T01–03 were Done, FD-T04 In Progress and FD-T05–16 in Current Work; final committed subset awaits team review.
- **Planning baseline:** merged `BevBranch` at `dcd6bb0`
- **Owner / reviewer:** unassigned on every ticket, as Bevin requested
- **Estimates:** proposed active team-hours, including focused tests and ordinary review fixes; selected planning ranges; capacity risk recorded in the S5 control

## Outcome and sources

Deliver the five-node Field Data journey from the existing Desktop, preserving
one ecosystem and one wallet through three six-round simulations and two shops.
The player can earn, intervene, buy journey upgrades and reorder income, then
see correct success or failure results.

Use the [reviewed gameplay baseline](FIELD_DATA_JOURNEY_DESIGN.md) for rules,
the [PDD](FIELD_DATA_PROCESS_DESIGN.md) for process/delivery stages, the
[SDD](FIELD_DATA_SOLUTION_DESIGN.md) for proposed integration and the
[combined planning draft](FIELD_DATA_AND_SIMULATION_ALIGNMENT.md) for Salty's
related work and scope boundaries. The proposed type names are implementation
guidance, not a requirement to create one class for every ticket.

`FD-T01` through `FD-T16` are stable task references on the S5 Trello cards.
Every ticket starts Not started with no owner/reviewer, as requested. Its
acceptance boxes below describe future evidence; none is checked by writing
this document. A receipt means a recorded award or purchase with its identity,
amount and resulting balance.

## Ticket index and dependencies

| Draft ID | Actionable outcome | PDD stage | Depends on | Proposed hours |
| --- | --- | --- | --- | ---: |
| FD-T01 | Agree the test-route entry and first implementation boundaries | FD-I01 | Team review | 1–2 |
| FD-T02 | Freeze and validate the starter journey definition | FD-I01 | FD-T01 | 3–5 |
| FD-T03 | Calculate ordered income with a complete breakdown | FD-I04 | FD-T02 | 3–5 |
| FD-T04 | Run all 18 rounds in one retained journey session | FD-I02 | FD-T02 | 4–7 |
| FD-T05 | Settle awards once and enforce the journey wallet lifecycle | FD-I02 | FD-T03, FD-T04 | 4–6 |
| FD-T06 | Add the six atomic population controls | FD-I03 | FD-T05 | 4–7 |
| FD-T07 | Apply the six signed Hare shop-effect families | FD-I04 | FD-T02, FD-T04 | 5–8 |
| FD-T08 | Offer and resolve free round/node rewards | FD-I04 | FD-T05 | 3–5 |
| FD-T09 | Generate and purchase deterministic shop offers | FD-I04 | FD-T05, FD-T07 | 4–7 |
| FD-T10 | Buy random or chosen income reorders | FD-I04 | FD-T09 | 3–5 |
| FD-T11 | Show round income, rewards and population actions in Noesis | FD-I05 | FD-T06, FD-T08 | 4–6 |
| FD-T12 | Show shops and reorder controls in Noesis | FD-I05 | FD-T10, FD-T11 | 4–7 |
| FD-T13 | Author the test route and verify the complete Desktop flow | FD-I05 | FD-T01, FD-T02, FD-T11, FD-T12 | 3–5 |
| FD-T14 | Run the same journey domain headlessly and export replay evidence | FD-I06 | FD-T13 | 6–10 |
| FD-T15 | Freeze and run the matched 200-seed comparison | FD-I07 | FD-T14 | 3–5 |
| FD-T16 | Review internal self-play and record accept/revise | FD-I07 | FD-T15 | 3–5 |

The selected tickets total **57–95 active team-hours**, before unknown machine
time or a new tuning iteration. Bevin selected all 16 for a four-week S5 after
the over-capacity warning. The [S5 control](Sprints/S5-control-record.md)
records the 80–120h combined availability. Josh removed the protected review
reserve on October 16 and will use separate dedicated review time. The 80h
conservative planning floor leaves a 15h upper-estimate overage before new
ecology/playability work. Re-estimate remaining work with Sim before final
S5 prioritization and at each implementation review.

The original **FD-T01–FD-T05** recommendation remains a useful 15–25h foundation
sequence within the full selected scope.
It produces a domain fixture that can exercise the continuous route and its
awards. It does not deliver the complete player-facing shop journey. FD-T03 and
FD-T04 can proceed independently after FD-T02; FD-T07 can follow FD-T04 while
other domain work proceeds. Agree shared-file ownership before parallel edits.

## Published Trello index - 2026-10-16

[Active Sprint 5 control](https://trello.com/c/hqQczLQ9/135-sprint-5-control-record-field-data-journey-implementation) |
[Repository control and capacity](Sprints/S5-control-record.md)

| Card | Verified workflow list, October 17 | Execution state |
| --- | --- | --- |
| [FD-T01](https://trello.com/c/kl9F3o2l/119-fd-t01-agree-the-test-route-entry-and-first-implementation-boundaries) | Done | Done |
| [FD-T02](https://trello.com/c/3ZTf7hmC/120-fd-t02-freeze-and-validate-the-starter-journey-definition) | Done | Done |
| [FD-T03](https://trello.com/c/kdncn6mH/121-fd-t03-calculate-ordered-income-with-a-complete-breakdown) | Done | Done |
| [FD-T04](https://trello.com/c/LKzm9j7r/122-fd-t04-run-all-18-rounds-in-one-retained-journey-session) | Done | Done (verified October 17) |
| [FD-T05](https://trello.com/c/3cGXubBh/123-fd-t05-settle-awards-once-and-enforce-the-journey-wallet-lifecycle) | Done | Done (verified October 17) |
| [FD-T06](https://trello.com/c/CMld0U0x/124-fd-t06-add-the-six-atomic-population-controls) | Done | Done (verified October 17) |
| [FD-T07](https://trello.com/c/LFKRoQpN/125-fd-t07-apply-the-six-signed-hare-shop-effect-families) | Done | Done (verified October 17) |
| [FD-T08](https://trello.com/c/XB4jwxit/126-fd-t08-offer-and-resolve-free-round-node-rewards) | Done | Done (verified October 17) |
| [FD-T09](https://trello.com/c/Hrl7GNAo/127-fd-t09-generate-and-purchase-deterministic-shop-offers) | Done | Done (verified October 17) |
| [FD-T10](https://trello.com/c/C9D3evir/128-fd-t10-buy-random-or-chosen-income-reorders) | Done | Done (verified October 17) |
| [FD-T11](https://trello.com/c/GZFJ6QPo/129-fd-t11-show-round-income-rewards-and-population-actions-in-noesis) | Done | 5/5 verified |
| [FD-T12](https://trello.com/c/J2EqiGv3/130-fd-t12-show-shops-and-reorder-controls-in-noesis) | Done | 5/5 verified |
| [FD-T13](https://trello.com/c/FGFDVXcQ/131-fd-t13-author-the-test-route-and-verify-the-complete-desktop-flow) | In Progress | Human visual acceptance remains |
| [FD-T14](https://trello.com/c/XB3Yc2j7/132-fd-t14-run-the-same-journey-domain-headlessly-and-export-replay-evidence) | Done | Five-fixture parity verified |
| [FD-T15](https://trello.com/c/nnujHAoK/133-fd-t15-freeze-and-run-the-matched-200-seed-comparison) | Done | 600/600 current-source replays |
| [FD-T16](https://trello.com/c/osmzcZVf/134-fd-t16-review-internal-self-play-and-record-accept-revise) | In Progress | Team self-play and decision remain |

### Kickoff checkpoint - 2026-10-16

FD-T01 is Done (4/4 criteria, under Bevin's explicit review waiver); FD-T02 is
Done (5/5); FD-T03 is Done (5/5); FD-T04 and FD-T05 are In Progress with all
their acceptance items open. The last verified board state plus the T05 move
implies 16 Done, 2 In Progress, 11 Current Work, and 65 open acceptance items. FD-T02 passed
7/7 focused Editor tests. FD-T03 passed 8/8 calculation tests and its
dependent T02 definition tests passed 7/7. T04 runtime and Editor test
assemblies compile with Unity C#. Its explicit preview composition is now
wired: a matching resolved definition prepares the retained 18-round manager
session, the preview remains the sole clock forwarder, and legacy reward
callbacks are bypassed in that mode. The focused `FieldData` EditMode suite
passed 24/24, including 16 journey definition/session/ledger tests. Tests
cover failure at the first 100-tick boundary without a payout. A test
fixture now holds Hare alive so route lifecycle coverage does not imply a
balance result; disposal also transitions a failed session to Closed. These
checks do not validate rendered preview behavior or complete ticket acceptance.
Both T04 and T05 remain In Progress with their acceptance items open. The last
verified board snapshot plus the verified T05 move implies 16 Done, 2 In
Progress, 11 Current Work, and 65 open acceptance items. See the
[T02 checkpoint](FIELD_DATA_SOLUTION_DESIGN.md#15-fd-t02-implementation-checkpoint---2026-10-16)
and [T03 checkpoint](FIELD_DATA_SOLUTION_DESIGN.md#16-fd-t03-ordered-income-checkpoint---2026-10-16),
plus the [T04 implementation checkpoint](FIELD_DATA_SOLUTION_DESIGN.md#17-fd-t04-retained-journey-session---2026-10-16).

### Execution checkpoint - 2026-10-16, continued

The Unity Authoring category passed 20/20, but that category did not include
the new Field Data portable-definition test methods; run those by exact test
name after Editor compilation completes. `CellSim.Batch` builds with zero
warnings/errors, and its self-test now completes two deterministic
18-round Field Data replays with 18 population awards, three node-clear awards,
a successful Hare intervention reflected at the next board boundary, a
successful-results discard, first-affordability records, supplied human time,
and preserved not-executed actions. These fixtures do not stand in for the
authored Forest Edge route or Unity parity.

Added T05 journey-session extinction cases at route ticks 100, 600 and 1800.
The previous passing session suite covered only tick 100; later boundary
extinction was ledger-only. The new integration cases remain unrun while the
Editor is unreachable.

The Forest Edge UI route reached the second simulation in the integration
smoke but failed at tick 1,495 under the original setup. Bevin directed use of
another existing scenario or seed for success-flow verification; that run has
not yet been produced. FD-T04/T05 and FD-T06–T13 retain their open acceptance
items; FD-T14 has working headless infrastructure but still needs the authored
snapshot, Unity/headless parity, and its exact DTO tests. FD-T15's explicit
10100–10299 seed list, Forest Edge/definition asset hashes and three policy arms are frozen in
`artifacts/field-data-t15/seed-manifest.json`; no study runs have started. FD-T16 requires actual internal
self-play findings and a team accept/revise decision. No Trello statuses or
comments changed.

### Execution checkpoint - 2026-10-17

After Unity recovered, the three new natural-extinction boundary cases passed
at ticks 100, 600 and 1,800 (3/3); the complete
`FieldDataJourneySessionTests` fixture passed 12/12; and all FieldData-named
EditMode tests passed 46/46. The two exact portable-definition contract tests
passed 1/1 each. The route-entry and first-shop PlayMode integration test passed
1/1 at `artifacts/unity-tests-20261017-000653`.

That first PlayMode attempt exposed `CS0165` in
`CellSimPortable.ExportPortableFromCommandLine`: `definitionError` was only
assigned when a non-null asset reached `TryResolve`. The exporter now checks a
missing asset and an invalid definition separately; Unity recompilation
completed with `compilationFailed: false`, then the focused PlayMode test passed.
The test exercises Desktop map selection and the first simulation/shop boundary;
it does not clear all 18 rounds or prove the success path.

Authored Forest Edge seed search found a full-route success at seed 10125.
`FieldDataTestRouteCompletesThroughBothShopsOnFixedSeed` passed 1/1 at
`artifacts/unity-tests-20261017-001248/PlayMode-results.json`, advancing that
same seed through all 1,800 ticks and both shops in the actual Desktop UI, then
verifying success and leftover-FD discard. This verifies the success route only.
The additional failure/restart coverage and remaining T13 gaps are recorded
below; T13 stays In Progress until its remaining items are completed or
explicitly identified as outstanding.

`FieldDataNaturalFailureForfeitsJourneyWalletAndRestartStartsAtZero` passed
1/1 at `artifacts/unity-tests-20261017-001949/PlayMode-results.json`, then the
complete FieldData PlayMode subset passed 3/3 at
`artifacts/unity-tests-20261017-002057/PlayMode-results.json`. The failure
test drove the live UI through a natural Hare extinction, verified the round's
unsettled payout was skipped and journey FD was forfeited, then used the results
action to start a new run/session with zero FD and unchanged progression
currency. The success-flow test likewise verified successful remainder discard
and restart to a fresh run/session. Explicit manual End, Desktop profile/unlock
identity, shop re-entry/repeat actions, visual captures and human review remain
open.

The complete FieldData-named PlayMode subset passed 3/3 at
`artifacts/unity-tests-20261017-002057/PlayMode-results.json`, covering Desktop
route selection/first-shop entry, full success/restart, and natural failure with
wallet forfeiture and restart.
The FieldData-named EditMode suite was rerun after removing the temporary seed
probe and passed 46/46 at `artifacts/unity-tests-20261017-002151/EditMode-results.json`.

The `CellSim.Batch` Release build and self-test were rerun on October 17 after
the route integration work. Build completed with 0 warnings/errors; the
self-test passed the 18-round replay, exactly-once income/node awards,
successful-results discard, deterministic boundaries, action statuses,
first-affordability and timing provenance cases.

FD-T04/T05 and FD-T06–T13 still need their acceptance evidence reconciled before
closure. The authored Forest Edge snapshot cannot be exported while the Editor
is open; live Unity/headless parity remains open. The full-route success check
now passes on seed 10125. The frozen 200-seed comparison has not run, and FD-T16 still requires
actual team self-play and an accept/revise decision. No Trello status or
checklist was changed in this checkpoint.

### Current execution checkpoint - 2026-10-17, continued

The authored-settings Desktop route now uses Forest Edge's 0.2-second step
interval and fixed seed 10100. The clean PlayMode suite passed 4/4 at
`artifacts/unity-tests-20261017-004043/PlayMode-results.xml`, covering the
18-round/two-shop success path, natural failure, manual abandonment through
End confirmation, and route entry. Success, natural failure and abandonment
preserve seeded Desktop profile currency and feature unlocks through results
and a fresh journey. The earlier 10125 PlayMode success inherited a 0.1-second
editor setting and is retained as historical evidence only.

T13 remains In Progress because repeated results/Continue and shop re-entry
checks, fresh graphics captures and independent human review remain open.

T14's authored snapshot is
`artifacts/field-data-t14/forest-edge-field-data-20261017.json`; its Unity
reference contains 200 rows for seeds 10100–10299. A no-purchase headless run
on seed 10100 completed all 18 rounds, recorded 18 population awards and three
node awards, and discarded 52 FD at successful results. Its final populations
(Hare 1, Fox 0, Plant 717) match the Unity reference. T14 remains In Progress
until Unity and headless runs compare action/offer identities, receipts,
purchase and reorder outcomes, and report compatibility on a common fixture.
The snapshot records HEAD `acc371ac`; because the working tree was dirty, that
SHA is a base anchor rather than a clean build revision. The Oct 17 handoff
records the snapshot, reference and DLL hashes.

T15 ran all 600 matched replays (200 seeds in each of the three frozen arms);
all reports completed, each seed appears in every arm, and scenario/definition
fingerprints match. Baseline survival was 45/200; the Remove Hare arm survived
50/200, Add Hare 0/200, and save-for-second-shop-×2 45/200. These are
descriptive outputs, not a pass threshold or causal tuning conclusion.
Individual reports and aggregates are in
`artifacts/field-data-t15/run-20261017/cohort-200x3/`. Automated runtime totals
872,424.8 ms, averaging 1,454 ms per replay; active team setup/analysis time was
not captured, so that T15 criterion remains open.

T16 is not complete. It needs internal team self-play, actual decision-time
notes, observed opportunity-cost examples, and a team accept/revise decision.
Automated replays do not replace these human findings, and the next content
addition remains deferred until the team has reviewed them.

### Verified T04–T06 closeout - 2026-10-17

T04, T05 and T06 are now Done on Trello, with all five criteria checked on each
card, corrected status text and a read-back completion comment. Current clean
Unity evidence is `artifacts/unity-tests-20261017-010128/EditMode-results.xml`
(49/49 FieldData tests) and
`artifacts/unity-tests-20261017-010227/PlayMode-results.xml` (5/5 route,
legacy-entry and terminal-lifecycle tests). Repeated Continue/results and shop
re-entry checks pass. The profile tests now seed and preserve species/Genome
unlock IDs as well as banked currency and feature IDs.

T06 review corrected Plant additions overwriting existing resources and
population capacity counting a shared creature/resource cell only once.
The latter was reproduced by a failing test before correction; all nine
population-control tests now pass, including placement-sequence preservation
after rejection and unchanged natural ecology counters. T07 is next. T13
still requires rendered evidence and independent human review; T14–T16 remain
open. See the [closeout handoff](handoffs/2026-10-17-0106-codex-fd-t04-t06-verified-closeout.md).

## Ticket descriptions

### FD-T01 — Agree the test-route entry and first implementation boundaries

**October 16 acceptance:** Complete; [entry/compatibility record](FIELD_DATA_ENTRY_REVIEW.md).
Bevin accepted larger-map entry then test-route selection and waived any further
review, including the original additional Salty review. This is a user-directed
exception to the first criterion, not a claim that Salty reviewed it.

**Outcome:** A developer knows how the five-node Field Data test is launched
from the canonical Desktop and which existing behaviors to preserve.

Record the selected map/definition entry and review the SDD's explicit opt-in
composition. Identify the current ecology revision used during development.
Keep the 29-marker content map and standalone six-round route usable. Resolve
how the first-test journey is named in planning without rewriting the overall
roadmap contract by implication.

- [x] Record the launch path and map/definition selection with Bevin and Salty's review.
- [x] Confirm the five-node/18-round acceptance route and the existing map/standalone compatibility boundary.
- [x] Record Field Data as this ticket set's scope; permanent rewards, Genome effects and ecology corrections remain separate unless explicitly selected.
- [x] Identify any technical proposal needing revision and update the PDD/SDD accordingly; no code is needed to close this decision ticket.

**Trace:** FD-I01; PDD P-01/P-02; SDD §§3–5/11; checklist 4.06.

### FD-T02 — Freeze and validate the starter journey definition

**October 16 completion:** The definition asset, immutable snapshot and
validator are complete; all 7 focused Editor tests passed. Runtime launch
selection and test-map integration remain FD-T13.

**Outcome:** One validated, immutable definition contract supplies route,
catalog, price, quantity, reward-cadence and eligibility data for a run.

Implement the definition/authoring boundary and stable identities described in
the SDD. Resolve values once at launch; keep runtime balance, offers and owned
upgrades out of shared assets. Author the current six interventions and three
income entries, with the explicit data shape for signed Hare offers. FD-T07
finishes the twelve signed entries after their runtime mappings are available;
an unmapped entry cannot enter a run in the meantime.

- [x] Capture the exact starter rules, 100-tick rounds and six-round nodes, including shop eligibility from visit 1 or 2.
- [x] Reject invalid routes, costs, duplicate IDs and unsupported effects/targets before a journey starts.
- [x] Produce versioned/fingerprinted resolved inputs; later asset edits cannot alter an active run.
- [x] Preserve serialized identities and existing enum values; append Shop only where the selected integration requires it.
- [x] Add focused authoring/definition tests and preserve legacy definitions when the new mode is not selected.

**Trace:** FD-I01; SDD §§4/11; baseline Steps 1–2 and checklist 4.09–4.11.

### FD-T03 — Calculate ordered income with a complete breakdown

**Outcome:** Given closing populations and an ordered income list, calculate
the exact award and numerical steps used to explain it.

Implement +1 FD per Hare, ×1.5 and ×2 as population-payout operations. Preserve
instance order and compound multipliers; floor only the final amount. Keep the
10-FD node bonus outside this calculation. Do not award currency in the UI.

- [x] With 20 Hares, +1/Hare then ×2 yields 80 FD; reversing those operations yields 60 FD.
- [x] Verify one final floor with a fractional fixture, rather than rounding every step or carrying a remainder.
- [x] Return the base count, ordered operations, intermediate totals and final award without changing a wallet.
- [x] Reject out-of-range/invalid arithmetic without wrapping a balance or adding an unapproved gameplay cap.
- [x] Add focused calculation tests; taking a modifier never recalculates an already-settled payout.

**Trace:** FD-I04; PDD P-03; SDD §6; checklist 4.09, 4.13, 4.25–4.26.

### FD-T04 — Run all 18 rounds in one retained journey session

**October 17 completion:** Done, 5/5 criteria verified in the closeout above.

**Outcome:** One session coordinates the complete route without recreating the
ecosystem, wallet or progression at shops.

Add the feature-local session and its allowed flow states around the existing
SimulationManager/runner and SpeciesProgression. Keep the current preview as
the sole Unity clock forwarder. Make boundaries and terminal completion reach
the session through explicit composition, bypassing competing legacy flow in
the selected mode.

- [x] Traverse rounds 1–18 at ticks 100–1800; pause for both shops at 600 and 1200.
- [x] Preserve the runner, board, identities, resource/creature state, history, Genome snapshot and retained progression through both shops.
- [x] Tick 1800 reaches the same final-round handling even if the runner reports Complete instead of a decision boundary.
- [x] Decisions/shops do not advance time; illegal or stale transitions cannot resume or create another journey.
- [x] Verify subscription cleanup/rebinding and legacy route compatibility with focused lifecycle fixtures.

**Trace:** FD-I02; PDD P-01/P-02; SDD §§3–5; checklist 4.20–4.21.

### FD-T05 — Settle awards once and enforce the journey wallet lifecycle

**October 17 completion:** Done, 5/5 criteria verified in the closeout above.

**Outcome:** A successful round pays once; failure, success exit and a fresh
journey handle Field Data correctly without changing permanent progression.

Use one session-owned settlement path for boundary and completion callbacks.
Retain separate population and node-clear receipts. Reuse the merged reset
behavior where compatible and bypass the old preview's competing award paths.

- [x] New journeys start at 0 FD; population income settles once and each of the three successful node clears adds a separate 10 FD.
- [x] Reopening results/repeating Continue does not repay; a retry after population settlement can finish a missing node bonus without resetting choices.
- [x] Natural extinction at ticks 100, 600 or 1800 fails before that round's payout/choice/bonus; incomplete-route End/abandon forfeits the remaining balance.
- [x] Success shows leftover FD and discards it on results exit; no permanent cash-out or conversion is added.
- [x] Test that banked currency, species/Genome unlocks and feature IDs are unchanged; legitimate existing profile time accounting remains allowed.

**Trace:** FD-I02; PDD P-03/P-06/P-08; SDD §§5–6/11; checklist 4.07, 4.18, 4.22–4.24.

### FD-T06 — Add the six atomic population controls

**October 17 completion:** Done, 5/5 criteria verified in the closeout above.

**Outcome:** Between rounds, a paid add/remove request either changes the full
requested population and wallet together, or changes neither.

Add/remove 1 Hare, add/remove 1 Fox and add/remove 5 Plants, each for 10 FD.
Use deterministic random valid positions/targets. Respect separate creature
and plant-resource cell contents and update the boundary/previous grid safely.

- [x] All six operations support repeat purchases in legal windows, including nonterminal round-6 results before shops.
- [x] Reject insufficient FD, too few removal targets, insufficient full-batch space and removal of the last selected-species individual without partial changes or charges.
- [x] Plant removal preserves a creature sharing that cell; creature removal preserves its underlying resource/terrain.
- [x] Rejected actions leave IDs/random ordinals unchanged; repeated deliberate clicks are distinct purchases while retries of one request cannot charge twice.
- [x] Preserve the closing population used for settled income; record interventions separately from natural births/deaths/kills.

**Trace:** FD-I03; PDD P-05; SDD §7; checklist 4.08, 4.19; baseline purchase rules.

### FD-T07 — Apply the six signed Hare shop-effect families

**October 17 completion:** Done, 5/5 on Trello. The full clean EditMode suite
passed 390/390 at `artifacts/unity-tests-20261017-010934/EditMode-results.xml`.
All twelve variants change only their registered target; both orders of
opposing copies cancel before bounds. The session test verifies independent
ordinary/shop caps, unchanged complete cell state across activation, retained
effects at the later node and an empty shop-effect state in a fresh journey.
The [completion comment](https://trello.com/c/LFKRoQpN/125-fd-t07-apply-the-six-signed-hare-shop-effect-families#comment-6ad303a1ab8fcca7ca4f47ac)
was posted and visibly read back before moving the card to Done.

**Outcome:** Positive and negative shop species effects persist through the
journey, cancel correctly and coexist with ordinary upgrades.

Implement net signed overlays for Long Stride, Sharper Instinct, Reinforced
Hide, Warren Comfort, Deep Digestion and Abundant Seedfall using the baseline's
exact amounts. Resolve against ordinary progression before final bounds. Add
the explicit signed avoidance registry/options mapping required by the SDD.

- [x] All twelve variants affect the intended attribute; no display-name/reflection mapping or negative value through the old positive-only constructor.
- [x] Opposing copies cancel against the base even at bounds; current energy, cooldowns and creature state are not reset/refilled.
- [x] A shared three-copy family cap spans signs and future target metadata, separately from ordinary upgrade levels; cancellation does not refund a copy.
- [x] Shop effects begin in the next simulation, remain active for later nodes and reset at journey end.
- [x] Registry, runtime applier, fingerprint and telemetry mapping agree in focused tests before assets enter a run, preserving [UPG-C01–C04/C06](Planning%20Concerns/upgrade-system.md).

**Trace:** FD-I04; PDD P-07; SDD §8; checklist 4.11, 4.21.

### FD-T08 — Offer and resolve free round/node rewards

**Historical October 17 completion (income sources superseded):** Done, 5/5 on Trello at that checkpoint. The clean session fixture passed
13/13 at `artifacts/unity-tests-20261017-011229/EditMode-results.xml`, including
all 17 available free income acquisitions in one 18-round route, source gates,
no income copy cap, exactly-once/re-entry behavior, ordinary eligibility caps,
fewer eligible options and no terminal-success reward. The
[completion comment](https://trello.com/c/XB4jwxit/126-fd-t08-offer-and-resolve-free-round-node-rewards#comment-6ad30411b204551ae1b3be1d)
and Done-list state were visibly read back.

**Outcome:** Each eligible break presents a stable pick-one free choice with
Skip, using the approved source pool and activation timing.

After rounds 1–5 of each simulation and after Simulation 1 and 2 clear, offer
ordinary species upgrades only. All income modifiers are shop-only: +1 FD/Hare
and ×1.5 from Shop 1, ×2 from Shop 2. The old source-eligibility test results
below describe the prior rule and must not be used as evidence for this update.

- [x] Draw three distinct eligible options or fewer if needed, deterministically; income remains eligible rather than guaranteed.
- [x] Choosing one resolves the source once; Skip grants no award or penalty. Reopening does not generate a fresh choice.
- [x] Free species rewards cost no FD. Shop-acquired income appends to current execution order and first affects the next unsettled payout. (The old completion evidence predates the October 17 shop-only correction.)
- [x] Income has no copy cap; ordinary upgrades retain their existing cap and separate state.
- [x] No free reward is offered after terminal success; test round/node timing and replay-safe resolution.

**Trace:** FD-I04; PDD P-04/P-06; SDD §§4/9; checklist 4.10, 4.26.

### FD-T09 — Generate and purchase deterministic shop offers

**October 17 completion:** Done, 5/5 on Trello. The clean session fixture passed
16/16 at `artifacts/unity-tests-20261017-011558/EditMode-results.xml`, including
multiple distinct purchases, stacking a naturally repeated item across visits,
one free reroll per capped offer, purchased/sign/duplicate exclusions, exhausted
replacement pools, funds/stale-intent rejection and independent seeded draws.
The [completion comment](https://trello.com/c/Hrl7GNAo/127-fd-t09-generate-and-purchase-deterministic-shop-offers#comment-6ad304c70cd7305455f8df95)
and Done-list state were visibly read back.

**Outcome:** Each shop retains its seeded offers, handles purchases once per
offer, and lets the player optionally reroll capped species offers.

Draw five distinct equal-weight legal offers from the starter pool: signed
Hare effects and income, with ×2 eligible only at shop 2. Price species offers
at 30 FD and income at 20/30/50 FD. Keep population controls outside shops.

- [x] A visit never displays both signs of the same family; re-entering preserves offers, purchases and exclusions.
- [x] Allow multiple distinct purchases per visit; later distinct offers can stack within their applicable cap.
- [x] Capped species offers may appear locked; buying is disabled and one optional free reroll is available per capped offer/visit.
- [x] Replacements exclude capped, duplicate, conflicting or ineligible items, including conflicts with already-purchased same-visit offers; use fewer offers if necessary.
- [x] Test funds/stale-intent rejection and receipt-based exactly-once spending; preserve independent deterministic offer/action streams.

**Trace:** FD-I04; PDD P-07; SDD §9; checklist 2.13a–2.13h, 4.10–4.11.

### FD-T10 — Buy random or chosen income reorders

**October 17 completion:** Done, 5/5 on Trello. The full clean EditMode suite
passed 395/395 at `artifacts/unity-tests-20261017-011725/EditMode-results.xml`.
Coverage includes identical-effect instances, changed random order, full chosen
permutations, stale drafts after acquisition, exact escalating/reset prices,
random/chosen receipt retries, cancellation and funds rejection without side
effects, separate histories and frozen payouts. The
[completion comment](https://trello.com/c/C9D3evir/128-fd-t10-buy-random-or-chosen-income-reorders#comment-6ad3052be60eebf3edf2e15d)
and Done-list state were visibly read back. T11's subsequent process review
adds the PDD-required node-choice gate before any shop action; its regression
and adapted second-shop fixtures are being verified separately.

**Outcome:** Shop actions change future income execution order while retaining
owned instances and the original acquisition history.

Implement immediate random shuffle without preview and chosen full-permutation
preview/confirm/cancel. After either successful paid reorder, increment the
visit's price step: random is ceil(10 × 1.1^k), chosen is exactly 10 times it.

- [x] Random requires at least two instances and changes their order; same-effect copies remain distinct instances.
- [x] Chosen preview leaves committed state untouched; confirmation validates a complete current permutation and funds; cancel costs nothing.
- [x] Verify random/chosen charges 10/100, 11/110, 13/130 and 14/140, with reset at the next shop.
- [x] Rejections/cancellation do not advance prices; retrying one accepted intent does not charge twice.
- [x] Keep acquisition and effective/reorder histories separately; later acquisitions append to current order and no prior payout changes.

**Trace:** FD-I04; PDD P-07; SDD §§6/9; checklist R.01–R.09, 4.27–4.28.

### FD-T11 — Show round income, rewards and population actions in Noesis

**October 17 completion:** Done, 5/5. Full runtime and dedicated rendered checks
verify the round actions, choice gate, readable settlement and terminal wallet.
The [completion comment](https://trello.com/c/GZFJ6QPo/129-fd-t11-show-round-income-rewards-and-population-actions-in-noesis#comment-6ad30fb374b66cefdb9b4301)
and Done-list state were read back. See the [current handoff](handoffs/2026-10-17-0202-codex-field-data-rendering-parity-and-cohort.md).

**Outcome:** The player can understand the payout and use the round-break
actions from the canonical simulation screen.

Compose the journey helper explicitly and project read-only facts into the
existing shell ViewModel. Show all species counts, selected count, ordered
income steps, separate node bonus and current balance. Bind reward/Skip,
population controls, Continue and terminal results to authoritative commands.

- [x] Display cost/effect before clicking; a population purchase applies on one click and balance updates after every transaction.
- [x] Population controls appear only in allowed windows, including pre-shop round-6 results, and disappear after terminal success.
- [x] The free choice resolves before population buying; selected income effects are clear for the next payout.
- [x] Failure shows FD lost; success shows leftover FD until exit. A stale/disabled command is rejected in the domain too.
- [x] Add focused Noesis integration and rendered checks; preserve board rendering/zoom/pan and one clock forwarder. No economy calculations move into XAML/ViewModel.

**Trace:** FD-I05; PDD P-03–P-06/P-08; SDD §10; checklist 4.12–4.17.

### FD-T12 — Show shops and reorder controls in Noesis

**October 17 completion:** Done, 5/5. Both shops and the separate capped-ownership
fixture pass runtime/rendered checks, including paid random/chosen actions and
rejections. The [completion comment](https://trello.com/c/J2EqiGv3/130-fd-t12-show-shops-and-reorder-controls-in-noesis#comment-6ad30fcfdf44f34843f53283)
and Done-list state were read back. See the [current handoff](handoffs/2026-10-17-0202-codex-field-data-rendering-parity-and-cohort.md).

**Outcome:** The player can buy eligible offers, reroll capped items and use
both reorder modes without losing their place or accidentally spending twice.

Add the shop presentation and an uncommitted chosen-reorder draft to the
existing shell. Show prices, effects, ownership/cap state and effective income
order. Let the session decide which commands are currently allowed.

- [x] Purchase/reroll state and the visible wallet update immediately; population controls are not shop cards.
- [x] Random reorder has no preview or confirmation; chosen shows full order, preview/cost and confirm/cancel.
- [x] Cancellation/reopening preserves the committed sequence, wallet, offers and current shop price step.
- [x] Repeated binding/open/close cannot add duplicate subscriptions, purchases or clock advancement.
- [x] Focused runtime/visual checks cover both shops, affordability, capped offers, reroll exhaustion and reorder confirmation/rejection.

**Trace:** FD-I05; PDD P-07; SDD §10; checklist 4.13, 4.15, 4.27.

### FD-T13 — Author the test route and verify the complete Desktop flow

**October 17 checkpoint:** In Progress, 4/5. The actual authored Desktop route,
lifecycle regressions and graphics pass; independent human visual accept/revise
remains unperformed. The [status comment](https://trello.com/c/FGFDVXcQ/131-fd-t13-author-the-test-route-and-verify-the-complete-desktop-flow#comment-6ad310245bd0c5f853d52e48)
and open human criterion were read back. Use the [self-play record](FIELD_DATA_SELF_PLAY_REVIEW.md).

**Outcome:** The agreed entry launches the real Simulation → Shop →
Simulation → Shop → Simulation route from the Desktop.

Wire the reviewed definition/map and explicit helper composition into the
player entry. Preserve the accepted scrolling map/panorama and old route when
not selected. Exercise a complete journey plus failure and a fresh-journey reset.

- [x] Play all 18 rounds and both shops through the actual UI with the same board/progression; no developer fixture substitutes for the final acceptance route.
- [x] Verify ordinary round, nonterminal round-6 and final-round order, repeated results/Continue, shop re-entry and lifecycle cleanup.
- [x] Check natural failure, manual abandonment, success exit and a new journey against protected profile currency/unlock/feature state.
- [x] Run the relevant integrated suites and graphics checks; retain captures and record independent human accept/revise without counting automated tests as human approval. Prior dated checks/captures are retained; Bevin explicitly accepts the initial slice and confirms APS fixed on October21.
- [x] Update the PDD/SDD, checklist and handoff from actual delivery/evidence; keep unperformed checks open.

**Trace:** FD-I05; PDD P-01–P-08; SDD §§10–11/13; checklist 4.06, 4.18–4.28.

### FD-T14 — Run the same journey domain headlessly and export replay evidence

**October 17 completion:** Done, 5/5. Five authored action fixtures match Unity
at every decision and board boundary (`artifacts/unity-tests-20261017-015621`);
repeat determinism, explicit missing-pool rejection and legacy ecology-report
compatibility also pass. The shared/exported ordinary reward pool corrects a
live/headless offer mismatch. The [completion comment](https://trello.com/c/XB3Yc2j7/132-fd-t14-run-the-same-journey-domain-headlessly-and-export-replay-evidence#comment-6ad31038fe6aafd780c2190e)
and Done-list state were read back. The [current handoff](handoffs/2026-10-17-0202-codex-field-data-rendering-parity-and-cohort.md)
supersedes the historical incomplete checkpoint below; T13 human review remains separate.

**October 16 implementation checkpoint:** The standalone runner now compiles
the shared Field Data definition, session, ledger, shop, intervention and
simulation manager sources. Its `journey-replay` command restores a versioned,
fingerprinted definition from a portable snapshot, executes scheduled actions,
records all-species populations, payouts, shops, receipts, first-affordability,
execution/acquisition histories and explicit succeeded/rejected/not-executed
statuses, and distinguishes automated runtime from optional human decision
milliseconds. A self-test completed the full 18-round route twice from the same
fixture: 18 population awards, three node awards, a successful-results discard,
matching deterministic boundaries/awards, and an incomplete off-boundary
action record. The Unity portable-definition round-trip/tamper tests have not
run; the 20/20 Authoring category (`artifacts/unity-tests-20261016-231614`)
did not include those new test methods. The
Release `CellSim.Batch` build and its complete self-test also passed.

This is not full T14 acceptance: a snapshot/reference bundle exported from the
authored Forest Edge asset and direct Unity-versus-headless parity for real
offers, purchases, reorder histories and board outcomes have not been
verified. The Editor/Pipeline recovery issue recorded at kickoff is resolved;
the focused Unity Editor and Desktop tests now pass. Authored snapshot export
still requires the Editor to be saved and closed. Keep T14 open until export
and parity checks succeed.

**Outcome:** The existing standalone tool can execute the same journey and
action schedule, producing evidence that matches bounded Unity fixtures.

Add the required shared sources to compilation and embedded provenance, then
extend current runner entry points with opt-in definition/actions. Export a
versioned Field Data section rather than copying economy logic into a new tool.

- [x] Replay stable offer/action/purchase/reorder identities with matching receipts, execution order and board outcomes in Unity and headless fixtures.
- [x] Export inputs/fingerprints, all-species payout counts, income breakdowns, offers, buy/skip/cost/balance, shop-arrival balance, terminal status and acquisition/effective histories.
- [x] Record first affordability by node/round/tick/balance and decision timing units; distinguish automated runtime from human deliberation time.
- [x] Preserve rejected/crashed/incomplete statuses, separate interventions from natural ecology counters and retain old report meanings.
- [x] Demonstrate deterministic replay and report-schema compatibility before claiming support for the 200-seed economy panel.

**Trace:** FD-I06; SDD §§9/12/13; checklist 4.28, 5.04–5.07.

### FD-T15 — Freeze and run the matched 200-seed comparison

**October 17 completion:** Done, 5/5. All 600 current-source replays and 200
paired sets validate against frozen inputs; active/automated timing and unknown
human effort are explicitly distinguished. The [completion comment](https://trello.com/c/nnujHAoK/133-fd-t15-freeze-and-run-the-matched-200-seed-comparison#comment-6ad31076355d619d119e2f2b)
and Done-list state were read back. See the [current handoff](handoffs/2026-10-17-0202-codex-field-data-rendering-parity-and-cohort.md).

**Outcome:** A reproducible comparison bundle supplies actual evidence for the
first Field Data review, with matched inputs across strategy arms.

Before execution, freeze 200 explicit seed IDs, scenario/Genome/ecology/build/
definition snapshots and declared action policies. Specify weaker/stronger
paths and the 10-FD intervention versus saving-for-50-FD ×2 comparison. Keep
natural offer availability rather than silently forcing a desired shop draw.

- [x] Run the same 200 seeds per arm from fresh matched inputs; preserve the continuous world inside each journey.
- [x] Record real offers, unavailable strategy opportunities, receipts and complete/stopped/crashed statuses without silently replacing missing runs.
- [x] Retain a manifest, objective per-run records and paired summaries using the approved affordability/spending/timing measures.
- [x] Keep ecology fixed across economy arms. Any chosen ecology revision is separately validated and frozen before the comparison.
- [x] Report active setup/analysis effort separately from measured machine time; no assumed throughput or survival acceptance threshold.

**Trace:** FD-I07; PDD §7; SDD §12; checklist 5.01–5.07, 5.12, 5.14.

### FD-T16 — Review internal self-play and record accept/revise

**October 17 checkpoint:** In Progress, 0/5. Evidence and the
[self-play record](FIELD_DATA_SELF_PLAY_REVIEW.md) are ready; human observations,
decision timing and accept/revise have not occurred. The [preparation comment](https://trello.com/c/osmzcZVf/134-fd-t16-review-internal-self-play-and-record-accept-revise#comment-6ad3109304378b509570784b)
was posted/read back. No balance or next-content decision is inferred from automation.

**Outcome:** The team can explain whether the loop works well enough to keep
or needs a bounded revision, based on repeatable evidence and direct self-play.

Play the matched weaker/stronger choices, review earning versus saving and
look for income purchases crowding out useful species/population actions.
Combine observations with FD-T15's records. This is internal testing, without
external interviews, teach-back requests or behavioral-research requirements.

- [ ] Record team observations, real decision time and simulation ticks; do not substitute automated runtime for player deliberation.
- [ ] Review intervention versus saving, payout/effect clarity and concrete opportunity-cost examples without inventing a numeric pass gate.
- [x] Record accept/revise and its evidence. Bevin's complete seed10100 custom-path playthrough felt good; keep the first loop with the specific APS accounting correction (recorded October20). No price/effect bundle was requested. This local record does not claim all other criteria or Trello were closed.
- [ ] A revision requires matched replays through FD-T15 on both weaker/stronger paths; do not claim isolated causality for a multi-value bundle.
- [ ] Choose the next content addition only after findings exist; permanent economy, Fox/Plant/multispecies shop effects and random events remain separate decisions.

**Trace:** FD-I07; PDD §7; baseline Step 5; checklist 5.02–5.03, 5.08–5.16.

## Shared completion and scheduling rules

Each implementation ticket includes its focused correctness tests and a concise
handoff. FD-T13 verifies integrated behavior; FD-T14 proves replay/report parity;
FD-T15/16 evaluate the economy. Passing one layer does not complete the others.
Use the [documented validation workflow](UNITY_PLUGIN_WORKFLOWS.md) and record
actual passed/failed/skipped results, revision, artifacts and remaining limits.

Reuse the merged profile storage, journey reset and scrolling map. Retain
P1-032's separate launch-input/real exit-relaunch acceptance; it is not a new
profile implementation hidden in this backlog. Do not add the deferred
crowding/movement/litter changes or permanent reward economy to these tickets.

All 16 tickets remain visible for S5 priority review with Sim. Allocate
owners/reviewers and confirm the final subset during that review; leaving them
unassigned at kickoff is Bevin's explicit exception. Reconcile completed
foundation cards, inspect FD-T04's remaining implementation, review technical
proposals against the current code, and retain the documented capacity risk.
Kickoff and card creation do not satisfy any implementation, runtime, balance
or self-play acceptance criterion.

## Requirement coverage

| Requirement group | Draft tickets |
| --- | --- |
| Scope and immutable inputs | FD-T01, FD-T02 |
| Continuous journey, wallet and protected permanent state | FD-T04, FD-T05, FD-T13 |
| Ordered income and source activation | FD-T03, FD-T08, FD-T09 |
| Six population controls and rejection | FD-T06, FD-T11 |
| Signed shop effects, caps and offers | FD-T07, FD-T09, FD-T12 |
| Reorder pricing, interaction and histories | FD-T10, FD-T12, FD-T14 |
| Payout/results display and full route | FD-T11, FD-T12, FD-T13 |
| Checklist 4.07–4.28 correctness/delivery | FD-T03–FD-T14, with item traces in each body |
| Checklist 5.01–5.16 comparison/review | FD-T14, FD-T15, FD-T16 |

## Historical draft review evidence — 2026-10-16, before publication

The draft was checked for unique IDs, valid dependencies without cycles,
estimate sums, links and coverage of the seven PDD stages. It preserves the
original numbered design checklist. No implementation, test suite, comparison
run, board edit, commit or push is claimed by this ticket-planning work.
