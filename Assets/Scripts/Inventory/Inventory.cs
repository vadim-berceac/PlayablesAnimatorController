using UnityEngine;

public class Inventory : MonoBehaviour
{
  [SerializeField] [Range(0,5)] private int weaponInHandsAnimationIndex;
  
  public bool IsWeaponDraw { get; private set; }

  public int GetWeaponInHandsAnimationIndex()
  {
     return weaponInHandsAnimationIndex;
  }

  public void SetWeaponDraw(bool value)
  {
      IsWeaponDraw = value;
  }
}
