using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

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

        [SerializeField] private InputActionReference _clickActionReference; 
        [SerializeField] private InputActionReference _pointerPositionActionReference; 
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

        [SerializeField] private HoveredComponentFinder<Selectable> _hoveredComponentFinder;

        private void OnEnable()
        {
            Cursor.visible = !_hideHardwareCursor;
        }

        private void Update()
        {

            _clickOn = _clickActionReference.action.ReadValue<float>() > 0;

            Vector2 pointerPosition = _pointerPositionActionReference.action.ReadValue<Vector2>();


            if (_setScreenPosition)
            {
                transform.position = pointerPosition;
            }


            _hoverOn = _hoveredComponentFinder.IsComponentFound;

            // Determine if dragging has started while hovering
            if (_clickOn && !_isDragging && _hoverOn && Vector2.Distance(pointerPosition, _lastPointerPosition) > 0.1f)
            {
                _isDragging = true;
                _dragStartedWhileHovering = true;
            }

            // Reset dragging state when click is released
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
            //Debug.Log($"State changed to: {currentState}");
            UpdateCursorVisibility();
        }

        private void UpdateCursorVisibility()
        {
            // Disable all cursors
            cursorSet.defaultCursor.SetActive(false);
            cursorSet.hoverCursor.SetActive(false);
            cursorSet.dragCursor.SetActive(false);
            cursorSet.clickCursor.SetActive(false);

            // Enable the cursor for the current state
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
