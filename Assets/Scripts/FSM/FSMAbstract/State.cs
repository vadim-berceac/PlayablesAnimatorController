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
    [field: SerializeField] public float TimeToExit { get; set; }
    [field: SerializeField] public bool ResetBufferedInput { get; set; } = true;
    
    [field: Space (3)]
    [field: Header("Animation Settings")]
    [field: SerializeField] public ClipBlendData[] ClipBlendData { get; set; }
    
    protected List<SwitchStateCondition<IStateMachine>> SwitchStateConditions = new List<SwitchStateCondition<IStateMachine>>();
    
    public virtual void OnEnter(IStateMachine stateMachine)
    {
        stateMachine.StatesTimer.Start(this);
        if (ClipBlendData == null || ClipBlendData.Length == 0)
        {
            return;
        }
        stateMachine.AnimatorController.Play(GetAnimationMixerPlayable(stateMachine.AnimatorController.GraphCore.Graph), GetBlendParams()
            , stateMachine.PreviousState ? CrossFadeTime : 0);

        if (ResetBufferedInput)
        {
            stateMachine.InputHandler.ResetBufferedInput();
        }
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

    private AnimationMixerPlayable GetAnimationMixerPlayable(PlayableGraph graph, int activeClipIndex = 0)
    {
        var mixer = AnimationMixerPlayable.Create(graph, ClipBlendData.Length);

        for (var i = 0; i < ClipBlendData.Length; i++)
        {
            if (ClipBlendData[i].Clip == null)
            {
                continue;
            }
            var clipPlayable = AnimationClipPlayable.Create(graph, ClipBlendData[i].Clip);

            graph.Connect(clipPlayable, 0, mixer, i);
          
            mixer.SetInputWeight(i, i == activeClipIndex ? 1f : 0f);
        }

        return mixer;
    }

    private List<BlendParams> GetBlendParams()
    {
        var blendParams = new List<BlendParams>();
        foreach (var b in ClipBlendData)
        {
            blendParams.Add(b.BlendParams);
        }
        return blendParams;
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