# S3-04 — Mutation readability and bounded Forest Edge review

**Status:** Implementation, comparison, and player-facing review complete
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

The first implementation and evidence pass is recorded in the
[2026-09-27 Tough Hide handoff](../handoffs/2026-09-27-codex-s3-04-tough-hide.md).
The five existing Mutations now produce three free, distinct options at phase
decisions, with Skip and repeat-level behavior. Josh's player-copy and generic
card-marker treatment was accepted for this slice after reviewing the bounded
Tough Hide evidence.

The two-pool-plus-Reinforcements bridge described in the dated planning and
baseline sections below was superseded by this three-distinct-Mutation offer.
Reinforcements remains historical context, not the current third choice.

## Locked product boundaries

- The player gets three Mutation options or chooses Skip at the boundaries
  after rounds 1–5. A selected Mutation is temporary and applies to the current
  expedition; Mutation choices are not Genome purchases.
- S3-04 uses the five existing experimental Hare Mutations as its bounded
  rotating pool. Each phase offer contains three distinct choices from that
  pool. Earlier pool choices may appear again later;
  selecting one again increases its level and reapplies/stacks its effect.
- Mutation selection is free in this S3 slice. Skip grants no reward during
  S3-04; a future Skip bonus remains a separate deferred economy decision.
- The player-facing explanation stays bite-sized and directional. Its wording
  is derived from repeatable, predictable impacts established through retained
  Stat-Line evidence, but it does not expose that evidence as a statistic
  breakdown or reduce the decision to raw numeric values.
- Prefer a recognizable icon, keyword or Mutation identity, followed by a
  short description of the direction it takes the species. When the evidence
  cannot support a precise player-facing claim, deliberately abstract it to
  simpler language such as “Hunter Lv2 — better tracking and sharper teeth.”
- The following phase summary may report what was observed during that phase.
  It must remain distinct from the repeatable impact used to author the
  Mutation description and must not attribute every broader outcome in one run
  to that Mutation.
- Use existing candidates only. Do not add catalog content, rebalance effects,
  add a Skip bonus or Mutation cost, or expand into Genome-tree design.
- Keep player copy separate from researcher evidence. No raw diagnostics or
  unsupported causal certainty enters the player window.

## Baseline implementation audit at plan approval — 2026-09-26

This began as a source/test inspection, not a Unity runtime test. The live
player preview currently forces `bevExperimentalFeaturesEnabled` on in
[`SpeciesSimulationPreview.Awake()`](../../Assets/Scripts/Game/Presentation/SpeciesSimulationPreview.cs).
That routes phase offers through the legacy BEV catalog, whose deterministic
offer builder in
[`SpeciesUpgrade.cs`](../../Assets/Scripts/Game/Species/SpeciesUpgrade.cs)
returns two choices from the role-specific pool and appends fixed
Reinforcements as the third.
The phase-choice purchase path treats both authored and legacy options as
free and formats them without a Data cost. Focused PlayMode coverage verifies
all three legacy choices, the repeatable Reinforcements choice, zero-Data
selection, the authored-option path, and same-run repeated choices. The full
Unity run passed EditMode 256/256 and PlayMode 33/33; see
[`artifacts/unity-tests-20260924-134934`](../../artifacts/unity-tests-20260924-134934/).
This confirms implementation behavior only, not Reinforcements balance or
catalog approval.

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
Phase decisions waive catalog Data costs in both offer paths; non-phase legacy
reward purchases retain their existing costs. This slice allows repeatable pool
choices with level/stack progression, offers repeatable Reinforcements, and
gives Skip no reward. Any broader Mutation or Skip economy remains deferred.

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

**Approved S3 bridge:** keep the current five Hare BEV effects as the bounded
rotating pool, show two distinct pool choices and fixed repeatable
Reinforcements as the third choice, and keep research identities/evidence
separate from the player-facing Mutation projection. All three phase choices
are free. Previously selected pool Mutations may appear again at later choice
boundaries; selecting one again increases its displayed level and stacks its
defined effect. Reinforcements adds one player-species individual at a
deterministic open cell in the next phase and can be selected again. The seven
authored candidates remain provisional; do not silently promote them or expand
their catalog in this task.

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
| 1 | Apply the approved five-Mutation bridge and repeat/level-stacking rule across five choice moments, then audit stable IDs, exact effects, phase applicability, and evidence measures for the bounded candidate set. | Sim, with Josh as reviewer | 1.5h Sim; 0.5h Josh | Implemented bridge/source rule and candidate/effect/timing map. |
| 2 | Define the player-facing offer and next-phase summary pattern: an icon and readable Mutation identity, a short evidence-backed description of the species direction, and one concise factual observation at the next summary. Use qualitative abstraction when the evidence cannot support a precise claim. | Josh | 0.75h | Copy and presentation acceptance notes for the selected candidates. |
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
   Establish which impacts are repeatable and predictable enough to inform the
   Mutation description. Report what changed, what did not, variation across
   the small seed panel, and any result that did not match the player-facing
   expectation.
4. Translate the supported impact into concise qualitative player language.
   If the evidence does not justify a precise claim, use a broader directional
   description instead of presenting a noisy number or an unsupported promise.
5. Treat the result as diagnostic evidence for the reviewed candidate and
   scenario only. Do not infer player fun, universal value, or full-catalog
   balance from this pass.

