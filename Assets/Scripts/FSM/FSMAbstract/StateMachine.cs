using System;
using UnityEngine;

public interface IStateMachine
{
    public PlayablesAnimatorController AnimatorController { get; set; }
    public StatesContainer StatesContainer { get; set; }
    public StatesTransition StatesTransition { get; set; }
    public StatesTimer StatesTimer { get; set; }
    public InputHandler InputHandler { get; set; }
    public State CurrentState { get; set; }
    public State PreviousState { get; set; }
    
    public void SwitchState(StateType stateType);
}

public abstract class FSMAbstract : IStateMachine
{
    public PlayablesAnimatorController AnimatorController { get; set; }
    public StatesContainer StatesContainer { get; set; }
    public StatesTransition StatesTransition { get; set; }
    public StatesTimer StatesTimer { get; set; }
    public InputHandler InputHandler { get; set; }
    public State CurrentState { get; set; }
    public State PreviousState { get; set; }
    public Action <State, State> OnStateChanged { get; set; }

    public void SwitchState(StateType stateType)
    {
        var newState = StatesContainer.GetStateByStateType(stateType);
        
        CurrentState?.OnExit(this);
        PreviousState = CurrentState;
        CurrentState = newState;
        CurrentState?.OnEnter(this);
        OnStateChanged?.Invoke(PreviousState, CurrentState);
    }

    public virtual void Update()
    {
        CurrentState?.OnUpdate(this);
        StatesTransition.UpdateBlending();
        AnimatorController.OnUpdate(Time.deltaTime, InputHandler.GetMoveInput().x, InputHandler.GetMoveInput().y);
    }

    public virtual void FixedUpdate()
    {
        CurrentState?.OnFixedUpdate(this);
    }

    public virtual void LateUpdate()
    {
        CurrentState?.OnLateUpdate(this);
    }

    public virtual void OnDestroy()
    {
        AnimatorController?.Dispose();
    }
}
