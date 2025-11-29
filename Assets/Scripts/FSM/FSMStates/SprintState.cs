using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SprintState", menuName = "FSM/States/SprintState")]
public class SprintState : State
{
    private void OnEnable()
    {
        SwitchStateConditions = new List<SwitchStateCondition<IStateMachine>>()
        {
            new(c => (c.StatesTimer.IsFinished || !c.InputHandler.GetRunInput() 
                                               || c.InputHandler.GetMoveInput().y <= 0), c => StateType.Run),
            new(c => (c.InputHandler.GetJumpInput()), c => StateType.Jump)
        };
    }
}
