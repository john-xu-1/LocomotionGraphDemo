using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;



namespace LocomotionGraph
{
    public class LocomotionGraphDebugger : MonoBehaviour
    {

        public LocomotionGraph locomotionGraph;

        public bool isUIActive = false;

        public enum HandleLocomotionGraphFunction { displayPlatformGraph, printPlatformPath, isValidWall }
        public HandleLocomotionGraphFunction locomotionFunction;

        private bool displayPlatformGraph;
        List<PlatformChunkGraph> platformGraph;

        public class PlatformChunkGraph
        {
            public PlatformChunk platform;
            public Color graphColor;
        }

        private int startingPlatformID;
        public int generateChunkGraphDepth = 0;
        public GameObject lineprefab;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if(!isUIActive) HandleMouseClickDebugging();
            DisplayPlatformGraph();
        }

        protected void DisplayPlatformGraph()
        {
            if (displayPlatformGraph)
            {
                foreach (PlatformChunkGraph platform in platformGraph)
                {
                    foreach (int nodeID in platform.platform.connectedPlatforms)
                    {
                        PlatformChunk destination = locomotionGraph.RoomChunk.GetPlatform(nodeID);
                        Vector2 start = platform.platform.GetCenterTilePos();
                        Vector2 dir = destination.GetCenterTilePos() - start;
                        Debug.DrawRay(start, dir, platform.graphColor);
                        //LineRenderer line = new LineRenderer();
                        LineRenderer line = Instantiate(lineprefab).GetComponent<LineRenderer>();
                        line.SetPositions(new Vector3[] { start, (Vector2)destination.GetCenterTilePos()});
                        //Debug.Log($"{platform.graphColor}");
                    }
                }
            }
        }

        private Vector2Int TilePosFromClick(Vector2 mousePosition)
        {
            return new Vector2Int((int)Mathf.Floor(Camera.main.ScreenToWorldPoint(Input.mousePosition).x), (int)Mathf.Floor(Camera.main.ScreenToWorldPoint(Input.mousePosition).y));
        }

        private void HandleMouseClickDebugging()
        {
            if (locomotionFunction == HandleLocomotionGraphFunction.displayPlatformGraph)
            {
                HandleDisplayPlatformGraph();
            }
            else if (locomotionFunction == HandleLocomotionGraphFunction.printPlatformPath)
            {
                HandlePrintPlatformPath();
            }
            else if (locomotionFunction == HandleLocomotionGraphFunction.isValidWall)
            {
                HandleIsValidWall();
            }


        }

        protected void HandleDisplayPlatformGraph()
        {
            if (locomotionGraph.RoomChunk != null && Input.GetMouseButtonUp(0))
            {
                displayPlatformGraph = false;
                Vector2Int clickTile = TilePosFromClick(Input.mousePosition);// new Vector2Int((int)Mathf.Floor(Camera.main.ScreenToWorldPoint(Input.mousePosition).x), (int)Mathf.Floor(Camera.main.ScreenToWorldPoint(Input.mousePosition).y));

                // find platformID from click tile 
                startingPlatformID = locomotionGraph.RoomChunk.GetPlatformID(clickTile);
                int filledChunkID = startingPlatformID / FilledChunk.nodeIDOffset;
                int platformChunkId = startingPlatformID % FilledChunk.nodeIDOffset;
                //Debug.LogWarning($"clickTile: {clickTile} PlatformIDClicked: {startingPlatformID}  filledChunkID:{filledChunkID} platformID{platformChunkId}");

                // check that clicked tile is within the room chunk, not an empty tile and a surface tile
                if (startingPlatformID < 0)
                {
                    Debug.LogWarning($"Click is out of range of RoomChunk. startingPlatformID({startingPlatformID}) filledChunkID({filledChunkID}) platformID({platformChunkId})");
                    return;
                }
                else if (startingPlatformID == 0)
                {
                    Debug.LogWarning($"Click is on empty tile and not a starting platform. startingPlatformID({startingPlatformID}) filledChunkID({filledChunkID}) platformID({platformChunkId})");
                    return;
                }
                else if (platformChunkId == 0)
                {
                    Debug.LogWarning($"Click is on filled tile but is not a platform tile. startingPlatformID({startingPlatformID}) filledChunkID({filledChunkID}) platformID({platformChunkId})");
                    return;
                }

                // start thread to generate the chunk graph
                Thread thread = new Thread(GenerateChunkGraphThread);
                thread.Start();
            }
        }

