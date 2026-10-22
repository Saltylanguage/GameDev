# Rabbit Genome planning handoff for Luna

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-10-21-1720-codex-rabbit-genome-implementation-plan
- Owner: codex
- Branch: codex/audio-direction-and-sourcing
- Baseline commit: 6ffb7b10 plus the existing uncommitted work
- Date: 2026-10-21
- Supersedes: none

## Request and outcome

Josh requested planning before switching to Luna to conserve implementation
budget. [The remaining-work plan](../RABBIT_GENOME_IMPLEMENTATION_PLAN.md)
provides bounded packets, code entry points, acceptance checks and explicit
design gates. Start with RG-00 and RG-01 after Josh asks to implement them.
New mechanic values remain recommendations awaiting design acceptance.

The plan preserves all 27 candidate slots (8 Trailblazer / 10 Warren / 9 Gardener)
and explains the completed POC/stat foundation. The old prerequisite checklist
now reflects that foundation, population work and current delivery gaps.

## Two source-confirmed gaps

1. PopulationIntervention.cs is a batch SharedSource but is absent from
   PortableSourceFiles in CellSimPortable.cs. RG-00 adds provenance coverage and
   a focused build-input/manifest/hash regression check before further frozen runs.
2. CanUsePopulationControl and Noesis visibility only allow PhaseDecision.
   The original FD-T06 also requires nonterminal round-6 purchases before shops.
   RG-01 extends the prototype cycle-result window while preserving settled
   population/income, the pool, atomic charging and terminal rejection.

The current prototype encounters do not establish completion of the shop/full
Field Data journey. Integration should reuse the operative FD-T04 session and
FD-T09/12/13 work when available; the plan forbids a competing session/economy.

## Changed and checked

Documentation only: the implementation plan, readiness checklist, WORKING_STATE
entry and this handoff. No gameplay code/assets, Trello cards or balance values
changed, and no tests/experiments ran. Plan/checklist relative links and Markdown
fences were checked. Previous runtime verification remains in the October 21
population-decisions handoff; it is not new evidence from this planning task.

## Suggested next user prompt

> Read docs/RABBIT_GENOME_IMPLEMENTATION_PLAN.md and implement RG-00, then RG-01
> only. Preserve the existing local work, follow the packet acceptance checks,
> use focused tests and final affected UI captures, and report back before RG-02.
