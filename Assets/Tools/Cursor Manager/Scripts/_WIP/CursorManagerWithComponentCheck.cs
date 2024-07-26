using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.InputSystem.UI;
using Tools.UGUI.Helpers;

namespace Tools.UGUI.CursorManager
{
    /// <summary>
    /// Manages cursor states and visuals based on user interaction and hovered components.
    /// </summary>
    [AddComponentMenu("Tools/UGUI/Cursor Manager/Selectable Hover Handler")]
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
        [SerializeField] private bool setScreenPosition = true;
        [SerializeField] private bool hideHardwareCursor = true;
        [SerializeField] private HoveredComponentFinder_Selectable hoveredComponentFinder;

        public UnityEvent OnClickStatusChanged;
        public UnityEvent OnHoverStatusChanged;

        private bool clickOn;
        private bool hoverOn;
        private bool isDragging;
        private bool dragStartedWhileHovering;
        private Vector2 lastPointerPosition;
        private State currentState;

        private void OnEnable()
        {
            Cursor.visible = !hideHardwareCursor;
        }

        private void Update()
        {
            if (!(EventSystem.current.currentInputModule is InputSystemUIInputModule inputModule))
            {
                Debug.LogWarning("Current input module is not an InputSystemUIInputModule.");
                return;
            }

            UpdateInputState(inputModule);
            UpdateCursorPosition(inputModule);
            UpdateDragState();
            CalculateNewState();
        }

        private void UpdateInputState(InputSystemUIInputModule inputModule)
        {
            clickOn = inputModule.leftClick.action.ReadValue<float>() > 0;
            hoverOn = hoveredComponentFinder.IsComponentFound;
        }

        private void UpdateCursorPosition(InputSystemUIInputModule inputModule)
        {
            if (setScreenPosition)
            {
                transform.position = inputModule.point.action.ReadValue<Vector2>();
            }
        }

        private void UpdateDragState()
        {
            Vector2 pointerPosition = Mouse.current.position.ReadValue();

            if (clickOn && !isDragging && hoverOn && Vector2.Distance(pointerPosition, lastPointerPosition) > 0.1f)
            {
                isDragging = true;
                dragStartedWhileHovering = true;
            }

            if (!clickOn)
            {
                isDragging = false;
                dragStartedWhileHovering = false;
            }

            lastPointerPosition = pointerPosition;
        }

        private void CalculateNewState()
        {
            State newState = DetermineNewState();

            if (newState != currentState)
            {
                currentState = newState;
                UpdateCursorVisibility();
            }
        }

        private State DetermineNewState()
        {
            if (clickOn && hoverOn && !isDragging)
                return State.Click;
            if (isDragging && dragStartedWhileHovering)
                return State.Drag;
            if (hoverOn)
                return State.Hover;
            return State.Default;
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
