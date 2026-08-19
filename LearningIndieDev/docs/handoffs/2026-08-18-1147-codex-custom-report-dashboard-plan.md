# custom report dashboard tooling proposal

[Working state](../WORKING_STATE.md) | Status: proposed; feasibility spike needs
human approval

- Owner: codex
- Audience: Sim and developer-tooling contributors
- Branch: `codex/cellular-sprite-tiling`
- Baseline commit: `029f2a7`
- Date: 2026-08-18

## Note for Sim

We explored a tool that would let users build custom game-report dashboards in
spreadsheet software and reimport them for display through Noesis. The important
finding is that the current CSV report cannot support that round trip: CSV keeps
data values, but discards charts, colors, layout, merged regions, named ranges,
and other presentation semantics. This is missing information, not a parser gap.

The recommended direction is to preserve CSV as a clean data export and evaluate
an `.xlsx` authoring workflow. The game would generate factual data sheets and
stable named report ranges; users would build a `Dashboard` sheet; an importer
would compile a safe subset of workbook layout and chart semantics into a
validated `DashboardDefinition`, then generate constrained Noesis XAML.

## Why this could be valuable

- Users could create report presentations without Unity, C#, or direct XAML.
- One dashboard template could display many reports with the same schema.
- Community dashboards could emphasize different questions without requiring a
  new first-party screen for every analysis style.
- Developers and designers could prototype report UX in familiar spreadsheet
  tools before promoting it into permanent game UI.
- Report evidence would remain separate from presentation and styling.

## Boundaries to preserve

- Do not attempt ordinary CSV-to-XAML reconstruction; the visual semantics do
  not survive CSV export.
- Do not accept arbitrary user XAML or execute macros, external links, or
  unrestricted spreadsheet formulas.
- Import through a versioned `DashboardDefinition` and a whitelist of supported
  Noesis controls, bindings, styles, and resource types.
- Treat generated XAML as derived output, not the canonical user artifact.
- Keep dashboards presentation-only; they cannot alter reports or simulation.

## Proposed first decision

Approve only a 1-2 day feasibility spike initially. Use one real report and a
sample workbook containing one metric card, one activity table, and one
population-history line chart. Prove that the workbook can be parsed into a
deterministic dashboard definition and rebound to a second compatible report.

The complete proposed phases, risks, mappings, benefits, and success criteria
are in [`CUSTOM_REPORT_DASHBOARD_TOOLING_PLAN.md`](../CUSTOM_REPORT_DASHBOARD_TOOLING_PLAN.md).

## Validation performed

Documentation review only. No workbook parser, XAML generator, runtime dashboard,
or prototype asset was created, and no implementation direction has been
approved yet.

