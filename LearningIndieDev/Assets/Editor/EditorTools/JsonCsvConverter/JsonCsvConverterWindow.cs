using System;
using System.IO;
using System.Text;
using SaltyGame.EditorTools.JsonCsv;
using UnityEditor;
using UnityEngine;

namespace SaltyGame.EditorTools
{
    public sealed class JsonCsvConverterWindow : EditorWindow
    {
        const int PreviewCharacterLimit = 12000;
        const float PathButtonWidth = 80f;
        const float ActionButtonHeight = 30f;

        enum ConversionDirection
        {
            JsonToCsv,
            CsvToJson,
        }

        ConversionDirection direction;
        string inputPath = string.Empty;
        string outputPath = string.Empty;
        string preview = string.Empty;
        string statusMessage = "Choose an input file to begin.";
        MessageType statusType = MessageType.Info;
        Vector2 previewScrollPosition;
        char delimiter = ',';
        bool flattenNestedObjects = true;
        bool inferCsvValueTypes = true;
        bool parseJsonValuesInCsvCells = true;
        bool writeIndentedJson = true;

        [MenuItem("Salty Game/Tools/JSON-CSV Converter")]
        static void Open()
        {
            var window = GetWindow<JsonCsvConverterWindow>();
            window.titleContent = new GUIContent("JSON-CSV Converter");
            window.minSize = new Vector2(560f, 520f);
            window.Show();
        }

        void OnGUI()
        {
            EditorGUILayout.LabelField("JSON ↔ CSV Converter", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "JSON input must be an array of objects. Nested objects can be flattened into escaped dot paths; "
                + "arrays and other complex values are stored as JSON inside a CSV cell.",
                MessageType.Info);

            EditorGUI.BeginChangeCheck();
            direction = (ConversionDirection)EditorGUILayout.EnumPopup("Direction", direction);
            if (EditorGUI.EndChangeCheck())
            {
                OnDirectionChanged();
            }

            DrawPathField("Input", inputPath, BrowseForInput, value => inputPath = value);
            DrawPathField("Output", outputPath, BrowseForOutput, value => outputPath = value);

            EditorGUILayout.Space(6f);
            DrawOptions();

            EditorGUILayout.Space(6f);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Refresh Preview", GUILayout.Height(ActionButtonHeight)))
            {
                RefreshPreview();
            }

            using (new EditorGUI.DisabledScope(string.IsNullOrWhiteSpace(inputPath) || string.IsNullOrWhiteSpace(outputPath)))
            {
                if (GUILayout.Button("Convert", GUILayout.Height(ActionButtonHeight)))
                {
                    ConvertFile();
                }
            }

            using (new EditorGUI.DisabledScope(!OutputFileExists()))
            {
                if (GUILayout.Button("Reveal Output", GUILayout.Height(ActionButtonHeight)))
                {
                    EditorUtility.RevealInFinder(Path.GetFullPath(outputPath));
                }
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.HelpBox(statusMessage, statusType);
            DrawPreview();
        }

        void DrawOptions()
        {
            EditorGUILayout.LabelField("Conversion Options", EditorStyles.boldLabel);
            var delimiterText = EditorGUILayout.TextField("Delimiter", delimiter.ToString());
            if (delimiterText.Length == 1)
            {
                delimiter = delimiterText[0];
            }
            else if (delimiterText.Length > 1)
            {
                SetStatus("Delimiter must be exactly one character.", MessageType.Warning);
            }

            if (direction == ConversionDirection.JsonToCsv)
            {
                flattenNestedObjects = EditorGUILayout.Toggle("Flatten nested objects", flattenNestedObjects);
            }
            else
            {
                inferCsvValueTypes = EditorGUILayout.Toggle("Infer value types", inferCsvValueTypes);
                parseJsonValuesInCsvCells = EditorGUILayout.Toggle("Parse JSON cell values", parseJsonValuesInCsvCells);
                writeIndentedJson = EditorGUILayout.Toggle("Indent JSON output", writeIndentedJson);
            }
        }

        void DrawPreview()
        {
            EditorGUILayout.LabelField("Output Preview", EditorStyles.boldLabel);
            previewScrollPosition = EditorGUILayout.BeginScrollView(previewScrollPosition);
            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.TextArea(
                    string.IsNullOrEmpty(preview) ? "No preview available." : preview,
                    GUILayout.ExpandHeight(true));
            }

            EditorGUILayout.EndScrollView();
        }

