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
- Resolve the current Noesis atlas texture-source defect enough to run the
  upgrade preview at gameplay scale, or record a bounded fallback using the
  existing presentation path.
- Keep Forest Edge balance and Fox reproduction findings as evidence inputs;
  do not rebalance from a single seed.

## Committed work packages

### S2.1 — Upgrade contract and boundary (4h)

Define and record:

- stable upgrade IDs and an ordered loadout representation;
- the boundary between temporary per-run upgrades and permanent Lab research;
- effect grammar for numeric, spatial, conditional, and tradeoff changes;
- cost/prerequisite fields, stacking and exclusion rules, and preview text;
- the immutable run-start snapshot and fingerprint implications.

Acceptance: a short design/TDD note and focused data tests make the boundary
unambiguous without introducing save/load or a generalized plugin framework.

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

## Reserve and parallel lanes

Keep a 3-hour integration reserve for Unity/Noesis defects, review changes, and
test repair. Tooling may consume part of the reserve only when it removes a
measured repeated friction and retains a manual fallback. The Fox eating-state
telemetry fix and the EX-002 intervention matrix remain parallel evidence lanes;
they are not prerequisites for the first catalog unless their results change an
upgrade decision.

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
