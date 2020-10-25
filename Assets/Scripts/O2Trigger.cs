using Plugins;
using UnityEngine;

public class O2Trigger : CacheBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!GameManager.Instance.PlayerAlive) return;
        if (!other.CompareTag("Player")) return;
        GameManager.Instance.oxygenAmount = 100;
        GameManager.Instance.UpdateOxygenDisplay();
    }
    private void OnDrawGizmos()
    {
        DebugExtension.DrawBounds(collider.bounds, Color.green);
    }
}
