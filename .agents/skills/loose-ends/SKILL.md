---
name: loose-ends
description: Triage project organization and understanding gaps: unassigned work, informal plans, orphaned or misplaced documents and artifacts, forgotten decisions, risks, follow-ups, contradictions, and assistant uncertainty. Use for the LooseEnds skill, `/Loose Ends`, `Loose Ends`, or `Show me my Loose Ends` in the GameDev repository.
---

# Loose Ends

Run the project's Loose Ends review and return an evidence-backed, triaged report.

## Workflow

1. Read `LearningIndieDev/AGENTS.md` and `LearningIndieDev/docs/LOOSE_ENDS.md`.
2. Inspect the current repository state, relevant guidance, `docs/WORKING_STATE.md`, handoffs, plans, TODOs, recent decisions, implementation state, and relevant conversation context.
3. Identify:
   - unassigned work;
   - plans that remain informal or are not durably recorded;
   - orphaned or misplaced artifacts and documents;
   - decisions, risks, assumptions, or follow-ups that could be forgotten;
   - contradictions or uncertainty in the assistant's understanding.
4. For every finding, report priority (`P0`-`P2`), evidence/location, current status, recommended next action, likely owner, and confidence.
5. Distinguish new, resolved, and still-open items. Do not silently modify project files while reporting.
6. Update `LearningIndieDev/docs/LOOSE_ENDS.md` only when the user explicitly asks to record or resolve findings.

Keep the report concise enough to act on, but include the evidence needed to verify each finding.

## Resources (optional)

Create only the resource directories this skill actually needs. Delete this section if no resources are required.

### scripts/
Executable code (Python/Bash/etc.) that can be run directly to perform specific operations.

**Examples from other skills:**
- PDF skill: `fill_fillable_fields.py`, `extract_form_field_info.py` - utilities for PDF manipulation
- DOCX skill: `document.py`, `utilities.py` - Python modules for document processing

**Appropriate for:** Python scripts, shell scripts, or any executable code that performs automation, data processing, or specific operations.

**Note:** Scripts may be executed without loading into context, but can still be read by Codex for patching or environment adjustments.

### references/
Documentation and reference material intended to be loaded into context to inform Codex's process and thinking.

**Examples from other skills:**
- Product management: `communication.md`, `context_building.md` - detailed workflow guides
- BigQuery: API reference documentation and query examples
- Finance: Schema documentation, company policies

**Appropriate for:** In-depth documentation, API references, database schemas, comprehensive guides, or any detailed information that Codex should reference while working.

### assets/
Files not intended to be loaded into context, but rather used within the output Codex produces.

**Examples from other skills:**
- Brand styling: PowerPoint template files (.pptx), logo files
- Frontend builder: HTML/React boilerplate project directories
- Typography: Font files (.ttf, .woff2)

**Appropriate for:** Templates, boilerplate code, document templates, images, icons, fonts, or any files meant to be copied or used in the final output.

---

**Not every skill requires all three types of resources.**
