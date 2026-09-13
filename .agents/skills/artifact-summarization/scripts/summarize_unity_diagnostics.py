#!/usr/bin/env python3
"""Stream Unity logs and NUnit result XML into one compact diagnostics index.

Raw artifacts are never changed.  The index is for triage and context loading;
the original log/XML remains the authority for forensic detail.
"""

from __future__ import annotations

import argparse
import hashlib
import json
import re
import statistics
import xml.etree.ElementTree as ET
from collections import Counter, defaultdict
from datetime import datetime, timezone
from pathlib import Path
from typing import Any, Iterable


SCHEMA = "unity-diagnostics-summary-v1"
MAX_SAMPLE_LENGTH = 240
MAX_SAMPLES_PER_KIND = 3
MAX_TOP_SIGNALS = 12

ANSI_RE = re.compile(r"\x1b\[[0-?]*[ -/]*[@-~]")
VERSION_RE = re.compile(r"Version is '([^']+)'")
DATE_RE = re.compile(r"^Date:\s*(.+)$", re.IGNORECASE)
EXIT_RE = re.compile(r"(?:Exiting with code|return code)\s*(-?\d+)", re.IGNORECASE)
MEMORY_RE = re.compile(r"^##utp:(\{.*\})$")
ERROR_RE = re.compile(r"\b(error|fatal|crash|abort|failure|failed|exception|assert)\b", re.IGNORECASE)
WARNING_RE = re.compile(r"\bwarning\b", re.IGNORECASE)
SENSITIVE_LINE_RE = re.compile(
    r"^\s*(machine id|legacy\.machinebinding\d*|environment(?:user|domain|hostname)|"
    r"devicename|devicemodel|session id|correlation id|external correlation id)\b",
    re.IGNORECASE,
)
PID_RE = re.compile(r"\b(?:pid|process\s*id|processid)\s*[:=]?\s*\d+\b", re.IGNORECASE)
GUID_RE = re.compile(r"\b[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}\b", re.IGNORECASE)
TIMESTAMP_RE = re.compile(r"\b\d{4}-\d{2}-\d{2}T[^\s]+Z\b")


def sha256_file(path: Path) -> str:
    digest = hashlib.sha256()
    with path.open("rb") as handle:
        for chunk in iter(lambda: handle.read(1024 * 1024), b""):
            digest.update(chunk)
    return digest.hexdigest()


def safe_int(value: Any) -> int | None:
    try:
        return int(value)
    except (TypeError, ValueError):
        return None


def safe_float(value: Any) -> float | None:
    try:
        result = float(value)
    except (TypeError, ValueError):
        return None
    return round(result, 6) if result == result else None


def sanitize(value: str, artifact_root: Path) -> str:
    """Normalize paths/volatile IDs and refuse to copy machine identity data."""

    value = ANSI_RE.sub("", value).strip()
    if not value or SENSITIVE_LINE_RE.search(value):
        return "<redacted>" if value else ""
    for root in (str(artifact_root), str(artifact_root).replace("\\", "/")):
        value = value.replace(root, "<artifacts>")
    value = value.replace("\\", "/")
    value = PID_RE.sub("<pid>", value)
    value = GUID_RE.sub("<guid>", value)
    value = TIMESTAMP_RE.sub("<timestamp>", value)
    value = re.sub(r"\s+", " ", value)
    return value[:MAX_SAMPLE_LENGTH]


def relative(path: Path, artifact_root: Path) -> str:
    return path.relative_to(artifact_root).as_posix()


def parse_command(lines: list[str], artifact_root: Path) -> dict[str, Any]:
    args: list[str] = []
    in_args = False
    project_path = ""
    for line in lines:
        stripped = line.strip()
        if stripped == "COMMAND LINE ARGUMENTS:":
            in_args = True
            continue
        if in_args and stripped.startswith("Successfully changed project path"):
            project_path = sanitize(stripped.split(":", 1)[-1], artifact_root)
            break
        if in_args and stripped and len(args) < 40:
            args.append(sanitize(stripped, artifact_root))
    parsed: dict[str, Any] = {}
    for index, token in enumerate(args[:-1]):
        if token in {"-testPlatform", "-testResults", "-projectPath", "-executeMethod"}:
            parsed[token.lstrip("-")] = args[index + 1]
    if args:
        parsed["arguments"] = args
    if project_path:
        parsed["resolvedProjectPath"] = project_path
    return parsed


