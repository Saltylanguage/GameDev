# M2 independent survey handoff

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-11-16-1158-codex-m2-independent-survey-handoff
- Owner: Codex
- Branch: `codex/s6-predator-first`
- Baseline commit: `71c0c7b7`
- Date: 2026-11-16
- Supersedes: none

## Summary

Josh and Sim can run the same M2 manual questionnaire independently on the same gameplay baseline, preserve separate answers, and compare them by question ID afterward. This is a review procedure, not a playtest result or M2 acceptance.

## Changes

- Added the [Q0–Q23 questionnaire](../M2_MANUAL_REVIEW_QUESTIONNAIRE.md), covering the ten M2 outcome rows plus a separate M2-04 factual-report accuracy question.
- Added the [independent review packet](../M2_INDEPENDENT_REVIEW_PACKET.md) with the target branch/gameplay revision, comparable route and display setup, exact optional Unity verification commands, the known authored-route failure, and the side-by-side comparison method.
- Added a [blank response template](../M2_MANUAL_REVIEW_RESPONSE_TEMPLATE.md). Each reviewer keeps a separate copy; blank fields mean unanswered, not passed.
- Indexed the packet from `docs/WORKING_STATE.md`.

## Decisions and assumptions

- The target gameplay baseline is the already pushed `71c0c7b7`. This packet changes documentation only; each reviewer records the exact checkout or build actually tested. A later gameplay change requires aligning both reviewers on the new revision before comparing affected answers.
- Josh and Sim should not see each other's answers before both finish. Differences are classified as observation, judgment, or setup/blocked differences before any product decision.
- Both are project insiders. Their review does not replace M2-10's separate first-time external-player evidence. Josh remains the factual-report and milestone decision owner; Sim's Q23 answer is independent review input.

## Validation

- The questionnaire has contiguous Q0–Q23 identifiers and includes M2-01 through M2-10. The response template lists the same IDs.
- Local links to the roadmap, current M2 handoff, Chrono art set, and M2-04 factual report resolve in this checkout. Documentation whitespace was checked. No Unity test or manual player review was run for this documentation change.

## Risks and incomplete work

- No human survey answers exist yet. The authored Hare route still blocks genuine full-journey, strategy, and results acceptance if it ends early; the procedure records those as `Blocked` and continues with reachable checks.
- The 300 MB raw M2-04 experiment bundles and visual screenshots are ignored local artifacts. Sim can read the committed factual report and seed metrics, or rerun the documented checks on his machine.
- Separate insider judgments can expose disagreement, but cannot by themselves close the external-player gate or approve new ecology values.

## Next useful step

Share the pushed branch and packet with Sim. Josh and Sim each answer privately, then compare their responses and record explicit decisions and follow-up work. Do not fill in missing answers from automated screenshots or infer acceptance from aligned Yes answers.
