using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DrawState", menuName = "FSM/UpperBodyStates/Draw")]
public class DrawState: State
{
    private void OnEnable()
    {
        SwitchStateConditions = new List<SwitchStateCondition<IStateMachine>>()
        {
            new(c => (c.StatesTimer.IsFinished), c => StateType.UpperCombatIdle),
        };
    }


    public override void OnUpdate(IStateMachine stateMachine)
    {
        base.OnUpdate(stateMachine);
        if (stateMachine.StatesTimer.IsEventTriggered)
        {
            stateMachine.Character.Inventory.DrawPrimaryWeapon();
        }
    }
}
