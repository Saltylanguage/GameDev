namespace SaltyGame
{
    public sealed class CookingActivity : IActivity
    {
        const float Duration = 1f;
        float elapsed;

        public string DisplayName => "Cooking Berries";
        public ActivityKind Kind => ActivityKind.Cooking;
        public bool RequiresTimingInput => false;
        public bool IsComplete { get; private set; }
        public bool IsCancelled { get; private set; }
        public float Progress => elapsed / Duration;
        public string StatusText => "Cooking 2 berries into a meal...";
        public ActivityResult Result { get; private set; }

        public void Tick(float deltaTime)
        {
            if (IsComplete || IsCancelled)
                return;

            elapsed += deltaTime;
            if (elapsed < Duration)
                return;

            IsComplete = true;
            Result = new ActivityResult(true, ResourceId.CookedMeal, 1);
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
