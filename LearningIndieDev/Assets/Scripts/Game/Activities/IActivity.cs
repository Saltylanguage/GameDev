namespace SaltyGame
{
    public interface IActivity
    {
        string DisplayName { get; }
        ActivityKind Kind { get; }
        bool RequiresTimingInput { get; }
        bool IsComplete { get; }
        bool IsCancelled { get; }
        float Progress { get; }
        string StatusText { get; }
        ActivityResult Result { get; }
        void Tick(float deltaTime);
        void Submit(float timing);
        void Cancel();
    }
}
