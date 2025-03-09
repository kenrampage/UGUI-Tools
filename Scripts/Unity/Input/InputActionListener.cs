using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace KenRampage.Unity.Input
{
    /// <summary>
    /// Listens for Input Action events and relays them through UnityEvents.
    /// Handles action states for Started (pressed), Performed (held), and Canceled (released).
    /// </summary>
    [AddComponentMenu("Ken Rampage/Unity/Input/Input Action Listener")]
    public class InputActionListener : MonoBehaviour
    {
        [SerializeField] private InputActionReference _actionReference;

        public UnityEvent onStarted;
        public UnityEvent onPerformed;
        public UnityEvent onCanceled;

        private void Update()
        {
            if (_actionReference == null || _actionReference.action == null)
                return;

            var action = _actionReference.action;

            if (action.WasPressedThisFrame())
            {
                onStarted.Invoke();
            }

            if (action.IsPressed())
            {
                onPerformed.Invoke();
            }

            if (action.WasReleasedThisFrame())
            {
                onCanceled.Invoke();
            }
        }
    }
}