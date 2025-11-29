
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public class GraphCore
{
    public PlayableGraph Graph { get; private set; }
    public AnimationLayerMixerPlayable LayerMixer { get; private set; }

    public GraphCore(Animator animator, int layerCount)
    {
        animator.applyRootMotion = false;
        
        Graph = PlayableGraph.Create();
        var playableOutput = AnimationPlayableOutput.Create(Graph, "Animation", animator);
        
        LayerMixer = AnimationLayerMixerPlayable.Create(Graph);
        LayerMixer.SetInputCount(layerCount);
        playableOutput.SetSourcePlayable(LayerMixer);
        
        Graph.Play();
    }

    public void SetLayerWeight(int layerIndex, float weight)
    {
        if (LayerMixer.IsValid())
        {
            LayerMixer.SetInputWeight(layerIndex, Mathf.Clamp01(weight));
        }
    }
    
    public void Dispose()
    {
        Graph.Destroy();
    }
}
