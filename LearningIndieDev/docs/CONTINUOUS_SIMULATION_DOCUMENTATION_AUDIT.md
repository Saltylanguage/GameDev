# Consecutive simulation phases — documentation coverage

**Date:** 2026-09-05. **Status:** CF-0 through CF-3 are verified; CF-4 phase telemetry and direct-report validator parity are implemented; the CF-5 checkpoint seam and opt-in headless schedule are implemented and smoke-verified. EX-010 remains unexecuted.
**Parent:** [Architecture review and plan](CONTINUOUS_SIMULATION_FLOW_PLAN.md).

## Scope and audit method

The review inventoried first-party project documentation, searched run/phase,
restart/reset, upgrade timing, replay and telemetry references, and read the
controlling design, architecture, upgrade, sprint, Stat-Line and research
contracts alongside their code. Keyword matches were classified by meaning;
not every “phase,” “Continue,” or “reset” refers to this lifecycle.

This pass updates committed direction and adds implementation/applicability
notices to active documents. It does not claim that every proposed API, code
comment, tool help string or acceptance checkbox has already been migrated.
The table below is the project-wide closure list for the implementation pass.

CF-0 is now closed: the lifecycle, boundary-effect, above-cap energy and
phase/expedition result decisions are locked in the [canonical plan](CONTINUOUS_SIMULATION_FLOW_PLAN.md),
and the [schema-21 fresh legacy fixture](fixtures/continuous-simulation/legacy-fresh-schema-21/README.md)
is preserved with provenance. The latest implementation pass adds versioned
phase windows, ordered acquisition timing, checkpoint copy/restore, and fresh
Unity evidence. Remaining closure work is EX-010 contract-specific schedule
parity, documentation cleanup, and human review; it does not reopen the
product contract.

## Active design and engineering documents

| Document | Planning-pass disposition | Implementation closure |
| --- | --- | --- |
| [GDD](GDD_TEMPLATE.md) | Rewrote player promise, core flow and progression around one expedition with purchase/skip continuation. | Replace pending labels when runtime gates pass; retain unresolved pacing/mechanic decisions until accepted. |
| [Product brief](PRODUCT_BRIEF.md) | Explicit same-world continuity, optional Skip, phase versus terminal results, acquisition-time replay and distinction from disk saves. | Verify runtime against the locked cadence/terminal contract and actual outcomes; the longer presentation target remains separate. |
| [Project context](PROJECT_CONTEXT.md) | Durable continuity direction and immutable base/per-phase rules boundary. | Update current implementation state, remove superseded prototype descriptions in touched sections. |
| [TDD](TDD_TEMPLATE.md) | Added lifecycle invariants and canonical implementation/evidence links. | Fill actual APIs, checkpoint format and validation results; unrelated template placeholders remain unrelated. |
| [Working state](WORKING_STATE.md) | Added plan, impact and audit entry points. | Link final integration handoff; keep history out of this index. |
| [Game architecture map](Architecture%20Maps/GAME_ARCHITECTURE_AND_FLOWS.md) | Diagram explicitly loops through same-world Continue and supports Skip. | Verify diagram against actual domain ownership/terminal states. |
| [MVVM UI contracts](UNITY_MVVM_UI_CONTRACTS.md) | Distinguished boundary and final state, proposed Continue command and snapshot revision requirements. | Replace proposed names with actual APIs; retain compatible bindings during migration. |
| [MVVM architecture plan](UNITY_MVVM_ARCHITECTURE_PLAN.md) | Added continuation dependency while preserving T0–T7 acceptance. | Confirm domain ownership and no scene reload at phase transitions. |
| [Main Menu/Lab delivery](MAIN_MENU_LAB_DELIVERY_PLAN.md) | Distinguished Simulation phase flow from terminal Lab transition and disk persistence. | Verify launch/return/abort routes and final result handoff. |
| [Upgrade direction](UPGRADE_SYSTEM_DIRECTION.md) | Run lifetime and acquisition timing clarified; Seed Pouches live-effect gap recorded. | Review all seven production offers plus legacy BEV options for boundary applicability; update actual effect descriptions. |
| [Upgrade authoring](UPGRADE_AUTHORING_GUIDE.md) | Added phase-acquisition provenance and initialization-only eligibility caveat. | Document the accepted authoring/validation fields and reject unavailable boundary effects. |
| [Scientific-data economy](SCIENTIFIC_DATA_ECONOMY.md) | Currency survives phases; separate phase awards and once-only terminal settlement. | Document accepted reward policy; no permanent economy is implicitly added. |
| [Hare/Fox treatment](Species%20Design/HARE_FOX_ITERATIVE_TREATMENT.md) | Old 20-second fixture bounded; evolved-world balance/recovery requires new evidence. | Add matched multi-phase trajectories and decisions; distinguish ecological waves from UI phases. |
| [Hare/Fox implementation](Species%20Design/HARE_FOX_IMPLEMENTATION_PLAN.md) | Linked continuity dependency and late-phase retests. | Close lifecycle gates before claiming upgrade-build recovery is validated. |
| [Reactive ecology](REACTIVE_SPECIES_ECOLOGY_PLAN.md) | Same-world assumption explicit, future reactive work remains unapproved. | When scheduled, define response timing without implicit reseeding or resource refill. |
| [Performance baseline](PERFORMANCE_BASELINE_PLAN.md) | Added full-expedition history/event retention measurement case. | Record measured tick/memory/report costs; do not extrapolate a 20-second profile. |

