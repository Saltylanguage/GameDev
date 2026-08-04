using UnityEngine;

namespace SaltyGame
{
    public interface IActivityTarget
    {
        string DisplayName { get; }
        bool CanInteract { get; }
        Vector2 Position { get; }
        IActivity CreateActivity();
        void ApplyActivityResult(ActivityResult result);
        void ResetForNewDay();
    }
}
