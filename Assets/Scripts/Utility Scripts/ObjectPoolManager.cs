using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ObjectPoolManager : MonoBehaviour
{
    public List<GameObject> ObjectsToPool = new List<GameObject>();
    public List<eObjectPoolNames> ObjectPoolNames = new List<eObjectPoolNames>();
    
    private Dictionary<eObjectPoolNames, GameObjectPool> m_ObjectPoolDictionary = new Dictionary<eObjectPoolNames, GameObjectPool>();
   
    void Start()
    {
        if (GameObject.FindGameObjectsWithTag("ObjectPool").Length > 1)
        {
            Debug.LogError("Can't have more than one Object Pool Manager in a scene.");
        }

        foreach (eObjectPoolNames key in ObjectPoolNames)
        {
            m_ObjectPoolDictionary.Add(key, new GameObjectPool(ObjectsToPool[(int)key], 10));
        }
    }
    public GameObjectPool GetPoolForObject(eObjectPoolNames i_ObjectName)
    {
        return m_ObjectPoolDictionary[i_ObjectName];
    }
	
}
