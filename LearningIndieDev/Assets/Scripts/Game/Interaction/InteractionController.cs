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
        readonly IReadOnlyList<IActivityTarget> targets;

        public IActivityTarget CurrentTarget { get; private set; }

        public InteractionController(Transform player, PlayerInputAdapter input, ActivityController activities, IReadOnlyList<IActivityTarget> targets)
        {
            this.player = player;
            this.input = input;
            this.activities = activities;
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

            if (CurrentTarget != null && input.InteractPressed)
                activities.Start(CurrentTarget);
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
