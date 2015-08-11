using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;

[CustomEditor(typeof(ObjectPoolManager))]
[Serializable]
public class ObjectPoolManagerEditor : Editor
{

    [SerializeField]
    private int m_ListSize = 0;

    [SerializeField]
    private ObjectPoolManager poolManager;

    [SerializeField]
    private bool m_PoolListsExpanded = true;
    [SerializeField]
    private List<bool> m_SubPoolListsExpanded = new List<bool>() { false };
    [SerializeField]
    private const bool v_AllowSceneObjects = true;

    void OnEnable()
    {
        poolManager = target as ObjectPoolManager;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        GUI.changed = false;

        EditorGUILayout.BeginVertical();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Amount of pools:");

        m_ListSize = EditorGUILayout.IntField(poolManager.ObjectPoolNames.Count);

        EditorGUILayout.EndHorizontal();

        if (m_ListSize != poolManager.ObjectsToPool.Count)
        {
            List<eObjectPoolNames> lastPoolNames = new List<eObjectPoolNames>(poolManager.ObjectPoolNames.GetRange(0, m_ListSize <= poolManager.ObjectPoolNames.Count ? m_ListSize : poolManager.ObjectPoolNames.Count));
            List<GOListWrapper> lastPoolObjects = new List<GOListWrapper>(poolManager.ObjectsToPool.GetRange(0, m_ListSize <= poolManager.ObjectsToPool.Count ? m_ListSize : poolManager.ObjectsToPool.Count));
            List<IntListWrapper> lastPoolSizes = new List<IntListWrapper>(poolManager.ObjectPoolStartAmounts.GetRange(0, m_ListSize <= poolManager.ObjectPoolStartAmounts.Count ? m_ListSize : poolManager.ObjectPoolStartAmounts.Count));
            List<int> lastSubListSizes = new List<int>(poolManager.SubListSizes.GetRange(0, m_ListSize <= poolManager.SubListSizes.Count ? m_ListSize : poolManager.SubListSizes.Count));

            poolManager.ObjectsToPool = new List<GOListWrapper>(m_ListSize);
            poolManager.ObjectPoolStartAmounts = new List<IntListWrapper>(m_ListSize);
            poolManager.ObjectPoolNames = new List<eObjectPoolNames>(m_ListSize);
            poolManager.SubListSizes = new List<int>(m_ListSize);

            poolManager.ObjectPoolNames.AddRange(lastPoolNames);
            poolManager.ObjectsToPool.AddRange(lastPoolObjects);
            for (int i = lastPoolObjects.Count; i < m_ListSize; ++i)
            {
                poolManager.ObjectsToPool.Add(new GOListWrapper());
            }
            poolManager.ObjectPoolStartAmounts.AddRange(lastPoolSizes);
            for (int i = lastPoolSizes.Count; i < m_ListSize; ++i)
            {
                poolManager.ObjectPoolStartAmounts.Add(new IntListWrapper());
            }
            poolManager.SubListSizes.AddRange(lastSubListSizes);
            for (int i = lastSubListSizes.Count; i < m_ListSize; ++i)
            {
                poolManager.SubListSizes.Add(0);
            }
        }

        if (m_PoolListsExpanded = EditorGUILayout.Foldout(m_PoolListsExpanded, "Object Pool"))
        {
            for (int i = 0; i < m_ListSize; ++i)
            {
                EditorGUILayout.BeginHorizontal();

                if (poolManager.ObjectPoolNames.Count > i)
                {
                    poolManager.ObjectPoolNames[i] = (eObjectPoolNames)EditorGUILayout.EnumPopup(poolManager.ObjectPoolNames[i]);
                }
                else
                {
                    poolManager.ObjectPoolNames.Add(eObjectPoolNames.Bullet);
                }

                poolManager.SubListSizes[i] = EditorGUILayout.IntField(poolManager.ObjectsToPool[i].InnerList.Count);

                for (int j = 0; j < m_ListSize; ++j)
                {
                    if (poolManager.SubListSizes[i] != poolManager.ObjectsToPool[j].InnerList.Count)
                    {
                        List<int> lastSubPoolSizes = new List<int>(poolManager.ObjectPoolStartAmounts[j].InnerList.GetRange(0, poolManager.SubListSizes[j] <= poolManager.ObjectPoolStartAmounts[j].InnerList.Count ? poolManager.SubListSizes[j] : poolManager.ObjectPoolStartAmounts[j].InnerList.Count));
                        List<GameObject> lastSubObjectsToPool = new List<GameObject>(poolManager.ObjectsToPool[j].InnerList.GetRange(0, poolManager.SubListSizes[j] <= poolManager.ObjectsToPool[j].InnerList.Count ? poolManager.SubListSizes[j] : poolManager.ObjectsToPool[j].InnerList.Count));

                        poolManager.ObjectPoolStartAmounts[j] = new IntListWrapper(poolManager.SubListSizes[j]);
                        poolManager.ObjectsToPool[j] = new GOListWrapper(poolManager.SubListSizes[j]);

                        poolManager.ObjectPoolStartAmounts[j].InnerList.AddRange(lastSubPoolSizes);
                        for (int k = lastSubPoolSizes.Count; k < poolManager.SubListSizes[j]; ++k)
                        {
                            poolManager.ObjectPoolStartAmounts[j].InnerList.Add(0);
                        }
                        poolManager.ObjectsToPool[j].InnerList.AddRange(lastSubObjectsToPool);
                        for (int k = lastSubObjectsToPool.Count; k < poolManager.SubListSizes[j]; ++k)
                        {
                            poolManager.ObjectsToPool[j].InnerList.Add(null);
                        }
                    }
                }


                EditorGUILayout.EndHorizontal();

                while (m_SubPoolListsExpanded.Count <= i)
                {
                    m_SubPoolListsExpanded.Add(false);
                }
                if (m_SubPoolListsExpanded[i] = EditorGUILayout.Foldout(m_SubPoolListsExpanded[i], "Pooled Objects"))
                {
                    for (int j = 0; j < poolManager.ObjectsToPool[i].InnerList.Count; ++j)
                    {
                        EditorGUILayout.BeginHorizontal();

                        if (poolManager.ObjectPoolStartAmounts[i].InnerList.Count > i)
                        {
                            poolManager.ObjectPoolStartAmounts[i].InnerList[j] = EditorGUILayout.IntField(poolManager.ObjectPoolStartAmounts[i].InnerList[j]);
                        }
                        else
                        {
                            poolManager.ObjectPoolStartAmounts[i].InnerList.Add(0);
                        }

                        if (poolManager.ObjectsToPool[i].InnerList.Count > i)
                        {
                            poolManager.ObjectsToPool[i].InnerList[j] = EditorGUILayout.ObjectField(poolManager.ObjectsToPool[i].InnerList[j], typeof(GameObject), !v_AllowSceneObjects) as GameObject;
                        }
                        else
                        {
                            poolManager.ObjectsToPool[i].InnerList.Add(null);
                        }

                        EditorGUILayout.EndHorizontal();
                    }

                    EditorGUILayout.Space();
                }
            }
        }

        EditorGUILayout.EndVertical();

        if (!Application.isPlaying)
        {
            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
