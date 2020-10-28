#region

using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

#endregion


[RequireComponent(typeof(LineRenderer))]
public class WirePhysics : MonoBehaviour
{
    [SerializeField] private float attractionForce;
    [SerializeField] private Transform[] endPoints;
    [NonSerialized] private LineRenderer[] _lrs;
    [SerializeField] private int sections;
    [SerializeField] private int solveIteration;
    [SerializeField] private Transform[] startPoints;

    [Button("Generate Line")]
    private void GenerateLineRendererSegments()
    {
        if (startPoints.Length != endPoints.Length || _lrs.Length != startPoints.Length) return;
        for (var index = 0; index < _lrs.Length; index++)
        {
            var lr = _lrs[index];
            Vector3[] points = new Vector3[sections];
            var startPoint = startPoints[index].position;
            var endPoint = endPoints[index].position;
            for (int i = 0; i < sections; i++)
                points[i] = Vector3.Lerp(startPoint, endPoint, (float) i / sections);

            lr.positionCount = sections;
            lr.SetPositions(points);
        }

        UpdateSimulation();
    }

    [Button("Simulate")]
    private void UpdateSimulation()
    {
        if (startPoints.Length != endPoints.Length || _lrs.Length != startPoints.Length) return;
        for (int i = 0; i < 3; i++)
        {
            SimulateSegmentPositions(.1f);
        }
    }

    private void FindLineRenderer()
    {
        var lrs = new List<LineRenderer>();
        lrs.AddRange(GetComponentsInChildren<LineRenderer>());
        _lrs = lrs.ToArray();
    }
    private void Awake()
    {
        FindLineRenderer();
        GenerateLineRendererSegments();
    }

    private void OnValidate()
    {
        FindLineRenderer();
        GenerateLineRendererSegments();
    }

    private void SimulateSegmentPositions(float deltaTime)
    {
        if (startPoints.Length != endPoints.Length || _lrs.Length != startPoints.Length) return;
        for (var index = 0; index < _lrs.Length; index++)
        {
            var lr = _lrs[index];
            Vector3[] points = new Vector3[lr.positionCount];
            lr.GetPositions(points);
            points[0] = startPoints[index].position;
            points[points.Length - 1] = endPoints[index].position;

            for (int numSolveIteration = 0; numSolveIteration < solveIteration; numSolveIteration++)
            {
                for (int i = 1; i < points.Length - 1; i++)
                {
                    Vector3 offsetToPrev = points[i - 1] - points[i];
                    Vector3 offsetToNext = points[i + 1] - points[i];
                    Vector3 velocity = offsetToPrev * attractionForce + offsetToNext * attractionForce;
                    points[i] += velocity * deltaTime / solveIteration;
                }

                for (int i = 1; i < points.Length - 1; i++) points[i] += Physics.gravity * deltaTime / solveIteration;
            }

            lr.SetPositions(points);
        }
    }

    private void FixedUpdate()
    {
        SimulateSegmentPositions(Time.fixedDeltaTime);
    }
}