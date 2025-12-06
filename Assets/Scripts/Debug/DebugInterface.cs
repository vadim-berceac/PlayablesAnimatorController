using UnityEngine;
using UnityEngine.UI;

public class DebugInterface : MonoBehaviour
{
   [field: SerializeField] private Character FSM { get; set; }
   [field: SerializeField] private Text prevStateText { get; set; }
   [field: SerializeField] private Text currentStateText { get; set; }
   [field: SerializeField] private Text animationSpeedText { get; set; }
   [field: SerializeField] private Text movementSpeedText { get; set; }
   
   [field: Space (5)]
   
   [field: SerializeField] private Text prevStateText0 { get; set; }
   [field: SerializeField] private Text currentStateText0 { get; set; }
   [field: SerializeField] private Text animationSpeedText0 { get; set; }
   [field: SerializeField] private Text movementSpeedText0 { get; set; }

   private void Awake()
   {
      FSM.FullBodyFsm.OnStateChanged += OnStateChanged;
      FSM.FullBodyFsm.StatesTransition.OnMovementSpeedChanged += OnMovementSpeedChanged;
      FSM.FullBodyFsm.StatesTransition.OnAnimationSpeedChanged += OnAnimationSpeedChanged;
      
      OnStateChanged(FSM.FullBodyFsm.PreviousState, FSM.FullBodyFsm.CurrentState);
      OnAnimationSpeedChanged(FSM.FullBodyFsm.StatesTransition.CurrentAnimationSpeed);
      OnMovementSpeedChanged(FSM.FullBodyFsm.StatesTransition.CurrentMovementSpeed);
      //
      FSM.UpperBodyFsm.OnStateChanged += OnStateChanged0;
      FSM.UpperBodyFsm.StatesTransition.OnMovementSpeedChanged += OnMovementSpeedChanged0;
      FSM.UpperBodyFsm.StatesTransition.OnAnimationSpeedChanged += OnAnimationSpeedChanged0;
      
      OnStateChanged0(FSM.UpperBodyFsm.PreviousState, FSM.UpperBodyFsm.CurrentState);
      OnAnimationSpeedChanged0(FSM.UpperBodyFsm.StatesTransition.CurrentAnimationSpeed);
      OnMovementSpeedChanged0(FSM.UpperBodyFsm.StatesTransition.CurrentMovementSpeed);
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
   
   private void OnStateChanged0(State previous, State current)
   {
      if (previous)
      {
         prevStateText0.text = previous.ToString();
      }

      if (current)
      {
         currentStateText0.text = current.ToString();
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
   
   private void OnMovementSpeedChanged0(float value)
   {
      movementSpeedText0.text = value.ToString();
   }

   private void OnAnimationSpeedChanged0(float value)
   {
      animationSpeedText0.text = value.ToString();
   }

   private void OnDisable()
   {
      FSM.FullBodyFsm.OnStateChanged -= OnStateChanged;
      FSM.FullBodyFsm.StatesTransition.OnMovementSpeedChanged -= OnMovementSpeedChanged;
      FSM.FullBodyFsm.StatesTransition.OnAnimationSpeedChanged -= OnAnimationSpeedChanged;
      
      //
      FSM.UpperBodyFsm.OnStateChanged -= OnStateChanged;
      FSM.UpperBodyFsm.StatesTransition.OnMovementSpeedChanged -= OnMovementSpeedChanged;
      FSM.UpperBodyFsm.StatesTransition.OnAnimationSpeedChanged -= OnAnimationSpeedChanged;
   }
}
