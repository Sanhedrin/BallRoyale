using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Networking;

[AddComponentMenu("BallGame Scripts/Utilities/Object Pool Manager")]
public class ObjectPoolManager : NetworkBehaviour
{
    private static ObjectPoolManager m_Instance = null;
    public static ObjectPoolManager Instance
    {
        get
        {
            return m_Instance;
        }
    }


    public List<GameObject> ObjectsToPool = new List<GameObject>();
    public List<eObjectPoolNames> ObjectPoolNames = new List<eObjectPoolNames>();
    
    private Dictionary<eObjectPoolNames, GameObjectPool> m_ObjectPoolDictionary = new Dictionary<eObjectPoolNames, GameObjectPool>();

    void Start()
    {
        if (isServer)
        {
            m_Instance = this;

            if (GameObject.FindGameObjectsWithTag(ConstNames.ObjectPoolManager).Length > 1)
            {
                Debug.LogError("Can't have more than one Object Pool Manager in a scene.");
            }

            foreach (eObjectPoolNames key in ObjectPoolNames)
            {
                m_ObjectPoolDictionary.Add(key, new GameObjectPool(ObjectsToPool[(int)key], 10));
            }
        }
    }

    [Server]
    public GameObjectPool GetPoolForObject(eObjectPoolNames i_ObjectName)
    {
        return m_ObjectPoolDictionary[i_ObjectName];
    }
	
}
