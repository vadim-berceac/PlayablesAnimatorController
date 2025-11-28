using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public class PlayablesAnimatorController
{
    private readonly Animator _animator;
    private readonly PlayableGraph _playableGraph;

    private readonly AnimationMixerPlayable _generalMixerPlayable;
    private AnimationMixerPlayable _previousMixerPlayable;
    private AnimationMixerPlayable _currentMixerPlayable;
    
    private float _crossFadeDuration;
    private float _crossFadeTime;
    private int _activePort = 0;

    private bool _pendingCleanup;
    private int _pendingPort = -1;
    private AnimationMixerPlayable _fadingOutMixer;
    
    private const float TinyWeight = 1e-6f;
   
    public PlayableGraph PlayableGraph => _playableGraph;
    public bool IsCrossFading { get; private set; }

    public PlayablesAnimatorController(Animator animator)
    {
        _animator = animator;
        _playableGraph = PlayableGraph.Create("PlayableGraph");
        var playableOutput = AnimationPlayableOutput.Create(_playableGraph, "Animation", _animator);
      
        _animator.applyRootMotion = false;
        _generalMixerPlayable = AnimationMixerPlayable.Create(_playableGraph, 2);
        playableOutput.SetSourcePlayable(_generalMixerPlayable);
        
        _generalMixerPlayable.SetInputWeight(0, 0f);
        _generalMixerPlayable.SetInputWeight(1, 0f);
       
        _playableGraph.Play();
    }
  
    public void Play(AnimationMixerPlayable nextStateMixerPlayable, float crossFadeDuration)
    {
        _crossFadeDuration = crossFadeDuration;

        var isFirst = !_previousMixerPlayable.IsValid();
        var transitionTime = 0.0;
        if (!isFirst)
        {
            transitionTime = _previousMixerPlayable.GetTime();
        }

        if (isFirst)
        {
            _currentMixerPlayable = nextStateMixerPlayable;
            NormalizeMixerWeights(_currentMixerPlayable);
            _generalMixerPlayable.SetInputWeight(0, 0f); 
            _playableGraph.Connect(_currentMixerPlayable, 0, _generalMixerPlayable, 0);
            ResetInputs(_currentMixerPlayable, transitionTime);
            _currentMixerPlayable.SetTime(transitionTime); 
            _generalMixerPlayable.SetInputWeight(0, TinyWeight);
            _playableGraph.Evaluate();
            _generalMixerPlayable.SetInputWeight(0, 1f);
            NormalizeGeneralWeights();
            _previousMixerPlayable = _currentMixerPlayable;
            _activePort = 0;
            IsCrossFading = false;
            return;
        }
        
        var sourcePort = _activePort;
        var targetPort = 1 - sourcePort;
        var interrupting = IsCrossFading;

        if (interrupting)
        {
            transitionTime = _currentMixerPlayable.GetTime();
            _generalMixerPlayable.SetInputWeight(sourcePort, 0f);
            _generalMixerPlayable.SetInputWeight(targetPort, 1f);
            NormalizeGeneralWeights();
            
            _playableGraph.Disconnect(_generalMixerPlayable, sourcePort);
            DestroyMixerAndInputs(_previousMixerPlayable);
            
            _activePort = targetPort;
            _previousMixerPlayable = _currentMixerPlayable;
            IsCrossFading = false;
        }

        if (interrupting)
        {
            sourcePort = _activePort;
            targetPort = 1 - sourcePort;
            transitionTime = _previousMixerPlayable.GetTime();  
        }
        
        if (_pendingCleanup && _pendingPort == targetPort)
        {
            _playableGraph.Disconnect(_generalMixerPlayable, _pendingPort);
            DestroyMixerAndInputs(_fadingOutMixer);
            
            _pendingCleanup = false;
            _pendingPort = -1;
            _fadingOutMixer = default;
        }
       
        _crossFadeTime = 0f;
        IsCrossFading = true;
        _previousMixerPlayable = _currentMixerPlayable;
        _currentMixerPlayable = nextStateMixerPlayable;
        NormalizeMixerWeights(_currentMixerPlayable);  
        
        var clipPlayable = _currentMixerPlayable.GetInput(0); 
        if (clipPlayable.IsValid())
        {
            var animationClip = ((AnimationClipPlayable)clipPlayable).GetAnimationClip();
            if (animationClip != null && !animationClip.isLooping)
            {
                transitionTime = 0.0;  
            }
        }

        _generalMixerPlayable.SetInputWeight(targetPort, 0f); 
        _playableGraph.Disconnect(_generalMixerPlayable, targetPort);
        _playableGraph.Connect(_currentMixerPlayable, 0, _generalMixerPlayable, targetPort);
        ResetInputs(_currentMixerPlayable, transitionTime);
        _currentMixerPlayable.SetTime(transitionTime);  
        _generalMixerPlayable.SetInputWeight(targetPort, TinyWeight);
        _playableGraph.Evaluate();
        _generalMixerPlayable.SetInputWeight(targetPort, 0f);
        _generalMixerPlayable.SetInputWeight(sourcePort, 1f);
        NormalizeGeneralWeights();
    }

    public void OnUpdate()
    {
        CleanUp();
        CrossFade();
    }

    private void CrossFade()
    {
        if (!IsCrossFading) return;

        _crossFadeTime += Time.deltaTime;
        var t = Mathf.Clamp01(_crossFadeTime / _crossFadeDuration);

        var sourcePort = _activePort;
        var targetPort = 1 - sourcePort;
       
        _generalMixerPlayable.SetInputWeight(sourcePort, 1f - t);
        _generalMixerPlayable.SetInputWeight(targetPort, t);
        NormalizeGeneralWeights(); 

        if (t >= 1f)
        {
            IsCrossFading = false;
            _generalMixerPlayable.SetInputWeight(sourcePort, 0f);
            _generalMixerPlayable.SetInputWeight(targetPort, 1f);
            NormalizeGeneralWeights();
           
            _fadingOutMixer = _previousMixerPlayable;
            _pendingCleanup = true;
            _pendingPort = sourcePort;
            _activePort = targetPort;
            _previousMixerPlayable = _currentMixerPlayable;
        }
    }

    private void CleanUp()
    {
        if (!_pendingCleanup)
        {
           return;
        }
        
        var pendingPort = _pendingPort;
            
        _playableGraph.Disconnect(_generalMixerPlayable, pendingPort);
        DestroyMixerAndInputs(_fadingOutMixer);
            
        _pendingCleanup = false;
        _pendingPort = -1;
        _fadingOutMixer = default;
    }

    private void NormalizeMixerWeights(AnimationMixerPlayable mixer)
    {
        if (!mixer.IsValid()) return;
        var sum = 0f;
        var count = mixer.GetInputCount();
        for (var i = 0; i < count; i++)
        {
            sum += mixer.GetInputWeight(i);
        }
        if (Mathf.Approximately(sum, 0f)) return; 
        if (sum > 1f + Mathf.Epsilon)
        {
            for (var i = 0; i < count; i++)
            {
                mixer.SetInputWeight(i, mixer.GetInputWeight(i) / sum);
            }
        }
    }

    private void NormalizeGeneralWeights()
    {
        var sum = 0f;
        var count = _generalMixerPlayable.GetInputCount();
        for (var i = 0; i < count; i++)
        {
            sum += _generalMixerPlayable.GetInputWeight(i);
        }
        if (Mathf.Approximately(sum, 0f)) return;
        if (!Mathf.Approximately(sum, 1f))
        {
            for (var i = 0; i < count; i++)
            {
                _generalMixerPlayable.SetInputWeight(i, _generalMixerPlayable.GetInputWeight(i) / sum);
            }
        }
    }

    private void ResetInputs(AnimationMixerPlayable mixer, double time)
    {
        for (var i = 0; i < mixer.GetInputCount(); i++)
        {
            var input = mixer.GetInput(i);
            if (input.IsValid())
            {
                input.SetTime(time);
                input.SetSpeed(1);
            }
        }
    }

    private void DestroyMixerAndInputs(AnimationMixerPlayable mixer)
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

    public void Dispose()
    {
        _playableGraph.Destroy();
    }
}