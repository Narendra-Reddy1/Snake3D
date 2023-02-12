using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;

public class PoolHandler : MonoBehaviour
{
    //Allows you to have any number of pools with its own key
    Dictionary<string, List<GameObject>> mPools = new Dictionary<string, List<GameObject>>();

    Transform poolParent;

    Transform pPoolParent
    {
        get
        {
            if (poolParent == null)
            {
                poolParent = GameObject.FindObjectOfType<PoolHandler>().transform;
            }

            return poolParent;
        }
    }

    public static PoolHandler instance { get; private set; } = null;

    void Awake()
    {
        instance = this;
        DontDestroyOnLoad(instance);
    }

    /// <summary>
    /// Create a pool if one dosent exist
    /// </summary>
    /// <param name="name"></param>
    /// <param name="size"></param>
    /// <param name="template"></param>
    public void CreatePool(string name, byte size, GameObject template)
    {
        List<GameObject> pool = new List<GameObject>();
        GameObject currentObject;
        for (int i = 0; i < size; i++)
        {
            currentObject = Instantiate(template, pPoolParent, true);
            currentObject.SetActive(false);
            pool.Add(currentObject);
        }

        mPools.Add(name, pool);
    }

    //Created just for this snake task
    public void CreatePoolOverNetworkForSceneObjects(string name, byte size, GameObject template, Transform parent, bool worldPositionStays = true)
    {
        List<GameObject> pool = new List<GameObject>();
        GameObject currentObject;
        for (int i = 0; i < size; i++)
        {
            currentObject = PhotonNetwork.InstantiateRoomObject(template.name, Vector3.zero, Quaternion.identity);
            currentObject.name = template.name;
            currentObject.transform.SetParent(parent, worldPositionStays);
            currentObject.SetActive(false);
            pool.Add(currentObject);
        }
        mPools.Add(name, pool);
    }
    public void CreatePool(string name, byte size, GameObject template, Transform parent, bool worldPositionStays = true)
    {
        List<GameObject> pool = new List<GameObject>();
        GameObject currentObject;
        for (int i = 0; i < size; i++)
        {
            currentObject = Instantiate(template, parent, worldPositionStays);
            currentObject.SetActive(false);
            pool.Add(currentObject);
        }
        mPools.Add(name, pool);
    }
    /// <summary>
    /// Removes the given unused pool from the persistent list. If it is exists.
    /// </summary>
    /// <param name="poolName"></param>
    public void DestroyPool(string poolName)
    {
        if (mPools.ContainsKey(poolName))
        {
            foreach (GameObject go in mPools[poolName])
            {
                Destroy(go);
            }
            mPools.Remove(poolName);
        }
    }

    /// <summary>
    /// Cleanup the pool without destroying the objects to be reused later
    /// </summary>
    /// <param name="name"></param>
    public void DeSpawnPool(string name)
    {
        if (mPools.ContainsKey(name))
        {
            List<GameObject> pool = mPools[name];

            for (int i = 0; i < pool.Count; i++)
            {
                pool[i].transform.SetParent(pPoolParent);
                pool[i].SetActive(false);
            }
        }
    }

    /// <summary>
    /// Fetch an element from the pool
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public GameObject SpawnElementFromPool(string name, string objectKey, bool checkDuplicates = false)
    {
        if (mPools.ContainsKey(name))
        {
            List<GameObject> pool = mPools[name];
            GameObject poolItem = null;

            if (checkDuplicates)
            {
                poolItem = GetActiveInPool(name, objectKey);
                if (poolItem != null)
                {
                    return poolItem;
                }
            }

            poolItem = pool.Find(e => !e.activeInHierarchy);

            if (poolItem == null)
            {
                poolItem = Instantiate(pool[0], pPoolParent, true);
                poolItem.SetActive(true);
                pool.Add(poolItem);
                mPools[name] = pool;
            }
            poolItem.name = objectKey;

            return poolItem;
        }

        return null;
    }

    public bool PoolExists(string name)
    {
        return mPools.ContainsKey(name);
    }

    GameObject GetActiveInPool(string poolName, string itemName)
    {
        if (mPools.ContainsKey(poolName))
        {
            return mPools[poolName].Find(e => e.activeInHierarchy && e.name.Equals(itemName));
        }
        return null;
    }
}
