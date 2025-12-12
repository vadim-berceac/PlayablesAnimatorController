using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Attack0State", menuName = "FSM/UpperBodyStates/Attack0State")]
public class Attack0State : State
{
    private void OnEnable()
    {
        SwitchStateConditions = new List<SwitchStateCondition<IStateMachine>>()
        {
            new(c => (c.StatesTimer.IsFinished), c => StateType.UpperCombatIdle),
        };
    }
}
