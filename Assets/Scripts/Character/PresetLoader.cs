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

      // Настройки ретаргетинга
      var options = new AdvancedSkinRetargeting.RetargetingOptions
      {
         useNameMatching = true,           // Точное совпадение имен
         useFuzzyMatching = true,          // Нечеткое совпадение (для Mixamo разных версий)
         useHierarchyMatching = true,      // Маппинг по иерархии
         allowBindposeRecalculation = true, // Пересчет bindpose при необходимости
         updateWhenOffscreen = false,
         logUnmappedBones = true           // Логировать проблемы
      };

      PresetLoaderSettings.SkeletonRenderer.ApplyNewSkinWithRetargeting(
         newSkinMesh,
         PresetLoaderSettings.Animator,
         prefabAnimator,
         options
      );
   }

}

[System.Serializable]
public struct PresetLoaderSettings
{
   [field: SerializeField] public SkinnedMeshRenderer SkeletonRenderer { get; set; }
   [field: SerializeField] public Animator Animator { get; set; }
}
