using UnityEngine;

public class StatesTimer
{
    private float _duration;      
    private float _elapsed;       
    private bool _isActive;       
    
    private float _eventNormalizedTime = -1f; 
    private bool _eventTriggeredThisFrame = false;

    public bool IsFinished => !_isActive && _elapsed >= _duration;

    public float Progress => _duration > 0f ? _elapsed / _duration : 0f;

    public float Elapsed => _elapsed;

    public float Remaining => Mathf.Max(0f, _duration - _elapsed);
    public bool IsEventTriggered
    {
        get
        {
            bool triggered = _eventTriggeredThisFrame;
            _eventTriggeredThisFrame = false; 
            return triggered;
        }
    }

    private void SetDuration(float duration)
    {
        _duration = Mathf.Max(0f, duration); 
        _elapsed = 0f;
        _isActive = _duration > 0f;
        _eventTriggeredThisFrame = false;
    }
    
    public void SetActionNormalizedTime(float normalizedTime)
    {
        _eventNormalizedTime = Mathf.Clamp01(normalizedTime);
    }

    public void Start(State state, int collectionIndex)
    {
        var collection = state.ClipBlendDataCollections[collectionIndex];
       
        _eventNormalizedTime = -1f;
        _eventTriggeredThisFrame = false;

        switch (state.TimerStartMode)
        {
            case TimerStartMode.None:
                SetDuration(0f);
                break;

            case TimerStartMode.StateValue:
                SetDuration(state.TimeToExit);
                break;

            case TimerStartMode.NonLoopiedAnimationLenght:
            {
                if (collection.ClipsBlendData[0].Clip == null || 
                    collection.ClipsBlendData[0].Clip.length == 0 || 
                    collection.ClipsBlendData[0].Clip.isLooping)
                {
                    SetDuration(0f);
                }
                else
                {
                    SetDuration(collection.ClipsBlendData[0].Clip.length);
                }
                break;
            }
        }
    }

    public void Update()
    {
        if (!_isActive) return;

        var previousElapsed = _elapsed;
        _elapsed += Time.fixedDeltaTime;

        if (_eventNormalizedTime >= 0f && _duration > 0f)
        {
            var eventTime = _eventNormalizedTime * _duration;
          
            if (previousElapsed < eventTime && _elapsed >= eventTime)
            {
                _eventTriggeredThisFrame = true;
            }
        }

        if (_elapsed < _duration)
        {
            return;
        }

        _elapsed = _duration; 
        _isActive = false;   
    }

    public void Reset()
    {
        _elapsed = 0f;
        _duration = 0f;
        _isActive = false;
        _eventNormalizedTime = -1f;
        _eventTriggeredThisFrame = false;
    }
}

public enum TimerStartMode
{
    None,
    StateValue,
    NonLoopiedAnimationLenght
}