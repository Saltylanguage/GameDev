using System.Collections.Generic;
using UnityEngine;

namespace SaltyGame
{
    public sealed class InteractionController
    {
        const float InteractionRange = 1.6f;
        readonly Transform player;
        readonly PlayerInputAdapter input;
        readonly ActivityController activities;
        readonly SurvivalState survival;
        readonly IReadOnlyList<IActivityTarget> targets;

        public IActivityTarget CurrentTarget { get; private set; }
        string pendingMessage;
        bool sleepRequested;

        public InteractionController(Transform player, PlayerInputAdapter input, ActivityController activities, SurvivalState survival, IReadOnlyList<IActivityTarget> targets)
        {
            this.player = player;
            this.input = input;
            this.activities = activities;
            this.survival = survival;
            this.targets = targets;
        }

        public void Tick()
        {
            CurrentTarget = FindNearestTarget();
            if (activities.IsActive)
            {
                if (input.CancelPressed)
                    activities.Cancel();
                else if (input.HitPressed)
                    activities.SubmitHit();
                return;
            }

            if (CurrentTarget is CampfireInteractable campfire)
            {
                if (input.EatPressed)
                {
                    campfire.TryEat(survival, out pendingMessage);
                    return;
                }

                if (input.SleepPressed)
                {
                    if (campfire.IsBuilt)
                        sleepRequested = true;
                    else
                        pendingMessage = "Build the campfire before sleeping at camp.";
                    return;
                }

                if (input.InteractPressed && campfire.IsBuilt && !campfire.CanCook)
                {
                    pendingMessage = campfire.GetCookStatus();
                    return;
                }
            }

            if (CurrentTarget != null && input.InteractPressed)
            {
                if (!activities.Start(CurrentTarget))
                    pendingMessage = activities.LastFailureMessage;
            }
        }

        public string ConsumeMessage()
        {
            var message = pendingMessage;
            pendingMessage = null;
            return message;
        }

        public bool ConsumeSleepRequest()
        {
            var requested = sleepRequested;
            sleepRequested = false;
            return requested;
        }

        IActivityTarget FindNearestTarget()
        {
            IActivityTarget nearest = null;
            var nearestDistance = InteractionRange;
            foreach (var target in targets)
            {
                if (!target.CanInteract)
                    continue;

                var distance = Vector2.Distance(player.position, target.Position);
                if (distance <= nearestDistance)
                {
                    nearest = target;
                    nearestDistance = distance;
                }
            }
            return nearest;
        }
    }
}
