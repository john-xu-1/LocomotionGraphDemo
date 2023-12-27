using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LocomotionGraph
{
    public class WallChunk : NodeChunk
    {
        public WallChunk(RoomChunk roomChunk)
        {
            this.roomChunk = roomChunk;
        }

        public static bool IsValidWall(Vector2Int wallTile, int jumpHeight, bool rightSide, RoomChunk roomChunk)
        {
            int xOffset = rightSide ? 1 : -1;

            // valid wall tils is when there is at least one attached tile that is jumpheight + 1 above the ground

            // valid wall + offset --> count down till zero if not ground (empty) then true else

            // return to org position and go up and count down till zero, if wall exists and surface, true, else false

            int count = jumpHeight;
            int x = wallTile.x;
            int y = wallTile.y;
            Debug.Log($"x:{x} y:{y}");
            if (roomChunk.FilledTile(x + xOffset, y))
            {
                return false;
            }

            //move down to find ground
            while (count > 0 && !roomChunk.FilledTile(x + xOffset, y))
            {
                count--;
                y++; //down is positive
            }

            //either found ground or wall tile is higher than jumpheight from ground
            if (count == 0 && !roomChunk.FilledTile(x + xOffset, y)) return true;
            

            //move up from ground to find valid tile above ground
            y = wallTile.y - 1;
            while (count > 0 && !roomChunk.FilledTile(x + xOffset, y) && roomChunk.FilledTile(x, y))
            {
                count--;
                y--; //up is negative
            }

            if (count == 0 && !roomChunk.FilledTile(x + xOffset, y) && roomChunk.FilledTile(x, y)) return true;

            return false;
        }

    }

}
