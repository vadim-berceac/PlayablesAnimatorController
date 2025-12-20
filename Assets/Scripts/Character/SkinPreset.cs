using UnityEngine;

[CreateAssetMenu(fileName = "SkinPreset", menuName = "ScriptableObjects/SkinPreset")]
public class SkinPreset : ScriptableObject
{
    [field: SerializeField] public GameObject SkinPrefab { get; set; }
}
