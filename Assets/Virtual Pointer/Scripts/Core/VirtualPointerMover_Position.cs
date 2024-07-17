using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles direct movement of a virtual pointer based on input actions, following the hardware mouse cursor.
/// </summary>
[AddComponentMenu("Tools/Virtual Pointer/VirtualPointerMover by Position")]
public class VirtualPointerMover_Position : MonoBehaviour
{
    [SerializeField] private InputActionReference _pointerPositionInputAction;
    [Space(10)]
    [SerializeField] private RectTransform _pointerRectTransform;

    #region Unity Methods
    private void OnEnable()
    {
        _pointerPositionInputAction.action.Enable();
        _pointerPositionInputAction.action.performed += OnPointerPositionInputPerformed;
    }

    private void OnDisable()
    {
        _pointerPositionInputAction.action.Disable();
        _pointerPositionInputAction.action.performed -= OnPointerPositionInputPerformed;
    }
    #endregion

    #region Private Methods

    private void OnPointerPositionInputPerformed(InputAction.CallbackContext context)
    {
        Vector2 newPosition = context.ReadValue<Vector2>();
        UpdateObjectPosition(newPosition);
    }

    private void UpdateObjectPosition(Vector2 newPosition)
    {
        if (_pointerRectTransform != null)
        {
            // Find the canvas RectTransform by traversing up the hierarchy.
            RectTransform canvasRectTransform = null;
            Transform currentTransform = _pointerRectTransform;

            while (currentTransform != null)
            {
                if (currentTransform.GetComponent<Canvas>() != null)
                {
                    canvasRectTransform = currentTransform.GetComponent<RectTransform>();
                    break;
                }
                currentTransform = currentTransform.parent;
            }

            if (canvasRectTransform != null)
            {
                // Get the canvas dimensions.
                Vector2 canvasSize = canvasRectTransform.sizeDelta;

                // Convert the screen position to the local position of the canvas.
                Vector2 localPoint;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, newPosition, null, out localPoint);

                // Assume the size of the pointer object is 1 pixel.
                Vector2 pointerSize = new Vector2(1, 1);

                // Calculate half the size of the pointer.
                Vector2 halfPointerSize = pointerSize * 0.5f;

                // Calculate the clamped position based on the center of the pointer.
                Vector2 clampedPosition = localPoint;
                clampedPosition.x = Mathf.Clamp(clampedPosition.x, -canvasSize.x * 0.5f + halfPointerSize.x, canvasSize.x * 0.5f - halfPointerSize.x);
                clampedPosition.y = Mathf.Clamp(clampedPosition.y, -canvasSize.y * 0.5f + halfPointerSize.y, canvasSize.y * 0.5f - halfPointerSize.y);

                _pointerRectTransform.anchoredPosition = clampedPosition;
            }
            else
            {
                Debug.LogError("canvasRectTransform is null");
            }
        }
    }

    #endregion
}
