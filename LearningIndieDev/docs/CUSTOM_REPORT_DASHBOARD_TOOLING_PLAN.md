# Custom report dashboard developer-tooling plan

## Status

**Proposed developer tooling; not approved implementation work.**

This plan explores a workflow in which the game exports factual report data,
users design dashboards outside the game in a spreadsheet application, and a
developer tool imports the authored workbook into a constrained Noesis/XAML
dashboard that can display compatible game reports.

The central proposal is an **XLSX-to-Noesis dashboard compiler**, not a generic
CSV-to-XAML converter. CSV remains useful as a portable data export, but it does
not contain the presentation information needed to reconstruct a dashboard.

## High-level goal

Enable players, designers, researchers, and community tool authors to create
custom report dashboards using familiar spreadsheet software without requiring
them to edit Unity scenes, write C#, or author Noesis XAML directly.

The intended workflow is:

```text
Game report data
    -> generated XLSX workbook with stable named data ranges
    -> user-authored Dashboard sheet in Excel or compatible software
    -> validated DashboardDefinition
    -> generated, constrained Noesis XAML
    -> reusable in-game dashboard bound to compatible report data
```

The report remains the factual source. The imported dashboard controls only how
that data is arranged and styled; it must not rewrite report evidence or alter
simulation results.

## Why the current CSV output is not enough

The current experiment CSV is deliberately a flat, Excel-ready data export. It
contains one row per seed with run metadata and final population columns. That
is useful for filtering, formulas, external charts, and independent analysis.

CSV does not preserve the information needed to reverse-engineer a visual tree:

- charts or chart configuration;
- fonts, colors, fills, borders, and alignment;
- merged cells and dashboard regions;
- column widths and row heights;
- images and icons;
- conditional formatting;
- named ranges and stable semantic bindings;
- multiple worksheets;
- the distinction between a metric card, table, legend, or chart;
- reusable bindings that can display a future report rather than one set of
  exported values.

Once an Excel or Google Sheets dashboard is exported as CSV, those presentation
semantics are gone. No converter can reconstruct them reliably because the
information is absent rather than merely encoded differently.

CSV should therefore remain a supported **data interchange format**, while XLSX
is evaluated as the **dashboard authoring format**.

## What this work would enable

### External visual authoring

Users could arrange report metrics, tables, and supported charts in Excel or a
compatible spreadsheet tool and preview the information hierarchy before
importing it into the game.

### Reusable dashboard templates

A dashboard would bind to stable report semantics such as final population,
population history, species activity, or mortality causes. The same dashboard
could then display another report using the same data contract instead of
embedding values from one run.

### Community-created presentation

Players and researchers could share dashboard templates independently from
simulation data. Different dashboards could emphasize balance, population
history, mortality, reproduction, or individual-species behavior without a new
hard-coded game screen for each use case.

### Faster iteration for developers and designers

The team could prototype report presentation in a spreadsheet before committing
to a permanent in-game screen. Useful community patterns could later inform
first-party dashboards.

### Safe separation of evidence and presentation

The factual report remains immutable. A dashboard definition determines layout,
style, and bindings only. This follows the studio requirement to keep generated
evidence separate from interpretation and human presentation choices.

## Proposed workbook contract

The game or an editor tool would export an `.xlsx` workbook with generated data
sheets and one user-owned presentation sheet:

```text
ReportMetadata
RunSummary
PopulationHistory
SpeciesActivity
DeathEvents
Dashboard
```

Generated sheets would expose versioned named ranges or tables such as:

```text
Report_FinalPopulation
Report_TotalDeaths
Report_DurationSeconds
Report_PopulationHistory
Report_SpeciesActivity
Report_DeathsByCause
```

The `Dashboard` sheet would use ordinary cell layout, formatting, formulas that
reference approved names, and a supported subset of spreadsheet charts. Named
ranges provide the semantic bridge between workbook elements and stable runtime
bindings.

For example, a cell referencing `Report_FinalPopulation` could compile to a
metric binding, while a line chart sourcing `Report_PopulationHistory` could
compile to an approved Noesis chart control.

## Required intermediate model

The workbook parser should not generate XAML directly. It should first create a
versioned, UI-independent model:

```text
DashboardDefinition
  - schema version and source workbook metadata
  - grid rows and columns
  - text and metric elements
  - report-data bindings
  - tables and columns
  - supported charts and series
  - style tokens
  - import warnings and unsupported features
```

The import pipeline becomes:

```text
XLSX
    -> untrusted workbook reader
    -> contract and limit validation
    -> DashboardDefinition
    -> deterministic XAML generator
    -> Noesis import/compile validation
```

Keeping `DashboardDefinition` between the workbook and XAML provides a stable
place for validation, schema migration, testing, previews, and future authoring
formats.

## Initial spreadsheet-to-Noesis mapping

The proof of concept should intentionally support a small subset:

| Spreadsheet feature | Proposed Noesis representation |
| --- | --- |
| Cell or merged region | `Border` containing a `TextBlock` |
| Rows and columns | XAML `Grid` definitions |
| Fill, border, and alignment | Whitelisted `Border` and text properties |
| Static label | `TextBlock` literal text |
| Approved named-range reference | View-model binding |
| Rectangular table | Dashboard table or `ItemsControl` |
| Line chart | Approved `DashboardLineChart` control |
| Bar chart | Approved `DashboardBarChart` control |
| Number format | Validated display-format token |

