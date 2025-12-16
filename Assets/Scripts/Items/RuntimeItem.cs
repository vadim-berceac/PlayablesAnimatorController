using UnityEngine;


public class RuntimeItem<T> where T : ItemData
{
    public T ItemData { get; private set; }

    protected RuntimeItem(T itemData)
    {
        ItemData = itemData;
    }
    
    public static RuntimeItem<T> Create(T itemData)
    {
        if (itemData is WeaponData weaponData)
            return new RuntimeWeapon(weaponData) as RuntimeItem<T>;

        if (itemData is WearableData wearableData)
            return new RuntimeWearable(wearableData) as RuntimeItem<T>;

        return new RuntimeItem<T>(itemData);
    }
}


public class RuntimeWearable : RuntimeItem<WearableData>, IWearableRuntime
{
    public RuntimeWearable(WearableData data) : base(data) { }

    public void Equip(Animator animator)
    {
       
    }

    public void UnEquip()
    {
        
    }
}


public class RuntimeWeapon : RuntimeItem<WeaponData>, IWearableRuntime, IWeaponRuntime
{
    public RuntimeWeapon(WeaponData data) : base(data) { }

    public void Equip(Animator animator)
    {
        
    }

    public void UnEquip()
    {
        
    }

    public void Draw()
    {
       
    }

    public void UnDraw()
    {
        
    }
}