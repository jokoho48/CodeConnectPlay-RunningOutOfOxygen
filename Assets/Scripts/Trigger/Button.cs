using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

public class Button : Triggerable
{
    [SerializeField] private Color activatedColor;
    // private Color startColor;
    [SerializeField] private Renderer triggerButton;
    [SerializeField] private float yOffset;
    public UnityEvent onTriggered;
    [ReadOnly] private bool _hasTriggered;
    [ColorUsage(false, true)]public Color wireActiveColor;

    public override void DoTrigger()
    {
        base.DoTrigger();
        if (!_hasTriggered)
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

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = activatedColor;
        var bounds = triggerButton.bounds;
        Gizmos.DrawWireCube(bounds.center - new Vector3(0,yOffset, 0), bounds.size);
        Gizmos.color = Color.white;
    }
}