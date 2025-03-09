using UnityEngine;
using UnityEngine.Events;

namespace KenRampage.Utilities.Inspector
{
    /// <summary>
    /// Invokes UnityEvents when Unity lifecycle functions occur.
    /// Supports Awake, OnEnable, Start, OnApplicationFocus, OnApplicationPause, OnApplicationQuit, OnDestroy, and OnDisable.
    /// Reference: https://docs.unity3d.com/560/Documentation/uploads/Main/monobehaviour_flowchart.svg
    /// </summary>
    [AddComponentMenu("Ken Rampage/Utilities/Inspector/Event Function Helper")]
    public class EventFunctionHelper : MonoBehaviour
    {
        private enum EventFunctionType
        {
            Awake,
            OnEnable,
            Start,
            OnApplicationFocus,
            OnApplicationPause,
            OnApplicationQuit,
            OnDestroy,
            OnDisable,
        }

        [Header("Settings")]
        [SerializeField] private EventFunctionType _targetEventFunctionType;
        public UnityEvent ResponseEvent;

        #region System Event Functions
        private void Awake()
        {
            if (_targetEventFunctionType == EventFunctionType.Awake)
            {
                ResponseEvent?.Invoke();
            }
        }

        private void OnEnable()
        {
            if (_targetEventFunctionType == EventFunctionType.OnEnable)
            {
                ResponseEvent?.Invoke();
            }
        }

        private void Start()
        {
            if (_targetEventFunctionType == EventFunctionType.Start)
            {
                ResponseEvent?.Invoke();
            }
        }

        private void OnApplicationFocus()
        {
            if (_targetEventFunctionType == EventFunctionType.OnApplicationFocus)
            {
                ResponseEvent?.Invoke();
            }
        }

        private void OnApplicationPause()
        {
            if (_targetEventFunctionType == EventFunctionType.OnApplicationPause)
            {
                ResponseEvent?.Invoke();
            }
        }

        private void OnApplicationQuit()
        {
            if (_targetEventFunctionType == EventFunctionType.OnApplicationQuit)
            {
                ResponseEvent?.Invoke();
            }
        }

        private void OnDestroy()
        {
            if (_targetEventFunctionType == EventFunctionType.OnDestroy)
            {
                ResponseEvent?.Invoke();
            }
        }

        private void OnDisable()
        {
            if (_targetEventFunctionType == EventFunctionType.OnDisable)
            {
                ResponseEvent?.Invoke();
            }
        }
        #endregion
    }
}