## Production, Stat-Line and research documents

| Document / surface | Planning-pass disposition | Implementation closure |
| --- | --- | --- |
| [Roadmap](../ROADMAP.md), [incomplete-feature plan](INCOMPLETE_FEATURES_ACTION_PLAN.md), [future sprint roadmap](FUTURE_SPRINT_ROADMAP.md) | New foundational dependency and explicit capacity planning. | Schedule packages; no silent addition to S2. |
| [S2 execution plan](NEXT_WORK_BUCKET_PLAN.md), [S2 control record](Sprints/S2-control-record.md) | Accepted launch-time implementation distinguished from proposed continuation; S2.3 owner is aligned to Josh. | Keep the board mapping and actual sprint allocation current as the continuation work is scheduled. |
| [S1 Stat-Line tickets](SPRINT_1_SPECIES_STAT_LINE_TICKETS.md) | Added mapping to new window/ledger/serialization/replay retests; original acceptance retained. | Sim reviews phase and expedition semantics and new acceptance evidence. |
| [Species analytics guidance](Species%20Design/SPECIES_STATISTICS_ANALYTICS_GUIDANCE.md) | Window context and pooled-count rates explicit. | Keep stable stat names, denominators and validity states consistent with code. |
| [Herbivore validation](HERBIVORE_STATISTICS_VALIDATION.md) | Applicability note and existing cause/litter/denominator limitations linked. | Revise formulas only after accepted stat decisions; independently validate actual new reports. |
| [Simulation tooling](UNITY_SIMULATION_TOOLING.md) | Current fresh-window mode versus continuation contracts distinguished; direct ForestEdge/Hare Stat-Line bundle and generic phase schedule now validate locally. | Document phase-aware report fields, checkpoint boundary limits, and the EX-010-specific schedule gate. |
| [Research plan](Research/AI_ASSISTED_ECOLOGY_LAB_RESEARCH_PLAN.md) | v1.15 dependency notice; no P3 promotion or experimental scope change. | Lock EX-010 only after shared runtime/report/checkpoint gates. |
| [Research paper](Research/AI_ASSISTED_ECOLOGY_LAB_RESEARCH_PAPER.md), [change-impact brief](Research/CHANGE_IMPACT_ANALYSIS_RESEARCH_BRIEF.md) | Current applicability overlay; old results do not establish continuation. | New dated analysis/evidence for changed claims; do not rewrite historical results. |
| [Research index](Research/README.md), [experiment index](Research/Experiments/README.md) | Link evidence validity register at retrieval entry points. | Index new phase-aware studies and supersession links when they exist. |
| [Predictive AI architecture map](Architecture%20Maps/PREDICTIVE_AI_RESEARCH_ARCHITECTURE_AND_FLOWS.md) | Checkpoint/schedule/window dependency connected to EX-010. | Update actual envelope and producer/consumer versions after implementation. |
| [EX-010 proposal](Research/Experiments/EX-010-Sequential-Upgrade-Continuation/README.md) | Concrete implementation dependencies and lineage split requirements added. | Draft contract is prepared; human approval and EX-010-specific schedule parity are still required before execution. |
| [New experiment report template](Research/Templates/EXPERIMENT_REPORT_TEMPLATE.md) | Added lifecycle, window, checkpoint, schedule and termination fields. | Verify templates against actual serializers and validity handling. |
| [Upgrade concerns](Planning%20Concerns/upgrade-system.md), [Stat-Line/AI concerns](Planning%20Concerns/STATLINE_PREDICTIVE_AI_CROSSOVER.md) | Linked review without changing accepted triggers/severity/status. | Review proposed CF concerns; update accepted statuses only from evidence or explicit decisions. |
| [Stat-Line card](https://trello.com/c/pZ4qG2DM), [research card](https://trello.com/c/DViOsvbd) | Planning notices with validity limits and retests; the S2.3 card owner is reconciled to Josh. | Keep scope, list placement, and completion state synchronized with the sprint record; preserve historical handoffs. |

## Documents intentionally retained, with explicit applicability

| Document family | Why its existing text is retained | Required treatment |
| --- | --- | --- |
| `docs/handoffs/` and S0/S1 closeout/control records | Historical implementation and accepted tests, not current specifications. | Preserve history; add a new handoff and link the impact overlay. Do not relabel old runs as phases retroactively. |
| Completed EX-001/001B/002/007/008/009 reports, analyses, predictions and human decisions; P3 cohesive report and dated gate review | Immutable or dated evidence for the original mode/contract. | [Validity register](CONTINUOUS_SIMULATION_EVIDENCE_IMPACT.md) identifies non-transferable claims. New experiments/analyses get new IDs. |
| `DeepResearch_Treatement/` reviews/protocol/backlog and research source readings | Dated review or literature records; some historical status is intentionally old. | Consult current research plan and impact overlay; do not silently modernize the evidence being reviewed. |
| EX-007-specific prediction/input templates | Bound to EX-007's frozen launch-time question and schema family. | Create new EX-010 inputs after contract review; do not transform the EX-007 template into a different experiment. |
| `LOOSE_ENDS.md` | Existing P2-021 already tracks sequential continuation. This task is not a Loose Ends ledger-edit request. | Refer to the new canonical plan; update ledger status only through an authorized triage/recording pass. |
| `CUSTOM_REPORT_DASHBOARD_TOOLING_PLAN.md` | Future tooling design, no current importer implementation is changed. | Future importer must consume declared phase/window versions; add concrete fields when its implementation is scheduled. |
| `ART_PRODUCTION_SETUP_PLAN.md`, Fox/Rabbit art and other presentation production plans | “Phase summary” already describes a surface; art work does not own simulation resets. | Revalidate phase/final labels, transitions and screenshots when UI changes land. No asset recreation required for this review. |
| `CELLSIM_REMOTE_WORKER.md`, relay/operations docs | Process/license/host guidance remains applicable. | Amend job payload examples and supported continuation modes with CLI/worker migration. |
| Generic design templates, hunting/species idea scratchpads | Ecological/behavior “phases” and future mechanics are not reward-break resets. | Apply shared terminology when promoting a concrete feature; do not implement new mechanics in this refactor. |
| Unity standards/adoption, studio guidelines, collaboration, release process and retained island/Life/cave docs | These do not specify the species expedition loop; their restart/reset meanings are unrelated. | Preserve existing guidance and prototype separation. |

## Code-adjacent documentation deferred with implementation

Update these in the same package as the behavior they describe:

- XML comments and API names around manager/run/runner completion and restart;
- XAML “PLAY NEXT SIMULATION”/“next run” copy, button availability, selected
  upgrade summary, phase/final status and developer settings duration labels;
- CLI help and examples in `CellSim.ps1`, run scripts, submitters and workers;
- serializer field documentation, schema fixtures, JSON/CSV/Markdown headers,
  comparison rejection messages and independent-validator explanations;
- replay/visual-evidence instructions and capture test assumptions that currently
  wait for `Complete` before the first reward break;
- production and legacy upgrade descriptions where accepted semantics change;
- new baseline manifest/registry references, with old evidence kept immutable.

Changing those now would falsely document APIs or behavior that do not exist.

## Closure checks

- [x] CF-0 decisions and the versioned fresh legacy fixture are recorded in the
      canonical plan and evidence-impact notice.
- [ ] Every active design statement about the loop matches the accepted contract.
- [ ] All runtime/API/help/copy changes describe executed behavior, with no stale
      `Complete`/restart aliases at the reward boundary.
- [ ] Search `next run`, `next simulation`, `run-start`, `restart`, `reset`,
      `20 seconds`, `200 ticks`, `phase`, `resume`, and `upgrade` again across
      first-party docs/code/help; classify each remaining reference as current,
      explicit fresh research, historical, or unrelated.
- [ ] All report/template/validator versions and commands agree; current JSON,
      Markdown and CSV phase fields, direct Stat-Line export and generic
      schedule command are aligned, but EX-010-specific schedule parity still
      needs its final pass.
- [ ] Historical claim limits are reachable from active research/tooling indexes.
- [ ] Sim/Josh review and board/repository ownership reconciliation are recorded.
- [x] New handoff links actual tests, report/bundle evidence, and the branch
      baseline; no graphics screenshot was required for this code/report gate.

These implementation boxes remain open for EX-010-specific command, document,
and human-review closure; the runtime phase/checkpoint seam, generic schedule,
and direct validator parity now have focused tests and fresh Unity evidence.
