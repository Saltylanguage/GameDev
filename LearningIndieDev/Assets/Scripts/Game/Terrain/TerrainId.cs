using System;

namespace SaltyGame
{
    /// <summary>
    /// Stable identity for terrain definitions. Presentation and simulation
    /// behavior can evolve without changing the key stored in a cell.
    /// </summary>
    public readonly struct TerrainId : IEquatable<TerrainId>
    {
        readonly string value;

        public TerrainId(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Terrain id cannot be empty.", nameof(value));
            }

            this.value = value.Trim();
        }

        public string Value => value;
        public bool IsValid => !string.IsNullOrWhiteSpace(value);

        public bool Equals(TerrainId other)
        {
            return string.Equals(value, other.value, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            return obj is TerrainId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return StringComparer.Ordinal.GetHashCode(value ?? string.Empty);
        }

        public override string ToString()
        {
            return value ?? string.Empty;
        }

        public static bool operator ==(TerrainId left, TerrainId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TerrainId left, TerrainId right)
        {
            return !left.Equals(right);
        }
    }

    public static class TerrainIds
    {
        public static readonly TerrainId Bare = new TerrainId("bare");
        public static readonly TerrainId Grass = new TerrainId("grass");
        // Renderer-ready family for Terrain_01's second 16-tile set. It is
        // intentionally not part of the default simulation terrain registry yet.
        public static readonly TerrainId Desert = new TerrainId("desert");
    }
}
