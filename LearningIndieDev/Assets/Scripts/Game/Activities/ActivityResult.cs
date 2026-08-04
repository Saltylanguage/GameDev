namespace SaltyGame
{
    public readonly struct ActivityResult
    {
        public readonly bool Succeeded;
        public readonly string ResourceId;
        public readonly int Amount;

        public ActivityResult(bool succeeded, string resourceId, int amount)
        {
            Succeeded = succeeded;
            ResourceId = resourceId;
            Amount = amount;
        }
    }

}
