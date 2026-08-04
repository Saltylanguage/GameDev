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
                return false;

            TimeOfDay = (TimeOfDay)((int)TimeOfDay + 1);
            return TimeOfDay == TimeOfDay.Night;
        }

        public void BeginNextDay()
        {
            Day++;
            TimeOfDay = TimeOfDay.Morning;
        }
    }
}
