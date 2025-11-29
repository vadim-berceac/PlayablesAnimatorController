using UnityEngine;
using UnityEngine.UI;

public class DebugInterface : MonoBehaviour
{
   [field: SerializeField] private Character FSM { get; set; }
   [field: SerializeField] private Text prevStateText { get; set; }
   [field: SerializeField] private Text currentStateText { get; set; }
   [field: SerializeField] private Text animationSpeedText { get; set; }
   [field: SerializeField] private Text movementSpeedText { get; set; }

   private void Awake()
   {
      FSM.FullBodyFsm.OnStateChanged += OnStateChanged;
      FSM.FullBodyFsm.StatesTransition.OnMovementSpeedChanged += OnMovementSpeedChanged;
      FSM.FullBodyFsm.StatesTransition.OnAnimationSpeedChanged += OnAnimationSpeedChanged;
      
      OnStateChanged(FSM.FullBodyFsm.PreviousState, FSM.FullBodyFsm.CurrentState);
      OnAnimationSpeedChanged(FSM.FullBodyFsm.StatesTransition.CurrentAnimationSpeed);
      OnMovementSpeedChanged(FSM.FullBodyFsm.StatesTransition.CurrentMovementSpeed);
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
      FSM.FullBodyFsm.OnStateChanged -= OnStateChanged;
      FSM.FullBodyFsm.StatesTransition.OnMovementSpeedChanged -= OnMovementSpeedChanged;
      FSM.FullBodyFsm.StatesTransition.OnAnimationSpeedChanged -= OnAnimationSpeedChanged;
   }
}
