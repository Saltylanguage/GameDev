# Handoff journal

This directory contains one concise Markdown note per independently reviewable
piece of work. It replaces a single growing change log and lets contributors add
context on separate branches with fewer merge conflicts.

Create a note from the repository's Unity project directory:

```powershell
.\tools\New-Handoff.ps1 -Owner "your-name" -Topic "short feature name"
```

Files use `YYYY-MM-DD-HHmm-owner-topic.md`, so ordinary filename sorting puts them
in chronological order. Every note records its originating branch and commit and
links back to [`WORKING_STATE.md`](../WORKING_STATE.md).

Keep notes short and evidence-based. Include:

- What changed and why.
- Decisions or assumptions future work must respect.
- Validation that actually ran.
- Known risks, incomplete work, and likely integration conflicts.
- The next useful step.

Do not paste chat transcripts, duplicate full diffs, or rewrite old shared notes
when later work changes the conclusion. Add a newer note that supersedes the old
one and link to it when useful.
