using UnityEngine;
using System.Linq;

public class StatesContainer : MonoBehaviour
{
    [SerializeField] private StatesSet[] statesSet;

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
        return statesSet.Any(s => s.States.GroupBy(x => x.StateType).Any(g => g.Count() > 1));
    }

    public StateType GetStartStateType(SetType setType)
    {
        var set = statesSet.FirstOrDefault(s => s.SetType == setType);
        return set.States[set.StartStateIndex].StateType;
    }

    public State GetStateByStateType(SetType setType, StateType stateType)
    {
        return statesSet
            .Where(s => s.SetType == setType)
            .SelectMany(s => s.States)
            .FirstOrDefault(x => x.StateType == stateType);
    }

    public AvatarMask GetAvatarMaskBySetType(SetType setType)
    {
        return statesSet.FirstOrDefault(s => s.SetType == setType).AvatarMask;
    }
}

[System.Serializable]
public struct StatesSet
{
    [field: SerializeField] public SetType SetType { get; set; }
    [field: SerializeField] public int StartStateIndex { get; set; }
    [field: SerializeField] public AvatarMask AvatarMask { get; private set; }
    [field: SerializeField] public State[] States { get; set; }
}

public enum SetType
{
    None,
    FullBody,
    UpperBody
}