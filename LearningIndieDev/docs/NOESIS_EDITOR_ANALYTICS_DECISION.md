# Noesis editor analytics decision brief

Status: **vendor/license-owner decision required**

## Observed behavior

The embedded NoesisGUI 3.2.13 Editor assembly calls
`GoogleAnalyticsHelper.Install(version)` when the package version stored in
`Noesis.settings` changes. The helper sends a Google Analytics `unity_install`
event containing platform, Unity version, and Noesis version. It generates a
random client identifier for the request and suppresses transport errors.

The assembly definition is Editor-only, so this path is not compiled into the
Windows player. Opening/importing the project after a package-version change can
still transmit from a developer machine. No project setting or public package
documentation in this checkout exposes consent or opt-out behavior. The helper
contains a vendor credential; do not reproduce it in project documentation.

## Decision options

1. Accept and document the vendor's Editor telemetry under the studio privacy
   policy.
2. **Preferred:** ask the vendor/license owner for a supported opt-out,
   consent-gated package, or sanctioned update.
3. With license-owner approval, maintain a narrowly documented embedded-package
   patch that gates or removes only the install-event call. This is technically
   small but creates a vendor-fork/rebase obligation.

Network blocking is useful defense-in-depth but is not a portable project fix.
Credential rotation, if required, belongs to the vendor.

## Verification after a decision

- Reimport with unchanged and changed package versions and observe whether the
  install path executes.
- Verify ordinary Noesis Editor import behavior.
- Run the graphics-capable Play Mode presentation checks.
- Build the Windows player and confirm no Editor assembly is included.

Do not modify the embedded package until the vendor/license owner selects an
option.
