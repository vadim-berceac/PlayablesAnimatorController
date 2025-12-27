using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Character : MonoBehaviour
{ 
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
        SetInputMode(CharacterSettings.BrainInputModule);
        
        StatesContainer = statesContainer;
        AvatarMasksContainer = avatarMasksContainer;
        GraphCore = new GraphCore(CharacterSettings.Animator, 3);
        
        FullBodyFsm = new Fsm(this, 0, SetType.FullBody);
        var fullSmConfigs = new List<(int graphPortIndex, int outputPortIndex, AvatarMask mask, bool isAdditive, float weight)>
            {
                (0, 0, AvatarMasksContainer.GetMask(AvatarMaskType.FullBody), false, 1f)
            };
        FullBodyFsm.ConnectToMultipleLayers(fullSmConfigs);
        FullBodyFsm.SetStatesTransition(_statesTransition = new StatesTransition(FullBodyFsm));
        
        UpperBodyFsm = new Fsm(this, 1, SetType.UpperBody);
        var upperSmConfigs = new List<(int graphPortIndex, int outputPortIndex, AvatarMask mask, bool isAdditive, float weight)>
            {
               (1, 0, AvatarMasksContainer.GetMask(AvatarMaskType.UpperBody), true, 1f),
               (2, 1, AvatarMasksContainer.GetMask(AvatarMaskType.BothHands), false, 1f)
            };
        UpperBodyFsm.ConnectToMultipleLayers(upperSmConfigs);
        UpperBodyFsm.SetStatesTransition(_statesTransition);
    }
    
    public void SetInputMode(ICharacterInput input)
    {
        //IsAI = input is AIBrainInputModule;
        InputHandler = new InputHandler(input);
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
    [field: SerializeField] public AIBrainInputModule BrainInputModule { get; set; }
    [field: SerializeField] public Animator Animator { get; private set; }
    [field: SerializeField] public Inventory Inventory { get; set; }
    [field: SerializeField] public WeaponIKController WeaponIKController { get; set; }
}

