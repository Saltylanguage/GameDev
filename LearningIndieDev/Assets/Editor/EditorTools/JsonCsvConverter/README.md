# JSON-CSV Converter

The JSON-CSV Converter is an editor-only data utility available from
`Salty Game > Tools > JSON-CSV Converter`.

## Supported data

JSON-to-CSV input must be a JSON array whose items are objects:

```json
[
  {
    "name": "hare",
    "stats": {
      "energy": 45
    },
    "diet": ["fern", "grass"]
  }
]
```

With nested-object flattening enabled, this produces columns such as
`name`, `stats.energy`, and `diet`. Arrays and other complex values are stored
as compact JSON inside a quoted CSV cell.

Property names containing `.` or `\` are escaped in CSV headers so converting
the CSV back to JSON restores the original property names.

## Using the window

1. Select `JSON To Csv` or `Csv To Json`.
2. Choose an input file and destination file.
3. Adjust the conversion options if necessary.
4. Select **Refresh Preview** to inspect the converted output.
5. Select **Convert** to write the destination.

The converter asks before replacing an existing file and refuses to use the
same file as both input and output. Output is written as UTF-8 without a byte
order mark.

## Conversion options

- **Delimiter:** Defaults to a comma. A semicolon or tab-like alternative may
  be selected when required by another tool, provided it is one character.
- **Flatten nested objects:** Converts nested object properties to escaped
  dot-separated columns. When disabled, nested objects remain JSON cell values.
- **Infer value types:** Converts recognizable CSV values to JSON numbers,
  booleans, or nulls. Disable this when values such as `0012` must remain strings.
- **Parse JSON cell values:** Restores CSV cells beginning with valid `{...}` or
  `[...]` JSON as objects or arrays.
- **Indent JSON output:** Produces human-readable JSON instead of compact JSON.

## Round-trip limitations

CSV does not store type or schema information. The converter therefore cannot
always distinguish an empty string from null or a numeric-looking string from a
number. Disable type inference when preserving text is more important than
recovering primitive JSON types.

Missing and empty CSV fields currently become JSON null values. Exact lossless
round trips would require a separate schema file and are outside the current
tool scope.

## Verification

The Edit Mode tests are in `Assets/Tests/Editor/JsonCsvConverterTests.cs`.
Run them from Unity's Test Runner or, with the Editor closed, from PowerShell:

```powershell
.\tools\Invoke-UnityTests.ps1 -Mode EditMode
```
