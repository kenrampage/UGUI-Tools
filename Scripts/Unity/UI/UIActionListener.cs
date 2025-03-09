using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace KenRampage.Unity.UI
{
    /// <summary>
    /// Listens for UI input events (clicks, scrolls, movement) and relays them through UnityEvents.
    /// Provides separate events for left, middle, and right clicks, as well as scroll, move, submit and cancel actions.
    /// </summary>
    [AddComponentMenu("Ken Rampage/Unity/Event System/UI Action Listener")]
    public class UIActionListener : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IScrollHandler, IMoveHandler, ISubmitHandler, ICancelHandler
    {
        [System.Serializable]
        public class EventsClass
        {
            public UnityEvent OnLeftClick;
            public UnityEvent OnMiddleClick;
            public UnityEvent OnRightClick;
            public UnityEvent OnScroll;
            public UnityEvent OnMove;
            public UnityEvent OnSubmit;
            public UnityEvent OnCancel;
        }

        [SerializeField] private EventsClass _events;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
                _events.OnLeftClick.Invoke();
            else if (eventData.button == PointerEventData.InputButton.Middle)
                _events.OnMiddleClick.Invoke();
            else if (eventData.button == PointerEventData.InputButton.Right)
                _events.OnRightClick.Invoke();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            // Optionally use if needed
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            // Optionally use if needed
        }

        public void OnScroll(PointerEventData eventData)
        {
            _events.OnScroll.Invoke();
        }

        public void OnMove(AxisEventData eventData)
        {
            _events.OnMove.Invoke();
        }

        public void OnSubmit(BaseEventData eventData)
        {
            _events.OnSubmit.Invoke();
        }

        public void OnCancel(BaseEventData eventData)
        {
            _events.OnCancel.Invoke();
        }
    }
}