using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace SaltyGame
{
    public enum SpeciesUpgradeScope
    {
        PerRun,
    }

    public readonly struct SpeciesUpgradeModifier
    {
        public SpeciesUpgradeModifier(string attributeId, float signedValue)
        {
            if (string.IsNullOrWhiteSpace(attributeId))
            {
                throw new ArgumentException("Attribute id cannot be empty.", nameof(attributeId));
            }

            if (float.IsNaN(signedValue) || float.IsInfinity(signedValue) || signedValue == 0f)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(signedValue),
                    signedValue,
                    "Modifier value must be finite and non-zero.");
            }

            AttributeId = attributeId.Trim();
            SignedValue = signedValue;
        }

        public string AttributeId { get; }
        public float SignedValue { get; }
    }

    public sealed class SpeciesUpgradeSnapshot
    {
        public const string ContractVersion = "species-upgrade-v1";

        readonly IReadOnlyList<SpeciesUpgradeModifier> modifiers;
        readonly IReadOnlyList<string> prerequisiteUpgradeIds;
        readonly IReadOnlyList<string> excludedUpgradeIds;

        public SpeciesUpgradeSnapshot(
            string id,
            string displayName,
            string description,
            SpeciesId targetSpecies,
            int cost,
            IEnumerable<SpeciesUpgradeModifier> modifiers,
            IEnumerable<string> prerequisiteUpgradeIds = null,
            IEnumerable<string> excludedUpgradeIds = null)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Upgrade id cannot be empty.", nameof(id));
            }

            if (string.IsNullOrWhiteSpace(displayName))
            {
                throw new ArgumentException("Upgrade display name cannot be empty.", nameof(displayName));
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentException("Upgrade description cannot be empty.", nameof(description));
            }

            if (!targetSpecies.IsValid)
            {
                throw new ArgumentException("Target species id is required.", nameof(targetSpecies));
            }

            if (cost < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(cost), cost, "Upgrade cost cannot be negative.");
            }

            if (modifiers == null)
            {
                throw new ArgumentNullException(nameof(modifiers));
            }

            var copiedModifiers = new List<SpeciesUpgradeModifier>();
            var seenAttributeIds = new HashSet<string>(StringComparer.Ordinal);
            foreach (var modifier in modifiers)
            {
                SpeciesAttributeRegistry.ValidateModifier(modifier);

                if (!seenAttributeIds.Add(modifier.AttributeId))
                {
                    throw new ArgumentException(
                        $"Attribute id '{modifier.AttributeId}' may only appear once in an upgrade.",
                        nameof(modifiers));
                }

                copiedModifiers.Add(modifier);
            }

            if (copiedModifiers.Count == 0)
            {
                throw new ArgumentException("An upgrade must contain at least one modifier.", nameof(modifiers));
            }

            Id = id.Trim();
            DisplayName = displayName.Trim();
            Description = description.Trim();
            TargetSpecies = targetSpecies;
            Cost = cost;
            Scope = SpeciesUpgradeScope.PerRun;
            this.modifiers = new ReadOnlyCollection<SpeciesUpgradeModifier>(copiedModifiers);
            this.prerequisiteUpgradeIds = CopyIds(prerequisiteUpgradeIds, nameof(prerequisiteUpgradeIds));
            this.excludedUpgradeIds = CopyIds(excludedUpgradeIds, nameof(excludedUpgradeIds));
            ValidateUpgradeRelationships();
            Fingerprint = CreateFingerprint();
        }

        public string Id { get; }
        public string DisplayName { get; }
        public string Description { get; }
        public SpeciesId TargetSpecies { get; }
        public int Cost { get; }
        public SpeciesUpgradeScope Scope { get; }
        public IReadOnlyList<SpeciesUpgradeModifier> Modifiers => modifiers;
        public IReadOnlyList<string> PrerequisiteUpgradeIds => prerequisiteUpgradeIds;
        public IReadOnlyList<string> ExcludedUpgradeIds => excludedUpgradeIds;
        public string RegistryFingerprint => SpeciesAttributeRegistry.Fingerprint;
        public string Fingerprint { get; }

        public SpeciesRules Apply(SpeciesRules rules)
        {
            if (rules == null)
            {
                throw new ArgumentNullException(nameof(rules));
            }

            var result = rules;
            foreach (var modifier in modifiers)
            {
                result = SpeciesAttributeRegistry.Apply(result, modifier);
            }

            return result;
        }

        string CreateFingerprint()
        {
            var canonical = new StringBuilder(256);
            Append(canonical, ContractVersion);
            Append(canonical, Id);
            Append(canonical, DisplayName);
            Append(canonical, Description);
            Append(canonical, TargetSpecies.Value);
            Append(canonical, (int)Scope);
            Append(canonical, Cost);
            Append(canonical, RegistryFingerprint);
            foreach (var modifier in modifiers)
            {
                Append(canonical, modifier.AttributeId);
                Append(canonical, modifier.SignedValue);
            }

            foreach (var prerequisiteId in prerequisiteUpgradeIds)
            {
                Append(canonical, prerequisiteId);
            }

            foreach (var excludedId in excludedUpgradeIds)
            {
                Append(canonical, excludedId);
            }

            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(canonical.ToString()));
                var result = new StringBuilder(bytes.Length * 2);
                foreach (var value in bytes)
                {
                    result.Append(value.ToString("x2", CultureInfo.InvariantCulture));
                }

                return result.ToString();
            }
        }

        void ValidateUpgradeRelationships()
        {
            var excluded = new HashSet<string>(excludedUpgradeIds, StringComparer.Ordinal);
            foreach (var prerequisiteId in prerequisiteUpgradeIds)
            {
                if (string.Equals(prerequisiteId, Id, StringComparison.Ordinal))
                {
                    throw new ArgumentException(
                        $"Upgrade '{Id}' cannot require itself.",
                        nameof(prerequisiteUpgradeIds));
                }

                if (excluded.Contains(prerequisiteId))
                {
                    throw new ArgumentException(
                        $"Upgrade '{Id}' cannot both require and exclude '{prerequisiteId}'.",
                        nameof(prerequisiteUpgradeIds));
                }
            }

            if (excluded.Contains(Id))
            {
                throw new ArgumentException(
                    $"Upgrade '{Id}' cannot exclude itself.",
                    nameof(excludedUpgradeIds));
            }
        }

        static IReadOnlyList<string> CopyIds(IEnumerable<string> ids, string parameterName)
        {
            var copied = new List<string>();
            var seen = new HashSet<string>(StringComparer.Ordinal);
            if (ids != null)
            {
                foreach (var id in ids)
                {
                    if (string.IsNullOrWhiteSpace(id))
                    {
                        throw new ArgumentException("Upgrade ids cannot be empty.", parameterName);
                    }

                    var normalized = id.Trim();
                    if (!seen.Add(normalized))
                    {
                        throw new ArgumentException(
                            $"Upgrade id '{normalized}' may only appear once.",
                            parameterName);
                    }

                    copied.Add(normalized);
                }
            }

            return new ReadOnlyCollection<string>(copied);
        }

        static void Append(StringBuilder builder, string value)
        {
            builder.Append(value?.Length ?? 0).Append(':').Append(value).Append(';');
        }

        static void Append(StringBuilder builder, int value)
        {
            Append(builder, value.ToString(CultureInfo.InvariantCulture));
        }

        static void Append(StringBuilder builder, float value)
        {
            Append(builder, value.ToString("R", CultureInfo.InvariantCulture));
        }
    }
}
