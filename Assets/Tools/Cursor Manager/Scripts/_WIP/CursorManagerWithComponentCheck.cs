using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.InputSystem.UI;
using Tools.UGUI.CursorManager;

namespace Tools.UGUI.CursorManager._WIP
{
    public class CursorManagerWithComponentCheck : MonoBehaviour
    {
        public enum State
        {
            Default,
            Hover,
            Drag,
            Click
        }

        [System.Serializable]
        public class CursorSet
        {
            public GameObject defaultCursor;
            public GameObject hoverCursor;
            public GameObject dragCursor;
            public GameObject clickCursor;
        }
        [SerializeField] private CursorSet cursorSet;
        [SerializeField] private bool _setScreenPosition;
        [SerializeField] private bool _hideHardwareCursor;

        private bool _clickOn;
        private bool _hoverOn;
        private bool _isDragging;
        private bool _dragStartedWhileHovering;
        private Vector2 _lastPointerPosition;

        [SerializeField] private State currentState;

        public UnityEvent OnClickStatusChanged;
        public UnityEvent OnHoverStatusChanged;

        [SerializeField] private HoveredComponentFinder_Selectable _hoveredComponentFinder;

        private void OnEnable()
        {
            Cursor.visible = !_hideHardwareCursor;
        }

        private void Update()
        {
            var inputModule = EventSystem.current.currentInputModule as InputSystemUIInputModule;

            if (inputModule == null)
            {
                Debug.LogWarning("Current input module is not an InputSystemUIInputModule.");
                return;
            }

            _clickOn = inputModule.leftClick.action.ReadValue<float>() > 0;
            Vector2 pointerPosition = inputModule.point.action.ReadValue<Vector2>();

            if (_setScreenPosition)
            {
                transform.position = pointerPosition;
            }

            _hoverOn = _hoveredComponentFinder.IsComponentFound;

            if (_clickOn && !_isDragging && _hoverOn && Vector2.Distance(pointerPosition, _lastPointerPosition) > 0.1f)
            {
                _isDragging = true;
                _dragStartedWhileHovering = true;
            }

            if (!_clickOn)
            {
                _isDragging = false;
                _dragStartedWhileHovering = false;
            }

            _lastPointerPosition = pointerPosition;
            CalculateNewState();
        }

        private void CalculateNewState()
        {
            State newState;

            if (_clickOn && _hoverOn && !_isDragging)
            {
                newState = State.Click;
            }
            else if (_isDragging && _dragStartedWhileHovering)
            {
                newState = State.Drag;
            }
            else if (_hoverOn)
            {
                newState = State.Hover;
            }
            else
            {
                newState = State.Default;
            }

            if (newState != currentState)
            {
                currentState = newState;
                HandleStateChange();
            }
        }

        private void HandleStateChange()
        {
            UpdateCursorVisibility();
        }

        private void UpdateCursorVisibility()
        {
            cursorSet.defaultCursor.SetActive(false);
            cursorSet.hoverCursor.SetActive(false);
            cursorSet.dragCursor.SetActive(false);
            cursorSet.clickCursor.SetActive(false);

            switch (currentState)
            {
                case State.Default:
                    cursorSet.defaultCursor.SetActive(true);
                    break;
                case State.Hover:
                    cursorSet.hoverCursor.SetActive(true);
                    break;
                case State.Drag:
                    cursorSet.dragCursor.SetActive(true);
                    break;
                case State.Click:
                    cursorSet.clickCursor.SetActive(true);
                    break;
            }
        }
    }
}