        protected void HandlePrintPlatformPath()
        {
            if (locomotionGraph.RoomChunk != null && Input.GetMouseButtonUp(0))
            {
                //Vector2 clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2Int clickTile = TilePosFromClick(Input.mousePosition); //new Vector2Int((int)Mathf.Floor(clickPos.x), (int)Mathf.Floor(clickPos.y));

                PrintPoiPlatform(clickTile);

                PrintPlatform(clickTile);
                //GenerateConnectedChunkGraph();
            }
        }

        protected void HandleIsValidWall()
        {
            if (Input.GetMouseButtonUp(1)) //check right side of wall
            {
                Vector2Int clickTile = locomotionGraph.RoomChunk.GetWorldToGridPos(TilePosFromClick(Input.mousePosition));
                Debug.Log(clickTile);
                if (locomotionGraph.RoomChunk.FilledTile(clickTile.x, clickTile.y))
                {
                    Debug.Log($"isValidWall right empty: {WallChunk.IsValidWall(clickTile, locomotionGraph.jumpHeight, true, locomotionGraph.RoomChunk)}");
                }
            }
            else if (Input.GetMouseButtonUp(0)) //check left side of wall
            {
                Vector2Int clickTile = locomotionGraph.RoomChunk.GetWorldToGridPos(TilePosFromClick(Input.mousePosition));
                if (locomotionGraph.RoomChunk.FilledTile(clickTile.x, clickTile.y))
                {
                    Debug.Log($"isValidWall left empty: {WallChunk.IsValidWall(clickTile, locomotionGraph.jumpHeight, false, locomotionGraph.RoomChunk)}");
                }
            }
        }


        private void GenerateChunkGraphThread()
        {
            List<int> platformNodes = new List<int>();
            platformNodes.Add(startingPlatformID);

            int graphListIndex = 0;
            int depth = 0;
            while (depth < generateChunkGraphDepth)
            {
                int platformNodesCount = platformNodes.Count;
                for (int i = graphListIndex; i < platformNodesCount; i += 1)
                {
                    foreach (int nodeDestionationId in locomotionGraph.RoomChunk.GetPlatformEdges(platformNodes[i], locomotionGraph.jumpHeight, locomotionGraph.checkConnection))
                    {
                        if (!platformNodes.Contains(nodeDestionationId)) platformNodes.Add(nodeDestionationId);
                    }
                }
                graphListIndex = platformNodesCount;
                depth += 1;
            }

            List<PlatformChunk> graphList = new List<PlatformChunk>();
            foreach (int nodeID in platformNodes)
            {
                PlatformChunk platform = locomotionGraph.RoomChunk.GetPlatform(nodeID);
                if (!graphList.Contains(platform))
                {
                    locomotionGraph.RoomChunk.GetPlatformEdges(nodeID, locomotionGraph.jumpHeight, locomotionGraph.checkConnection);
                    graphList.Add(platform);
                }
            }
            List<PlatformChunkGraph> platformChunkGraphs = new List<PlatformChunkGraph>();
            foreach (PlatformChunk platform in graphList)
            {
                PlatformChunkGraph platformChunkGraph = new PlatformChunkGraph();
                platformChunkGraph.platform = platform;
                platformChunkGraph.graphColor = Color.red;
                platformChunkGraphs.Add(platformChunkGraph);
            }
            platformGraph = platformChunkGraphs;
            displayPlatformGraph = true;
        }

