using UnityEngine;

[CreateAssetMenu(fileName = "RunState", menuName = "FSM/States/RunState")]
public class RunState : State
{
    public override void CheckSwitchConditions(IStateMachine stateMachine)
    {
        base.CheckSwitchConditions(stateMachine);
        if (!stateMachine.InputHandler.GetRunInput())
        {
            stateMachine.SwitchState(StateType.Walk);
        }
        
        if (Mathf.Approximately(stateMachine.StatesTransition.CurrentMovementSpeed, MovementSpeed) &&
            stateMachine.StatesTimer.IsFinished)
        {
            stateMachine.SwitchState(StateType.Sprint);
        }
        
        if (stateMachine.InputHandler.GetJumpInput())
        {
            stateMachine.SwitchState(StateType.Jump);
        }
    }
}
