using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.InputSystem.UI;

namespace KenRampage.Utilities.UI
{
    /// <summary>
    /// Manages cursor states and visuals based on user interaction and hovered components.
    /// </summary>
    [AddComponentMenu("Ken Rampage/Utilities/UI/Cursor Manager")]
    public class CursorManager : MonoBehaviour
    {
        #region Nested Classes
        [System.Serializable]
        public class Settings
        {
            public bool setScreenPosition = true;
            public bool hideHardwareCursor = true;
            [Space(10)]
            public CursorSet cursorSet;
        }

        [System.Serializable]
        public class CursorSet
        {
            public GameObject defaultCursor;
            public GameObject hoverCursor;
            public GameObject dragCursor;
            public GameObject clickCursor;
        }

        [System.Serializable]
        public class DebugInfo
        {
            public State currentState;
        }

        [System.Serializable]
        public class EventsContainer
        {
            public UnityEvent<bool> OnClickStatusChanged;
            public UnityEvent<bool> OnHoverStatusChanged;
        }
        #endregion

        #region Enums
        public enum State
        {
            Default,
            Hover,
            Drag,
            Click
        }
        #endregion

        #region Serialized Fields
        [SerializeField] private DebugInfo _debugInfo;
        [SerializeField] private Settings _settings;
        [SerializeField] private EventsContainer _events;
        #endregion

        #region Private Fields
        private bool _isClickOn;
        private bool _isHoverOn;
        private bool _isDragging;
        private bool _dragStartedWhileHovering;
        private Vector2 _lastPointerPosition;
        private InputSystemUIInputModule _inputModule;
        #endregion

        #region Unity Methods
        private void OnEnable()
        {
            HideCursor();
        }

        private void OnDisable()
        {
            ShowCursor();
        }

        private void Update()
        {
            UpdateInputState();
            UpdateCursorPosition();
            UpdateDragState();
            CalculateNewState();
            HideCursor();
        }
        #endregion

        #region Input and State Update Methods
        /// <summary>
        /// Updates the input state and triggers events for click and hover status changes.
        /// </summary>
        private void UpdateInputState()
        {
            if (_inputModule == null)
            {
                _inputModule = EventSystem.current.currentInputModule as InputSystemUIInputModule;
                if (_inputModule == null)
                {
                    Debug.LogWarning("Current input module is not an InputSystemUIInputModule.");
                    return;
                }
            }

            bool newClickOn = _inputModule.leftClick.action.ReadValue<float>() > 0;
            bool newHoverOn = EventSystem.current.IsPointerOverGameObject();

            if (newClickOn != _isClickOn)
            {
                _isClickOn = newClickOn;
                _events.OnClickStatusChanged.Invoke(_isClickOn);
            }

            if (newHoverOn != _isHoverOn)
            {
                _isHoverOn = newHoverOn;
                _events.OnHoverStatusChanged.Invoke(_isHoverOn);
            }
        }

        /// <summary>
        /// Updates the cursor position if setScreenPosition is enabled.
        /// </summary>
        private void UpdateCursorPosition()
        {
            if (_settings.setScreenPosition)
            {
                transform.position = _inputModule.point.action.ReadValue<Vector2>();
            }
        }

        /// <summary>
        /// Updates the drag state based on current input and hover status.
        /// </summary>
        private void UpdateDragState()
        {
            Vector2 pointerPosition = _inputModule.point.action.ReadValue<Vector2>();

            if (_isClickOn && !_isDragging && _isHoverOn && Vector2.Distance(pointerPosition, _lastPointerPosition) > 0.1f)
            {
                _isDragging = true;
                _dragStartedWhileHovering = true;
            }

            if (!_isClickOn)
            {
                _isDragging = false;
                _dragStartedWhileHovering = false;
            }

            _lastPointerPosition = pointerPosition;
        }

        /// <summary>
        /// Calculates the new cursor state and handles state changes.
        /// </summary>
        private void CalculateNewState()
        {
            State newState = DetermineNewState();

            if (newState != _debugInfo.currentState)
            {
                _debugInfo.currentState = newState;
                HandleStateChange();
            }
        }

        /// <summary>
        /// Determines the new cursor state based on current input and drag status.
        /// </summary>
        /// <returns>The new cursor state.</returns>
        private State DetermineNewState()
        {
            if (_isClickOn && _isHoverOn && !_isDragging)
                return State.Click;
            if (_isDragging && _dragStartedWhileHovering)
                return State.Drag;
            if (_isHoverOn)
                return State.Hover;
            return State.Default;
        }
        #endregion

        #region Cursor Visibility Methods
        /// <summary>
        /// Handles state changes by updating cursor visibility.
        /// </summary>
        private void HandleStateChange()
        {
            UpdateCursorVisibility();
        }

        /// <summary>
        /// Updates the visibility of cursor GameObjects based on the current state.
        /// </summary>
        private void UpdateCursorVisibility()
        {
            _settings.cursorSet.defaultCursor.SetActive(false);
            _settings.cursorSet.hoverCursor.SetActive(false);
            _settings.cursorSet.dragCursor.SetActive(false);
            _settings.cursorSet.clickCursor.SetActive(false);

            switch (_debugInfo.currentState)
            {
                case State.Default:
                    _settings.cursorSet.defaultCursor.SetActive(true);
                    break;
                case State.Hover:
                    _settings.cursorSet.hoverCursor.SetActive(true);
                    break;
                case State.Drag:
                    _settings.cursorSet.dragCursor.SetActive(true);
                    break;
                case State.Click:
                    _settings.cursorSet.clickCursor.SetActive(true);
                    break;
            }
        }

        /// <summary>
        /// Shows the hardware cursor if hideHardwareCursor is enabled.
        /// </summary>
        private void ShowCursor()
        {
            if (_settings.hideHardwareCursor)
            {
                Cursor.visible = true;
            }
        }

        /// <summary>
        /// Hides the hardware cursor if hideHardwareCursor is enabled.
        /// </summary>
        private void HideCursor()
        {
            if (_settings.hideHardwareCursor)
            {
                Cursor.visible = false;
            }
        }
        #endregion
    }
}