def summarize_log(path: Path, artifact_root: Path) -> dict[str, Any]:
    counts: Counter[str] = Counter()
    samples: dict[str, list[str]] = defaultdict(list)
    signals: Counter[str] = Counter()
    dates: list[str] = []
    versions: list[str] = []
    exit_codes: list[int] = []
    command_lines: list[str] = []
    allocated_memory: list[int] = []
    line_count = 0
    has_success_marker = False
    has_crash_marker = False

    digest = hashlib.sha256()
    with path.open("rb") as handle:
        for raw in handle:
            digest.update(raw)
            line_count += 1
            line = raw.decode("utf-8", errors="replace").rstrip("\r\n")
            stripped = line.strip()
            if len(command_lines) < 80:
                command_lines.append(line)
            if match := VERSION_RE.search(line):
                versions.append(match.group(1))
            if match := DATE_RE.search(line):
                # The run timestamp is useful provenance, unlike volatile
                # identifiers that sanitize() intentionally removes.
                dates.append(match.group(1).strip())
            if match := EXIT_RE.search(line):
                code = safe_int(match.group(1))
                if code is not None:
                    exit_codes.append(code)
            if "Exiting batchmode successfully" in line or "Run completed" in line:
                has_success_marker = True
            if re.search(r"\b(?:Crash!!!|Aborting|Fatal Error)\b", line, re.IGNORECASE):
                has_crash_marker = True

            is_license = "[Licensing::" in line
            if is_license:
                counts["licenseLines"] += 1
            if ERROR_RE.search(line):
                counts["errorLines"] += 1
                counts["licenseErrorLines"] += int(is_license)
                counts["nonLicenseErrorLines"] += int(not is_license)
                if len(samples["error"]) < MAX_SAMPLES_PER_KIND:
                    sample = sanitize(line, artifact_root)
                    if sample and sample != "<redacted>":
                        samples["error"].append(sample)
            if WARNING_RE.search(line):
                counts["warningLines"] += 1
                if len(samples["warning"]) < MAX_SAMPLES_PER_KIND:
                    sample = sanitize(line, artifact_root)
                    if sample and sample != "<redacted>":
                        samples["warning"].append(sample)
            if re.search(r"exception", line, re.IGNORECASE):
                counts["exceptionLines"] += 1
                if len(samples["exception"]) < MAX_SAMPLES_PER_KIND:
                    sample = sanitize(line, artifact_root)
                    if sample and sample != "<redacted>":
                        samples["exception"].append(sample)
            if re.search(r"\(Filename:|^\s*at\s+", line):
                counts["stackTraceLines"] += 1

            if match := MEMORY_RE.match(stripped):
                try:
                    payload = json.loads(match.group(1))
                except json.JSONDecodeError:
                    counts["unparsedMemoryEvents"] += 1
                else:
                    if payload.get("type") == "MemoryLeaks":
                        counts["memoryLeakEvents"] += 1
                        memory = safe_int(payload.get("allocatedMemory"))
                        if memory is not None:
                            allocated_memory.append(memory)

            if (ERROR_RE.search(line) or WARNING_RE.search(line)) and not re.search(
                r"^\s*(?:at\s+|\(Filename:|UnityEngine\.TestRunner\.)", line
            ):
                signal = sanitize(line, artifact_root)
                if signal and signal != "<redacted>":
                    signals[signal] += 1

    exit_code = exit_codes[-1] if exit_codes else None
    if has_crash_marker or (exit_code is not None and exit_code != 0):
        outcome = "failed"
    elif has_success_marker or exit_code == 0:
        outcome = "success"
    else:
        outcome = "unknown"
    folder = path.parent.name
    row: dict[str, Any] = {
        "path": relative(path, artifact_root),
        "folder": folder,
        "bytes": path.stat().st_size,
        "sha256": digest.hexdigest(),
        "lines": line_count,
        "outcome": outcome,
        "unityVersion": versions[-1] if versions else None,
        "date": dates[0] if dates else None,
        "exitCode": exit_code,
        "counts": dict(counts),
        "command": parse_command(command_lines, artifact_root),
    }
    if allocated_memory:
        row["memoryLeakAllocatedBytes"] = {
            "min": min(allocated_memory),
            "max": max(allocated_memory),
            "mean": round(statistics.fmean(allocated_memory), 3),
        }
    if samples:
        row["samples"] = dict(samples)
    if signals:
        row["topSignals"] = [
            {"count": count, "message": message}
            for message, count in signals.most_common(MAX_TOP_SIGNALS)
        ]
    result_xml = path.with_name(path.stem + "-results.xml")
    if result_xml.exists():
        row["resultXml"] = relative(result_xml, artifact_root)
    return row


