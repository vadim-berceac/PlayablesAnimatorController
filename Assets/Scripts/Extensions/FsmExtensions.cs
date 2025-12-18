using System.Collections.Generic;
using UnityEngine.Animations;
using UnityEngine.Playables;

public static class FsmExtensions
{
    public static void SwitchAnimation(this IStateMachine stateMachine)
    {
        var collectionIndex = stateMachine.CurrentState.LinkToWeaponIndex 
            ? stateMachine.Character.Inventory.GetWeaponInHandsAnimationIndex() : 0;
        stateMachine.StatesTimer.Start(stateMachine.CurrentState, collectionIndex);
        stateMachine.StatesTimer.SetActionNormalizedTime(stateMachine.CurrentState.ActionTime);
        var clipBlendDataCollection = stateMachine.CurrentState.ClipBlendDataCollections[collectionIndex];
        if (clipBlendDataCollection.ClipsBlendData == null || clipBlendDataCollection.ClipsBlendData.Length == 0)
        {
            return;
        }
        stateMachine.AnimatorController.Play(GetAnimationMixerPlayable(stateMachine.AnimatorController.GraphCore.Graph,
                clipBlendDataCollection.ClipsBlendData),GetBlendParams(clipBlendDataCollection.ClipsBlendData)
            , stateMachine.PreviousState ? stateMachine.CurrentState.CrossFadeTime : 0);
    }
    
    private static AnimationMixerPlayable GetAnimationMixerPlayable(PlayableGraph graph, ClipBlendData[] clipBlendData, int activeClipIndex = 0)
    {
        var mixer = AnimationMixerPlayable.Create(graph, clipBlendData.Length);

        for (var i = 0; i < clipBlendData.Length; i++)
        {
            if (clipBlendData[i].Clip == null)
            {
                continue;
            }
            var clipPlayable = AnimationClipPlayable.Create(graph, clipBlendData[i].Clip);

            graph.Connect(clipPlayable, 0, mixer, i);
          
            mixer.SetInputWeight(i, i == activeClipIndex ? 1f : 0f);
        }

        return mixer;
    }

    private static List<BlendParams> GetBlendParams(ClipBlendData[] clipBlendData)
    {
        var blendParams = new List<BlendParams>();
        foreach (var b in clipBlendData)
        {
            blendParams.Add(b.BlendParams);
        }
        return blendParams;
    }
}
