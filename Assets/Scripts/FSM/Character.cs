using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Character : MonoBehaviour
{
    [field: SerializeField] public bool IsPlayerControlled { get; set; }
    [field: SerializeField] public AnimationSettings AnimationSettings { get; set; }
   
    public Fsm FullBodyFsm { get; private set; }
    public Fsm UpperBodyFsm { get; private set; }
    public GraphCore GraphCore { get; private set; }
    public StatesContainer StatesContainer { get; private set; }
    public InputHandler InputHandler { get; private set; }
    
    private StatesTransition _statesTransition;

    [Inject]
    private void Construct(StatesContainer statesContainer, PlayerInput playerInput)
    {
        if (IsPlayerControlled)
        {
            InputHandler = new InputHandler(playerInput);
        }
        
        StatesContainer = statesContainer;
        GraphCore = new GraphCore(AnimationSettings.Animator, 3);
        
        FullBodyFsm = new Fsm(this, 0, SetType.FullBody);
        GraphCore.SetUpLayer(0, StatesContainer.GetAvatarMaskBySetType(SetType.FullBody), false);
        GraphCore.SetLayerWeight(0,1);
        
        _statesTransition = new StatesTransition(FullBodyFsm);
        FullBodyFsm.SetStatesTransition(_statesTransition);
        
        //тестовая стейт машина для верха тела
        UpperBodyFsm = new Fsm(this, 1, SetType.UpperBody);
        GraphCore.SetUpLayer(1, StatesContainer.GetAvatarMaskBySetType(SetType.UpperBody), false);
        var layerConfigs = new List<(int graphPortIndex, int outputPortIndex, AvatarMask mask, bool isAdditive)>
        {
            (1, 0, StatesContainer.GetAvatarMaskBySetType(SetType.UpperBody), true),
            (2, 1, StatesContainer.GetAvatarMaskBySetType(SetType.Hands), false)
        };
        UpperBodyFsm.ConnectToMultipleLayers(layerConfigs);
        GraphCore.SetLayerWeight(1,1);
        UpperBodyFsm.SetStatesTransition(_statesTransition);
    }

    private void Update()
    {
        FullBodyFsm.Update();
        UpperBodyFsm.Update();
        _statesTransition.UpdateBlending();
    }

    private void FixedUpdate()
    {
        FullBodyFsm.FixedUpdate();
        UpperBodyFsm.FixedUpdate();
    }

    private void LateUpdate()
    {
        FullBodyFsm.LateUpdate();
        UpperBodyFsm.LateUpdate();
    }

    private void OnDestroy()
    {
       GraphCore.Dispose();
    }
}

[System.Serializable]
public struct AnimationSettings
{
    [field: SerializeField] public Animator Animator { get; private set; }
}

