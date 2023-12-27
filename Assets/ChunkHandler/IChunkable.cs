using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChunkHandler
{
    interface IChunkable
    {
        void GenerateFromChunks(List<Chunk> chunks);
}
}

