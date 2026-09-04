# Concern record template

Use this only when the named work block has no existing authoritative concern
section or record. Replace placeholders and remove empty optional fields.

```markdown
# Planning concerns — <feature or work block>

**Scope:** <what this record covers and excludes>
**Canonical plan:** <path or link>
**Human owner:** <name, role, or Unknown>
**Status:** Active | Closed

## Active concerns

### <work-id>-C01 — <short conversational title>

- **Severity:** Mild | Extreme
- **Status:** Open | Acknowledged | Mitigated | Accepted Risk
- **Trigger:** <specific observable condition or proposed action>
- **Why it matters:** <concrete consequence>
- **Evidence:** <decision, file, code, test, report, or user direction>
- **Smallest mitigation:** <bounded action or decision>
- **Owner:** <person or role, if known>
- **Recorded:** <date and source revision when useful>
- **Waiver:** <scope, decision owner, and date; omit when none>

## Closed concerns

### <work-id>-C00 — <title>

- **Severity:** Mild | Extreme
- **Status:** Resolved | Superseded
- **Resolution:** <what changed and supporting evidence>
- **Closed:** <date and decision owner>
```

Keep the active section first. Move a concern to the closed section instead of
deleting it. One sentence per field is usually enough.
