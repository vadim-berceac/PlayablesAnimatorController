using UnityEngine;
using UnityEngine.Animations.Rigging;
using System.Collections;

public class WeaponIKController : MonoBehaviour
{
    [field: SerializeField] private RigSetup[] RigSetups { get; set; }
    
    private RigSetup _currentRigSetup;
    private Coroutine _transitionCoroutine;
    private RigBuilder _rigBuilder;

    private void Awake()
    {
        _rigBuilder = GetComponentInParent<RigBuilder>();
        
        if (_rigBuilder == null)
        {
            Debug.LogError("RigBuilder не найден!");
            return;
        }

        foreach (var rigSetup in RigSetups)
        {
            if (rigSetup.Rig == null)
            {
               continue;
            }
            var rigIndex = _rigBuilder.layers.FindIndex(layer => layer.rig == rigSetup.Rig);
            if (rigIndex >= 0)
            {
                var layer = _rigBuilder.layers[rigIndex];
                layer.active = true; 
                _rigBuilder.layers[rigIndex] = layer;
            }
            rigSetup.Rig.weight = 0f;
        }
       
        _rigBuilder.Build();
    }

    public void EnableRig(AnimationSet animationSet, float inTime)
    {
        var targetRig = System.Array.Find(RigSetups, rig => rig.AnimationSet == animationSet);
        
        if (targetRig == null || targetRig.Rig == null)
        {
            Debug.LogWarning($"Риг для AnimationSet {animationSet} не найден!");
            return;
        }

        if (_currentRigSetup != null && _currentRigSetup.Rig == targetRig.Rig)
        {
            return;
        }

        if (_transitionCoroutine != null)
        {
            StopCoroutine(_transitionCoroutine);
        }

        _transitionCoroutine = StartCoroutine(TransitionRig(_currentRigSetup, targetRig, inTime));
        _currentRigSetup = targetRig;
    }

    public void DisableCurrentRig(float inTime)
    {
        if (_currentRigSetup == null || _currentRigSetup.Rig == null)
        {
            return;
        }

        if (_transitionCoroutine != null)
        {
            StopCoroutine(_transitionCoroutine);
        }

        _transitionCoroutine = StartCoroutine(TransitionToZero(_currentRigSetup.Rig, inTime));
        _currentRigSetup = null;
    }

    private IEnumerator TransitionRig(RigSetup fromRig, RigSetup toRig, float duration)
    {
        if (_rigBuilder == null) yield break;

        var elapsed = 0f;
        var fromWeight = (fromRig != null && fromRig.Rig != null) ? fromRig.Rig.weight : 0f;
        var toWeight = toRig.Rig.weight;

        if (duration <= 0f)
        {
            if (fromRig != null && fromRig.Rig != null) 
            {
                fromRig.Rig.weight = 0f;
            }
            toRig.Rig.weight = 1f;
            _rigBuilder.Build(); 
            _transitionCoroutine = null;
            yield break;
        }

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            var t = Mathf.Clamp01(elapsed / duration);

            if (fromRig != null && fromRig.Rig != null)
            {
                fromRig.Rig.weight = Mathf.Lerp(fromWeight, 0f, t);
            }

            toRig.Rig.weight = Mathf.Lerp(toWeight, 1f, t);
            
            _rigBuilder.Build();

            yield return null;
        }

        if (fromRig != null && fromRig.Rig != null)
        {
            fromRig.Rig.weight = 0f;
        }
        toRig.Rig.weight = 1f;
        
        _rigBuilder.Build();
        _transitionCoroutine = null;
    }

    private IEnumerator TransitionToZero(Rig rig, float duration)
    {
        if (_rigBuilder == null) yield break;

        var elapsed = 0f;
        var startWeight = rig.weight;

        if (duration <= 0f)
        {
            rig.weight = 0f;
            _rigBuilder.Build();
            _transitionCoroutine = null;
            yield break;
        }

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            var t = Mathf.Clamp01(elapsed / duration);

            rig.weight = Mathf.Lerp(startWeight, 0f, t);
           
            _rigBuilder.Build();

            yield return null;
        }

        rig.weight = 0f;
        _rigBuilder.Build();
        _transitionCoroutine = null;
    }
}

[System.Serializable]
public class RigSetup
{
    [field: SerializeField] public Rig Rig { get; private set; }
    [field: SerializeField] public AnimationSet AnimationSet { get; private set; }
}