using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace SaltyGame
{
    /// <summary>
    /// Creates one deterministic identity for an ordered set of resolved upgrade
    /// snapshots. The order is part of the identity because additive upgrades
    /// can still be non-commutative once the simulation consumes them.
    /// </summary>
    public static class SpeciesUpgradeLoadoutFingerprint
    {
        public const string Version = "species-upgrade-loadout-v1";

        public static string Create(IReadOnlyList<SpeciesUpgradeSnapshot> upgrades)
        {
            var canonical = new StringBuilder(256);
            Append(canonical, Version);
            foreach (var upgrade in upgrades ?? Array.Empty<SpeciesUpgradeSnapshot>())
            {
                if (upgrade == null)
                {
                    throw new ArgumentException("Upgrade loadouts cannot contain null snapshots.", nameof(upgrades));
                }

                Append(canonical, upgrade.Id);
                Append(canonical, upgrade.Fingerprint);
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

        static void Append(StringBuilder builder, string value)
        {
            builder.Append(value?.Length ?? 0).Append(':').Append(value).Append(';');
        }
    }
}
