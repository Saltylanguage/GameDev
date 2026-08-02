using UnityEngine;
using UnityEngine.InputSystem;

namespace SaltyGame
{
    public sealed class PlayerInputAdapter : MonoBehaviour
    {
        public Vector2 Move
        {
            get
            {
                var keyboard = Keyboard.current;
                if (keyboard == null)
                    return Vector2.zero;

                return new Vector2(
                    (keyboard.dKey.isPressed ? 1 : 0) - (keyboard.aKey.isPressed ? 1 : 0),
                    (keyboard.wKey.isPressed ? 1 : 0) - (keyboard.sKey.isPressed ? 1 : 0));
            }
        }

        public bool InteractPressed => Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame;
        public bool HitPressed => Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame;
        public bool CancelPressed => Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame;
        public bool DebugPressed => Keyboard.current != null && Keyboard.current.f3Key.wasPressedThisFrame;
    }
}
