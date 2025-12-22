using System.Linq;
using UnityEngine;

public class AvatarMasksContainer : MonoBehaviour
{
    [field: SerializeField] private AvatarMaskSetup[] AvatarMaskSetups { get; set; }

    public AvatarMask GetMask(AvatarMaskType avatarMaskType)
    {
        return AvatarMaskSetups.FirstOrDefault(m => m.MaskType == avatarMaskType).Mask;
    }
}

[System.Serializable]
public struct AvatarMaskSetup
{
    [field: SerializeField] public AvatarMaskType MaskType { get; set; }
    [field: SerializeField] public AvatarMask Mask { get; set; }
}

public enum AvatarMaskType
{
    None,
    FullBody,
    UpperBody,
    LowerBody,
    Torso,
    RightArm,
    LeftArm,
    Head,
    BothHands
}
