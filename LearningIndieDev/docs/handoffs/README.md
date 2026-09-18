# Handoff journal

This directory contains one concise Markdown note per independently reviewable
piece of work. It replaces a single growing change log and lets contributors add
context on separate branches with fewer merge conflicts.

Create a note from the repository's Unity project directory:

```powershell
.\tools\New-Handoff.cmd -Owner "your-name" -Topic "short feature name"
```

Files use `YYYY-MM-DD-HHmm-owner-topic.md`, so ordinary filename sorting puts them
in chronological order. Every note records its originating branch and commit and
links back to [`WORKING_STATE.md`](../WORKING_STATE.md).

New notes use handoff schema 1. Their handoff ID matches the filename without
`.md`, and their metadata includes status, owner, branch, baseline commit, date,
and `Supersedes`. Leave `Supersedes: none` when the note does not replace an
earlier handoff. Otherwise, list the earlier handoff filename relative to this
directory.

Validate new metadata, local links, and artifact references with:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\Test-Handoffs.ps1
```

Historical notes remain valid without schema 1 metadata. The validator checks
their local Markdown links but does not require old, machine-local generated
artifacts to remain in this checkout. Schema-1 notes are the current handoff
format, so their artifact references are checked for local availability. Add
`-ShowWarnings` when the individual warning list is needed.

Keep notes short and evidence-based. Include:

- What changed and why.
- Decisions or assumptions future work must respect.
- Validation that actually ran.
- Known risks, incomplete work, and likely integration conflicts.
- The next useful step.

Do not paste chat transcripts, duplicate full diffs, or rewrite old shared notes
when later work changes the conclusion. Add a newer note that supersedes the old
one and link to it when useful.
