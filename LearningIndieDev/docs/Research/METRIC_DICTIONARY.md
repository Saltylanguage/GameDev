# CellSim metric dictionary

The machine-readable source of truth is
[`METRIC_DICTIONARY_V1.json`](METRIC_DICTIONARY_V1.json). Version 1 defines
every field in the herbivore Stat-Line, including each score's meaning, unit,
direction, code source, and valid measurement windows.

## How to use it

- Match a report value through `reportField`.
- Use `id` as the stable name in experiment contracts, analyses, and AI prompts.
- Check the matching status field before using a derived numeric metric. `N/A`
  is not a measured zero, and `INVALID` must not be analyzed.
- Compare values only when their measurement window, scenario, ruleset, and run
  conditions are compatible.
- Treat direction as interpretation, not a universal balance target. More
  survivors or births can still be undesirable if the design calls for a
  bounded population or meaningful tradeoffs.

## Versioning rule

Create a new dictionary version when a definition, formula, unit, direction,
source event, or valid window changes. Correcting prose without changing the
meaning may keep the same version.

New experiment reports record `metricDictionaryId` and
`metricDictionaryVersion`. Their artifact bundles also contain the exact
dictionary as `metric-dictionary.json`; `manifest.json` records its identity
and SHA-256 hash. This keeps an archived experiment understandable even after
the project dictionary evolves.
