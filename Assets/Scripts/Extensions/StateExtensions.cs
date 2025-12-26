using System.Collections.Generic;
using UnityEngine;

public static class StateExtensions
{
    public static void SwitchAvatarMask(this State state, IStateMachine stateMachine, bool linkToWeaponIndex, 
        ClipBlendDataCollection[] clipBlendDataCollections, int handSetupIndex)
    {
        if (stateMachine.SetType != SetType.UpperBody || !linkToWeaponIndex ||stateMachine.Character.Inventory.
                GetWeaponInHandsAnimationIndex(handSetupIndex) == 0)
        {
            return;
        }
        var clipBlendData = clipBlendDataCollections[stateMachine.Character.Inventory.GetWeaponInHandsAnimationIndex(handSetupIndex)].
            ClipsBlendData[0];
       
        if (clipBlendData.LayersConfigs == null || clipBlendData.LayersConfigs.Length == 0)
        {
            var defaultConfigs = new List<(int graphPortIndex, AvatarMask mask, bool isAdditive)>
            {
                (1,stateMachine.Character.AvatarMasksContainer.GetMask(AvatarMaskType.UpperBody), false),
                (2,stateMachine.Character.AvatarMasksContainer.GetMask(AvatarMaskType.BothHands), false)
            };
            stateMachine.Character.UpperBodyFsm.SetAvatarMask(defaultConfigs);
            return;
        }

        var upperSmConfigs = new List<(int graphPortIndex, AvatarMask mask, bool isAdditive)> { };
        upperSmConfigs.Clear();
        
        foreach (var layerConfigs in clipBlendData.LayersConfigs)
        {
            upperSmConfigs.Add((layerConfigs.GraphPortIndex,
                stateMachine.Character.AvatarMasksContainer.GetMask(layerConfigs.MaskType),
                layerConfigs.IsAdditive));
        }
        stateMachine.Character.UpperBodyFsm.SetAvatarMask(upperSmConfigs);
    }
    
    public static void CheckDuplicatingState(this State state, IStateMachine stateMachine)
    {
        if (stateMachine.SetType != SetType.UpperBody)
            return;

        var character = stateMachine.Character;
        if (character.FullBodyFsm == null || character.UpperBodyFsm == null)
            return;

        var statesMatch = character.FullBodyFsm.CurrentState?.StateType == 
                          character.UpperBodyFsm.CurrentState?.StateType;

        var targetWeight = statesMatch ? 0f : 1f;

        stateMachine.GraphCore.SetLayerWeight(1, targetWeight);
        stateMachine.GraphCore.SetLayerWeight(2, targetWeight);

        //Debug.Log($"Duplicating check: states match = {statesMatch}, setting upper layers weight to {targetWeight}");
    }
}
