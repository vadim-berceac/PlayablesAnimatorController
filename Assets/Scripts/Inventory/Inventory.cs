using UnityEngine;

public class Inventory : MonoBehaviour
{
    [field: SerializeField] public InventorySettings InventorySettings { get; set; }
    [field: SerializeField] private WeaponData PrimaryWeapon { get; set; }
    public bool IsWeaponDraw { get; private set; }
    
    public RuntimeWeapon PrimaRuntimeWeapon { get; private set; }

    public void Awake()
    {
        PrimaRuntimeWeapon = (RuntimeWeapon) RuntimeItem.Create(PrimaryWeapon);
        PrimaRuntimeWeapon.Equip(InventorySettings.Animator);
        PrimaRuntimeWeapon.Draw(InventorySettings.Animator);
    }

    public int GetWeaponInHandsAnimationIndex()
    {
        return ((WeaponData)PrimaRuntimeWeapon.ItemData).AnimationSetIndex;
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
