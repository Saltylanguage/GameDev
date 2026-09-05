using System;
using System.Collections.Generic;

namespace SaltyGame
{
    public enum SpeciesBehaviorState
    {
        Wandering,
        Hunting,
        Eating,
        Mating,
        Sleeping,
        Attacking,
        Threatened,
        Dead,
    }

    public enum SpeciesDeathCause
    {
        Unknown,
        Combat,
        Starvation,
        Crowding,
        Wilt,
        PopulationLimit,
        ResourceConsumed,
    }

    public enum SpeciesReproductionOutcome
    {
        BlockedEnergy,
        BlockedMateRequirement,
        BlockedGroupLimit,
        FailedChanceRoll,
        BlockedNoBirthLocation,
        SuccessfulAttempt,
    }

    public readonly struct SpeciesReproductionActivity
    {
        internal SpeciesReproductionActivity(
            int candidates,
            int blockedEnergy,
            int blockedMateRequirement,
            int blockedGroupLimit,
            int failedChanceRoll,
            int blockedNoBirthLocation,
            int successfulAttempts)
        {
            Candidates = candidates;
            BlockedEnergy = blockedEnergy;
            BlockedMateRequirement = blockedMateRequirement;
            BlockedGroupLimit = blockedGroupLimit;
            FailedChanceRoll = failedChanceRoll;
            BlockedNoBirthLocation = blockedNoBirthLocation;
            SuccessfulAttempts = successfulAttempts;
        }

        public int Candidates { get; }
        public int BlockedEnergy { get; }
        public int BlockedMateRequirement { get; }
        public int BlockedGroupLimit { get; }
        public int FailedChanceRoll { get; }
        public int BlockedNoBirthLocation { get; }
        public int SuccessfulAttempts { get; }

        public int ClassifiedCandidates =>
            BlockedEnergy
            + BlockedMateRequirement
            + BlockedGroupLimit
            + FailedChanceRoll
            + BlockedNoBirthLocation
            + SuccessfulAttempts;

        public bool IsReconciled => Candidates == ClassifiedCandidates;

        internal SpeciesReproductionActivity Add(SpeciesReproductionOutcome outcome)
        {
            return new SpeciesReproductionActivity(
                Candidates + 1,
                BlockedEnergy + (outcome == SpeciesReproductionOutcome.BlockedEnergy ? 1 : 0),
                BlockedMateRequirement + (outcome == SpeciesReproductionOutcome.BlockedMateRequirement ? 1 : 0),
                BlockedGroupLimit + (outcome == SpeciesReproductionOutcome.BlockedGroupLimit ? 1 : 0),
                FailedChanceRoll + (outcome == SpeciesReproductionOutcome.FailedChanceRoll ? 1 : 0),
                BlockedNoBirthLocation + (outcome == SpeciesReproductionOutcome.BlockedNoBirthLocation ? 1 : 0),
                SuccessfulAttempts + (outcome == SpeciesReproductionOutcome.SuccessfulAttempt ? 1 : 0));
        }

        internal SpeciesReproductionActivity Subtract(SpeciesReproductionActivity baseline)
        {
            return new SpeciesReproductionActivity(
                Candidates - baseline.Candidates,
                BlockedEnergy - baseline.BlockedEnergy,
                BlockedMateRequirement - baseline.BlockedMateRequirement,
                BlockedGroupLimit - baseline.BlockedGroupLimit,
                FailedChanceRoll - baseline.FailedChanceRoll,
                BlockedNoBirthLocation - baseline.BlockedNoBirthLocation,
                SuccessfulAttempts - baseline.SuccessfulAttempts);
        }
    }

    public readonly struct SpeciesDeathEvent
    {
        internal SpeciesDeathEvent(
            SpeciesId species,
            long entityId,
            int age,
            int x,
            int y,
            int tick,
            SpeciesDeathCause cause,
            bool isCreature)
        {
            Species = species;
            EntityId = entityId;
            Age = age;
            X = x;
            Y = y;
            Tick = tick;
            Cause = cause;
            IsCreature = isCreature;
        }

        public SpeciesId Species { get; }
        public long EntityId { get; }
        public int Age { get; }
        public int X { get; }
        public int Y { get; }
        public int Tick { get; }
        public SpeciesDeathCause Cause { get; }
        public bool IsCreature { get; }
    }

    public readonly struct SpeciesCombatRollEvent
    {
        internal SpeciesCombatRollEvent(
            SpeciesId attackerSpecies,
            SpeciesId targetSpecies,
            int tick,
            int attackRoll,
            int attackModifier,
            int blockRoll,
            int blockModifier,
            bool hit)
        {
            AttackerSpecies = attackerSpecies;
            TargetSpecies = targetSpecies;
            Tick = tick;
            AttackRoll = attackRoll;
            AttackModifier = attackModifier;
            BlockRoll = blockRoll;
            BlockModifier = blockModifier;
            Hit = hit;
        }

        public SpeciesId AttackerSpecies { get; }
        public SpeciesId TargetSpecies { get; }
        public int Tick { get; }
        public int AttackRoll { get; }
        public int AttackModifier { get; }
        public int BlockRoll { get; }
        public int BlockModifier { get; }
        public bool Hit { get; }
        public int AttackTotal => AttackRoll + AttackModifier;
        public int BlockTotal => BlockRoll + BlockModifier;
        public float ExpectedHitProbability =>
            SpeciesSimulation.GetOpposedRollHitProbability(AttackModifier, BlockModifier);
    }

    public readonly struct SpeciesCombatCooldownSuppressionEvent
    {
        internal SpeciesCombatCooldownSuppressionEvent(
            SpeciesId attackerSpecies,
            long entityId,
            int x,
            int y,
            int tick,
            int remainingTicks)
        {
            AttackerSpecies = attackerSpecies;
            EntityId = entityId;
            X = x;
            Y = y;
            Tick = tick;
            RemainingTicks = remainingTicks;
        }

        public SpeciesId AttackerSpecies { get; }
        public long EntityId { get; }
        public int X { get; }
        public int Y { get; }
        public int Tick { get; }
        public int RemainingTicks { get; }
    }

    public readonly struct SpeciesBehaviorTransition
    {
        internal SpeciesBehaviorTransition(
            SpeciesId species,
            long entityId,
            int age,
            int x,
            int y,
            int tick,
            SpeciesBehaviorState previousState,
            SpeciesBehaviorState currentState)
        {
            Species = species;
            EntityId = entityId;
            Age = age;
            X = x;
            Y = y;
            Tick = tick;
            PreviousState = previousState;
            CurrentState = currentState;
        }

        public SpeciesId Species { get; }
        public long EntityId { get; }
        public int Age { get; }
        public int X { get; }
        public int Y { get; }
        public int Tick { get; }
        public SpeciesBehaviorState PreviousState { get; }
        public SpeciesBehaviorState CurrentState { get; }
    }

    public readonly struct SpeciesTrackedBehavior
    {
        internal SpeciesTrackedBehavior(
            SpeciesId species,
            long entityId,
            int age,
            int x,
            int y,
            SpeciesBehaviorState state,
            int stateTicks)
        {
            Species = species;
            EntityId = entityId;
            Age = age;
            X = x;
            Y = y;
            State = state;
            StateTicks = stateTicks;
        }

        public SpeciesId Species { get; }
        public long EntityId { get; }
        public int Age { get; }
        public int X { get; }
        public int Y { get; }
        public SpeciesBehaviorState State { get; }
        public int StateTicks { get; }
    }

