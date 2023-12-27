using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Threading;
using ChunkHandler;
using LocomotionGraph;

namespace NoiseTerrain
{
    public class ProceduralMapGenerator : MapGenerator
    {
        public bool closedLevel;

        public ChunkHandler.ChunkHandler chunks;

        public Vector2Int chunkID;
        public Vector2Int _chunkRadius = new Vector2Int(5, 4);
        public Vector2Int _tileRulesRadius = new Vector2Int(4, 3);
        public Vector2Int _tileRulesFixRadius = new Vector2Int(3, 2);
        public Vector2Int tileRadius = new Vector2Int(2, 1);

        private Vector2Int chunkRadius { get { return _chunkRadius + _tileRulesRadius + _tileRulesFixRadius; } }
        private Vector2Int tileRulesRadius { get { return _tileRulesRadius + _tileRulesFixRadius; } }
        private Vector2Int tileRulesFixRadius { get { return _tileRulesFixRadius; } }

        public List<Vector2Int> visibleChunkIDs = new List<Vector2Int>();
        public List<Vector2Int> toFixChunkIDs = new List<Vector2Int>();
        public List<Vector2Int> toDisplayChunks = new List<Vector2Int>();

        public Vector2Int roomSize = new Vector2Int(32, 32);
        public Transform target;


        public TileRules tileRules;
        public TileBase waterTile;
        public Tilemap waterTilemap;
        public TileBase lavaTile;
        public Tilemap lavaTilemap;

        //List<Chunk> chunks = new List<Chunk>();
        //public bool debugFixTileRules;
        public bool fixTileRules;
        Thread handleFixTileRulesThread;
        public FixSubChunk fixSubChunk;
        public bool active = false;

        public int displayCountPerFrame = 1000;

        

        private void OnDestroy()
        {
            fixTileRules = false;
            fixSubChunk.fixTileRules = fixTileRules;
            handleFixTileRulesThread.Abort();
            Debug.Log("Exit");
        }
        private void Start()
        {
            chunks.SetUp(width, height);
            handleFixTileRulesThread = new Thread(HandleFixTileRulesThread);
            handleFixTileRulesThread.Start();
            //fixTileRules = true;
        }
        private void Update()
        {
            
            if (!active) return;
            Vector2Int chunkID = chunks.GetChunkID(target.position);

            

            //update changed chunks
            for (int i = 0; i < visibleChunkIDs.Count; i += 1)
            {
                Chunk visibleChunk = chunks.GetChunk(visibleChunkIDs[i]);
                if (visibleChunk.valueChanged)
                {
                    if (!toDisplayChunks.Contains(visibleChunkIDs[i]))
                    {
                        //toDisplayChunks.Add(visibleChunkIDs[i]);
                        GenerateMap(visibleChunkIDs[i]);
                    }

                    visibleChunk.valueChanged = false;
                }
            }

            // ---------------------------chunk.Load(distance)--------------------------------
            //if(chunkID != this.chunkID)
            //{
            //    foreach(Vector2Int visibleChunkID in visibleChunkIDs)
            //    {
            //        int distance = (int)Vector2Int.Distance(visibleChunkID, chunkID);
            //        GetChunk(visibleChunkID).Load(distance);
            //    }
            //}

            //----------- display chunk -----------------------
            //for (int i = toDisplayChunks.Count - 1; i >= 0; i -= 1)
            //{
            //    lock (toFixChunkIDs)
            //    {
            //        if (!toFixChunkIDs.Contains(toDisplayChunks[i]))
            //        {
            //            //Debug.Log($"Displaying chunk {toDisplayChunks[i]}");
            //            GenerateMap(toDisplayChunks[i]);
            //            visibleChunkIDs.Add(toDisplayChunks[i]);
            //            toDisplayChunks.RemoveAt(i);
            //        }
            //    }

            //}
            // ------------------------- new display chunk -------------------------------
            int displayCountPerFrame = this.displayCountPerFrame;
            int toDisplayIndex = toDisplayChunks.Count - 1;
            while (toDisplayIndex >= 0 && displayCountPerFrame > 0)
            {
                lock (toFixChunkIDs)
                {
                    if (!toFixChunkIDs.Contains(toDisplayChunks[toDisplayIndex]))
                    {
                        //Debug.Log($"Displaying chunk {toDisplayChunks[i]}");
                        GenerateMap(toDisplayChunks[toDisplayIndex]);
                        visibleChunkIDs.Add(toDisplayChunks[toDisplayIndex]);
                        toDisplayChunks.RemoveAt(toDisplayIndex);
                        displayCountPerFrame -= 1;
                    }
                }
                toDisplayIndex -= 1;
            }
            CheckMap(chunkID);

            if (!setupComplete && toDisplayChunks.Count == 0 && toFixChunkIDs.Count == 0)
            {
                setupComplete = true;
            }
        }
        public void SetupProceduralMapGenerator(Vector2Int chunkRadius, Vector2Int chunkSize, Vector2 pos, bool active)
        {
            this.active = active;
            tileRadius = chunkRadius;
            transform.position = (Vector3)pos + Vector3.forward * transform.position.z;
            width = chunkSize.x;
            height = chunkSize.y;
        }

