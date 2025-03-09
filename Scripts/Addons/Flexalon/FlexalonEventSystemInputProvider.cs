using UnityEngine;
using Flexalon;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

namespace KenRampage.Addons.Flexalon
{
    /// <summary>
    /// Provides input handling for Flexalon using Unity's Event System and Input System.
    /// Tracks pointer position and click states for UI interaction.
    /// </summary>
    [AddComponentMenu("Ken Rampage/Addons/Flexalon/Event System Input Provider")]
    public class FlexalonEventSystemInputProvider : MonoBehaviour, InputProvider
    {
        private bool _isActive = false;

        public bool Active => _isActive;
        public Vector3 UIPointer => GetCurrentPointerPosition();
        public Ray Ray => Camera.main.ScreenPointToRay(UIPointer);
        public InputMode InputMode => InputMode.Raycast;
        public GameObject ExternalFocusedObject => null;

        private void Update()
        {
            UpdateInputState();
        }

        private void UpdateInputState()
        {
            if (EventSystem.current?.currentInputModule is InputSystemUIInputModule inputModule)
            {
                var pointAction = inputModule.point.action;
                var clickAction = inputModule.leftClick.action;

                _isActive = clickAction.IsPressed();
            }
            else
            {
                _isActive = false;
            }
        }

        private Vector3 GetCurrentPointerPosition()
        {
            if (EventSystem.current?.currentInputModule is InputSystemUIInputModule inputModule)
            {
                var pointAction = inputModule.point.action;
                return pointAction.ReadValue<Vector2>();
            }

            return Input.mousePosition;
        }
    }
}