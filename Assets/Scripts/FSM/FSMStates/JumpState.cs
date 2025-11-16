using UnityEngine;

[CreateAssetMenu(fileName = "JumpState", menuName = "FSM/States/JumpState")]
public class JumpState : State
{
    public override void CheckSwitchConditions(IStateMachine stateMachine)
    {
        if (stateMachine.StatesTimer.IsFinished)
        {
            stateMachine.SwitchState(stateMachine.PreviousState.StateType);
        }
    }
}
