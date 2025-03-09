using UnityEngine;
using UnityEngine.InputSystem;

namespace KenRampage.Utilities.UI.Bindings
{
    /// <summary>
    /// Makes a UI object follow input action position values.
    /// Useful for cursor or pointer-following UI elements.
    /// </summary>
    [AddComponentMenu("Ken Rampage/Utilities/UI/Bindings/Object Follow Input Action")]
    public class ObjectFollowInputAction : MonoBehaviour
    {
        [SerializeField] private InputActionReference _inputAction;
        private RectTransform _rectTransform;

        private void Start()
        {
            _rectTransform = GetComponent<RectTransform>();
            _inputAction.action.Enable();
            _inputAction.action.performed += OnInputActionPerformed;
        }

        private void OnDestroy()
        {
            _inputAction.action.Disable();
            _inputAction.action.performed -= OnInputActionPerformed;
        }

        private void OnInputActionPerformed(InputAction.CallbackContext context)
        {
            Vector2 newPosition = context.ReadValue<Vector2>();
            UpdateCursorPosition(newPosition);
        }

        private void UpdateCursorPosition(Vector2 newPosition)
        {
            _rectTransform.position = newPosition;
        }
    }
}