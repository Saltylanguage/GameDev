# Additive implementation backlog

These are bounded, reviewable changes extracted from the second-pass audit.
They are not authorization to alter production simulation behavior.

## P0 — protect the next prediction

### DR-001 — Generate bounded-input summaries

**Goal:** Remove hand-entered baseline values such as the EX-007 `RFS` error.

**Smallest implementation:** Read one `report.json`, emit only the fields
allowed by the contract, and fail if a checked-in summary disagrees.

**Acceptance:** A deliberately altered summary fails; the EX-007 baseline
reproduces `RFS=0.400402` and `APS=0.906114`.

### DR-002 — Version the metric dictionary

**Goal:** Prevent semantic mistakes such as treating `PREY` as food activity.

**Smallest implementation:** One Markdown/JSON dictionary with metric ID,
plain-language definition, unit, source field, aggregation, direction, validity
statuses, and limitations. Hash it into the prediction context.

**Acceptance:** A reviewer can interpret `PREY`, `RFS`, `pAVI`, and `APS`
without reading simulation code, and a changed definition changes the hash.

### DR-003 — Seal and record the AI context

**Goal:** Make “the AI saw only the allowed evidence” auditable.

**Smallest implementation:** Run confirmatory forecasts in a fresh task and
write a context manifest containing model/reasoning setting, prompt hash,
ordered input paths and SHA-256 hashes, invocation/time, and prediction hash.

**Acceptance:** A third party can reconstruct the permitted context and detect
any modified or extra input. Absence of intervention results is verifiable from
the manifest.

### DR-004 — Define forecast events and confidence ownership

**Goal:** Stop comparing one arm-level confidence with an arbitrary count of
correlated outcomes.

**Smallest implementation:** Require every probability to reference a stable
event ID with metric/composite definition, panel, threshold, and outcome rule.
Store Brier loss after resolution.

**Acceptance:** Every confidence value has exactly one resolvable event; the
same event cannot silently become multiple calibration cases.

### DR-005 — Register and retire seed panels

**Goal:** Preserve a genuinely blind path for future promotion.

**Smallest implementation:** Add an append-only seed registry with scenario,
range, role (`development`, `validation`, `blind-promotion`), disclosure time,
experiments used, and retirement status.

**Acceptance:** Seeds 1–20 are labelled development; 101–105 and 106–110 are
labelled disclosed/consumed; a new blind panel cannot overlap them.

### DR-006 — Complete the human P3 decision

**Goal:** Keep the AI from implicitly promoting its own evidence.

**Smallest implementation:** Human owner records Accept, Reject, Revise and
rerun, Inconclusive, or Archive for EX-007, with scope and review time.

**Acceptance:** The current P3 status points to a completed decision and states
what it does and does not authorize.

## P1 — make evidence portable and scoreable

### DR-007 — Unify local and worker artifact packaging

**Goal:** Make every valid run produce the same five-file bundle.

**Smallest implementation:** Reuse the worker's existing StatLine exporter from
the direct local path and run the same validators before success.

**Acceptance:** A direct `Run-CellularExperiment.ps1` invocation produces and
validates `report.json`, `report.csv`, `statline.csv`, `manifest.json`, and
`unity.log` without a manual follow-up.

### DR-008 — Complete run provenance

**Goal:** Make old evidence comparable after branches, packages, or tools
change.

**Smallest implementation:** Extend the manifest with branch, normalized Unity
version, package/project fingerprint, wrapper revision, host, and explicit
source-tree state. Keep AI provenance in the separate prediction record.

**Acceptance:** A reviewer can reproduce the run boundary and distinguish run
provenance from prediction provenance.

### DR-009 — Add machine-readable paired scoring

**Goal:** Make every human table rebuildable and keep small panels honest.

**Smallest implementation:** Emit one row per prediction event and seed with
baseline, intervention, delta, direction hit, threshold hit, band hit, and
unresolved reason. Aggregate mean, median, IQR, range, and sign rate by panel.

**Acceptance:** `AI_ANALYSIS.md` tables can be regenerated; five-seed panels
display all five paired deltas and are labelled transfer smoke tests.

### DR-010 — Separate primary, secondary, and exploratory endpoints

**Goal:** Prevent post-run telemetry selection from becoming the success rule.

**Smallest implementation:** Add endpoint family and `confirmatory`/
`exploratory` fields to experiment, prediction, report, and score templates.

**Acceptance:** One primary endpoint determines the main result, registered
secondary endpoints are visible, and all later observations stay exploratory.

### DR-011 — Publish a current P3 status note

**Goal:** Stop the historical gate snapshot from being mistaken for current
state.

**Smallest implementation:** Add a dated status page linking EX-007 through
EX-009, the pending human decision, consumed seed panels, and the revised
EX-009 interpretation. Leave old records untouched.

**Acceptance:** A new contributor can identify the current gate and next action
from one page.

## P2 — answer product questions proportionately

### DR-012 — Protect upgrade commutativity in code

**Goal:** Close the current EX-009 question at the layer where it originates.

**Smallest implementation:** Unit-test that applying `faster-movement` then
`crowding-tolerance`, or the reverse, yields identical `SpeciesRules` and
ruleset fingerprints. Optionally run one same-seed end-to-end smoke pair after
Unity preflight succeeds.

**Acceptance:** The unit test fails if either application becomes order
sensitive. Any retained runtime pair has identical run objects while distinct
provenance fingerprints preserve the requested order.

### DR-013 — Run a clean factorial interaction package when needed

For one supported pair, run B/A/C/AC on the same development seeds and
predefine the interaction contrast. Freeze the contract before opening a fresh
validation or blind-promotion panel. Do not reuse seeds 101–110 as blind.

### DR-014 — Start an append-only forecast registry

Store event IDs, probabilities, context hashes, held-out outcomes, Brier loss,
causal status, freshness, and human decisions. Do not fit a calibration model
until enough independent forecast events exist to justify one.

### DR-015 — Add spatial/time-window telemetry only for a named question

Candidate fields include occupancy concentration, movement entropy,
time-to-displacement, and late-window starvation. Each needs a metric
definition and a cost/usefulness review before adoption.

## Deliberately deferred

- Autonomous experiment selection.
- A generalized surrogate or metamodel.
- A dashboard that becomes a second source of truth.
- Continuous-range claims from one catalog value.
- Learned confidence calibration from the current tiny registry.
- Player-facing balance recommendations from simulation output alone.
