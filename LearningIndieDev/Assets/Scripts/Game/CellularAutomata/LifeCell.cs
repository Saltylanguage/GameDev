namespace SaltyGame
{
    public readonly struct LifeCell
    {
        public LifeCell(bool isAlive)
        {
            IsAlive = isAlive;
        }

        public bool IsAlive { get; }
    }
}
