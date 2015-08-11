using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Random = UnityEngine.Random;

[Serializable]
public class GameObjectPool 
{
    [SerializeField]
    private List<GameObject> m_GameObjectList = new List<GameObject>();

    [SerializeField]
    private GameObject m_PoolObj = null;

    public GameObjectPool(GameObject i_GameObjForPool, int i_Amount = 0)
    { 
        m_PoolObj = i_GameObjForPool;

        for (int i = 0; i < i_Amount; i++)
        {
            addToPool();
        }
    }

    private void addToPool()
    {
        GameObject newGameobj = GameObject.Instantiate(m_PoolObj, new Vector3(-1337, -1337, -1337), Quaternion.identity) as GameObject;
        newGameobj.SetActive(false);
        m_GameObjectList.Add(newGameobj);
        NetworkServer.Spawn(newGameobj);
    }

    public GameObject PullObject()
    {
        GameObject objToPull = null;
        foreach (GameObject obj in m_GameObjectList)
        {
            if (!obj.activeInHierarchy)
            {
                obj.SetActive(true);
                objToPull = obj;
                break;
            }
        }

        if (objToPull == null)
        {
            addToPool();
            objToPull = m_GameObjectList[Random.Range(0, m_GameObjectList.Count - 1)];
        }

        return objToPull; 
    }
}

