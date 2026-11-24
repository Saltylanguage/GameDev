# M2 vertical slice meeting tracks

[Working state](../WORKING_STATE.md) | Status: planned

- Handoff schema: 1
- Handoff ID: 2026-11-17-0741-codex-m2-vertical-slice-meeting-tracks
- Owner: Codex
- Branch: codex/m2-species-scenario-select
- Baseline commit: 971b81bc
- Date: 2026-11-17
- Supersedes: none

## Summary

Josh's November 16 meeting direction, recorded November 17: finish a technical
Track A before returning to balance work. Josh will help Sim with Track A.
Track B, visual polish and player feedback across UI, VFX and audio, is deferred
until later. This is a scoped work inventory, not M2 acceptance or a change to
any production balance value. The current ten-outcome M2 gate needs deliberate
reconciliation with the newly requested species and terrain work.

## Changes

- Created focused Trello cards for new Track A work, reused the existing water
  card, and created one deferred all-screen UI card. Existing simulation-event,
  Field Report and audio cards remain the delivery homes for Track B feedback.
- Recorded the direction in the roadmap, project context and working-state
  index. This handoff changes no gameplay code or authored balance asset.

## Decisions and assumptions

Track A is technical preparation before balance work. The numbered list is
Josh's inventory, not an approved implementation order within the track.

| # | Work | Trello home |
| --- | --- | --- |
| A1 | Wrapping integration and map-edge rule | [VS-A1](https://trello.com/c/wsmjVY20); the original [wrapping ticket](https://trello.com/c/tkOVt9XS) remains Done. |
| A2 | Water: ponds and rivers | Existing [WTR-01](https://trello.com/c/AhPFXox7), In Progress. |
| A3 | ScriptableObject Trinket skeleton and verifiable dummy assets | [VS-A3](https://trello.com/c/wf9cwfhD). |
| A4 | Mutation cards | [VS-A4](https://trello.com/c/IKjreLWW); decide which part belongs with later UI polish. |
| A5 | Terrain Upgrade system: burrows, berries, trees, improved mushrooms and further additions | [VS-A5](https://trello.com/c/5QZIUMoD); distinct from the completed Hare [burrowing feature](https://trello.com/c/DQwFKLSF). |
| A6 | Shift toward cozier, individually legible creatures instead of large anonymous specimen populations | [VS-A6](https://trello.com/c/YVqjvxYn); establish a measurable prototype before choosing counts. |
| A7 | Bring other playable species in early so their mechanics can be tested before balance | [VS-A7](https://trello.com/c/IiCOUGOz); choose roster/order before splitting species work. |

Track B is deferred while Josh helps Sim with Track A. The current reward
screen prototype is the polish benchmark for all screens. [VS-B1](https://trello.com/c/3QjBofCv)
tracks the all-screen UI treatment. Important-event icons and board cues belong
with [VF-01](https://trello.com/c/RWRw4cjb); a visually clear event log and
Field Report need coordination across [VF-02](https://trello.com/c/AVJ0KXrJ),
[M2-08](https://trello.com/c/DPPvzn9u) and VS-B1; event and reward audio cues
belong with [M2-06](https://trello.com/c/72NTdlpm). The [M2-07 feedback card](https://trello.com/c/bqa1KUTK)
continues to own aggregation and rate limits. These are related homes, not
duplicate delivery commitments.

Open decisions for Josh and Sim:

- The completed wrapping ticket says all maps wrap; WTR-01 says islands are
  bounded. Resolve the rule for water and shape maps before integration.
- The current M2 outcome contract is Hare-first and puts broader species
  content in M3. Agree which additional species, if any, become required for
  M2 and revise the milestone gate explicitly.
- Decide the technical versus later UI-polish split for Mutation cards, the
  first representative Terrain Upgrade, the Trinket player-flow contract,
  and what counts as a successful creature-scale prototype.
- Owners, reviewers, estimates, timing and acceptance details are still to be
  assigned. No ticket here approves a Fox count, other balance value, or M2
  closeout.

## Validation

- Read the existing Trello wrapping, water, species, UI, event-feedback and
  audio cards before mapping the new work; new cards were created on the game
  board. No Unity test or manual gameplay validation was run for this planning
  update.
- The code correction branch remains separate from the pinned `fda77319`
  independent-review baseline. Its earlier focused tests and remaining manual
  checks are in the [selection/audio handoff](2026-11-16-1939-codex-m2-selection-audio-listener.md).

## Risks and incomplete work

- The new Track A scope is larger than the existing ten-row M2 gate. Do not
  infer that additional species, Trinkets or terrain upgrades are already
  accepted or that the old gate has been closed.
- Forest Edge route survival, replay motivation, permanent-unlock validation
  and corrected-branch human listening/visual review remain open. Josh's
  independent questionnaire response stays private until Sim's independent
  response is complete and the two are compared.
- Wrapping and bounded-island rules conflict in current Trello descriptions;
  implementation must wait for the explicit topology decision.

## Next useful step

Josh and Sim compare their independent M2 responses, then agree the Track A
topology, roster, first proof items and acceptance criteria. Assign owners and
estimates, update the formal M2 gate and sprint control, and schedule Track B
after Track A's technical foundation is usable. Keep balance experiments and
human M2 decisions separate from this work inventory.