        //public override void GenerateMap()
        //{

        //}

        protected override int GetMinX()
        {

            return (chunkID.x - tileRadius.x) * width;

        }

        protected override int GetMinY()
        {
            return (chunkID.y - tileRadius.y) * height;

        }

        protected override int GetMaxX()
        {
            return (chunkID.x + tileRadius.x) * width + width - 1;

        }

        protected override int GetMaxY()
        {
            return (chunkID.y + tileRadius.y) * height + height - 1;



        }
        


        public void CheckMap(Vector2Int chunkID)
        {
            if (chunkID != this.chunkID)
            {
                Debug.Log("chunkID != this.chunkID");
                lock (toFixChunkIDs)
                {
                    this.chunkID = chunkID;
                    //layer 1 largest layer
                    List<Vector2Int> toBuildChunks = new List<Vector2Int>();
                    for (int x = -chunkRadius.x - tileRadius.x; x <= chunkRadius.x + tileRadius.x; x += 1)
                    {
                        for (int y = -chunkRadius.y - tileRadius.y; y <= chunkRadius.y + tileRadius.y; y += 1)
                        {

                            toBuildChunks.Add(chunkID + new Vector2Int(x, y));
                        }
                    }

                    //layer 2 second largest layer
                    List<Vector2Int> toCheckTileRulesChunks = new List<Vector2Int>();
                    for (int x = -tileRulesRadius.x - tileRadius.x; x <= tileRulesRadius.x + tileRadius.x; x += 1)
                    {
                        for (int y = -tileRulesRadius.y - tileRadius.y; y <= tileRulesRadius.y + tileRadius.y; y += 1)
                        {
                            toCheckTileRulesChunks.Add(chunkID + new Vector2Int(x, y));
                        }
                    }

                    //layer 3
                    List<Vector2Int> toFixTileRulesChunks = new List<Vector2Int>();
                    for (int x = 0; x <= tileRulesFixRadius.x + tileRadius.x; x += 1)
                    {
                        for (int y = 0; y <= tileRulesFixRadius.y + tileRadius.y; y += 1)
                        {
                            if (fixTileRules)
                            {
                                toFixTileRulesChunks.Add(chunkID + new Vector2Int(x, y));
                                if (x != 0) toFixTileRulesChunks.Add(chunkID + new Vector2Int(-x, y));
                                if (y != 0) toFixTileRulesChunks.Add(chunkID + new Vector2Int(x, -y));
                                if (x != 0 && y != 0) toFixTileRulesChunks.Add(chunkID + new Vector2Int(-x, -y));
                            }
                        }
                    }



                    // layer ?? visible layer smallest layer
                    List<Vector2Int> toDisplayChunks = new List<Vector2Int>();
                    for (int x = -tileRadius.x; x <= tileRadius.x; x += 1)
                    {
                        for (int y = -tileRadius.y; y <= tileRadius.y; y += 1)
                        {
                            toDisplayChunks.Add(chunkID + new Vector2Int(x, y));
                        }
                    }

                    //find chunks to be removed
                    for (int i = visibleChunkIDs.Count - 1; i >= 0; i -= 1)
                    {

                        if (!toDisplayChunks.Contains(visibleChunkIDs[i]) && !this.toDisplayChunks.Contains(visibleChunkIDs[i]))
                        {
                            //Debug.LogWarning($"Removing {visibleChunkIDs[i]}");
                            ClearMap(visibleChunkIDs[i]);
                            visibleChunkIDs.RemoveAt(i);

                        }
                        else if (!toDisplayChunks.Contains(visibleChunkIDs[i]))
                        {
                            //Debug.LogWarning($"Removing 2 {visibleChunkIDs[i]} {this.toDisplayChunks.Remove(visibleChunkIDs[i])}"); // must remove next line for debug
                            this.toDisplayChunks.Remove(visibleChunkIDs[i]);
                            visibleChunkIDs.RemoveAt(i);

                        }
                    }

                    //build chunks
                    for (int i = 0; i < toBuildChunks.Count; i += 1)
                    {
                        Chunk chunk = chunks.GetChunk(toBuildChunks[i]);
                        if (chunk == null)
                        {
                            int minX = toBuildChunks[i].x * width;
                            int maxX = (toBuildChunks[i].x + 1) * width - 1;
                            int minY = toBuildChunks[i].y * height;
                            int maxY = (toBuildChunks[i].y + 1) * height - 1;

                            float threshold = 0;

                            if (closedLevel && (toBuildChunks[i].x % roomSize.x == 0 || toBuildChunks[i].y % roomSize.y == 0))
                                threshold = -1;
                            chunk = new Chunk(toBuildChunks[i], GenerateBoolMap(minX, maxX, minY, maxY, threshold), chunks);
                            chunks.AddChunk(chunk);
                        }
                    }

                    //check tileRules
                    for (int i = 0; i < toCheckTileRulesChunks.Count; i += 1)
                    {
                        Chunk chunk = chunks.GetChunk(toCheckTileRulesChunks[i]);
                        Utility.CheckTileRules(chunk, tileRules);
                        // Debug.Log($"Checking Chunk {chunk.chunkID}");
                    }

                    //fix tileRules
                    for (int i = 0; i < toFixTileRulesChunks.Count; i += 1)
                    {
                        if (!toFixChunkIDs.Contains(toFixTileRulesChunks[i]) && !visibleChunkIDs.Contains(toFixTileRulesChunks[i]) && !this.toDisplayChunks.Contains(toFixTileRulesChunks[i]))
                        {
                            Chunk chunk = chunks.GetChunk(toFixTileRulesChunks[i]);
                            Utility.CheckTileRules(chunk, tileRules); // need to check in case invalid were fixed in an overlapping subchunk
                            if (chunk.hasInvalidTile)
                            {

                                toFixChunkIDs.Add(toFixTileRulesChunks[i]);

                            }
                        }

                    }

                    for (int i = 0; i < toDisplayChunks.Count; i += 1)
                    {
                        if (!this.toDisplayChunks.Contains(toDisplayChunks[i]) && !visibleChunkIDs.Contains(toDisplayChunks[i]))
                        {
                            this.toDisplayChunks.Add(toDisplayChunks[i]);
                        }
                    }

                }

                Utility.SortToChunkIDs(chunkID, toFixChunkIDs);

            }


        }

        

