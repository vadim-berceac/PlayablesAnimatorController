using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RunState", menuName = "FSM/States/RunState")]
public class RunState : State
{
    private void OnEnable()
    {
        SwitchStateConditions = new List<SwitchStateCondition<IStateMachine>>()
        {
            new(c => (!c.InputHandler.GetRunInput()), c => StateType.Walk),
            new(c => (Mathf.Approximately(c.StatesTransition.CurrentMovementSpeed, MovementSpeed) &&
                      c.StatesTimer.IsFinished), c => StateType.Sprint),
            new(c => (c.InputHandler.GetJumpInput()), c => StateType.Jump)
        };
    }
}
