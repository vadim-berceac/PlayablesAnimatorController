using UnityEngine;

[CreateAssetMenu(fileName = "IdleState", menuName = "FSM/States/IdleState")]
public class IdleState : State
{
    public override void CheckSwitchConditions(IStateMachine stateMachine)
    {
        base.CheckSwitchConditions(stateMachine);
        if (stateMachine.InputHandler.GetMoveInput().magnitude > 0.05f)
        {
            stateMachine.SwitchState(StateType.Walk);
        }
        if (stateMachine.InputHandler.GetJumpInput())
        {
            stateMachine.SwitchState(StateType.Jump);
        }
    }
}
