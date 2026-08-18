using System;
using System.Collections.Generic;

namespace SaltyGame.EditorTools.JsonCsv
{
    public sealed class ConversionResult
    {
        readonly List<string> warnings;

        public ConversionResult(string content, int rowCount, int columnCount, IEnumerable<string> warnings = null)
        {
            Content = content ?? throw new ArgumentNullException(nameof(content));
            RowCount = rowCount;
            ColumnCount = columnCount;
            this.warnings = warnings == null ? new List<string>() : new List<string>(warnings);
        }

        public string Content { get; }

        public int RowCount { get; }

        public int ColumnCount { get; }

        public IReadOnlyList<string> Warnings => warnings;
    }
}
