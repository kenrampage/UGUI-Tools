using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

/// <summary>
/// Handles directional movement of a virtual pointer based on input actions.
/// </summary>
[AddComponentMenu("Tools/Virtual Pointer/Virtual Pointer Mover Directional")]
public class VirtualPointerMover_Direction : MonoBehaviour
{
    [SerializeField] private InputActionReference _moveActionReference; // Reference to the input action for movement.
    [SerializeField] private float _moveSpeed = 1000.0f; // Speed at which the pointer moves.
    [Space(10)]
    [SerializeField] private RectTransform _pointerRectTransform; // The RectTransform of the pointer.

    #region Unity Methods
    private void OnEnable()
    {
        _moveActionReference.action.Enable(); // Enable the input action.
    }

    private void OnDisable()
    {
        _moveActionReference.action.Disable(); // Disable the input action when the GameObject is disabled.
    }

    private void Update()
    {
        HandleMovement();
    }
    #endregion

    #region Private Methods

    private void HandleMovement()
    {
        Vector2 moveInput = _moveActionReference.action.ReadValue<Vector2>();
        Vector2 moveAmount = CalculateMoveAmount(moveInput);
        UpdatePointerPosition(moveAmount);
    }

    private Vector2 CalculateMoveAmount(Vector2 moveInput)
    {
        return moveInput * _moveSpeed * Time.unscaledDeltaTime;
    }

    private void UpdatePointerPosition(Vector2 moveAmount)
    {
        if (_pointerRectTransform != null)
        {
            _pointerRectTransform.anchoredPosition += moveAmount;
            ClampPointerPosition();
        }
    }

    private void ClampPointerPosition()
    {
        RectTransform canvasRectTransform = GetCanvasRectTransform();
        if (canvasRectTransform != null)
        {
            Vector2 canvasSize = canvasRectTransform.sizeDelta;
            Vector2 pointerSize = new Vector2(1, 1);
            Vector2 halfPointerSize = pointerSize * 0.5f;
            Vector2 clampedPosition = _pointerRectTransform.anchoredPosition;
            clampedPosition.x = Mathf.Clamp(clampedPosition.x, -canvasSize.x * 0.5f + halfPointerSize.x, canvasSize.x * 0.5f - halfPointerSize.x);
            clampedPosition.y = Mathf.Clamp(clampedPosition.y, -canvasSize.y * 0.5f + halfPointerSize.y, canvasSize.y * 0.5f - halfPointerSize.y);
            _pointerRectTransform.anchoredPosition = clampedPosition;
        }
        else
        {
            Debug.LogError("canvasRectTransform is null");
        }
    }

    private RectTransform GetCanvasRectTransform()
    {
        Transform currentTransform = _pointerRectTransform;
        while (currentTransform != null)
        {
            if (currentTransform.GetComponent<Canvas>() != null)
            {
                return currentTransform.GetComponent<RectTransform>();
            }
            currentTransform = currentTransform.parent;
        }
        return null;
    }

    #endregion
}
