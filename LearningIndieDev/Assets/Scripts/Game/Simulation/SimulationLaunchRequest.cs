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

        public SimulationLaunchRequest(
            string profileId,
            string scenarioId,
            string playerSpeciesId,
            int seed,
            IEnumerable<string> orderedUpgradeIds = null,
            string rulesetFingerprint = "")
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

            this.orderedUpgradeIds = copiedUpgradeIds.AsReadOnly();
        }

        public string ProfileId { get; }
        public string ScenarioId { get; }
        public string PlayerSpeciesId { get; }
        public int Seed { get; }
        public IReadOnlyList<string> OrderedUpgradeIds => orderedUpgradeIds;
        public string RulesetFingerprint { get; }
    }
}
