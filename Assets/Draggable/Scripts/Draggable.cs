using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using System;

namespace Tools.UGUI
{
    /// <summary>
    /// Allows a UI element to be draggable within a canvas.
    /// Handles events for pointer interactions and dragging.
    /// Can connect to a DraggableNode for snapping functionality.
    /// Inherits from Unity's Selectable class.
    /// </summary>
    [AddComponentMenu("Tools/UGUI/Draggable")]
    [RequireComponent(typeof(CanvasGroup)), RequireComponent(typeof(RectTransform))]
    public class Draggable : Selectable, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        #region Nested Classes
        [Serializable]
        public class DraggableEvents
        {
            public UnityEvent OnPointerDownEvent;
            public UnityEvent OnPointerUpEvent;
            public UnityEvent OnDragStartEvent;
            public UnityEvent OnDragEvent;
            public UnityEvent OnDragEndEvent;
            public UnityEvent<GameObject> OnConnectEvent;
            public UnityEvent<GameObject> OnDisconnectEvent;
        }

        [Serializable]
        public class DraggableData
        {
            [SerializeField]
            private DraggableNode _connectedNode; // Reference to the connected node

            public DraggableNode ConnectedNode
            {
                get { return _connectedNode; }
                internal set { _connectedNode = value; }
            }
        }

        #endregion
        #region Fields
        [Header("Settings")]
        [SerializeField] private bool _handleDragPosition = true;
        [SerializeField] [Tooltip("Leave blank for no Key required")] private string _key; // Key for matching with nodes
        private RectTransform _rectTransform;
        private Canvas _canvas;
        private CanvasGroup _canvasGroup;
        [Space(10)]
        public DraggableData Data;
        public DraggableEvents Events;

        #endregion

        #region Unity Methods

        protected override void Awake()
        {
            base.Awake();
            _rectTransform = GetComponent<RectTransform>();
            _canvas = GetComponentInParent<Canvas>();
            _canvasGroup = GetComponent<CanvasGroup>();

        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets the connected node for this draggable object.
        /// </summary>
        /// <param name="node">The node to connect to.</param>
        public void SetConnectedNode(DraggableNode node)
        {
            Data.ConnectedNode = node;
            Events.OnConnectEvent?.Invoke(node.gameObject);
        }

        /// <summary>
        /// Gets the key for this draggable object.
        /// </summary>
        /// <returns>The key string.</returns>
        public string GetKey()
        {
            return _key;
        }

        /// <summary>
        /// Called when the pointer is pressed down on the draggable object.
        /// </summary>
        /// <param name="eventData">Pointer event data.</param>
        public override void OnPointerDown(PointerEventData eventData)
        {
            if (!IsInteractable()) return;

            base.OnPointerDown(eventData);
            Events.OnPointerDownEvent?.Invoke();
        }

        /// <summary>
        /// Called when the pointer is released from the draggable object.
        /// </summary>
        /// <param name="eventData">Pointer event data.</param>
        public override void OnPointerUp(PointerEventData eventData)
        {
            if (!IsInteractable()) return;

            base.OnPointerUp(eventData);
            Events.OnPointerUpEvent?.Invoke();
        }

        /// <summary>
        /// Called when dragging begins on the draggable object.
        /// </summary>
        /// <param name="eventData">Pointer event data.</param>
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!IsInteractable()) return;

            SetCanvasGroupBlocksRaycasts(false);
            if (Data.ConnectedNode != null)
            {
                Events.OnDisconnectEvent?.Invoke(Data.ConnectedNode.gameObject);
                Data.ConnectedNode.DisconnectObject();
                Data.ConnectedNode = null;
            }
            Events.OnDragStartEvent?.Invoke();
        }

        /// <summary>
        /// Called while dragging the draggable object.
        /// </summary>
        /// <param name="eventData">Pointer event data.</param>
        public void OnDrag(PointerEventData eventData)
        {
            if (!IsInteractable()) return;

            if (_handleDragPosition)
            {
                _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
            }

            Events.OnDragEvent?.Invoke();
        }

        /// <summary>
        /// Called when dragging ends on the draggable object.
        /// </summary>
        /// <param name="eventData">Pointer event data.</param>
        public void OnEndDrag(PointerEventData eventData)
        {
            if (!IsInteractable()) return;

            SetCanvasGroupBlocksRaycasts(true);
            Events.OnDragEndEvent?.Invoke();
        }

        #endregion

        #region Private Methods

        private void SetCanvasGroupBlocksRaycasts(bool value)
        {
            _canvasGroup.blocksRaycasts = value;
        }

        #endregion
    }
}
