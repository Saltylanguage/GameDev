# AI workflow value ledger

This ledger measures whether AI-assisted work helps the project in practice.
It records time, correction, rework, verification, cost visibility, and human
review for a small sample of real tasks.

This is an observational studio record. It is not a formal P5 experiment, does
not compare against a human-only control, and cannot establish that AI caused an
outcome. A future research phase needs its own human-approved contract.

## What this unlocks

After 10–20 representative tasks, the team should be able to answer:

- Which kinds of work benefit most from AI assistance?
- Where does AI move effort into review, correction, or reruns?
- Which task risks or model choices are associated with better outcomes?
- Which parts of the workflow should be kept, simplified, or stopped?

## Files

- [`workflow-value-ledger-v1.jsonl`](workflow-value-ledger-v1.jsonl) is the
  machine-readable record. Each line is one task.
- [`HUMAN_SUMMARY.md`](HUMAN_SUMMARY.md) is the decision-facing summary. It
  remains pending until the minimum sample exists and a human reviews it.

## Recording rules

1. Start a record before material task work when practical.
2. Use one record for one independently reviewable outcome.
3. Record actual measurements. Use `null` and explain why when a value is not
   available; do not reconstruct timing, cost, or review effort from memory.
4. Close the record only after the task's checks have run or been reported as
   unavailable.
5. Do not edit a closed record to improve the story. Add a correction record
   that names the original `task_id`.
6. Human usefulness is optional until a reviewer supplies it. AI must not fill
   in a human rating.
7. Setup, training, and ledger-maintenance work may be recorded but should use
   `included_in_sample: false` unless the human explicitly wants it analyzed.

## Version 1 record

Each JSON object contains:

| Field | Meaning |
| --- | --- |
| `task_id`, `title`, `task_type` | Stable identity and kind of work. |
| `included_in_sample` | Whether the task counts toward the 10–20 task review. |
| `risk` | Green, yellow, or red using SG-002. |
| `routing` | Model and reasoning effort actually used, or `not exposed`. |
| `timing` | Start, first useful evidence, completion, human review, and measurement quality. |
| `friction` | Corrections, rework cycles, manual steps, automated runs, reruns, and unsupported claims caught. |
| `outcome` | Status, result, checks, and evidence paths. |
| `cost` | Approximate usage cost when exposed by the tool or account. |
| `human_review` | Pending or completed review and an optional 1–5 usefulness rating. |
| `notes` | Material limitations or context not captured above. |

`first_useful_evidence_minutes` measures time from task start to the first result
that materially reduced uncertainty. `total_elapsed_minutes` ends when the
outcome is ready for review, not when a later human review occurs.

## Sample review gate

Review the ledger after 10–20 included tasks spanning more than one kind of
work. The human summary must state which records were included, where data is
missing, and which conclusions are observations rather than causal claims.

The first review should decide only whether to keep, simplify, revise, or stop
parts of the workflow. It does not authorize a general claim that AI improves
game development.
