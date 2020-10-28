using Plugins;
using UnityEngine;

public class Trigger : CacheBehaviour
{
    [SerializeField] private Triggerable _triggerableObject;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _triggerableObject.OnTrigger();
        }
    }

    private void OnDrawGizmos()
    {
        DebugExtension.DrawBounds(collider.bounds, Color.green);
    }
}

public abstract class Triggerable : CacheBehaviour, ITriggerable
{
    public virtual void OnTrigger()
    {
    }
}
public interface ITriggerable
{
    void OnTrigger();
}