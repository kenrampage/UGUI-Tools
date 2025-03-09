using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Obvious.Soap;
using System;

namespace KenRampage.Addons.SOAP.Listeners
{
    /// <summary>
    /// Listens for ScriptableEventBool events and triggers corresponding UnityEvents when the event value matches defined conditions.
    /// Supports multiple value-based responses through a configurable list of EventValueResponseBool entries.
    /// </summary>
    [AddComponentMenu("Ken Rampage/Addons/SOAP/Listeners/Event Value Listener Bool")]
    public class EventValueListenerBool : MonoBehaviour
    {
        [SerializeField] private ScriptableEventBool _scriptableEvent;
        [SerializeField] private List<EventValueResponseBool> _valueResponses;

        [Serializable]
        public class EventValueResponseBool
        {
            public bool Value;
            public UnityEvent<bool> Event;
        }

        private void OnEnable()
        {
            _scriptableEvent.OnRaised += ProcessValueResponses;
        }

        private void OnDisable()
        {
            _scriptableEvent.OnRaised -= ProcessValueResponses;
        }

        private void ProcessValueResponses(bool value)
        {
            foreach (var response in _valueResponses)
            {
                if (response.Value == value)
                {
                    response.Event?.Invoke(value);
                }
            }
        }
    }
}