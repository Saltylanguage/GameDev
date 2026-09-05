# Sprint 2 Plan — First Trustworthy Upgrade Loop

> **Status:** Active
> **Dates:** September 3–16, 2026
> **Plan owner:** Josh
> **Capacity:** Josh 20h; Sim 20h

## Goal

**New dependency review, 2026-09-04:** the
[consecutive-phase plan](CONTINUOUS_SIMULATION_FLOW_PLAN.md) covers continuing
the same ecosystem after an upgrade or Skip. The completed work below describes
the existing launch-time/fresh-window slice, not continued-world acceptance.
Its test evidence and historical scope remain valid. The new packages are
proposed and are not silently added to S2's committed capacity. References below
to the “next run” in the prototype describe current behavior; target gameplay
continues the next phase of the same expedition.

[Stat-Line/research impact](CONTINUOUS_SIMULATION_EVIDENCE_IMPACT.md) records
the continued-flow retests. The S2.3 board card is now Josh-owned, matching
this plan; Sim is not assigned to the upgrade stream and remains in the
separate telemetry/fixture lane below.

Give the player a small, understandable upgrade choice during a Forest Edge
run. The game and the research tools should use the same upgrade definition so
we can trust the results we collect.

## What success looks like

- The player can see a small catalog of temporary upgrades.
- One choice carries into the next run and the result explains what changed.
- The same choice can be tested by the research tools without being rewritten
  by hand.
- Baseline and upgraded runs can be repeated with the same seeds and compared.
- We clearly separate working behavior from balance questions still needing
  design review.

This sprint does not include the Lab wallet, permanent upgrades, save data, a
large upgrade library, or final art and audio.

## Ownership and capacity

Josh owns the entire upgrade stream: rules, assets, authoring, tests, research
handoff, evidence, and acceptance. Sim is not assigned to this feature; Sim's
Sprint 2 work remains in the separate lanes below.

The upgrade stream is larger than one 20-hour lane, so work is staged. The
combined research-and-balance block below is a working 6-hour estimate and
should be confirmed when the next sprint is kicked off.

## Work at a glance

| Work | Owner | Effort | Status |
| --- | --- | ---: | --- |
| Write down the upgrade rules | Josh | 4h | Complete |
| Create the first catalog and authoring path | Josh | 8h | Complete; seven production assets and the catalog validator are ready |
| Apply upgrades consistently and show what changed | Josh | 8h | Complete |
| Add catalog fixtures and invalid-combination tests | Josh | 3h | Complete; 200/200 Edit Mode tests pass |
| Show upgrade choices and the result in the game | Josh | 4h | Complete; 200/200 Edit Mode and 14/14 runnable Play Mode tests pass |
| Connect upgrades to research and review balance evidence | Josh | 6h | Adapter-backed fixture path and bounded EX-007/EX-009 decisions complete; production balance and player-experience follow-up remain separate |
| EX-002 research intervention preparation | Josh | 5h | Separate research lane |
| Fox mating/eating telemetry discrepancy | Sim | 3h | Separate Sim lane |
| BoardSnapshot test-fixture repair | Sim | 2h | Separate Sim lane |

## Completed work

The current implementation has passed the final Edit Mode gate: **200 of 200
tests passed**. The catalog slice is complete with seven authored production
assets across the Trailblazer, Warren, and Gardeners directions. The
player-facing flow uses those assets, shows costs and availability, applies the
selected temporary upgrade to the next run, and summarizes the selected effect
when the run ends.

The research bridge is now exercised end to end. EX-007's historical research
fixtures can be resolved through the same snapshot adapter as production
loadouts, and the locked EX-009 order comparison completed on the same held-out
seeds with zero deltas across the recorded outcomes and available telemetry.
The raw bundles and a diffable paired table are linked from the experiment
packages. This closes the implementation handoff; it does not close the human
balance decision or authorize a production balance change.

## Completed block — connect research and balance review

This was the coordinated block with two closely related parts. The catalog and
adapter bridge are complete; the remaining work is evidence review, a human
balance review, and any follow-up tests the decision requests. The bounded
EX-007 and EX-009 decisions are now recorded; neither promotes a production
balance change.

### Part 1: Use the same upgrade in research

**Intention:** Make sure a research experiment tests the same upgrade the player
would receive.

**Needed:** A small adapter that takes the resolved upgrade values used by the
game and creates the research input from them. This is complete:
`-UpgradeAssetSequence` resolves production assets by stable ID and feeds the
same immutable snapshots into the simulation runner. An explicit
`-UpgradeAssetCatalogPath` lets historical research fixtures use the same
adapter without substituting production values.

**Problem solved:** The game and research tools could otherwise look like they
are testing the same upgrade while quietly using different values or order.

**Done when:** A game loadout and a research input agree on the upgrade ID,
values, order, and run signature. The report records the full prediction input
and snapshot fingerprints, so the research path no longer needs a separate
handwritten version of the intervention. This gate is complete. The remaining
human work is to decide whether a separate balance or player-experience test is
warranted.

### Part 2: Review the evidence and player experience

**Intention:** Decide what is working and what still needs tuning.

**Needed:** Matched baseline-versus-upgrade runs and a short written decision
record. Font, layout, and visual-feedback review are a separate work track.

**Problem solved:** A single interesting run or unclear UI could be mistaken for
proof that an upgrade is balanced and ready.

**Done when:** The matched runs are repeatable and the handoff records accepted
behavior, tuning questions, and explicit cuts. No balance conclusion is based
on one seed. The research-side matched-run and order checks are complete, and
the bounded human decisions are recorded. Player-experience, balance, and
visual-feedback follow-up remain separate work.

## Next bucket — continuation parity foundation

