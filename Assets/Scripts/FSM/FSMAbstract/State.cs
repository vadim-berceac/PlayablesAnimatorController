using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public abstract class State : ScriptableObject, IState
{
    [field: SerializeField] public StateType StateType { get; set; }
    [field: Space (3)]
    
    [field: Header("Speed Settings")]
    [field: SerializeField] public SpeedMaxRule SpeedMaxRule { get; set; }
    [field: SerializeField] public float MovementSpeed { get; set; }
    [field: SerializeField] public float AnimationSpeed { get; set; }
    [field: SerializeField] public float EnterBlendSpeed { get; set; }
    [field: SerializeField] public float CrossFadeTime { get; set; }
    
    [field: Space (3)]
    [field: Header("Timer Settings")]
    [field: SerializeField] public TimerStartMode TimerStartMode { get; set; }
    [field: SerializeField, Range(0f, 1f)] public float ActionTime { get; set; }
    [field: SerializeField] public float TimeToExit { get; set; }
    [field: SerializeField] public bool ResetBufferedInput { get; set; } = true;
    
    [field: Space (3)]
    [field: Header("Animation Settings")]
    [field: SerializeField] public bool LinkToWeaponIndex { get; set; }
    [field: SerializeField] public ClipBlendDataCollection[] ClipBlendDataCollections { get; set; }
    
    protected List<SwitchStateCondition<IStateMachine>> SwitchStateConditions = new ();
    
    public virtual void OnEnter(IStateMachine stateMachine)
    {
        this.CheckDuplicatingState(stateMachine);
        stateMachine.SwitchAnimation();

        if (ResetBufferedInput)
        {
            stateMachine.Character.InputHandler.ResetBufferedInput();
        }
        
        this.SwitchAvatarMask(stateMachine, LinkToWeaponIndex, ClipBlendDataCollections);
    }

    public virtual void OnUpdate(IStateMachine stateMachine)
    {
        CheckSwitchConditions(stateMachine);
    }

    public virtual void OnFixedUpdate(IStateMachine stateMachine)
    {
        stateMachine.StatesTimer.Update();
    }
    
    public virtual void OnLateUpdate(IStateMachine stateMachine) { }

    private void CheckSwitchConditions(IStateMachine stateMachine)
    {
        if (SwitchStateConditions == null || SwitchStateConditions.Count == 0)
        {
            return;
        }

        foreach (var c in SwitchStateConditions)
        {
            if (!c.Check(stateMachine, out var newState))
            {
                continue;
            }
            stateMachine.SwitchState(newState);
            return;
        }
    }

    public virtual void OnExit(IStateMachine stateMachine)
    {
        stateMachine.StatesTimer.Reset();
    }
}

public interface IState
{
    public StateType StateType { get; set; }
    public float MovementSpeed { get; set; }
    public float AnimationSpeed { get; set; }
    public float EnterBlendSpeed { get; set; }
    
    public void OnEnter(IStateMachine stateMachine);
    public void OnUpdate(IStateMachine stateMachine);
    public void OnFixedUpdate(IStateMachine stateMachine);
    public void OnLateUpdate(IStateMachine stateMachine);
    public void OnExit(IStateMachine stateMachine);
}