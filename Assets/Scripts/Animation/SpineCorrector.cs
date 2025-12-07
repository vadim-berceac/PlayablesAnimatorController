using UnityEngine;

public class SpineCorrector : MonoBehaviour
{
    [SerializeField] private Transform originalSpine;

    private Quaternion _rotationOffset = Quaternion.identity;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if(originalSpine != null)
        {
           return;
        }
        var parent = transform.parent;
        if(parent == null)
        {
           return;
        }
        var hips = parent.Find("B-hips");
        if(hips == null)
        {
           return;
        }
        var spine = hips.Find("B-spine");
        if(spine != null)
        {
            originalSpine = spine;
        }
    }  
#endif
    private void Awake()
    {
        if(originalSpine != null)
        {
            _rotationOffset = Quaternion.Inverse(transform.rotation) * originalSpine.rotation;
        }
    }

    private void LateUpdate()
    {
        if(originalSpine == null)
        {
            return;
        }
        originalSpine.rotation = transform.rotation * _rotationOffset;
    }
}