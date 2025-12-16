using System.Collections.Generic;
using UnityEngine;

public interface IItemData
{
    public string Name { get; set;}
    public Sprite Icon { get; set;}
    public int Price { get; set; }
    public GameObject GroundPrefab { get; set; }
}

public interface IWearableData
{
    public WearableModel[] VisibleModels { get; set; }
}

public interface IWeaponData
{
    public float Damage { get; set; }
    public float Range { get; set; }
    public BonePosition HandPosition { get; set; }
}

[CreateAssetMenu(fileName = "ItemData", menuName = "ScriptableObjects/Items/ItemData")]
public class ItemData : ScriptableObject, IItemData
{
    [field: SerializeField] public string Name { get; set;}
    [field: SerializeField] public Sprite Icon { get; set;}
    [field: SerializeField] public int Price { get; set; }
    [field: SerializeField] public GameObject GroundPrefab { get; set; }
}

[CreateAssetMenu(fileName = "WearableItemData", menuName = "ScriptableObjects/Items/WearableItemData")]
public class WearableData : ItemData, IWearableData
{
    [field: SerializeField] public WearableModel[] VisibleModels { get; set; }
}

[CreateAssetMenu(fileName = "WeaponItemData", menuName = "ScriptableObjects/Items/WeaponItemData")]
public class WeaponData : WearableData, IWeaponData
{
    [field: SerializeField] public float Damage { get; set; }
    [field: SerializeField] public float Range { get; set; }
    [field: SerializeField] public BonePosition HandPosition { get; set; }
}
