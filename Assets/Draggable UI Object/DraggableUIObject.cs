using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

[RequireComponent(typeof(CanvasGroup))]
public class DraggableUIObject : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] private bool _handleDragPosition = true;
    private RectTransform _rectTransform;
    private Canvas _canvas;
    private CanvasGroup _canvasGroup;
    [SerializeField] private DraggableUIObject_Node _connectedNode; // Reference to the connected node

    [Header("Events")]
    public UnityEvent OnPointerDownEvent;
    public UnityEvent OnPointerUpEvent;
    public UnityEvent OnDragStartEvent;
    public UnityEvent OnDragEvent;
    public UnityEvent OnDragEndEvent;
    public UnityEvent OnConnectEvent;
    public UnityEvent OnDisconnectEvent;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public void SetConnectedNode(DraggableUIObject_Node node)
    {
        _connectedNode = node;
        OnConnectEvent?.Invoke();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnPointerDownEvent?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OnPointerUpEvent?.Invoke();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _canvasGroup.blocksRaycasts = false;
        if (_connectedNode != null)
        {
            _connectedNode.DisconnectObject();
            _connectedNode = null;
            OnDisconnectEvent?.Invoke();
        }
        OnDragStartEvent?.Invoke();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_handleDragPosition)
        {
            _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
        }
        OnDragEvent?.Invoke();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _canvasGroup.blocksRaycasts = true;
        OnDragEndEvent?.Invoke();
    }

    public void OnDrop(PointerEventData eventData)
    {
        // Handle drop logic here if necessary
    }
}
