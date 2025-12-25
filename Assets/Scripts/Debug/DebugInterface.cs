using UnityEngine;
using UnityEngine.UI;

public class DebugInterface : MonoBehaviour
{
   private Character _character;
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
      CharacterSelector.OnCharacterSelected += OnCharacterChanged;
   }
   
   private void OnCharacterChanged(AIBrainInputModule brain)
   {
      Subscribe(brain.Character);
   }
   
   private void Subscribe (Character character)
   {
      Unsubscribe();
      
      _character = character;
      
      _character.FullBodyFsm.OnStateChanged += OnStateChanged;
      _character.FullBodyFsm.StatesTransition.OnMovementSpeedChanged += OnMovementSpeedChanged;
      _character.FullBodyFsm.StatesTransition.OnAnimationSpeedChanged += OnAnimationSpeedChanged;
      
      OnStateChanged(_character.FullBodyFsm.PreviousState, _character.FullBodyFsm.CurrentState);
      OnAnimationSpeedChanged(_character.FullBodyFsm.StatesTransition.CurrentAnimationSpeed);
      OnMovementSpeedChanged(_character.FullBodyFsm.StatesTransition.CurrentMovementSpeed);
      //
      _character.UpperBodyFsm.OnStateChanged += OnStateChanged0;
      _character.UpperBodyFsm.StatesTransition.OnMovementSpeedChanged += OnMovementSpeedChanged0;
      _character.UpperBodyFsm.StatesTransition.OnAnimationSpeedChanged += OnAnimationSpeedChanged0;
      
      OnStateChanged0(_character.UpperBodyFsm.PreviousState, _character.UpperBodyFsm.CurrentState);
      OnAnimationSpeedChanged0(_character.UpperBodyFsm.StatesTransition.CurrentAnimationSpeed);
      OnMovementSpeedChanged0(_character.UpperBodyFsm.StatesTransition.CurrentMovementSpeed);
   }

   private void Unsubscribe()
   {
      if (_character == null)
      {
         return;
      }
      _character.FullBodyFsm.OnStateChanged -= OnStateChanged;
      _character.FullBodyFsm.StatesTransition.OnMovementSpeedChanged -= OnMovementSpeedChanged;
      _character.FullBodyFsm.StatesTransition.OnAnimationSpeedChanged -= OnAnimationSpeedChanged;
      
      //
      _character.UpperBodyFsm.OnStateChanged -= OnStateChanged;
      _character.UpperBodyFsm.StatesTransition.OnMovementSpeedChanged -= OnMovementSpeedChanged;
      _character.UpperBodyFsm.StatesTransition.OnAnimationSpeedChanged -= OnAnimationSpeedChanged;
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
      CharacterSelector.OnCharacterSelected -= OnCharacterChanged;
      Unsubscribe();
   }
}