        public override void GenerateMap(int seed)
        {
            this.seed = seed;
            setupComplete = false;
            active = true;
        }

        public override void GenerateMap()
        {
            //int minX = chunkID.x * width;
            //int maxX = (chunkID.x + 1) * width - 1;
            //int minY = chunkID.y * height;
            //int maxY = (chunkID.y + 1) * height - 1;
            //GenerateMap(minX, maxX, minY, maxY);
        }
        public void GenerateMap(Vector2Int chunkID)
        {
            Chunk chunk = chunks.GetChunk(chunkID);
            int minX = chunkID.x * width;
            int maxX = (chunkID.x + 1) * width - 1;
            int minY = chunkID.y * height;
            int maxY = (chunkID.y + 1) * height - 1;

            //if (chunk.fullTilemap == null) chunk.fullTilemap = Instantiate(fullTilemap,fullTilemap.transform.parent);
            for (int x = 0; x < width; x += 1)
            {
                for (int y = 0; y < height; y += 1)
                {
                    bool[] neighbors = chunk.GetTileNeighbors(x, y);
                    if (neighbors != null)
                    {
                        TileBase tile = tileRules.GetSprite(neighbors);
                        if (tile == null)
                        {
                            tile = fullTile;
                        }
                        /*chunk.*/
                        fullTilemap.SetTile(new Vector3Int(x + minX, -y - minY, 0), tile);
                    }
                    else
                    {
                        /*chunk.*/
                        fullTilemap.SetTile(new Vector3Int(x + minX, -y - minY, 0), null);
                    }

                }
            }

            chunk.BuildChunk(seed);
        }