        public void GenerateConnectedChunkGraph()
        {
            displayPlatformGraph = false;
            Thread thread = new Thread(GenerateConnectedChunkGraphThread);
            thread.Start();
        }
        /// <summary>
        /// Find the list of sources of default sources for each platform chunk and find the each group of sourcechunk that have the same set of sources.
        /// Each group of source chunks is labeled with different colors so display
        /// </summary>
        private void GenerateConnectedChunkGraphThread()
        {
            //find all platformIDs
            List<int> platformIDs = locomotionGraph.RoomChunk.GetPlatformIDs();

            List<int> sourceIDs = new List<int>();
            for (int i = 0; i < platformIDs.Count; i += 1)
            {
                int id = platformIDs[i];
                bool isSource = true;
                for (int j = 0; j < platformIDs.Count; j += 1)
                {
                    if (locomotionGraph.RoomChunk.GetPlatformEdges(platformIDs[j], locomotionGraph.jumpHeight, locomotionGraph.checkConnection).Contains(id))
                    {
                        isSource = false;
                        break;
                    }
                }
                if (isSource)
                {
                    //Debug.Log($"SourceID {id}");
                    sourceIDs.Add(id);
                    //roomChunk.GetPlatform(id).defaultSources.Add(roomChunk.GetPlatform(id));
                }
            }

            //finds all sources for each platform and adds to defaultSources in the NodeChunk
            Debug.Log($"sourceIDs.Count:{sourceIDs.Count}  platformIDs.Count:{platformIDs.Count}");
            List<int> sourceIDsCopy = new List<int>(sourceIDs);
            while (sourceIDsCopy.Count > 0)
            {
                int currentSource = sourceIDsCopy[0];
                sourceIDsCopy.RemoveAt(0);
                List<int> frontier = new List<int>();
                frontier.Add(currentSource);
                while (frontier.Count > 0)
                {
                    int current = frontier[0];
                    frontier.RemoveAt(0);
                    PlatformChunk platform = locomotionGraph.RoomChunk.GetPlatform(current);
                    PlatformChunk source = locomotionGraph.RoomChunk.GetPlatform(currentSource);
                    if (!platform.defaultSources.Contains(source))
                    {
                        platform.defaultSources.Add(source);
                        foreach (int connection in platform.connectedPlatforms)
                        {
                            frontier.Add(connection);
                        }
                    }

                }
            }

            //find all platform connections
            List<PlatformChunkGraph> platforms = new List<PlatformChunkGraph>();
            int connectChunkID = 0;
            Color[] colors = { Color.red, Color.blue, Color.cyan, Color.green, Color.magenta, Color.yellow, Color.white, Color.black };

            List<PlatformChunkGraph> chunkGroups = new List<PlatformChunkGraph>();
            foreach (int platformID in platformIDs)
            {
                PlatformChunk platform = locomotionGraph.RoomChunk.GetPlatform(platformID);
                PlatformChunkGraph platformChunkGraph = new PlatformChunkGraph();
                platformChunkGraph.platform = platform;
                platforms.Add(platformChunkGraph);

                bool foundChunkGroup = false;
                for (int i = 0; i < chunkGroups.Count; i++)
                {
                    if (platform != chunkGroups[i].platform && platform.defaultSources.Count == chunkGroups[i].platform.defaultSources.Count)
                    {
                        bool matching = true;
                        foreach (PlatformChunk source in platform.defaultSources)
                        {
                            if (!chunkGroups[i].platform.defaultSources.Contains(source))
                            {
                                matching = false;
                                break;
                            }
                        }
                        if (matching)
                        {
                            platformChunkGraph.graphColor = chunkGroups[i].graphColor;

                            foundChunkGroup = true;
                            break;
                        }
                    }

                }
                if (!foundChunkGroup)
                {
                    chunkGroups.Add(platformChunkGraph);
                    int colorID = (chunkGroups.Count - 1) % colors.Length;
                    platformChunkGraph.graphColor = colors[colorID];
                    //Debug.Log($"{platformChunkGraph.platform.nodeID}:{platformChunkGraph.graphColor}");
                }
            }
            Debug.Log($"chunkGroups.Count:{chunkGroups.Count}  platforms.Count:{platforms.Count}");

            platformGraph = platforms;
            displayPlatformGraph = true;
        }

        private void PrintPlatform(Vector2Int startTile)
        {
            int platformID = locomotionGraph.RoomChunk.GetPlatformID(startTile);
            if (platformID == 0)
            {
                locomotionGraph.RoomChunk.PrintPath(startTile, locomotionGraph.jumpHeight, platformID);
            }
            else if (platformID % 512 > 0)
            {
                locomotionGraph.RoomChunk.PrintPath(new Vector2Int(startTile.x, startTile.y + 1), locomotionGraph.jumpHeight, platformID);
            }
        }

        private void PrintPoiPlatform (Vector2Int startTile)
        {
            int platformID = locomotionGraph.RoomChunk.GetPlatformID(startTile);
            int x = startTile.x;
            int y = startTile.y;

            if (platformID == 0)
            {
                // clicked on air and then the path is where the player can reach from that point
            }
            else if (platformID % 512 > 0)
            {
                // clicked on a platform and then the path is to which platforms the player can rich
                y++;
            }
            int[,] platformPath = locomotionGraph.RoomChunk.GetPath(new Vector2Int(x, y), locomotionGraph.jumpHeight, platformID);
            POIHandler poiHandler = FindObjectOfType<POIHandler>();
            for (int i = 0; i < platformPath.GetLength(0); i++)
            {
                for (int j = 0; j < platformPath.GetLength(1); j++)
                {
                    if (platformPath[i,j] >= 0)
                    {
                        Vector2 pos = new Vector2(i + locomotionGraph.RoomChunk.minTile.x + 0.5f, -j + 1 - locomotionGraph.RoomChunk.maxTile.y - 0.5f);
                        poiHandler.SetPOI(pos, $"{platformPath[i, j]}", new Color(1f / 255, 50f/ 255, 32f /255));
                    }
                }
            }
        }
    }
}

