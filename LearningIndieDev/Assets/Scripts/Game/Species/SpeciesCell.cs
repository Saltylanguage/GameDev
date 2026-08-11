using System;

namespace SaltyGame
{
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
            float foodReserve = 0f)
            : this(
                species,
                true,
                health,
                energy,
                age,
                foodEaten,
                foodReserve,
                SpeciesTerrain.Bare,
                terrainEnergy: 0f,
                isResourceSpecies: species == SpeciesIds.Plant)
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
            SpeciesTerrain terrain,
            float terrainEnergy,
            bool isResourceSpecies)
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

            IsOccupied = isOccupied;
            SpeciesId = species;
            Health = health;
            Energy = energy;
            Age = age;
            FoodEaten = foodEaten;
            FoodReserve = foodReserve;
            Terrain = terrain;
            TerrainEnergy = terrainEnergy;
            this.isResourceSpecies = isResourceSpecies;
        }

        readonly bool isResourceSpecies;

        public static SpeciesCell Empty => default;

        public static SpeciesCell Grass(float energy)
        {
            return Grass(SpeciesIds.Plant, energy);
        }

        public static SpeciesCell Grass(SpeciesId resourceSpecies, float energy)
        {
            return new SpeciesCell(
                resourceSpecies,
                false,
                0,
                0,
                0,
                0,
                0f,
                SpeciesTerrain.Grass,
                energy,
                isResourceSpecies: true);
        }

        public bool IsOccupied { get; }
        public bool IsCreature => IsOccupied && !isResourceSpecies;
        public bool IsGrass => Terrain == SpeciesTerrain.Grass;
        public bool IsPlantResource => (Terrain == SpeciesTerrain.Grass && TerrainEnergy > 0f)
            || (IsOccupied && isResourceSpecies);
        public SpeciesId SpeciesId { get; }
        [Obsolete("Use SpeciesId instead.")]
        public SpeciesArchetype Species => SpeciesId.ToLegacyArchetype(SpeciesId);
        public SpeciesTerrain Terrain { get; }
        public float TerrainEnergy { get; }
        public int Health { get; }
        public int Energy { get; }
        public int Age { get; }
        public int FoodEaten { get; }
        public float FoodReserve { get; }

        public SpeciesCell WithEntity(
            SpeciesId species,
            int health,
            int energy,
            int age,
            int foodEaten,
            float foodReserve)
        {
            return new SpeciesCell(
                species,
                true,
                health,
                energy,
                age,
                foodEaten,
                foodReserve,
                Terrain,
                TerrainEnergy,
                isResourceSpecies: species == SpeciesIds.Plant);
        }

        public SpeciesCell WithoutEntity()
        {
            return Terrain == SpeciesTerrain.Grass
                ? Grass(SpeciesIds.Plant, TerrainEnergy)
                : Empty;
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
                    FoodReserve)
                : Empty;
        }

        public SpeciesCell WithTerrainEnergy(float energy)
        {
            if (Terrain != SpeciesTerrain.Grass)
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
                Terrain,
                energy,
                isResourceSpecies);
        }
    }
}
