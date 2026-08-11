using System;

namespace SaltyGame
{
    public sealed class SpeciesAwarenessRules
    {
        public static SpeciesAwarenessRules None { get; } = new SpeciesAwarenessRules();

        readonly GridPattern visionPattern;

        public SpeciesAwarenessRules(int visionRange = 0, int intelligence = 0)
        {
            if (visionRange < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(visionRange), visionRange, "Vision range cannot be negative.");
            }

            if (intelligence < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(intelligence), intelligence, "Intelligence cannot be negative.");
            }

            VisionRange = visionRange;
            Intelligence = intelligence;
            visionPattern = GridPatternTemplates.CreateMooreRange(visionRange);
        }

        public int VisionRange { get; }
        public int Intelligence { get; }
        public GridPattern VisionPattern => visionPattern;
    }
}
