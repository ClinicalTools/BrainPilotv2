using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierLineRenderer : MonoBehaviour {

    public int numPoints = 100;
    public bool isActive = false;

    public Transform origin;
    public Transform destination;
    public float controlPointDistance = 3f;

    public LineRenderer line;

    private void Start()
    {
        isActive = false;
    }

    public void SetActive(bool status)
    {
        isActive = status;
    }

    private void Update()
    {
        if (isActive && origin != null && destination != null)
        {
            UpdateLineRenderer(CalculatePath());
        }
    }

    private void UpdateLineRenderer(Vector3[] points)
    {
        if (line == null)
        {
            line = GetComponent<LineRenderer>();
        }
        line.positionCount = numPoints;
        line.SetPositions(points);
    }

    private Vector3[] CalculatePath()
    {
        Vector3[] path = new Vector3[numPoints];

        for (int i = 0; i < numPoints; i++)
        {
            float t = (float)i / (float)numPoints;

            path[i] = BezierPathCalculation(origin.position,
                    OffsetPositionByDirection(origin.position, origin.forward, controlPointDistance),
                    OffsetPositionByDirection(destination.position, destination.forward, controlPointDistance),
                    destination.position, t);
            //Debug.Log(path[i]);
        }

        return path;
    }

    Vector3 OffsetPositionByDirection(Vector3 position, Vector3 direction, float amount)
    {
        return position + direction.normalized * amount;
    }

    Vector3 BezierPathCalculation(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float tt = t * t;
        float ttt = t * tt;
        float u = 1.0f - t;
        float uu = u * u;
        float uuu = u * uu;

        Vector3 B = new Vector3();
        B = uuu * p0;
        B += 3.0f * uu * t * p1;
        B += 3.0f * u * tt * p2;
        B += ttt * p3;

        return B;
    }


}
