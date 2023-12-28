using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocomotionEdge : MonoBehaviour
{
    [SerializeField] private LineRenderer line;

    public void SetPositions(Vector3[] points)
    {
        line.SetPositions(points);
    }
}
