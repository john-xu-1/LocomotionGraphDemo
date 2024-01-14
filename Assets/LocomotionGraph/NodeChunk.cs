using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LocomotionGraph
{
    public abstract class NodeChunk
    {
        public int nodeID;
        public List<Vector2Int> jumpTiles = new List<Vector2Int>();
        public int[,] path;
        public List<int> connectedPlatforms;
        protected RoomChunk roomChunk;
        public List<NodeChunk> defaultSources = new List<NodeChunk>();
        public bool checkConnection;

        public virtual void SetPath(int platformID, int jumpHeight)
        {

        }

        public Vector2Int GetTilePos(Vector2Int tile)
        {
            return new Vector2Int(tile.x + roomChunk.minTile.x, -tile.y + 1 - roomChunk.maxTile.y);
        }

        public virtual Vector2Int GetFluidEdge(int sinkID)
        {
            return default;
        }

        public virtual Vector2Int GetCenterTile()
        {
            return jumpTiles[0];
        }

        public virtual Vector2Int GetCenterTilePos()
        {
            return GetTilePos(GetCenterTile());
        }


        /// <summary>
        /// Returns true if connectedID was valid and thus removed.
        /// </summary>
        /// <param name="connectedID"></param>
        /// <returns></returns>
        public virtual bool RemoveConnectedPlatform(int connectedID)
        {
            if (connectedPlatforms.Contains(connectedID))
            {
                connectedPlatforms.Remove(connectedID);
                return true;
            }
            else
            {
                Debug.LogWarning($"invalid connectedID({connectedID}) to be removed.");
                return false;
            }
        }

        public virtual bool AddConnectPlatform(int connectedID)
        {
            if (!connectedPlatforms.Contains(connectedID))
            {
                connectedPlatforms.Add(connectedID);
                return true;
            }
            else
            {
                Debug.LogWarning($"connectedID({connectedID}) is already in connectedPlatoforms");
                return false;
            }
        }
    }
}

