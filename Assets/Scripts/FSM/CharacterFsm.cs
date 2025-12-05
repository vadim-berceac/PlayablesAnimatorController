
using System.Collections.Generic;
using UnityEngine;

public class Fsm : FSMAbstract
{ 
    public Fsm(StatesContainer statesContainer, InputHandler inputHandler, GraphCore graphCore, int graphPortIndex, SetType setType)
    {
       SetType = setType;
       GraphCore = graphCore;
       StatesContainer = statesContainer;
       InputHandler = inputHandler;
       AnimatorController = new PlayablesAnimatorController(GraphCore, graphPortIndex);
       StatesTimer = new StatesTimer();
       
       SwitchState(statesContainer.GetStartStateType(SetType));
    }

    public void ConnectToMultipleLayers(List<(int graphPortIndex, int outputPortIndex, AvatarMask mask, bool isAdditive)> layerConfigs)
    {
        AnimatorController.ConnectToMultipleLayers(layerConfigs);
    }
}
