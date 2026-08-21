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
        Fleeing,
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
    }

    public readonly struct SpeciesBehaviorTransition
    {
        internal SpeciesBehaviorTransition(
            SpeciesId species,
            long entityId,
            int age,
            int x,
            int y,
            SpeciesBehaviorState previousState,
            SpeciesBehaviorState currentState)
        {
            Species = species;
            EntityId = entityId;
            Age = age;
            X = x;
            Y = y;
            PreviousState = previousState;
            CurrentState = currentState;
        }

        public SpeciesId Species { get; }
        public long EntityId { get; }
        public int Age { get; }
        public int X { get; }
        public int Y { get; }
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
    }

    public sealed class SpeciesSimulationMetrics
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
        readonly List<SpeciesBehaviorTransition> behaviorTransitions =
            new List<SpeciesBehaviorTransition>();
        readonly List<SpeciesDeathEvent> deathEvents =
            new List<SpeciesDeathEvent>();
        readonly List<SpeciesCombatRollEvent> combatRollEvents =
            new List<SpeciesCombatRollEvent>();
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

        public IReadOnlyList<SpeciesBehaviorTransition> BehaviorTransitions => behaviorTransitions;
        public IReadOnlyList<SpeciesDeathEvent> DeathEvents => deathEvents;
        public IReadOnlyList<SpeciesCombatRollEvent> CombatRollEvents => combatRollEvents;
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
            behaviorTransitions.Clear();
            deathEvents.Clear();
            combatRollEvents.Clear();
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
    }
}
