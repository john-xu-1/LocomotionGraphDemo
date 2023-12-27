using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChunkHandler
{
    public class ChunkObjectEnemy : ChunkObject
    {
        private void OnDestroy()
        {
            mychunk.RemoveChunkObject(this);
        }
        public override void Load()
        {
            Chunk currentChunk = mychunk.GetChunk(transform.position);
            if (currentChunk == mychunk)
            {
                gameObject.SetActive(true);
                Debug.Log($"loading {gameObject.name}");
            }
            else
            {
                mychunk.RemoveChunkObject(this);
                currentChunk.AddChunkObject(this);
                Debug.Log($"wrong Load chunk {currentChunk.chunkID} vs {mychunk.chunkID}");
                mychunk = currentChunk;
            }
        }

        public override void Unload()
        {
            Chunk currentChunk = mychunk.GetChunk(transform.position);
            if (currentChunk == mychunk)
            {
                gameObject.SetActive(false);
                Debug.Log($"unloading {gameObject.name}");
            }
            else
            {
                mychunk.RemoveChunkObject(this);
                currentChunk.AddChunkObject(this);
                Debug.Log($"wrong Unload chunk {currentChunk.chunkID} vs {mychunk.chunkID}");
                mychunk = currentChunk;
            }
        }
    }
}