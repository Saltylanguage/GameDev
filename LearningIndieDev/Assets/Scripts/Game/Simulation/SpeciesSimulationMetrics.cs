using System.Collections.Generic;

namespace SaltyGame
{
    public readonly struct SpeciesSimulationActivity
    {
        internal SpeciesSimulationActivity(
            int births,
            float foodConsumed,
            int movementSteps,
            int damageDealt,
            int combatKills,
            int deaths,
            int starvationDeaths,
            int crowdingDeaths,
            int wiltDeaths,
            int populationLimitRemovals)
        {
            Births = births;
            FoodConsumed = foodConsumed;
            MovementSteps = movementSteps;
            DamageDealt = damageDealt;
            CombatKills = combatKills;
            Deaths = deaths;
            StarvationDeaths = starvationDeaths;
            CrowdingDeaths = crowdingDeaths;
            WiltDeaths = wiltDeaths;
            PopulationLimitRemovals = populationLimitRemovals;
        }

        public int Births { get; }
        public float FoodConsumed { get; }
        public int MovementSteps { get; }
        public int DamageDealt { get; }
        public int CombatKills { get; }
        public int Deaths { get; }
        public int StarvationDeaths { get; }
        public int CrowdingDeaths { get; }
        public int WiltDeaths { get; }
        public int PopulationLimitRemovals { get; }

        internal SpeciesSimulationActivity Add(
            int births = 0,
            float foodConsumed = 0f,
            int movementSteps = 0,
            int damageDealt = 0,
            int combatKills = 0,
            int deaths = 0,
            int starvationDeaths = 0,
            int crowdingDeaths = 0,
            int wiltDeaths = 0,
            int populationLimitRemovals = 0)
        {
            return new SpeciesSimulationActivity(
                Births + births,
                FoodConsumed + foodConsumed,
                MovementSteps + movementSteps,
                DamageDealt + damageDealt,
                CombatKills + combatKills,
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

        public SpeciesSimulationActivity GetActivity(SpeciesId species)
        {
            return activityBySpecies.TryGetValue(species, out var activity)
                ? activity
                : default;
        }

        public void Clear()
        {
            activityBySpecies.Clear();
        }

        internal void Record(
            SpeciesId species,
            int births = 0,
            float foodConsumed = 0f,
            int movementSteps = 0,
            int damageDealt = 0,
            int combatKills = 0,
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
                movementSteps,
                damageDealt,
                combatKills,
                deaths,
                starvationDeaths,
                crowdingDeaths,
                wiltDeaths,
                populationLimitRemovals);
        }
    }
}
