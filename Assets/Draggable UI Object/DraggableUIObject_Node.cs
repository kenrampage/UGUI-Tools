using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;


[RequireComponent(typeof(CanvasGroup))]
public class DraggableUIObject_Node : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler
{
    public RectTransform targetRectTransform;
    private CanvasGroup _canvasGroup;

    private bool isHovering;
    [SerializeField] private DraggableUIObject _connectedObject;
    [SerializeField] private bool _onEnableSnapConnectedObject;

    [Header("Events")]
    public UnityEvent OnPointerEnterEvent;
    public UnityEvent OnPointerExitEvent;
    public UnityEvent OnConnectToObjectEvent;
    public UnityEvent OnDisconnectObjectEvent;



    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        if (_onEnableSnapConnectedObject && _connectedObject != null)
        {
            ConnectObject(_connectedObject);
        }
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (IsDraggingDraggableUIObject(eventData))
        {
            isHovering = true;
            OnPointerEnterEvent?.Invoke();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (IsDraggingDraggableUIObject(eventData))
        {
            isHovering = false;
            OnPointerExitEvent?.Invoke();
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (isHovering)
        {
            ConnectObject(eventData.pointerDrag.GetComponent<DraggableUIObject>());
        }
    }

    private void ConnectObject(DraggableUIObject draggableUIObject)
    {
        if (targetRectTransform != null)
        {
            if (draggableUIObject != null)
            {
                draggableUIObject.transform.position = targetRectTransform.position;
                _connectedObject = draggableUIObject;
                _connectedObject.SetConnectedNode(this);

                _canvasGroup.blocksRaycasts = false;
                OnConnectToObjectEvent?.Invoke();


            }
        }
    }

    public void DisconnectObject()
    {
        _connectedObject = null;
        _canvasGroup.blocksRaycasts = true;
        OnDisconnectObjectEvent?.Invoke(); // Invoke disconnect event
                                           // Perform disconnection logic here
    }

    private bool IsDraggingDraggableUIObject(PointerEventData eventData)
    {
        return eventData.pointerDrag != null && eventData.pointerDrag.GetComponent<DraggableUIObject>() != null;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        print("OnBeginDrag");
        if (_connectedObject != null && eventData.pointerDrag == _connectedObject.gameObject)
        {
            DisconnectObject();
        }
    }
}

