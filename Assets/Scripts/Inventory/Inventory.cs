using UnityEngine;

[RequireComponent(typeof(Animator), typeof(PresetLoader))]
public class Inventory : MonoBehaviour
{
    [field: SerializeField] public InventorySettings InventorySettings { get; set; }
    public bool IsWeaponDrawState { get; private set; }
    
    public RuntimeWeapon PrimaRuntimeWeapon { get; private set; }

    public void Awake()
    {
        PrimaRuntimeWeapon = (RuntimeWeapon) RuntimeItem.Create(InventorySettings.Loader.CharacterData.PrimaryWeapon);
        PrimaRuntimeWeapon.Equip(InventorySettings.Animator);
    }

    public int GetWeaponInHandsAnimationIndex()
    {
        return (int)((WeaponData)PrimaRuntimeWeapon.ItemData).AnimationSet;
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
    [field: SerializeField] public PresetLoader Loader { get; set; }
    [field: SerializeField] public Animator Animator { get; private set; }
}
