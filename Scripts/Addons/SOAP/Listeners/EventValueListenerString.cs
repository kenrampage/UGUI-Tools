using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Obvious.Soap;
using System;

namespace KenRampage.Addons.SOAP.Listeners
{
    /// <summary>
    /// Listens for ScriptableEventString events and triggers corresponding UnityEvents when the event value matches defined conditions.
    /// Supports multiple value-based responses through a configurable list of EventValueResponseString entries.
    /// </summary>
    [AddComponentMenu("Ken Rampage/Addons/SOAP/Listeners/Event Value Listener String")]
    public class EventValueListenerString : MonoBehaviour
    {
        [SerializeField] private ScriptableEventString _scriptableEvent;
        [SerializeField] private List<EventValueResponseString> _valueResponses;

        [Serializable]
        public class EventValueResponseString
        {
            public string Value;
            public UnityEvent Event;
        }

        private void OnEnable()
        {
            _scriptableEvent.OnRaised += ProcessValueResponses;
        }

        private void OnDisable()
        {
            _scriptableEvent.OnRaised -= ProcessValueResponses;
        }

        private void ProcessValueResponses(string value)
        {
            foreach (var response in _valueResponses)
            {
                if (response.Value == value)
                {
                    response.Event?.Invoke();
                }
            }
        }
    }
}