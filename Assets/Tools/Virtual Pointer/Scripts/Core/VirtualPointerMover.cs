using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

namespace Tools.UGUI.VirtualPointer
{
    /// <summary>
    /// Manages the movement of a virtual pointer using both directional and positional input.
    /// </summary>
    [AddComponentMenu("Tools/UGUI/Virtual Pointer/Virtual Pointer Mover")]
    public class VirtualPointerMover : MonoBehaviour
    {
        #region Fields
        [Header("Directional Movement Settings")]
        [SerializeField] private InputActionReference _moveActionReference;
        [SerializeField] private float _moveSpeed = 1000.0f;
        [SerializeField] private bool _enableDirectionalMovement = true;

        [Header("Positional Movement Settings")]
        [SerializeField] private InputActionReference _pointerPositionInputAction;
        [SerializeField] private bool _enablePositionalMovement = true;

        [Header("Common Settings")]
        [SerializeField] private RectTransform _pointerRectTransform;
        #endregion

        #region Unity Methods
        private void OnEnable()
        {
            EnableInputActions();
        }

        private void OnDisable()
        {
            DisableInputActions();
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
        private void EnableInputActions()
        {
            if (_moveActionReference != null && _enableDirectionalMovement)
            {
                _moveActionReference.action.Enable();
            }

            if (_pointerPositionInputAction != null && _enablePositionalMovement)
            {
                _pointerPositionInputAction.action.Enable();
                _pointerPositionInputAction.action.performed += OnPointerPositionInputPerformed;
            }
        }

        private void DisableInputActions()
        {
            if (_moveActionReference != null)
            {
                _moveActionReference.action.Disable();
            }

            if (_pointerPositionInputAction != null)
            {
                _pointerPositionInputAction.action.Disable();
                _pointerPositionInputAction.action.performed -= OnPointerPositionInputPerformed;
            }
        }

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
                Debug.LogError("Canvas RectTransform not found");
            }
        }

        private RectTransform GetCanvasRectTransform()
        {
            Transform currentTransform = _pointerRectTransform;
            while (currentTransform != null)
            {
                if (currentTransform.TryGetComponent(out Canvas canvas))
                {
                    return currentTransform as RectTransform;
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
                    Vector2 localPoint;
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, newPosition, null, out localPoint);
                    _pointerRectTransform.anchoredPosition = ClampPositionToCanvas(localPoint, canvasRectTransform.sizeDelta);
                }
                else
                {
                    Debug.LogError("Canvas RectTransform not found");
                }
            }
        }

        private Vector2 ClampPositionToCanvas(Vector2 position, Vector2 canvasSize)
        {
            Vector2 pointerSize = new Vector2(1, 1);
            Vector2 halfPointerSize = pointerSize * 0.5f;
            Vector2 clampedPosition = position;
            clampedPosition.x = Mathf.Clamp(clampedPosition.x, -canvasSize.x * 0.5f + halfPointerSize.x, canvasSize.x * 0.5f - halfPointerSize.x);
            clampedPosition.y = Mathf.Clamp(clampedPosition.y, -canvasSize.y * 0.5f + halfPointerSize.y, canvasSize.y * 0.5f - halfPointerSize.y);
            return clampedPosition;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Sets whether directional movement is enabled.
        /// </summary>
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

        /// <summary>
        /// Sets whether positional movement is enabled.
        /// </summary>
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

        /// <summary>
        /// Sets the pointer position to match the given GameObject's position.
        /// </summary>
        public void SetPointerPositionToGameObject(GameObject target)
        {
            if (target == null || _pointerRectTransform == null) return;

            Canvas targetCanvas = target.GetComponentInParent<Canvas>();
            if (targetCanvas == null) return;

            Vector2 screenPosition;

            if (targetCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                RectTransform targetRect = target.GetComponent<RectTransform>();
                screenPosition = targetRect != null ? RectTransformUtility.WorldToScreenPoint(null, targetRect.position) : RectTransformUtility.WorldToScreenPoint(null, target.transform.position);
            }
            else
            {
                Camera worldCamera = targetCanvas.worldCamera ?? Camera.main;
                screenPosition = RectTransformUtility.WorldToScreenPoint(worldCamera, target.transform.position);
            }

            _pointerRectTransform.position = screenPosition;
            ClampPointerPosition();
        }

        /// <summary>
        /// Sets the pointer position to match the currently selected object's position.
        /// </summary>
        public void SetPointerPositionToCurrentSelectedObject()
        {
            if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject != null)
            {
                SetPointerPositionToGameObject(EventSystem.current.currentSelectedGameObject);
            }
        }
        #endregion
    }
}
