using System.Collections.Generic;
using SaltyGame;

namespace SaltyGame.EditorTools
{
    static class SimulationReportSerialization
    {
        public static SimulationSpeciesActivityRecord[] CreateActivity(
            SpeciesSimulationMetrics metrics,
            IReadOnlyList<SpeciesId> species)
        {
            var activity = new SimulationSpeciesActivityRecord[species.Count];
            for (var index = 0; index < species.Count; index++)
            {
                var source = metrics.GetActivity(species[index]);
                activity[index] = new SimulationSpeciesActivityRecord
                {
                    speciesId = species[index].Value,
                    births = source.Births,
                    foodConsumed = source.FoodConsumed,
                    movementSteps = source.MovementSteps,
                    damageDealt = source.DamageDealt,
                    combatKills = source.CombatKills,
                    deaths = source.Deaths,
                    starvationDeaths = source.StarvationDeaths,
                    crowdingDeaths = source.CrowdingDeaths,
                    wiltDeaths = source.WiltDeaths,
                    populationLimitRemovals = source.PopulationLimitRemovals,
                };
            }

            return activity;
        }

        public static SimulationPopulationSnapshotRecord[] CreatePopulationHistory(
            IReadOnlyList<SpeciesPopulationSnapshot> populationHistory,
            IReadOnlyList<SpeciesId> species)
        {
            var snapshots = new SimulationPopulationSnapshotRecord[populationHistory.Count];
            for (var index = 0; index < snapshots.Length; index++)
            {
                var source = populationHistory[index];
                var counts = new SimulationSpeciesPopulationRecord[species.Count];
                for (var speciesIndex = 0; speciesIndex < species.Count; speciesIndex++)
                {
                    var speciesId = species[speciesIndex];
                    counts[speciesIndex] = new SimulationSpeciesPopulationRecord
                    {
                        speciesId = speciesId.Value,
                        population = source.GetCount(speciesId),
                    };
                }

                snapshots[index] = new SimulationPopulationSnapshotRecord
                {
                    tick = source.Tick,
                    empty = source.Empty,
                    species = counts,
                };
            }

            return snapshots;
        }

        public static List<SpeciesId> GetSpecies(IReadOnlyList<SpeciesPopulationSnapshot> history)
        {
            var species = new List<SpeciesId>();
            for (var index = 0; index < history.Count; index++)
            {
                foreach (var entry in history[index].Counts)
                {
                    if (!species.Contains(entry.Key))
                    {
                        species.Add(entry.Key);
                    }
                }
            }

            species.Sort((left, right) => string.CompareOrdinal(left.Value, right.Value));
            return species;
        }
    }

    [System.Serializable]
    sealed class SimulationPopulationSnapshotRecord
    {
        public int tick;
        public int empty;
        public SimulationSpeciesPopulationRecord[] species;
    }

    [System.Serializable]
    sealed class SimulationSpeciesPopulationRecord
    {
        public string speciesId;
        public int population;
    }

    [System.Serializable]
    sealed class SimulationSpeciesActivityRecord
    {
        public string speciesId;
        public int births;
        public float foodConsumed;
        public int movementSteps;
        public int damageDealt;
        public int combatKills;
        public int deaths;
        public int starvationDeaths;
        public int crowdingDeaths;
        public int wiltDeaths;
        public int populationLimitRemovals;
    }
}
