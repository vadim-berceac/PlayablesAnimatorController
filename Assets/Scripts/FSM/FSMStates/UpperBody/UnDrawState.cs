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
        stateMachine.Character.Inventory.UnDrawPrimaryWeapon();
    }
    
    public override void OnUpdate(IStateMachine stateMachine)
    {
        base.OnUpdate(stateMachine);
        if (stateMachine.StatesTimer.IsEventTriggered)
        {
            stateMachine.Character.Inventory.UnDrawPrimaryWeapon();
        }
    }
}
