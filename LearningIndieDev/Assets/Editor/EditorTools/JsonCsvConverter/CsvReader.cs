using System;
using System.Collections.Generic;
using System.Text;

namespace SaltyGame.EditorTools.JsonCsv
{
    public static class CsvReader
    {
        public static IReadOnlyList<IReadOnlyList<string>> Read(string csv, char delimiter = ',')
        {
            if (csv == null)
            {
                throw new ArgumentNullException(nameof(csv));
            }

            ValidateDelimiter(delimiter);

            var rows = new List<IReadOnlyList<string>>();
            var row = new List<string>();
            var field = new StringBuilder();
            var insideQuotes = false;
            var fieldStarted = false;
            var quotedFieldClosed = false;

            for (var index = 0; index < csv.Length; index++)
            {
                var character = csv[index];
                if (insideQuotes)
                {
                    if (character != '"')
                    {
                        field.Append(character);
                        continue;
                    }

                    if (index + 1 < csv.Length && csv[index + 1] == '"')
                    {
                        field.Append('"');
                        index++;
                    }
                    else
                    {
                        insideQuotes = false;
                        quotedFieldClosed = true;
                    }

                    continue;
                }

                if (quotedFieldClosed && character != delimiter && character != '\r' && character != '\n')
                {
                    throw CreateFormatException("Unexpected content followed a closing quote", rows.Count + 1, row.Count + 1);
                }

                if (character == '"')
                {
                    if (fieldStarted || field.Length > 0)
                    {
                        throw CreateFormatException("A quote appeared inside an unquoted field", rows.Count + 1, row.Count + 1);
                    }

                    insideQuotes = true;
                    fieldStarted = true;
                    quotedFieldClosed = false;
                }
                else if (character == delimiter)
                {
                    AddField(row, field);
                    fieldStarted = false;
                    quotedFieldClosed = false;
                }
                else if (character == '\r' || character == '\n')
                {
                    AddField(row, field);
                    rows.Add(row);
                    row = new List<string>();
                    fieldStarted = false;
                    quotedFieldClosed = false;

                    if (character == '\r' && index + 1 < csv.Length && csv[index + 1] == '\n')
                    {
                        index++;
                    }
                }
                else
                {
                    field.Append(character);
                    fieldStarted = true;
                }
            }

            if (insideQuotes)
            {
                throw CreateFormatException("The final quoted field was not closed", rows.Count + 1, row.Count + 1);
            }

            if (fieldStarted || field.Length > 0 || row.Count > 0)
            {
                AddField(row, field);
                rows.Add(row);
            }

            return rows;
        }

        static void AddField(List<string> row, StringBuilder field)
        {
            row.Add(field.ToString());
            field.Clear();
        }

        static void ValidateDelimiter(char delimiter)
        {
            if (delimiter == '"' || delimiter == '\r' || delimiter == '\n')
            {
                throw new ArgumentException("The CSV delimiter cannot be a quote or line break.", nameof(delimiter));
            }
        }

        static FormatException CreateFormatException(string message, int row, int column)
        {
            return new FormatException($"{message} at CSV row {row}, column {column}.");
        }
    }
}
