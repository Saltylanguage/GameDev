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
                var reproduction = metrics.GetReproductionActivity(species[index]);
                activity[index] = new SimulationSpeciesActivityRecord
                {
                    speciesId = species[index].Value,
                    births = source.Births,
                    foodConsumed = source.FoodConsumed,
                    foodActionAttempts = source.FoodActionAttempts,
                    foodActionSuccesses = source.FoodActionSuccesses,
                    foodActionFailures = source.FoodActionFailures,
                    movementSteps = source.MovementSteps,
                    damageDealt = source.DamageDealt,
                    combatKills = source.CombatKills,
                    deaths = source.Deaths,
                    starvationDeaths = source.StarvationDeaths,
                    crowdingDeaths = source.CrowdingDeaths,
                    wiltDeaths = source.WiltDeaths,
                    populationLimitRemovals = source.PopulationLimitRemovals,
                    stateTransitions = metrics.GetStateTransitions(species[index]),
                    reproductionCandidates = reproduction.Candidates,
                    reproductionBlockedEnergy = reproduction.BlockedEnergy,
                    reproductionBlockedMateRequirement = reproduction.BlockedMateRequirement,
                    reproductionBlockedGroupLimit = reproduction.BlockedGroupLimit,
                    reproductionFailedChanceRoll = reproduction.FailedChanceRoll,
                    reproductionBlockedNoBirthLocation = reproduction.BlockedNoBirthLocation,
                    reproductionSuccessfulAttempts = reproduction.SuccessfulAttempts,
                    reproductionReconciled = reproduction.IsReconciled,
                };
            }

            return activity;
        }

        public static SimulationSpeciesBehaviorRecord[] CreateBehavior(
            SpeciesSimulationMetrics metrics,
            IReadOnlyList<SpeciesId> species)
        {
            var records = new List<SimulationSpeciesBehaviorRecord>();
            foreach (var speciesId in species)
            {
                foreach (SpeciesBehaviorState state in System.Enum.GetValues(typeof(SpeciesBehaviorState)))
                {
                    records.Add(new SimulationSpeciesBehaviorRecord
                    {
                        speciesId = speciesId.Value,
                        state = state.ToString(),
                        ticks = metrics.GetStateTicks(speciesId, state),
                    });
                }
            }

            return records.ToArray();
        }

        public static SimulationSpeciesBehaviorTransitionRecord[] CreateBehaviorTransitions(
            SpeciesSimulationMetrics metrics)
        {
            var source = metrics.BehaviorTransitions;
            var records = new SimulationSpeciesBehaviorTransitionRecord[source.Count];
            for (var index = 0; index < records.Length; index++)
            {
                var transition = source[index];
                records[index] = new SimulationSpeciesBehaviorTransitionRecord
                {
                    speciesId = transition.Species.Value,
                    entityId = transition.EntityId,
                    age = transition.Age,
                    x = transition.X,
                    y = transition.Y,
                    previousState = transition.PreviousState.ToString(),
                    currentState = transition.CurrentState.ToString(),
                };
            }

            return records;
        }

        public static SimulationSpeciesTrackedBehaviorRecord[] CreateTrackedBehavior(
            SpeciesSimulationMetrics metrics,
            IReadOnlyList<SpeciesId> species)
        {
            var records = new List<SimulationSpeciesTrackedBehaviorRecord>();
            foreach (var speciesId in species)
            {
                if (!metrics.TryGetTrackedBehavior(speciesId, out var tracked))
                {
                    continue;
                }

                records.Add(new SimulationSpeciesTrackedBehaviorRecord
                {
                    speciesId = tracked.Species.Value,
                    entityId = tracked.EntityId,
                    age = tracked.Age,
                    x = tracked.X,
                    y = tracked.Y,
                    state = tracked.State.ToString(),
                    stateTicks = tracked.StateTicks,
                });
            }

            return records.ToArray();
        }

        public static SimulationSpeciesDeathRecord[] CreateDeathEvents(SpeciesSimulationMetrics metrics)
        {
            var source = metrics.DeathEvents;
            var records = new SimulationSpeciesDeathRecord[source.Count];
            for (var index = 0; index < records.Length; index++)
            {
                var death = source[index];
                records[index] = new SimulationSpeciesDeathRecord
                {
                    speciesId = death.Species.Value,
                    entityId = death.EntityId,
                    age = death.Age,
                    x = death.X,
                    y = death.Y,
                    tick = death.Tick,
                    cause = death.Cause.ToString(),
                    isCreature = death.IsCreature,
                };
            }

            return records;
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
        public int foodActionAttempts;
        public int foodActionSuccesses;
        public int foodActionFailures;
        public int movementSteps;
        public int damageDealt;
        public int combatKills;
        public int deaths;
        public int starvationDeaths;
        public int crowdingDeaths;
        public int wiltDeaths;
        public int populationLimitRemovals;
        public int stateTransitions;
        public int reproductionCandidates;
        public int reproductionBlockedEnergy;
        public int reproductionBlockedMateRequirement;
        public int reproductionBlockedGroupLimit;
        public int reproductionFailedChanceRoll;
        public int reproductionBlockedNoBirthLocation;
        public int reproductionSuccessfulAttempts;
        public bool reproductionReconciled;
    }

    [System.Serializable]
    sealed class SimulationSpeciesBehaviorRecord
    {
        public string speciesId;
        public string state;
        public int ticks;
    }

    [System.Serializable]
    sealed class SimulationSpeciesBehaviorTransitionRecord
    {
        public string speciesId;
        public long entityId;
        public int age;
        public int x;
        public int y;
        public string previousState;
        public string currentState;
    }

    [System.Serializable]
    sealed class SimulationSpeciesTrackedBehaviorRecord
    {
        public string speciesId;
        public long entityId;
        public int age;
        public int x;
        public int y;
        public string state;
        public int stateTicks;
    }

    [System.Serializable]
    sealed class SimulationSpeciesDeathRecord
    {
        public string speciesId;
        public long entityId;
        public int age;
        public int x;
        public int y;
        public int tick;
        public string cause;
        public bool isCreature;
    }
}
