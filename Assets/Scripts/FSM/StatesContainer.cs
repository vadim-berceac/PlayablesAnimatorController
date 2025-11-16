using UnityEngine;
using System.Linq;

public class StatesContainer : MonoBehaviour
{
    [SerializeField] private State[] states;

    private void Awake()
    {
        if (!CheckDuplicates())
        {
            return;
        }
        Debug.LogError("Duplicate states found!");
    }

    private bool CheckDuplicates()
    {
        return states.GroupBy(x => x.StateType).Any(g => g.Count() > 1);
    }

    public State GetStateByStateType(StateType stateType)
    {
        return states.FirstOrDefault(x => x.StateType == stateType);
    }
}
