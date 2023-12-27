using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LocomotionGraph;
using ChunkHandler;
using System.Threading;
using System.IO;

public class LocomotionChunkGraph : LocomotionGraph.LocomotionGraph, IChunkable
{
    List<Chunk> roomChunks;
    public bool locomotionGraphSetupComplete = false;

    public void GenerateFromChunks(List<Chunk> chunks)
    {
        roomChunks = chunks;
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

        boolMapThread = new bool[width, height];

        Debug.Log($"minXID: {minXID} minYID: {minYID} maxXID: {maxXID} maxYID: {maxYID}");

        //calc the min/max tiles maxY and min Y are flipped since positive y is down
        minTileThread = new Vector2Int(minXID * roomChunks[0].width, maxYID * roomChunks[0].height + roomChunks[0].height - 1);
        maxTileThread = new Vector2Int(maxXID * roomChunks[0].width + roomChunks[0].width - 1, minYID * roomChunks[0].height);

        Debug.Log($"minTileThread: {minTileThread} maxTileThread: {maxTileThread}");
        for (int x = minTileThread.x; x <= maxTileThread.x; x++)
        {
            for(int y = maxTileThread.y; y <= minTileThread.y; y++)
            {
                boolMapThread[x - minTileThread.x, y - maxTileThread.y] = roomChunks[0].GetTile(x, y);
                
            }
        }

    }

    //bool locomotionGraphSetupComplete = false;
    private void SetOnLocomotionGraphComplete()
    {
        locomotionGraphSetupComplete = false;
        onLocomotionGraphSetupComplete += LocomotionGraphSetupComplete;
    }
    public void LocomotionGraphSetupComplete()
    {
        locomotionGraphSetupComplete = true;
        onLocomotionGraphSetupComplete -= LocomotionGraphSetupComplete;
    }

    public void SetRoomChunk(List<Chunk> roomChunks, int seed)
    {
        SetOnLocomotionGraphComplete();
        this.seed = seed;

        GenerateFromChunks(roomChunks);

        Thread thread = new Thread(SetRoomChunkThread);
        thread.Start();


    }
    
    public void SetRoomChunk (string bitmapFilename, int seed)
    {
        IEnumerable<string> file = File.ReadLines(bitmapFilename);
        List<List<bool>> listBoolMap = new List<List<bool>>();
        foreach (string line in file)
        {
            List<bool> rowBoolMap = new List<bool>();
            foreach (char bit in line)
            {
                if (bit == '0') rowBoolMap.Add(false);
                else rowBoolMap.Add(true);
            }

            listBoolMap.Add(rowBoolMap);

        }

        //List<List<bool>> listBoolMap = new List<List<bool>>();
        int height = listBoolMap.Count;
        int width = listBoolMap[0].Count;
        bool[,] boolMap = new bool[width, height];
        for (int x = 0; x < width; x += 1)
        {
            for (int y = 0; y < height; y += 1)
            {
                boolMap[x, y] = listBoolMap[y][x];
            }
        }
        SetRoomChunk(boolMap, seed);
    }

    public void SetRoomChunk (bool[,] boolMap, int seed)
    {
        // assign LocomotionGraphSetupComplete to delegate LcocomotionGraph.LocomotionGraphSetupComplete
        SetOnLocomotionGraphComplete();


        this.seed = seed;
        boolMapThread = boolMap;
        Thread thread = new Thread(SetRoomChunkThread);
        thread.Start();
    }

    
}
