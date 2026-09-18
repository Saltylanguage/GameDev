# S3-04 — Mutation readability and bounded Forest Edge review

**Status:** Working plan — runtime mismatch audited; owner decisions remain open
**Owner:** Josh (product and player-facing acceptance)
**Evidence owner:** Sim
**Planned effort:** Josh 2h; Sim 6h
**Sprint:** S3, M1 closeout

## Goal

Make a Mutation easy to understand at the decision point and help the player
recognize its consequence in the following phase. Test one bounded
Forest Edge/Hare slice and record an accept-or-revise decision. This is a
readability and scoped evidence review, not approval of the full Mutation
catalog or a production-balance claim.

## Locked product boundaries

- The player gets three Mutation options or chooses Skip at the boundaries
  after rounds 1–5. A selected Mutation is temporary and applies to the current
  expedition; Mutation choices are not Genome purchases.
- The player-facing explanation stays bite-sized: an icon and keyword such as
  “hunting +1,” plus only the brief timing/tradeoff/consequence needed to make
  the choice understandable. Do not show a full statistic breakdown.
- The following phase summary should connect the selected Mutation to an
  observable result without implying that one noisy run proves causality.
- Use existing candidates only. Do not add catalog content, rebalance effects,
  decide Skip bonuses or Mutation costs, or expand into Genome-tree design.
- Keep player copy separate from researcher evidence. No raw diagnostics or
  unsupported causal certainty enters the player window.

## Current implementation audit and decision gate

This is a source/test inspection, not a Unity runtime test. The live player
preview currently forces `bevExperimentalFeaturesEnabled` on in
[`SpeciesSimulationPreview.Awake()`](../../Assets/Scripts/Game/Presentation/SpeciesSimulationPreview.cs).
That routes phase offers through the legacy BEV catalog, whose deterministic
offer builder in
[`SpeciesUpgrade.cs`](../../Assets/Scripts/Game/Species/SpeciesUpgrade.cs)
returns **two** choices (five Hare candidates are available to that builder).
Existing PlayMode tests in
[`SpeciesPresentationPlayModeTests.cs`](../../Assets/Tests/PlayMode/SpeciesPresentationPlayModeTests.cs)
assert two choices. The accepted contract requires three choices at each of
five boundaries. S3-03 is marked complete and its retained tests cover five
choice boundaries, but they do not cover the required three options at each
boundary. Treat the cardinality difference as an open contract gap; Josh must
decide whether the smallest offer-builder/test correction belongs in S3-04 or
should be tracked separately. Do not report the offer as contract-complete
until the three-choice requirement is verified.

The preview and both current scenes serialize seven provisional authored Hare
candidates under
[`Upgrades/Production`](../../Assets/Data/CellularSimulation/Upgrades/Production/).
`SeedPouches` changes starting food/energy and is marked ineligible after run
start by `SpeciesUpgradeSnapshot.CanApplyAfterRunStart`. The other six pass
that specific timing check, but the authored reward path prevents an
already-owned asset from being chosen again. Six choices cannot guarantee
three eligible authored options at all five boundaries if selected assets
leave the offer pool. Passing the narrow timing check is not proof that a
candidate's effect has been validated as a meaningful player Mutation. The
current implementation's cost and owned/available gates are inherited upgrade
behavior, not an approved Mutation-economy decision. The expedition contract
does not settle Mutation costs or a Skip bonus; S3-04 must not invent either.

Initial candidate screen from the serialized assets (raw values below are for
the internal audit only, not player copy):

| Candidate | Current authored effect | After-run timing gate | Initial note |
| --- | --- | --- | --- |
| Trailblazer: Long Stride | Movement speed +0.5; reproduction-neighbor requirement +1 | Passes | Two effects and a clear tradeoff; verify measured result. |
| Trailblazer: Far Sight | Vision range +1; energy metabolism +1 | Passes | Potentially legible; verify “see food sooner” in current Hare behavior. |
| Warren: Room to Breed | Reproduction group size +1; crowding energy penalty -1; metabolism +1 | Passes | Three effects; likely too dense for the first readable slice. |
| Warren: Guarded Burrow | Combat block +2; movement speed -0.25 | Passes | Candidate tradeoff has a direct combat measure; verify timing and presentation. |
| Gardeners: Careful Sowing | Seed-drop chance +0.1; movement speed -0.25 | Passes | Target is Hare although the authored description refers to spreading seeds; validate species semantics before use. |
| Familial Bond: Large Litters | Crowding tolerance +3 | Passes | Single modifier, but confirm the authored title/description matches the actual consequence. |
| Gardeners: Seed Pouches | Starting food reserve +2; starting energy -2 | Fails | Starting-state-only values cannot be applied at a phase boundary. |

