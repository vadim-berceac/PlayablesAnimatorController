using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Character : MonoBehaviour
{
    [field: SerializeField] public bool IsPlayerControlled { get; set; }
    [field: SerializeField] public AnimationSettings AnimationSettings { get; set; }

    private StatesTransition _statesTransition;
    private GraphCore _graphCore;
    public Fsm FullBodyFsm { get; set; }
    public Fsm UpperBodyFsm { get; set; }
    private StatesContainer _statesContainer;
    private InputHandler _inputHandler;

    [Inject]
    private void Construct(StatesContainer statesContainer, PlayerInput playerInput)
    {
        if (IsPlayerControlled)
        {
            _inputHandler = new InputHandler(playerInput);
        }
        
        _statesContainer = statesContainer;
        
        _graphCore = new GraphCore(AnimationSettings.Animator, 3);
        FullBodyFsm = new Fsm(_statesContainer, _inputHandler, _graphCore, 0, SetType.FullBody);
        //UpperBodyFsm = new Fsm(_statesContainer, _inputHandler, _graphCore, 1, SetType.UpperBody);
        
        _graphCore.SetLayerWeight(0,1);
        _graphCore.SetUpLayer(0, _statesContainer.GetAvatarMaskBySetType(SetType.FullBody), false);
        
        //_graphCore.SetLayerWeight(1,1);
        //_graphCore.SetUpLayer(1, _statesContainer.GetAvatarMaskBySetType(SetType.UpperBody), false);
        
        // var layerConfigs = new List<(int layerIndex, AvatarMask mask, bool isAdditive)>
        // {
        //     (0, _statesContainer.GetAvatarMaskBySetType(SetType.UpperBody), false)
        // };
        // UpperBodyFsm.ConnectToMultipleLayers(layerConfigs);
        
        _statesTransition = new StatesTransition(FullBodyFsm);
        FullBodyFsm.SetStatesTransition(_statesTransition);
        //UpperBodyFsm.SetStatesTransition(_statesTransition);
    }

    private void Update()
    {
        FullBodyFsm.Update();
        _statesTransition.UpdateBlending();
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

