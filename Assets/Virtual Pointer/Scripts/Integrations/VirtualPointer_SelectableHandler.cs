using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(RectTransform))]
public class VirtualPointer_SelectableHandler : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] private Selectable _previousHoveredSelectable;
    [SerializeField] private Selectable _currentHoveredSelectable;
    [Space(10)]
    [SerializeField] private Selectable _currentSelectedSelectable;

    [Header("Settings")]
    [SerializeField] private InputActionReference _clickActionReference;
    [SerializeField] private RectTransform _pointerRectTransform;
    [Space(10)]
    [SerializeField] private bool _handleSelection = false;

    [Header("Events")]
    public UnityEvent<GameObject> OnHoverStart;
    public UnityEvent<GameObject> OnHoverChange;
    public UnityEvent<GameObject> OnHoverEnd;
    public UnityEvent<GameObject> OnClickDown;
    public UnityEvent OnClickUp;



    #region Unity Methods
    private void Awake()
    {
        if (_pointerRectTransform == null)
        {
            _pointerRectTransform = GetComponent<RectTransform>();

            if (_pointerRectTransform == null)
            {
                Debug.LogError("Script needs a Rect Transform. Should be set to Pointer Object", this);
            }
        }
    }

    private void OnEnable()
    {
        if (_clickActionReference != null)
        {
            _clickActionReference.action.performed += OnClickPerformed;
        }
    }

    private void OnDisable()
    {
        if (_clickActionReference != null)
        {
            _clickActionReference.action.performed -= OnClickPerformed;
        }
    }

    private void Update()
    {
        LookForSelectable();
        HandleSelectableState();
        CheckSelectedSelectable();
    }
    #endregion

    #region Public Methods
    public void ClearHoverState()
    {
        _previousHoveredSelectable = null;
        _currentHoveredSelectable = null;
    }
    #endregion

    #region Private Methods
    private void LookForSelectable()
    {
        PointerEventData pointerEventData = GetPointerEventData();
        List<RaycastResult> raycastResults = RaycastSelectables(pointerEventData);

        bool selectableFound = false;

        if (raycastResults.Count > 0)
        {
            foreach (RaycastResult result in raycastResults)
            {
                Selectable selectable = result.gameObject.GetComponent<Selectable>();

                if (selectable != null)
                {
                    _previousHoveredSelectable = _currentHoveredSelectable;
                    _currentHoveredSelectable = selectable;
                    selectableFound = true;

                    break;
                }
            }
        }

        if (!selectableFound)
        {
            _previousHoveredSelectable = _currentHoveredSelectable;
            _currentHoveredSelectable = null;
        }
    }

    private void HandleSelectableState()
    {
        if (_currentHoveredSelectable != _previousHoveredSelectable)
        {
            switch (_currentHoveredSelectable, _previousHoveredSelectable)
            {
                case (null, not null):
                    OnHoverEnd?.Invoke(_previousHoveredSelectable.gameObject);
                    Deselect();
                    break;

                case (not null, null):
                    OnHoverStart?.Invoke(_currentHoveredSelectable.gameObject);
                    Select(_currentHoveredSelectable);
                    break;

                case (not null, not null):
                    OnHoverChange?.Invoke(_currentHoveredSelectable.gameObject);
                    Select(_currentHoveredSelectable);
                    break;
            }

            _previousHoveredSelectable = _currentHoveredSelectable;
        }
    }

    private void Select(Selectable selectable)
    {
        if (_handleSelection)
        {
            EventSystem.current.SetSelectedGameObject(selectable.gameObject);
        }
    }

    private void Deselect()
    {
        if (_handleSelection)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    private void CheckSelectedSelectable()
    {
        GameObject currentSelectedGameObject = EventSystem.current.currentSelectedGameObject;

        if (currentSelectedGameObject != null)
        {
            _currentSelectedSelectable = currentSelectedGameObject.GetComponent<Selectable>();
        }
        else
        {
            _currentSelectedSelectable = null;
        }

        if (_currentSelectedSelectable != _currentHoveredSelectable)
        {
            // Handle the case where the currently selected Selectable is different from the currently hovered Selectable
            // You can add your custom logic here
        }
    }

    private PointerEventData GetPointerEventData()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        Vector2 screenPosition = RectTransformUtility.WorldToScreenPoint(null, _pointerRectTransform.position);
        pointerEventData.position = screenPosition;
        return pointerEventData;
    }

    private List<RaycastResult> RaycastSelectables(PointerEventData pointerEventData)
    {
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResults);
        return raycastResults;
    }

    private void OnClickPerformed(InputAction.CallbackContext context)
    {
        float value = _clickActionReference.action.ReadValue<float>();

        if (_currentHoveredSelectable != null)
        {
            if (value == 0)
            {
                OnClickUp?.Invoke();
            }
            else if (value != 0)
            {
                OnClickDown?.Invoke(_currentHoveredSelectable.gameObject);
            }
        }
    }


    #endregion
}
