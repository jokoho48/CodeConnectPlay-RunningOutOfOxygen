using Plugins;
using UnityEngine;

public class Trigger : CacheBehaviour
{
    [SerializeField] private Triggerable _triggerableObject;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _triggerableObject.DoTrigger();
        }
    }

    private void OnDrawGizmos()
    {
        DebugExtension.DrawBounds(collider.bounds, Color.green);
    }
}

public abstract class Triggerable : CacheBehaviour
{
    public virtual void DoTrigger()
    {
    }
}