    public readonly struct SpeciesSimulationActivity
    {
        internal SpeciesSimulationActivity(
            int births,
            float foodConsumed,
            int foodActionAttempts,
            int foodActionSuccesses,
            int foodActionFailures,
            int movementSteps,
            int damageDealt,
            int combatKills,
            int combatOpportunities,
            int combatAttempts,
            int combatHits,
            int combatBlocked,
            int combatDamageApplications,
            int combatNonLethalHits,
            int combatLethalHits,
            int deaths,
            int starvationDeaths,
            int crowdingDeaths,
            int wiltDeaths,
            int populationLimitRemovals)
        {
            Births = births;
            FoodConsumed = foodConsumed;
            FoodActionAttempts = foodActionAttempts;
            FoodActionSuccesses = foodActionSuccesses;
            FoodActionFailures = foodActionFailures;
            MovementSteps = movementSteps;
            DamageDealt = damageDealt;
            CombatKills = combatKills;
            CombatOpportunities = combatOpportunities;
            CombatAttempts = combatAttempts;
            CombatHits = combatHits;
            CombatBlocked = combatBlocked;
            CombatDamageApplications = combatDamageApplications;
            CombatNonLethalHits = combatNonLethalHits;
            CombatLethalHits = combatLethalHits;
            Deaths = deaths;
            StarvationDeaths = starvationDeaths;
            CrowdingDeaths = crowdingDeaths;
            WiltDeaths = wiltDeaths;
            PopulationLimitRemovals = populationLimitRemovals;
        }

        public int Births { get; }
        public float FoodConsumed { get; }
        public int FoodActionAttempts { get; }
        public int FoodActionSuccesses { get; }
        public int FoodActionFailures { get; }
        public int MovementSteps { get; }
        public int DamageDealt { get; }
        public int CombatKills { get; }
        public int CombatOpportunities { get; }
        public int CombatAttempts { get; }
        public int CombatHits { get; }
        public int CombatBlocked { get; }
        public int CombatDamageApplications { get; }
        public int CombatNonLethalHits { get; }
        public int CombatLethalHits { get; }
        public int Deaths { get; }
        public int StarvationDeaths { get; }
        public int CrowdingDeaths { get; }
        public int WiltDeaths { get; }
        public int PopulationLimitRemovals { get; }

        internal SpeciesSimulationActivity Add(
            int births = 0,
            float foodConsumed = 0f,
            int foodActionAttempts = 0,
            int foodActionSuccesses = 0,
            int foodActionFailures = 0,
            int movementSteps = 0,
            int damageDealt = 0,
            int combatKills = 0,
            int combatOpportunities = 0,
            int combatAttempts = 0,
            int combatHits = 0,
            int combatBlocked = 0,
            int combatDamageApplications = 0,
            int combatNonLethalHits = 0,
            int combatLethalHits = 0,
            int deaths = 0,
            int starvationDeaths = 0,
            int crowdingDeaths = 0,
            int wiltDeaths = 0,
            int populationLimitRemovals = 0)
        {
            return new SpeciesSimulationActivity(
                Births + births,
                FoodConsumed + foodConsumed,
                FoodActionAttempts + foodActionAttempts,
                FoodActionSuccesses + foodActionSuccesses,
                FoodActionFailures + foodActionFailures,
                MovementSteps + movementSteps,
                DamageDealt + damageDealt,
                CombatKills + combatKills,
                CombatOpportunities + combatOpportunities,
                CombatAttempts + combatAttempts,
                CombatHits + combatHits,
                CombatBlocked + combatBlocked,
                CombatDamageApplications + combatDamageApplications,
                CombatNonLethalHits + combatNonLethalHits,
                CombatLethalHits + combatLethalHits,
                Deaths + deaths,
                StarvationDeaths + starvationDeaths,
                CrowdingDeaths + crowdingDeaths,
                WiltDeaths + wiltDeaths,
                PopulationLimitRemovals + populationLimitRemovals);
        }

        internal SpeciesSimulationActivity Subtract(SpeciesSimulationActivity baseline)
        {
            return new SpeciesSimulationActivity(
                Births - baseline.Births,
                FoodConsumed - baseline.FoodConsumed,
                FoodActionAttempts - baseline.FoodActionAttempts,
                FoodActionSuccesses - baseline.FoodActionSuccesses,
                FoodActionFailures - baseline.FoodActionFailures,
                MovementSteps - baseline.MovementSteps,
                DamageDealt - baseline.DamageDealt,
                CombatKills - baseline.CombatKills,
                CombatOpportunities - baseline.CombatOpportunities,
                CombatAttempts - baseline.CombatAttempts,
                CombatHits - baseline.CombatHits,
                CombatBlocked - baseline.CombatBlocked,
                CombatDamageApplications - baseline.CombatDamageApplications,
                CombatNonLethalHits - baseline.CombatNonLethalHits,
                CombatLethalHits - baseline.CombatLethalHits,
                Deaths - baseline.Deaths,
                StarvationDeaths - baseline.StarvationDeaths,
                CrowdingDeaths - baseline.CrowdingDeaths,
                WiltDeaths - baseline.WiltDeaths,
                PopulationLimitRemovals - baseline.PopulationLimitRemovals);
        }
    }

    public enum SpeciesHerbivoreMetricStatus
    {
        Valid,
        NotApplicable,
        Invalid,
    }

