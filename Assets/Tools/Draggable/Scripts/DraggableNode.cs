using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;

namespace Tools.UGUI.Draggable
{
    /// <summary>
    /// Represents a node that can connect to a draggable UI element.
    /// Handles events for pointer interactions and dropping.
    /// Can snap a draggable selectable element to a target position.
    /// </summary>
    [AddComponentMenu("Tools/UGUI/Draggable Node")]
    [RequireComponent(typeof(CanvasGroup)), RequireComponent(typeof(RectTransform))]
    public class DraggableNode : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler
    {
        #region Nested Classes
        [Serializable]
        public class DraggableData
        {
            [SerializeField] private Draggable _connectedObject;

            public Draggable ConnectedObject
            {
                get { return _connectedObject; }
                internal set { _connectedObject = value; }
            }
        }

        [Serializable]
        public class DraggableNodeEvents
        {
            public UnityEvent OnPointerEnterEvent;
            public UnityEvent OnPointerExitEvent;
            public UnityEvent<GameObject> OnConnectObjectEvent;
            public UnityEvent<GameObject> OnDisconnectObjectEvent;
        }

        #endregion
        #region Fields

        [Header("Settings")]
        [SerializeField] private bool _onEnableSnapConnectedObject;
        [SerializeField] private string _key; // Key for matching with draggables
        private RectTransform _rectTransform;
        private CanvasGroup _canvasGroup;
        private bool _isHovering;

        public DraggableData Data;
        public DraggableNodeEvents Events;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _rectTransform = GetComponent<RectTransform>();
        }

        private void OnEnable()
        {
            if (_onEnableSnapConnectedObject && Data.ConnectedObject != null)
            {
                ConnectObject(Data.ConnectedObject);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Called when the pointer enters the draggable node.
        /// </summary>
        /// <param name="eventData">Pointer event data.</param>
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (IsDraggingDraggable(eventData))
            {
                _isHovering = true;
                Events.OnPointerEnterEvent?.Invoke();
            }
        }

        /// <summary>
        /// Called when the pointer exits the draggable node.
        /// </summary>
        /// <param name="eventData">Pointer event data.</param>
        public void OnPointerExit(PointerEventData eventData)
        {
            if (IsDraggingDraggable(eventData))
            {
                _isHovering = false;
                Events.OnPointerExitEvent?.Invoke();
            }
        }

        /// <summary>
        /// Called when an object is dropped on the draggable node.
        /// </summary>
        /// <param name="eventData">Pointer event data.</param>
        public void OnDrop(PointerEventData eventData)
        {
            if (_isHovering)
            {
                Draggable draggable = eventData.pointerDrag.GetComponent<Draggable>();
                if (draggable != null && (string.IsNullOrEmpty(_key) || _key == draggable.GetKey()))
                {
                    ConnectObject(draggable);
                }
            }
        }

        /// <summary>
        /// Connects a draggable object to this node.
        /// </summary>
        /// <param name="draggable">The draggable object to connect.</param>
        private void ConnectObject(Draggable draggable)
        {
            if (_rectTransform != null)
            {
                if (draggable != null)
                {
                    draggable.transform.position = _rectTransform.position;
                    Data.ConnectedObject = draggable;
                    Data.ConnectedObject.SetConnectedNode(this);

                    SetCanvasGroupBlocksRaycasts(false);
                    Events.OnConnectObjectEvent?.Invoke(draggable.gameObject);
                }
            }
        }

        /// <summary>
        /// Disconnects the currently connected draggable object.
        /// </summary>
        public void DisconnectObject()
        {
            if (Data.ConnectedObject != null)
            {
                Events.OnDisconnectObjectEvent?.Invoke(Data.ConnectedObject.gameObject);
                Data.ConnectedObject = null;
                SetCanvasGroupBlocksRaycasts(true);
            }
        }

        /// <summary>
        /// Called when dragging begins on the draggable node.
        /// </summary>
        /// <param name="eventData">Pointer event data.</param>
        public void OnBeginDrag(PointerEventData eventData)
        {
            print("OnBeginDrag");
            if (Data.ConnectedObject != null && eventData.pointerDrag == Data.ConnectedObject.gameObject)
            {
                DisconnectObject();
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Checks if the pointer is dragging a draggable UI object.
        /// </summary>
        /// <param name="eventData">Pointer event data.</param>
        /// <returns>True if dragging a draggable UI object, otherwise false.</returns>
        private bool IsDraggingDraggable(PointerEventData eventData)
        {
            return eventData.pointerDrag != null && eventData.pointerDrag.GetComponent<Draggable>() != null;
        }

        private void SetCanvasGroupBlocksRaycasts(bool value)
        {
            _canvasGroup.blocksRaycasts = value;
        }

        #endregion
    }
}