        public void ClearMap(Vector2Int chunkID)
        {
            int minX = chunkID.x * width;
            int maxX = (chunkID.x + 1) * width - 1;
            int minY = chunkID.y * height;
            int maxY = (chunkID.y + 1) * height - 1;
            ClearMap(minX, maxX, minY, maxY);
            chunks.GetChunk(chunkID).ClearChunk();
        }

        public override void ClearMap()
        {
            int minX = chunkID.x * width;
            int maxX = (chunkID.x + 1) * width - 1;
            int minY = chunkID.y * height;
            int maxY = (chunkID.y + 1) * height - 1;
            ClearMap(minX, maxX, minY, maxY);
        }

        public bool[,] GenerateBoolMap(Vector2Int chunkID)
        {
            int minX = chunkID.x * width;
            int maxX = (chunkID.x + 1) * width - 1;
            int minY = chunkID.y * height;
            int maxY = (chunkID.y + 1) * height - 1;

            return GenerateBoolMap(minX, maxX, minY, maxY, 0);
        }

        public void PlaceWater(Vector2Int startPos)
        {
            StartCoroutine(PlaceLiquid(waterTile, waterTilemap, startPos, true));
        }
        public void PlaceLava(Vector2Int startPos)
        {
            StartCoroutine(PlaceLiquid(lavaTile, lavaTilemap, startPos, true));
        }
        private IEnumerator PlaceLiquid(TileBase liquidTile, Tilemap tilemap, Vector2Int posStart, bool fillHorizontal)
        {
            yield return null;
            if (!tilemap.GetTile(new Vector3Int(posStart.x, posStart.y, 0)))
            {
                List<Vector2Int> fillTiles = WaterChunk.FindFloodTiles(posStart, chunks);

                foreach (Vector2Int tile in fillTiles)
                {
                    tilemap.SetTile(new Vector3Int(tile.x, tile.y, 0), liquidTile);
                }
                //tilemap.SetTile(new Vector3Int(posStart.x, posStart.y, 0), liquidTile);

                //if (fillHorizontal && visibleChunkIDs.Contains(chunks.GetChunkID(posStart + Vector2Int.left)) && !chunks.GetTile(posStart + Vector2Int.left))
                //    StartCoroutine(PlaceLiquid(liquidTile, tilemap, posStart + Vector2Int.left, fillHorizontal));
                //if (fillHorizontal && visibleChunkIDs.Contains(chunks.GetChunkID(posStart + Vector2Int.right)) && !chunks.GetTile(posStart + Vector2Int.right))
                //    StartCoroutine(PlaceLiquid(liquidTile, tilemap, posStart + Vector2Int.right,fillHorizontal));
                //if (visibleChunkIDs.Contains(chunks.GetChunkID(posStart + Vector2Int.down)) && !chunks.GetTile(posStart + Vector2Int.down))
                //    StartCoroutine(PlaceLiquid(liquidTile, tilemap, posStart + Vector2Int.down, false));
                //else if(visibleChunkIDs.Contains(chunks.GetChunkID(posStart + Vector2Int.down)))
                //{
                //    if(visibleChunkIDs.Contains(chunks.GetChunkID(posStart + Vector2Int.left)) && !chunks.GetTile(posStart + Vector2Int.left))
                //        StartCoroutine(PlaceLiquid(liquidTile, tilemap, posStart + Vector2Int.left, false));
                //    if (visibleChunkIDs.Contains(chunks.GetChunkID(posStart + Vector2Int.right)) && !chunks.GetTile(posStart + Vector2Int.right))
                //        StartCoroutine(PlaceLiquid(liquidTile, tilemap, posStart + Vector2Int.right, false));
                //}
            }

        }

