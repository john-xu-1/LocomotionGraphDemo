using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace NoiseTerrain
{
    [CustomEditor(typeof(MapGenerator))]
    public class MapGeneratorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            MapGenerator mapGenerator = (MapGenerator)target;

            if (DrawDefaultInspector())
            {
                if (mapGenerator.autoUpdate)
                {
                    mapGenerator.GenerateMap();
                }
            }
            if (GUILayout.Button("Generate Map"))
            {
                mapGenerator.GenerateMap();
            }
            if (GUILayout.Button("Clear Map"))
            {
                mapGenerator.ClearMap();
            }
        }
    }

    [CustomEditor(typeof(ProceduralMapGenerator))]
    public class ProceduralMapGeneratorEditor : MapGeneratorEditor { }
}
