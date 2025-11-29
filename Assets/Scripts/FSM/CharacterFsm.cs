using UnityEngine;

public class Fsm : FSMAbstract
{ 
    public Fsm(StatesContainer statesContainer, PlayerInput playerInput,
        Animator animator, bool isPlayerControlled)
   {
       StatesContainer = statesContainer;
       AnimatorController = new PlayablesAnimatorController(animator);
       StatesTransition = new StatesTransition(this);
       StatesTimer = new StatesTimer();

       if (isPlayerControlled)
       {
           InputHandler = new InputHandler(playerInput);
       }
       else
       {
           //InputHandler = new InputHandler(botInput);
       }
       
       SwitchState(StateType.Idle);
   }
}
