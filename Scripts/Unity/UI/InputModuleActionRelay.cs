using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

namespace KenRampage.Unity.UI
{
    /// <summary>
    /// Relays input actions from Unity's Input System UI Module to UnityEvents.
    /// Handles button inputs, movement, pointing, and scroll wheel actions through a configurable event system.
    /// </summary>
    [AddComponentMenu("Ken Rampage/Unity/Event System/Input Module Action Relay")]
    public class InputModuleActionRelay : MonoBehaviour
    {
        [System.Serializable]
        public class Events
        {
            public UnityEvent<float> OnSubmit;
            public UnityEvent<float> OnCancel;
            public UnityEvent<Vector2> OnMove;
            public UnityEvent<Vector2> OnPoint;
            public UnityEvent<float> OnLeftClick;
            public UnityEvent<Vector2> OnScrollWheel;
            public UnityEvent<float> OnMiddleClick;
            public UnityEvent<float> OnRightClick;
        }

        [SerializeField] private Events _events;

        private InputSystemUIInputModule _currentInputModule;
        private UnityEngine.EventSystems.EventSystem _lastEventSystem;

        private void Update()
        {
            CheckForInputSystemChanges();
            CheckButtonInputs();
        }

        private void CheckForInputSystemChanges()
        {
            if (UnityEngine.EventSystems.EventSystem.current == null) return;
            
            if (UnityEngine.EventSystems.EventSystem.current != _lastEventSystem || 
                UnityEngine.EventSystems.EventSystem.current.currentInputModule != _currentInputModule)
            {
                _lastEventSystem = UnityEngine.EventSystems.EventSystem.current;
                _currentInputModule = UnityEngine.EventSystems.EventSystem.current.currentInputModule as InputSystemUIInputModule;
                SetupActionEvents();
            }
        }

        private void SetupActionEvents()
        {
            if (_currentInputModule != null)
            {
                SetupActionEvent(_currentInputModule.submit, _events.OnSubmit);
                SetupActionEvent(_currentInputModule.cancel, _events.OnCancel);
                SetupActionEvent(_currentInputModule.move, _events.OnMove);
                SetupActionEvent(_currentInputModule.point, _events.OnPoint);
                SetupActionEvent(_currentInputModule.leftClick, _events.OnLeftClick);
                SetupActionEvent(_currentInputModule.scrollWheel, _events.OnScrollWheel);
                SetupActionEvent(_currentInputModule.middleClick, _events.OnMiddleClick);
                SetupActionEvent(_currentInputModule.rightClick, _events.OnRightClick);
            }
        }

        private void SetupActionEvent<T>(InputActionReference actionReference, UnityEvent<T> actionEvent) where T : struct
        {
            if (actionReference != null && actionReference.action != null)
            {
                switch (actionReference.action.type)
                {
                    case InputActionType.Button:
                        // Button actions are handled in Update
                        break;
                    case InputActionType.Value:
                    case InputActionType.PassThrough:
                        actionReference.action.performed += ctx => actionEvent.Invoke(ctx.ReadValue<T>());
                        break;
                }
            }
        }

        private void CheckButtonInputs()
        {
            if (_currentInputModule != null)
            {
                CheckButtonInput(_currentInputModule.submit, _events.OnSubmit);
                CheckButtonInput(_currentInputModule.cancel, _events.OnCancel);
                CheckButtonInput(_currentInputModule.leftClick, _events.OnLeftClick);
                CheckButtonInput(_currentInputModule.middleClick, _events.OnMiddleClick);
                CheckButtonInput(_currentInputModule.rightClick, _events.OnRightClick);
            }
        }

        private void CheckButtonInput<T>(InputActionReference actionReference, UnityEvent<T> actionEvent) where T : struct
        {
            if (actionReference != null && actionReference.action != null && actionReference.action.type == InputActionType.Button)
            {
                if (actionReference.action.triggered)
                {
                    actionEvent.Invoke(actionReference.action.ReadValue<T>());
                }
            }
        }

        private void OnDisable()
        {
            if (_currentInputModule != null)
            {
                RemoveActionEvent(_currentInputModule.submit, _events.OnSubmit);
                RemoveActionEvent(_currentInputModule.cancel, _events.OnCancel);
                RemoveActionEvent(_currentInputModule.move, _events.OnMove);
                RemoveActionEvent(_currentInputModule.point, _events.OnPoint);
                RemoveActionEvent(_currentInputModule.leftClick, _events.OnLeftClick);
                RemoveActionEvent(_currentInputModule.scrollWheel, _events.OnScrollWheel);
                RemoveActionEvent(_currentInputModule.middleClick, _events.OnMiddleClick);
                RemoveActionEvent(_currentInputModule.rightClick, _events.OnRightClick);
            }
        }

        private void RemoveActionEvent<T>(InputActionReference actionReference, UnityEvent<T> actionEvent) where T : struct
        {
            if (actionReference != null && actionReference.action != null)
            {
                switch (actionReference.action.type)
                {
                    case InputActionType.Value:
                    case InputActionType.PassThrough:
                        actionReference.action.performed -= ctx => actionEvent.Invoke(ctx.ReadValue<T>());
                        break;
                }
            }
        }
    }
}