        static void DrawPathField(string label, string value, Action browse, Action<string> setValue)
        {
            EditorGUILayout.BeginHorizontal();
            setValue(EditorGUILayout.TextField(label, value));
            if (GUILayout.Button("Browse…", GUILayout.Width(PathButtonWidth)))
            {
                browse();
            }

            EditorGUILayout.EndHorizontal();
        }

        void BrowseForInput()
        {
            var extension = direction == ConversionDirection.JsonToCsv ? "json" : "csv";
            var selectedPath = EditorUtility.OpenFilePanel("Choose input file", GetInitialDirectory(inputPath), extension);
            if (string.IsNullOrEmpty(selectedPath))
            {
                return;
            }

            inputPath = selectedPath;
            if (string.IsNullOrWhiteSpace(outputPath))
            {
                outputPath = Path.ChangeExtension(inputPath, GetOutputExtension());
            }

            RefreshPreview();
        }

        void BrowseForOutput()
        {
            var extension = GetOutputExtension();
            var defaultName = string.IsNullOrWhiteSpace(inputPath)
                ? $"converted.{extension}"
                : $"{Path.GetFileNameWithoutExtension(inputPath)}.{extension}";
            var selectedPath = EditorUtility.SaveFilePanel(
                "Choose output file",
                GetInitialDirectory(outputPath),
                defaultName,
                extension);

            if (!string.IsNullOrEmpty(selectedPath))
            {
                outputPath = selectedPath;
            }
        }

        void RefreshPreview()
        {
            if (!TryConvertInput(out var result))
            {
                preview = string.Empty;
                return;
            }

            preview = result.Content.Length <= PreviewCharacterLimit
                ? result.Content
                : result.Content.Substring(0, PreviewCharacterLimit) + "\n\n… preview truncated …";
            previewScrollPosition = Vector2.zero;
            SetSuccessStatus("Preview generated", result);
            Repaint();
        }

        void ConvertFile()
        {
            if (!TryGetFullPath(inputPath, "input", out var fullInputPath)
                || !TryGetFullPath(outputPath, "output", out var fullOutputPath))
            {
                return;
            }

            if (string.Equals(fullInputPath, fullOutputPath, StringComparison.OrdinalIgnoreCase))
            {
                SetStatus("Input and output must be different files.", MessageType.Error);
                return;
            }

            if (File.Exists(fullOutputPath)
                && !EditorUtility.DisplayDialog(
                    "Replace existing file?",
                    $"The destination already exists:\n\n{fullOutputPath}\n\nReplace it?",
                    "Replace",
                    "Cancel"))
            {
                SetStatus("Conversion cancelled; the existing file was not changed.", MessageType.Info);
                return;
            }

            if (!TryConvertInput(out var result))
            {
                return;
            }

            try
            {
                var outputDirectory = Path.GetDirectoryName(fullOutputPath);
                if (string.IsNullOrEmpty(outputDirectory) || !Directory.Exists(outputDirectory))
                {
                    SetStatus("The output directory does not exist.", MessageType.Error);
                    return;
                }

                File.WriteAllText(fullOutputPath, result.Content, new UTF8Encoding(false));
                outputPath = fullOutputPath;
                preview = result.Content.Length <= PreviewCharacterLimit
                    ? result.Content
                    : result.Content.Substring(0, PreviewCharacterLimit) + "\n\n… preview truncated …";
                SetSuccessStatus($"Wrote {fullOutputPath}", result);

                if (EditorUtility.DisplayDialog("Conversion complete", statusMessage, "Reveal File", "Close"))
                {
                    EditorUtility.RevealInFinder(fullOutputPath);
                }
            }
            catch (Exception exception) when (exception is IOException || exception is UnauthorizedAccessException)
            {
                SetStatus($"Could not write the output file: {exception.Message}", MessageType.Error);
            }
        }

