using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IdleState", menuName = "FSM/States/IdleState")]
public class IdleState : State
{
    private void OnEnable()
    {
        SwitchStateConditions = new List<SwitchStateCondition<IStateMachine>>()
        {
            new(c => (c.InputHandler.GetMoveInput().magnitude > 0.05f), c => StateType.Walk),
            new(c => (c.InputHandler.GetJumpInput()), c => StateType.Jump),
        };
    }
}
