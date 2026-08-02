namespace SaltyGame
{
    public sealed class ActivityController
    {
        readonly InventoryState inventory;
        IActivityTarget target;

        public IActivity Active { get; private set; }
        public bool IsActive => Active != null && !Active.IsComplete && !Active.IsCancelled;
        public float Meter => Active?.Progress ?? 0f;

        public ActivityController(InventoryState inventory)
        {
            this.inventory = inventory;
        }

        public bool Start(IActivityTarget target)
        {
            if (IsActive || target == null || !target.CanInteract)
                return false;

            this.target = target;
            Active = target.CreateActivity();
            return Active != null;
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
