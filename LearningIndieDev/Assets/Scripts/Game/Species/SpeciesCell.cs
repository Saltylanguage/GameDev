using System;

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
                movementCost: 1f)
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
            float movementCost)
        {
            if (health < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(health), health, "Health cannot be negative.");
            }

            if (energy < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(energy), energy, "Energy cannot be negative.");
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

            IsOccupied = isOccupied;
            SpeciesId = species;
            Health = health;
            Energy = energy;
            Age = age;
            FoodEaten = foodEaten;
            FoodReserve = foodReserve;
            IsAlpha = isAlpha && isOccupied && !isResourceSpecies;
            this.terrainId = terrainId;
            TerrainEnergy = terrainEnergy;
            this.isResourceSpecies = isResourceSpecies;
            this.isResourceTerrain = isResourceTerrain;
            this.isPassable = isPassable;
            this.movementCost = movementCost;
        }

        readonly bool isResourceSpecies;
        readonly bool isResourceTerrain;
        readonly TerrainId terrainId;
        readonly bool isPassable;
        readonly float movementCost;

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
                movementCost: definition.MovementCost);
        }

        public bool IsOccupied { get; }
        public bool IsCreature => IsOccupied && !isResourceSpecies;
        public bool IsTerrainResource => isResourceTerrain && TerrainEnergy > 0f;
        public bool IsPlantResource => IsTerrainResource
            || (IsOccupied && isResourceSpecies);
        public bool IsPassable => !terrainId.IsValid || isPassable;
        public float MovementCost => terrainId.IsValid ? movementCost : 1f;
        public SpeciesId SpeciesId { get; }

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
        public int Age { get; }
        public int FoodEaten { get; }
        public float FoodReserve { get; }
        public bool IsAlpha { get; }

        public SpeciesCell WithEntity(
            SpeciesId species,
            int health,
            int energy,
            int age,
            int foodEaten,
            float foodReserve,
            bool isAlpha = false)
        {
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
                MovementCost);
        }

        public SpeciesCell WithoutEntity()
        {
            return TerrainId == TerrainIds.Bare
                ? Empty
                : new SpeciesCell(
                    SpeciesId,
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
                    MovementCost);
        }

        public SpeciesCell WithoutPlantResource()
        {
            return IsCreature
                ? new SpeciesCell(
                    SpeciesId,
                    Health,
                    Energy,
                    Age,
                    FoodEaten,
                    FoodReserve,
                    IsAlpha)
                : Empty;
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
                MovementCost);
        }
    }
}
