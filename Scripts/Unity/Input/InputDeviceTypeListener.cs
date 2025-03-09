using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Controls;
using System.Linq;

namespace KenRampage.Unity.Input
{
    /// <summary>
    /// Listens for input from different device types (Gamepad, Mouse/Keyboard, Touch) and fires corresponding events
    /// when active input is detected from each device type. Also tracks and broadcasts input type changes.
    /// </summary>
    [AddComponentMenu("Ken Rampage/Unity/Input/Input Device Type Listener")]
    public class InputDeviceTypeListener : MonoBehaviour
    {
        #region Nested Classes
        [System.Serializable]
        public class EventsClass
        {
            public UnityEvent OnGamepadInputDetected;
            public UnityEvent OnMKBInputDetected;
            public UnityEvent OnTouchInputDetected;
            public UnityEvent<string> OnInputTypeChanged;
        }

        [System.Serializable]
        public class SettingsClass
        {
            [Header("Input Detection Settings")]
            public float StickDeadzone = 0.1f;
            public bool EnableGamepad = true;
            public bool EnableMKB = true;
            public bool EnableTouch = true;

            [Header("Debug Settings")]
            public bool DebugMode;
        }
        #endregion
        
        #region Fields
        [SerializeField] private SettingsClass _settings;
        [SerializeField] private EventsClass _events;
        private string _currentInputType = "";
        #endregion

        #region Unity Event Functions
        private void OnEnable()
        {
            InputSystem.onEvent += OnInputEvent;
        }

        private void OnDisable()
        {
            InputSystem.onEvent -= OnInputEvent;
        }
        #endregion

        #region Input Detection Methods
        private bool HasGamepadInput(Gamepad gamepad)
        {
            if (!_settings.EnableGamepad) return false;
            
            return gamepad.allControls.Any(control => 
                control is ButtonControl button && button.isPressed ||
                control is StickControl stick && stick.ReadValue().magnitude > _settings.StickDeadzone ||
                control is Vector2Control vector && vector.ReadValue().magnitude > _settings.StickDeadzone);
        }

        private bool HasKeyboardInput(Keyboard keyboard)
        {
            if (!_settings.EnableMKB) return false;

            return keyboard.allControls.Any(control => 
                control is KeyControl key && key.isPressed);
        }

        private bool HasMouseInput(Mouse mouse)
        {
            if (!_settings.EnableMKB) return false;

            return mouse.wasUpdatedThisFrame;
        }

        private bool HasTouchInput(Touchscreen touch)
        {
            if (!_settings.EnableTouch) return false;

            return touch.allControls.Any(control => 
                control is TouchControl touchControl && touchControl.isInProgress);
        }
        #endregion

        #region Event Processing
        private void OnInputEvent(InputEventPtr eventPtr, InputDevice device)
        {
            if (eventPtr.type != StateEvent.Type && eventPtr.type != DeltaStateEvent.Type)
                return;

            string newInputType = "";

            switch (device)
            {
                case Gamepad gamepad:
                    if (HasGamepadInput(gamepad))
                    {
                        _events.OnGamepadInputDetected?.Invoke();
                        newInputType = "Gamepad";
                    }
                    break;
                case Keyboard keyboard:
                    if (HasKeyboardInput(keyboard))
                    {
                        _events.OnMKBInputDetected?.Invoke();
                        newInputType = "MKB";
                    }
                    break;
                case Mouse mouse:
                    if (HasMouseInput(mouse))
                    {
                        _events.OnMKBInputDetected?.Invoke();
                        newInputType = "MKB";
                    }
                    break;
                case Touchscreen touch:
                    if (HasTouchInput(touch))
                    {
                        _events.OnTouchInputDetected?.Invoke();
                        newInputType = "Touch";
                    }
                    break;
            }

            if (!string.IsNullOrEmpty(newInputType) && newInputType != _currentInputType)
            {
                _currentInputType = newInputType;
                _events.OnInputTypeChanged?.Invoke(_currentInputType);
                
                if (_settings.DebugMode)
                {
                    Debug.Log($"Input type changed to: {_currentInputType}");
                }
            }
        }
        #endregion
    }
}