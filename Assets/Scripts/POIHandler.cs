using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class POIHandler : MonoBehaviour
{
    public POI prefab;

    List<POI> pool = new List<POI>();
    List<POI> used = new List<POI>();

    public int SetPOI(Vector2 pos, string label, Color color)
    {
        POI poi = Get();
        poi.gameObject.SetActive(true);
        poi.Setup(label, color);
        poi.transform.position = pos;

        used.Add(poi);
        return used.Count - 1;
    }

    public void ReturnPOI(POI poi)
    {
        Return(poi);
    }

    public void ReturnPOI(int poiID)
    {
        if(poiID < used.Count && used[poiID] != null)
            Return(used[poiID]);
        else
        {
            Debug.LogWarning($"poiID {poiID} does not exist");
        }
    }

    private POI Get()
    {
        if (pool.Count > 0)
        {
            POI poi = pool[0];
            pool.RemoveAt(0);
            return poi;
        }
        return Instantiate(prefab);
    }

    private void Return(POI poi)
    {
        pool.Add(poi);
        poi.gameObject.SetActive (false);
        used.Remove(poi);
    }
}
