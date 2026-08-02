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

        public void AdvanceActivity()
        {
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