**Status:** CF-0 contract and fixture complete. CF-1 lifecycle and the
controlled CF-2/CF-3 preview path are implemented and covered by managed
builds/focused tests; current Unity-suite revalidation is pending while the
editor is open. Telemetry and experiment integration remain.
**Owner:** Josh, with Sim reviewing the phase and expedition measurements.
**Effort:** CF-1 is estimated at 8–12h. These are proposed packages, not an
automatic change to the completed S2 capacity.

### Goal

Prove that Continue and Skip preserve the same evolving world before using the
new flow to study mid-run upgrades. The first proof should be small enough to
debug quickly and strict enough to catch a reset, an extra tick, or lost state.

The 100-tick boundary is a test fixture only. It does not change the product
target of a normal 200-tick phase.

### Work items

1. **Consume the locked contract.** Use the lifecycle, launch-only effect,
   above-cap energy, Restart and phase/expedition result decisions recorded in
   CF-0; do not reopen them in runtime code.
2. **Capture a fresh control.** Run an uninterrupted 200-tick case from the
   current code revision and preserve its seed, scenario, ruleset, options, and
   report provenance.
3. **Run the parity test.** Compare the uninterrupted case with 100 ticks,
   Skip, and another 100 ticks using the same initial conditions.
4. **Run one upgrade probe.** From the same tick-100 checkpoint, apply a live
   behavior upgrade and continue for 100 ticks. The controlled preview now
   supports this interaction; record exactly when it becomes active and what
   changes afterward.
5. **Write the result down.** Keep the test output separate from the old
   launch-time reports and state whether parity passed, failed, or remains
   unresolved.

### Test cases

| Case | Setup | What it tells us |
| --- | --- | --- |
| A | 200 ticks without a break | Current-revision control |
| B | 100 ticks → Skip → 100 ticks | Whether Skip preserves the world |
| C | 100 ticks → upgrade → 100 ticks | Whether a boundary upgrade is applied at the right time |

### Acceptance gate

- A and B match at tick 100 and at tick 200 for the complete simulation state,
  not only final population.
- The comparison includes cell state, entity identity, prior-grid/perception
  state, event order, raw counters, population history, and Stat-Line output.
- Skip changes no rules or purchase/loadout state. The completed phase reward is
  settled once before either Skip or Purchase, and Continue does not consume an
  extra tick.
- C records the upgrade and its effective tick as 101. Nothing before that tick
  is relabelled or changed.
- C uses an upgrade that can actually act after tick 100. If the final result is
  unchanged because no relevant opportunity occurs, that is recorded as “no
  observed effect in this fixture,” not treated as automatic proof of failure.
- The old 200-tick report remains historical and untouched. Exact comparison uses
  a fresh control from the same current revision and compatible report contract.

### Stop conditions

- If A and B differ, stop before adding more upgrade behavior. Find the reset,
  clock, prior-grid, identity, or telemetry problem first.
- If phase and expedition Stat-Line meanings are still unclear, stop at the
  contract instead of changing formulas to make the test pass.
- Do not use Seed Pouches for the first mid-run upgrade probe. Its current
  effects are starting reserve and starting energy, so its phase-break behavior
  has not been defined.

### Not part of this bucket

- Full EX-010 execution or predictive calibration for continued play.
- Player-facing polish or a new prediction dashboard.
- New upgrade design or balance conclusions.
- Rewriting old EX-007, EX-008, or EX-009 evidence.

The detailed lifecycle and telemetry dependencies are in the [continuation
flow plan](CONTINUOUS_SIMULATION_FLOW_PLAN.md) and the [Stat-Line and research
impact review](CONTINUOUS_SIMULATION_EVIDENCE_IMPACT.md). The existing
cross-over concerns remain in the [Stat-Line/Predictive AI concern record](Planning%20Concerns/STATLINE_PREDICTIVE_AI_CROSSOVER.md).

## Carried-forward research item

**EX-010 — Sequential upgrade continuation** is proposed but intentionally not
part of this S2 implementation commitment. It asks what changes when upgrades
are acquired between simulation segments and the player continues from the
current state. It depends on the active-run checkpoint/resume flow, which is
currently out of scope. Keep it visible in the research index and Loose Ends;
do not treat EX-009's launch-time result as an answer to EX-010.

## Evidence for the completed launch-time slice

- Run the baseline and each selected upgrade with the same 20 seeds.
- Repeat the comparison on five held-out seeds before promoting an effect
  direction.
- Keep the scenario, grid, duration, step interval, and starting roster the
  same across each comparison.
- Record the selected upgrades and the run signature with every result.
- Check replay equality and the existing telemetry totals before discussing
  balance.

The existing Forest Edge baseline and faster-movement results remain evidence
inputs. The faster-movement result is descriptive only because its held-out
effect changes direction.

## Previous S2 review points

- **Mid-sprint:** Confirm the catalog rules and one repeatable baseline-versus-
  upgrade run before adding more content.
- **Final review:** Confirm game/research agreement, run the matched comparison,
  inspect the preview and result text, and record carry-over work.

## Out of scope for the completed S2 slice

- Lab currency, permanent research, save/load, or disk checkpoint resume;
- a general upgrade plug-in framework;
- new species or scenario content beyond the Forest Edge slice;
- font, layout, and visual-feedback review;
- final-volume art, audio, or broad UI framework work;
- unrelated deferred mechanics such as scent or generalized event systems.

## Completed S2 definition of done

- The upgrade rules and catalog are documented and traceable.
- A baseline and upgraded run can be repeated from the recorded seed and
  selected upgrade.
- The game and research tools use the same upgrade values.
- Focused tests pass and the no-upgrade baseline remains unchanged.
- Remaining balance, art, telemetry, and research questions are named clearly
  for the next sprint.
