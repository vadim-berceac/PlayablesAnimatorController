using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RunState", menuName = "FSM/States/RunState")]
public class RunState : State
{
    private void OnEnable()
    {
        SwitchStateConditions = new List<SwitchStateCondition<IStateMachine>>()
        {
            new(c => (!c.Character.InputHandler.GetRunInput()), c => StateType.Walk),
            new(c => (Mathf.Approximately(c.StatesTransition.CurrentMovementSpeed, MovementSpeed) &&
                      c.StatesTimer.IsFinished) && c.Character.InputHandler.GetMoveInput().y > 0, c => StateType.Sprint),
            new(c => (c.Character.InputHandler.GetJumpInput()), c => StateType.Jump),
            new(c => (c.Character.InputHandler.GetDrawInput() && c.Character.Inventory.GetWeaponInHandsAnimationIndex(0) > 0 && c.SetType == SetType.UpperBody 
                      && !c.Character.Inventory.IsWeaponDrawState), c => StateType.Draw),
            new(c => (c.Character.InputHandler.GetDrawInput() && c.SetType == SetType.UpperBody 
                                                              && c.Character.Inventory.IsWeaponDrawState), c => StateType.UnDraw),
        };
    }
}
