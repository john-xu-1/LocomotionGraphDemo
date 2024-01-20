using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LocomotionGraph;

public class LocomotionEdgeHandler : MonoBehaviour
{
    public LocomotionGraph.LocomotionGraph locomotionGraph;
    public LocomotionEdge locomotionEdgePrefab;
    private List<LocomotionEdge> edgePool = new List<LocomotionEdge>();
    private List<LocomotionEdge> usedEdges = new List<LocomotionEdge>();
    public bool DisplayRayCast;

    private List<LocomotionGraphDebugger.PlatformChunkGraph> platformGraph;
    private bool displayPlatformGraph = false;

    public bool edgeHovered { get { return edgesHovered > 0; } }
    private int edgesHovered = 0;

    public struct EdgeID
    {
        public int sourceID;
        public int sinkID;
    }

    public EdgeID toRemoveEdge = default;
    public int removingEdgeCounter = -1;

    private List<EdgeID> removedEdgeIDs = new List<EdgeID>();

    private void Update()
    {
        if (removingEdgeCounter == 0)
        {
            RemoveEdge(toRemoveEdge);
        }
        if (removingEdgeCounter >= 0)
        {
            removingEdgeCounter--;
        }
        if (displayPlatformGraph) DisplayLocomotionGraph();
        DisplayPlatformGraph();
    }

    public void DisplayLocomotionGraph(bool display)
    {
        DisplayRayCast = display;
        if (!display) HandleReturnLocomotionEdges();
    }

    public void DisplayLocomotionGraph(List<LocomotionGraphDebugger.PlatformChunkGraph> platformGraph)
    {
        removedEdgeIDs.Clear();
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
                //LineRenderer line = new LineRenderer();
                LocomotionEdge edge = GetLocomotionEdge();
                edge.SetPositions(new Vector3[] { start, (Vector2)destination.GetCenterTilePos() });
                edge.SetNodeIDs(platform.platform.nodeID, nodeID);
                //Debug.Log($"{platform.graphColor}");
            }
        }
        displayPlatformGraph = false;
    }

    public void EdgeHovering(int value)
    {
        edgesHovered = Mathf.Clamp(edgesHovered + value, 0, int.MaxValue);
    }

    public void RemoveEdge(int sourceID, int sinkID)
    {
        toRemoveEdge = new EdgeID();
        toRemoveEdge.sinkID = sinkID;
        toRemoveEdge.sourceID = sourceID;
        removingEdgeCounter = 2;
        
        
    }

    public void RemoveEdge(EdgeID edgeID)
    {
        PlatformChunk platformChunk = locomotionGraph.RoomChunk.GetPlatform(edgeID.sourceID);
        if (platformChunk.RemoveConnectedPlatform(edgeID.sinkID))
        {
            Debug.Log("Redraw Everything");
            HandleReturnLocomotionEdges();
            FindObjectOfType<LocomotionGraphDebugger>().DisplayPlatformGraph();
            EdgeHovering(-1);
            removedEdgeIDs.Add(edgeID);

        }


    }

    public void AddEdgeID(EdgeID edgeID)
    {
        PlatformChunk platformChunk = locomotionGraph.RoomChunk.GetPlatform(edgeID.sourceID);
        if (platformChunk.RemoveConnectedPlatform(edgeID.sinkID))
        {
            HandleReturnLocomotionEdges();
            FindObjectOfType<LocomotionGraphDebugger>().DisplayPlatformGraph();
            removedEdgeIDs.Remove(edgeID);

        }
    }

    private LocomotionEdge GetLocomotionEdge()
    {
        if (edgePool.Count > 0)
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
        for (int i = usedEdges.Count - 1; i >= 0; i--)
        {
            LocomotionEdge edge = usedEdges[i];
            usedEdges.RemoveAt(i);
            edgePool.Add(edge);
            edge.gameObject.SetActive(false);
        }
    }

    protected void DisplayPlatformGraph()
    {
        if (DisplayRayCast)
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