def element_text(element: ET.Element | None) -> str:
    return " ".join(element.itertext()).strip() if element is not None else ""


def summarize_test_result(path: Path, artifact_root: Path) -> dict[str, Any]:
    tree = ET.parse(path)
    root = tree.getroot()
    counts: Counter[str] = Counter()
    failures: list[dict[str, Any]] = []
    for case in root.iter("test-case"):
        result = case.attrib.get("result", "Unknown")
        counts[result] += 1
        if result.lower() not in {"passed", "skipped", "inconclusive"}:
            failure = case.find(".//failure")
            row: dict[str, Any] = {
                "name": case.attrib.get("fullname") or case.attrib.get("name", ""),
                "result": result,
            }
            if failure is not None:
                message = element_text(failure.find("message"))
                stack = element_text(failure.find("stack-trace"))
                if message:
                    row["message"] = sanitize(message, artifact_root)
                if stack:
                    row["stack"] = sanitize(stack, artifact_root)
            failures.append(row)
    root_counts = {
        key: safe_int(root.attrib.get(key))
        for key in ("testcasecount", "total", "passed", "failed", "inconclusive", "skipped")
        if root.attrib.get(key) is not None
    }
    row: dict[str, Any] = {
        "path": relative(path, artifact_root),
        "folder": path.parent.name,
        "bytes": path.stat().st_size,
        "sha256": sha256_file(path),
        "platform": path.stem.removesuffix("-results"),
        "start": root.attrib.get("start-time"),
        "end": root.attrib.get("end-time"),
        "durationSeconds": safe_float(root.attrib.get("duration")),
        "counts": root_counts or dict(counts),
        "caseResults": dict(counts),
    }
    if failures:
        row["failures"] = failures
    return row


