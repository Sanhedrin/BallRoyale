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
    public Dictionary<eObjectPoolNames, GameObjectPool> m_ObjectPoolDictionary = new Dictionary<eObjectPoolNames, GameObjectPool>();  

    public ObjectPoolManager()
    {
        
    }   
	
}
