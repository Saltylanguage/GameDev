using System;
using System.Threading;

namespace SaltyGame
{
    [Obsolete("Use TerrainId and TerrainDefinition instead.")]
    public enum SpeciesTerrain
    {
        Bare,
        Grass,
    }

    public readonly struct SpeciesCell
    {
        public SpeciesCell(
            SpeciesId species,
            int health = 1,
            int energy = 0,
            int age = 0,
            int foodEaten = 0,
            float foodReserve = 0f,
            bool isAlpha = false)
            : this(
                species,
                true,
                health,
                energy,
                age,
                foodEaten,
                foodReserve,
                isAlpha,
                TerrainIds.Bare,
                terrainEnergy: 0f,
                isResourceSpecies: species == SpeciesIds.Plant,
                isResourceTerrain: false,
                isPassable: true,
                movementCost: 1f,
                resourceSpeciesId: species == SpeciesIds.Plant ? species : default,
                behaviorState: SpeciesBehaviorState.Wandering,
                behaviorStateTicks: 0,
                entityId: species == SpeciesIds.Plant ? 0L : AllocateEntityId())
        {
        }

        SpeciesCell(
            SpeciesId species,
            bool isOccupied,
            int health,
            int energy,
            int age,
            int foodEaten,
            float foodReserve,
            bool isAlpha,
            TerrainId terrainId,
            float terrainEnergy,
            bool isResourceSpecies,
            bool isResourceTerrain,
            bool isPassable,
            float movementCost,
            SpeciesId resourceSpeciesId = default,
            SpeciesBehaviorState behaviorState = SpeciesBehaviorState.Wandering,
            int behaviorStateTicks = 0,
            long entityId = 0,
            int attackCooldownTicksRemaining = 0,
            float energyRemainder = 0f,
            long trackingTargetEntityId = 0,
            int trackingTargetX = 0,
            int trackingTargetY = 0,
            int trackingTicksRemaining = 0)
        {
            if (health < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(health), health, "Health cannot be negative.");
            }

            if (energy < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(energy), energy, "Energy cannot be negative.");
            }

            if (energyRemainder < 0f || energyRemainder >= 1f || float.IsNaN(energyRemainder) || float.IsInfinity(energyRemainder))
            {
                throw new ArgumentOutOfRangeException(nameof(energyRemainder), energyRemainder, "Energy remainder must be between zero and one.");
            }

