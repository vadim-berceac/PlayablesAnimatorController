using UnityEngine;

[CreateAssetMenu(fileName = "WeaponItemData", menuName = "ScriptableObjects/Items/WeaponItemData")]
public class WeaponData : WearableData, IWeaponData
{
    [field: SerializeField] public HandPosition[] HandSetups { get; set; }
}

[System.Serializable]
public struct WeaponAttributes
{
    [field: SerializeField] public float Damage { get; set; }
    [field: SerializeField] public float Range { get; set; }
    [field: SerializeField] public float Speed { get; set; }
}