using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UpperCombatIdleState", menuName = "FSM/UpperBodyStates/UpperCombatIdleState")]
public class UpperCombatIdleState: State
{
    private void OnEnable()
    {
        SwitchStateConditions = new List<SwitchStateCondition<IStateMachine>>()
        {
            new(c => (c.InputHandler.GetDrawInput() && c.Character.Inventory.IsWeaponDrawState), c => StateType.UnDraw),
            new(c => (c.InputHandler.GetAttackInput() && c.Character.Inventory.IsWeaponDrawState), c => StateType.Attack0)
        };
    }
}
