using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Cinemachine;
using UnityEngine;
using Zenject;

[BurstCompile]
public class CharacterSelector : MonoBehaviour
{
   [field: SerializeField] private CharacterSelectorCameraSettings CameraSettings { get; set; }
   
   private readonly List<AIBrainInputModule> _inputBrainModules = new ();
   private AIBrainInputModule _selectedBrain;
   private ICharacterInput _playerInput;
   private const float Threshold = 0.01f;
   private float _targetYaw;
   private float _targetPitch;
   
   public static Action<AIBrainInputModule> OnCharacterSelected { get; set; }

   [Inject]
   private void Construct(PlayerInput playerInput)
   {
      _playerInput = playerInput;
   }

   private void LateUpdate()
   {
      CameraRotation();
   }
   
   private void CameraRotation()
   {
      if (_selectedBrain == null)
      {
         return;
      }

      if (_playerInput.Look.sqrMagnitude >= Threshold)
      {
         _targetYaw += _playerInput.Look.x * CameraSettings.HorizontalRotationSpeed;
         _targetPitch += _playerInput.Look.y * CameraSettings.VerticalRotationSpeed;
      }

      _targetYaw = ClampAngle(_targetYaw, float.MinValue, float.MaxValue);
      _targetPitch = ClampAngle(_targetPitch, CameraSettings.BottomClamp, CameraSettings.TopClamp);

      _selectedBrain.transform.rotation = Quaternion.Euler(
         _targetPitch + CameraSettings.CameraAngleOverride,
         _targetYaw,
         0.0f
      );
   }

   
   private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
   {
      if (lfAngle < -360f) lfAngle += 360f;
      if (lfAngle > 360f) lfAngle -= 360f;
      return Mathf.Clamp(lfAngle, lfMin, lfMax);
   }

   public List<AIBrainInputModule> GetInputBrainModules()
   {
      return _inputBrainModules;
   }

   public AIBrainInputModule GetSelectedBrain()
   {
      return _selectedBrain;
   }

   public void Connect(AIBrainInputModule brain)
   {
      if (_inputBrainModules.Contains(brain))
      {
         return;
      }
      _inputBrainModules.Add(brain);
   }

   public void Disconnect(AIBrainInputModule brain)
   {
      if (!_inputBrainModules.Contains(brain))
      {
         return;
      }
      _inputBrainModules.Remove(brain);
   }

   public void SelectByIndex(int characterIndex)
   {
      if (_inputBrainModules.Count <= characterIndex)
      {
         return;
      }

      DeselectCurrentBrain();

      CameraSettings.VirtualCamera.Follow = _inputBrainModules[characterIndex].transform;
      _targetYaw = _inputBrainModules[characterIndex].transform.rotation.eulerAngles.y;
      _inputBrainModules[characterIndex].Enable(false);
      _inputBrainModules[characterIndex].Character.SetInputMode(_playerInput);
      _selectedBrain = _inputBrainModules[characterIndex];
      
      OnCharacterSelected?.Invoke(_selectedBrain);
   }

   private void DeselectCurrentBrain()
   {
      if (_selectedBrain == null)
      {
         return;
      }
      
      CameraSettings.VirtualCamera.Follow = null;
      _selectedBrain.Enable(true);
      _selectedBrain.Character.SetInputMode(_selectedBrain);
      _selectedBrain = null;
   }
}

[System.Serializable]
public struct CharacterSelectorCameraSettings
{
   [field: SerializeField] public CinemachineCamera VirtualCamera { get; set; }
   [field: SerializeField, Range(0, 1)] public float HorizontalRotationSpeed { get; set; }
   [field: SerializeField, Range(0, 1)] public float VerticalRotationSpeed { get; set; }
   [field: SerializeField, Range(0, 90)] public float TopClamp { get; set; }
   [field: SerializeField, Range(-90, 0)] public float BottomClamp { get; set; }
   [field: SerializeField] public float CameraAngleOverride { get; set; }
}
