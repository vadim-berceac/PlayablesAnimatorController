using UnityEngine;

[CreateAssetMenu(fileName = "WeaponItemData", menuName = "ScriptableObjects/Items/WeaponItemData")]
public class WeaponData : WearableData, IWeaponData
{
    [field: SerializeField] public float Damage { get; set; }
    [field: SerializeField] public float Range { get; set; }
    [field: SerializeField, Range(0,6)] public int AnimationSetIndex { get; set; }
    [field: SerializeField] public BonePosition HandPosition { get; set; }
    [field: SerializeField] public int ModelIndex { get; set; }
}