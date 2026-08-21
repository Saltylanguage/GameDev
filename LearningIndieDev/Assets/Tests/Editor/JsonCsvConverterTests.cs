using System;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using SaltyGame.EditorTools.JsonCsv;

namespace SaltyGame.EditorTests
{
    public sealed class JsonCsvConverterTests
    {
        [Test]
        public void JsonToCsv_FlatObjects_WritesDeterministicColumns()
        {
            const string json = "[{\"name\":\"hare\",\"age\":2},{\"age\":3,\"name\":\"fox\"}]";

            var result = JsonCsvConverter.ConvertJsonToCsv(json);

            Assert.That(result.Content, Is.EqualTo("age,name\r\n2,hare\r\n3,fox"));
            Assert.That(result.RowCount, Is.EqualTo(2));
            Assert.That(result.ColumnCount, Is.EqualTo(2));
        }

        [Test]
        public void CsvToJson_FlatRows_InfersPrimitiveTypes()
        {
            const string csv = "name,age,active,ratio\r\nhare,2,true,1.5";

            var result = JsonCsvConverter.ConvertCsvToJson(csv);
            var row = (JObject)JArray.Parse(result.Content)[0];

            Assert.That(row.Value<string>("name"), Is.EqualTo("hare"));
            Assert.That(row.Value<long>("age"), Is.EqualTo(2));
            Assert.That(row.Value<bool>("active"), Is.True);
            Assert.That(row.Value<decimal>("ratio"), Is.EqualTo(1.5m));
        }

        [Test]
        public void JsonToCsv_DifferentProperties_UsesUnionOfColumns()
        {
            const string json = "[{\"name\":\"hare\"},{\"energy\":5}]";

            var result = JsonCsvConverter.ConvertJsonToCsv(json);

            Assert.That(result.Content, Is.EqualTo("energy,name\r\n,hare\r\n5,"));
        }

        [Test]
        public void JsonToCsv_QuotedAndMultilineStrings_EscapesCsvFields()
        {
            const string json = "[{\"note\":\"fast, \\\"alert\\\"\\nnow\"}]";

            var result = JsonCsvConverter.ConvertJsonToCsv(json);

            Assert.That(result.Content, Is.EqualTo("note\r\n\"fast, \"\"alert\"\"\nnow\""));
        }

        [Test]
        public void CsvToJson_QuotedAndMultilineFields_RestoresText()
        {
            const string csv = "note\r\n\"fast, \"\"alert\"\"\r\nnow\"";

            var result = JsonCsvConverter.ConvertCsvToJson(csv);
            var row = (JObject)JArray.Parse(result.Content)[0];

            Assert.That(row.Value<string>("note"), Is.EqualTo("fast, \"alert\"\r\nnow"));
        }

        [Test]
        public void JsonToCsv_UnicodeText_PreservesCharacters()
        {
            const string json = "[{\"name\":\"lièvre 🐇\"}]";

            var result = JsonCsvConverter.ConvertJsonToCsv(json);

            Assert.That(result.Content, Does.Contain("lièvre 🐇"));
        }

        [Test]
        public void RoundTrip_IsoFormattedString_DoesNotConvertItToDate()
        {
            const string json = "[{\"created\":\"2026-08-18T12:34:56Z\"}]";

            var csv = JsonCsvConverter.ConvertJsonToCsv(json).Content;
            var roundTrippedJson = JsonCsvConverter.ConvertCsvToJson(csv).Content;

            Assert.That(csv, Is.EqualTo("created\r\n2026-08-18T12:34:56Z"));
            Assert.That(roundTrippedJson, Does.Contain("\"created\": \"2026-08-18T12:34:56Z\""));
        }

        [Test]
        public void JsonToCsv_NestedObject_FlattensToEscapedPath()
        {
            const string json = "[{\"species\":{\"stats\":{\"energy\":45}}}]";

            var result = JsonCsvConverter.ConvertJsonToCsv(json);

            Assert.That(result.Content, Is.EqualTo("species.stats.energy\r\n45"));
        }

        [Test]
        public void CsvToJson_FlattenedPath_ExpandsNestedObjects()
        {
            const string csv = "species.stats.energy\r\n45";

            var result = JsonCsvConverter.ConvertCsvToJson(csv);
            var row = (JObject)JArray.Parse(result.Content)[0];

            Assert.That(row.SelectToken("species.stats.energy").Value<long>(), Is.EqualTo(45));
        }

        [Test]
        public void RoundTrip_PropertyContainingDotAndBackslash_PreservesPropertyName()
        {
            const string json = "[{\"a.b\\\\c\":\"value\"}]";

            var csv = JsonCsvConverter.ConvertJsonToCsv(json).Content;
            var roundTrippedJson = JsonCsvConverter.ConvertCsvToJson(csv).Content;
            var row = (JObject)JArray.Parse(roundTrippedJson)[0];

            Assert.That(row.Value<string>("a.b\\c"), Is.EqualTo("value"));
        }

        [Test]
        public void RoundTrip_ArrayValue_PreservesArray()
        {
            const string json = "[{\"name\":\"hare\",\"diet\":[\"fern\",\"grass\"]}]";

            var csv = JsonCsvConverter.ConvertJsonToCsv(json).Content;
            var roundTrippedJson = JsonCsvConverter.ConvertCsvToJson(csv).Content;
            var diet = (JArray)JArray.Parse(roundTrippedJson)[0]["diet"];

            Assert.That(diet.Values<string>(), Is.EqualTo(new[] { "fern", "grass" }));
        }

