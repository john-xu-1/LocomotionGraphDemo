using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LocomotionGraph
{
    [CustomEditor(typeof(LocomotionTilemapGraph))]
    public class LocomotionTilemapGraphEditor : Editor
    {
        LocomotionTilemapGraph myTarget;
        private void OnEnable()
        {
            myTarget = (LocomotionTilemapGraph)target;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if (GUILayout.Button("Solve Locomotion Graph"))
            {
                myTarget.StartSolveLocomotion();    
            }
            if (GUILayout.Button ("Diplsay Nodes"))
            {
                foreach (FilledChunk filledChunk in FindObjectOfType<LocomotionChunkGraph>().RoomChunk.filledChunks)
                {
                    foreach (PlatformChunk platformChunk in filledChunk.platforms)
                    {
                        if (platformChunk.jumpTiles.Count > 0)
                        {
                            string poiText = $"{platformChunk.nodeID.ToString()}\n";
                            FindObjectOfType<POIHandler>().SetPOI(platformChunk.GetCenterTilePos() /*GetTilePos(platformChunk.jumpTiles[0]) */, poiText, new Color(1f/255,50f/255,32f/255));
                        }
                        else Debug.LogWarning($"NodeID {platformChunk.nodeID} is missing jumpTiles");
                    }
                }
            }
        }
    }
}

