using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class MovablePlatform : MonoBehaviour
{
    public Vector3 moveOffset;
    public float duration;
    public Ease ease;
    public void DoMove()
    {
        transform.DOMove(transform.position + moveOffset, duration).SetEase(ease);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        var position = transform.position;
        var targetPosition = position + moveOffset;
        Gizmos.DrawLine(position, targetPosition);
        Gizmos.color = Color.white;
        DebugExtension.DrawPoint(targetPosition, Color.red, 0.5f);
    }
}
