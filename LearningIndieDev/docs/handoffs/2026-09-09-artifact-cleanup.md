# Artifact cleanup execution — 2026-09-09

Purpose: remove only old, clean, unreferenced diagnostic data after the V1
summaries and retention audit established what must remain.

## Approved scope

- Remove all `.log` files from these 28 older `unity-tests-*` folders. Their
  NUnit result XML remains in place, and each folder has no failed/error test
  case and is not referenced by current documentation:

  - `unity-tests-20260811-222258`
  - `unity-tests-20260811-222601`
  - `unity-tests-20260811-222857`
  - `unity-tests-20260811-223226`
  - `unity-tests-20260811-224703`
  - `unity-tests-20260811-224755`
  - `unity-tests-20260811-225127`
  - `unity-tests-20260812-061428`
  - `unity-tests-20260812-212448`
  - `unity-tests-20260812-220052`
  - `unity-tests-20260904-000653`
  - `unity-tests-20260904-124028`
  - `unity-tests-20260904-124356`
  - `unity-tests-20260904-134132`
  - `unity-tests-20260904-141802`
  - `unity-tests-20260904-160637`
  - `unity-tests-20260904-160909`
  - `unity-tests-20260904-165536`
  - `unity-tests-20260904-191016`
  - `unity-tests-20260905-033121`
  - `unity-tests-20260905-033607`
  - `unity-tests-20260905-035622`
  - `unity-tests-20260905-062522`
  - `unity-tests-20260905-205420`
  - `unity-tests-20260906-043628`
  - `unity-tests-board`
  - `unity-tests-elevated-2`
  - `unity-tests-species-rules`

- Remove only `report.json` and `unity.log` from these two semantically equal,
  clean duplicate bundles. Their summaries and supporting metadata remain:

  - `cellular-experiment-20260811-222638` (retained representative:
    `cellular-experiment-20260811-223303`)
  - `cellular-experiment-20260905-035528` (retained representative:
    `cellular-experiment-20260905-035353`)

Measured reclaim: 1,346,659,855 bytes of test logs plus 2,928,610 bytes of
duplicate raw report/log payloads: 1,349,588,465 bytes (about 1.35 GB).

## Execution result

Completed successfully. All 41 exact targets were present, matched the
pre-cleanup summary sizes and SHA-256 hashes, and were removed. The retained
NUnit XML, compact summaries, duplicate representatives, parse-warning raw
reports, and current-day test evidence were verified afterward. The diagnostics
index now reports 416 remaining Unity logs and 90 NUnit result XML files.

## Explicitly kept

- All NUnit XML result files.
- All current-day `unity-tests-20260909-*` logs.
- All failed/error test logs and their result XML.
- All logs without a trustworthy exit marker outside the approved list.
- All cited, active, or research-relevant logs and reports.
- The three `20260905-063539`, `063730`, and `063858` duplicate candidates,
  because their source reports require trailing-comma recovery.
- All production Unity/gameplay/UI/art files.

## Verification gate

Before deletion, each target must exist, match the diagnostics-summary byte
count and SHA-256, and belong to a clean, unreferenced result folder. After
deletion, every target must be absent while each retained XML, summary, and
representative bundle remains present. Run DirtyBoy afterward and record any
working-tree changes; do not stage, commit, or push.
