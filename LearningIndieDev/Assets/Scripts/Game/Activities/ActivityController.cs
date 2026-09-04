namespace SaltyGame
{
    public sealed class ActivityController 
    {
        readonly InventoryState inventory;
        readonly SurvivalState survival;
        IActivityTarget target;

        public IActivity CurrentActivity { get; private set; }
        public bool IsActive => CurrentActivity != null && !CurrentActivity.IsComplete && !CurrentActivity.IsCancelled;
        public float Meter => CurrentActivity?.Progress ?? 0f;
        public ActivityResult LastResult { get; private set; }
        public string LastActivityName { get; private set; }
        public string LastFailureMessage { get; private set; }

        public ActivityController(InventoryState inventory, SurvivalState survival)
        {
            this.inventory = inventory;
            this.survival = survival;
        }

        public bool Start(IActivityTarget target)
        {
            if (IsActive || target == null || !target.CanInteract)
                return false;

            var activity = target.CreateActivity();
            if (activity == null)
            {
                LastFailureMessage = "That action is unavailable right now.";
                return false;
            }

            if (!survival.CanStart(activity.Kind))
            {
                LastFailureMessage = survival.CannotStartMessage(activity.Kind);
                return false;
            }

            LastFailureMessage = null;
            this.target = target;
            CurrentActivity = activity;
            return true;
        }

        public bool Tick(float deltaTime)
        {
            if (CurrentActivity == null)
                return false;

            CurrentActivity.Tick(deltaTime);
            if (CurrentActivity.IsComplete || CurrentActivity.IsCancelled)
            {
                var completed = CurrentActivity.IsComplete;
                if (CurrentActivity.IsComplete)
                {
                    LastResult = CurrentActivity.Result;
                    LastActivityName = CurrentActivity.DisplayName;
                    survival.CompleteActivity(CurrentActivity.Kind);
                    inventory.Add(CurrentActivity.Result.ResourceId, CurrentActivity.Result.Amount);
                    target.ApplyActivityResult(CurrentActivity.Result);
                }
                CurrentActivity = null;
                target = null;
                return completed;
            }

            return false;
        }

        public void SubmitHit()
        {
            if (IsActive)
                CurrentActivity.Submit(Meter);
        }

        public void Cancel()
        {
            if (IsActive)
                CurrentActivity.Cancel();
        }
    }
}
