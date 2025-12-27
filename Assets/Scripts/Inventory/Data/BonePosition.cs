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
    [field: SerializeField] public bool Enabled { get; set; }
}

[System.Serializable]
public struct WearableModel
{
    [field: SerializeField] public GameObject WearablePrefab { get; set; }
    [field: SerializeField] public BonePosition BonePosition { get; set; }
}

[System.Serializable]
public struct HandPosition
{
    [field: SerializeField] public WeaponAttributes WeaponAttributes { get; set; }
    [field: SerializeField] public AnimationSet AnimationSet { get; set; }
    [field: SerializeField] public WeaponType Type { get; set; }
    [field: SerializeField] public int WearableModelIndex { get; set; }
    [field: SerializeField] public bool IK { get; set; }
    [field: SerializeField] public BonePosition BonePosition { get; set; }
}