“Passes” means only that the current `CanApplyAfterRunStart` property does not
reject those attribute IDs. It is not runtime or balance validation. The
current legacy `Tough Hide` option is outside this authored-asset table and is
the separate proposed first review candidate below.

**Recommended bridge for Josh to confirm:** keep the current five Hare BEV
effects as the bounded S3 Mutation slice and show three options from that
existing set, while keeping research identities/evidence separate from the
player-facing Mutation projection. This avoids new catalog content, but still
requires Josh to approve whether previously selected Mutations may appear or be
selected again and how repeated effects stack. The seven authored candidates
remain provisional; do not silently promote them or expand their catalog in
this task. If the bridge is rejected, pause UI/evidence implementation until
the replacement pool can support three choices at every decision point without
breaking the no-catalog-expansion constraint.

The present offer formatter includes raw modifier rows and cost/availability
text. The current phase message reports only data earned from survivors, and
the normal Rewards/Results path exposes BEV Stat-Line and upgrade-level
diagnostics. These are observed source/UI bindings; whether every live scene
renders them has not been rechecked in Unity. The S3-04 presentation work must
replace the diagnostic-first language on the player surface while keeping
research output on an explicitly separate developer/research surface.

## Work plan

| Step | Work | Owner | Effort | Output |
| --- | --- | --- | ---: | --- |
| 1 | Reconcile the source pool and repeat/stacking rule for five choice moments, then audit stable IDs, exact effects, phase applicability, and evidence measures for the approved bounded candidate set. | Sim, with Josh as decision owner | 1.5h Sim; 0.5h Josh | Approved bridge/source decision and candidate/effect/timing map. |
| 2 | Define the player-facing offer and next-phase summary pattern: icon + keyword, when it takes effect, a brief expected change/tradeoff, then one concise factual observation at the next summary. | Josh | 0.75h | Copy and presentation acceptance notes for the selected candidates. |
| 3 | Run a bounded, deterministic Forest Edge/Hare comparison for the selected candidates. Use matched inputs and a small seed panel; preserve mutation identity, acquisition round, values, order, and run provenance. Expand the panel only if variance makes the observation unclear. | Sim | 3.5h | Evidence comparing expectation with observed local and species-level outcomes, with limitations stated. |
| 4 | Review the offer and following phase summary against the accepted player contract and the evidence. Record accept/revise for this slice and any separate follow-up needed. | Josh with Sim | 0.75h Josh; 0.5h Sim | Human decision and updated S3-04 notes. |
| **Total** |  |  | **2h Josh; 5.5h Sim** | **0.5h of Sim time remains for corrections/reporting within the planned 6h.** |

The S3-04 card allocates Josh 2h and Sim 6h. Reserve Sim's remaining 0.5h
for a focused rerun or report correction; do not use it to broaden the catalog
or seed panel without a scope decision.

## Evidence approach

1. Identify the exact candidate and its predicted local effect before running
   anything. Check that its effect is supported by the stable attribute
   registry/runtime applier and that its timing matches a phase-boundary
   Mutation.
2. Compare baseline and selected Mutation using the same Forest Edge/Hare
   scenario, starting state, seed, and six-round schedule. Retain the ordered
   Mutation snapshot and the phase in which it is acquired.
3. Inspect the next phase's local measure and relevant Hare/species outcome.
   Report what changed, what did not, variation across the small seed panel,
   and any result that did not match the player-facing expectation.
4. Treat the result as diagnostic evidence for the reviewed candidate and
   scenario only. Do not infer player fun, universal value, or full-catalog
   balance from this pass.

## Acceptance

- The phase boundary presents three eligible temporary Mutations and Skip
  under the S3-03-approved choice/economy behavior. The offer does not imply
  Genome ownership or expose raw modifier rows; do not invent Mutation price
  or Skip reward wording while those rules remain unsettled.
