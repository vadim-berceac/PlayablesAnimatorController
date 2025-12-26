using System;
using UnityEngine;

public interface IStateMachine
{
    public Character Character { get; set;}
    public SetType SetType { get; set; }
    public GraphCore GraphCore { get; set; }
    public PlayablesAnimatorController AnimatorController { get; set; }
    public StatesContainer StatesContainer { get; set; }
    public StatesTransition StatesTransition { get; set; }
    public StatesTimer StatesTimer { get; set; }
    public State CurrentState { get; set; }
    public State PreviousState { get; set; }
    
    public void SwitchState(StateType stateType);
    public void SetStatesTransition(StatesTransition transition);
    public bool IsWaitingForCrossFade();
    public void SetWaitingForCrossFade(bool waiting);
}

public abstract class FSMAbstract : IStateMachine
{
    public Character Character { get; set;}
    public SetType SetType { get; set; }
    public GraphCore GraphCore { get; set; }
    public PlayablesAnimatorController AnimatorController { get; set; }
    public StatesContainer StatesContainer { get; set; }
    public StatesTransition StatesTransition { get; set; }
    public StatesTimer StatesTimer { get; set; }
    public State CurrentState { get; set; }
    public State PreviousState { get; set; }
    public Action <State, State> OnStateChanged { get; set; }
    
    protected bool WaitingForCrossFade;

    public void SetStatesTransition(StatesTransition transition)
    {
        StatesTransition = transition;
    }

    public void SwitchState(StateType stateType)
    {
        var newState = StatesContainer.GetStateByStateType(SetType, stateType);
        CurrentState?.OnExit(this);
        PreviousState = CurrentState;
        CurrentState = newState;
        CurrentState?.OnEnter(this);
        OnStateChanged?.Invoke(PreviousState, CurrentState);
    }

    public virtual void Update()
    {
        CurrentState?.OnUpdate(this);
        AnimatorController.OnUpdate(Time.deltaTime, Character.InputHandler.GetMoveInput().x, Character.InputHandler.GetMoveInput().y);
    }

    public virtual void FixedUpdate()
    {
        CurrentState?.OnFixedUpdate(this);
    }

    public virtual void LateUpdate()
    {
        CurrentState?.OnLateUpdate(this);
    }

    public bool IsWaitingForCrossFade() 
    {
        return WaitingForCrossFade;
    }

    public void SetWaitingForCrossFade(bool waiting) 
    {
        WaitingForCrossFade = waiting;
    }
}
