# Correctness First Engineering Project Skill

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Owner: Codex
- Branch: codex/cellular-sprite-tiling
- Baseline commit: d0fb092c
- Date: 2026-08-21

## Summary

Added the pruned Correctness-First Engineering safety harness as an active
repository skill. Other developers receive it with the project and can invoke
it as `$correctness-first-engineering`; Codex may also select it automatically
when its narrow uncertainty, failure, or correctness-review trigger applies.

## Changes

- Added `.agents/skills/correctness-first-engineering/SKILL.md`.
- Added `agents/openai.yaml` with the user-facing name
  `Skill: Correctness-First Engineering`.
- Added the skill to the active inventory in
  `docs/AI_WORKFLOW_SKILLS_PLAN.md` and clarified that safety skills supplement
  rather than replace project and domain guidance.
- Included the previously requested Trello push reminder in
  `docs/COLLABORATION_WORKFLOW.md` in this shared batch.

## Decisions and assumptions

- The internal ID remains `correctness-first-engineering` because skill IDs use
  lowercase hyphen-case; the requested title is the display name and document
  heading.
- Implicit invocation remains enabled by default, but the description excludes
  routine low-risk edits to avoid loading the harness everywhere.
- The skill governs reasoning quality only. Existing project guidance continues
  to govern permissions, Unity-specific rules, product intent, and domain work.

## Validation

- Equivalent structural validation passed: valid frontmatter keys and naming,
  349-character description, no unfinished placeholders, 48-character UI short
  description, exact display name, and a default prompt that names the skill.
- `git diff --check` passed for the skill and related documentation; Git emitted
  only the repository's normal LF-to-CRLF warnings.
- Manual representative-prompt routing review:
  - repeated failed Unity fix: should invoke;
  - risky migration or conflicting evidence: should invoke;
  - routine label rename or other low-risk edit: should not invoke unless asked.
- The bundled `quick_validate.py` could not execute because its bundled Python
  environment lacks the `yaml` module. No dependency was installed; its visible
  structural checks were reproduced locally instead.

## Risks and incomplete work

- Skill discovery is read at session startup, so an already-running Codex
  session may need to be restarted or refreshed before the new skill appears.
- Real usage should be watched for over-triggering or unnecessary clarification;
  refine the description from observed behavior rather than adding speculative
  rules now.

## Next useful step

Pull the branch in a fresh Codex session, confirm the skill appears beside
LooseEnds, and use it on the next genuinely uncertain or failed technical task.
