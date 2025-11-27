using System;

public readonly struct SwitchStateCondition<T> where T : IStateMachine
{
    private readonly Func<T, bool> _condition;
    private readonly Func<T, StateType> _stateSelector;

    public SwitchStateCondition(Func<T, bool> condition, Func<T, StateType> stateSelector)
    {
        _condition = condition;
        _stateSelector = stateSelector;
    }
    
    public bool Check(T value, out StateType targetState)
    {
        var animatorController = value.AnimatorController;
        if (_condition(value) && !animatorController.IsCrossFading)
        {
            targetState = _stateSelector(value);
            return true;
        }
        targetState = default;
        return false;
    }
}