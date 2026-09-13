---
name: dirtyboy
description: "Capture a local Git/Unity project context snapshot and gently flag missed cleanup, unverified changes, and process shortcuts before or after a work block."
metadata:
  short-description: "Context snapshot with a gentle process check"
---

# DirtyBoy

Use DirtyBoy at the start and end of a feature block, before a handoff, or when
context has become uncertain. It is read-only by default and produces a compact
project snapshot plus evidence-backed “finger wag” findings.

Run from the repository root:

```powershell
powershell -File .agents/skills/dirtyboy/scripts/dirtyboy.ps1
```

Persist a snapshot only when it will be useful for a handoff:

```powershell
powershell -File .agents/skills/dirtyboy/scripts/dirtyboy.ps1 `
  -OutputPath LearningIndieDev/artifacts/dirtyboy-context-latest.md
```

The script reports:

- current branch, commit, staged/unstaged/untracked files, and diff hygiene;
- Unity project roots, project structure, relevant guidance documents, and
  available test/experiment commands;
- conflict markers, changed files with TODO/FIXME/HACK/TEMP markers, oversized
  generated-looking changes, missing tests or documentation around code edits,
  and stale or missing test evidence when it can prove those conditions.

The tone is a gentle correction, not a blocker. Report facts first, then name
the smallest cleanup or verification action. Do not scold clean work, invent a
violation from missing evidence, or make edits to fix findings automatically.
DirtyBoy may write only the explicit `-OutputPath`; it must never stage, commit,
reset, delete, push, or modify project files.

Use the companion `$artifact-summarization` skill when Unity reports are large;
DirtyBoy should point to the summarizer rather than loading raw JSON into the
conversation.
