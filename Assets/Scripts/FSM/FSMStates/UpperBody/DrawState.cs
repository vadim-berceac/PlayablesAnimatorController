using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DrawState", menuName = "FSM/UpperBodyStates/Draw")]
public class DrawState: State
{
    private void OnEnable()
    {
        SwitchStateConditions = new List<SwitchStateCondition<IStateMachine>>()
        {
            new(c => (c.StatesTimer.IsFinished), c => c.PreviousState.StateType),
        };
    }

    public override void OnEnter(IStateMachine stateMachine)
    {
        stateMachine.Character.Inventory.SetWeaponDraw(true);
        base.OnEnter(stateMachine);
    }
}
