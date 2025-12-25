using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IdleState", menuName = "FSM/States/IdleState")]
public class IdleState : State
{
    private void OnEnable()
    {
        SwitchStateConditions = new List<SwitchStateCondition<IStateMachine>>()
        {
            new(c => (c.Character.InputHandler.GetMoveInput().magnitude > 0.05f), c => StateType.Walk),
            new(c => (c.Character.InputHandler.GetCrouchInput()), c => StateType.Crouch),
            new(c => (c.Character.InputHandler.GetDrawInput() && c.Character.Inventory.GetWeaponInHandsAnimationIndex() > 0 && c.SetType == SetType.UpperBody 
                                                    && !c.Character.Inventory.IsWeaponDrawState), c => StateType.Draw),
            new(c => (c.Character.InputHandler.GetJumpInput()), c => StateType.Jump),
        };
    }
}
