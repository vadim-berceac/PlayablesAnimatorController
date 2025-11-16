using UnityEngine;

public class StatesTimer
{
    private float _duration;      
    private float _elapsed;       
    private bool _isActive;       
   
    public bool IsFinished => !_isActive && _elapsed >= _duration;
   
    public float Progress => _duration > 0f ? _elapsed / _duration : 0f;

    public float Elapsed => _elapsed;

    public float Remaining => Mathf.Max(0f, _duration - _elapsed);
    

    private void SetDuration(float duration)
    {
        _duration = Mathf.Max(0f, duration); 
        _elapsed = 0f;
        _isActive = _duration > 0f;
    }

    public void Start(State state)
    {
        switch (state.TimerStartMode)
        {
            case TimerStartMode.None: SetDuration(0f); break;
            case TimerStartMode.StateValue : SetDuration(state.TimeToExit); break;
            case TimerStartMode.NonLoopiedAnimationLenght:
            {
                if (state.TestClip == null || state.TestClip.length == 0 || state.TestClip.isLooping)
                {
                    SetDuration(0f);
                }
                else
                {
                    SetDuration(state.TestClip.length);
                }
            };
                break;
        }
    }
   
    public void Update()
    {
        if (!_isActive) return;

        _elapsed += Time.deltaTime;

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
    }
}

public enum TimerStartMode
{
    None,
    StateValue,
    NonLoopiedAnimationLenght
}