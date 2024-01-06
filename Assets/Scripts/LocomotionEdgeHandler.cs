using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LocomotionGraph;


public class LocomotionEdgeHandler : MonoBehaviour
{
    public LocomotionGraph.LocomotionGraph locomotionGraph;
    public LocomotionEdge locomotionEdgePrefab;
    private List<LocomotionEdge> edgePool = new List<LocomotionEdge>();
    public bool DisplayRayCast;

    private List<LocomotionGraphDebugger.PlatformChunkGraph> platformGraph;

    private void Update()
    {
        
    }

    public void DisplayLocomotionGraph (bool display)
    {
        DisplayRayCast = display;
        if(!display) HandleReturnLocomotionEdges();
    }

    public void DisplayLocomotionGraph(List<LocomotionGraphDebugger.PlatformChunkGraph> platformGraph)
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
                LineRenderer line = Instantiate(locomotionEdgePrefab).GetComponent<LineRenderer>();
                line.SetPositions(new Vector3[] { start, (Vector2)destination.GetCenterTilePos() });
                //Debug.Log($"{platform.graphColor}");
            }
        }
    }

    private void HandleReturnLocomotionEdges()
    {

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
