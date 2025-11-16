using UnityEngine;
using UnityEngine.UI;

public class DebugInterface : MonoBehaviour
{
   [field: SerializeField] private FSMAbstract FSM { get; set; }
   [field: SerializeField] private Text prevStateText { get; set; }
   [field: SerializeField] private Text currentStateText { get; set; }
   [field: SerializeField] private Text animationSpeedText { get; set; }
   [field: SerializeField] private Text movementSpeedText { get; set; }

   private void Awake()
   {
      FSM.OnStateChanged += OnStateChanged;
      FSM.StatesTransition.OnMovementSpeedChanged += OnMovementSpeedChanged;
      FSM.StatesTransition.OnAnimationSpeedChanged += OnAnimationSpeedChanged;
      
      OnStateChanged(FSM.PreviousState, FSM.CurrentState);
      OnAnimationSpeedChanged(FSM.StatesTransition.CurrentAnimationSpeed);
      OnMovementSpeedChanged(FSM.StatesTransition.CurrentMovementSpeed);
   }

   private void OnStateChanged(State previous, State current)
   {
      if (previous)
      {
         prevStateText.text = previous.ToString();
      }

      if (current)
      {
         currentStateText.text = current.ToString();
      }
   }

   private void OnMovementSpeedChanged(float value)
   {
      movementSpeedText.text = value.ToString();
   }

   private void OnAnimationSpeedChanged(float value)
   {
      animationSpeedText.text = value.ToString();
   }

   private void OnDisable()
   {
      FSM.OnStateChanged -= OnStateChanged;
      FSM.StatesTransition.OnMovementSpeedChanged -= OnMovementSpeedChanged;
      FSM.StatesTransition.OnAnimationSpeedChanged -= OnAnimationSpeedChanged;
   }
}
