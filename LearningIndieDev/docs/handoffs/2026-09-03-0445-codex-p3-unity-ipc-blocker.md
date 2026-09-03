# P3 execution handoff — Unity UPM/licensing IPC blocker

Date: 2026-09-03

## Result

P3 is still open. The worker publication path is healthy, but the corrected
EX-007 runs cannot reach simulation because Unity cannot establish its local
Package Manager/licensing IPC services.

## Evidence

- Exact stale editor/UPM processes from an earlier attempt were found and
  closed with the user's standing Unity open/close authorization: Unity PID
  `22556` and its orphaned Package Manager PID `49860`.
- Clean preflight retries still failed after the stale pair was removed:
  `artifacts/unity-preflight-20260903-042903/license-probe.log`,
  `artifacts/unity-preflight-20260903-043444/license-probe.log`,
  `artifacts/unity-preflight-20260903-043725/license-probe.log`, and
  `artifacts/unity-preflight-20260903-043849/license-probe.log` each end with
  `Could not connect to IPC stream "Upm-*" after 30.0 seconds` and a non-zero
  Unity exit.
- A controlled `-noUpm` probe reached assembly reload but then recorded
  `Licensing is not yet initialized`, a 60-second licensing timeout, a refused
  `LicenseClient-joshc` channel, and `Licensing initialization failed after
  74.81s` in `artifacts/no-upm-probe.log`.
- The local entitlement file exists at
  `C:/Users/joshc/AppData/Local/Unity/licenses/UnityEntitlementLicense.xml`.
  This rules out a missing-license-file explanation; the remaining failure is
  the machine-level Unity licensing/IPC handshake.
- After the probes, no Unity or UnityPackageManager process remained. Process
  inspection found one live Hub-owned licensing client (PID `21236`) and eight
  orphaned `Unity.Licensing.Client.exe` process records created by earlier
  Unity launch attempts. The eight orphan records have zero threads/handles,
  no executable path or command line, and parent PIDs that no longer exist;
  they are not nine active licensing services. They were not force-terminated
  because the standing authorization covers Unity editor open/close, not
  indiscriminate licensing-service termination.

## Interpretation

This is an external Unity/Hub service-state blocker, not a simulation or
worker-packaging failure. No preflight bypass was added, and no result is
valid P3 evidence until Unity produces a complete, validated bundle.

## Next action

Reboot (or, if preferred, use Unity Hub to sign out/in and restart its
licensing service), then confirm no Unity/UnityPackageManager processes remain
and rerun `tools/Test-UnityPreflight.ps1`. Once it passes, resubmit the failed
training baseline, process the pending held-out baseline, and continue EX-007
in its declared order.
