using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChunkHandler;
using System.IO;

namespace NoiseTerrain
{
    public static class Utility 
    {
        public static void CheckTileRules(Chunk chunk, TileRules tileRules)
        {

            for (int x = 0; x < chunk.width; x += 1)
            {
                for (int y = 0; y < chunk.height; y += 1)
                {
                    bool[] neighbors = chunk.GetTileNeighbors(x, y);
                    if (neighbors != null)
                    {
                        bool valid = tileRules.GetValidTile(neighbors);
                        chunk.SetInvalidTile(x, y, !valid);

                        if (!valid) chunk.hasInvalidTile = true;
                    }

                }
            }
            //chunk.SetInvalidTile();
        }
        public static bool CheckTileRules(SubChunk subChunk, TileRules tileRules)
        {
            for (int x = 1; x < subChunk.tiles.GetLength(0) - 1; x += 1)
            {
                for (int y = 1; y < subChunk.tiles.GetLength(1) - 1; y += 1)
                {
                    bool[] neighbors = subChunk.GetTileNeighbors(x, y);
                    if (neighbors != null && !tileRules.GetValidTile(neighbors)) return false;

                }
            }
            return true;
        }

        public static bool CheckTileRules(List<bool> tiles, int width, TileRules tileRules)
        {
            int height = tiles.Count / width;
            for (int x = 1; x < width - 1; x += 1)
            {
                for (int y = 1; y < height - 1; y += 1)
                {
                    int index = x + y * width;
                    bool[] neighbors = SubChunk.GetTileNeighbors(index, width, tiles);
                    if (neighbors != null && !tileRules.GetValidTile(neighbors)) return false;

                }
            }
            return true;
        }

        public static void SortToChunkIDs(Vector2Int chunkID, List<Vector2Int> toFixChunkIDs)
        {
            for(int i = 0; i < toFixChunkIDs.Count; i += 1)
            {
                for(int j = 0; j < toFixChunkIDs.Count - 1; j += 1)
                {
                    if(Vector2Int.Distance(chunkID, toFixChunkIDs[j]) > Vector2Int.Distance(chunkID, toFixChunkIDs[j + 1])){
                        Vector2Int temp = toFixChunkIDs[j];
                        toFixChunkIDs[j] = toFixChunkIDs[j + 1];
                        toFixChunkIDs[j + 1] = temp;
                    }
                }
            }

        }

        public static void PrintChunksToBoolMap(List<Chunk> roomChunks, string filename)
        {

            int minYID = int.MaxValue;
            int minXID = int.MaxValue;
            int maxYID = int.MinValue;
            int maxXID = int.MinValue;

            Debug.Log($"roomChunks.Count: {roomChunks.Count}");
            foreach (Chunk chunk in roomChunks)
            {
                minXID = Mathf.Min(chunk.chunkID.x, minXID);
                minYID = Mathf.Min(chunk.chunkID.y, minYID);
                maxXID = Mathf.Max(chunk.chunkID.x, maxXID);
                maxYID = Mathf.Max(chunk.chunkID.y, maxYID);
            }

            Vector2Int roomChunkSize = new Vector2Int(maxXID - minXID + 1, maxYID - minYID + 1);
            //chunks = new Chunk[roomChunkSize.x, roomChunkSize.y];
            int width = roomChunkSize.x * roomChunks[0].width;
            int height = roomChunkSize.y * roomChunks[0].height;

            bool[,] boolMap = new bool[width, height];

            Debug.Log($"minXID: {minXID} minYID: {minYID} maxXID: {maxXID} maxYID: {maxYID}");

            //calc the min/max tiles maxY and min Y are flipped since positive y is down
            Vector2Int minTileThread = new Vector2Int(minXID * roomChunks[0].width, maxYID * roomChunks[0].height + roomChunks[0].height - 1);
            Vector2Int maxTileThread = new Vector2Int(maxXID * roomChunks[0].width + roomChunks[0].width - 1, minYID * roomChunks[0].height);

            Debug.Log($"minTileThread: {minTileThread} maxTileThread: {maxTileThread}");
            for (int x = minTileThread.x; x <= maxTileThread.x; x++)
            {
                for (int y = maxTileThread.y; y <= minTileThread.y; y++)
                {
                    boolMap[x - minTileThread.x, y - maxTileThread.y] = roomChunks[0].GetTile(x, y);

                }
            }


            string map0_1 = "";
            for (int y = 0; y < height; y += 1)
            {
                for (int x = 0; x < width; x += 1)
                {
                    map0_1 += boolMap[x, y] ? "1" : "0";
                }
                map0_1 += "\n";
            }
            Debug.Log(map0_1);
            CreateFile(map0_1, $"{filename}.txt");
        }

        public static string DataFilePath = @"DataFiles/temp";


        public static string CreateFile(string content, string filename)
        {
            string relativePath = GetPathToFile(filename);
            //if (Application.isEditor)
            //{
            //    if (!Directory.Exists(Path.Combine("Assets", DataFilePath)))
            //    {
            //        Directory.CreateDirectory(Path.Combine("Assets", DataFilePath));
            //    }
            //    relativePath = Path.Combine("Assets", DataFilePath, filename);
            //}
            //else
            //{
            //    if (!Directory.Exists(DataFilePath))
            //    {
            //        Directory.CreateDirectory(DataFilePath);
            //    }
            //    relativePath = Path.Combine(DataFilePath, filename);
            //}

            using (StreamWriter streamWriter = File.CreateText(relativePath))
            {
                streamWriter.Write(content);
            }
            return relativePath;
        }

        public static string GetPathToFile(string filename)
        {
            string relativePath;
            if (Application.isEditor)
            {
                if (!Directory.Exists(Path.Combine("Assets", DataFilePath)))
                {
                    Directory.CreateDirectory(Path.Combine("Assets", DataFilePath));
                }
                relativePath = Path.Combine("Assets", DataFilePath, filename);
            }
            else
            {
                if (!Directory.Exists(DataFilePath))
                {
                    Directory.CreateDirectory(DataFilePath);
                }
                relativePath = Path.Combine(DataFilePath, filename);
            }

            return relativePath;
        }

    }
}

