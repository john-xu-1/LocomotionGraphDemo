using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class POIHandler : MonoBehaviour
{
    public POI prefab;

    List<POI> pool = new List<POI>();

    public void SetPOI(Vector2 pos, string label, Color color)
    {
        POI poi = Get();
        poi.gameObject.SetActive(true);
        poi.Setup(label, color);
        poi.transform.position = pos;
    }

    public void ReturnPOI(POI poi)
    {
        Return(poi);
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
    }
}
