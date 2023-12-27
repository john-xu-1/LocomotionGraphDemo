using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LocomotionGraph
{
    public class WallChunkDebugger : MonoBehaviour
    {
        public static WallChunkDebugger singleton;
        List<int> wallIDs = new List<int>();
        List<Vector2Int> walls = new List<Vector2Int>();
        public bool displayWalls;
        public Color[] colors;


        private void Start()
        {
            singleton = this;
        }

        public void SetWalls(List<Vector2Int> walls, bool display)
        {
            this.walls = walls;
            displayWalls = display;
        }

        public void AddWalls(List<Vector2Int> walls, List<int> wallIDs, bool display)
        {
            for (int i = 0; i < walls.Count; i++)
            {
                if (this.walls.Contains(walls[i])) Debug.LogWarning($"{walls[i]} already in walls.");
                this.walls.Add(walls[i]);
                this.wallIDs.Add(wallIDs[i]);
            }


            displayWalls = display;
        }

        public void ClearWalls()
        {
            walls.Clear();
        }

        private void OnDrawGizmos()
        {
            if (!displayWalls) return;
            for (int i = 0; i < walls.Count; i++)
            {
                Color color = colors[wallIDs[i] % colors.Length];
                Gizmos.color = color;
                Gizmos.DrawCube(new Vector3(walls[i].x + 0.5f, -walls[i].y + 0.5f, 0), new Vector3(1, 1, 1));
            }
        }
    }

}
