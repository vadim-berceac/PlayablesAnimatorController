using System.Collections.Generic;
using UnityEngine;


public class RuntimeItem
{
    protected ItemData ItemData { get; private set; }

    protected RuntimeItem(ItemData itemData)
    {
        ItemData = itemData;
    }

    public static RuntimeItem Create(ItemData itemData)
    {
        return itemData switch
        {
            WeaponData weaponData => new RuntimeWeapon(weaponData),
            WearableData wearableData => new RuntimeWearable(wearableData),
            _ => new RuntimeItem(itemData)
        };
    }
}


public class RuntimeWearable : RuntimeItem
{
    protected List<Transform> Parts { get; set; } = new();
    protected new WearableData ItemData => (WearableData)base.ItemData;

    public RuntimeWearable(WearableData data) : base(data) { }

    public void Equip(Animator animator)
    {
        foreach (var model in ItemData.VisibleModels)
        {
            var part = Object.Instantiate(model.WearablePrefab);
            Parts.Add(part.transform);
            animator.AttachToBone(part.transform, model.BonePosition.HumanBodyBone, model.BonePosition.Position,
                model.BonePosition.Rotation, model.BonePosition.Scale);
        }
    }

    public void UnEquip()
    {
        foreach (var part in Parts)
        {
            Object.Destroy(part);
        }
        Parts.Clear();
    }
}


public class RuntimeWeapon : RuntimeWearable
{
    protected new WeaponData ItemData => (WeaponData)base.ItemData;

    public RuntimeWeapon(WeaponData data) : base(data) { }

    public void Draw(int partIndex)
    {
       
    }

    public void UnDraw()
    {
        
    }
}