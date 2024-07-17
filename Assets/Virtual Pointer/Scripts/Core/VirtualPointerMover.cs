using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

/// <summary>
/// Handles movement of a virtual pointer based on input actions, supporting both directional and positional movement.
/// </summary>
[AddComponentMenu("Tools/Virtual Pointer/Virtual Pointer Mover")]
public class VirtualPointerMover : MonoBehaviour
{
    [Header("Directional Movement Settings")]
    [SerializeField] private InputActionReference _moveActionReference; // Reference to the input action for directional movement.
    [SerializeField] private float _moveSpeed = 1000.0f; // Speed at which the pointer moves directionally.
    [SerializeField] private bool _enableDirectionalMovement = true; // Toggle for enabling/disabling directional movement.

    [Header("Positional Movement Settings")]
    [SerializeField] private InputActionReference _pointerPositionInputAction; // Reference to the input action for positional movement.
    [SerializeField] private bool _enablePositionalMovement = true; // Toggle for enabling/disabling positional movement.

    [Header("Common Settings")]
    [SerializeField] private RectTransform _pointerRectTransform; // The RectTransform of the pointer.

    #region Unity Methods
    private void OnEnable()
    {
        if (_moveActionReference != null && _enableDirectionalMovement)
        {
            _moveActionReference.action.Enable(); // Enable the directional input action.
        }

        if (_pointerPositionInputAction != null && _enablePositionalMovement)
        {
            _pointerPositionInputAction.action.Enable(); // Enable the positional input action.
            _pointerPositionInputAction.action.performed += OnPointerPositionInputPerformed;
        }
    }

    private void OnDisable()
    {
        if (_moveActionReference != null)
        {
            _moveActionReference.action.Disable(); // Disable the directional input action.
        }

        if (_pointerPositionInputAction != null)
        {
            _pointerPositionInputAction.action.Disable(); // Disable the positional input action.
            _pointerPositionInputAction.action.performed -= OnPointerPositionInputPerformed;
        }
    }

    private void Update()
    {
        if (_enableDirectionalMovement)
        {
            HandleDirectionalMovement();
        }
    }
    #endregion

    #region Private Methods

    private void HandleDirectionalMovement()
    {
        if (_moveActionReference == null) return;

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

    private void OnPointerPositionInputPerformed(InputAction.CallbackContext context)
    {
        if (_enablePositionalMovement)
        {
            Vector2 newPosition = context.ReadValue<Vector2>();
            UpdateObjectPosition(newPosition);
        }
    }

    private void UpdateObjectPosition(Vector2 newPosition)
    {
        if (_pointerRectTransform != null)
        {
            RectTransform canvasRectTransform = GetCanvasRectTransform();
            if (canvasRectTransform != null)
            {
                Vector2 canvasSize = canvasRectTransform.sizeDelta;
                Vector2 localPoint;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, newPosition, null, out localPoint);
                Vector2 pointerSize = new Vector2(1, 1);
                Vector2 halfPointerSize = pointerSize * 0.5f;
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

    #region Public Methods

    public void SetEnableDirectionalMovement(bool enable)
    {
        _enableDirectionalMovement = enable;
        if (_enableDirectionalMovement)
        {
            _moveActionReference.action.Enable();
        }
        else
        {
            _moveActionReference.action.Disable();
        }
    }

    public void SetEnablePositionalMovement(bool enable)
    {
        _enablePositionalMovement = enable;
        if (_enablePositionalMovement)
        {
            _pointerPositionInputAction.action.Enable();
            _pointerPositionInputAction.action.performed += OnPointerPositionInputPerformed;
        }
        else
        {
            _pointerPositionInputAction.action.Disable();
            _pointerPositionInputAction.action.performed -= OnPointerPositionInputPerformed;
        }
    }

    #endregion
}