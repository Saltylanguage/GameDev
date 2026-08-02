using System;

namespace SaltyGame
{
    public sealed class MiningActivity : IActivity
    {
        readonly int rewardAmount;
        int health;
        float elapsed;

        public MiningActivity(int health, int rewardAmount)
        {
            if (health <= 0)
                throw new ArgumentOutOfRangeException(nameof(health), "Health must be greater than zero.");
            if (rewardAmount <= 0)
                throw new ArgumentOutOfRangeException(nameof(rewardAmount), "Reward amount must be greater than zero.");

            this.health = health;
            this.rewardAmount = rewardAmount;
        }

        public string DisplayName => "Mining";
        public bool IsComplete { get; private set; }
        public bool IsCancelled { get; private set; }
        public float Progress => elapsed * 1.25f % 2f <= 1f ? elapsed * 1.25f % 2f : 2f - elapsed * 1.25f % 2f;
        public string StatusText => $"Rock health: {health}";
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

            health -= timing >= 0.4f && timing <= 0.6f ? 2 : 1;
            if (health > 0)
                return;

            IsComplete = true;
            Result = new ActivityResult(true, ResourceId.Stone, rewardAmount);
        }

        public void Cancel()
        {
            if (IsComplete)
                return;

            IsCancelled = true;
            Result = new ActivityResult(false, null, 0);
        }
    }
}
