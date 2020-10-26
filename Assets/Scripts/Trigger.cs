using DG.Tweening;
using NaughtyAttributes;
using Plugins;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.TextCore.LowLevel;

public class Trigger : CacheBehaviour
{
    [SerializeField] private Color activatedColor;
    // private Color startColor;
    [SerializeField] private Renderer triggerButton;
    [SerializeField] private float yOffset;
    public UnityEvent onTriggered;
    [ReadOnly] private bool _hasTriggered;
    [ColorUsage(false, true)]public Color wireActiveColor;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_hasTriggered)
        {
            triggerButton.material.DOColor(activatedColor, 0.25f);
            triggerButton.transform.DOMoveY(triggerButton.transform.position.y - yOffset, 0.25f);
            onTriggered.Invoke();
            _hasTriggered = true;
            var lr = GetComponent<LineRenderer>();
            if (lr)
            {
                lr.material.DOColor(wireActiveColor, "_EmissionColor",0.25f);
                foreach (var lrs in GetComponentsInChildren<LineRenderer>())
                {
                    lrs.material.DOColor(wireActiveColor, "_EmissionColor",0.25f);
                }
            }
        }
    }
    
    private void OnDrawGizmos()
    {
        DebugExtension.DrawBounds(collider.bounds, Color.green);
        Bounds bounds = triggerButton.bounds;
        bounds.center = bounds.center + new Vector3(0, -yOffset, 0);
        DebugExtension.DrawBounds(bounds, Color.red);
    }
}
