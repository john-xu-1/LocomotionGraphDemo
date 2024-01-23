using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LocomotionGraph
{
    [System.Serializable]
    public class FilledChunk
    {
        public List<Vector2Int> filledTiles = new List<Vector2Int>();
        public List<Vector2Int> groundTiles = new List<Vector2Int>();
        //public List<Vector2Int> wallTiles = new List<Vector2Int>();
        public List<Vector2Int> leftWallTiles = new List<Vector2Int>();
        public List<Vector2Int> rightWallTiles = new List<Vector2Int>();
        public List<PlatformChunk> platforms = new List<PlatformChunk>();
        public List<WallChunk> walls = new List<WallChunk>();
        int[,] platformIDs;
        int[,] wallIDs;
        int minX, minY, maxX, maxY;
        RoomChunk roomChunk;

        int filledChunkID;

        public static int nodeIDOffset = 512;

        public FilledChunk(int filledChunkID)
        {
            this.filledChunkID = filledChunkID;
        }

        public int GetPlatformID(Vector2Int tile)
        {
            //Debug.Log($"{tile.x - minX} {-tile.y - maxY} minX: {minX} maxY: {maxY}");
            return platformIDs[tile.x - minX, tile.y - minY];
        }
        public void SetupFilledChunk(RoomChunk roomChunk)
        {
            this.roomChunk = roomChunk;
            minX = int.MaxValue;
            minY = int.MaxValue;
            maxX = int.MinValue;
            maxY = int.MinValue;
            foreach (Vector2Int tile in filledTiles)
            {
                if (tile.x > maxX) maxX = tile.x;
                if (tile.y > maxY) maxY = tile.y;
                if (tile.x < minX) minX = tile.x;
                if (tile.y < minY) minY = tile.y;
            }
        }
        public void SetWallChunks(int jumpHeight)
        {
            int width = maxX - minX + 1;
            int height = maxY - minY + 1;
            wallIDs = new int[width, height];

            Debug.Log($"------------------------({minX},{minY}) ({maxX},{maxY})---------------");
            SetWallChunks(leftWallTiles, jumpHeight, false);
            SetWallChunks(rightWallTiles, jumpHeight, true);

            
            List<Vector2Int> validWalls = new List<Vector2Int>();
            List<int> validWallIDs = new List<int>();
            for (int x = 0; x < wallIDs.GetLength(0); x++)
            {
                for (int y = 0; y < wallIDs.GetLength(1); y++)
                {
                    if (wallIDs[x,y] > 0)
                    {
                        validWalls.Add(new Vector2Int(x + minX, y + minY));
                        validWallIDs.Add(wallIDs[x, y]);
                    }
                }
            }
            if (WallChunkDebugger.singleton) WallChunkDebugger.singleton.AddWalls(validWalls, validWallIDs, true);
        }
        int wallID = 0;
        private void SetWallChunks(List<Vector2Int> wallSideTiles, int jumpHeight, bool rightSide)
        {
            List<Vector2Int> toVisit = new List<Vector2Int>(wallSideTiles);
            List<Vector2Int> visited = new List<Vector2Int>();

            int breakCounter = 1000;
            while (toVisit.Count > 0)
            {
                
                Vector2Int validWall = FindValidWall(toVisit, jumpHeight, rightSide);
                if (validWall.x != -1 || validWall.y != -1)
                {
                    wallID += 1;
                    wallIDs[validWall.x - minX, validWall.y - minY] = wallID;
                    foreach (Vector2Int connectedWall in GetValidWalls(validWall, rightSide))
                    {
                        toVisit.Remove(connectedWall);
                        wallIDs[connectedWall.x - minX, connectedWall.y - minY] = wallID;
                    }
                    breakCounter -= 1;
                    if (breakCounter < 0)
                    {
                        Debug.LogWarning("SetWallChunks toVisit.Count break");
                        break;
                    }
                }
                
            }
        }
        private Vector2Int FindValidWall(List<Vector2Int> wallTiles, int jumpHeight, bool rightSide)
        {
            Vector2Int validWall = new Vector2Int(-1, -1);

            while (wallTiles.Count > 0 && validWall.x == -1 && validWall.y == -1)
            {
                Vector2Int checkWall = wallTiles[0];
                wallTiles.RemoveAt(0);
                if (WallChunk.IsValidWall(checkWall, jumpHeight, rightSide, roomChunk))
                {
                    validWall = checkWall;
                }
            }

            return validWall;
        }
        private List<Vector2Int> GetValidWalls(Vector2Int wallTile, bool rightSide)
        {
            int xOffset = rightSide ? 1 : -1;
            List<Vector2Int> validWalls = new List<Vector2Int>();

            //loop up until empty or no longer a wall
            int y = wallTile.y - 1;
            while (y -minY >= 0 && roomChunk.FilledTile(wallTile.x, y) && !roomChunk.FilledTile(wallTile.x + xOffset, y))
            {
                validWalls.Add(new Vector2Int(wallTile.x, y));
                y--;
            }

            //loop down until empty or no longer a wall
            y = wallTile.y + 1;
            while (y - minY < wallIDs.GetLength(1) && roomChunk.FilledTile(wallTile.x, y) && !roomChunk.FilledTile(wallTile.x + xOffset, y))
            {
                validWalls.Add(new Vector2Int(wallTile.x, y));
                y++;
            }
            return validWalls;
        }
        public void SetPlatforms(int jumpHeight)
        {
            //SetupFilledChunk(roomChunk);
            int width = maxX - minX + 1;
            int height = maxY - minY + 1;
            platformIDs = new int[width, height];
            foreach (Vector2Int tile in filledTiles)
            {
                platformIDs[tile.x - minX, tile.y - minY] = -1;
            }

            List<Vector2Int> toVisit = new List<Vector2Int>(groundTiles);
            List<Vector2Int> visited = new List<Vector2Int>();
            int platformID = 0;
            int breakCounter = 10000;
            while (toVisit.Count > 0)
            {
                List<Vector2Int> frontier = new List<Vector2Int>();
                platformID += 1;
                frontier.Add(toVisit[0]);
                //toVisit.RemoveAt(0);
                while (frontier.Count > 0)
                {
                    Vector2Int current = frontier[0];
                    frontier.RemoveAt(0);
                    visited.Add(current);
                    toVisit.Remove(current);

                    platformIDs[current.x - minX, current.y - minY] = platformID;

                    FindPlatformNeighbor(-1, current, jumpHeight, platformID, toVisit, visited, frontier, roomChunk);
                    FindPlatformNeighbor(1, current, jumpHeight, platformID, toVisit, visited, frontier, roomChunk);
                    breakCounter -= 1;
                    if (breakCounter < 0)
                    {
                        Debug.LogWarning("frontier.Count break");
                        break;
                    }
                }
                breakCounter -= 1;
                if (breakCounter < 0)
                {
                    Debug.LogWarning("toVisit.Count break");
                    break;
                }
            }

            for (int i = 0; i < platformID; i += 1) platforms.Add(new PlatformChunk(roomChunk, nodeIDOffset * filledChunkID + i + 1));
            //string printMap = "";
            for (int y = 0; y < platformIDs.GetLength(1); y += 1)
            {
                for (int x = 0; x < platformIDs.GetLength(0); x += 1)
                {
                    //if (platformIDs[x, y] == -1)
                    //{
                    //    printMap += "X";
                    //}
                    //else
                    //{
                    //    printMap += platformIDs[x, y].ToString();
                    //}
                    if (platformIDs[x, y] > 0 && y + minY > 0)
                    {
                        platforms[platformIDs[x, y] - 1].jumpTiles.Add(new Vector2Int(x + minX, y + minY));
                        //Debug.Log($"jumpTiles[{x + minX}, {y + minY}] = {roomChunk.GetTile(x + minX, y + minY)}");
                    }
                }
                //printMap += "\n";
            }
            //Debug.Log(printMap);
        }
        

        

        private void FindPlatformNeighbor(int xOffset, Vector2Int current, int jumpHeight, int platformID, List<Vector2Int> toVisit, List<Vector2Int> visited, List<Vector2Int> frontier, RoomChunk roomChunk)
        {
            int y = current.y;
            int breakCounter = 1000;
            while (Mathf.Abs(y - current.y) <= jumpHeight && y - minY >= 0 && y - minY < platformIDs.GetLength(1) && current.x + xOffset - minX >= 0 && current.x + xOffset - minX < platformIDs.GetLength(0))
            {
                if (platformIDs[current.x + xOffset - minX, y - minY] != 0)
                {
                    //check up

                    if (y > 0 && !roomChunk.FilledTile(current.x + xOffset, y - 1))
                    {
                        //plaform tile found
                        platformIDs[current.x + xOffset - minX, y - minY] = platformID;
                        Vector2Int neighbor = new Vector2Int(current.x + xOffset, y);
                        if (!visited.Contains(neighbor) && !frontier.Contains(neighbor))
                        {
                            frontier.Add(neighbor);
                        }

                        break;
                    }
                    else
                    {
                        //check up
                        y -= 1;
                    }
                }
                else
                {
                    //check down
                    y += 1;
                    if (y - minY >= platformIDs.GetLength(1) || platformIDs[current.x - minX, y - minY] == 0) break;
                }
                breakCounter -= 1;
                if (breakCounter < 0)
                {
                    Debug.LogWarning("FindPlatformNeighbor break");
                    break;
                }
            }
        }
    }
}