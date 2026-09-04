using System;
using System.Collections.Generic;

namespace SaltyGame
{
    /// <summary>
    /// Immutable input copied at the Lab boundary and consumed by one Simulation
    /// scene. It contains identifiers and values only; no scene objects cross the
    /// scene unload.
    /// </summary>
    public sealed class SimulationLaunchRequest
    {
        readonly IReadOnlyList<string> orderedUpgradeIds;
        readonly IReadOnlyList<SpeciesUpgradeSnapshot> orderedUpgradeSnapshots;

        public SimulationLaunchRequest(
            string profileId,
            string scenarioId,
            string playerSpeciesId,
            int seed,
            IEnumerable<string> orderedUpgradeIds = null,
            string rulesetFingerprint = "",
            IEnumerable<SpeciesUpgradeSnapshot> orderedUpgradeSnapshots = null)
        {
            if (string.IsNullOrWhiteSpace(profileId))
            {
                throw new ArgumentException("A profile id is required.", nameof(profileId));
            }

            if (string.IsNullOrWhiteSpace(scenarioId))
            {
                throw new ArgumentException("A scenario id is required.", nameof(scenarioId));
            }

            if (string.IsNullOrWhiteSpace(playerSpeciesId))
            {
                throw new ArgumentException("A player species id is required.", nameof(playerSpeciesId));
            }

            ProfileId = profileId.Trim();
            ScenarioId = scenarioId.Trim();
            PlayerSpeciesId = playerSpeciesId.Trim();
            Seed = seed;
            RulesetFingerprint = rulesetFingerprint ?? string.Empty;

            var copiedUpgradeIds = new List<string>();
            if (orderedUpgradeIds != null)
            {
                foreach (var upgradeId in orderedUpgradeIds)
                {
                    if (!string.IsNullOrWhiteSpace(upgradeId))
                    {
                        copiedUpgradeIds.Add(upgradeId.Trim());
                    }
                }
            }

            var copiedUpgradeSnapshots = new List<SpeciesUpgradeSnapshot>();
            if (orderedUpgradeSnapshots != null)
            {
                foreach (var upgrade in orderedUpgradeSnapshots)
                {
                    if (upgrade == null)
                    {
                        throw new ArgumentException("Upgrade snapshots cannot be null.", nameof(orderedUpgradeSnapshots));
                    }

                    copiedUpgradeSnapshots.Add(upgrade);
                }
            }

            // Snapshot-backed callers do not need to duplicate the stable IDs;
            // expose the canonical ordered IDs for legacy consumers as well.
            if (copiedUpgradeIds.Count == 0)
            {
                foreach (var upgrade in copiedUpgradeSnapshots)
                {
                    copiedUpgradeIds.Add(upgrade.Id);
                }
            }
            else if (copiedUpgradeSnapshots.Count > 0)
            {
                if (copiedUpgradeIds.Count != copiedUpgradeSnapshots.Count)
                {
                    throw new ArgumentException(
                        "Ordered upgrade ids must match the ordered snapshot count.",
                        nameof(orderedUpgradeIds));
                }

                for (var index = 0; index < copiedUpgradeIds.Count; index++)
                {
                    if (!string.Equals(copiedUpgradeIds[index], copiedUpgradeSnapshots[index].Id, StringComparison.Ordinal))
                    {
                        throw new ArgumentException(
                            "Ordered upgrade ids must match snapshot ids in the same order.",
                            nameof(orderedUpgradeIds));
                    }
                }
            }

            this.orderedUpgradeIds = copiedUpgradeIds.AsReadOnly();
            this.orderedUpgradeSnapshots = copiedUpgradeSnapshots.AsReadOnly();
        }

        public string ProfileId { get; }
        public string ScenarioId { get; }
        public string PlayerSpeciesId { get; }
        public int Seed { get; }
        public IReadOnlyList<string> OrderedUpgradeIds => orderedUpgradeIds;
        public IReadOnlyList<SpeciesUpgradeSnapshot> OrderedUpgradeSnapshots => orderedUpgradeSnapshots;
        public string RulesetFingerprint { get; }
    }
}
