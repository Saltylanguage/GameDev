namespace SaltyGame
{
    public readonly struct HeatCell
    {
        public HeatCell(float temperature)
        {
            Temperature = temperature;
        }

        public float Temperature { get; }
    }
}
