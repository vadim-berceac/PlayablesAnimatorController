using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public static class AnimationMixerExtensions
{
    public static void ResetInputs(this AnimationMixerPlayable mixer, double time)
    {
        for (var i = 0; i < mixer.GetInputCount(); i++)
        {
            var input = mixer.GetInput(i);
            if (!input.IsValid())
            {
                continue;
            }
            input.SetTime(time);
            input.SetSpeed(1);
        }
    }
    
    public static void NormalizeMixerWeights(this AnimationMixerPlayable mixer)
    {
        if (!mixer.IsValid()) return;
        var sum = 0f;
        var count = mixer.GetInputCount();
        for (var i = 0; i < count; i++)
        {
            sum += mixer.GetInputWeight(i);
        }
        if (Mathf.Approximately(sum, 0f)) return; 
        if (sum <= 1f + Mathf.Epsilon)
        {
            return;
        }
        for (var i = 0; i < count; i++)
        {
            mixer.SetInputWeight(i, mixer.GetInputWeight(i) / sum);
        }
    }
    
    public static void DestroyMixerAndInputs(this AnimationMixerPlayable mixer)
    {
        if (!mixer.IsValid()) return;
        for (var i = 0; i < mixer.GetInputCount(); i++)
        {
            var input = mixer.GetInput(i);
            if (input.IsValid())
                input.Destroy();
        }
        mixer.Destroy();
    }
    
    public static double GetTransitionTime(this AnimationMixerPlayable mixer, AnimationMixerPlayable previousMixer)
    {
        var clipPlayable = mixer.GetInput(0);
        if (clipPlayable.IsValid())
        {
            var animationClip = ((AnimationClipPlayable)clipPlayable).GetAnimationClip();
            if (animationClip != null && !animationClip.isLooping)
            {
                return 0.0;
            }
        }
        return previousMixer.IsValid() ? previousMixer.GetTime() : 0.0;
    }
    
    public static void SetWeights(this AnimationMixerPlayable mixer, bool normalizeEach, params (int port, float weight)[] weights)
    {
        foreach (var (port, weight) in weights)
        {
            mixer.SetInputWeight(port, weight);
            if (normalizeEach)
            {
                mixer.NormalizeMixerWeights();
            }
        }

        if (!normalizeEach)
        {
            mixer.NormalizeMixerWeights();
        }
    }
}
