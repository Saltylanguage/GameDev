# Current-state audit

This is an evidence audit of the predictive-AI research lane as found on
2026-09-03. Priorities describe risk to the research claim, not production
severity. “Observed” means directly supported by repository evidence;
“inference” is explicitly labelled.

## Findings

| Priority | Finding | Evidence and confidence | Recommended action |
|---|---|---|---|
| P0 | The pre-registered baseline summary has the wrong mean `RFS`. | `AI_INPUT-EXP-007-0001.md` says `0.63`; recomputing its referenced report gives `0.400402` across 20 seeds. **Observed; high confidence.** | Generate bounded summaries from `report.json` and fail registration on any mismatch. Preserve the historical input with its erratum. |
| P0 | The claimed AI information boundary is not independently auditable. | The prediction has a prose pre-registration note but no model ID, exact prompt, context manifest/hash, invocation ID, or fresh-context evidence. This does not show leakage; it leaves leakage **unknown**. **Observed absence; high confidence.** | Run confirmatory forecasts in a fresh sealed task and store the exact allowed bundle, hashes, model, prompt, and timestamp. |
| P0 | The current confidence check has no well-defined forecast event. | Each arm has one confidence (`0.56` or `0.62`), but the analysis treats four correlated metrics across two panels as eight direction checks. Those checks are neither independent nor the event to which the confidence was explicitly attached. **Observed; high confidence.** | Put probability on each defined metric event, or define one composite event before execution. Score only those events with a proper scoring rule. |
| P0 | Former held-out panels have been consumed. | Results for seeds 101–105 and 106–110 are published and have informed later questions. They remain valid for their original tests but are no longer unseen for future confirmation. **Observed; high confidence.** | Register seed-panel roles and retire a panel from blind use after disclosure. Reserve a one-time blind promotion panel or scenario. |
| P0 | Human promotion has not happened. | EX-007 `HUMAN_DECISION.md` remains pending; the P3 gate requires a human decision. **Observed; high confidence.** | Record Accept, Reject, Revise and rerun, Inconclusive, or Archive, with scope and evidence references. |
| P0 | P3's formal exit gate still includes unexecuted EX-003. | The phase table requires EX-003/EX-007 held-out and false-cause gates; no EX-003 package is present. **Observed; high confidence.** | Execute a bounded EX-003 or explicitly revise the canonical exit gate through human review. |
| P1 | “Training seeds” contradicts the canonical terminology rule. | The plan says “training” is appropriate only after a separate model-training design is approved; EX-007 calls seeds 1–20 training despite no model fitting. **Observed; high confidence.** | Use **development** or **calibration** panel prospectively. Add a note to historical packages rather than rewriting them. |
| P1 | A five-seed held-out panel has low decision resolution. | With five non-tied paired signs, even 5/5 agreement has exact two-sided sign probability `0.0625`. Exact values are useful, but the panel alone cannot support a conventional two-sided 5% sign rule. **Verified calculation; high confidence.** | Label five seeds a transfer smoke test. Use a larger fresh blind panel when promotion depends on stable direction or effect size. |
| P1 | Endpoint multiplicity permits accidental cherry-picking. | EX-007 scores several outcomes and inspects additional telemetry after the run, but no primary/secondary/exploratory family controls the decision. **Observed; medium-high confidence.** | Name one primary endpoint, a small pre-registered secondary family, and keep all other telemetry exploratory. |
| P1 | Local and worker bundle contracts differ. | `Run-CellularExperiment.ps1` writes report/manifest/log; `Start-CellSimWorker.ps1` adds `statline.csv`. **Observed; high confidence.** | Use one shared StatLine exporter and enforce the five-file bundle before analysis. |
| P1 | Run and prediction provenance is incomplete. | The manifest has commit, dirty state, scenario, executable, and arguments, but lacks normalized branch, package fingerprint, host, wrapper revision, and analysis provenance. **Observed; high confidence.** | Add normalized run fields; store analysis/prediction provenance beside, not inside, immutable raw results. |
| P1 | The current status is split between a stale gate snapshot and newer evidence. | `P3_GATE_REVIEW_2026-09-03.md` says EX-007 is contract-only; the plan records completed EX-007 evidence, EX-008, and blocked EX-009. **Observed; high confidence.** | Publish a dated superseding status note without rewriting the historical snapshot. |
| P1 | `PREY` required a post-run semantic correction. | The input treated it as a resource event; code/report define it as Hare deaths caused by carnivores. **Observed; high confidence.** | Version a metric dictionary and require acknowledgement of its hash before prediction. |
| P2 | EX-009 is currently framed as a research blocker when it is mostly an implementation invariant. | The two upgrades add to distinct fields; both orderings have the same ruleset fingerprint; all 20 matched run objects are identical. Only the provenance fingerprint differs because order is recorded. **Observed; high confidence within current code path.** | Add a commutativity unit test and optionally one runtime smoke test. Reopen as research only if upgrade application becomes stateful or order-sensitive. |
| P2 | Interaction evidence is incomplete. | EX-007 did not contain all B/A/C/AC cells on both panels; EX-008 added C on a different held-out panel. **Observed; high confidence.** | If interaction matters, run a clean four-cell same-seed design with a pre-defined interaction contrast and fresh validation panel. |
| P2 | Mean final population can hide timing and spatial effects. | The plan calls for occupancy, entropy, concentration, displacement, and threshold metrics, while EX-007 mainly scores aggregate StatLines. **Observed; high confidence.** | Add one named spatial or time-window metric only when a specific hypothesis needs it. |

## Important distinctions

- **Consumed is not invalid.** The existing held-out panels still support the
  experiments for which they were first reserved. They simply cannot remain
  blind for later, adaptively chosen claims.
- **Unknown is not contaminated.** Missing context provenance prevents an
  audit; it is not proof that the model saw intervention results.
- **Same ruleset is not a universal order theorem.** The present two upgrades
  commute under the current additive implementation. Other upgrade types or
  future stateful mechanics may not.
- **Small is not useless.** Five seeds are useful for catching reversals and
  inspecting exact paired behavior. They are too coarse for strong promotion
  claims by themselves.

## What should not be changed

- Do not rewrite the original EX-007 prediction to repair RFS, PREY, or the
  “training” label. Preserve it and attach explicit errata.
- Do not replace raw reports with averages or a dashboard. Every summary must
  remain rebuildable from immutable artifacts.
- Do not call EX-007 a general predictive capability or a balance result.
- Do not bypass Unity preflight. If an EX-009 smoke run is retained, a failed
  preflight remains an operational result rather than experimental evidence.

## Resolved strengths

- Independent artifact and StatLine validators exist.
- Reports distinguish observation, interpretation, and design judgment.
- Seed-matched intervention comparisons are already the normal pattern.
- The plan prevents AI self-promotion and keeps human review authoritative.
- Ruleset and provenance fingerprints make the order-test diagnosis possible.
