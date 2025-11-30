
using System.Collections.Generic;
using UnityEngine;

public class Fsm : FSMAbstract
{ 
    public Fsm(StatesContainer statesContainer, InputHandler inputHandler, GraphCore graphCore, int graphIndex, SetType setType)
    {
       SetType = setType;
       GraphCore = graphCore;
       StatesContainer = statesContainer;
       InputHandler = inputHandler;
       AnimatorController = new PlayablesAnimatorController(GraphCore, graphIndex);
       StatesTransition = new StatesTransition(this);
       StatesTimer = new StatesTimer();
       
       SwitchState(StateType.Idle);
    }

    public void ConnectToMultipleLayers(List<(int layerIndex, AvatarMask mask, bool isAdditive)> layerConfigs)
    {
        AnimatorController.ConnectToMultipleLayers(layerConfigs);
    }
}
