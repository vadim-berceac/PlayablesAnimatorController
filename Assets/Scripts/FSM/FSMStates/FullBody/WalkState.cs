using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WalkState", menuName = "FSM/States/WalkState")]
public class WalkState : State
{
    private void OnEnable()
    {
        SwitchStateConditions = new List<SwitchStateCondition<IStateMachine>>()
        {
            new(c => (c.InputHandler.GetRunInput() &&
                      Mathf.Approximately(c.StatesTransition.CurrentMovementSpeed, MovementSpeed)), c => StateType.Run),
            new(c => (Mathf.Approximately(c.InputHandler.GetMoveInput().magnitude, 0)), c => StateType.Idle),
            new(c => (c.InputHandler.GetCrouchInput()), c => StateType.Crouch),
            new(c => (c.InputHandler.GetJumpInput()), c => StateType.Jump),
            new(c => (c.InputHandler.GetDrawInput() && c.Character.Inventory.GetWeaponInHandsAnimationIndex() > 0 && c.SetType == SetType.UpperBody 
                      && !c.Character.Inventory.IsWeaponDrawState), c => StateType.Draw),
            new(c => (c.InputHandler.GetDrawInput() && c.SetType == SetType.UpperBody 
                                                    && c.Character.Inventory.IsWeaponDrawState), c => StateType.UnDraw),
        };
    }
}
