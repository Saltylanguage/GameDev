using System;

namespace SaltyGame
{
    public readonly struct SpeciesCell
    {
        public SpeciesCell(
            SpeciesArchetype species,
            int health = 1,
            int energy = 0,
            int age = 0,
            int foodEaten = 0)
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

            IsOccupied = true;
            Species = species;
            Health = health;
            Energy = energy;
            Age = age;
            FoodEaten = foodEaten;
        }

        public static SpeciesCell Empty => default;

        public bool IsOccupied { get; }
        public SpeciesArchetype Species { get; }
        public int Health { get; }
        public int Energy { get; }
        public int Age { get; }
        public int FoodEaten { get; }
    }
}
