using System;
using System.Collections.Generic;
using System.Text;

namespace SaltyGame.EditorTools.JsonCsv
{
    public static class CsvWriter
    {
        const string LineEnding = "\r\n";

        public static string Write(IEnumerable<IEnumerable<string>> rows, char delimiter = ',')
        {
            if (rows == null)
            {
                throw new ArgumentNullException(nameof(rows));
            }

            ValidateDelimiter(delimiter);

            var output = new StringBuilder();
            var firstRow = true;
            foreach (var row in rows)
            {
                if (!firstRow)
                {
                    output.Append(LineEnding);
                }

                WriteRow(output, row, delimiter);
                firstRow = false;
            }

            return output.ToString();
        }

        static void WriteRow(StringBuilder output, IEnumerable<string> row, char delimiter)
        {
            if (row == null)
            {
                throw new ArgumentException("CSV rows cannot be null.", nameof(row));
            }

            var firstField = true;
            foreach (var value in row)
            {
                if (!firstField)
                {
                    output.Append(delimiter);
                }

                AppendField(output, value ?? string.Empty, delimiter);
                firstField = false;
            }
        }

        static void AppendField(StringBuilder output, string value, char delimiter)
        {
            var requiresQuotes = value.IndexOf(delimiter) >= 0
                || value.IndexOf('"') >= 0
                || value.IndexOf('\r') >= 0
                || value.IndexOf('\n') >= 0;

            if (!requiresQuotes)
            {
                output.Append(value);
                return;
            }

            output.Append('"');
            output.Append(value.Replace("\"", "\"\""));
            output.Append('"');
        }

        static void ValidateDelimiter(char delimiter)
        {
            if (delimiter == '"' || delimiter == '\r' || delimiter == '\n')
            {
                throw new ArgumentException("The CSV delimiter cannot be a quote or line break.", nameof(delimiter));
            }
        }
    }
}
