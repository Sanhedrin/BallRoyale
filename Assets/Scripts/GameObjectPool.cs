﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


    public class GameObjectPool
    {
        private List<GameObject> m_GameObjectList = new List<GameObject>();

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
            GameObject newGameobj = GameObject.Instantiate(m_PoolObj);
            newGameobj.SetActive(false);
            m_GameObjectList.Add(newGameobj);
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
                objToPull = m_GameObjectList[m_GameObjectList.Count - 1];
            }

            return objToPull;  
        }
    }

