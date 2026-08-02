namespace SaltyGame
{
    public enum TimeOfDay
    {
        Morning,
        Afternoon,
        Night
    }

    public sealed class GameClock
    {
        const float CycleLengthSeconds = 120f;

        public int Day { get; private set; } = 1;
        public TimeOfDay TimeOfDay { get; private set; } = TimeOfDay.Morning;
        public float CycleProgress { get; private set; }

        public void Tick(float deltaTime)
        {
            CycleProgress += deltaTime / CycleLengthSeconds;
            if (CycleProgress < 1f)
                return;

            CycleProgress -= 1f;
            if (TimeOfDay == TimeOfDay.Night)
            {
                Day++;
                TimeOfDay = TimeOfDay.Morning;
            }
            else
            {
                TimeOfDay = (TimeOfDay)((int)TimeOfDay + 1);
            }
        }
    }
}
