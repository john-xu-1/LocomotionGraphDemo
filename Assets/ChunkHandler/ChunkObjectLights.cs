using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChunkHandler
{
    public class ChunkObjectLights : ChunkObject
    {
        public List<GameObject> LightPrefabs;
        public static List<List<GameObject>> lightsObjectPool;

        List<int> lightTypes = new List<int>();
        List<Vector2> lightPositions = new List<Vector2>();
        List<GameObject> myObjects = new List<GameObject>();

        // Start is called before the first frame update
        void Start()
        {
            if (lightsObjectPool == null)
            {
                lightsObjectPool = new List<List<GameObject>>();
                for (int i = 0; i < LightPrefabs.Count; i += 1)
                {
                    lightsObjectPool.Add(new List<GameObject>());
                }
            }
        }

        public void AddLight(int type, Vector2 pos)
        {
            lightTypes.Add(type);
            lightPositions.Add(pos);
        }

        bool loaded = false;
        public override void Load()
        {
            if (loaded) return;
            loaded = true;
            for (int i = 0; i < lightTypes.Count; i += 1)
            {
                GameObject light = GetLight(lightTypes[i]);
                light.transform.position = lightPositions[i];
                light.SetActive(true);
                if (myObjects.Count <= i)
                {
                    myObjects.Add(light);
                }
                else
                {
                    myObjects[i] = light;
                }
            }

        }

        public override void Unload()
        {
            if (!loaded) return;
            loaded = false;
            for (int i = lightTypes.Count - 1; i >= 0; i -= 1)
            {
                if (myObjects.Count > i)
                {
                    ReturnLight(lightTypes[i], myObjects[i]);
                    myObjects.RemoveAt(i);
                }

            }
        }

        GameObject GetLight(int type)
        {
            if (lightsObjectPool[type].Count > 0)
            {
                GameObject light = lightsObjectPool[type][0];
                lightsObjectPool[type].RemoveAt(0);
                return light;
            }
            else
            {
                return Instantiate(LightPrefabs[type]);
            }
        }
        void ReturnLight(int type, GameObject light)
        {
            lightsObjectPool[type].Add(light);
            light.SetActive(false);
        }

    }
}