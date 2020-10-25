using DG.Tweening;
using NaughtyAttributes;
using Plugins;
using UnityEngine;
using UnityEngine.Events;

public class Trigger : CacheBehaviour
{
    [SerializeField] private Color activatedColor;
    // private Color startColor;
    [SerializeField] private Renderer triggerButton;
    [SerializeField] private float yOffset;
    public UnityEvent onTriggered;
    [ReadOnly] private bool _hasTriggered;

    public void Start()
    {
        // startColor = triggerButton.material.color;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_hasTriggered)
        {
            triggerButton.material.DOColor(activatedColor, 0.25f);
            triggerButton.transform.DOMoveY(triggerButton.transform.position.y - yOffset, 0.25f);
            onTriggered.Invoke();
            _hasTriggered = true;
        }
    }

    /*
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && _hasTriggered)
        {
            triggerButton.material.DOColor(startColor, 0.25f);
            
            triggerButton.transform.DOMoveY(triggerButton.transform.position.y + yOffset, 0.25f);
            // onTriggered.Invoke();
            _hasTriggered = false;
        }
    }
    */
    private void OnDrawGizmos()
    {
        DebugExtension.DrawBounds(collider.bounds, Color.green);
        Bounds bounds = triggerButton.bounds;
        bounds.center = bounds.center + new Vector3(0, -yOffset, 0);
        DebugExtension.DrawBounds(bounds, Color.red);
    }
}
