using UnityEngine;

[CreateAssetMenu(fileName = "WeaponItemData", menuName = "ScriptableObjects/Items/WeaponItemData")]
public class WeaponData : WearableData, IWeaponData
{
    [field: SerializeField] public float Damage { get; set; }
    [field: SerializeField] public float Range { get; set; }
    [field: SerializeField] public AnimationSet AnimationSet { get; set; }
    [field: SerializeField] public WeaponRange RangeType { get; set; }
    [field: SerializeField] public HandPosition HandSetup { get; set; }
}