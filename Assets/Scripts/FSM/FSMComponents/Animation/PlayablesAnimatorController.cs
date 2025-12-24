using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public class PlayablesAnimatorController
{
    private readonly GraphCore _graphCore;
    private readonly AnimationMixerPlayable _generalMixerPlayable;
    private AnimationMixerPlayable _previousMixerPlayable;
    private AnimationMixerPlayable _currentMixerPlayable;
    private List<BlendParams> _currentBlendParams = new();
    private float _crossFadeDuration;
    private float _crossFadeTime;
    private int _activePort;
    private bool _pendingCleanup;
    private int _pendingPort = -1;
    private AnimationMixerPlayable _fadingOutMixer;
   
    private bool _isMaskCrossFading;
    private float _maskCrossFadeDuration;
    private float _maskCrossFadeTime;
    private readonly List<(int layerIndex, AvatarMask oldMask, AvatarMask newMask, bool isAdditive)> _maskTransitions = new();
    private readonly Dictionary<int, (int tempLayerIndex, float originalWeight, int tempOutputIndex)> _tempMaskLayers = new();
    private int _tempOriginalOutputCount;
    private int _tempOriginalInputCount;

    private const float TinyWeight = 1e-6f;
    private const float ParamSmoothing = 4f;
    private const float Power = 2f;
    private const float Eps = 1e-5f;
    private const float Threshold = 0.05f;

    private Vector2 _smoothedParams = Vector2.zero;
    private readonly Dictionary<int, (AvatarMask mask, bool isAdditive)> _layerConfigs = new();

    public GraphCore GraphCore => _graphCore;
    public bool IsCrossFading { get; private set; }
    private bool _crossFadeComplete;

    public bool ConsumeCrossFading()
    {
        if (_crossFadeComplete)
        {
            _crossFadeComplete = false;
            return true;
        }
        return false;
    }

    public PlayablesAnimatorController(GraphCore graphCore, int graphPort)
    {
        _graphCore = graphCore;
        _generalMixerPlayable = AnimationMixerPlayable.Create(_graphCore.Graph, 2);
        _graphCore.LayerMixer.ConnectInput(graphPort, _generalMixerPlayable, 0);
        _generalMixerPlayable.SetWeights(false, (0, 0), (0, 1));
    }

    public void Play(AnimationMixerPlayable nextStateMixerPlayable, List<BlendParams> blendParamsList, float crossFadeDuration)
    {
        _currentBlendParams = blendParamsList;
        _crossFadeDuration = crossFadeDuration;

        if (IsFirstPlay())
        {
            HandleFirstPlay(nextStateMixerPlayable);
            return;
        }

        var sourcePort = _activePort;
        var targetPort = 1 - sourcePort;

        if (IsCrossFading)
        {
            HandleInterrupt(sourcePort, targetPort);
        }

        HandlePendingCleanup(targetPort);
        PrepareCrossFade(nextStateMixerPlayable, sourcePort, targetPort);
    }

    public void ConnectToMultipleLayers(List<(int graphPortIndex, int outputPortIndex, AvatarMask mask, bool isAdditive, float weight)> layerConfigs)
    {
        if (!_generalMixerPlayable.IsValid()) return;

        _generalMixerPlayable.SetOutputCount(layerConfigs.Count + 1);

        foreach (var config in layerConfigs)
        {
            var layerIndex = config.graphPortIndex;
            var outputPortIndex = config.outputPortIndex;
            var mask = config.mask;
            var isAdditive = config.isAdditive;
            var weight = config.weight;

            _graphCore.SetUpLayer(layerIndex, mask, isAdditive);
            _graphCore.Graph.Disconnect(_graphCore.LayerMixer, layerIndex);
            _graphCore.Graph.Connect(_generalMixerPlayable, outputPortIndex, _graphCore.LayerMixer, layerIndex);
            _graphCore.SetLayerWeight(layerIndex, weight);

            _layerConfigs[layerIndex] = (mask, isAdditive);
        }

        _graphCore.Graph.Evaluate();
    }

    public void SetAvatarMask(List<(int layerIndex, AvatarMask avatarMask, bool isAdditive)> layerConfigs, float crossFadeDuration = 0.2f)
    {
        if (_isMaskCrossFading)
        {
            CleanupMaskCrossFade();
        }

        _maskTransitions.Clear();
        foreach (var config in layerConfigs)
        {
            var layerIndex = config.layerIndex;
            var newMask = config.avatarMask;
            var isAdditive = config.isAdditive;
            var oldMask = GetCurrentMask(layerIndex);
            _maskTransitions.Add((layerIndex, oldMask, newMask, isAdditive));
        }

        if (crossFadeDuration <= 0f)
        {
            foreach (var config in layerConfigs)
            {
                _graphCore.SetUpLayer(config.layerIndex, config.avatarMask, config.isAdditive);
                _layerConfigs[config.layerIndex] = (config.avatarMask, config.isAdditive);
            }
            _graphCore.Graph.Evaluate();
            _maskTransitions.Clear();
            return;
        }

        SetupMaskCrossFadeLayers();

        _maskCrossFadeDuration = crossFadeDuration;
        _maskCrossFadeTime = 0f;
        _isMaskCrossFading = true;
    }

    public void OnUpdate(float deltaTime, params float[] parameters)
    {
        CleanUp();
        CrossFade(deltaTime);
        CrossFadeMasks(deltaTime);
        RotationSmooth(deltaTime, parameters);
        SetAnimationParams(_smoothedParams.x, _smoothedParams.y);
    }

    private void CrossFadeMasks(float deltaTime)
    {
        if (!_isMaskCrossFading) return;

        _maskCrossFadeTime += deltaTime;
        var t = Mathf.Clamp01(_maskCrossFadeTime / _maskCrossFadeDuration);
        var smoothT = t * t * (3f - 2f * t); 

        foreach (var kvp in _tempMaskLayers)
        {
            var originalLayerIndex = kvp.Key;
            var tempLayerIndex = kvp.Value.tempLayerIndex;
            var originalWeight = kvp.Value.originalWeight;

            _graphCore.SetLayerWeight(originalLayerIndex, originalWeight * (1f - smoothT));

            _graphCore.SetLayerWeight(tempLayerIndex, originalWeight * smoothT);
        }

        if (t < 1f) return;

        FinishMaskCrossFade();
    }

    private void SetupMaskCrossFadeLayers()
    {
        _tempMaskLayers.Clear();
        _tempOriginalInputCount = _graphCore.LayerMixer.GetInputCount();
        var maxLayerIndex = _tempOriginalInputCount;
        _tempOriginalOutputCount = _generalMixerPlayable.GetOutputCount();
        var currentOutputCount = _tempOriginalOutputCount;

        foreach (var transition in _maskTransitions)
        {
            var originalLayerIndex = transition.layerIndex;
            var newMask = transition.newMask;
            var isAdditive = transition.isAdditive;

            var originalWeight = _graphCore.LayerMixer.GetInputWeight(originalLayerIndex);

            var tempLayerIndex = maxLayerIndex;
            maxLayerIndex++;

            if (tempLayerIndex >= _graphCore.LayerMixer.GetInputCount())
            {
                _graphCore.LayerMixer.SetInputCount(tempLayerIndex + 1);
            }

            _graphCore.SetUpLayer(tempLayerIndex, newMask, isAdditive);

            var currentMixer = _graphCore.LayerMixer.GetInput(originalLayerIndex);
            if (currentMixer.IsValid())
            {
                var newOutputIndex = currentOutputCount;
                _generalMixerPlayable.SetOutputCount(currentOutputCount + 1);
                _graphCore.Graph.Connect(_generalMixerPlayable, newOutputIndex, _graphCore.LayerMixer, tempLayerIndex);
                currentOutputCount++;
                _tempMaskLayers[originalLayerIndex] = (tempLayerIndex, originalWeight, newOutputIndex);
            }

            _graphCore.SetLayerWeight(tempLayerIndex, 0f);
        }

        _graphCore.Graph.Evaluate();
    }

    private void CleanupMaskCrossFade()
    {
        foreach (var transition in _maskTransitions)
        {
            _graphCore.SetUpLayer(transition.layerIndex, transition.newMask, transition.isAdditive);
            if (_tempMaskLayers.TryGetValue(transition.layerIndex, out var tempInfo))
            {
                _graphCore.SetLayerWeight(transition.layerIndex, tempInfo.originalWeight);
                _layerConfigs[transition.layerIndex] = (transition.newMask, transition.isAdditive);
            }
        }

        foreach (var kvp in _tempMaskLayers)
        {
            var tempLayerIndex = kvp.Value.tempLayerIndex;
            _graphCore.Graph.Disconnect(_graphCore.LayerMixer, tempLayerIndex);
            _graphCore.SetLayerWeight(tempLayerIndex, 0f);
        }

        _graphCore.LayerMixer.SetInputCount(_tempOriginalInputCount);
        _generalMixerPlayable.SetOutputCount(_tempOriginalOutputCount);

        _tempMaskLayers.Clear();
        _maskTransitions.Clear();
        _isMaskCrossFading = false;
        _graphCore.Graph.Evaluate();
    }

    private void FinishMaskCrossFade()
    {
        foreach (var transition in _maskTransitions)
        {
            var layerIndex = transition.layerIndex;
            _graphCore.SetUpLayer(layerIndex, transition.newMask, transition.isAdditive);
            if (_tempMaskLayers.TryGetValue(layerIndex, out var tempInfo))
            {
                _graphCore.SetLayerWeight(layerIndex, tempInfo.originalWeight);
                _layerConfigs[layerIndex] = (transition.newMask, transition.isAdditive);
            }
        }

        foreach (var kvp in _tempMaskLayers)
        {
            var tempLayerIndex = kvp.Value.tempLayerIndex;
            _graphCore.Graph.Disconnect(_graphCore.LayerMixer, tempLayerIndex);
            _graphCore.SetLayerWeight(tempLayerIndex, 0f);
        }

        _graphCore.LayerMixer.SetInputCount(_tempOriginalInputCount);
        _generalMixerPlayable.SetOutputCount(_tempOriginalOutputCount);

        _graphCore.Graph.Evaluate();
        _tempMaskLayers.Clear();
        _maskTransitions.Clear();
        _isMaskCrossFading = false;
    }

    private AvatarMask GetCurrentMask(int layerIndex)
    {
        if (_layerConfigs.TryGetValue(layerIndex, out var config))
        {
            return config.mask;
        }
        return null;
    }

    private void RotationSmooth(float deltaTime, params float[] parameters)
    {
        var targetParams = Vector2.zero;
        if (parameters != null && parameters.Length >= 2)
        {
            targetParams = new Vector2(Mathf.Clamp(parameters[0], -1f, 1f), Mathf.Clamp(parameters[1], -1f, 1f));
        }
        _smoothedParams = Vector2.Lerp(_smoothedParams, targetParams, 1f - Mathf.Exp(-ParamSmoothing * deltaTime));
    }

    private void CrossFade(float deltaTime)
    {
        if (!IsCrossFading) return;

        _crossFadeTime += deltaTime;
        var t = Mathf.Clamp01(_crossFadeTime / _crossFadeDuration);

        var sourcePort = _activePort;
        var targetPort = 1 - sourcePort;

        _generalMixerPlayable.SetWeights(true, (sourcePort, 1f - t), (targetPort, t));

        if (t < 1f) return;

        FinishCrossFade(sourcePort, targetPort);
    }

    private void CleanUp()
    {
        if (!_pendingCleanup) return;

        DisconnectAndDestroy(_pendingPort, _fadingOutMixer);
        _pendingCleanup = false;
        _pendingPort = -1;
        _fadingOutMixer = default;
    }

    private void SetAnimationParams(float x, float y)
    {
        if (!_currentMixerPlayable.IsValid()) return;

        var inputCount = _currentMixerPlayable.GetInputCount();
        if (inputCount != _currentBlendParams.Count || inputCount == 0) return;

        var weights = new float[inputCount];
        var total = 0f;
        var minDist = float.MaxValue;
        var nearestIdx = -1;

        for (var i = 0; i < inputCount; i++)
        {
            var bp = _currentBlendParams[i];
            var dx = x - bp.Param1;
            var dy = y - bp.Param2;
            var dist = Mathf.Sqrt(dx * dx + dy * dy);

            if (dist < minDist)
            {
                minDist = dist;
                nearestIdx = i;
            }

            var w = 1f / Mathf.Pow(Mathf.Max(dist, Eps), Power);
            weights[i] = w;
            total += w;
        }

        if (minDist < Threshold)
        {
            for (var i = 0; i < inputCount; i++)
            {
                _currentMixerPlayable.SetInputWeight(i, i == nearestIdx ? 1f : 0f);
            }
            return;
        }

        if (total < 1e-6f)
        {
            var uniform = 1f / inputCount;
            for (var i = 0; i < inputCount; i++)
            {
                _currentMixerPlayable.SetInputWeight(i, uniform);
            }
            return;
        }

        for (var i = 0; i < inputCount; i++)
        {
            var normalized = weights[i] / total;
            _currentMixerPlayable.SetInputWeight(i, normalized);
        }
    }

    private bool IsFirstPlay() => !_previousMixerPlayable.IsValid();

    private void HandleFirstPlay(AnimationMixerPlayable nextMixer)
    {
        _currentMixerPlayable = nextMixer;
        _currentMixerPlayable.NormalizeMixerWeights();
        ConnectMixer(_currentMixerPlayable, 0, 0.0);
        _generalMixerPlayable.SetWeights(false, (0, 1f));
        _previousMixerPlayable = _currentMixerPlayable;
        _activePort = 0;
        IsCrossFading = false;
        _crossFadeComplete = false;
    }

    private void HandleInterrupt(int sourcePort, int targetPort)
    {
        _generalMixerPlayable.SetWeights(true, (sourcePort, 0f), (targetPort, 1f));
        DisconnectAndDestroy(sourcePort, _previousMixerPlayable);
        _activePort = targetPort;
        _previousMixerPlayable = _currentMixerPlayable;
        IsCrossFading = false;
        _crossFadeComplete = false;
    }

    private void HandlePendingCleanup(int targetPort)
    {
        if (_pendingCleanup && _pendingPort == targetPort)
        {
            DisconnectAndDestroy(_pendingPort, _fadingOutMixer);
            _pendingCleanup = false;
            _pendingPort = -1;
            _fadingOutMixer = default;
        }
    }

    private void PrepareCrossFade(AnimationMixerPlayable nextMixer, int sourcePort, int targetPort)
    {
        _crossFadeTime = 0f;
        IsCrossFading = true;
        _crossFadeComplete = true;

        _previousMixerPlayable = _currentMixerPlayable;
        _currentMixerPlayable = nextMixer;
        _currentMixerPlayable.NormalizeMixerWeights();

        var transitionTime = _currentMixerPlayable.GetTransitionTime(_previousMixerPlayable);
        ConnectMixer(_currentMixerPlayable, targetPort, transitionTime);

        _generalMixerPlayable.SetWeights(true, (targetPort, 0f), (sourcePort, 1f));
    }

    private void FinishCrossFade(int sourcePort, int targetPort)
    {
        IsCrossFading = false;
        _crossFadeComplete = false;

        _generalMixerPlayable.SetWeights(true, (sourcePort, 0f), (targetPort, 1f));

        _fadingOutMixer = _previousMixerPlayable;
        _pendingCleanup = true;
        _pendingPort = sourcePort;

        _activePort = targetPort;
        _previousMixerPlayable = _currentMixerPlayable;
    }

    private void ConnectMixer(AnimationMixerPlayable mixer, int port, double time)
    {
        _graphCore.Graph.Disconnect(_generalMixerPlayable, port);
        _graphCore.Graph.Connect(mixer, 0, _generalMixerPlayable, port);
        mixer.ResetInputs(time);
        mixer.SetTime(time);
        _generalMixerPlayable.SetInputWeight(port, TinyWeight);
        _graphCore.Graph.Evaluate();
    }

    private void DisconnectAndDestroy(int port, AnimationMixerPlayable mixer)
    {
        _graphCore.Graph.Disconnect(_generalMixerPlayable, port);
        mixer.DestroyMixerAndInputs();
    }
}