        bool TryConvertInput(out ConversionResult result)
        {
            result = null;
            if (!TryGetFullPath(inputPath, "input", out var fullInputPath))
            {
                return false;
            }

            if (!File.Exists(fullInputPath))
            {
                SetStatus($"Input file does not exist: {fullInputPath}", MessageType.Error);
                return false;
            }

            try
            {
                var input = File.ReadAllText(fullInputPath, Encoding.UTF8);
                var options = CreateOptions();
                result = direction == ConversionDirection.JsonToCsv
                    ? JsonCsvConverter.ConvertJsonToCsv(input, options)
                    : JsonCsvConverter.ConvertCsvToJson(input, options);
                return true;
            }
            catch (Exception exception) when (
                exception is ArgumentException
                || exception is FormatException
                || exception is IOException
                || exception is UnauthorizedAccessException)
            {
                SetStatus($"Conversion failed: {exception.Message}", MessageType.Error);
                return false;
            }
        }

        ConversionOptions CreateOptions()
        {
            return new ConversionOptions
            {
                Delimiter = delimiter,
                FlattenNestedObjects = flattenNestedObjects,
                InferCsvValueTypes = inferCsvValueTypes,
                ParseJsonValuesInCsvCells = parseJsonValuesInCsvCells,
                WriteIndentedJson = writeIndentedJson,
            };
        }

        void OnDirectionChanged()
        {
            var previousInputPath = inputPath;
            inputPath = outputPath;
            outputPath = string.IsNullOrWhiteSpace(previousInputPath)
                ? string.Empty
                : Path.ChangeExtension(previousInputPath, GetOutputExtension());
            preview = string.Empty;
            SetStatus("Direction changed. Review the input and output paths before converting.", MessageType.Info);
        }

        string GetOutputExtension()
        {
            return direction == ConversionDirection.JsonToCsv ? "csv" : "json";
        }

        bool OutputFileExists()
        {
            if (string.IsNullOrWhiteSpace(outputPath))
            {
                return false;
            }

            try
            {
                return File.Exists(Path.GetFullPath(outputPath));
            }
            catch (Exception exception) when (exception is ArgumentException || exception is NotSupportedException)
            {
                return false;
            }
        }

        static string GetInitialDirectory(string path)
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                try
                {
                    var fullPath = Path.GetFullPath(path);
                    var directory = Directory.Exists(fullPath) ? fullPath : Path.GetDirectoryName(fullPath);
                    if (!string.IsNullOrEmpty(directory) && Directory.Exists(directory))
                    {
                        return directory;
                    }
                }
                catch (Exception exception) when (exception is ArgumentException || exception is NotSupportedException)
                {
                    // Fall back to the project directory for an incomplete path typed into the field.
                }
            }

            return Directory.GetCurrentDirectory();
        }

        bool TryGetFullPath(string path, string label, out string fullPath)
        {
            fullPath = string.Empty;
            if (string.IsNullOrWhiteSpace(path))
            {
                SetStatus($"Choose an {label} file.", MessageType.Warning);
                return false;
            }

            try
            {
                fullPath = Path.GetFullPath(path);
                return true;
            }
            catch (Exception exception) when (exception is ArgumentException || exception is NotSupportedException)
            {
                SetStatus($"The {label} path is invalid: {exception.Message}", MessageType.Error);
                return false;
            }
        }

        void SetSuccessStatus(string prefix, ConversionResult result)
        {
            var warningSummary = result.Warnings.Count == 0
                ? string.Empty
                : $" {result.Warnings.Count} warning(s): {string.Join(" ", result.Warnings)}";
            SetStatus(
                $"{prefix}. {result.RowCount} data row(s), {result.ColumnCount} column(s).{warningSummary}",
                result.Warnings.Count == 0 ? MessageType.Info : MessageType.Warning);
        }

        void SetStatus(string message, MessageType type)
        {
            statusMessage = message;
            statusType = type;
            Repaint();
        }
    }
}
