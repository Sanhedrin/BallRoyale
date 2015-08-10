using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ObjectPoolManager : MonoBehaviour
{
    public List<GameObject> ObjectsToPool = new List<GameObject>();
    public List<eObjectPoolNames> ObjectPoolNames = new List<eObjectPoolNames>();
    
    private Dictionary<eObjectPoolNames, GameObjectPool> m_ObjectPoolDictionary = new Dictionary<eObjectPoolNames, GameObjectPool>();  

    public ObjectPoolManager()
    {
        
    }   
	
}
