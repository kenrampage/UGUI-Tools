using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class VirtualPointerCursorStateMachine : MonoBehaviour
{
    #region Nested Classes


    [Serializable]
    public class CursorState
    {
        public string StateName;
        public GameObject Object;
    }

    #endregion

    #region Fields

    [SerializeField]
    private List<CursorState> _states;

    private CursorState _currentState;
    private CursorState _previousState;


    #endregion

    #region Unity Methods

    private void Update()
    {

    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Sets the state of the state machine to the specified state.
    /// </summary>
    /// <param name="newStateName">The name of the new state.</param>
    public void SetState(string newStateName)
    {
        if (_states == null || _states.Count == 0)
        {
            Debug.LogWarning("State list is empty or null.");
            return;
        }

        CursorState newState = _states.Find(state => state.StateName == newStateName);
        if (newState != null && newState != _currentState)
        {
            HandleStateChange(newState);
        }
        else
        {
            Debug.LogWarning($"State '{newStateName}' not found in the state list.");
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Handles the state change logic.
    /// </summary>
    /// <param name="newState">The new state to transition to.</param>
    private void HandleStateChange(CursorState newState)
    {
        foreach (var pair in _states)
        {
            print("Iteration");

            if (pair.Object != null) 
            {
                // Enable the GameObject if its key matches the provided key, otherwise disable it
                pair.Object.SetActive(pair.StateName == newState.StateName);

            }
        }
    }

    #endregion
}
