using UnityEngine;

[CreateAssetMenu(fileName = "WalkState", menuName = "FSM/States/WalkState")]
public class WalkState : State
{
    public override void CheckSwitchConditions(IStateMachine stateMachine)
    {
        base.CheckSwitchConditions(stateMachine);
        if (stateMachine.InputHandler.GetRunInput() &&
            Mathf.Approximately(stateMachine.StatesTransition.CurrentMovementSpeed, MovementSpeed))
        {
            stateMachine.SwitchState(StateType.Run);
        }
        
        if (Mathf.Approximately(stateMachine.InputHandler.GetMoveInput().magnitude, 0))
        {
            stateMachine.SwitchState(StateType.Idle);
        }
        
        if (stateMachine.InputHandler.GetJumpInput())
        {
            stateMachine.SwitchState(StateType.Jump);
        }
    }
}
