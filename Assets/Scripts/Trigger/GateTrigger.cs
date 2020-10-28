using UnityEngine;

public class GateTrigger : Triggerable
{
    public Transform teleportationTarget;
    public override void OnTrigger()
    {
        base.OnTrigger();
        GameManager.Instance.player.transform.position = teleportationTarget.position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, teleportationTarget.position);
        Gizmos.color = Color.white;
    }
}
