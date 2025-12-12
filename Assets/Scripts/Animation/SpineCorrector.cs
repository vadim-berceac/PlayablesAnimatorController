using UnityEngine;

public class SpineCorrector : MonoBehaviour
{
    [SerializeField] private Transform originalSpine;

    private Quaternion _rotationOffset = Quaternion.identity;
    private Quaternion _targetRotation;
    private Transform _transform;

    private void Awake()
    {
        if(originalSpine == null)
        {
           return;
        }
        
        _transform = transform;
        _rotationOffset = Quaternion.Inverse(_transform.rotation) * originalSpine.rotation;
    }

    private void LateUpdate()
    {
        if (originalSpine == null) return;

        _rotationOffset = Quaternion.Inverse(_transform.rotation) * originalSpine.rotation;
        _targetRotation = Quaternion.Euler(0f, _rotationOffset.eulerAngles.y, 0f);
        originalSpine.rotation = _transform.rotation * _targetRotation;
    }
}