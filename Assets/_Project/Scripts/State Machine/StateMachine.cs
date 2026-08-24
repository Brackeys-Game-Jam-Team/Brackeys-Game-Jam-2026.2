using System;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : IState
{
    private readonly Dictionary<Type, IState> states = new();
    private IState defaultState;

    public object Context { get; private set; }
    public IState CurrentState { get; private set; }
    //public Type CurrentStateType { get; private set; }
    //public Type PreviousStateType { get; private set; }

    public StateMachine(object context)
    {
        Context = context;
    }

    #region IState Implementation
    public virtual void Enter()
    {
        if (defaultState != null)
        {
            CurrentState = defaultState;
            //CurrentStateType = defaultState.GetType();
            CurrentState.Enter();
        }
    }

    public virtual void Exit()
    {
        CurrentState?.Exit();
        CurrentState = null;
        //CurrentStateType = null;
    }

    public virtual void Update(float deltaTime)
    {
        CurrentState?.Update(deltaTime);
    }

    public virtual void FixedUpdate(float deltaTime)
    {
        CurrentState?.FixedUpdate(deltaTime);
    }

    public virtual void LateUpdate(float deltaTime)
    {
        CurrentState?.LateUpdate(deltaTime);
    }
    #endregion

    #region State Management
    public void AddState(IState state)
    {
        var type = state.GetType();

        if (!states.ContainsKey(type))
            states[type] = state;

        else
            Debug.LogWarning($"State {type.Name} is already registered.");
    }

    public void SetDefaultState<T>() where T : IState
    {
        if (states.TryGetValue(typeof(T), out var state))
            defaultState = state;
    }

    public void ChangeState<T>() where T : IState
    {
        ChangeState(typeof(T));
    }

    public void ChangeState(Type type)
    {
        if (!states.TryGetValue(type, out var state))
        {
            foreach (var pair in states)
            {
                if (type.IsAssignableFrom(pair.Key))
                {
                    state = pair.Value;
                    break;
                }
            }
        }

        if (state == null)
        {
            Debug.LogError($"State {type.Name} not found in machine.");
            return;
        }

        if (CurrentState == state)
            return;

        CurrentState?.Exit();
        //PreviousStateType = CurrentStateType;
        //CurrentStateType = type;
        CurrentState = state;
        CurrentState.Enter();
        Debug.Log($"[HSM] Transitioned to: {type.Name}");
    }

    public T GetState<T>() where T : IState
    {
        return states.TryGetValue(typeof(T), out var state) ? (T)state : default;
    }

    public bool IsInState<T>() where T : IState
    {
        return CurrentState.GetType() == typeof(T);
        //return CurrentStateType == typeof(T);
    }
    #endregion
}