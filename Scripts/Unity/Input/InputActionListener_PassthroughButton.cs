using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace KenRampage.Unity.Input
{
    /// <summary>
    /// Listens for Input Action events specifically for button inputs and provides detailed state tracking.
    /// Supports started, performed, canceled events and includes value-based triggering with debouncing.
    /// </summary>
    [AddComponentMenu("Ken Rampage/Unity/Input/Input Action Listener Passthrough Button")]
    public class InputActionListener_PassthroughButton : MonoBehaviour
    {
        [SerializeField] private InputActionReference _inputActionReference;

        public UnityEvent OnActionStarted;
        public UnityEvent OnActionPerformed;
        public UnityEvent OnActionCanceled;
        public UnityEvent<float> OnActionTriggered;

        private bool _lastButtonState = false;

        #region Init
        private void OnEnable()
        {
            if (_inputActionReference != null)
            {
                _inputActionReference.action.Enable();
                _inputActionReference.action.performed += HandleActionPerformed;
            }
        }

        private void OnDisable()
        {
            if (_inputActionReference != null)
            {
                _inputActionReference.action.Disable();
                _inputActionReference.action.performed -= HandleActionPerformed;
            }
        }
        #endregion

        #region Input Responses
        private void HandleActionPerformed(InputAction.CallbackContext context)
        {
            OnActionPerformed.Invoke();

            if (context.control is ButtonControl button)
            {
                OnActionTriggered.Invoke(button.ReadValue());

                bool currentButtonState = button.isPressed;
                if (currentButtonState != _lastButtonState)
                {
                    if (currentButtonState)
                    {
                        OnActionStarted.Invoke();
                    }
                    else
                    {
                        OnActionCanceled.Invoke();
                    }

                    _lastButtonState = currentButtonState;
                }
            }
        }
        #endregion
    }
}