        // Temp Method for generating liquid after ASP locomotion solver
        public void GenerateLiquid(int sourceID, int sinkID)
        {
            NodeChunk node = locomotionGraph.RoomChunk.GetPlatform(sourceID);
            Vector2Int fluidStart = node.GetFluidEdge(sinkID);
            if (!locomotionGraph.RoomChunk.FilledTile(fluidStart.x, fluidStart.y))
            {
                StartCoroutine(PlaceLiquid(waterTile, waterTilemap, new Vector2Int(fluidStart.x, -fluidStart.y), true));
            }
        }

        public LocomotionChunkGraph locomotionGraph;
        public List<Chunk> GetRoomChunk()
        {
            List<Chunk> roomChunks = new List<Chunk>();
            //roomSize should be double the tileRadius if all visible chunks should be in one room
            for (int x = -roomSize.x / 2; x <= roomSize.x / 2; x += 1)
            {
                for (int y = -roomSize.y / 2; y <= roomSize.y / 2; y += 1)
                {
                    roomChunks.Add(chunks.GetChunk(chunkID + new Vector2Int(x, y)));
                }
            }
            //locomotionGraph.SetRoomChunk(roomChunks, seed);
            return roomChunks;
        }

        private void HandleFixTileRulesThread()
        {
            while (fixTileRules)
            {
                Vector2Int chunkID = Vector2Int.zero;
                bool chunkIDFound = false;
                lock (toFixChunkIDs)
                {
                    if (toFixChunkIDs.Count > 0)
                    {
                        chunkID = toFixChunkIDs[0];
                        chunkIDFound = true;
                    }
                }
                if (chunkIDFound)
                {
                    Chunk chunk = chunks.GetChunk(chunkID);
                    if (chunk != null)
                    {
                        lock (chunk)
                        {
                            Utility.CheckTileRules(chunk, tileRules); // need to check in case invalid were fixed in an overlapping subchunk
                            if (chunk.hasInvalidTile)
                            {
                                HandleFixTileRules(chunk);

                            }
                        }

                        lock (toFixChunkIDs)
                        {
                            toFixChunkIDs.RemoveAt(0);
                            chunk.hasInvalidTile = false;
                        }
                    }

                }

            }

        }

        public int fixTileRuleBorder = 2;
        private void HandleFixTileRules(Chunk chunk)
        {
            Debug.Log($"Fixing Chunk {chunk.chunkID}");
            List<SubChunk> subChunks = chunk.GetInvalidSubChunks(fixTileRuleBorder);
            Debug.Log($"chunkID = {chunk.chunkID} : subChunks.Count = {subChunks.Count}");
            foreach (SubChunk subChunk in subChunks)
            {
                if (Mathf.Abs(chunkID.x - chunk.chunkID.x) <= tileRadius.x && Mathf.Abs(chunkID.y - chunk.chunkID.y) <= tileRadius.y)
                {
                    subChunk.PrintTiles();
                }
            }
            if (!fixTileRules) return;
            foreach (SubChunk subChunk in subChunks)
            {
                fixSubChunk.Fix(subChunk, fixTileRuleBorder, tileRules);
                while (!fixSubChunk.ready)
                {/*waiting for fixSubChunk to be done*/ }
                if (!subChunk.hasInvalid)
                {
                    subChunk.PrintTiles();
                    for (int y = 0; y < subChunk.tiles.GetLength(1); y += 1)
                    {
                        for (int x = 0; x < subChunk.tiles.GetLength(0); x += 1)
                        {
                            //Debug.Log($"{x}x{y} = {subChunk.tiles[x, y]}");
                            chunk.SetTile(x + subChunk.minX, y + subChunk.minY, subChunk.tiles[x, y]);
                        }
                    }
                }
                else
                {
                    Debug.LogWarning("TileRule not fixed");
                }
            }
        }



    }
}