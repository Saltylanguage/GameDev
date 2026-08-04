using UnityEngine;

namespace SaltyGame
{
    public sealed class PlayerController
    {
        const float MoveSpeed = 3.4f;
        readonly Transform transform;
        readonly PlayerInputAdapter input;
        readonly ActivityController activities;

        public PlayerController(Transform transform, PlayerInputAdapter input, ActivityController activities)
        {
            this.transform = transform;
            this.input = input;
            this.activities = activities;
        }

        public void Tick(float deltaTime)
        {
            if (activities.IsActive)
                return;

            var move = input.Move;
            transform.position += (Vector3)(move.normalized * MoveSpeed * deltaTime);
            transform.position = new Vector3(
                Mathf.Clamp(transform.position.x, -5.8f, 5.8f),
                Mathf.Clamp(transform.position.y, -3.5f, 3.5f),
                transform.position.z);
        }
    }
}
