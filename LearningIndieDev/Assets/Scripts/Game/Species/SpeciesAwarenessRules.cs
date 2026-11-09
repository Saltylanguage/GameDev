using System;

namespace SaltyGame
{
    public sealed class SpeciesAwarenessRules
    {
        public static SpeciesAwarenessRules None { get; } = new SpeciesAwarenessRules();

        readonly GridPattern visionPattern;
        readonly GridPattern predatorDetectionPattern;

        public SpeciesAwarenessRules(int visionRange = 0, int intelligence = 0, int predatorDetectionRangeBonus = 0)
        {
            if (visionRange < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(visionRange), visionRange, "Vision range cannot be negative.");
            }

            if (intelligence < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(intelligence), intelligence, "Intelligence cannot be negative.");
            }

            if (predatorDetectionRangeBonus < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(predatorDetectionRangeBonus), predatorDetectionRangeBonus, "Predator detection range cannot be negative.");
            }

            VisionRange = visionRange;
            Intelligence = intelligence;
            PredatorDetectionRangeBonus = predatorDetectionRangeBonus;
            visionPattern = GridPatternTemplates.CreateMooreRange(visionRange);
            predatorDetectionPattern = GridPatternTemplates.CreateMooreRange(visionRange + predatorDetectionRangeBonus);
        }

        public int VisionRange { get; }
        public int Intelligence { get; }
        public int PredatorDetectionRangeBonus { get; }
        public GridPattern VisionPattern => visionPattern;
        public GridPattern PredatorDetectionPattern => predatorDetectionPattern;
    }
}
