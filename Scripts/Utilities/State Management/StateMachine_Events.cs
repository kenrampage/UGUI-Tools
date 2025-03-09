using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace KenRampage.Utilities.StateManagement
{
    /// <summary>
    /// A state machine that uses Unity events to handle state transitions.
    /// States can be configured to trigger events on entering, updating, and exiting.
    /// </summary>
    [AddComponentMenu("Ken Rampage/Utilities/State Management/State Machine | Events")]
    public class StateMachine_Events : StateMachine
    {
        #region Nested Classes
        [System.Serializable]
        public class StateEvents
        {
            public UnityEvent OnEnter;
            public UnityEvent OnUpdate;
            public UnityEvent OnExit;
        }

        [System.Serializable]
        public class StateWithEvents : State
        {
            public StateEvents Events;
        }
        #endregion

        #region Fields
        [SerializeField] private List<StateWithEvents> _states;
        #endregion

        #region Unity Methods
        private void Update()
        {
            if (_currentState != null)
            {
                var stateWithEvents = _currentState as StateWithEvents;
                stateWithEvents?.Events.OnUpdate?.Invoke();
            }
        }
        #endregion

        #region Protected Methods
        protected override State FindState(string stateName)
        {
            return _states?.FirstOrDefault(state => state.StateName == stateName);
        }

        protected override void OnStateEnter(State state)
        {
            var stateWithEvents = state as StateWithEvents;
            stateWithEvents?.Events.OnEnter?.Invoke();
        }

        protected override void OnStateExit(State state)
        {
            var stateWithEvents = state as StateWithEvents;
            stateWithEvents?.Events.OnExit?.Invoke();
        }
        #endregion
    }
}