#region
using System.Collections.Generic;
using NaughtyAttributes;
using Plugins;
using UnityEngine;
#endregion

public class WirePhysics : MonoBehaviour
{
    [SerializeField] private float attractionForce;
    [SerializeField, ReorderableList] private Transform[] endPoints;
    [SerializeField] private int sections;
    [SerializeField] private int solveIteration;
    [SerializeField, ReorderableList] private Transform[] startPoints;
    private readonly List<LineRenderer> _lineRenderer = new List<LineRenderer>();

    [SerializeField] private GameObject lineRendererPrefab;
    [Button("Generate Line")]
    private void GenerateLineRendererSegments()
    {
        if (startPoints.Length != endPoints.Length) return;
        foreach (var lr in GetComponentsInChildren<LineRenderer>())
        {
            lr.gameObject.Destroy();
        }
        _lineRenderer.Clear();

        for (var index = 0; index < startPoints.Length; index++)
        {
            var lr = UnityExtensions.Instantiate(lineRendererPrefab, transform.position, Quaternion.identity, transform).GetComponent<LineRenderer>();
            Vector3[] points = new Vector3[sections];
            var startPoint = startPoints[index].position;
            var endPoint = endPoints[index].position;
            for (int i = 0; i < sections; i++)
                points[i] = Vector3.Lerp(startPoint, endPoint, (float) i / sections);

            lr.positionCount = sections;
            lr.SetPositions(points);
            _lineRenderer.Add(lr);
        }
        UpdateSimulation();
    }

    [Button("Simulate")]
    private void UpdateSimulation()
    {
        if (startPoints.Length != endPoints.Length) return;
        for (int i = 0; i < 10; i++)
        {
            SimulateSegmentPositions(.01f);
        }
    }

    private void Awake()
    {
        GenerateLineRendererSegments();
    }

    private void SimulateSegmentPositions(float deltaTime)
    {
        
        if (startPoints.Length != endPoints.Length) return;
        for (var index = 0; index < _lineRenderer.Count; index++)
        {
            var lr = _lineRenderer[index];
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