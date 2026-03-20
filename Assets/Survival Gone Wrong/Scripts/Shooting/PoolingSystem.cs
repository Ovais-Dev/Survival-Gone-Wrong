using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolingSystem : MonoBehaviour
{
    // object pooled first and can be used generally from here

    [Header("Object")]
    [SerializeField] private GameObject poolObject;
    [SerializeField] private int initialPoolCount;
    [SerializeField] private GameObject poolContainer;

    List<GameObject> poolList = new List<GameObject>();
    void Start()
    {
        InitializedPool();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void InitializedPool()
    {
        for(int i =0; i < initialPoolCount; i++)
        {
            SpawnObject();
        }
    }
    GameObject SpawnObject()
    {
        GameObject poolObj = Instantiate(poolObject,poolContainer.transform);
        poolList.Add(poolObj);
        poolObj.SetActive(false);
        return poolObj;
    }
    public GameObject GetPoolObject()
    {
        foreach(GameObject pObj in poolList)
        {
            if (!pObj.activeInHierarchy)
            {
                return pObj;
            }
        }
        return SpawnObject();
    }
    public void SetPoolingProperties(GameObject obj, int count)
    {
        foreach(GameObject g in poolList)
        {
            Destroy(g);
        }
        poolList.Clear();
        poolObject = obj;
        initialPoolCount = count;
        InitializedPool();
    }
}
