using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChunkHandler
{
    public class ChunkHandler : MonoBehaviour
    {
        public static ChunkHandler singlton;
        private int width, height;
        public void SetUp(int width, int height)
        {
            singlton = this;
            this.width = width;
            this.height = height;
        }

        Dictionary<Vector2Int, Chunk> chunkDict = new Dictionary<Vector2Int, Chunk>();

        
        public bool GetTile(Vector2Int pos)
        {
            int x = ((pos.x % width) + width) % width;
            int y = ((-pos.y % height) + height) % height;
            //Debug.Log($"{x} {y}");
            return GetChunk(GetChunkID(pos)).GetTile(x, y);
        }
        public void AddChunk(Chunk chunk)
        {
            //chunks.Add(chunk);
            chunkDict[chunk.chunkID] = chunk;
        }

        public Chunk GetChunk (Vector3 worldPos)
        {
            return GetChunk(GetChunkID(worldPos));
        }

        public Chunk GetChunk(Vector2Int chunkID)
        {
            //foreach (Chunk chunk in chunks)
            //{
            //    if (chunk.chunkID == chunkID) return chunk;
            //}
            //return null;
            if (chunkDict.ContainsKey(chunkID)) return chunkDict[chunkID];
            else return null;
        }

        public Vector2Int GetChunkID(Vector2 pos)
        {
            int xOffset = pos.x < 0 ? -1 : 0;
            int yOffset = pos.y > 0 ? 1 : 0;

            return new Vector2Int(xOffset + ((int)pos.x - xOffset) / width, -yOffset - ((int)pos.y - yOffset) / height);
        }


        public void SetTile(Vector2Int pos, bool value)
        {
            int x = ((pos.x % width) + width) % width;
            int y = ((-pos.y % height) + height) % height;
            Debug.Log($"{x} {y}");
            GetChunk(GetChunkID(pos)).SetTile(x, y, value);
        }



    }
}

