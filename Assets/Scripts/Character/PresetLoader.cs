using UnityEngine;

public class PresetLoader : MonoBehaviour
{
   [field: SerializeField] public CharacterData CharacterData { get; set; }

   private void Awake()
   {
      SetName();
   }

   private void SetName()
   {
      gameObject.name = CharacterData.Name;
   }
}
