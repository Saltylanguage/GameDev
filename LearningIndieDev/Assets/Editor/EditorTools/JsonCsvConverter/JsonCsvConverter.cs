using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SaltyGame.EditorTools.JsonCsv
{
    public static class JsonCsvConverter
    {
        public static ConversionResult ConvertJsonToCsv(string json, ConversionOptions options = null)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new ArgumentException("JSON input cannot be empty.", nameof(json));
            }

            options ??= new ConversionOptions();
            var root = ParseJsonArray(json);
            var flattenedRows = new List<Dictionary<string, string>>(root.Count);
            var headers = new SortedSet<string>(StringComparer.Ordinal);

            for (var index = 0; index < root.Count; index++)
            {
                if (!(root[index] is JObject source))
                {
                    throw new FormatException($"JSON array item {index} must be an object, but was {root[index].Type}.");
                }

                var flattened = new Dictionary<string, string>(StringComparer.Ordinal);
                FlattenObject(source, string.Empty, flattened, options.FlattenNestedObjects);
                flattenedRows.Add(flattened);
                headers.UnionWith(flattened.Keys);
            }

            var orderedHeaders = headers.ToList();
            if (orderedHeaders.Count == 0)
            {
                throw new FormatException("JSON input does not contain any properties to use as CSV columns.");
            }

            var csvRows = new List<IEnumerable<string>>(flattenedRows.Count + 1) { orderedHeaders };
            foreach (var flattened in flattenedRows)
            {
                csvRows.Add(orderedHeaders.Select(header => flattened.TryGetValue(header, out var value) ? value : string.Empty));
            }

            return new ConversionResult(
                CsvWriter.Write(csvRows, options.Delimiter),
                flattenedRows.Count,
                orderedHeaders.Count);
        }

        public static ConversionResult ConvertCsvToJson(string csv, ConversionOptions options = null)
        {
            if (string.IsNullOrWhiteSpace(csv))
            {
                throw new ArgumentException("CSV input cannot be empty.", nameof(csv));
            }

            options ??= new ConversionOptions();
            var rows = CsvReader.Read(csv, options.Delimiter);
            if (rows.Count == 0)
            {
                throw new FormatException("CSV input does not contain a header row.");
            }

            var headers = rows[0].ToList();
            if (headers.Count > 0)
            {
                headers[0] = headers[0].TrimStart('\uFEFF');
            }

            ValidateHeaders(headers);

            var output = new JArray();
            var warnings = new List<string>();
            for (var rowIndex = 1; rowIndex < rows.Count; rowIndex++)
            {
                var row = rows[rowIndex];
                if (row.Count > headers.Count)
                {
                    throw new FormatException($"CSV row {rowIndex + 1} contains {row.Count} fields, but the header contains {headers.Count}.");
                }

                if (row.Count < headers.Count)
                {
                    warnings.Add($"CSV row {rowIndex + 1} has {row.Count} fields; {headers.Count - row.Count} missing values were treated as null.");
                }

                var item = new JObject();
                for (var columnIndex = 0; columnIndex < headers.Count; columnIndex++)
                {
                    var value = columnIndex < row.Count ? row[columnIndex] : string.Empty;
                    JsonPathUtility.SetValue(item, headers[columnIndex], ParseCsvValue(value, options));
                }

                output.Add(item);
            }

            var formatting = options.WriteIndentedJson ? Formatting.Indented : Formatting.None;
            return new ConversionResult(output.ToString(formatting), output.Count, headers.Count, warnings);
        }

        static JArray ParseJsonArray(string json)
        {
            try
            {
                var root = ParseJsonToken(json);
                if (!(root is JArray array))
                {
                    throw new FormatException($"JSON root must be an array, but was {root.Type}.");
                }

                return array;
            }
            catch (JsonReaderException exception)
            {
                throw new FormatException($"Invalid JSON at line {exception.LineNumber}, position {exception.LinePosition}: {exception.Message}", exception);
            }
        }

        static void FlattenObject(
            JObject source,
            string parentPath,
            IDictionary<string, string> destination,
            bool flattenNestedObjects)
        {
            foreach (var property in source.Properties())
            {
                var path = JsonPathUtility.Combine(parentPath, property.Name);
                if (flattenNestedObjects && property.Value is JObject nested && nested.HasValues)
                {
                    FlattenObject(nested, path, destination, true);
                }
                else
                {
                    destination.Add(path, FormatJsonValue(property.Value));
                }
            }
        }

        static string FormatJsonValue(JToken value)
        {
            switch (value.Type)
            {
                case JTokenType.Null:
                case JTokenType.Undefined:
                    return string.Empty;
                case JTokenType.String:
                    return value.Value<string>();
                case JTokenType.Boolean:
                    return value.Value<bool>() ? "true" : "false";
                case JTokenType.Integer:
                    return Convert.ToString(((JValue)value).Value, CultureInfo.InvariantCulture);
                case JTokenType.Float:
                    return Convert.ToString(((JValue)value).Value, CultureInfo.InvariantCulture);
                case JTokenType.Date:
                    return value.Value<DateTime>().ToString("o", CultureInfo.InvariantCulture);
                default:
                    return value.ToString(Formatting.None);
            }
        }

        static JToken ParseCsvValue(string value, ConversionOptions options)
        {
            if (value.Length == 0)
            {
                return JValue.CreateNull();
            }

            if (options.ParseJsonValuesInCsvCells && (value[0] == '[' || value[0] == '{'))
            {
                try
                {
                    return ParseJsonToken(value);
                }
                catch (JsonReaderException)
                {
                    // A cell beginning with a bracket is still allowed to be an ordinary string.
                }
            }

            if (!options.InferCsvValueTypes)
            {
                return new JValue(value);
            }

            if (string.Equals(value, "null", StringComparison.OrdinalIgnoreCase))
            {
                return JValue.CreateNull();
            }

            if (bool.TryParse(value, out var boolean))
            {
                return new JValue(boolean);
            }

            if (long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var integer))
            {
                return new JValue(integer);
            }

            if (decimal.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var number))
            {
                return new JValue(number);
            }

            if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var floatingPointNumber))
            {
                return new JValue(floatingPointNumber);
            }

            return new JValue(value);
        }

        static JToken ParseJsonToken(string json)
        {
            using (var stringReader = new StringReader(json))
            using (var jsonReader = new JsonTextReader(stringReader) { DateParseHandling = DateParseHandling.None })
            {
                return JToken.ReadFrom(jsonReader);
            }
        }

        static void ValidateHeaders(IReadOnlyList<string> headers)
        {
            if (headers.Count == 0)
            {
                throw new FormatException("CSV header row cannot be empty.");
            }

            var uniqueHeaders = new HashSet<string>(StringComparer.Ordinal);
            for (var index = 0; index < headers.Count; index++)
            {
                if (string.IsNullOrWhiteSpace(headers[index]))
                {
                    throw new FormatException($"CSV header column {index + 1} is empty.");
                }

                if (!uniqueHeaders.Add(headers[index]))
                {
                    throw new FormatException($"CSV header '{headers[index]}' appears more than once.");
                }
            }
        }
    }
}
