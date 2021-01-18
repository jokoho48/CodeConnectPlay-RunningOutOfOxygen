using System;
using NaughtyAttributes;
using UnityEngine;

[ExecuteInEditMode]
public class MovementCloner : MonoBehaviour
{
    public Transform transformToCopy;
    public bool copyPosition;
    [ShowIf("copyPosition")] public Vector3 positionOffset;
    public bool copyScale;
    [ShowIf("copyScale")] public Vector3 scaleOffset;
    public bool copyRotation;
    [ShowIf("copyRotation")] public Vector3 rotationOffset;
    
    private void LateUpdate()
    {
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        if (!transformToCopy) return;
        if (copyPosition)
            transform.position = transformToCopy.position + positionOffset;
        if (copyScale)
            transform.localScale = transformToCopy.localScale + scaleOffset;
        if (copyRotation)
            transform.rotation = Quaternion.Euler(transformToCopy.rotation.eulerAngles + rotationOffset);
    }
}
