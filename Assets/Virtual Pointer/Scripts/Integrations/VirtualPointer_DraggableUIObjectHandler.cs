using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.Events;


public class VirtualPointer_DraggableUIObjectHandler : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] private DraggableUIObject _previousHoveredDraggable;
    [SerializeField] private DraggableUIObject _currentHoveredDraggable;
    [Space(10)]
    [SerializeField] private DraggableUIObject _currentDraggedDraggable;
    [Space(10)]
    private bool _isDragging = false;

    [Header("Settings")]
    [SerializeField] private RectTransform _pointerRectTransform;

    [Header("Events")]
    public UnityEvent<GameObject> OnDragStart;
    public UnityEvent<GameObject> OnDragging;
    public UnityEvent<GameObject> OnDragEnd;

    public UnityEvent<GameObject> OnDraggableHovered;
    public UnityEvent<GameObject> OnDraggableUnhovered;

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

    private void Update()
    {
        UpdateHoveredDraggable();
        UpdateDraggingState();
    }

    #endregion

    #region Private Methods

    private void UpdateHoveredDraggable()
    {
        if (_isDragging) return;

        var pointerEventData = CreatePointerEventData();
        var raycastResults = RaycastUIObjects(pointerEventData);

        DraggableUIObject foundDraggable = FindFirstDraggable(raycastResults);

        if (foundDraggable != null)
        {
            UpdateHoveredObject(foundDraggable);
        }
        else if (_currentHoveredDraggable != null)
        {
            UnhoverCurrentObject();
        }
    }

    private DraggableUIObject FindFirstDraggable(List<RaycastResult> raycastResults)
    {
        foreach (var result in raycastResults)
        {
            var draggable = result.gameObject.GetComponent<DraggableUIObject>();
            if (draggable != null)
            {
                return draggable;
            }
        }
        return null;
    }

    private void UpdateHoveredObject(DraggableUIObject newHovered)
    {

        if (_currentHoveredDraggable != null)
        {
            UnhoverCurrentObject();
        }

        _previousHoveredDraggable = _currentHoveredDraggable;
        _currentHoveredDraggable = newHovered;
        if (_currentHoveredDraggable != null)
        {
            OnDraggableHovered.Invoke(_currentHoveredDraggable.gameObject);
            RegisterDragEvents(_currentHoveredDraggable);
        }

    }

    private void UnhoverCurrentObject()
    {
        if (_currentHoveredDraggable != null)
        {
            OnDraggableUnhovered.Invoke(_currentHoveredDraggable.gameObject);
            UnregisterDragEvents(_currentHoveredDraggable);
            _currentHoveredDraggable = null;
        }
    }

    private void RegisterDragEvents(DraggableUIObject draggable)
    {
        if (draggable != null)
        {
            draggable.OnPointerDownEvent.AddListener(HandlePointerDown);
            draggable.OnPointerUpEvent.AddListener(HandlePointerUp);
            draggable.OnDragStartEvent.AddListener(HandleDragStart);
            draggable.OnDragEvent.AddListener(HandleDragging);
            draggable.OnDragEndEvent.AddListener(HandleDragEnd);
        }
    }

    private void UnregisterDragEvents(DraggableUIObject draggable)
    {
        if (draggable != null)
        {
            draggable.OnPointerDownEvent.RemoveListener(HandlePointerDown);
            draggable.OnPointerUpEvent.RemoveListener(HandlePointerUp);
            draggable.OnDragEndEvent.RemoveListener(HandleDragEnd);
            draggable.OnDragEvent.RemoveListener(HandleDragging);
            draggable.OnDragStartEvent.RemoveListener(HandleDragStart);
        }
    }

    private PointerEventData CreatePointerEventData()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        Vector2 screenPosition = RectTransformUtility.WorldToScreenPoint(null, _pointerRectTransform.position);
        pointerEventData.position = screenPosition;
        return pointerEventData;
    }

    private List<RaycastResult> RaycastUIObjects(PointerEventData pointerEventData)
    {
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResults);
        return raycastResults;
    }

    private void UpdateDraggingState()
    {
        if (_isDragging && _currentDraggedDraggable != null)
        {
            OnDragging.Invoke(_currentDraggedDraggable.gameObject);
        }
    }

    #endregion

    #region Public Methods

    public void HandlePointerDown()
    {
        HandleDragStart();
    }

    public void HandlePointerUp()
    {
        HandleDragEnd();
    }

    public void HandleDragStart()
    {
        if (_isDragging) return;

        _previousHoveredDraggable = _currentHoveredDraggable;
        _currentDraggedDraggable = _currentHoveredDraggable;

        if (_currentDraggedDraggable != null)
        {
            OnDragStart.Invoke(_currentDraggedDraggable.gameObject);
            _isDragging = true;
        }
    }

    public void HandleDragging()
    {
        if (_isDragging && _currentDraggedDraggable != null)
        {
            OnDragging.Invoke(_currentDraggedDraggable.gameObject);
        }
    }

    public void HandleDragEnd()
    {
        if (_isDragging)
        {
            OnDragEnd.Invoke(_currentDraggedDraggable.gameObject);
            _isDragging = false;
            UnregisterDragEvents(_currentDraggedDraggable);
            _currentDraggedDraggable = null;
        }
    }

    #endregion
}
