using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace KenRampage.Utilities.StateManagement
{
    /// <summary>
    /// A state machine that executes Unity events in sequence with configurable delays.
    /// Each state can have multiple steps for enter, update, and exit transitions.
    /// </summary>
    [AddComponentMenu("Ken Rampage/Utilities/State Management/State Machine | Event Steps")]
    public class StateMachine_EventSteps : StateMachine
    {
        #region Nested Classes
        [System.Serializable]
        public class StateEvent
        {
            public UnityEvent Event;
        }

        [System.Serializable]
        public class StateEventStep
        {
            [Tooltip("Optional description for this step.")]
            public string Description;
            public float Wait;
            public StateEvent Event;
        }

        [System.Serializable]
        public class StateWithEventSteps : State
        {
            public List<StateEventStep> OnEnter;
            public List<StateEventStep> OnUpdate;
            public List<StateEventStep> OnExit;
        }
        #endregion

        #region Fields
        [SerializeField] private List<StateWithEventSteps> _states;
        #endregion

        #region Unity Methods
        private void Update()
        {
            if (_currentState != null)
            {
                var stateWithSteps = _currentState as StateWithEventSteps;
                if (stateWithSteps?.OnUpdate != null)
                {
                    StartCoroutine(InvokeEventsWithDelay(stateWithSteps.OnUpdate));
                }
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
            var stateWithSteps = state as StateWithEventSteps;
            if (stateWithSteps?.OnEnter != null)
            {
                StartCoroutine(InvokeEventsWithDelay(stateWithSteps.OnEnter));
            }
        }

        protected override void OnStateExit(State state)
        {
            var stateWithSteps = state as StateWithEventSteps;
            if (stateWithSteps?.OnExit != null)
            {
                StartCoroutine(InvokeEventsWithDelay(stateWithSteps.OnExit));
            }
        }

        private IEnumerator InvokeEventsWithDelay(List<StateEventStep> stateEvents)
        {
            foreach (var stateEvent in stateEvents)
            {
                yield return new WaitForSecondsRealtime(stateEvent.Wait);
                stateEvent.Event?.Event?.Invoke();
            }
        }
        #endregion
    }
}