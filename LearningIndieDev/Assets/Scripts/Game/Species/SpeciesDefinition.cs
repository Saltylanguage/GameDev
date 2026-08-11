using System;

namespace SaltyGame
{
    public sealed class SpeciesDefinition
    {
        public SpeciesDefinition(SpeciesId id, SpeciesRules rules)
        {
            Id = id;
            Rules = rules ?? throw new ArgumentNullException(nameof(rules));
        }

        public SpeciesId Id { get; }
        [Obsolete("Use Id instead.")]
        public SpeciesArchetype Archetype => SpeciesId.ToLegacyArchetype(Id);
        public SpeciesRules Rules { get; }
    }
}
