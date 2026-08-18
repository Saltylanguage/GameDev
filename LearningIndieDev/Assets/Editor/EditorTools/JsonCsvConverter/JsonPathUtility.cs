using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace SaltyGame.EditorTools.JsonCsv
{
    public static class JsonPathUtility
    {
        const char Separator = '.';
        const char EscapeCharacter = '\\';

        public static string Combine(string parentPath, string propertyName)
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            var escapedName = propertyName
                .Replace(EscapeCharacter.ToString(), new string(EscapeCharacter, 2))
                .Replace(Separator.ToString(), $"{EscapeCharacter}{Separator}");
            return string.IsNullOrEmpty(parentPath) ? escapedName : $"{parentPath}{Separator}{escapedName}";
        }

        public static IReadOnlyList<string> Split(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("A JSON property path cannot be empty.", nameof(path));
            }

            var segments = new List<string>();
            var segment = new StringBuilder();
            var escaping = false;

            foreach (var character in path)
            {
                if (escaping)
                {
                    segment.Append(character);
                    escaping = false;
                }
                else if (character == EscapeCharacter)
                {
                    escaping = true;
                }
                else if (character == Separator)
                {
                    AddSegment(segments, segment, path);
                }
                else
                {
                    segment.Append(character);
                }
            }

            if (escaping)
            {
                throw new FormatException($"JSON property path '{path}' ends with an escape character.");
            }

            AddSegment(segments, segment, path);
            return segments;
        }

        public static void SetValue(JObject target, string path, JToken value)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            var segments = Split(path);
            var current = target;
            for (var index = 0; index < segments.Count - 1; index++)
            {
                var segment = segments[index];
                if (current.TryGetValue(segment, out var existing) && existing.Type != JTokenType.Object)
                {
                    throw new FormatException($"JSON path '{path}' conflicts with the value at '{segment}'.");
                }

                if (existing == null)
                {
                    existing = new JObject();
                    current.Add(segment, existing);
                }

                current = (JObject)existing;
            }

            var leafName = segments[segments.Count - 1];
            if (current.ContainsKey(leafName))
            {
                throw new FormatException($"JSON path '{path}' appears more than once in the CSV header.");
            }

            current.Add(leafName, value);
        }

        static void AddSegment(List<string> segments, StringBuilder segment, string path)
        {
            if (segment.Length == 0)
            {
                throw new FormatException($"JSON property path '{path}' contains an empty segment.");
            }

            segments.Add(segment.ToString());
            segment.Clear();
        }
    }
}
