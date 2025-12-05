using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "JumpState", menuName = "FSM/States/JumpState")]
public class JumpState : State
{
    private void OnEnable()
    {
        SwitchStateConditions = new List<SwitchStateCondition<IStateMachine>>()
        {
            new(c => (c.StatesTimer.IsFinished), c => c.PreviousState.StateType),
        };
    }
}
