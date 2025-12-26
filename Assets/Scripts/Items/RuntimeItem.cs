using System.Collections.Generic;
using UnityEngine;


public class RuntimeItem
{
    public ItemData ItemData { get; private set; }

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
        if (ItemData.VisibleModels.Length == 0)
        {
            Debug.LogWarning($"No part to equip in {ItemData.name}");
            return;
        }
        foreach (var model in ItemData.VisibleModels)
        {
            var part = Object.Instantiate(model.WearablePrefab);
            Parts.Add(part.transform);
            animator.AttachToBone(part.transform, model.BonePosition.HumanBodyBone, model.BonePosition.Position,
                model.BonePosition.Rotation, model.BonePosition.Scale, model.BonePosition.Enabled);
        }
    }

    public void UnEquip()
    {
        if (Parts.Count == 0)
        {
            Debug.LogWarning($"No part to equip in {ItemData.name}");
            return;
        }
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
    private BonePosition _previousBonePosition;
    private Transform _part;
    
    public bool IsDraw { get; private set; }

    public RuntimeWeapon(WeaponData data) : base(data) { }

    public void Draw(Animator animator)
    {
        if (Parts.Count == 0)
        {
            Debug.LogWarning($"No part to equip in {ItemData.name}");
            return;
        }

        _previousBonePosition = ItemData.VisibleModels[ItemData.HandSetups[0].WearableModelIndex].BonePosition;
        
        _part = Parts[ItemData.HandSetups[0].WearableModelIndex];
        var data = ItemData.HandSetups[0].BonePosition;
        animator.AttachToBone(_part, data.HumanBodyBone, data.Position, data.Rotation, data.Scale, data.Enabled);
        IsDraw = true;
    }

    public void UnDraw(Animator animator)
    {
        if (Parts.Count == 0)
        {
            Debug.LogWarning($"No part to equip in {ItemData.name}");
            return;
        }
        animator.AttachToBone(_part, _previousBonePosition.HumanBodyBone, 
            _previousBonePosition.Position, _previousBonePosition.Rotation, _previousBonePosition.Scale, _previousBonePosition.Enabled);
        IsDraw = false;
    }
}