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
    public HandPosition[] HandSetups { get; set; }
}

[CreateAssetMenu(fileName = "ItemData", menuName = "ScriptableObjects/Items/ItemData")]
public class ItemData : ScriptableObject, IItemData
{
    [field: SerializeField] public string Name { get; set;}
    [field: SerializeField] public Sprite Icon { get; set;}
    [field: SerializeField] public int Price { get; set; }
    [field: SerializeField] public GameObject GroundPrefab { get; set; }
}