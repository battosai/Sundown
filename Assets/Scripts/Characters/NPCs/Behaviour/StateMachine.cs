using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using Game;

public class StateMachine : MonoBehaviour
{
    private Dictionary<Type, BaseState> states;
    public BaseState currentState {get; private set;}
    public event Action<BaseState> OnStateChanged;

    public void SetStates(Dictionary<Type, BaseState> states)
    {
        this.states = states;
    }

    private void Update()
    {
        if(currentState == null)
            currentState = states.Values.First();
        // Debug.Log($"Current State: {currentState}");
        Type nextStateType = currentState?.Tick();
        if(nextStateType != null && nextStateType != currentState?.GetType())
            SwitchState(nextStateType);
    }

    private void SwitchState(Type nextStateType)
    {
        currentState = states[nextStateType];
        OnStateChanged?.Invoke(currentState);
    }
}