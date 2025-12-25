using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WalkState", menuName = "FSM/States/WalkState")]
public class WalkState : State
{
    private void OnEnable()
    {
        SwitchStateConditions = new List<SwitchStateCondition<IStateMachine>>()
        {
            new(c => (c.Character.InputHandler.GetRunInput() &&
                      Mathf.Approximately(c.StatesTransition.CurrentMovementSpeed, MovementSpeed)), c => StateType.Run),
            new(c => (Mathf.Approximately(c.Character.InputHandler.GetMoveInput().magnitude, 0)), c => StateType.Idle),
            new(c => (c.Character.InputHandler.GetCrouchInput()), c => StateType.Crouch),
            new(c => (c.Character.InputHandler.GetJumpInput()), c => StateType.Jump),
            new(c => (c.Character.InputHandler.GetDrawInput() && c.Character.Inventory.GetWeaponInHandsAnimationIndex() > 0 && c.SetType == SetType.UpperBody 
                      && !c.Character.Inventory.IsWeaponDrawState), c => StateType.Draw),
            new(c => (c.Character.InputHandler.GetDrawInput() && c.SetType == SetType.UpperBody 
                                                              && c.Character.Inventory.IsWeaponDrawState), c => StateType.UnDraw),
        };
    }
}
