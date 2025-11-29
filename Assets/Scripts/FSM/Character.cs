using UnityEngine;
using Zenject;

public class Character : MonoBehaviour
{
    [field: SerializeField] public bool IsPlayerControlled { get; set; }
    [field: SerializeField] public AnimationSettings AnimationSettings { get; set; }
    
    private GraphCore _graphCore;
    public Fsm FullBodyFsm { get; set; }
    private StatesContainer _statesContainer;
    private PlayerInput _playerInput;

    [Inject]
    private void Construct(StatesContainer statesContainer, PlayerInput playerInput)
    {
        _statesContainer = statesContainer;
        _playerInput = playerInput;
        
        _graphCore = new GraphCore(AnimationSettings.Animator, 2);
        FullBodyFsm = new Fsm(_statesContainer, _playerInput, _graphCore, 0, SetType.FullBody, IsPlayerControlled);
        
        _graphCore.SetLayerWeight(0,1);
        _graphCore.SetUpLayer(0, _statesContainer.GetAvatarMaskBySetType(SetType.FullBody), false);
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
       _graphCore.Dispose();
    }
}

[System.Serializable]
public struct AnimationSettings
{
    [field: SerializeField] public Animator Animator { get; private set; }
}