def markdown(summary: dict[str, Any]) -> str:
    totals = summary["totals"]
    lines = [
        "# Unity diagnostics summary (V1)",
        "",
        f"Generated: `{summary['generatedUtc']}`",
        "",
        "Raw Unity logs and NUnit XML were not modified. This index is a triage aid; the raw artifacts remain the forensic authority.",
        "",
        "## Inventory",
        "",
        "| Evidence | Files | Bytes |",
        "| --- | ---: | ---: |",
        f"| Unity logs | {totals['logs']} | {totals['logBytes']:,} |",
        f"| NUnit result XML | {totals['testResults']} | {totals['testResultBytes']:,} |",
        "",
        "## Log outcomes",
        "",
        "| Outcome | Count |",
        "| --- | ---: |",
    ]
    for outcome, count in sorted(summary["logOutcomes"].items()):
        lines.append(f"| {outcome} | {count} |")
    lines.extend(
        [
            "",
            "Counts intentionally include Unity/framework and licensing text. A successful exit is not proof that every line is harmless; use the paired NUnit result or raw log when investigating a failure.",
            "",
            "## Test results needing attention",
            "",
            "| Result XML | Platform | Total | Passed | Failed | Skipped | Duration (s) |",
            "| --- | --- | ---: | ---: | ---: | ---: | ---: |",
        ]
    )
    attention = [row for row in summary["testResults"] if row.get("caseResults", {}).get("Failed", 0) or row.get("caseResults", {}).get("Error", 0)]
    if not attention:
        lines.append("| None | — | — | — | — | — | — |")
    else:
        for row in attention:
            counts = row.get("counts", {})
            lines.append(
                f"| `{row['path']}` | {row.get('platform', '—')} | {counts.get('total', '—')} | {counts.get('passed', '—')} | {counts.get('failed', '—')} | {counts.get('skipped', '—')} | {row.get('durationSeconds', '—')} |"
            )
    lines.extend(["", "## Logs with failed or unknown process exit", "", "| Log | Outcome | Exit | Non-license errors | Warnings |", "| --- | --- | ---: | ---: | ---: |"])
    log_attention = [
        row
        for row in summary["logs"]
        if row["outcome"] != "success"
    ]
    if not log_attention:
        lines.append("| None | — | — | — | — |")
    else:
        for row in log_attention:
            counts = row.get("counts", {})
            lines.append(
                f"| `{row['path']}` | {row['outcome']} | {row.get('exitCode', '—')} | {counts.get('nonLicenseErrorLines', 0)} | {counts.get('warningLines', 0)} |"
            )
    lines.extend(
        [
            "",
            "## Compression boundary",
            "",
            "The summary keeps provenance hashes, line counts, Unity/version/date/command identity, exit evidence, severity counts, bounded redacted signal samples, memory-leak totals, and failed test cases. It omits raw event order and full stack/log replay. Do not delete a raw log solely because this summary exists.",
        ]
    )
    return "\n".join(lines) + "\n"


def build_summary(artifact_root: Path) -> dict[str, Any]:
    logs = sorted(artifact_root.rglob("*.log"))
    test_results = sorted(artifact_root.rglob("*-results.xml"))
    log_rows = [summarize_log(path, artifact_root) for path in logs]
    test_rows = [summarize_test_result(path, artifact_root) for path in test_results]
    outcomes = Counter(row["outcome"] for row in log_rows)
    return {
        "schema": SCHEMA,
        "generatedUtc": datetime.now(timezone.utc).isoformat().replace("+00:00", "Z"),
        "artifactRoot": artifact_root.as_posix(),
        "totals": {
            "logs": len(log_rows),
            "logBytes": sum(row["bytes"] for row in log_rows),
            "testResults": len(test_rows),
            "testResultBytes": sum(row["bytes"] for row in test_rows),
        },
        "logOutcomes": dict(outcomes),
        "logs": log_rows,
        "testResults": test_rows,
        "limitations": [
            "Raw logs and NUnit XML remain the forensic authority.",
            "Diagnostic counts include routine Unity/framework/licensing text and require interpretation.",
            "Machine identity values, volatile IDs, and full raw event order are redacted or omitted from the compact index.",
        ],
    }


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--artifact-root", type=Path, required=True)
    parser.add_argument("--output", type=Path, required=True)
    parser.add_argument("--markdown-output", type=Path)
    args = parser.parse_args()
    artifact_root = args.artifact_root.resolve()
    if not artifact_root.is_dir():
        parser.error(f"Artifact root is not a directory: {artifact_root}")
    summary = build_summary(artifact_root)
    args.output.parent.mkdir(parents=True, exist_ok=True)
    args.output.write_text(json.dumps(summary, ensure_ascii=False, separators=(",", ":")), encoding="utf-8")
    markdown_path = args.markdown_output or args.output.with_suffix(".md")
    markdown_path.write_text(markdown(summary), encoding="utf-8")
    print(f"logs={summary['totals']['logs']} testResults={summary['totals']['testResults']}")
    print(f"json={args.output.resolve()}")
    print(f"markdown={markdown_path.resolve()}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
