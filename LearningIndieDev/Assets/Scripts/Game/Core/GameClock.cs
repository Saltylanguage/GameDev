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
        public int Day { get; private set; } = 1;
        public TimeOfDay TimeOfDay { get; private set; } = TimeOfDay.Morning;

        public bool AdvanceActivity()
        {
            if (TimeOfDay == TimeOfDay.Night)
            {
                Day++;
                TimeOfDay = TimeOfDay.Morning;
                return true;
            }

            TimeOfDay = (TimeOfDay)((int)TimeOfDay + 1);
            return false;
        }
    }
}