Metric cards can be represented initially as styled merged regions containing a
label and one approved report binding. Pie charts, images, conditional
formatting, and more complex elements should wait until the basic contract is
proven.

## Safety and validation boundaries

Imported workbooks are untrusted content. The tool must never execute macros,
external links, arbitrary formulas, code, or arbitrary XAML.

The compiler should enforce:

- a whitelist of supported workbook elements and generated Noesis controls;
- bindings only to versioned report-contract keys;
- limits on sheets, cells, visual elements, charts, series, and data points;
- approved fonts, colors, images, number formats, and style properties;
- deterministic element ordering and XAML output;
- actionable workbook sheet/cell diagnostics;
- rejection or explicit warnings for unsupported features;
- no external file, URI, network, macro, or executable-content resolution;
- Noesis import and target-platform validation before a dashboard is accepted.

Generated XAML is derived output. The validated `DashboardDefinition` should be
the canonical imported artifact so it can be audited and regenerated.

## Deliberate non-goals

The first implementation should not attempt to:

- reconstruct a dashboard from a normal CSV file;
- provide pixel-identical Excel rendering;
- support arbitrary XAML supplied by users;
- implement arbitrary spreadsheet formulas or execute macros;
- support pivot tables, slicers, VBA, custom Excel controls, or every chart type;
- allow a dashboard to mutate simulation state or report evidence;
- replace CSV as a simple, portable report-data export;
- promise compatibility with every spreadsheet application before testing its
  XLSX output against the contract.

## Proposed delivery phases

### Phase 0 - Contract and feasibility spike

**Estimate:** 1-2 days.

- Select one real simulation report and one representative dashboard question.
- Define stable scalar, table, and time-series report keys.
- Create one sample workbook manually.
- Confirm the chosen XLSX reader can inspect cells, merged regions, formatting,
  named ranges/tables, and simple chart sources without launching Excel.
- Record unsupported workbook features and licensing/dependency implications.

**Gate:** demonstrate that the sample workbook can be read deterministically and
mapped into a small hand-verified `DashboardDefinition`.

### Phase 1 - Report workbook export

**Estimate:** 2-4 days.

- Normalize current report data into versioned metadata, scalar metrics, series,
  categories, and tables.
- Export generated data sheets and stable named ranges/tables.
- Keep CSV export available for data-only consumers.
- Add schema-version and missing-field behavior.

**Gate:** two reports with the same schema can populate the same workbook
contract without changing dashboard semantics.

### Phase 2 - Workbook dashboard importer

**Estimate:** 3-6 days.

- Parse the `Dashboard` sheet and supported styles/layout.
- Resolve only approved report names.
- Produce a validated, versioned `DashboardDefinition`.
- Report unsupported features with exact sheet and cell locations.
- Add malicious and oversized-workbook tests.

**Gate:** the sample metric cards, table, and one line chart import without
executing workbook content or embedding current report values.

### Phase 3 - Deterministic Noesis XAML generation

**Estimate:** 3-5 days.

- Generate a constrained XAML tree from `DashboardDefinition`.
- Escape all text and resource values.
- Bind dashboard elements through stable view-model keys.
- Add golden-file and Noesis import tests.

**Gate:** generated XAML imports successfully and displays a second compatible
report without regeneration of its layout.

### Phase 4 - Preview and runtime host

**Estimate:** 4-8 days.

- Add an editor preview with validation diagnostics.
- Add the runtime dashboard view model and report-data adapter.
- Select and load an approved dashboard definition.
- Handle missing data and schema mismatch visibly.
- Validate in an actual player build.

**Gate:** a user-authored workbook dashboard can be imported, previewed, and
used safely with compatible reports in the game.

### Phase 5 - Authoring package and acceptance

**Estimate:** 2-5 days.

- Provide a starter workbook and contract documentation.
- Test Excel and at least one compatible spreadsheet application.
- Add import/export examples and troubleshooting guidance.
- Run a small external authoring test and record where users struggle.

**Gate:** a user unfamiliar with the Unity project can customize the starter
dashboard and reimport it using documented steps.

## First proof-of-concept dashboard

Keep the initial dashboard deliberately small:

1. Final player-species population metric card.
2. Per-species activity table.
3. Population-over-time line chart.

Use one current report to author the workbook, then load a second report with the
same schema into the generated dashboard. That second-report check proves that
the importer created reusable bindings rather than a screenshot or static copy
of the first report.

## Expected effort and decision point

A contract spike should take 1-2 days. A narrow proof of concept covering cells,
merged regions, styles, named bindings, one table, and one line chart is likely
1-2 weeks. A dependable user-facing workflow is likely 3-6 weeks depending on
Noesis chart-control work and cross-application XLSX compatibility.

Do not approve the full implementation from this estimate alone. Approve Phase
0 first, then decide whether the imported `DashboardDefinition` is faithful and
useful enough to justify the runtime and authoring investment.

## Success criteria

This work is valuable if it proves all of the following:

- report exports remain factual, portable, and independent of presentation;
- a non-programmer can design a useful dashboard externally;
- the imported dashboard is reusable across compatible reports;
- unsupported or unsafe workbook content fails clearly;
- generated XAML is deterministic, constrained, and valid in Noesis;
- the workflow reduces the need to hard-code every report presentation in Unity.

