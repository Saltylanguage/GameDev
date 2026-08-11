using System;

namespace SaltyGame
{
    /// <summary>Promotes a chance-based subset of one species' newborn creatures.</summary>
    public sealed class AlphaOffspringRule
    {
        public AlphaOffspringRule(
            SpeciesId species,
            float chance,
            int healthBonus = 0,
            int energyBonus = 0)
        {
            if (!species.IsValid || species == SpeciesIds.Plant)
            {
                throw new ArgumentException("Alpha offspring must target a non-plant species.", nameof(species));
            }

            if (chance < 0f || chance > 1f)
            {
                throw new ArgumentOutOfRangeException(nameof(chance), chance, "Alpha chance must be between zero and one.");
            }

            if (healthBonus < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(healthBonus), healthBonus, "Health bonus cannot be negative.");
            }

            if (energyBonus < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(energyBonus), energyBonus, "Energy bonus cannot be negative.");
            }

            Species = species;
            Chance = chance;
            HealthBonus = healthBonus;
            EnergyBonus = energyBonus;
        }

        public SpeciesId Species { get; }
        public float Chance { get; }
        public int HealthBonus { get; }
        public int EnergyBonus { get; }

        public SpeciesCell Apply(SpeciesCell offspring, Random random)
        {
            if (random == null)
            {
                throw new ArgumentNullException(nameof(random));
            }

            if (!offspring.IsCreature || offspring.SpeciesId != Species || Chance <= 0f)
            {
                return offspring;
            }

            return Chance >= 1f || random.NextDouble() <= Chance
                ? offspring.WithAlpha(HealthBonus, EnergyBonus)
                : offspring;
        }
    }
}
