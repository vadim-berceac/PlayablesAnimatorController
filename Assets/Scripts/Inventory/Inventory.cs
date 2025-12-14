using UnityEngine;

public class Inventory : MonoBehaviour
{
    [field: SerializeField] public InventorySettings InventorySettings { get; set; }
    [field: SerializeField, Range(0, 6)] private int weaponInHandsAnimationIndex;
  
    public bool IsWeaponDraw { get; private set; }

    public int GetWeaponInHandsAnimationIndex()
    {
       return weaponInHandsAnimationIndex;
    }

    public void SetWeaponDraw(bool value)
    {
       IsWeaponDraw = value;
    }
}

[System.Serializable]
public struct InventorySettings
{
    [field: SerializeField] public Animator Animator { get; private set; }
}
