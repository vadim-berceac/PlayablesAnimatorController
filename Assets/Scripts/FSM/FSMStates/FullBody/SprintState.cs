using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SprintState", menuName = "FSM/States/SprintState")]
public class SprintState : State
{
    private void OnEnable()
    {
        SwitchStateConditions = new List<SwitchStateCondition<IStateMachine>>()
        {
            new(c => (c.StatesTimer.IsFinished || !c.Character.InputHandler.GetRunInput() 
                                               || c.Character.InputHandler.GetMoveInput().y <= 0), c => StateType.Run),
            new(c => (c.Character.InputHandler.GetJumpInput()), c => StateType.Jump),
            new(c => (c.Character.InputHandler.GetDrawInput() && c.Character.Inventory.GetWeaponInHandsAnimationIndex() > 0 && c.SetType == SetType.UpperBody 
                      && !c.Character.Inventory.IsWeaponDrawState), c => StateType.Draw),
            new(c => (c.Character.InputHandler.GetDrawInput() && c.SetType == SetType.UpperBody 
                                                              && c.Character.Inventory.IsWeaponDrawState), c => StateType.UnDraw),
        };
    }
}
