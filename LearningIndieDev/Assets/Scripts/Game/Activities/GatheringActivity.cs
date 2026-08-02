using System;

namespace SaltyGame
{
    public sealed class GatheringActivity : IActivity
    {
        const float StrongHitMinimum = 0.4f;
        const float StrongHitMaximum = 0.6f;

        readonly int requiredActions;
        int actions;
        int collected;
        float elapsed;

        public GatheringActivity(int requiredActions)
        {
            if (requiredActions <= 0)
                throw new ArgumentOutOfRangeException(nameof(requiredActions), "Required actions must be greater than zero.");

            this.requiredActions = requiredActions;
        }

        public string DisplayName => "Gathering";
        public ActivityKind Kind => ActivityKind.Gathering;
        public bool RequiresTimingInput => true;
        public bool IsComplete { get; private set; }
        public bool IsCancelled { get; private set; }
        public float Progress => elapsed * 1.25f % 2f <= 1f ? elapsed * 1.25f % 2f : 2f - elapsed * 1.25f % 2f;
        public string StatusText => $"Berries gathered: {actions}/{requiredActions}";
        public ActivityResult Result { get; private set; }

        public void Tick(float deltaTime)
        {
            if (!IsComplete && !IsCancelled)
                elapsed += deltaTime;
        }

        public void Submit(float timing)
        {
            if (IsComplete || IsCancelled)
                return;

            actions++;
            collected += IsStrongTiming(timing) ? 2 : 1;
            if (actions < requiredActions)
                return;

            IsComplete = true;
            Result = new ActivityResult(true, ResourceId.Berries, collected);
        }

        public void Cancel()
        {
            if (IsComplete)
                return;

            IsCancelled = true;
            Result = new ActivityResult(false, null, 0);
        }

        public static bool IsStrongTiming(float timing)
        {
            return timing >= StrongHitMinimum && timing <= StrongHitMaximum;
        }
    }
}
