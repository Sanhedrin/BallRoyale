using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ObjectPoolManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> m_ObjectsToPool = new List<GameObject>();
    [SerializeField]
    private List<eObjectPoolNames> m_ObjectPollNames = new List<eObjectPoolNames>();
    private Dictionary<eObjectPoolNames, GameObjectPool> m_ObjectPoolDictionary = new Dictionary<eObjectPoolNames, GameObjectPool>();  

     void Start()
    {
        if (GameObject.FindGameObjectsWithTag("ObjectPool").Length > 1)
            Debug.LogError("mor then one ObjectPool crated");
        foreach(eObjectPoolNames key in m_ObjectPollNames)
        {
            m_ObjectPoolDictionary[key] = new GameObjectPool(m_ObjectsToPool[(int)key], 10);
        }
    }   
	public GameObjectPool getPool(eObjectPoolNames i_key)
     {
         return m_ObjectPoolDictionary[i_key];
     }
}
