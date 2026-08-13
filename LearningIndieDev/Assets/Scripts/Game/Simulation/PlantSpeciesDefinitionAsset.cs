using UnityEngine;

namespace SaltyGame
{
    [CreateAssetMenu(menuName = "Salty Game/Species/Plant", fileName = "PlantSpecies")]
    public sealed class PlantSpeciesDefinitionAsset : SpeciesDefinitionAsset
    {
        protected override SpeciesRole GetRole() => SpeciesRole.Plant;
    }
}
