using UnityEngine;

[System.Serializable]
public struct ClipBlendData
{
    [field: SerializeField] public AnimationClip Clip { get; private set; }
    [field: SerializeField] public BlendParams BlendParams { get; private set; }
}

[System.Serializable]
public struct ClipBlendDataCollection
{
    [field: SerializeField] public AnimationSet AnimationSet { get; private set; }
    [field: SerializeField] public ClipBlendData[] ClipsBlendData { get; private set; }
}

[System.Serializable]
public struct BlendParams
{
    [field: SerializeField, Range (-1, 1)] public float Param1 { get; private set; }
    [field: SerializeField, Range (-1, 1)] public float Param2 { get; private set; }
}
