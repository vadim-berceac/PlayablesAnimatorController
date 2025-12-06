using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UpperDefaultState", menuName = "FSM/UpperBodyStates/UpperDefaultState")]
public class UpperDefaultState: State
{
    private void OnEnable()
    {
        SwitchStateConditions = new List<SwitchStateCondition<IStateMachine>>()
        {
            new(c => (c.StatesTimer.IsFinished), c => c.PreviousState.StateType),
        };
    }
}
