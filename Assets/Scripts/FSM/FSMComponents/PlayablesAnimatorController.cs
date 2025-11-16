using UnityEngine;

public class PlayablesAnimatorController
{
    private readonly Animator _animator;
    
    //временно до написания PlayablesAnimatorController
    private readonly int _speedHash = Animator.StringToHash("AnimSpeed");

    public PlayablesAnimatorController(Animator animator)
    {
        _animator = animator;
    }

    public void Play (AnimationClip clip)
    {
        // заготовка для постоянного варианта
        // но возможно заменить на бленд клипов
    }
    
    public void Play (string stateName)
    {
        //временно до написания PlayablesAnimatorController
        _animator.Play(stateName);
    }

    public void SetAnimationSpeed(float value)
    {
        //временно до написания PlayablesAnimatorController
        _animator.SetFloat(_speedHash, value);
    }
}
