using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LocomotionGraph;

public class LocomotionEdgeHandler : MonoBehaviour
{
    public LocomotionGraph.LocomotionGraph locomotionGraph;
    public LocomotionEdge locomotionEdgePrefab;
    private List<LocomotionEdge> edgePool = new List<LocomotionEdge>();
    public bool DisplayRaycast;

    private List<LocomotionGraphDebugger.PlatformChunkGraph> platformGraph;
    private bool displayPlatformGraph = false;

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

                LocomotionEdge edge = Instantiate(locomotionEdgePrefab);
                edge.SetPositions(new Vector3[] { start, (Vector2)destination.GetCenterTilePos() });

            }
        }
        displayPlatformGraph = false;
    }

    private void HandleReturnLocomotionEdges()
    {

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
