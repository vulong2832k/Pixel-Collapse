using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MultiPoolItem
{
    public string id;
    public GameObject prefab;
    public int initialCount = 100;
}

public class MultiPool : MonoBehaviour
{
    public List<MultiPoolItem> poolItems = new List<MultiPoolItem>();

    private Dictionary<string, Queue<GameObject>> poolDictionary = new Dictionary<string, Queue<GameObject>>();

    void Awake()
    {
        foreach (var item in poolItems)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < item.initialCount; i++)
            {
                GameObject obj = Instantiate(item.prefab);
                obj.SetActive(false);
                obj.transform.parent = this.transform;
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(item.id, objectPool);
        }
    }

    public GameObject GetFromPool(string id, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(id))
        {
            return null;
        }

        GameObject obj;
        if (poolDictionary[id].Count > 0)
        {
            obj = poolDictionary[id].Dequeue();
        }
        else
        {
            MultiPoolItem item = poolItems.Find(x => x.id == id);
            obj = Instantiate(item.prefab);
        }

        obj.SetActive(true);
        obj.transform.position = position;
        obj.transform.rotation = rotation;

        return obj;
    }

    public void ReturnToPool(string id, GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.parent = this.transform;
        if (poolDictionary.ContainsKey(id))
        {
            poolDictionary[id].Enqueue(obj);
        }
        else
        {
            Destroy(obj);
        }
    }
}