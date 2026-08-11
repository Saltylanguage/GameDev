using System;
using System.Collections.Generic;

namespace SaltyGame
{
    /// <summary>
    /// Stable, data-facing identity for a species. Display names and rules may
    /// change without changing this key.
    /// </summary>
    public readonly struct SpeciesId : IEquatable<SpeciesId>
    {
        readonly string value;

        public SpeciesId(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Species id cannot be empty.", nameof(value));
            }

            this.value = value.Trim();
        }

        public string Value => value;
        public bool IsValid => !string.IsNullOrWhiteSpace(value);

        public bool Equals(SpeciesId other)
        {
            return string.Equals(value, other.value, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            return obj is SpeciesId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return StringComparer.Ordinal.GetHashCode(value ?? string.Empty);
        }

        public override string ToString()
        {
            return value ?? string.Empty;
        }

        public static bool operator ==(SpeciesId left, SpeciesId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SpeciesId left, SpeciesId right)
        {
            return !left.Equals(right);
        }

        [Obsolete("Use SpeciesId values instead of SpeciesArchetype.")]
        public static implicit operator SpeciesId(SpeciesArchetype archetype)
        {
            switch (archetype)
            {
                case SpeciesArchetype.Plant:
                    return SpeciesIds.Plant;
                case SpeciesArchetype.Herbivore:
                    return SpeciesIds.Herbivore;
                case SpeciesArchetype.Carnivore:
                    return SpeciesIds.Carnivore;
                default:
                    throw new ArgumentOutOfRangeException(nameof(archetype), archetype, "Unknown legacy species archetype.");
            }
        }

        public static SpeciesArchetype ToLegacyArchetype(SpeciesId species)
        {
            if (species == SpeciesIds.Plant)
            {
                return SpeciesArchetype.Plant;
            }

            if (species == SpeciesIds.Herbivore)
            {
                return SpeciesArchetype.Herbivore;
            }

            if (species == SpeciesIds.Carnivore)
            {
                return SpeciesArchetype.Carnivore;
            }

            throw new InvalidOperationException($"Species id '{species}' has no legacy archetype value.");
        }
    }

    public static class SpeciesIds
    {
        public static readonly SpeciesId Plant = new SpeciesId("plant");
        public static readonly SpeciesId Herbivore = new SpeciesId("herbivore");
        public static readonly SpeciesId Carnivore = new SpeciesId("carnivore");
    }

    public static class SpeciesIdConversions
    {
        public static Dictionary<SpeciesId, TValue> FromLegacy<TValue>(
            IReadOnlyDictionary<SpeciesArchetype, TValue> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var converted = new Dictionary<SpeciesId, TValue>(source.Count);
            foreach (var entry in source)
            {
                converted[(SpeciesId)entry.Key] = entry.Value;
            }

            return converted;
        }
    }
}
