using UnityEngine;

public abstract class State : ScriptableObject, IState
{
    [field: SerializeField] public StateType StateType { get; set; }
    [field: Space (3)]
    
    [field: Header("Speed Settings")]
    [field: SerializeField] public float MovementSpeed { get; set; }
    [field: SerializeField] public float AnimationSpeed { get; set; }
    [field: SerializeField] public float EnterBlendSpeed { get; set; }
    
    [field: Space (3)]
    [field: Header("Timer Settings")]
    [field: SerializeField] public TimerStartMode TimerStartMode { get; set; }
    [field: SerializeField] public float TimeToExit { get; set; }
    
    [field: Space (3)]
    [field: Header("Animation Settings")]
    [field: SerializeField] public AnimationClip TestClip { get; set; } // временно, заменить на более сложный бленд

    public virtual void OnEnter(IStateMachine stateMachine)
    {
        stateMachine.StatesTimer.Start(this);
        if (TestClip == null)
        {
            return;
        }
        stateMachine.AnimatorController.Play(StateType.ToString());
    }

    public virtual void OnUpdate(IStateMachine stateMachine)
    {
        stateMachine.StatesTimer.Update();
        CheckSwitchConditions(stateMachine);
    }
    
    public virtual void OnFixedUpdate(IStateMachine stateMachine) { }
    public virtual void OnLateUpdate(IStateMachine stateMachine) { }
    public virtual void CheckSwitchConditions(IStateMachine stateMachine) { }

    public virtual void OnExit(IStateMachine stateMachine)
    {
        stateMachine.StatesTimer.Reset();
        stateMachine.InputHandler.ResetBufferedInput();
    }

    public override string ToString()
    {
        return StateType.ToString();
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
    public void CheckSwitchConditions(IStateMachine stateMachine);
    public void OnExit(IStateMachine stateMachine);
}

public enum StateType
{
    Idle,
    Walk,
    Run,
    Sprint,
    Jump
}

