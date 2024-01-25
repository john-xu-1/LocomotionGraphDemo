using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocomotionEdge : MonoBehaviour
{
    [SerializeField] private LineRenderer line;
    int sourceNodeID, sinkNodeID;
    LocomotionEdgeHandler locomotionEdgeHandler;

    public void SetEdgeHandler(LocomotionEdgeHandler locomotionEdgeHandler)
    {
        this.locomotionEdgeHandler = locomotionEdgeHandler;
    }

    public void SetPositions(Vector3[] points)
    {
        line.SetPositions(points);
        List<Vector2> twoDPoints = new List<Vector2>();
        foreach(Vector3 point in points)
        {
            twoDPoints.Add(new Vector2(point.x, point.y));
        }
        GetComponent<EdgeCollider2D>().SetPoints(twoDPoints);
    }

    public void SetNodeIDs(int sourceNodeID, int sinkNodeID)
    {
        this.sourceNodeID = sourceNodeID;
        this.sinkNodeID = sinkNodeID;
    }

    //private void OnMouseEnter()
    //{
    //    locomotionEdgeHandler.EdgeHovering(2);
    //}

    //private void OnMouseExit()
    //{
    //    locomotionEdgeHandler.EdgeHovering(-2);
    //}

    private void OnMouseOver()
    {
        locomotionEdgeHandler.EdgeHovering(1);
    }

    public void OnMouseUp()
    {
        FindObjectOfType<LocomotionEdgeHandler>().RemoveEdge(sourceNodeID, sinkNodeID);
    }
}
