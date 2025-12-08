using UnityEngine;

public class Inventory : MonoBehaviour
{
  [SerializeField] [Range(0,5)] private int weaponInHandsAnimationIndex;

  public int GetWeaponInHandsAnimationIndex()
  {
     return weaponInHandsAnimationIndex;
  }
}
