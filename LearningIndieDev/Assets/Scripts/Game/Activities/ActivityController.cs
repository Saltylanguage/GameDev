namespace SaltyGame
{
    public sealed class ActivityController
    {
        readonly InventoryState inventory;
        readonly SurvivalState survival;
        IActivityTarget target;

        public IActivity Active { get; private set; }
        public bool IsActive => Active != null && !Active.IsComplete && !Active.IsCancelled;
        public float Meter => Active?.Progress ?? 0f;
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
            Active = activity;
            return true;
        }

        public bool Tick(float deltaTime)
        {
            if (Active == null)
                return false;

            Active.Tick(deltaTime);
            if (Active.IsComplete || Active.IsCancelled)
            {
                var completed = Active.IsComplete;
                if (Active.IsComplete)
                {
                    LastResult = Active.Result;
                    LastActivityName = Active.DisplayName;
                    survival.CompleteActivity(Active.Kind);
                    inventory.Add(Active.Result.ResourceId, Active.Result.Amount);
                    target.ApplyActivityResult(Active.Result);
                }
                Active = null;
                target = null;
                return completed;
            }

            return false;
        }

        public void SubmitHit()
        {
            if (IsActive)
                Active.Submit(Meter);
        }

        public void Cancel()
        {
            if (IsActive)
                Active.Cancel();
        }
    }
}
