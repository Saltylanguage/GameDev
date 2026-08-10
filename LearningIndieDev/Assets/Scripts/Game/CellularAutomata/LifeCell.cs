namespace SaltyGame
{
    public readonly struct LifeCell
    {
        public enum State
        {
            Empty,
            Life,
            Plant,
            Fire,
        }

        public LifeCell(State currentState, float temperature = 0f)
        {
            CurrentState = currentState;
            Temperature = temperature;
        }

        public State CurrentState { get; }
        public float Temperature { get; }
    }
}
