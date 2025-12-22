using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Character : MonoBehaviour
{
    [field: SerializeField] public bool IsPlayerControlled { get; set; }
    [field: SerializeField] public CharacterSettings CharacterSettings { get; set; }
   
    public Inventory Inventory => CharacterSettings.Inventory;
    public Fsm FullBodyFsm { get; private set; }
    public Fsm UpperBodyFsm { get; private set; }
    public GraphCore GraphCore { get; private set; }
    public StatesContainer StatesContainer { get; private set; }
    public AvatarMasksContainer AvatarMasksContainer { get; private set; }
    public InputHandler InputHandler { get; private set; }
    
    private StatesTransition _statesTransition;

    [Inject]
    private void Construct(StatesContainer statesContainer, AvatarMasksContainer avatarMasksContainer, PlayerInput playerInput)
    {
        if (IsPlayerControlled)
        {
            InputHandler = new InputHandler(playerInput);
        }
        
        StatesContainer = statesContainer;
        AvatarMasksContainer = avatarMasksContainer;
        GraphCore = new GraphCore(CharacterSettings.Animator, 3);
        
        FullBodyFsm = new Fsm(this, 0, SetType.FullBody);
        GraphCore.SetUpLayer(0, AvatarMasksContainer.GetMask(AvatarMaskType.FullBody), false);
        GraphCore.SetLayerWeight(0,1);
        FullBodyFsm.SetStatesTransition(_statesTransition = new StatesTransition(FullBodyFsm));
        
        //тестовая стейт машина для верха тела
        UpperBodyFsm = new Fsm(this, 1, SetType.UpperBody);
        GraphCore.SetUpLayer(1, AvatarMasksContainer.GetMask(AvatarMaskType.UpperBody), false);
        var layerConfigs = new List<(int graphPortIndex, int outputPortIndex, AvatarMask mask, bool isAdditive, float weight)>
        {
            (1, 0, AvatarMasksContainer.GetMask(AvatarMaskType.UpperBody), true, 1f),
            (2, 1, AvatarMasksContainer.GetMask(AvatarMaskType.BothHands), false, 1f)
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
public struct CharacterSettings
{
    [field: SerializeField] public Animator Animator { get; private set; }
    [field: SerializeField] public Inventory Inventory { get; set; }
}

