
public class Fsm : FSMAbstract
{ 
    public Fsm(StatesContainer statesContainer, PlayerInput playerInput,
        GraphCore graphCore, int graphIndex, bool isPlayerControlled)
   {
       GraphCore = graphCore;
       StatesContainer = statesContainer;
       AnimatorController = new PlayablesAnimatorController(GraphCore, graphIndex);
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
