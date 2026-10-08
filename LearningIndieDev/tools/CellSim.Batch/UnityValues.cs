using System;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

// Value/serialization adapter for the linked production sources, compiled ONLY by
// CellSim.Batch. No Unity lifecycle, assets, physics, or engine calls are emulated.
namespace UnityEngine
{
    public struct Vector2Int : IEquatable<Vector2Int>
    {
        public int x;
        public int y;
        public Vector2Int(int x, int y) { this.x = x; this.y = y; }
        public static Vector2Int zero => new Vector2Int(0, 0);
        public static Vector2Int up => new Vector2Int(0, 1);
        public static Vector2Int down => new Vector2Int(0, -1);
        public static Vector2Int left => new Vector2Int(-1, 0);
        public static Vector2Int right => new Vector2Int(1, 0);
        public bool Equals(Vector2Int other) => x == other.x && y == other.y;
        public override bool Equals(object other) => other is Vector2Int value && Equals(value);
        public override int GetHashCode() => x.GetHashCode() ^ (y.GetHashCode() << 2);
        public static bool operator ==(Vector2Int a, Vector2Int b) => a.Equals(b);
        public static bool operator !=(Vector2Int a, Vector2Int b) => !a.Equals(b);
    }

    public struct Color
    {
        public float r, g, b, a;
        public Color(float r, float g, float b, float a = 1f)
        { this.r = r; this.g = g; this.b = b; this.a = a; }
    }

    public sealed class SerializeField : Attribute { }
    public sealed class MinAttribute : Attribute { public MinAttribute(float value) { } }
    public sealed class TextAreaAttribute : Attribute { }
    public sealed class TooltipAttribute : Attribute { public TooltipAttribute(string value) { } }
    public sealed class CreateAssetMenuAttribute : Attribute
    { public string menuName; public string fileName; }
    public class ScriptableObject
    {
        protected ScriptableObject() => throw new PlatformNotSupportedException("Use exported data; Unity assets cannot be created in CellSim.Batch.");
        public string name => throw new PlatformNotSupportedException("Unity asset access requires the Editor.");
    }
    public static class Debug
    {
        // Tracked transitions remain in metrics. Bulk runs avoid per-event console I/O.
        public static void Log(object value) { }
    }

    public static class JsonUtility
    {
        public static readonly JsonSerializerOptions Options = CreateOptions();
        static JsonSerializerOptions CreateOptions()
        {
            var resolver = new DefaultJsonTypeInfoResolver();
            resolver.Modifiers.Add(info =>
            {
                for (var i = info.Properties.Count - 1; i >= 0; i--)
                    if (info.Properties[i].AttributeProvider is System.Reflection.PropertyInfo)
                        info.Properties.RemoveAt(i);
            });
            return new JsonSerializerOptions { IncludeFields = true, TypeInfoResolver = resolver };
        }
        public static string ToJson(object value, bool pretty = false)
        {
            var options = new JsonSerializerOptions(Options) { WriteIndented = pretty };
            return JsonSerializer.Serialize(value, value.GetType(), options);
        }
        public static T FromJson<T>(string value) => JsonSerializer.Deserialize<T>(value, Options);
    }
}