        [Test]
        public void JsonToCsv_FlatteningDisabled_StoresNestedObjectInCell()
        {
            const string json = "[{\"stats\":{\"energy\":45}}]";
            var options = new ConversionOptions { FlattenNestedObjects = false };

            var result = JsonCsvConverter.ConvertJsonToCsv(json, options);

            Assert.That(result.Content, Is.EqualTo("stats\r\n\"{\"\"energy\"\":45}\""));
        }

        [Test]
        public void CsvToJson_TypeInferenceDisabled_PreservesStrings()
        {
            const string csv = "value\r\n0012";
            var options = new ConversionOptions { InferCsvValueTypes = false };

            var result = JsonCsvConverter.ConvertCsvToJson(csv, options);
            var row = (JObject)JArray.Parse(result.Content)[0];

            Assert.That(row.Value<string>("value"), Is.EqualTo("0012"));
        }

        [Test]
        public void CsvToJson_EmptyAndMissingFields_BecomeNullAndReportMissingField()
        {
            const string csv = "name,energy\r\nhare,";

            var result = JsonCsvConverter.ConvertCsvToJson(csv);
            var row = (JObject)JArray.Parse(result.Content)[0];

            Assert.That(row["energy"].Type, Is.EqualTo(JTokenType.Null));
            Assert.That(result.Warnings, Is.Empty);

            var missingResult = JsonCsvConverter.ConvertCsvToJson("name,energy\r\nhare");
            Assert.That(missingResult.Warnings, Has.Count.EqualTo(1));
        }

        [Test]
        public void Converters_CustomDelimiter_UsesSpecifiedCharacter()
        {
            var options = new ConversionOptions { Delimiter = ';' };

            var csv = JsonCsvConverter.ConvertJsonToCsv("[{\"a\":1,\"b\":2}]", options).Content;
            var json = JsonCsvConverter.ConvertCsvToJson(csv, options).Content;

            Assert.That(csv, Is.EqualTo("a;b\r\n1;2"));
            Assert.That(JArray.Parse(json)[0]["b"].Value<long>(), Is.EqualTo(2));
        }

        [Test]
        public void CsvToJson_Utf8BomBeforeFirstHeader_IgnoresBom()
        {
            const string csv = "\uFEFFname\r\nhare";

            var result = JsonCsvConverter.ConvertCsvToJson(csv);
            var row = (JObject)JArray.Parse(result.Content)[0];

            Assert.That(row.Property("name"), Is.Not.Null);
        }

        [Test]
        public void JsonToCsv_NonArrayRoot_ThrowsActionableError()
        {
            var exception = Assert.Throws<FormatException>(
                () => JsonCsvConverter.ConvertJsonToCsv("{\"name\":\"hare\"}"));

            Assert.That(exception.Message, Does.Contain("root must be an array"));
        }

        [Test]
        public void JsonToCsv_MalformedJson_ReportsLineAndPosition()
        {
            var exception = Assert.Throws<FormatException>(
                () => JsonCsvConverter.ConvertJsonToCsv("[{\"name\":}]"));

            Assert.That(exception.Message, Does.Contain("Invalid JSON at line"));
            Assert.That(exception.Message, Does.Contain("position"));
        }

        [Test]
        public void JsonToCsv_NonObjectArrayItem_ThrowsActionableError()
        {
            var exception = Assert.Throws<FormatException>(
                () => JsonCsvConverter.ConvertJsonToCsv("[{\"name\":\"hare\"},2]"));

            Assert.That(exception.Message, Does.Contain("array item 1"));
        }

        [Test]
        public void JsonToCsv_EmptyObjectArray_ThrowsBecauseNoColumnsExist()
        {
            var exception = Assert.Throws<FormatException>(
                () => JsonCsvConverter.ConvertJsonToCsv("[{}]"));

            Assert.That(exception.Message, Does.Contain("properties"));
        }

        [Test]
        public void CsvReader_UnclosedQuotedField_ReportsLocation()
        {
            var exception = Assert.Throws<FormatException>(() => CsvReader.Read("name\r\n\"hare"));

            Assert.That(exception.Message, Does.Contain("row 2, column 1"));
        }

        [Test]
        public void CsvReader_ContentAfterClosingQuote_Throws()
        {
            Assert.Throws<FormatException>(() => CsvReader.Read("name\r\n\"hare\"unexpected"));
        }

        [Test]
        public void CsvToJson_TooManyFields_ReportsRowCounts()
        {
            var exception = Assert.Throws<FormatException>(
                () => JsonCsvConverter.ConvertCsvToJson("name\r\nhare,extra"));

            Assert.That(exception.Message, Does.Contain("row 2 contains 2 fields"));
        }

        [Test]
        public void CsvToJson_ConflictingPaths_Throws()
        {
            var exception = Assert.Throws<FormatException>(
                () => JsonCsvConverter.ConvertCsvToJson("species,species.name\r\nhare,Hazel"));

            Assert.That(exception.Message, Does.Contain("conflicts"));
        }
    }
}
