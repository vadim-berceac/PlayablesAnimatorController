using UnityEngine;

[CreateAssetMenu(fileName = "SprintState", menuName = "FSM/States/SprintState")]
public class SprintState : State
{
    public override void CheckSwitchConditions(IStateMachine stateMachine)
    {
        base.CheckSwitchConditions(stateMachine);
        if (stateMachine.StatesTimer.IsFinished || !stateMachine.InputHandler.GetRunInput())
        {
            stateMachine.SwitchState(StateType.Run);
        }
        
        if (stateMachine.InputHandler.GetJumpInput())
        {
            stateMachine.SwitchState(StateType.Jump);
        }
    }
}
