#region

using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

#endregion


[RequireComponent(typeof(LineRenderer)), ExecuteInEditMode]
public class WirePhysics : MonoBehaviour
{
    [SerializeField] private float attractionForce;
    [SerializeField] private Transform endPoint;
    [SerializeField] private LineRenderer lr;
    [SerializeField] private int sections;
    [SerializeField] private int solveIteration;
    [SerializeField] private Transform startPoint;

    [Button("Generate Line")]
    private void GenereateLineRendererSegments()
    {
        if (!startPoint || !endPoint) return;
        Vector3[] points = new Vector3[sections];
        for (int i = 0; i < sections; i++)
            points[i] = Vector3.Lerp(startPoint.position, endPoint.position, (float) i / sections);

        lr.positionCount = sections;
        lr.SetPositions(points);
        SimulateSegmentPoitions(.1f);
    }

    private void OnValidate()
    {
        lr = GetComponent<LineRenderer>();
        GenereateLineRendererSegments();
    }

    private void SimulateSegmentPoitions(float deltaTime)
    {
        Vector3[] points = new Vector3[lr.positionCount];
        lr.GetPositions(points);
        points[0] = startPoint.position;
        points[points.Length - 1] = endPoint.position;

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

    private void FixedUpdate()
    {
        SimulateSegmentPoitions(Time.fixedDeltaTime);
    }
}