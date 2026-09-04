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
                    combatOpportunities = source.CombatOpportunities,
                    combatAttempts = source.CombatAttempts,
                    combatHits = source.CombatHits,
                    combatBlocked = source.CombatBlocked,
                    combatDamageApplications = source.CombatDamageApplications,
                    combatNonLethalHits = source.CombatNonLethalHits,
                    combatLethalHits = source.CombatLethalHits,
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

        public static SimulationUpgradeRecord[] CreateUpgradeLoadout(
            IReadOnlyList<SpeciesUpgradeSnapshot> upgrades)
        {
            if (upgrades == null || upgrades.Count == 0)
            {
                return new SimulationUpgradeRecord[0];
            }

            var records = new SimulationUpgradeRecord[upgrades.Count];
            for (var index = 0; index < records.Length; index++)
            {
                var upgrade = upgrades[index];
                records[index] = new SimulationUpgradeRecord
                {
                    order = index,
                    upgradeId = upgrade.Id,
                    displayName = upgrade.DisplayName,
                    targetSpeciesId = upgrade.TargetSpecies.Value,
                    scope = upgrade.Scope.ToString(),
                    cost = upgrade.Cost,
                    contractVersion = SpeciesUpgradeSnapshot.ContractVersion,
                    registryFingerprint = upgrade.RegistryFingerprint,
                    fingerprint = upgrade.Fingerprint,
                    modifiers = CreateUpgradeModifiers(upgrade.Modifiers),
                };
            }

            return records;
        }

        static SimulationUpgradeModifierRecord[] CreateUpgradeModifiers(
            IReadOnlyList<SpeciesUpgradeModifier> modifiers)
        {
            var records = new SimulationUpgradeModifierRecord[modifiers.Count];
            for (var index = 0; index < records.Length; index++)
            {
                records[index] = new SimulationUpgradeModifierRecord
                {
                    attributeId = modifiers[index].AttributeId,
                    signedValue = modifiers[index].SignedValue,
                };
            }

            return records;
        }

        public static SimulationSpeciesCombatRollRecord[] CreateCombatRolls(SpeciesSimulationMetrics metrics)
        {
            var source = metrics.CombatRollEvents;
            var records = new SimulationSpeciesCombatRollRecord[source.Count];
            for (var index = 0; index < records.Length; index++)
            {
                var roll = source[index];
                records[index] = new SimulationSpeciesCombatRollRecord
                {
                    attackerSpeciesId = roll.AttackerSpecies.Value,
                    targetSpeciesId = roll.TargetSpecies.Value,
                    tick = roll.Tick,
                    attackRoll = roll.AttackRoll,
                    attackModifier = roll.AttackModifier,
                    blockRoll = roll.BlockRoll,
                    blockModifier = roll.BlockModifier,
                    attackTotal = roll.AttackRoll + roll.AttackModifier,
                    blockTotal = roll.BlockRoll + roll.BlockModifier,
                    expectedHitProbability = roll.ExpectedHitProbability,
                    hit = roll.Hit,
                };
            }

            return records;
        }

        public static SimulationSpeciesCombatCooldownSuppressionRecord[] CreateCombatCooldownSuppressions(
            SpeciesSimulationMetrics metrics)
        {
            var source = metrics.CombatCooldownSuppressionEvents;
            var records = new SimulationSpeciesCombatCooldownSuppressionRecord[source.Count];
            for (var index = 0; index < records.Length; index++)
            {
                var suppression = source[index];
                records[index] = new SimulationSpeciesCombatCooldownSuppressionRecord
                {
                    attackerSpeciesId = suppression.AttackerSpecies.Value,
                    entityId = suppression.EntityId,
                    x = suppression.X,
                    y = suppression.Y,
                    tick = suppression.Tick,
                    remainingTicks = suppression.RemainingTicks,
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

        public static SimulationHerbivoreStatLineRecord CreateHerbivoreStatLine(
            SimulationRunState run,
            SpeciesId species)
        {
            var startingPopulation = run.PopulationHistory[0].GetCount(species);
            var finalPopulation = run.PopulationHistory[run.PopulationHistory.Count - 1].GetCount(species);
            var statLine = run.Metrics.CreateHerbivoreStatLine(
                species,
                startingPopulation,
                finalPopulation);
            return new SimulationHerbivoreStatLineRecord
            {
                speciesId = statLine.Species.Value,
                SPO = statLine.StartingPopulation,
                HPS = statLine.PredatorActiveHerbivoreSteps,
                EHS = statLine.EncounteredHerbivoreSteps,
                ECN = statLine.Encounters,
                PREY = statLine.Preyed,
                STRV = statLine.Starved,
                MAT = statLine.Mating,
                BIR = statLine.Births,
                CRWD = statLine.Crowding,
                FPO = statLine.FinalPopulation,
                expectedFPO = statLine.ExpectedFinalPopulation,
                fpoReconciled = statLine.PopulationReconciled,
                pAVI = statLine.InversePreyedAverage,
                pAVIStatus = GetMetricStatusText(statLine.InversePreyedAverageStatus),
                eAVI = statLine.InverseEncounterAverage,
                eAVIStatus = GetMetricStatusText(statLine.InverseEncounterAverageStatus),
                predAVG = statLine.PredationAverage,
                predAVGStatus = GetMetricStatusText(statLine.PredationAverageStatus),
                sAVI = statLine.InverseStarvedAverage,
                sAVIStatus = GetMetricStatusText(statLine.InverseStarvedAverageStatus),
                cAVI = statLine.InverseCrowdingAverage,
                cAVIStatus = GetMetricStatusText(statLine.InverseCrowdingAverageStatus),
                bAVG = statLine.BirthAverage,
                bAVGStatus = GetMetricStatusText(statLine.BirthAverageStatus),
                RFS = statLine.ReplicationFitnessScore,
                RFSStatus = GetMetricStatusText(statLine.ReplicationFitnessScoreStatus),
                APS = statLine.ActualPreyScore,
                APSStatus = GetMetricStatusText(statLine.ActualPreyScoreStatus),
            };
        }

        static string GetMetricStatusText(SpeciesHerbivoreMetricStatus status)
        {
            switch (status)
            {
                case SpeciesHerbivoreMetricStatus.NotApplicable:
                    return "N/A";
                case SpeciesHerbivoreMetricStatus.Invalid:
                    return "INVALID";
                default:
                    return "Valid";
            }
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
        public int combatOpportunities;
        public int combatAttempts;
        public int combatHits;
        public int combatBlocked;
        public int combatDamageApplications;
        public int combatNonLethalHits;
        public int combatLethalHits;
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

    [System.Serializable]
    sealed class SimulationUpgradeRecord
    {
        public int order;
        public string upgradeId;
        public string displayName;
        public string targetSpeciesId;
        public string scope;
        public int cost;
        public string contractVersion;
        public string registryFingerprint;
        public string fingerprint;
        public SimulationUpgradeModifierRecord[] modifiers;
    }

    [System.Serializable]
    sealed class SimulationUpgradeModifierRecord
    {
        public string attributeId;
        public float signedValue;
    }

    [System.Serializable]
    sealed class SimulationSpeciesCombatRollRecord
    {
        public string attackerSpeciesId;
        public string targetSpeciesId;
        public int tick;
        public int attackRoll;
        public int attackModifier;
        public int blockRoll;
        public int blockModifier;
        public int attackTotal;
        public int blockTotal;
        public float expectedHitProbability;
        public bool hit;
    }

    [System.Serializable]
    sealed class SimulationSpeciesCombatCooldownSuppressionRecord
    {
        public string attackerSpeciesId;
        public long entityId;
        public int x;
        public int y;
        public int tick;
        public int remainingTicks;
    }

    [System.Serializable]
    sealed class SimulationHerbivoreStatLineRecord
    {
        public string speciesId;
        public int SPO;
        public int HPS;
        public int EHS;
        public int ECN;
        public int PREY;
        public int STRV;
        public int MAT;
        public int BIR;
        public int CRWD;
        public int FPO;
        public int expectedFPO;
        public bool fpoReconciled;
        public float pAVI;
        public string pAVIStatus;
        public float eAVI;
        public string eAVIStatus;
        public float predAVG;
        public string predAVGStatus;
        public float sAVI;
        public string sAVIStatus;
        public float cAVI;
        public string cAVIStatus;
        public float bAVG;
        public string bAVGStatus;
        public float RFS;
        public string RFSStatus;
        public float APS;
        public string APSStatus;
    }
}
