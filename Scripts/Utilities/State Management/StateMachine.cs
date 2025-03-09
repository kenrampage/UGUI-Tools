using UnityEngine;
using System;

namespace KenRampage.Utilities.StateManagement
{
    /// <summary>
    /// Base class for state machine implementations.
    /// Handles core state tracking and transition logic.
    /// </summary>
    [AddComponentMenu("Ken Rampage/Utilities/State Management/State Machine")]
    public class StateMachine : MonoBehaviour
    {
        #region Nested Classes
        [Serializable]
        public class State
        {
            public string StateName;
            [HideInInspector] public bool IsCurrentState;
        }

        public enum InitializationTiming
        {
            Awake,
            Start,
            OnEnable,
            Never
        }

        [Serializable]
        public class Debug
        {
            public string CurrentStateName;
            public string PreviousStateName;
        }

        [Serializable]
        public class Settings
        {
            public bool EnterStateOnEnable = false;
            public bool ExitStateOnDisable = false;
            public bool ExitStateOnDestroy = false;
            [Space(10)]
            public string StartStateName;
            public InitializationTiming SetStartStateTiming = InitializationTiming.Never;
        }
        #endregion

        #region Fields
        [SerializeField] protected Settings _settings;
        [SerializeField] protected Debug _debug;
        
        protected State _currentState;
        protected State _previousState;
        protected readonly object _stateLock = new object();
        #endregion

        #region Unity Methods
        protected virtual void Awake()
        {
            if (_settings.SetStartStateTiming == InitializationTiming.Awake)
            {
                SetState(_settings.StartStateName);
            }
        }

        protected virtual void Start()
        {
            if (_settings.SetStartStateTiming == InitializationTiming.Start)
            {
                SetState(_settings.StartStateName);
            }
        }

        protected virtual void OnEnable()
        {
            if (_settings.SetStartStateTiming == InitializationTiming.OnEnable)
            {
                SetState(_settings.StartStateName);
            }
        }

        protected virtual void OnDisable()
        {
            if (_settings.ExitStateOnDisable && _currentState != null)
            {
                OnStateExit(_currentState);
            }
        }

        protected virtual void OnDestroy()
        {
            if (_settings.ExitStateOnDestroy && _currentState != null)
            {
                OnStateExit(_currentState);
            }
        }
        #endregion

        #region Public Methods
        public virtual void SetState(string newStateName)
        {
            if (string.IsNullOrEmpty(newStateName)) return;
            
            State newState = FindState(newStateName);
            if (newState != null && newState != _currentState)
            {
                HandleStateChange(newState);
            }
        }

        public string GetCurrentStateName() => _debug.CurrentStateName;
        public string GetPreviousStateName() => _debug.PreviousStateName;
        #endregion

        #region Protected Methods
        protected virtual void HandleStateChange(State newState)
        {
            lock (_stateLock)
            {
                if (_currentState != null)
                {
                    _currentState.IsCurrentState = false;
                    OnStateExit(_currentState);
                }

                _previousState = _currentState;
                _debug.PreviousStateName = _previousState?.StateName ?? "No Previous State";

                _currentState = newState;
                _debug.CurrentStateName = _currentState?.StateName ?? "No Current State";

                if (_currentState != null)
                {
                    _currentState.IsCurrentState = true;
                    OnStateEnter(_currentState);
                }
            }
        }

        protected virtual State FindState(string stateName)
        {
            // Override in derived classes to implement state lookup
            return null;
        }

        protected virtual void OnStateEnter(State state) { }
        protected virtual void OnStateUpdate(State state) { }
        protected virtual void OnStateExit(State state) { }
        #endregion
    }
}