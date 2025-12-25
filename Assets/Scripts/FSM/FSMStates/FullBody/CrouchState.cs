using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CrouchState", menuName = "FSM/States/CrouchState")]
public class CrouchState: State
{
    private void OnEnable()
    {
        SwitchStateConditions = new List<SwitchStateCondition<IStateMachine>>()
        {
            new(c => (c.Character.InputHandler.GetRunInput() &&
                      Mathf.Approximately(c.StatesTransition.CurrentMovementSpeed, MovementSpeed)), c => StateType.Run),
            new(c => (!c.Character.InputHandler.GetCrouchInput()), c => c.PreviousState.StateType),
            new(c => (c.Character.InputHandler.GetDrawInput() && c.SetType == SetType.UpperBody 
                                                              && !c.Character.Inventory.IsWeaponDrawState), c => StateType.Draw),
        };
    }
}