            if (age < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(age), age, "Age cannot be negative.");
            }

            if (foodEaten < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(foodEaten), foodEaten, "Food eaten cannot be negative.");
            }

            if (foodReserve < 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(foodReserve), foodReserve, "Food reserve cannot be negative.");
            }

            if (terrainEnergy < 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(terrainEnergy), terrainEnergy, "Terrain energy cannot be negative.");
            }

            if (movementCost <= 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(movementCost), movementCost, "Movement cost must be greater than zero.");
            }

            if (behaviorStateTicks < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(behaviorStateTicks),
                    behaviorStateTicks,
                    "Behavior state ticks cannot be negative.");
            }

            if (attackCooldownTicksRemaining < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(attackCooldownTicksRemaining),
                    attackCooldownTicksRemaining,
                    "Attack cooldown ticks cannot be negative.");
            }

            if (trackingTargetEntityId < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(trackingTargetEntityId),
                    trackingTargetEntityId,
                    "Tracking target entity id cannot be negative.");
            }

            if (trackingTicksRemaining < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(trackingTicksRemaining),
                    trackingTicksRemaining,
                    "Tracking ticks cannot be negative.");
            }

            IsOccupied = isOccupied;
            SpeciesId = species;
            Health = health;
            Energy = energy;
            EnergyRemainder = energyRemainder;
            Age = age;
            FoodEaten = foodEaten;
            FoodReserve = foodReserve;
            IsAlpha = isAlpha && isOccupied && !isResourceSpecies;
            this.terrainId = terrainId;
            TerrainEnergy = terrainEnergy;
            this.isResourceSpecies = isResourceSpecies;
            this.isResourceTerrain = isResourceTerrain;
            this.resourceSpeciesId = resourceSpeciesId;
            this.isPassable = isPassable;
            this.movementCost = movementCost;
            BehaviorState = behaviorState;
            BehaviorStateTicks = behaviorStateTicks;
            AttackCooldownTicksRemaining = attackCooldownTicksRemaining;
            TrackingTargetEntityId = isOccupied && !isResourceSpecies ? trackingTargetEntityId : 0L;
            TrackingTargetX = TrackingTargetEntityId > 0 ? trackingTargetX : 0;
            TrackingTargetY = TrackingTargetEntityId > 0 ? trackingTargetY : 0;
            TrackingTicksRemaining = TrackingTargetEntityId > 0 ? trackingTicksRemaining : 0;
            EntityId = isOccupied && !isResourceSpecies
                ? entityId > 0 ? entityId : AllocateEntityId()
                : 0L;
        }

        readonly bool isResourceSpecies;
        readonly bool isResourceTerrain;
        readonly SpeciesId resourceSpeciesId;
        readonly TerrainId terrainId;
        readonly bool isPassable;
        readonly float movementCost;
        static long nextEntityId;

        static long AllocateEntityId()
        {
            return Interlocked.Increment(ref nextEntityId);
        }

        public static SpeciesCell Empty => new SpeciesCell(
            default,
            false,
            0,
            0,
            0,
            0,
            0f,
            false,
            TerrainIds.Bare,
            0f,
                isResourceSpecies: false,
                isResourceTerrain: false,
                isPassable: true,
                movementCost: 1f);

        public static SpeciesCell Grass(float energy)
        {
            return Grass(SpeciesIds.Plant, energy);
        }

        public static SpeciesCell Grass(SpeciesId resourceSpecies, float energy)
        {
            return FromTerrain(TerrainDefaults.Grass, energy, resourceSpecies);
        }

        public static SpeciesCell FromTerrain(
            TerrainDefinition definition,
            float energy = 0f,
            SpeciesId resourceSpecies = default)
        {
            if (definition == null)
            {
                throw new ArgumentNullException(nameof(definition));
            }

            return new SpeciesCell(
                resourceSpecies,
                false,
                0,
                0,
                0,
                0,
                0f,
                false,
                definition.Id,
                energy,
                isResourceSpecies: false,
                isResourceTerrain: definition.ProvidesResource,
                isPassable: definition.IsPassable,
                movementCost: definition.MovementCost,
                resourceSpeciesId: resourceSpecies);
        }

        public bool IsOccupied { get; }
        public bool IsCreature => IsOccupied && !isResourceSpecies;
        public bool IsTerrainResource => isResourceTerrain && TerrainEnergy > 0f;
        public bool IsPlantResource => IsTerrainResource
            || (IsOccupied && isResourceSpecies);
        public SpeciesId ResourceSpeciesId => isResourceTerrain ? resourceSpeciesId : default;
        public bool IsPassable => !terrainId.IsValid || isPassable;
        public float MovementCost => terrainId.IsValid ? movementCost : 1f;
        public SpeciesId SpeciesId { get; }
        public long EntityId { get; }

        [Obsolete("Use SpeciesId instead.")]
        public SpeciesArchetype Species => SpeciesId.ToLegacyArchetype(SpeciesId);

        public TerrainId TerrainId => terrainId.IsValid ? terrainId : TerrainIds.Bare;

        [Obsolete("Use TerrainId instead.")]
        public SpeciesTerrain Terrain => TerrainId == TerrainIds.Grass
            ? SpeciesTerrain.Grass
            : SpeciesTerrain.Bare;

        public bool IsGrass => TerrainId == TerrainIds.Grass;
        public float TerrainEnergy { get; }
        public int Health { get; }
        public int Energy { get; }
        public float EnergyRemainder { get; }
        public int Age { get; }
        public int FoodEaten { get; }
        public float FoodReserve { get; }
        public bool IsAlpha { get; }
        public SpeciesBehaviorState BehaviorState { get; }
        public int BehaviorStateTicks { get; }
        public int AttackCooldownTicksRemaining { get; }
        public long TrackingTargetEntityId { get; }
        public int TrackingTargetX { get; }
        public int TrackingTargetY { get; }
        public int TrackingTicksRemaining { get; }

        public SpeciesCell WithEntity(
            SpeciesId species,
            int health,
            int energy,
            int age,
            int foodEaten,
            float foodReserve,
            bool isAlpha = false,
            long entityId = 0,
            float? energyRemainder = null)
        {
            var resolvedEntityId = entityId > 0
                ? entityId
                : IsCreature && SpeciesId == species
                    ? EntityId
                    : AllocateEntityId();
            var resolvedEnergyRemainder = energyRemainder ?? (IsCreature && SpeciesId == species ? EnergyRemainder : 0f);
            var preserveTracking = IsCreature
                && SpeciesId == species
                && resolvedEntityId == EntityId;
            return new SpeciesCell(
                species,
                true,
                health,
                energy,
                age,
                foodEaten,
                foodReserve,
                isAlpha,
                TerrainId,
                TerrainEnergy,
                isResourceSpecies: species == SpeciesIds.Plant,
                isResourceTerrain,
                IsPassable,
                MovementCost,
                resourceSpeciesId: resourceSpeciesId,
                behaviorState: BehaviorState,
                behaviorStateTicks: BehaviorStateTicks,
                entityId: resolvedEntityId,
                attackCooldownTicksRemaining: AttackCooldownTicksRemaining,
                energyRemainder: resolvedEnergyRemainder,
                trackingTargetEntityId: preserveTracking ? TrackingTargetEntityId : 0L,
                trackingTargetX: preserveTracking ? TrackingTargetX : 0,
                trackingTargetY: preserveTracking ? TrackingTargetY : 0,
                trackingTicksRemaining: preserveTracking ? TrackingTicksRemaining : 0);
        }

        public SpeciesCell WithBehaviorState(SpeciesBehaviorState state, int ticks = 0)
        {
            if (!IsCreature)
            {
                return this;
            }

            return new SpeciesCell(
                SpeciesId,
                true,
                Health,
                Energy,
                Age,
                FoodEaten,
                FoodReserve,
                IsAlpha,
                TerrainId,
                TerrainEnergy,
                isResourceSpecies,
                isResourceTerrain,
                IsPassable,
                MovementCost,
                resourceSpeciesId,
                state,
                ticks,
                EntityId,
                AttackCooldownTicksRemaining,
                EnergyRemainder,
                TrackingTargetEntityId,
                TrackingTargetX,
                TrackingTargetY,
                TrackingTicksRemaining);
        }

        public SpeciesCell WithAttackCooldown(int ticks)
        {
            if (ticks < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(ticks), ticks, "Attack cooldown ticks cannot be negative.");
            }

            if (!IsCreature)
            {
                return this;
            }

            return new SpeciesCell(
                SpeciesId,
                true,
                Health,
                Energy,
                Age,
                FoodEaten,
                FoodReserve,
                IsAlpha,
                TerrainId,
                TerrainEnergy,
                isResourceSpecies,
                isResourceTerrain,
                IsPassable,
                MovementCost,
                resourceSpeciesId,
                BehaviorState,
                BehaviorStateTicks,
                EntityId,
                ticks,
                EnergyRemainder,
                TrackingTargetEntityId,
                TrackingTargetX,
                TrackingTargetY,
                TrackingTicksRemaining);
        }

        public SpeciesCell WithTrackingTarget(long entityId, int x, int y, int ticksRemaining)
        {
            if (!IsCreature)
            {
                return this;
            }

            if (entityId <= 0 || ticksRemaining <= 0)
            {
                entityId = 0;
                x = 0;
                y = 0;
                ticksRemaining = 0;
            }

            return new SpeciesCell(
                SpeciesId,
                true,
                Health,
                Energy,
                Age,
                FoodEaten,
                FoodReserve,
                IsAlpha,
                TerrainId,
                TerrainEnergy,
                isResourceSpecies,
                isResourceTerrain,
                IsPassable,
                MovementCost,
                resourceSpeciesId,
                BehaviorState,
                BehaviorStateTicks,
                EntityId,
                AttackCooldownTicksRemaining,
                EnergyRemainder,
                entityId,
                x,
                y,
                ticksRemaining);
        }

        public SpeciesCell WithoutEntity()
        {
            return TerrainId == TerrainIds.Bare
                ? Empty
                : new SpeciesCell(
                    resourceSpeciesId.IsValid ? resourceSpeciesId : SpeciesId,
                    false,
                    0,
                    0,
                    0,
                    0,
                    0f,
                false,
                    TerrainId,
                    TerrainEnergy,
                    isResourceSpecies: false,
                    isResourceTerrain,
                    IsPassable,
                    MovementCost,
                    resourceSpeciesId: resourceSpeciesId);
        }

        public SpeciesCell WithoutPlantResource()
        {
            if (isResourceTerrain)
            {
                return WithTerrainEnergy(0f);
            }

            return IsOccupied && isResourceSpecies ? WithoutEntity() : this;
        }

        public SpeciesCell WithAlpha(int healthBonus, int energyBonus)
        {
            if (!IsCreature || healthBonus < 0 || energyBonus < 0)
            {
                return this;
            }

            return WithEntity(
                SpeciesId,
                Health + healthBonus,
                Energy + energyBonus,
                Age,
                FoodEaten,
                FoodReserve,
                isAlpha: true);
        }

        public SpeciesCell WithTerrainEnergy(float energy)
        {
            if (!isResourceTerrain)
            {
                return this;
            }

            return new SpeciesCell(
                SpeciesId,
                IsOccupied,
                Health,
                Energy,
                Age,
                FoodEaten,
                FoodReserve,
                IsAlpha,
                TerrainId,
                energy,
                isResourceSpecies,
                isResourceTerrain,
                IsPassable,
                MovementCost,
                resourceSpeciesId: resourceSpeciesId,
                behaviorState: BehaviorState,
                behaviorStateTicks: BehaviorStateTicks,
                entityId: EntityId,
                attackCooldownTicksRemaining: AttackCooldownTicksRemaining,
                energyRemainder: EnergyRemainder,
                trackingTargetEntityId: TrackingTargetEntityId,
                trackingTargetX: TrackingTargetX,
                trackingTargetY: TrackingTargetY,
                trackingTicksRemaining: TrackingTicksRemaining);
        }
    }
}
