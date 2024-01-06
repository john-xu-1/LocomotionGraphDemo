using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LocomotionGraph;

public class LocomotionEdgeHandler : MonoBehaviour
{
    public LocomotionGraph.LocomotionGraph locomotionGraph;
    public LocomotionEdge locomotionEdgePrefab;
    private List<LocomotionEdge> edgePool = new List<LocomotionEdge>(), usedEdges = new List<LocomotionEdge>();
    public bool DisplayRaycast;

    private List<LocomotionGraphDebugger.PlatformChunkGraph> platformGraph;
    private bool displayPlatformGraph = false;

    public bool edgeHovered { get { return edgesHovered > 0; } }
    private int edgesHovered = 0;

    private void Update()
    {
        if (displayPlatformGraph) DisplayLocomotionGraph();
        DisplayPlatformGraph();
    }

    public void DisplayLocomotionGraph(bool display)
    {
        DisplayRaycast = display;
        if(!display) HandleReturnLocomotionEdges();
    }

    public void DisplayLocomotionGraph(List<LocomotionGraphDebugger.PlatformChunkGraph> platformGraph)
    {
        this.platformGraph = platformGraph;
        displayPlatformGraph = true;
    }

    public void DisplayLocomotionGraph()
    {
        foreach (LocomotionGraphDebugger.PlatformChunkGraph platform in platformGraph)
        {
            foreach (int nodeID in platform.platform.connectedPlatforms)
            {
                PlatformChunk destination = locomotionGraph.RoomChunk.GetPlatform(nodeID);
                Vector2 start = platform.platform.GetCenterTilePos();
                Vector2 dir = destination.GetCenterTilePos() - start;
                Debug.DrawRay(start, dir, platform.graphColor);

                LocomotionEdge edge = GetLocomotionEdge();
                edge.SetPositions(new Vector3[] { start, (Vector2)destination.GetCenterTilePos() });
                edge.SetNodeIDs(platform.platform.nodeID, nodeID);
            }
        }
        displayPlatformGraph = false;
    }

    public void EdgeHovering(int value)
    {
        edgesHovered = Mathf.Clamp(edgesHovered + value, 0, int.MaxValue);
        Debug.Log(edgesHovered);
    }


    public void RemoveEdge(int sourceID, int sinkID)
    {
        PlatformChunk platformChunk = locomotionGraph.RoomChunk.GetPlatform(sourceID);


        if (platformChunk.RemoveConnectedPlatform(sinkID))
        {
            Debug.Log("Redraw everything!!!");
            EdgeHovering(-1);
            FindObjectOfType<LocomotionGraphDebugger>().DisplayPlatformGraph();
            
        }
    }


    private LocomotionEdge GetLocomotionEdge()
    {
        if(edgePool.Count > 0)
        {
            LocomotionEdge edge = edgePool[0];
            edge.gameObject.SetActive(true);
            edgePool.RemoveAt(0);
            usedEdges.Add(edge);
            return edge;
        }
        else
        {
            LocomotionEdge edge = Instantiate(locomotionEdgePrefab);
            usedEdges.Add(edge);
            edge.SetEdgeHandler(this);
            return edge;
        }
    }

    private void HandleReturnLocomotionEdges()
    {
        for(int i = usedEdges.Count -1; i >= 0; i--)
        {
            LocomotionEdge edge = usedEdges[i];
            usedEdges.RemoveAt(i);
            edgePool.Add(edge);
            edge.gameObject.SetActive(false);
        }
    }

    protected void DisplayPlatformGraph()
    {
        if (DisplayRaycast)
        {
            foreach (LocomotionGraphDebugger.PlatformChunkGraph platform in platformGraph)
            {
                foreach (int nodeID in platform.platform.connectedPlatforms)
                {
                    PlatformChunk destination = locomotionGraph.RoomChunk.GetPlatform(nodeID);
                    Vector2 start = platform.platform.GetCenterTilePos();
                    Vector2 dir = destination.GetCenterTilePos() - start;
                    Debug.DrawRay(start, dir, platform.graphColor);

                }
            }
        }
    }
}
