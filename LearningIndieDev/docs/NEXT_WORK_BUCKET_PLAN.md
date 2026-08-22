# Next work bucket — S2 first trustworthy upgrade loop

> Status: Proposed next two-week bucket | Owner: Josh + Sim | Capacity: 20 committed hours + 3-hour reserve

This bucket turns the current cellular-automata prototype into a small,
deterministic upgrade loop. It is the next candidate after Sprint 1 review; it
does not start until the Sprint 1 Windows development-build gate is either
accepted or explicitly carried as a named risk.

## Outcome

From a deterministic Forest Edge run, a player can inspect a small catalog,
choose a temporary upgrade, see its effective rule/loadout change, and produce
repeatable evidence showing what changed and why. The same slice distinguishes
per-run evolution from permanent Lab research without implementing the full
wallet, save system, or permanent progression.

## Entry gates and carry-over

- Close or explicitly carry the Sprint 1 Windows development-build/final-review
  gate on Trello card 53.
- Keep the existing Main Menu/Lab shell as the presentation entry point; do not
  reopen completed cards 51 or 52.
- Keep the graphics-capable Noesis atlas path as the accepted preview surface;
  the generic nographics texture failure is an environment boundary, not an
  S2 entry blocker.
- Current-head EditMode (139/139) and graphics-capable PlayMode (6/6) validate
  the Fox food-action implementation; do not promote balance evidence until a
  schema-7 Forest Edge run reconciles the counters across its seeds.
- Keep Forest Edge balance and Fox reproduction findings as evidence inputs;
  do not rebalance from a single seed.

## Committed work packages

### S2.1 — Upgrade contract and boundary (4h)

The current `SpeciesUpgrade`/`SpeciesProgression` path is a real, testable
per-run mechanic for movement speed, attack amount, and block amount. The
three reward buttons are therefore under-presented implementations, not empty
placeholders; they still lack a durable player-facing contract and result
summary. Keep this narrow catalog path and do not introduce a generic modifier
framework.

Define and record for each upgrade:

- stable upgrade IDs and an ordered loadout representation;
- player-facing name and description;
- affected mechanic/stat, magnitude/value, and valid range;
- tradeoff or cost, eligibility/preconditions, and persistence/duration scope;
- result-summary representation that explains what changed and what the next
  run will inherit;
- the boundary between temporary per-run upgrades and permanent Lab research;
- effect grammar for numeric, spatial, conditional, and tradeoff changes;
- cost/prerequisite fields, stacking and exclusion rules, and preview text;
- the immutable run-start snapshot and fingerprint implications.

Acceptance: this plan and focused data tests make the boundary unambiguous
without introducing save/load or a generalized plugin framework. Results must
be able to identify the selected upgrade, its applied value, and its scope even
before the final UX copy is implemented.

### S2.2 — First catalog slice (8h)

Implement roughly 6–10 explicit, data-backed candidates for the Forest Edge
slice. At minimum, prove one each of:

1. numeric effect;
2. relative-grid/spatial effect;
3. conditional effect;
4. tradeoff effect.

Each candidate must expose its ID, cost, prerequisite, effect summary, and
non-goals. The catalog should support the intended Trailblazer, Warren, and
Gardeners directions without pretending the entire upgrade library is final.

Acceptance: a player can inspect available, locked, and unaffordable states;
one temporary choice is applied to the next deterministic run; no real wallet
or persistence mutation occurs.

### S2.3 — Deterministic application and contribution evidence (5h)

- Apply an ordered temporary loadout at run start.
- Preserve the effective ruleset fingerprint and replay metadata.
- Record which upgrade changed which rule or result signal.
- Add focused Edit Mode coverage for ordering, stacking/exclusion, and
  unchanged baseline behavior.

Acceptance: paired baseline/upgrade runs use the same seed and report the
  effective loadout plus contribution telemetry; the baseline remains
  reproducible when no upgrade is selected.

### S2.4 — Review and balance evidence (3h)

- Run a small fixed-seed comparison for the four effect classes.
- Verify the preview language and board readability at the existing target
  resolutions.
- Record accepted behavior, tuning questions, and cuts in a handoff and the
  corresponding Trello cards.

Acceptance: evidence separates implementation correctness from balance claims;
no single-seed result is promoted as a product conclusion.

## Next evidence lane — Forest Edge baseline before balance tuning

The S1 build gate, telemetry validation, and the 20-seed Forest Edge control are
accepted. The control report is
`artifacts/cellular-experiment-20260820-123724/report.json` with analysis at
`artifacts/cellular-experiment-20260820-123724/analysis.md`.

Continue the matched Forest Edge experiment before changing balance values:

- 20 fixed seeds for the baseline, using the current authored scenario and
  player species; repeat the same 20 seeds for each single upgrade/control arm.
- Hold grid dimensions, duration, step interval, scenario fingerprint, and
  starting roster constant. Record the ordered loadout and effective ruleset
  fingerprint for every run.
- Capture final and peak populations, collapse/zero-population rate, births,
  deaths by cause, food consumed, food-action attempts/successes/failures,
  combat kills, state ticks/transitions, and reproduction reconciliation.
- Re-run any selected arm on five held-out seeds before promoting an effect
  direction. Derive acceptance thresholds from the baseline distribution;
  do not invent a target population or success threshold in advance.
- Require fixed-seed replay equality, attempts = successes + failures, death
  event/activity reconciliation, and no baseline regression before discussing
  balance or roster expansion.

## Reserve and parallel lanes

Keep a 3-hour integration reserve for Unity/Noesis defects, review changes, and
test repair. Tooling may consume part of the reserve only when it removes a
measured repeated friction and retains a manual fallback. The bounded EX-002
intervention matrix remains a parallel evidence lane; its historical schema-6
reports are not invalidated by the new schema-7 telemetry.

## Explicitly out of scope

- file save/load, migrations, corrupt-save recovery, or active-run resume;
- real currency earning, spending, banking, or permanent research persistence;
- a generalized upgrade/plugin framework;
- expanding the species/scenario roster beyond the Forest Edge slice;
- final-volume art, audio, or broad UI framework work;
- scent, generalized event buses, custom terrain registries, or other deferred
  mechanics without an activated trigger.

## Definition of done

- The contract and catalog are durable in repository docs and tracked on
  Trello with owner, reviewer, estimate, dependencies, and non-goals.
- A deterministic baseline and one upgraded run are reproducible from recorded
  seed, scenario, fingerprint, and ordered loadout.
- At least four effect classes are represented and visibly explained.
- Focused tests pass; the baseline path remains unchanged without a loadout.
- Remaining balance, art, telemetry, and research gaps are named rather than
  silently rolled into the next sprint.

## Promotion decision

At the Sprint 1 review, promote this bucket only if the shell/build gate is
accepted and the contract can be reviewed in one sitting. Otherwise carry the
specific failed gate—not the entire bucket—and re-estimate before starting.
