using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public class PlayablesAnimatorController
{
    private readonly Animator _animator;
    private readonly Transform _transform;
    private readonly PlayableGraph _playableGraph;

    private readonly AnimationMixerPlayable _generalMixerPlayable;
    private AnimationMixerPlayable _previousMixerPlayable = default;
    private AnimationMixerPlayable _currentMixerPlayable = default;
    
    private float _crossFadeDuration;
    private float _crossFadeTime;
    private int _activePort = 0;

    // Для отложенной очистки на конце кроссфейда (чтобы избежать T-pose на кадре отключения)
    private bool _pendingCleanup = false;
    private int _pendingPort = -1;
    private AnimationMixerPlayable _fadingOutMixer = default;

    // Fallback для маскировки T-pose: безопасный клип (idle или нейтральная поза), подключается к порту 2
    //private bool _hasFallback;
    //private AnimationClipPlayable _fallbackPlayable;
   
    public PlayableGraph PlayableGraph => _playableGraph;
    public bool IsCrossFading { get; private set; }

    public PlayablesAnimatorController(Animator animator)
    {
        _animator = animator;
        _transform = animator.transform;
        _playableGraph = PlayableGraph.Create("PlayableGraph");
        var playableOutput = AnimationPlayableOutput.Create(_playableGraph, "Animation", _animator);
        
        // Отключаем автоматическое применение root motion — будем применять вручную для контроля
        _animator.applyRootMotion = false;
        _generalMixerPlayable = AnimationMixerPlayable.Create(_playableGraph, 2);
        playableOutput.SetSourcePlayable(_generalMixerPlayable);
        
        _generalMixerPlayable.SetInputWeight(0, 0f);
        _generalMixerPlayable.SetInputWeight(1, 0f);
       
        _playableGraph.Play();
    }

    /// <summary>
    /// Запускает новый миксер с кроссфейдом
    /// </summary>
    public void Play(AnimationMixerPlayable nextStateMixerPlayable, float crossFadeDuration)
    {
        const float tinyWeight = 1e-6f;  // Выносим в начало метода, чтобы избежать конфликта имён в scopes
        _crossFadeDuration = crossFadeDuration;

        bool isFirst = !_previousMixerPlayable.IsValid();
        double transitionTime = 0.0;
        if (!isFirst)
        {
            // Матчинг времени для seamless перехода: новый миксер стартует с текущего времени старого
            transitionTime = _previousMixerPlayable.GetTime();
        }

        if (isFirst)
        {
            // Первое воспроизведение — мгновенное, без кроссфейда
            _currentMixerPlayable = nextStateMixerPlayable;
            NormalizeMixerWeights(_currentMixerPlayable);
            int port = 0;
            _generalMixerPlayable.SetInputWeight(port, 0f);  // Явно устанавливаем вес 0 перед подключением
            _playableGraph.Connect(_currentMixerPlayable, 0, _generalMixerPlayable, port);
            ResetInputs(_currentMixerPlayable, transitionTime);
            _currentMixerPlayable.SetTime(transitionTime);  // Устанавливаем время на миксер для синхронизации inputs
            // Pre-evaluate to warm up the new branch with tiny weight (избегаем distortion от evaluate at 0)
            _generalMixerPlayable.SetInputWeight(port, tinyWeight);
            _playableGraph.Evaluate();
            _generalMixerPlayable.SetInputWeight(port, 1f);
            NormalizeGeneralWeights();
            _previousMixerPlayable = _currentMixerPlayable;
            _activePort = port;
            IsCrossFading = false;
            return;
        }

        // Не первое воспроизведение
        int sourcePort = _activePort;
        int targetPort = 1 - sourcePort;
        bool interrupting = IsCrossFading;

        if (interrupting)
        {
            // Прерываем текущий кроссфейд: мгновенно переходим к текущему (на targetPort)
            // Обновляем transitionTime для прерывания (текущее время current)
            transitionTime = _currentMixerPlayable.GetTime();
            // Маскируем потенциальный T-pose fallback'ом перед отключением
            _generalMixerPlayable.SetInputWeight(sourcePort, 0f);
            _generalMixerPlayable.SetInputWeight(targetPort, 1f);
            NormalizeGeneralWeights();
            
            _playableGraph.Disconnect(_generalMixerPlayable, sourcePort);
            DestroyMixerAndInputs(_previousMixerPlayable);
            
            _activePort = targetPort;
            _previousMixerPlayable = _currentMixerPlayable;
            IsCrossFading = false;
        }

        // Если прерывали, корректируем порты для нового кроссфейда и время
        if (interrupting)
        {
            sourcePort = _activePort;
            targetPort = 1 - sourcePort;
            transitionTime = _previousMixerPlayable.GetTime();  // Теперь previous — это interrupted current
        }

        // Если есть отложенная очистка и мы переиспользуем pending порт (targetPort), выполняем очистку немедленно
        // с маскировкой fallback'ом
        if (_pendingCleanup && _pendingPort == targetPort)
        {
            _playableGraph.Disconnect(_generalMixerPlayable, _pendingPort);
            DestroyMixerAndInputs(_fadingOutMixer);
            
            _pendingCleanup = false;
            _pendingPort = -1;
            _fadingOutMixer = default;
        }

        // Запускаем новый кроссфейд
        _crossFadeTime = 0f;
        IsCrossFading = true;
        _previousMixerPlayable = _currentMixerPlayable;
        _currentMixerPlayable = nextStateMixerPlayable;
        NormalizeMixerWeights(_currentMixerPlayable);  // Нормализуем веса в новом миксере, чтобы избежать превышения sum>1

        // Подготавливаем порт для нового
        _generalMixerPlayable.SetInputWeight(targetPort, 0f);  // Явно устанавливаем вес 0 перед подключением
        _playableGraph.Disconnect(_generalMixerPlayable, targetPort);
        _playableGraph.Connect(_currentMixerPlayable, 0, _generalMixerPlayable, targetPort);
        ResetInputs(_currentMixerPlayable, transitionTime);
        _currentMixerPlayable.SetTime(transitionTime);  // Синхронизируем время миксера
        // Pre-evaluate to warm up the new branch with tiny weight (избегаем distortion от evaluate at 0)
        _generalMixerPlayable.SetInputWeight(targetPort, tinyWeight);
        _playableGraph.Evaluate();
        _generalMixerPlayable.SetInputWeight(targetPort, 0f);
        _generalMixerPlayable.SetInputWeight(sourcePort, 1f);
        NormalizeGeneralWeights();
    }

    public void OnUpdate()
    {
        // Отложенная очистка (на следующий кадр после установки веса=0) с маскировкой fallback'ом
        if (_pendingCleanup)
        {
            int pendingPort = _pendingPort;
            
            _playableGraph.Disconnect(_generalMixerPlayable, pendingPort);
            DestroyMixerAndInputs(_fadingOutMixer);
            
            _pendingCleanup = false;
            _pendingPort = -1;
            _fadingOutMixer = default;
        }

        CrossFade();
    }

    /// <summary>
    /// Применяет root motion вручную (вызывать в LateUpdate MonoBehaviour)
    /// Обнуляем Y-компоненту deltaPosition всегда для walk/idle (компенсирует blended drift)
    /// </summary>
    public void ApplyRootMotion()
    {
        var deltaPos = _animator.deltaPosition;
        var deltaRot = _animator.deltaRotation;
        deltaPos.y = 0f;  // Всегда обнуляем Y для избежания fly от baked offsets в clips
        _transform.position += deltaPos;
        _transform.rotation *= deltaRot;
    }

    private void CrossFade()
    {
        if (!IsCrossFading) return;

        _crossFadeTime += Time.deltaTime;
        float t = Mathf.Clamp01(_crossFadeTime / _crossFadeDuration);

        int sourcePort = _activePort;
        int targetPort = 1 - sourcePort;

        // Плавно уменьшаем вес старого и увеличиваем вес нового
        _generalMixerPlayable.SetInputWeight(sourcePort, 1f - t);
        _generalMixerPlayable.SetInputWeight(targetPort, t);
        NormalizeGeneralWeights();  // Гарантируем sum=1 после lerp (на случай FP errors)

        if (t >= 1f)
        {
            IsCrossFading = false;
            _generalMixerPlayable.SetInputWeight(sourcePort, 0f);
            _generalMixerPlayable.SetInputWeight(targetPort, 1f);
            NormalizeGeneralWeights();

            // Планируем очистку на следующий кадр (чтобы избежать T-pose на текущем)
            _fadingOutMixer = _previousMixerPlayable;
            _pendingCleanup = true;
            _pendingPort = sourcePort;

            // Обновляем active и previous (очистка миксера не повлияет, т.к. вес=0)
            _activePort = targetPort;
            _previousMixerPlayable = _currentMixerPlayable;
        }
    }

    /// <summary>
    /// Нормализует веса входов миксера, чтобы их сумма была <=1 (предотвращает усиление поз от sum>1)
    /// </summary>
    private void NormalizeMixerWeights(AnimationMixerPlayable mixer)
    {
        if (!mixer.IsValid()) return;
        float sum = 0f;
        int count = mixer.GetInputCount();
        for (int i = 0; i < count; i++)
        {
            sum += mixer.GetInputWeight(i);
        }
        if (Mathf.Approximately(sum, 0f)) return;  // Избегаем деления на 0
        if (sum > 1f + Mathf.Epsilon)
        {
            for (int i = 0; i < count; i++)
            {
                mixer.SetInputWeight(i, mixer.GetInputWeight(i) / sum);
            }
        }
    }

    /// <summary>
    /// Нормализует веса general mixer'а (гарантирует sum=1 после любых SetWeight)
    /// </summary>
    private void NormalizeGeneralWeights()
    {
        float sum = 0f;
        int count = _generalMixerPlayable.GetInputCount();
        for (int i = 0; i < count; i++)
        {
            sum += _generalMixerPlayable.GetInputWeight(i);
        }
        if (Mathf.Approximately(sum, 0f)) return;
        if (!Mathf.Approximately(sum, 1f))
        {
            for (int i = 0; i < count; i++)
            {
                _generalMixerPlayable.SetInputWeight(i, _generalMixerPlayable.GetInputWeight(i) / sum);
            }
        }
    }

    private void ResetInputs(AnimationMixerPlayable mixer, double time)
    {
        for (int i = 0; i < mixer.GetInputCount(); i++)
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
        for (int i = 0; i < mixer.GetInputCount(); i++)
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