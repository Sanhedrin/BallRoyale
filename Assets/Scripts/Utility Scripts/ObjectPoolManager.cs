using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Networking;

[AddComponentMenu("BallGame Scripts/Utilities/Object Pool Manager")]
[Serializable]
public class ObjectPoolManager : NetworkBehaviour
{
    [SerializeField]
    private static ObjectPoolManager m_Instance = null;
    public static ObjectPoolManager Instance
    {
        get
        {
            return m_Instance;
        }
    }

    public List<GOListWrapper> ObjectsToPool = new List<GOListWrapper>();
    public List<eObjectPoolNames> ObjectPoolNames = new List<eObjectPoolNames>();
    public List<IntListWrapper> ObjectPoolStartAmounts = new List<IntListWrapper>();
    public List<int> SubListSizes = new List<int>();

    [SerializeField]
    private Dictionary<eObjectPoolNames, GameObjectPool> m_ObjectPoolDictionary = new Dictionary<eObjectPoolNames, GameObjectPool>();

    void Start()
    {
        if (isServer)
        {
            m_Instance = this;

            if (GameObject.FindGameObjectsWithTag(ConstParams.ObjectPoolManager).Length > 1)
            {
                Debug.LogError("Can't have more than one Object Pool Manager in a scene.");
            }

            for(int i = 0; i < ObjectPoolStartAmounts.Count; ++i)
            {
                m_ObjectPoolDictionary.Add(ObjectPoolNames[i], null);

                for (int j = 0; j < ObjectsToPool[i].InnerList.Count; ++j)
                {
                    m_ObjectPoolDictionary[ObjectPoolNames[i]] = new GameObjectPool(ObjectsToPool[i].InnerList[j], ObjectPoolStartAmounts[i].InnerList[j]);
                }
            }
        }
    }

    [Server]
    public GameObjectPool GetPoolForObject(eObjectPoolNames i_ObjectName)
    {
        return m_ObjectPoolDictionary[i_ObjectName];
    }
	
}
