using UnityEngine;

[System.Serializable]
public struct BonePosition
{
    public enum AttachPoint
    {
        Left,
        Right,
        Front,
        Back,
        All
    }
    
    [field: SerializeField] public HumanBodyBones HumanBodyBone { get; set; }
    [field: SerializeField] public AttachPoint OccupiedPoint { get; set; }
    [field: SerializeField] public Vector3 Position { get; set; }
    [field: SerializeField] public Vector3 Rotation { get; set; }
    [field: SerializeField] public float Scale { get; set; }
}

[System.Serializable]
public struct WearableModel
{
    [field: SerializeField] public GameObject WearablePrefab { get; set; }
    [field: SerializeField] public BonePosition BonePosition { get; set; }
}
