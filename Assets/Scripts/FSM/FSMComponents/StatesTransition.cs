using System;
using UnityEngine;

public class StatesTransition
{
    public float CurrentMovementSpeed { get; private set; }
    public float CurrentAnimationSpeed { get; private set; }
    
    public Action<float> OnMovementSpeedChanged { get; set; }
    public Action<float> OnAnimationSpeedChanged { get; set; }
    private readonly FSMAbstract _fsmAbstract;
    private State _targetState;
    private float _animationFactor;

    public StatesTransition(FSMAbstract fsmAbstract)
    {
        _fsmAbstract = fsmAbstract;
    }
    
    public void UpdateBlending()
    {
        switch (_fsmAbstract.CurrentState.SpeedMaxRule)
        {
            case SpeedMaxRule.Override: { _targetState = _fsmAbstract.CurrentState; } break;
            case SpeedMaxRule.UsePreviousStateValue: { _targetState = _fsmAbstract.PreviousState; }; break;
            default: { _targetState = _fsmAbstract.CurrentState; }; break;
        }
        
        CurrentMovementSpeed = Mathf.MoveTowards(
            CurrentMovementSpeed,
            _targetState.MovementSpeed,
            _targetState.EnterBlendSpeed * Time.deltaTime
        );

     
        if (_targetState.MovementSpeed == 0f)
        {
            _animationFactor = (CurrentMovementSpeed == 0f) ? 1f : CurrentMovementSpeed;
        }
        else
        {
            _animationFactor = CurrentMovementSpeed / _targetState.MovementSpeed;
        }

        CurrentAnimationSpeed = _animationFactor;

        OnMovementSpeedChanged?.Invoke(CurrentMovementSpeed);
        OnAnimationSpeedChanged?.Invoke(CurrentAnimationSpeed);
    }
}

public enum SpeedMaxRule
{
    Override,
    UsePreviousStateValue,
}