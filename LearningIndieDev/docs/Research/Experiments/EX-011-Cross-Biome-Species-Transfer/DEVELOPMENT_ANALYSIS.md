# EX-011 development-only analysis

**Status:** Sealed before held-out execution
**Panel:** Development seeds `201–220` only
**Analyst:** GPT-5; reasoning effort not exposed
**Metric dictionary:** `cellsim-experiment-metrics` version 1

## Development result

The locked intervention increased `herbivore-statline.fpo` for all 20 paired
development seeds.

| Primary result | Value |
| --- | ---: |
| Control mean FPO | 256.2 Deer |
| Intervention mean FPO | 327.0 Deer |
| Mean paired difference | +70.8 Deer |
| Median paired difference | +63 Deer |
| Paired difference range | +15 to +165 Deer |
| Positive / zero / negative pairs | 20 / 0 / 0 |

This clears the development portion of the preregistered direction rule. It
does not establish transfer because held-out seeds remain unseen.

## What may explain the development result

The intervention reduced mean crowding deaths by 538.5 and mean starvation
deaths by 315.1 per run. It also produced 783.95 fewer births per run on
average. The higher final population therefore appears alongside less
population churn, not simply more reproduction.

That explanation is an inference from aggregate event differences. Because the
two upgrades were intentionally bundled, this experiment cannot assign the
effect to movement speed or crowding tolerance individually.

## Important contradiction

Mean `herbivore-statline.aps` fell by 3.82 even while mean FPO increased by
70.8. Mean `herbivore-statline.rfs` fell by 3.96. This is not a validation
failure: all 40 development Stat-Lines reconciled and both bundles passed.

It means FPO and the composite scores answer different questions in this
regime. The contract correctly made FPO primary, so APS or RFS cannot overturn
the primary development result. It also warns against using one composite as a
universal definition of ecological improvement.

## Held-out prediction

The original preregistered prediction is unchanged: mean paired FPO difference
will be positive on held-out seeds `301–305`, with at least 3 of 5 pairs
positive. No numeric effect-size band is added after seeing development data.

## Evidence

- Control bundle: `artifacts/cellular-experiment-20260912-215056/`
  (`reportSha256` `afef711cc304bc156fe9f89c2abd3bb517863038f6da4f8926c0b38924536149`)
- Intervention bundle: `artifacts/cellular-experiment-20260912-215333/`
  (`reportSha256` `1c0d24c78c65194314b3c4aa57c9aa9bbb3623e96f045af745535828383da12c`)
- Both bundles: report schema 26, metric dictionary version 1, artifact status
  `VALID`, Stat-Line status `VALIDATED_WITH_LIMITATIONS`.
- Source commit recorded by both manifests:
  `993a8440cbd86a4fb14caf7c5a58ed83c0f581eb`; the working tree was disclosed
  as dirty before and after each run.

The full generated comparison remains beside the intervention report as
`development-comparison.md`.