    public readonly struct SpeciesHerbivoreStatLine
    {
        public SpeciesHerbivoreStatLine(
            SpeciesId species,
            int startingPopulation,
            int predatorActiveHerbivoreSteps,
            int encounteredHerbivoreSteps,
            int encounters,
            int preyed,
            int starved,
            int mating,
            int births,
            int crowding,
            int finalPopulation)
        {
            if (!species.IsValid)
            {
                throw new ArgumentException("Herbivore stat line requires a valid species id.", nameof(species));
            }

            if (startingPopulation < 0 || predatorActiveHerbivoreSteps < 0
                || encounteredHerbivoreSteps < 0 || encounters < 0 || preyed < 0 || starved < 0
                || mating < 0 || births < 0 || crowding < 0 || finalPopulation < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(startingPopulation), "Herbivore stat counts cannot be negative.");
            }

            var populationBeforeStarvation = startingPopulation + births - preyed;
            var populationBeforeCrowding = populationBeforeStarvation - starved;
            var inversePreyedAverageStatus = GetRateStatus(preyed, encounters);
            var inverseEncounterAverageStatus = GetRateStatus(
                encounteredHerbivoreSteps,
                predatorActiveHerbivoreSteps);
            var inverseStarvedAverageStatus = GetRateStatus(starved, populationBeforeStarvation);
            var inverseCrowdingAverageStatus = GetRateStatus(crowding, populationBeforeCrowding);
            var birthAverageStatus = GetRateStatus(births, mating);
            var expectedFinalPopulation = populationBeforeCrowding - crowding;
            var populationReconciled = finalPopulation == expectedFinalPopulation;

            var inversePreyedAverage = inversePreyedAverageStatus == SpeciesHerbivoreMetricStatus.Valid
                ? 1f - (float)preyed / encounters
                : 0f;
            var inverseEncounterAverage = inverseEncounterAverageStatus == SpeciesHerbivoreMetricStatus.Valid
                ? 1f - (float)encounteredHerbivoreSteps / predatorActiveHerbivoreSteps
                : 0f;
            var predationAverageStatus = GetApplicableAverageStatus(
                inversePreyedAverageStatus,
                inverseEncounterAverageStatus);
            var predationAverage = GetApplicableAverage(
                inversePreyedAverage,
                inversePreyedAverageStatus,
                inverseEncounterAverage,
                inverseEncounterAverageStatus);
            var inverseStarvedAverage = inverseStarvedAverageStatus == SpeciesHerbivoreMetricStatus.Valid
                ? 1f - (float)starved / populationBeforeStarvation
                : 0f;
            var inverseCrowdingAverage = inverseCrowdingAverageStatus == SpeciesHerbivoreMetricStatus.Valid
                ? 1f - (float)crowding / populationBeforeCrowding
                : 0f;
            var birthAverage = birthAverageStatus == SpeciesHerbivoreMetricStatus.Valid
                ? (float)births / mating
                : 0f;

            var replicationFitnessNumerator = finalPopulation - startingPopulation;
            var replicationFitnessStatus = GetReplicationFitnessStatus(birthAverageStatus);
            var replicationFitnessScore = replicationFitnessStatus == SpeciesHerbivoreMetricStatus.Valid
                ? replicationFitnessNumerator * birthAverage
                : 0f;

            var actualPreyScoreStatus = populationReconciled
                && predationAverageStatus != SpeciesHerbivoreMetricStatus.Invalid
                && inverseStarvedAverageStatus != SpeciesHerbivoreMetricStatus.Invalid
                && inverseCrowdingAverageStatus != SpeciesHerbivoreMetricStatus.Invalid
                && birthAverageStatus != SpeciesHerbivoreMetricStatus.Invalid
                && replicationFitnessStatus != SpeciesHerbivoreMetricStatus.Invalid
                ? SpeciesHerbivoreMetricStatus.Valid
                : SpeciesHerbivoreMetricStatus.Invalid;
            var actualPreyScore = 0f;
            if (actualPreyScoreStatus == SpeciesHerbivoreMetricStatus.Valid)
            {
                if (replicationFitnessStatus == SpeciesHerbivoreMetricStatus.Valid)
                {
                    actualPreyScore += replicationFitnessScore;
                }

                if (predationAverageStatus == SpeciesHerbivoreMetricStatus.Valid)
                {
                    actualPreyScore += predationAverage;
                }

                if (inverseStarvedAverageStatus == SpeciesHerbivoreMetricStatus.Valid)
                {
                    actualPreyScore -= 1f - inverseStarvedAverage;
                }

                if (inverseCrowdingAverageStatus == SpeciesHerbivoreMetricStatus.Valid)
                {
                    actualPreyScore -= 1f - inverseCrowdingAverage;
                }
            }

            Species = species;
            StartingPopulation = startingPopulation;
            PredatorActiveHerbivoreSteps = predatorActiveHerbivoreSteps;
            EncounteredHerbivoreSteps = encounteredHerbivoreSteps;
            Encounters = encounters;
            Preyed = preyed;
            Starved = starved;
            Mating = mating;
            Births = births;
            Crowding = crowding;
            FinalPopulation = finalPopulation;
            ExpectedFinalPopulation = expectedFinalPopulation;
            PopulationReconciled = populationReconciled;
            InversePreyedAverage = inversePreyedAverage;
            InversePreyedAverageStatus = inversePreyedAverageStatus;
            InverseEncounterAverage = inverseEncounterAverage;
            InverseEncounterAverageStatus = inverseEncounterAverageStatus;
            PredationAverage = predationAverage;
            PredationAverageStatus = predationAverageStatus;
            InverseStarvedAverage = inverseStarvedAverage;
            InverseStarvedAverageStatus = inverseStarvedAverageStatus;
            InverseCrowdingAverage = inverseCrowdingAverage;
            InverseCrowdingAverageStatus = inverseCrowdingAverageStatus;
            BirthAverage = birthAverage;
            BirthAverageStatus = birthAverageStatus;
            ReplicationFitnessScore = replicationFitnessScore;
            ReplicationFitnessScoreStatus = replicationFitnessStatus;
            ActualPreyScore = actualPreyScore;
            ActualPreyScoreStatus = actualPreyScoreStatus;
        }

        static SpeciesHerbivoreMetricStatus GetRateStatus(int numerator, int denominator)
        {
            if (denominator < 0 || numerator > denominator)
            {
                return SpeciesHerbivoreMetricStatus.Invalid;
            }

            if (denominator == 0)
            {
                return numerator == 0
                    ? SpeciesHerbivoreMetricStatus.NotApplicable
                    : SpeciesHerbivoreMetricStatus.Invalid;
            }

            return SpeciesHerbivoreMetricStatus.Valid;
        }

        static SpeciesHerbivoreMetricStatus GetReplicationFitnessStatus(
            SpeciesHerbivoreMetricStatus birthAverageStatus)
        {
            if (birthAverageStatus == SpeciesHerbivoreMetricStatus.Invalid)
            {
                return SpeciesHerbivoreMetricStatus.Invalid;
            }

            if (birthAverageStatus == SpeciesHerbivoreMetricStatus.NotApplicable)
            {
                return SpeciesHerbivoreMetricStatus.NotApplicable;
            }

            return SpeciesHerbivoreMetricStatus.Valid;
        }

        static SpeciesHerbivoreMetricStatus GetApplicableAverageStatus(
            SpeciesHerbivoreMetricStatus first,
            SpeciesHerbivoreMetricStatus second)
        {
            if (first == SpeciesHerbivoreMetricStatus.Invalid
                || second == SpeciesHerbivoreMetricStatus.Invalid)
            {
                return SpeciesHerbivoreMetricStatus.Invalid;
            }

            return first == SpeciesHerbivoreMetricStatus.Valid
                || second == SpeciesHerbivoreMetricStatus.Valid
                ? SpeciesHerbivoreMetricStatus.Valid
                : SpeciesHerbivoreMetricStatus.NotApplicable;
        }

        static float GetApplicableAverage(
            float firstValue,
            SpeciesHerbivoreMetricStatus firstStatus,
            float secondValue,
            SpeciesHerbivoreMetricStatus secondStatus)
        {
            if (firstStatus == SpeciesHerbivoreMetricStatus.Valid
                && secondStatus == SpeciesHerbivoreMetricStatus.Valid)
            {
                return (firstValue + secondValue) * 0.5f;
            }

            if (firstStatus == SpeciesHerbivoreMetricStatus.Valid)
            {
                return firstValue;
            }

            return secondStatus == SpeciesHerbivoreMetricStatus.Valid ? secondValue : 0f;
        }

