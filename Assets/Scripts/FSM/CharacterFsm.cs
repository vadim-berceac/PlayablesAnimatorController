using UnityEngine;
using Zenject;

public class CharacterFsm : FSMAbstract
{ 
   [field: SerializeField] public bool IsPlayerControlled { get; set; }
   [field: SerializeField] public AnimationSettings AnimationSettings { get; set; }

   [Inject]
   private void Construct(StatesContainer statesContainer, PlayerInput playerInput)
   {
       StatesContainer = statesContainer;
       AnimatorController = new PlayablesAnimatorController(AnimationSettings.Animator);
       StatesTransition = new StatesTransition(this);
       StatesTimer = new StatesTimer();

       if (IsPlayerControlled)
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

[System.Serializable]
public struct AnimationSettings
{
    [field: SerializeField] public Animator Animator { get; private set; }
}
