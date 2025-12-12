using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnDrawState", menuName = "FSM/UpperBodyStates/UnDrawState")]
public class UnDrawState: State
{
    private void OnEnable()
    {
        SwitchStateConditions = new List<SwitchStateCondition<IStateMachine>>()
        {
            new(c => (c.StatesTimer.IsFinished), c => c.Character.FullBodyFsm.CurrentState.StateType),
        };
    }

    public override void OnExit(IStateMachine stateMachine)
    {
        base.OnExit(stateMachine);
        stateMachine.Character.Inventory.SetWeaponDraw(false);
    }
}
