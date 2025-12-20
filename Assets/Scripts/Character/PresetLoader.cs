using UnityEngine;

public class PresetLoader : MonoBehaviour
{
   [field: SerializeField] public CharacterData CharacterData { get; set; }
   [field: SerializeField] public PresetLoaderSettings PresetLoaderSettings { get; set; }

   private void Awake()
   {
      SetName();
      SetAppearance();
   }

   private void SetName()
   {
      gameObject.name = CharacterData.Name;
   }

   private void SetAppearance()
   {
      if (CharacterData.Skin == null || CharacterData.Skin.SkinPrefab == null)
         return;

      var newSkinMesh = CharacterData.Skin.SkinPrefab.GetComponentInChildren<SkinnedMeshRenderer>();
      var prefabAnimator = CharacterData.Skin.SkinPrefab.GetComponent<Animator>();

      PresetLoaderSettings.SkeletonRenderer.ApplyNewSkinWithRebind(
         newSkinMesh,
         PresetLoaderSettings.Animator,   // текущий персонаж
         prefabAnimator                   // аниматор из префаба
      );
   }

}

[System.Serializable]
public struct PresetLoaderSettings
{
   [field: SerializeField] public SkinnedMeshRenderer SkeletonRenderer { get; set; }
   [field: SerializeField] public Animator Animator { get; set; }
}
