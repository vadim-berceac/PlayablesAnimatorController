using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "ScriptableObjects/CharacterData")]
public class CharacterData : ScriptableObject
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public SkinPreset Skin { get; private set; }
    [field: SerializeField] public WeaponData PrimaryWeapon { get; set; }
}
