using System;

namespace SaltyGame
{
    public sealed class SpeciesDefinition
    {
        public SpeciesDefinition(SpeciesArchetype archetype, SpeciesRules rules)
        {
            Archetype = archetype;
            Rules = rules ?? throw new ArgumentNullException(nameof(rules));
        }

        public SpeciesArchetype Archetype { get; }
        public SpeciesRules Rules { get; }
    }
}