## Acceptance

- The phase boundary presents three distinct eligible temporary Mutations from
  the existing pool and Skip. All three Mutation choices are free and do not deduct Data, including
  when the player has no Data. Pool choices can reappear at later boundaries;
  choosing one again increments its level and stacks its defined effect.
  Skip has no S3-04 reward. The
  offer does not imply Genome ownership or expose raw modifier rows.
- Each reviewed offer has a recognizable icon and readable Mutation identity.
  Its concise description communicates an evidence-backed direction for the
  species without exposing the Stat-Line or reducing the choice to numeric
  values.
- Precise player-facing claims are supported by direct mechanics or repeatable
  controlled observations. When that standard is not met, the copy is
  deliberately abstracted to a simpler qualitative direction.
- The next phase summary reports relevant observations from the completed
  phase without treating every downstream population or ecological change as
  the Mutation's effect.
- A bounded Forest Edge/Hare evidence note records the candidate, exact
  snapshot, inputs/seeds, observed result, limitations, and an explicit copy
  and presentation decision.
- Focused regression coverage verifies three distinct offers plus Skip at each
  decision boundary, free selection with zero Data, and selection taking effect
  on the next phase without
  replacing the run, player copy staying within the approved display contract,
  phase summary values matching the completed phase window, and research-only
  diagnostics staying off the normal player surface.
- Existing snapshot/order/fingerprint and same-world continuation checks
  remain green; S3-04 does not weaken evidence provenance to simplify display.
- No catalog expansion or broad balance claim is made. Any behavior defect or
  unsupported candidate becomes a separate, explicitly scoped follow-up.

## Dependencies and safeguards

- S3-03 is marked complete. Its focused End/cancel test passed 1/1 and the
  original no-graphics PlayMode suite passed 31/33 with 0 failures and two
  expected graphics-only skips, using an isolated copy; no graphics pass is
  claimed for that run. After the Island Survivor test removal on 2026-09-18,
  the latest retained clean suites report EditMode 234/234 and no-graphics
  PlayMode 28/30 with 0 failures and 2 expected graphics-only skips. The
  earlier S3-03 run and its artifact bundle remain historical evidence. The
  baseline is already pushed; the phase-selection polish handoff still needs
  its focused PlayMode validation after the shared Editor is free. Avoid
  overlapping implementation edits while that check is pending.
- S3-03 closed the run-flow work; S3-04 must not silently reopen it. Keep the
  three-offer correction limited to satisfying the already-approved contract
  and add focused coverage. S3-04 owns readability, the five-Mutation bridge,
  free selection, later repeat offers with level/stack progression, and its
  bounded review. It must not infer any broader economy from inherited code.
- The current preview has both a forced-on legacy BEV offer path and an
  authored upgrade path. Phase-choice formatting must not show a Data cost;
  terminal legacy reward purchases retain their separate cost behavior.
  Implement the approved five-effect bridge without treating either path as a
  broader approved Mutation economy.
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

## Review decision

The current review accepts “Block more incoming attacks” and the existing
generic A/B/C card markers for this slice. The matched run is complete and
supports the directional copy, but does not establish a broader balance or
player-fun claim. Mutation-specific artwork remains outside this slice.

**S3 V1 closeout update — 2026-09-29:** Josh accepts the current Forest Edge
evidence for this sprint. The merged implementation offers three distinct
applicable Mutations from the five-Mutation Hare pool or Skip, and hides
Reproductive Drive or a coupled Fox Brood Drive response when it cannot apply.
The clean merged-baseline suite passes. This satisfies the current S3 V1 gate;
later mutation, ecology, and visual iteration remains expected.

## Confirmed product decisions

Josh confirmed that Mutation hints, prompts, and descriptions are authored
from repeatable, predictable impacts observed through retained Stat-Line
evidence. The player receives a concise qualitative description of the
direction the Mutation takes the species, not the underlying statistic
breakdown. If the evidence cannot support a precise claim, use a simpler
abstraction such as “Hunter Lv2 — better tracking and sharper teeth.” This
addresses the earlier causality-language concern; it remains an acceptance
rule and does not require a separate durable concern record.

For the bounded S3-04 bridge, each boundary offers three distinct applicable
choices from the five existing experimental Hare Mutations, or Skip. Pool
choices can appear again later; selecting one again increments its level and
stacks/reapplies its defined effect. An inapplicable effect is hidden, including
when a coupled species response is capped. All phase Mutations are free; Skip
has no reward. Repeatable Hare Reinforcements are a separate 10-Data purchase
that adds one individual in the following phase when space and population
limits allow. A future Skip bonus or broader Mutation economy is deferred.

**First review candidate, approved by Josh:** Tough Hide from the current experimental
Hare set. It is a compact defensive effect with a direct `CombatBlocked`
phase-window metric in
[`SpeciesSimulationMetrics.cs`](../../Assets/Scripts/Game/Simulation/SpeciesSimulationMetrics.cs).
Compare a matched no-Mutation arm with an arm selecting it after round 1 and
skipping later choices. Use the repeatable blocked-attack effect to author the
directional player description; keep any next-phase population result as an
observation rather than a promised effect. The matched comparison is complete;
see the handoff for its results and limitations.
