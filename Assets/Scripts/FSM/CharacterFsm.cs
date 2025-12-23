
using System.Collections.Generic;
using UnityEngine;

public class Fsm : FSMAbstract
{
    public Fsm (Character character, int graphPortIndex, SetType setType)
    {
       SetType = setType;
       Character = character;
       GraphCore = character.GraphCore;
       StatesContainer = character.StatesContainer;
       InputHandler = character.InputHandler;
       AnimatorController = new PlayablesAnimatorController(GraphCore, graphPortIndex);
       StatesTimer = new StatesTimer();
       
       SwitchState(character.StatesContainer.GetStartStateType(SetType));
    }

    public void ConnectToMultipleLayers(List<(int graphPortIndex, int outputPortIndex, AvatarMask mask, bool isAdditive, float weght)> layerConfigs)
    {
        AnimatorController.ConnectToMultipleLayers(layerConfigs);
    }

    public void SetAvatarMask(List<(int layerIndex, AvatarMask avatarMask, bool isAdditive)> layerConfigs)
    {
        AnimatorController.SetAvatarMask(layerConfigs);
    }
}
