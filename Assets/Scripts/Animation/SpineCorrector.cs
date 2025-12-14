using UnityEngine;

public class SpineCorrector : MonoBehaviour
{
    [SerializeField] private Transform originalSpine;
    [SerializeField] private Animator animator;
    
    private Transform _spine;
    private Transform _hips;
    private Transform _transform;
    
    private Vector3 _proxyForward;
    private Vector3 _hipsForward;
    private Vector3 _mixedForward;

    private float _angle;
    private float _t;

    private void Awake()
    {
        _spine = animator.GetBoneTransform(HumanBodyBones.Spine);
        _hips = animator.GetBoneTransform(HumanBodyBones.Hips);
        _transform = transform;
    }

    private void LateUpdate()
    {
        CorrectMixed();
    }

    private void CorrectMixed()
    {
        _proxyForward = _transform.forward;
        _hipsForward = _hips.forward;

        _angle = Vector3.Angle(_hipsForward, _proxyForward);

        _t = Mathf.InverseLerp(0f, 90f, _angle);

        _mixedForward = Vector3.Slerp(_proxyForward, _hipsForward, _t);

        _spine.rotation = Quaternion.LookRotation(_mixedForward, _spine.up);
    }
}