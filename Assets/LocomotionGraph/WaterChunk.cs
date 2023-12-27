using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LocomotionGraph
{
    public class WaterChunk : NodeChunk
    {
        public static List<Vector2Int> FindFloodTiles(Vector2Int start, ChunkHandler.ChunkHandler chunkHandler)
        {
            List<Vector2Int> fillTiles = new List<Vector2Int>();

            List<Vector2Int> to_visit = new List<Vector2Int>();
            to_visit.Add(start);

            while (to_visit.Count > 0)
            {
                Vector2Int cur = to_visit[0];
                to_visit.RemoveAt(0);

                if (!fillTiles.Contains(cur))
                {
                    fillTiles.Add(cur);

                    if (cur.y == start.y)//check left and right for surface flood
                    {
                        Vector2Int left = cur + Vector2Int.left;
                        Vector2Int right = cur + Vector2Int.right;

                        if (!to_visit.Contains(left) && !fillTiles.Contains(left) && !chunkHandler.GetTile(left)) to_visit.Add(left);
                        if (!to_visit.Contains(right) && !fillTiles.Contains(right) && !chunkHandler.GetTile(right)) to_visit.Add(right);
                    }

                    Vector2Int down = cur + Vector2Int.down;
                    if (!to_visit.Contains(down) && !fillTiles.Contains(down) && !chunkHandler.GetTile(down)) to_visit.Add(down);
                    else if (!to_visit.Contains(down) && !fillTiles.Contains(down) && chunkHandler.GetTile(down))
                    {
                        Vector2Int leftRun = cur + Vector2Int.left;
                        Vector2Int rightRun = cur + Vector2Int.right;
                        if (!to_visit.Contains(leftRun) && !fillTiles.Contains(leftRun) && !chunkHandler.GetTile(leftRun)) to_visit.Add(leftRun);
                        if (!to_visit.Contains(rightRun) && !fillTiles.Contains(rightRun) && !chunkHandler.GetTile(rightRun)) to_visit.Add(rightRun);
                    }
                }
            }

            return fillTiles;
        }
    }
}