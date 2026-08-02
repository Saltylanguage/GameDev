namespace SaltyGame
{
    public sealed class CampfireBuildActivity : IActivity
    {
        public string DisplayName => "Building Campfire";
        public ActivityKind Kind => ActivityKind.Building;
        public bool RequiresTimingInput => false;
        public bool IsComplete { get; private set; }
        public bool IsCancelled { get; private set; }
        public float Progress => 1f;
        public string StatusText => "Building a campfire...";
        public ActivityResult Result { get; private set; }

        public void Tick(float deltaTime)
        {
            if (IsComplete || IsCancelled)
                return;

            IsComplete = true;
            Result = new ActivityResult(true, null, 0);
        }

        public void Submit(float timing) { }

        public void Cancel()
        {
            if (IsComplete)
                return;

            IsCancelled = true;
            Result = new ActivityResult(false, null, 0);
        }
    }
}