- Each reviewed offer has a recognizable icon + keyword and communicates its
  application timing and practical tradeoff without a stat dump.
- The next phase summary surfaces an observed consequence relevant to that
  Mutation and does not overstate causation beyond the retained evidence.
- A bounded Forest Edge/Hare evidence note records the candidate, exact
  snapshot, inputs/seeds, observed result, limitations, and Josh's accept/revise
  decision.
- Focused regression coverage verifies three distinct offers plus Skip at each
  decision boundary, selection taking effect on the next phase without
  replacing the run, player copy staying within the approved display contract,
  phase summary values matching the completed phase window, and research-only
  diagnostics staying off the normal player surface.
- Existing snapshot/order/fingerprint and same-world continuation checks
  remain green; S3-04 does not weaken evidence provenance to simplify display.
- No catalog expansion or broad balance claim is made. Any behavior defect or
  unsupported candidate becomes a separate, explicitly scoped follow-up.

## Dependencies and safeguards

- S3-03 is marked complete. The focused End/cancel test passed 1/1 and the
  no-graphics PlayMode suite passed 31/33 with 0 failures and two expected
  graphics-only skips, using an isolated copy of the same source and scene
  corrections; no graphics pass is claimed. The changes are still local and
  the handoff's next step is to push them. Review that checkpoint after it is
  available, then avoid overlapping implementation edits.
- S3-03 closed the run-flow work; S3-04 must not silently reopen it. If Josh
  assigns the three-offer correction to S3-04, keep that correction limited to
  satisfying the already-approved contract and add focused coverage. S3-04
  owns readability and its bounded review; it must not infer Mutation cost,
  Skip bonus, repeat-selection, or stacking policy from inherited code.
- The current preview has both a forced-on legacy BEV offer path and an
  authored upgrade path that formats raw modifiers plus cost/ownership status.
  Confirm the player Mutation source after S3-03 before changing presentation;
  do not assume either upgrade path is already an approved Mutation economy.
- Apply the existing
  [simulation-window concern](../Planning%20Concerns/simulation-window.md),
  especially SIMWIN-C04 (keep raw diagnostics out of player UI) and SIMWIN-C03
  (presentation must not own simulation lifecycle).
- Apply the existing
  [upgrade-system concerns](../Planning%20Concerns/upgrade-system.md): UPG-C01
  for stable IDs and explicit runtime mapping, UPG-C03 for immutable run
  provenance, UPG-C04 for ordered Mutation semantics, and UPG-C06 if the legacy
  runtime path is adapted or replaced.
- The [accepted expedition contract](S3-02-expedition-contract.md) is
  authoritative. This plan does not reopen its six-round cadence, five choice
  moments, temporary Mutation scope, Skip decision, or no-stat-dump rule.

## Out of scope

- New Mutation assets, new effect types, or a catalog-wide rewrite.
- Production balance approval, exhaustive matchup testing, Adaptation Value
  calibration, or proving player fun.
- Changing Genome ownership, costs, active configuration, or Gene Lab flow.
- Deciding a Skip bonus or changing the performance-to-currency formula.
- Broad visual polish owned by S3-06.

## Owner decisions still needed

- Confirm this bounded plan and effort split.
- Choose or revise the recommended S3 bridge candidate source and state whether
  a selected Mutation may be offered/selected again during one expedition and
  how repeated effects stack.
- Confirm whether Mutations have a currency cost. The existing prototype shows
  costs, but the player contract does not settle this; do not mistake inherited
  code for an approved rule.
- Confirm the player-facing meaning is limited to icon + keyword, brief timing
  and tradeoff, and a concise observed next-phase consequence; no full stat
  detail is shown.
- After the candidate audit, approve the small review set before the matched
  run begins.

**Proposed first review candidate:** Tough Hide from the current experimental
Hare set. It is a compact defensive effect with a direct `CombatBlocked`
phase-window metric in
[`SpeciesSimulationMetrics.cs`](../../Assets/Scripts/Game/Simulation/SpeciesSimulationMetrics.cs).
Compare a matched no-Mutation arm with an arm selecting it after round 1 and
skipping later choices. Phrase the next-phase result as an observation (for
example, blocked attacks recorded), not a causal population claim. Josh must
approve or replace this candidate before the evidence run.
