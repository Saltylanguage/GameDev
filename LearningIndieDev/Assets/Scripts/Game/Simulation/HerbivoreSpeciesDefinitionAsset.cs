using UnityEngine;

namespace SaltyGame
{
    [CreateAssetMenu(menuName = "Salty Game/Species/Herbivore", fileName = "HerbivoreSpecies")]
    public sealed class HerbivoreSpeciesDefinitionAsset : SpeciesDefinitionAsset
    {
        protected override SpeciesRole GetRole() => SpeciesRole.Herbivore;
    }
}
