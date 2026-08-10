namespace SaltyGame
{
    public readonly struct ElementCell
    {
        public enum State
        {
            Empty,
            Plant,
            Fire,
        }

        public ElementCell(State currentState)
        {
            CurrentState = currentState;
        }

        public State CurrentState { get; }
    }
}