        public SpeciesId Species { get; }
        public int StartingPopulation { get; }
        public int PredatorActiveHerbivoreSteps { get; }
        public int EncounteredHerbivoreSteps { get; }
        public int Encounters { get; }
        public int Preyed { get; }
        public int Starved { get; }
        public int Mating { get; }
        public int Births { get; }
        public int Crowding { get; }
        public int FinalPopulation { get; }
        public int ExpectedFinalPopulation { get; }
        public bool PopulationReconciled { get; }
        public float InversePreyedAverage { get; }
        public SpeciesHerbivoreMetricStatus InversePreyedAverageStatus { get; }
        public float InverseEncounterAverage { get; }
        public SpeciesHerbivoreMetricStatus InverseEncounterAverageStatus { get; }
        public float PredationAverage { get; }
        public SpeciesHerbivoreMetricStatus PredationAverageStatus { get; }
        public float InverseStarvedAverage { get; }
        public SpeciesHerbivoreMetricStatus InverseStarvedAverageStatus { get; }
        public float InverseCrowdingAverage { get; }
        public SpeciesHerbivoreMetricStatus InverseCrowdingAverageStatus { get; }
        public float BirthAverage { get; }
        public SpeciesHerbivoreMetricStatus BirthAverageStatus { get; }
        public float ReplicationFitnessScore { get; }
        public SpeciesHerbivoreMetricStatus ReplicationFitnessScoreStatus { get; }
        public float ActualPreyScore { get; }
        public SpeciesHerbivoreMetricStatus ActualPreyScoreStatus { get; }
    }

    public interface ISpeciesSimulationMetricsView
    {
        SpeciesSimulationActivity GetActivity(SpeciesId species);
        SpeciesReproductionActivity GetReproductionActivity(SpeciesId species);
        int GetStateTicks(SpeciesId species, SpeciesBehaviorState state);
        int GetStateTransitions(SpeciesId species);
        int GetHerbivoreEncounters(SpeciesId species);
        int GetHerbivorePreyed(SpeciesId species);
        int GetPredatorActiveHerbivoreSteps(SpeciesId species);
        int GetEncounteredHerbivoreSteps(SpeciesId species);
        bool TryGetTrackedBehavior(SpeciesId species, out SpeciesTrackedBehavior behavior);
        IReadOnlyList<SpeciesBehaviorTransition> BehaviorTransitions { get; }
        IReadOnlyList<SpeciesDeathEvent> DeathEvents { get; }
        IReadOnlyList<SpeciesCombatRollEvent> CombatRollEvents { get; }
        IReadOnlyList<SpeciesCombatCooldownSuppressionEvent> CombatCooldownSuppressionEvents { get; }
        int ControlledOpportunityScheduled { get; }
        int ControlledOpportunityEligible { get; }
        int ControlledOpportunityUnfulfilledNoTarget { get; }
        int ControlledOpportunityUnfulfilledInvalidated { get; }
        SpeciesHerbivoreStatLine CreateHerbivoreStatLine(SpeciesId species, int startingPopulation, int finalPopulation);
    }

    public sealed class SpeciesSimulationMetrics : ISpeciesSimulationMetricsView
    {
        readonly Dictionary<SpeciesId, SpeciesSimulationActivity> activityBySpecies =
            new Dictionary<SpeciesId, SpeciesSimulationActivity>();
        readonly Dictionary<SpeciesId, SpeciesReproductionActivity> reproductionBySpecies =
            new Dictionary<SpeciesId, SpeciesReproductionActivity>();
        readonly Dictionary<SpeciesId, Dictionary<SpeciesBehaviorState, int>> stateTicksBySpecies =
            new Dictionary<SpeciesId, Dictionary<SpeciesBehaviorState, int>>();
        readonly Dictionary<SpeciesId, int> stateTransitionsBySpecies =
            new Dictionary<SpeciesId, int>();
        readonly Dictionary<SpeciesId, TrackedBehaviorCell> trackedBehaviorCells =
            new Dictionary<SpeciesId, TrackedBehaviorCell>();
        readonly Dictionary<SpeciesId, SpeciesTrackedBehavior> trackedBehaviors =
            new Dictionary<SpeciesId, SpeciesTrackedBehavior>();
        readonly Dictionary<SpeciesId, int> herbivoreEncountersBySpecies =
            new Dictionary<SpeciesId, int>();
        readonly Dictionary<SpeciesId, int> herbivorePreyedBySpecies =
            new Dictionary<SpeciesId, int>();
        readonly Dictionary<SpeciesId, int> predatorActiveHerbivoreStepsBySpecies =
            new Dictionary<SpeciesId, int>();
        readonly Dictionary<SpeciesId, int> encounteredHerbivoreStepsBySpecies =
            new Dictionary<SpeciesId, int>();
        readonly Dictionary<SpeciesId, int> predatorActiveHerbivoreStepsThisStepBySpecies =
            new Dictionary<SpeciesId, int>();
        readonly HashSet<SpeciesId> encounteredHerbivoreSpeciesThisStep = new HashSet<SpeciesId>();
        readonly List<SpeciesBehaviorTransition> behaviorTransitions =
            new List<SpeciesBehaviorTransition>();
        readonly List<SpeciesDeathEvent> deathEvents =
            new List<SpeciesDeathEvent>();
        readonly List<SpeciesCombatRollEvent> combatRollEvents =
            new List<SpeciesCombatRollEvent>();
        readonly List<SpeciesCombatCooldownSuppressionEvent> combatCooldownSuppressionEvents =
            new List<SpeciesCombatCooldownSuppressionEvent>();
        int currentTick = -1;
        int controlledOpportunityScheduled;
        int controlledOpportunityEligible;
        int controlledOpportunityUnfulfilledNoTarget;
        int controlledOpportunityUnfulfilledInvalidated;

        public SpeciesSimulationActivity GetActivity(SpeciesId species)
        {
            return activityBySpecies.TryGetValue(species, out var activity)
                ? activity
                : default;
        }

        public SpeciesReproductionActivity GetReproductionActivity(SpeciesId species)
        {
            return reproductionBySpecies.TryGetValue(species, out var activity)
                ? activity
                : default;
        }

        public int GetStateTicks(SpeciesId species, SpeciesBehaviorState state)
        {
            return stateTicksBySpecies.TryGetValue(species, out var stateTicks)
                && stateTicks.TryGetValue(state, out var ticks)
                ? ticks
                : 0;
        }

        public int GetStateTransitions(SpeciesId species)
        {
            return stateTransitionsBySpecies.TryGetValue(species, out var transitions)
                ? transitions
                : 0;
        }

        public int GetHerbivoreEncounters(SpeciesId species)
        {
            return herbivoreEncountersBySpecies.TryGetValue(species, out var encounters)
                ? encounters
                : 0;
        }

        public int GetHerbivorePreyed(SpeciesId species)
        {
            return herbivorePreyedBySpecies.TryGetValue(species, out var preyed)
                ? preyed
                : 0;
        }

        public int GetPredatorActiveHerbivoreSteps(SpeciesId species)
        {
            return predatorActiveHerbivoreStepsBySpecies.TryGetValue(species, out var steps)
                ? steps
                : 0;
        }

        public int GetEncounteredHerbivoreSteps(SpeciesId species)
        {
            return encounteredHerbivoreStepsBySpecies.TryGetValue(species, out var steps)
                ? steps
                : 0;
        }

        public IReadOnlyList<SpeciesBehaviorTransition> BehaviorTransitions => behaviorTransitions;
        public IReadOnlyList<SpeciesDeathEvent> DeathEvents => deathEvents;
        public IReadOnlyList<SpeciesCombatRollEvent> CombatRollEvents => combatRollEvents;
        public IReadOnlyList<SpeciesCombatCooldownSuppressionEvent> CombatCooldownSuppressionEvents =>
            combatCooldownSuppressionEvents;
        public int ControlledOpportunityScheduled => controlledOpportunityScheduled;
        public int ControlledOpportunityEligible => controlledOpportunityEligible;
        public int ControlledOpportunityUnfulfilledNoTarget => controlledOpportunityUnfulfilledNoTarget;
        public int ControlledOpportunityUnfulfilledInvalidated => controlledOpportunityUnfulfilledInvalidated;

        public bool TryGetTrackedBehavior(SpeciesId species, out SpeciesTrackedBehavior behavior)
        {
            return trackedBehaviors.TryGetValue(species, out behavior);
        }

        public void Clear()
        {
            activityBySpecies.Clear();
            reproductionBySpecies.Clear();
            stateTicksBySpecies.Clear();
            stateTransitionsBySpecies.Clear();
            trackedBehaviorCells.Clear();
            trackedBehaviors.Clear();
            herbivoreEncountersBySpecies.Clear();
            herbivorePreyedBySpecies.Clear();
            predatorActiveHerbivoreStepsBySpecies.Clear();
            encounteredHerbivoreStepsBySpecies.Clear();
            predatorActiveHerbivoreStepsThisStepBySpecies.Clear();
            encounteredHerbivoreSpeciesThisStep.Clear();
            behaviorTransitions.Clear();
            deathEvents.Clear();
            combatRollEvents.Clear();
            combatCooldownSuppressionEvents.Clear();
            currentTick = -1;
            controlledOpportunityScheduled = 0;
            controlledOpportunityEligible = 0;
            controlledOpportunityUnfulfilledNoTarget = 0;
            controlledOpportunityUnfulfilledInvalidated = 0;
        }

        internal void BeginTick(int tick)
        {
            currentTick = tick;
        }

        internal void BeginBehaviorTracking(Grid<SpeciesCell> source)
        {
            var liveSpecies = new HashSet<SpeciesId>();
            for (var y = 0; y < source.Height; y++)
            {
                for (var x = 0; x < source.Width; x++)
                {
                    var cell = source.GetCell(x, y);
                    if (cell.IsCreature)
                    {
                        liveSpecies.Add(cell.SpeciesId);
                    }
                }
            }

            foreach (var species in liveSpecies)
            {
                if (!trackedBehaviorCells.TryGetValue(species, out var tracked)
                    || !TryFindTrackedCell(source, species, tracked.EntityId, out tracked))
                {
                    tracked = FindYoungestCell(source, species);
                }

                trackedBehaviorCells[species] = tracked;
                var cell = source.GetCell(tracked.X, tracked.Y);
                trackedBehaviors[species] = new SpeciesTrackedBehavior(
                    species,
                    tracked.EntityId,
                    cell.Age,
                    tracked.X,
                    tracked.Y,
                    cell.BehaviorState,
                    cell.BehaviorStateTicks);
            }

            var extinctSpecies = new List<SpeciesId>();
            foreach (var tracked in trackedBehaviorCells)
            {
                if (!liveSpecies.Contains(tracked.Key))
                {
                    extinctSpecies.Add(tracked.Key);
                }
            }

            foreach (var species in extinctSpecies)
            {
                trackedBehaviorCells.Remove(species);
            }
        }

        internal bool IsTrackedBehaviorCell(SpeciesId species, int x, int y)
        {
            return trackedBehaviorCells.TryGetValue(species, out var tracked)
                && tracked.X == x
                && tracked.Y == y;
        }

        internal void RecordTrackedTransition(
            SpeciesId species,
            long entityId,
            int age,
            int x,
            int y,
            SpeciesBehaviorState previousState,
            SpeciesBehaviorState currentState)
        {
            if (species.IsValid)
            {
                trackedBehaviors[species] = new SpeciesTrackedBehavior(
                    species,
                    entityId,
                    age,
                    x,
                    y,
                    currentState,
                    stateTicks: 1);
                behaviorTransitions.Add(new SpeciesBehaviorTransition(
                    species,
                    entityId,
                    age,
                    x,
                    y,
                    currentTick,
                    previousState,
                    currentState));
            }
        }

        static bool TryFindTrackedCell(
            Grid<SpeciesCell> source,
            SpeciesId species,
            long entityId,
            out TrackedBehaviorCell tracked)
        {
            tracked = default;
            for (var y = 0; y < source.Height; y++)
            {
                for (var x = 0; x < source.Width; x++)
                {
                    var cell = source.GetCell(x, y);
                    if (!cell.IsCreature
                        || cell.SpeciesId != species
                        || cell.EntityId != entityId)
                    {
                        continue;
                    }

                    tracked = new TrackedBehaviorCell(x, y, cell.EntityId, cell.Age);
                    return true;
                }
            }

            return false;
        }

        static TrackedBehaviorCell FindYoungestCell(Grid<SpeciesCell> source, SpeciesId species)
        {
            var found = false;
            var youngest = default(TrackedBehaviorCell);
            for (var y = 0; y < source.Height; y++)
            {
                for (var x = 0; x < source.Width; x++)
                {
                    var cell = source.GetCell(x, y);
                    if (!cell.IsCreature || cell.SpeciesId != species)
                    {
                        continue;
                    }

                    if (!found || cell.Age < youngest.Age)
                    {
                        youngest = new TrackedBehaviorCell(x, y, cell.EntityId, cell.Age);
                        found = true;
                    }
                }
            }

            return youngest;
        }

        readonly struct TrackedBehaviorCell
        {
            public TrackedBehaviorCell(int x, int y, long entityId, int age)
            {
                X = x;
                Y = y;
                EntityId = entityId;
                Age = age;
            }

            public int X { get; }
            public int Y { get; }
            public long EntityId { get; }
            public int Age { get; }
        }

        internal void RecordState(
            SpeciesId species,
            SpeciesBehaviorState state,
            bool transitioned)
        {
            if (!species.IsValid)
            {
                return;
            }

            if (!stateTicksBySpecies.TryGetValue(species, out var stateTicks))
            {
                stateTicks = new Dictionary<SpeciesBehaviorState, int>();
                stateTicksBySpecies.Add(species, stateTicks);
            }

            stateTicks.TryGetValue(state, out var ticks);
            stateTicks[state] = ticks + 1;
            if (transitioned)
            {
                stateTransitionsBySpecies.TryGetValue(species, out var transitions);
                stateTransitionsBySpecies[species] = transitions + 1;
            }
        }

        internal void Record(
            SpeciesId species,
            int births = 0,
            float foodConsumed = 0f,
            int foodActionAttempts = 0,
            int foodActionSuccesses = 0,
            int foodActionFailures = 0,
            int movementSteps = 0,
            int damageDealt = 0,
            int combatKills = 0,
            int combatOpportunities = 0,
            int combatAttempts = 0,
            int combatHits = 0,
            int combatBlocked = 0,
            int combatDamageApplications = 0,
            int combatNonLethalHits = 0,
            int combatLethalHits = 0,
            int deaths = 0,
            int starvationDeaths = 0,
            int crowdingDeaths = 0,
            int wiltDeaths = 0,
            int populationLimitRemovals = 0)
        {
            if (!species.IsValid)
            {
                return;
            }

            activityBySpecies[species] = GetActivity(species).Add(
                births,
                foodConsumed,
                foodActionAttempts,
                foodActionSuccesses,
                foodActionFailures,
                movementSteps,
                damageDealt,
                combatKills,
                combatOpportunities,
                combatAttempts,
                combatHits,
                combatBlocked,
                combatDamageApplications,
                combatNonLethalHits,
                combatLethalHits,
                deaths,
                starvationDeaths,
                crowdingDeaths,
                wiltDeaths,
                populationLimitRemovals);
        }

        internal void RecordFoodAction(
            SpeciesId species,
            bool successful,
            float consumedAmount = 0f)
        {
            // Behavior-state ticks describe the pre-resolution decision. These
            // counters describe the resolver outcome and must reconcile.
            Record(
                species,
                foodConsumed: consumedAmount,
                foodActionAttempts: 1,
                foodActionSuccesses: successful ? 1 : 0,
                foodActionFailures: successful ? 0 : 1);
        }

        internal void RecordCombatOpportunity(SpeciesId attackerSpecies)
        {
            Record(attackerSpecies, combatOpportunities: 1);
        }

        internal void BeginHerbivoreExposureStep()
        {
            predatorActiveHerbivoreStepsThisStepBySpecies.Clear();
            encounteredHerbivoreSpeciesThisStep.Clear();
        }

        internal void RecordPredatorActiveHerbivoreStep(SpeciesId species)
        {
            if (!species.IsValid)
            {
                return;
            }

            predatorActiveHerbivoreStepsBySpecies.TryGetValue(species, out var steps);
            predatorActiveHerbivoreStepsBySpecies[species] = steps + 1;

            predatorActiveHerbivoreStepsThisStepBySpecies.TryGetValue(species, out var stepsThisStep);
            predatorActiveHerbivoreStepsThisStepBySpecies[species] = stepsThisStep + 1;
        }

        internal void RecordHerbivoreEncounter(SpeciesId species)
        {
            if (!species.IsValid)
            {
                return;
            }

            herbivoreEncountersBySpecies.TryGetValue(species, out var encounters);
            herbivoreEncountersBySpecies[species] = encounters + 1;
            if (encounteredHerbivoreSpeciesThisStep.Add(species)
                && predatorActiveHerbivoreStepsThisStepBySpecies.TryGetValue(
                    species,
                    out var stepsThisStep))
            {
                encounteredHerbivoreStepsBySpecies.TryGetValue(species, out var steps);
                encounteredHerbivoreStepsBySpecies[species] = steps + stepsThisStep;
            }
        }

        internal void RecordHerbivorePreyed(SpeciesId species)
        {
            if (!species.IsValid)
            {
                return;
            }

            herbivorePreyedBySpecies.TryGetValue(species, out var preyed);
            herbivorePreyedBySpecies[species] = preyed + 1;
        }

        public SpeciesHerbivoreStatLine CreateHerbivoreStatLine(
            SpeciesId species,
            int startingPopulation,
            int finalPopulation)
        {
            var starved = 0;
            var crowding = 0;
            foreach (var death in deathEvents)
            {
                if (!death.IsCreature || death.Species != species)
                {
                    continue;
                }

                if (death.Cause == SpeciesDeathCause.Starvation)
                {
                    starved++;
                }

                if (death.Cause == SpeciesDeathCause.Crowding)
                {
                    crowding++;
                }
            }

            return new SpeciesHerbivoreStatLine(
                species,
                startingPopulation,
                GetPredatorActiveHerbivoreSteps(species),
                GetEncounteredHerbivoreSteps(species),
                GetHerbivoreEncounters(species),
                GetHerbivorePreyed(species),
                starved,
                GetReproductionActivity(species).Candidates,
                GetActivity(species).Births,
                crowding,
                finalPopulation);
        }

        internal void RecordControlledOpportunityScheduled()
        {
            controlledOpportunityScheduled++;
        }

        internal void RecordControlledOpportunityEligible()
        {
            controlledOpportunityEligible++;
        }

        internal void RecordControlledOpportunityUnfulfilledNoTarget()
        {
            controlledOpportunityUnfulfilledNoTarget++;
        }

        internal void RecordControlledOpportunityUnfulfilledInvalidated()
        {
            controlledOpportunityUnfulfilledInvalidated++;
        }

        internal void RecordCombatAttempt(
            SpeciesId attackerSpecies,
            bool hit,
            bool blocked,
            int damageDealt,
            bool lethal)
        {
            Record(
                attackerSpecies,
                combatAttempts: 1,
                combatHits: hit ? 1 : 0,
                combatBlocked: blocked ? 1 : 0,
                combatDamageApplications: damageDealt > 0 ? 1 : 0,
                combatNonLethalHits: damageDealt > 0 && !lethal ? 1 : 0,
                combatLethalHits: lethal ? 1 : 0);
        }

        internal void RecordCombatRoll(
            SpeciesId attackerSpecies,
            SpeciesId targetSpecies,
            int attackRoll,
            int attackModifier,
            int blockRoll,
            int blockModifier,
            bool hit)
        {
            combatRollEvents.Add(new SpeciesCombatRollEvent(
                attackerSpecies,
                targetSpecies,
                currentTick,
                attackRoll,
                attackModifier,
                blockRoll,
                blockModifier,
                hit));
        }

        internal void RecordCombatCooldownSuppressed(
            SpeciesId attackerSpecies,
            long entityId,
            int x,
            int y,
            int remainingTicks)
        {
            combatCooldownSuppressionEvents.Add(new SpeciesCombatCooldownSuppressionEvent(
                attackerSpecies,
                entityId,
                x,
                y,
                currentTick,
                remainingTicks));
        }

        internal void RecordReproductionOutcome(
            SpeciesId species,
            SpeciesReproductionOutcome outcome)
        {
            if (!species.IsValid)
            {
                return;
            }

            reproductionBySpecies[species] = GetReproductionActivity(species).Add(outcome);
        }

        internal void RecordDeath(
            SpeciesCell cell,
            int x,
            int y,
            SpeciesDeathCause cause,
            int populationLimitRemovals = 0)
        {
            var species = cell.IsPlantResource && !cell.IsCreature
                ? (cell.ResourceSpeciesId.IsValid ? cell.ResourceSpeciesId : cell.SpeciesId)
                : cell.SpeciesId;
            if (!species.IsValid)
            {
                return;
            }

            Record(
                species,
                deaths: 1,
                starvationDeaths: cause == SpeciesDeathCause.Starvation ? 1 : 0,
                crowdingDeaths: cause == SpeciesDeathCause.Crowding ? 1 : 0,
                wiltDeaths: cause == SpeciesDeathCause.Wilt ? 1 : 0,
                populationLimitRemovals: populationLimitRemovals);
            deathEvents.Add(new SpeciesDeathEvent(
                species,
                cell.EntityId,
                cell.Age,
                x,
                y,
                currentTick,
                cause,
                cell.IsCreature));
        }

        internal SpeciesSimulationMetricsSnapshot CreateSnapshot()
        {
            return new SpeciesSimulationMetricsSnapshot(
                activityBySpecies,
                reproductionBySpecies,
                stateTicksBySpecies,
                stateTransitionsBySpecies,
                herbivoreEncountersBySpecies,
                herbivorePreyedBySpecies,
                predatorActiveHerbivoreStepsBySpecies,
                encounteredHerbivoreStepsBySpecies,
                controlledOpportunityScheduled,
                controlledOpportunityEligible,
                controlledOpportunityUnfulfilledNoTarget,
                controlledOpportunityUnfulfilledInvalidated);
        }

        internal SpeciesSimulationMetricsWindow CreateWindow(
            SpeciesSimulationMetricsSnapshot baseline,
            int startTickExclusive,
            int endTickInclusive)
        {
            var species = new HashSet<SpeciesId>(activityBySpecies.Keys);
            species.UnionWith(reproductionBySpecies.Keys);
            species.UnionWith(stateTicksBySpecies.Keys);
            species.UnionWith(stateTransitionsBySpecies.Keys);
            species.UnionWith(herbivoreEncountersBySpecies.Keys);
            species.UnionWith(herbivorePreyedBySpecies.Keys);
            species.UnionWith(predatorActiveHerbivoreStepsBySpecies.Keys);
            species.UnionWith(encounteredHerbivoreStepsBySpecies.Keys);

            var activity = new Dictionary<SpeciesId, SpeciesSimulationActivity>();
            var reproduction = new Dictionary<SpeciesId, SpeciesReproductionActivity>();
            var stateTicks = new Dictionary<SpeciesId, Dictionary<SpeciesBehaviorState, int>>();
            var stateTransitions = new Dictionary<SpeciesId, int>();
            var herbivoreEncounters = new Dictionary<SpeciesId, int>();
            var herbivorePreyed = new Dictionary<SpeciesId, int>();
            var predatorActiveHerbivoreSteps = new Dictionary<SpeciesId, int>();
            var encounteredHerbivoreSteps = new Dictionary<SpeciesId, int>();
            foreach (var speciesId in species)
            {
                activity[speciesId] = GetActivity(speciesId).Subtract(baseline.GetActivity(speciesId));
                reproduction[speciesId] = GetReproductionActivity(speciesId).Subtract(baseline.GetReproductionActivity(speciesId));
                var states = new Dictionary<SpeciesBehaviorState, int>();
                foreach (SpeciesBehaviorState state in Enum.GetValues(typeof(SpeciesBehaviorState)))
                {
                    var current = GetStateTicks(speciesId, state);
                    var before = baseline.GetStateTicks(speciesId, state);
                    if (current - before != 0)
                    {
                        states[state] = current - before;
                    }
                }

                stateTicks[speciesId] = states;
                stateTransitions[speciesId] = GetStateTransitions(speciesId) - baseline.GetStateTransitions(speciesId);
                herbivoreEncounters[speciesId] = GetHerbivoreEncounters(speciesId) - baseline.GetHerbivoreEncounters(speciesId);
                herbivorePreyed[speciesId] = GetHerbivorePreyed(speciesId) - baseline.GetHerbivorePreyed(speciesId);
                predatorActiveHerbivoreSteps[speciesId] = GetPredatorActiveHerbivoreSteps(speciesId)
                    - baseline.GetPredatorActiveHerbivoreSteps(speciesId);
                encounteredHerbivoreSteps[speciesId] = GetEncounteredHerbivoreSteps(speciesId)
                    - baseline.GetEncounteredHerbivoreSteps(speciesId);
            }

            var filteredDeaths = deathEvents.FindAll(value => value.Tick > startTickExclusive && value.Tick <= endTickInclusive);
            var filteredRolls = combatRollEvents.FindAll(value => value.Tick > startTickExclusive && value.Tick <= endTickInclusive);
            var filteredSuppressions = combatCooldownSuppressionEvents.FindAll(
                value => value.Tick > startTickExclusive && value.Tick <= endTickInclusive);
            var filteredTransitions = behaviorTransitions.FindAll(
                value => value.Tick > startTickExclusive && value.Tick <= endTickInclusive);
            var tracked = new Dictionary<SpeciesId, SpeciesTrackedBehavior>(trackedBehaviors);
            return new SpeciesSimulationMetricsWindow(
                activity,
                reproduction,
                stateTicks,
                stateTransitions,
                herbivoreEncounters,
                herbivorePreyed,
                predatorActiveHerbivoreSteps,
                encounteredHerbivoreSteps,
                controlledOpportunityScheduled - baseline.ControlledOpportunityScheduled,
                controlledOpportunityEligible - baseline.ControlledOpportunityEligible,
                controlledOpportunityUnfulfilledNoTarget - baseline.ControlledOpportunityUnfulfilledNoTarget,
                controlledOpportunityUnfulfilledInvalidated - baseline.ControlledOpportunityUnfulfilledInvalidated,
                filteredTransitions,
                filteredDeaths,
                filteredRolls,
                filteredSuppressions,
                tracked);
        }
    }

    internal sealed class SpeciesSimulationMetricsSnapshot
    {
        readonly IReadOnlyDictionary<SpeciesId, SpeciesSimulationActivity> activity;
        readonly IReadOnlyDictionary<SpeciesId, SpeciesReproductionActivity> reproduction;
        readonly IReadOnlyDictionary<SpeciesId, IReadOnlyDictionary<SpeciesBehaviorState, int>> stateTicks;
        readonly IReadOnlyDictionary<SpeciesId, int> stateTransitions;
        readonly IReadOnlyDictionary<SpeciesId, int> herbivoreEncounters;
        readonly IReadOnlyDictionary<SpeciesId, int> herbivorePreyed;
        readonly IReadOnlyDictionary<SpeciesId, int> predatorActiveHerbivoreSteps;
        readonly IReadOnlyDictionary<SpeciesId, int> encounteredHerbivoreSteps;

        internal SpeciesSimulationMetricsSnapshot(
            IReadOnlyDictionary<SpeciesId, SpeciesSimulationActivity> activity,
            IReadOnlyDictionary<SpeciesId, SpeciesReproductionActivity> reproduction,
            IReadOnlyDictionary<SpeciesId, Dictionary<SpeciesBehaviorState, int>> stateTicks,
            IReadOnlyDictionary<SpeciesId, int> stateTransitions,
            IReadOnlyDictionary<SpeciesId, int> herbivoreEncounters,
            IReadOnlyDictionary<SpeciesId, int> herbivorePreyed,
            IReadOnlyDictionary<SpeciesId, int> predatorActiveHerbivoreSteps,
            IReadOnlyDictionary<SpeciesId, int> encounteredHerbivoreSteps,
            int controlledOpportunityScheduled,
            int controlledOpportunityEligible,
            int controlledOpportunityUnfulfilledNoTarget,
            int controlledOpportunityUnfulfilledInvalidated)
        {
            this.activity = new Dictionary<SpeciesId, SpeciesSimulationActivity>(activity);
            this.reproduction = new Dictionary<SpeciesId, SpeciesReproductionActivity>(reproduction);
            var stateCopy = new Dictionary<SpeciesId, IReadOnlyDictionary<SpeciesBehaviorState, int>>();
            foreach (var entry in stateTicks)
            {
                stateCopy[entry.Key] = new Dictionary<SpeciesBehaviorState, int>(entry.Value);
            }

            this.stateTicks = stateCopy;
            this.stateTransitions = new Dictionary<SpeciesId, int>(stateTransitions);
            this.herbivoreEncounters = new Dictionary<SpeciesId, int>(herbivoreEncounters);
            this.herbivorePreyed = new Dictionary<SpeciesId, int>(herbivorePreyed);
            this.predatorActiveHerbivoreSteps = new Dictionary<SpeciesId, int>(predatorActiveHerbivoreSteps);
            this.encounteredHerbivoreSteps = new Dictionary<SpeciesId, int>(encounteredHerbivoreSteps);
            ControlledOpportunityScheduled = controlledOpportunityScheduled;
            ControlledOpportunityEligible = controlledOpportunityEligible;
            ControlledOpportunityUnfulfilledNoTarget = controlledOpportunityUnfulfilledNoTarget;
            ControlledOpportunityUnfulfilledInvalidated = controlledOpportunityUnfulfilledInvalidated;
        }

        public int ControlledOpportunityScheduled { get; }
        public int ControlledOpportunityEligible { get; }
        public int ControlledOpportunityUnfulfilledNoTarget { get; }
        public int ControlledOpportunityUnfulfilledInvalidated { get; }

        public SpeciesSimulationActivity GetActivity(SpeciesId species)
        {
            return activity.TryGetValue(species, out var value) ? value : default;
        }

        public SpeciesReproductionActivity GetReproductionActivity(SpeciesId species)
        {
            return reproduction.TryGetValue(species, out var value) ? value : default;
        }

        public int GetStateTicks(SpeciesId species, SpeciesBehaviorState state)
        {
            return stateTicks.TryGetValue(species, out var values)
                && values.TryGetValue(state, out var value)
                ? value
                : 0;
        }

        public int GetStateTransitions(SpeciesId species)
        {
            return stateTransitions.TryGetValue(species, out var value) ? value : 0;
        }

        public int GetHerbivoreEncounters(SpeciesId species)
        {
            return herbivoreEncounters.TryGetValue(species, out var value) ? value : 0;
        }

        public int GetHerbivorePreyed(SpeciesId species)
        {
            return herbivorePreyed.TryGetValue(species, out var value) ? value : 0;
        }

        public int GetPredatorActiveHerbivoreSteps(SpeciesId species)
        {
            return predatorActiveHerbivoreSteps.TryGetValue(species, out var value) ? value : 0;
        }

        public int GetEncounteredHerbivoreSteps(SpeciesId species)
        {
            return encounteredHerbivoreSteps.TryGetValue(species, out var value) ? value : 0;
        }
    }

    public sealed class SpeciesSimulationMetricsWindow : ISpeciesSimulationMetricsView
    {
        readonly IReadOnlyDictionary<SpeciesId, SpeciesSimulationActivity> activity;
        readonly IReadOnlyDictionary<SpeciesId, SpeciesReproductionActivity> reproduction;
        readonly IReadOnlyDictionary<SpeciesId, IReadOnlyDictionary<SpeciesBehaviorState, int>> stateTicks;
        readonly IReadOnlyDictionary<SpeciesId, int> stateTransitions;
        readonly IReadOnlyDictionary<SpeciesId, int> herbivoreEncounters;
        readonly IReadOnlyDictionary<SpeciesId, int> herbivorePreyed;
        readonly IReadOnlyDictionary<SpeciesId, int> predatorActiveHerbivoreSteps;
        readonly IReadOnlyDictionary<SpeciesId, int> encounteredHerbivoreSteps;
        readonly IReadOnlyDictionary<SpeciesId, SpeciesTrackedBehavior> trackedBehaviors;

        internal SpeciesSimulationMetricsWindow(
            IReadOnlyDictionary<SpeciesId, SpeciesSimulationActivity> activity,
            IReadOnlyDictionary<SpeciesId, SpeciesReproductionActivity> reproduction,
            IReadOnlyDictionary<SpeciesId, Dictionary<SpeciesBehaviorState, int>> stateTicks,
            IReadOnlyDictionary<SpeciesId, int> stateTransitions,
            IReadOnlyDictionary<SpeciesId, int> herbivoreEncounters,
            IReadOnlyDictionary<SpeciesId, int> herbivorePreyed,
            IReadOnlyDictionary<SpeciesId, int> predatorActiveHerbivoreSteps,
            IReadOnlyDictionary<SpeciesId, int> encounteredHerbivoreSteps,
            int controlledOpportunityScheduled,
            int controlledOpportunityEligible,
            int controlledOpportunityUnfulfilledNoTarget,
            int controlledOpportunityUnfulfilledInvalidated,
            IReadOnlyList<SpeciesBehaviorTransition> behaviorTransitions,
            IReadOnlyList<SpeciesDeathEvent> deathEvents,
            IReadOnlyList<SpeciesCombatRollEvent> combatRollEvents,
            IReadOnlyList<SpeciesCombatCooldownSuppressionEvent> combatCooldownSuppressionEvents,
            IReadOnlyDictionary<SpeciesId, SpeciesTrackedBehavior> trackedBehaviors)
        {
            this.activity = new System.Collections.ObjectModel.ReadOnlyDictionary<SpeciesId, SpeciesSimulationActivity>(
                new Dictionary<SpeciesId, SpeciesSimulationActivity>(activity));
            this.reproduction = new System.Collections.ObjectModel.ReadOnlyDictionary<SpeciesId, SpeciesReproductionActivity>(
                new Dictionary<SpeciesId, SpeciesReproductionActivity>(reproduction));
            var stateCopy = new Dictionary<SpeciesId, IReadOnlyDictionary<SpeciesBehaviorState, int>>();
            foreach (var entry in stateTicks)
            {
                stateCopy[entry.Key] = new System.Collections.ObjectModel.ReadOnlyDictionary<SpeciesBehaviorState, int>(
                    new Dictionary<SpeciesBehaviorState, int>(entry.Value));
            }

            this.stateTicks = new System.Collections.ObjectModel.ReadOnlyDictionary<SpeciesId, IReadOnlyDictionary<SpeciesBehaviorState, int>>(stateCopy);
            this.stateTransitions = new System.Collections.ObjectModel.ReadOnlyDictionary<SpeciesId, int>(
                new Dictionary<SpeciesId, int>(stateTransitions));
            this.herbivoreEncounters = new System.Collections.ObjectModel.ReadOnlyDictionary<SpeciesId, int>(
                new Dictionary<SpeciesId, int>(herbivoreEncounters));
            this.herbivorePreyed = new System.Collections.ObjectModel.ReadOnlyDictionary<SpeciesId, int>(
                new Dictionary<SpeciesId, int>(herbivorePreyed));
            this.predatorActiveHerbivoreSteps = new System.Collections.ObjectModel.ReadOnlyDictionary<SpeciesId, int>(
                new Dictionary<SpeciesId, int>(predatorActiveHerbivoreSteps));
            this.encounteredHerbivoreSteps = new System.Collections.ObjectModel.ReadOnlyDictionary<SpeciesId, int>(
                new Dictionary<SpeciesId, int>(encounteredHerbivoreSteps));
            this.trackedBehaviors = new System.Collections.ObjectModel.ReadOnlyDictionary<SpeciesId, SpeciesTrackedBehavior>(
                new Dictionary<SpeciesId, SpeciesTrackedBehavior>(trackedBehaviors));
            BehaviorTransitions = new List<SpeciesBehaviorTransition>(behaviorTransitions).AsReadOnly();
            DeathEvents = new List<SpeciesDeathEvent>(deathEvents).AsReadOnly();
            CombatRollEvents = new List<SpeciesCombatRollEvent>(combatRollEvents).AsReadOnly();
            CombatCooldownSuppressionEvents = new List<SpeciesCombatCooldownSuppressionEvent>(combatCooldownSuppressionEvents).AsReadOnly();
            ControlledOpportunityScheduled = controlledOpportunityScheduled;
            ControlledOpportunityEligible = controlledOpportunityEligible;
            ControlledOpportunityUnfulfilledNoTarget = controlledOpportunityUnfulfilledNoTarget;
            ControlledOpportunityUnfulfilledInvalidated = controlledOpportunityUnfulfilledInvalidated;
        }

        public const int ContractVersion = 1;
        public int ControlledOpportunityScheduled { get; }
        public int ControlledOpportunityEligible { get; }
        public int ControlledOpportunityUnfulfilledNoTarget { get; }
        public int ControlledOpportunityUnfulfilledInvalidated { get; }
        public IReadOnlyList<SpeciesBehaviorTransition> BehaviorTransitions { get; }
        public IReadOnlyList<SpeciesDeathEvent> DeathEvents { get; }
        public IReadOnlyList<SpeciesCombatRollEvent> CombatRollEvents { get; }
        public IReadOnlyList<SpeciesCombatCooldownSuppressionEvent> CombatCooldownSuppressionEvents { get; }
        public IReadOnlyDictionary<SpeciesId, SpeciesTrackedBehavior> TrackedBehaviors => trackedBehaviors;

        public SpeciesSimulationActivity GetActivity(SpeciesId species)
        {
            return activity.TryGetValue(species, out var value) ? value : default;
        }

        public SpeciesReproductionActivity GetReproductionActivity(SpeciesId species)
        {
            return reproduction.TryGetValue(species, out var value) ? value : default;
        }

        public int GetStateTicks(SpeciesId species, SpeciesBehaviorState state)
        {
            return stateTicks.TryGetValue(species, out var values)
                && values.TryGetValue(state, out var value)
                ? value
                : 0;
        }

        public int GetStateTransitions(SpeciesId species)
        {
            return stateTransitions.TryGetValue(species, out var value) ? value : 0;
        }

        public int GetHerbivoreEncounters(SpeciesId species)
        {
            return herbivoreEncounters.TryGetValue(species, out var value) ? value : 0;
        }

        public int GetHerbivorePreyed(SpeciesId species)
        {
            return herbivorePreyed.TryGetValue(species, out var value) ? value : 0;
        }

        public int GetPredatorActiveHerbivoreSteps(SpeciesId species)
        {
            return predatorActiveHerbivoreSteps.TryGetValue(species, out var value) ? value : 0;
        }

        public int GetEncounteredHerbivoreSteps(SpeciesId species)
        {
            return encounteredHerbivoreSteps.TryGetValue(species, out var value) ? value : 0;
        }

        public bool TryGetTrackedBehavior(SpeciesId species, out SpeciesTrackedBehavior behavior)
        {
            return trackedBehaviors.TryGetValue(species, out behavior);
        }

        public SpeciesHerbivoreStatLine CreateHerbivoreStatLine(
            SpeciesId species,
            int startingPopulation,
            int finalPopulation)
        {
            var starved = 0;
            var crowding = 0;
            foreach (var death in DeathEvents)
            {
                if (!death.IsCreature || death.Species != species)
                {
                    continue;
                }

                if (death.Cause == SpeciesDeathCause.Starvation)
                {
                    starved++;
                }

                if (death.Cause == SpeciesDeathCause.Crowding)
                {
                    crowding++;
                }
            }

            return new SpeciesHerbivoreStatLine(
                species,
                startingPopulation,
                GetPredatorActiveHerbivoreSteps(species),
                GetEncounteredHerbivoreSteps(species),
                GetHerbivoreEncounters(species),
                GetHerbivorePreyed(species),
                starved,
                GetReproductionActivity(species).Candidates,
                GetActivity(species).Births,
                crowding,
                finalPopulation);
        }
    }
}
