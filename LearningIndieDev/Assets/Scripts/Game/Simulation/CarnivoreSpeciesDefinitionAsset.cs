using UnityEngine;

namespace SaltyGame
{
    [CreateAssetMenu(menuName = "Salty Game/Species/Carnivore", fileName = "CarnivoreSpecies")]
    public sealed class CarnivoreSpeciesDefinitionAsset : SpeciesDefinitionAsset
    {
        protected override SpeciesRole GetRole() => SpeciesRole.Carnivore;
    }
}
