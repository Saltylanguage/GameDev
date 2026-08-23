# Future Sprint Roadmap — Draft Planning Horizon

> Status: Draft planning baseline | Updated: 2026-08-23 | Cadence: two weeks

This document turns the current planning themes into a dependency-aware horizon
for future sprint selection. It is deliberately a planning map, not a committed
Trello sprint backlog. A theme becomes sprint scope only after its outcome,
owner, estimate, acceptance evidence, and capacity impact are written down.

## Planning principles

- Keep one primary playable or product outcome per sprint.
- Separate design readiness from implementation commitment. A design spike can
  prepare later work without pulling the full feature into the current sprint.
- Treat tooling as an accelerator lane. A feature must retain a manual fallback
  and must not wait on a new tool unless the tool itself is the explicit sprint
  outcome.
- Keep the two-week planning capacity visible, but do not auto-fill capacity
  with speculative work. Reserve roughly 10–20% for integration, defects, and
  small evidence-producing improvements.
- Preserve deterministic scenario, seed, ruleset, upgrade-loadout, and save
  evidence so later balance and migration decisions can be reproduced.

## Theme map

| Theme | First useful outcome | Dependencies | Candidate horizon |
| --- | --- | --- | --- |
| File save/load | A versioned local profile can save, load, reset safely, migrate, and fall back from corrupt data. Keep settings, meta-progression, and active-run resume as separate decisions. | Profile shape, settlement/wallet rules, stable IDs | Design can start during S2–S3; implementation belongs after the profile and settlement contracts, targeting the roguelike-loop sprint. |
| Species and scenarios | A small roster and scenario matrix create different strategic pressures without balancing the whole library. | Upgrade vocabulary, slice scenario evidence, species identities | Slice co-design in S3; broader content after vertical-slice validation. |
| Reactive species / ecology | A deterministic predator–prey upgrade loop creates counterplay, tension, and recoverable pressure without hidden catch-up multipliers. | S2 upgrade contract/catalog, Forest Edge baseline envelope, existing BEV/EX-002 telemetry, Hare/Fox fixture | Design spike and paired counter trials in S3; recovery validation during S6. See [`REACTIVE_SPECIES_ECOLOGY_PLAN.md`](REACTIVE_SPECIES_ECOLOGY_PLAN.md). |
| Iteration tooling | One measured authoring/comparison workflow becomes faster without becoming a feature dependency. | Repeated manual friction, explicit inputs/outputs, manual fallback | Parallel lane from S2 onward; promote only evidence-backed tools. |
| Upgrade system | Clear separation between temporary per-run evolution and permanent Lab research, including costs, prerequisites, stacking, exclusions, and persistence boundaries. | Product economy and run cadence decisions | S2 design foundation; integrate permanent research after the wallet/settlement contract. |
| Actual upgrades | A small explicit catalog proves numeric, spatial, conditional, and tradeoff effects with visible previews and telemetry. | Upgrade grammar, deterministic effective ruleset, first slice species | S2 first catalog; S3 co-design and pruning. |
| Art bible | A compact visual language covers board readability, species roles, terrain, selection, danger, upgrades, typography, panels, animation, and feedback. | UI/UX states and a validated slice direction | Direction work can accompany S2–S3; lock the bible before broad asset production in S4. |
| UI/UX design | Player-facing flows, information hierarchy, language, focus, input, empty/locked/error states, and feedback are explicit and testable. | Current S1 shell contract, player/Dev Lab boundary | S1 establishes the shell; deepen and validate the full slice in S4. |

## Candidate sprint horizon

These are candidate outcomes, not commitments. Each review should select only
the next outcome supported by current evidence.

### S2 — First trustworthy upgrade loop

Define the per-run/permanent boundary and implement the smallest explicit
catalog (roughly 6–10 candidates). Prove one numeric, one spatial, one
conditional, and one tradeoff upgrade with previews, deterministic application,
and contribution telemetry. Record the data needed by future save/load and
settlement work, but do not build a generalized upgrade framework.

### S3 — Species and scenario co-design with an accelerator lane

Select the smallest vertical-slice roster, write concise species identities,
and create a scenario matrix that pressures different strategies. Run fixed-seed
baselines and comparisons. In parallel, improve only the highest-friction
iteration workflow—such as seeded A/B reports or definition validation—while
keeping a manual path for every feature decision.

The reactive-species design spike belongs in this horizon. Define explicit
Herbivore/Carnivore counter pairs, establish role-specific pressure envelopes,
and instrument the upgrade boundary before implementing rubber banding. The
bounded counterplay proposal and evidence sequence are in
[`REACTIVE_SPECIES_ECOLOGY_PLAN.md`](REACTIVE_SPECIES_ECOLOGY_PLAN.md).

### S4 — Art bible, UI/UX, and readable feedback

Lock the vertical-slice art direction and convert the existing UI contract into
a tested player-facing language and state model. Cover board scale, hierarchy,
focus, input, upgrade previews, danger, success, failure, and result feedback.
Use temporary assets where needed; defer final-volume production until
comprehension evidence supports the direction.

### S5 — Profile, save/load, and run settlement

Implement the smallest versioned local profile and settings format, safe reset,
corrupt-save fallback, migration tests, and deterministic run settlement. Make
earned, spent, banked, and lost data explainable. Active-run save/resume remains
a separate decision and should not silently enter this scope.

### S6 — Vertical-slice integration and validation

Connect the player flow from new profile through scenario launch, upgrade
choices, results, save/load, permanent research, and the next-run decision.
Run structured comprehension, build-diversity, replay-intent, compatibility,
and performance checks. Use the evidence to decide which species, scenarios,
and tools enter content alpha.

### Post-slice content alpha

Expand species, scenarios, upgrade families, mastery, and authoring support only
after the slice proves the upgrade grammar, save compatibility, readability, and
performance budgets. Unproven mechanics remain research or post-launch
candidates rather than becoming automatic sprint scope.

## Dependency sketch

```text
S1 UI shell and Lab boundary
  -> S2 upgrade grammar + first catalog
       -> S3 species/scenario co-design + reactive ecology spike
            -> S6 vertical-slice validation
  -> S4 art bible + UI/UX readability
  -> profile/settlement contracts
       -> S5 save/load + run settlement
            -> S6 vertical-slice validation

Tooling accelerator lane --------------------------> supports S2–S6
  (never a prerequisite unless explicitly selected as the sprint outcome)
```

## Promotion checklist for future Trello cards

Before moving a theme into `🎯 Upcoming Work`, record:

1. The single outcome and explicit non-goals.
2. The owner, reviewer, estimate, and two-week capacity impact.
3. Dependencies, risks, stable IDs, and data/persistence implications.
4. The thinnest end-to-end slice and its acceptance evidence.
5. The manual fallback if tooling slips or remains incomplete.

The next planning step is to refine S2 candidates from this map after the S1
review, not to populate every future sprint in advance.
