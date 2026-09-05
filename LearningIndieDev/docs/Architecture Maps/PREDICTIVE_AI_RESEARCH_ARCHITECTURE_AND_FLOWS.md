# Predictive AI research architecture and flows

> Status: Needs Review  
> Last reviewed: 2026-09-04
> Scope: Program roadmap, experiment operation, evidence lineage, and experiment selection

These diagrams summarize the current Predictive AI research program and the
second-pass research treatment. They do not promote an AI interpretation into
an accepted project decision. The research plan, immutable run evidence, and
recorded human decisions remain authoritative.

## 1. Program roadmap

```mermaid
flowchart LR
    P0["P0 — Frame<br/>Protocol, IDs, ownership<br/>EX-001 brief"]
    P1["P1 — Trust the instrument<br/>Determinism, fingerprints, replay<br/>EX-001 · EX-001B"]
    P2["P2 — Diagnose outcomes<br/>Causal interventions<br/>EX-002"]
    P3["P3 — Bound AI discovery<br/>Predictions, uncertainty, validation<br/>EX-003 · EX-007 · EX-008 · EX-009 · EX-010"]
    P4["P4 — Translate to design<br/>Explanations, upgrades, events<br/>EX-004 · EX-005 · EX-006"]
    P5["P5 — Validate collaboration<br/>More scenarios and contributors"]
    P6["P6 — Promotion decision<br/>Product · tooling · studio process"]

    P0 --> P1 --> P2 --> P3 --> P4 --> P5 --> P6

    NOW["Current gate<br/>EX-007 evidence complete<br/>Human decision pending<br/>EX-003 unresolved"]
    ORDER["EX-009 accepted for launch-time pair<br/>A/B matched for current additive pair<br/>EX-010 tracks mid-run continuation"]

    NOW -.-> P3
    ORDER -.-> P3
```

## 2. End-to-end operating loop

```mermaid
sequenceDiagram
    participant H as Human owner
    participant C as Experiment contract
    participant A as AI
    participant T as Unity / CellSim tooling
    participant E as Evidence system

    H->>C: Define question, scope, endpoints and decision rule
    C->>E: Register metrics, seed roles and provenance requirements
    E->>A: Supply sealed baseline and permitted context
    A-->>E: Record prediction, limits and defined confidence events
    H->>T: Approve the intervention and execution

    T->>T: Run Unity preflight

    alt Preflight fails
        T-->>E: Store failure record
        E-->>H: Report blocker; produce no experimental result
    else Preflight passes
        T->>T: Execute matched-seed arms
        T-->>E: Store immutable run bundles
        E->>E: Validate, normalize and score
        E->>A: Provide factual report and resolved forecast events
        A-->>H: Analysis, uncertainty, misses and possible causes
        H->>E: Accept, reject, revise, archive or mark inconclusive
        E-->>C: Accepted knowledge or next bounded experiment
    end
```

## 3. Evidence and data structure

```mermaid
flowchart TB
    subgraph Definition["Human-owned definition"]
        PLAN["Canonical research plan"]
        CONTRACT["Experiment contract"]
        METRICS["Versioned metric dictionary"]
        SEEDS["Seed-panel registry"]
        PLAN --> CONTRACT
        METRICS --> CONTRACT
        SEEDS --> CONTRACT
    end

    subgraph Prediction["Pre-run prediction"]
        CONTEXT["Sealed context manifest<br/>Prompt · model · file hashes"]
        PRED["Immutable prediction<br/>Direction · effect · event probability<br/>uncertainty · limits"]
        CONTRACT --> CONTEXT --> PRED
    end

    subgraph Raw["Immutable run evidence"]
        ARMS["Experiment arms"]
        RUNS["Seeded runs"]
        BUNDLE["Artifact bundle<br/>report.json · report.csv<br/>statline.csv · manifest.json · unity.log"]
        CONTRACT --> ARMS --> RUNS --> BUNDLE
    end

    subgraph Derived["Rebuildable derived evidence"]
        REPORT["Factual report"]
        NORMAL["Normalized metrics<br/>Events · fingerprints · paired deltas"]
        SCORE["Forecast-event scores"]
        ANALYSIS["Versioned analysis<br/>v1 · v2 · future revisions"]

        BUNDLE --> REPORT
        BUNDLE --> NORMAL
        PRED --> SCORE
        NORMAL --> SCORE
        REPORT --> ANALYSIS
        SCORE --> ANALYSIS
    end

    subgraph Governance["Human-controlled knowledge"]
        DECISION["Human decision<br/>Accept · Reject · Revise<br/>Inconclusive · Archive"]
        REGISTRY["Prediction and impact registry<br/>Range · causal status · calibration<br/>freshness · lineage"]
        NEXT["Next bounded experiment"]

        ANALYSIS --> DECISION
        DECISION --> REGISTRY
        DECISION --> NEXT
        NEXT --> CONTRACT
    end
```

## 4. Experiment selection and seed governance

```mermaid
flowchart LR
    Q{"What is the question?"}

    Q -->|"One changed variable"| SINGLE["B vs A<br/>Same seeds"]
    Q -->|"Two-variable interaction"| FACTORIAL["B · A · C · AC<br/>Predefine interaction contrast"]
    Q -->|"Application order"| SENSITIVE{"Can application<br/>be order-sensitive?"}

    SENSITIVE -->|"No: separate additive fields"| UNIT["Commutativity unit test<br/>Optional runtime smoke pair"]
    SENSITIVE -->|"Yes: shared state, caps,<br/>multipliers or side effects"| ORDER["AB vs BA<br/>Same seeds"]

    SINGLE --> DEV
    FACTORIAL --> DEV
    ORDER --> DEV

    DEV["Development panel<br/>Reusable exploration"]
    FREEZE["Freeze contract<br/>and sealed forecast"]
    VALIDATE["Validation panel<br/>Limited disclosure"]
    REVIEW["Human review"]
    BLIND["Blind-promotion panel<br/>One-time use"]
    DECIDE["Bounded decision"]
    CONSUMED["Disclosed / consumed<br/>Reusable as development evidence<br/>Never blind again"]

    DEV --> FREEZE --> VALIDATE --> REVIEW
    REVIEW -->|"Evidence sufficient"| DECIDE
    REVIEW -->|"Promotion requires stronger evidence"| BLIND --> DECIDE

    VALIDATE -.->|"After disclosure"| CONSUMED
    BLIND -.->|"After disclosure"| CONSUMED
```

## Authoritative sources

- [Predictive AI research plan](../Research/AI_ASSISTED_ECOLOGY_LAB_RESEARCH_PLAN.md)
- [Predictive AI research paper](../Research/AI_ASSISTED_ECOLOGY_LAB_RESEARCH_PAPER.md)
- [Deep-research treatment index](../DeepResearch_Treatement/README.md)
- [Second-pass delta](../DeepResearch_Treatement/SECOND_PASS_DELTA.md)
- [Revised research protocol](../DeepResearch_Treatement/REVISED_PROTOCOL.md)
- [AI-generated reports guideline](../Studio%20Guidelines/AI_GENERATED_REPORTS.md)
