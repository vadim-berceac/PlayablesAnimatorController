using UnityEngine;

[CreateAssetMenu(fileName = "WearableItemData", menuName = "ScriptableObjects/Items/WearableItemData")]
public class WearableData : ItemData, IWearableData
{
    [field: SerializeField] public WearableModel[] VisibleModels { get; set; }
}