using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace LocomotionGraph
{
    public class LocomotionTilemapGraph : MonoBehaviour
    {

        public Tilemap tilemap;
        public TileBase tileBase;
        public string levelBitmap;
        // Start is called before the first frame update
        void Start()
        {
            string filename = NoiseTerrain.Utility.GetPathToFile(levelBitmap);
            BuildTilemap(filename);

            FindObjectOfType<LocomotionChunkGraph>().SetRoomChunk(filename, 0);
            //StartCoroutine(SolveLocomotionGraph());
        }

        public void StartSolveLocomotion()
        {
            StartCoroutine(SolveLocomotionGraph());
        }

        public void BuildTilemap(string levelBitmapFilename)
        {
            List<List<bool>> boolmap = Utility.GetBoolMap(levelBitmapFilename);

            for (int y = 0; y < boolmap.Count; y++)
            {
                for (int x = 0; x < boolmap[0].Count; x++)
                {
                    if (boolmap[y][x]) tilemap.SetTile(new Vector3Int(x, -y, 0), tileBase);
                }
            }
        }

        public IEnumerator SolveLocomotionGraph()
        {
            LocomotionGraph locomotionGraph = FindObjectOfType<LocomotionChunkGraph>();
            StartCoroutine(locomotionGraph.SolveLocomotionGraph());
            while (locomotionGraph.generatingLocomotionGraph)
            {
                yield return null;
            }

        }
    }
}

