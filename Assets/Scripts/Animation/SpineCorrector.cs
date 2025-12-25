using UnityEngine;

public class SpineCorrector : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    
    [Header("Correction Settings")]
    [SerializeField] [Range(0f, 1f)] private float yawCorrectionStrength = 0.5f;
    [SerializeField] [Range(0f, 1f)] private float pitchCorrectionStrength = 0.3f;
    [SerializeField] [Range(0f, 1f)] private float rollCorrectionStrength = 0.2f;
    
    [SerializeField] private float yawAngleThreshold = 10f;
    [SerializeField] private float pitchAngleThreshold = 15f;
    [SerializeField] private float rollAngleThreshold = 15f;
    
    [SerializeField] private float smoothTime = 0.1f;
    [SerializeField] private bool enableCorrection = true;
    
    private Transform _spine;
    private Transform _hips;
    
    private float _currentYaw;
    private float _currentPitch;
    private float _currentRoll;
    
    private float _yawVelocity;
    private float _pitchVelocity;
    private float _rollVelocity;
    
    private Vector3 _lastHipsForward;
    private Vector3 _lastHipsUp;
    private float _stableTimer;
    private const float STABLE_TIME_REQUIRED = 0.05f;
    
    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
            
        _spine = animator.GetBoneTransform(HumanBodyBones.Spine);
        _hips = animator.GetBoneTransform(HumanBodyBones.Hips);
        
        if (_hips == null)
        {
            return;
        }
        _lastHipsForward = _hips.forward;
        _lastHipsUp = _hips.up;
    }
    
    private void LateUpdate()
    {
        if (!enableCorrection || _spine == null || _hips == null)
            return;
            
        ApplySmoothCorrection();
    }
    
    private void ApplySmoothCorrection()
    {
        var hipsForwardMovement = Vector3.Angle(_lastHipsForward, _hips.forward);
        var hipsUpMovement = Vector3.Angle(_lastHipsUp, _hips.up);
        
        if (hipsForwardMovement > 1f || hipsUpMovement > 1f)
        {
            _stableTimer = 0f;
        }
        else
        {
            _stableTimer += Time.deltaTime;
        }
        
        _lastHipsForward = _hips.forward;
        _lastHipsUp = _hips.up;
        
        var targetYaw = CalculateYawDifference();
        var targetPitch = CalculatePitchDifference();
        var targetRoll = CalculateRollDifference();
        
        _currentYaw = Mathf.SmoothDampAngle(_currentYaw, targetYaw, ref _yawVelocity, smoothTime);
        _currentPitch = Mathf.SmoothDampAngle(_currentPitch, targetPitch, ref _pitchVelocity, smoothTime);
        _currentRoll = Mathf.SmoothDampAngle(_currentRoll, targetRoll, ref _rollVelocity, smoothTime);
      
        if (_stableTimer > STABLE_TIME_REQUIRED &&
            Mathf.Abs(_yawVelocity) < 50f &&
            Mathf.Abs(_pitchVelocity) < 50f &&
            Mathf.Abs(_rollVelocity) < 50f)
        {
            ApplyCorrectionRotation();
        }
    }
    
    private float CalculateYawDifference()
    {
        var hipsForwardFlat = Vector3.ProjectOnPlane(_hips.forward, Vector3.up).normalized;
        var spineForwardFlat = Vector3.ProjectOnPlane(_spine.forward, Vector3.up).normalized;
        
        return Vector3.SignedAngle(hipsForwardFlat, spineForwardFlat, Vector3.up);
    }
    
    private float CalculatePitchDifference()
    {
        var hipsRight = _hips.right;
        
        var hipsForwardProjected = Vector3.ProjectOnPlane(_hips.forward, hipsRight).normalized;
        var spineForwardProjected = Vector3.ProjectOnPlane(_spine.forward, hipsRight).normalized;
        
        return Vector3.SignedAngle(hipsForwardProjected, spineForwardProjected, hipsRight);
    }
    
    private float CalculateRollDifference()
    {
        var hipsForward = _hips.forward;
       
        var hipsUpProjected = Vector3.ProjectOnPlane(_hips.up, hipsForward).normalized;
        var spineUpProjected = Vector3.ProjectOnPlane(_spine.up, hipsForward).normalized;
        
        return Vector3.SignedAngle(hipsUpProjected, spineUpProjected, hipsForward);
    }
    
    private void ApplyCorrectionRotation()
    {
        var correction = Quaternion.identity;
        
        if (Mathf.Abs(_currentYaw) > yawAngleThreshold)
        {
            var normalizedYaw = Mathf.InverseLerp(yawAngleThreshold, 45f, Mathf.Abs(_currentYaw));
            var yawStrength = normalizedYaw * yawCorrectionStrength;
            correction *= Quaternion.AngleAxis(-_currentYaw * yawStrength, Vector3.up);
        }
        
        if (Mathf.Abs(_currentPitch) > pitchAngleThreshold)
        {
            var normalizedPitch = Mathf.InverseLerp(pitchAngleThreshold, 45f, Mathf.Abs(_currentPitch));
            var pitchStrength = normalizedPitch * pitchCorrectionStrength;
            var pitchAxis = _hips.right;
            correction *= Quaternion.AngleAxis(-_currentPitch * pitchStrength, pitchAxis);
        }
        
        if (Mathf.Abs(_currentRoll) > rollAngleThreshold)
        {
            var normalizedRoll = Mathf.InverseLerp(rollAngleThreshold, 45f, Mathf.Abs(_currentRoll));
            var rollStrength = normalizedRoll * rollCorrectionStrength;
           
            var rollAxis = _hips.forward;
            correction *= Quaternion.AngleAxis(-_currentRoll * rollStrength, rollAxis);
        }
        
        var targetRotation = correction * _spine.rotation;
        _spine.rotation = Quaternion.Slerp(_spine.rotation, targetRotation, Time.deltaTime / smoothTime);
    }
}