namespace SaltyGame.EditorTools.JsonCsv
{
    public sealed class ConversionOptions
    {
        public char Delimiter { get; set; } = ',';

        public bool FlattenNestedObjects { get; set; } = true;

        public bool InferCsvValueTypes { get; set; } = true;

        public bool ParseJsonValuesInCsvCells { get; set; } = true;

        public bool WriteIndentedJson { get; set; } = true;
    }
}
