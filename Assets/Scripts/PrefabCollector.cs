using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabCollector
{
    public string prefabTag;

    public PrefabCollector(string prefabTag)
    {
        this.prefabTag = prefabTag;
    }

    public List<Vector3> CollectPrefabPositions()
    {
        GameObject[] prefabs = GameObject.FindGameObjectsWithTag(prefabTag);
        List<Vector3> positions = new List<Vector3>();

        foreach (var prefab in prefabs)
        {
            positions.Add(new Vector3(Mathf.RoundToInt(prefab.transform.position.x), Mathf.RoundToInt(prefab.transform.position.y), 0));
        }

        return positions;
    }


}
