using UnityEngine;
using Zenject;

public class Character : MonoBehaviour
{
    [field: SerializeField] public bool IsPlayerControlled { get; set; }
    [field: SerializeField] public AnimationSettings AnimationSettings { get; set; }
    
    public Fsm FullBodyFsm { get; set; }
    private StatesContainer _statesContainer;
    private PlayerInput _playerInput;

    [Inject]
    private void Construct(StatesContainer statesContainer, PlayerInput playerInput)
    {
        _statesContainer = statesContainer;
        _playerInput = playerInput;
        FullBodyFsm = new Fsm(_statesContainer, _playerInput, AnimationSettings.Animator, IsPlayerControlled);
    }

    private void Update()
    {
        FullBodyFsm.Update();
    }

    private void FixedUpdate()
    {
        FullBodyFsm.FixedUpdate();
    }

    private void LateUpdate()
    {
        FullBodyFsm.LateUpdate();
    }

    private void OnDestroy()
    {
        FullBodyFsm.OnDestroy();
    }
}

[System.Serializable]
public struct AnimationSettings
{
    [field: SerializeField] public Animator Animator { get; private set; }
}

