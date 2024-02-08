using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointDistributor : MonoBehaviour
{
    public int numberOfPoints = 50;
    public float radius = 10f;
    private List<Vector3> points;

    void Start()
    {
        GeneratePoints();
    }

    void GeneratePoints()
    {
        points = new List<Vector3>();

        for (int i = 0; i < numberOfPoints; i++)
        {
            points.Add(Random.onUnitSphere * radius);
        }
    }

    void OnDrawGizmos()
    {
        if (points == null) return;

        Gizmos.color = Color.white;
        foreach (Vector3 point in points)
        {
            Gizmos.DrawSphere(point, 0.1f); // Draws a small sphere with a radius of 0.5 at each point
        }
    }
}
