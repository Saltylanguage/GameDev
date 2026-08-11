# Next architecture batch

This note prepares the next higher-risk work without activating speculative
frameworks before their behavior is defined.

## CS-04 - Custom rule logic

### Candidate activation

The first likely trigger is **alpha offspring** or **vision/sight**. Both need
behavior that is more than a scalar value or a fixed built-in simulation stage:

- Alpha offspring needs conditional child qualification and state/stat changes.
- Vision needs perception, target selection, and a deterministic movement choice.

### Required decisions before implementation

1. Which mechanic is the first real consumer.
2. Whether the custom behavior runs before or after the built-in stages.
3. What state it may read and write.
4. How it receives deterministic randomness.
5. How the same rule is tested in isolation and in a full run.

### Guardrail

Start with one explicit rule-stage seam for the chosen mechanic. Do not add a
universal event bus, delegate registry, scripting language, or plugin system.

## CS-06 - Data asset/editor authoring

### Candidate direction

When reusable scenarios become a real workflow, use a read-only Unity
`ScriptableObject` definition asset that converts into the existing immutable
`CellularSimData` snapshot at run start.

Keep these boundaries separate:

```text
CellularSimDataAsset (serialized authoring definition)
    -> validation and conversion
CellularSimData (immutable runtime snapshot + fingerprint)
    -> SimulationRunState (mutable run state)
```

### Required decisions before implementation

1. Which scenario fields designers must author in the Inspector.
2. How species IDs, terrain IDs, and grid-pattern offsets are serialized.
3. Whether asset values replace or layer over code defaults.
4. How invalid references and duplicate IDs are reported in the Editor.
5. How asset changes affect fingerprints and A/B comparison records.

### Guardrail

Do not expose mutable runtime dictionaries through the asset or use the asset as
global run state. Preserve the current code-authored path until a scenario asset
is actually needed by a designer or repeatable experiment workflow.

## Current status

- Ruleset fingerprints and prototype audit are pushed in `97b77bc9`.
- CS-07 audit found no safe deletion candidate.
- CS-04 and CS-06 remain intentionally unimplemented until one candidate
  mechanic and one authoring workflow are selected.
