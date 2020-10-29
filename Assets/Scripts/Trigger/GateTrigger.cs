using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class GateTrigger : Triggerable
{
    public Transform teleportationTarget;
    public bool isActive = true;
    private static readonly int MaskMultiplier = Shader.PropertyToID("_MaskMultiplier");
    private Material _material;
    [SerializeField] private Light gateLight;
    [SerializeField] private Renderer gateRenderer;
    private void Awake()
    {
        _material = gateRenderer.material;
        if (!isActive)
        {
            gateLight.intensity = 0;
            _material.SetFloat(MaskMultiplier, 0);
        }
    }

    public override void DoTrigger()
    {
        base.DoTrigger();
        if (!isActive) return;
        GameManager.Instance.StepFinishActions.Enqueue(DoTeleport);
    }

    private void DoTeleport()
    {
        var player = GameManager.Instance.player.transform;
        var target = teleportationTarget.position;
        // We dont want to change the Y position so we just copy it from the current player y
        target = new Vector3(target.x, player.position.y, target.z);
        Sequence sequence = DOTween.Sequence();
        sequence.Append(player.transform.DOScale(Vector3.zero, 0.25f).SetEase(GameManager.Instance.scaleEase));
        sequence.AppendCallback(() => player.transform.position = target);
        sequence.Append(player.transform.DOScale(Vector3.one, 0.25f).SetEase(GameManager.Instance.scaleEase));
        sequence.OnComplete(GameManager.Instance.StepFinished);
    }

    public void DoActivate()
    {
        _material.DOFloat(1, MaskMultiplier, 2f).SetEase(GameManager.Instance.scaleEase);
        gateLight.DOIntensity(0.5f, 2f);
        isActive = true;
    }
    
    private void OnDrawGizmos()
    {
        if (!teleportationTarget) return;
        Gizmos.color = Color.blue;
        var position = teleportationTarget.position;
        Gizmos.DrawLine(transform.position, position);
        DebugExtension.DrawPoint(position, Color.blue);
        Gizmos.color = Color.white;
    }
}