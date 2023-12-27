using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChunkHandler
{
    public abstract class ChunkObject : MonoBehaviour
    {
        [SerializeField] GameObject myObjectPrefab;
        GameObject myObject;
        bool hasObject;
        List<GameObject> myObjectPool;

        protected bool initialized;
        public abstract void Load();
        public abstract void Unload();

        public Chunk mychunk;
        public int radius;

        public void SetMyObjectPool(List<GameObject> objectPool)
        {
            myObjectPool = objectPool;
        }
        public GameObject GetMyObject()
        {
            if (!hasObject)
            {
                return PopObjectPool();
            }
            else
            {
                return myObject;
            }
        }
        public GameObject PopObjectPool()
        {
            GameObject go;
            if (myObjectPool.Count > 0)
            {
                go = myObjectPool[0];
                myObjectPool.RemoveAt(0);
            }
            else
            {
                go = Instantiate(myObjectPrefab);
            }
            return go;
        }
    }
}