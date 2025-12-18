using UnityEngine;

public class Inventory : MonoBehaviour
{
    [field: SerializeField] public InventorySettings InventorySettings { get; set; }
    [field: SerializeField] private WeaponData PrimaryWeapon { get; set; }
    public bool IsWeaponDrawState { get; private set; }
    
    public RuntimeWeapon PrimaRuntimeWeapon { get; private set; }

    public void Awake()
    {
        PrimaRuntimeWeapon = (RuntimeWeapon) RuntimeItem.Create(PrimaryWeapon);
        PrimaRuntimeWeapon.Equip(InventorySettings.Animator);
    }

    public int GetWeaponInHandsAnimationIndex()
    {
        return ((WeaponData)PrimaRuntimeWeapon.ItemData).AnimationSetIndex;
    }
    public void DrawPrimaryWeapon()
    {
        PrimaRuntimeWeapon?.Draw(InventorySettings.Animator);
        IsWeaponDrawState = true;
    }

    public void UnDrawPrimaryWeapon()
    {
        PrimaRuntimeWeapon?.UnDraw(InventorySettings.Animator);
        IsWeaponDrawState = false;
    }
}

[System.Serializable]
public struct InventorySettings
{
    [field: SerializeField] public Animator Animator { get; private set; }
}
