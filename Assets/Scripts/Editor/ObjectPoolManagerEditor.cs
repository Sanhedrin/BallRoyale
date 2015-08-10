using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(ObjectPoolManager))]
public class ObjectPoolManagerEditor : Editor {
    
    private int m_ListSize = 0;
    
    private ObjectPoolManager poolManager;

    private bool m_PoolListsExpanded = true;
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

        if (m_ListSize != poolManager.ObjectPoolNames.Count)
        {
            List<eObjectPoolNames> lastPoolNames = new List<eObjectPoolNames>(poolManager.ObjectPoolNames.GetRange(0, m_ListSize <= poolManager.ObjectPoolNames.Count ? m_ListSize : poolManager.ObjectPoolNames.Count));
            List<GameObject> lastPoolObjects = new List<GameObject>(poolManager.ObjectsToPool.GetRange(0, m_ListSize <= poolManager.ObjectsToPool.Count ? m_ListSize : poolManager.ObjectsToPool.Count));

            poolManager.ObjectsToPool = new List<GameObject>(m_ListSize);
            poolManager.ObjectPoolNames = new List<eObjectPoolNames>(m_ListSize);

            poolManager.ObjectPoolNames.AddRange(lastPoolNames);
            poolManager.ObjectsToPool.AddRange(lastPoolObjects);
        }

        EditorGUILayout.EndHorizontal();

        if (m_PoolListsExpanded = EditorGUILayout.Foldout(m_PoolListsExpanded, "Items"))
        {
            for (int i = 0; i < m_ListSize; ++i)
            {
                EditorGUILayout.BeginHorizontal();

                if (poolManager.ObjectsToPool.Count > i)
                {
                    poolManager.ObjectsToPool[i] = EditorGUILayout.ObjectField(poolManager.ObjectsToPool[i], typeof(GameObject), !v_AllowSceneObjects) as GameObject;
                }
                else
                {
                    poolManager.ObjectsToPool.Add(null);
                }

                if (poolManager.ObjectPoolNames.Count > i)
                {
                    poolManager.ObjectPoolNames[i] = (eObjectPoolNames)EditorGUILayout.EnumPopup(poolManager.ObjectPoolNames[i]);
                }
                else
                {
                    poolManager.ObjectPoolNames.Add(eObjectPoolNames.Bullet);
                }

                EditorGUILayout.EndHorizontal();
            }
        }

        EditorGUILayout.EndVertical();

        if (GUI.changed)
        {
            serializedObject.ApplyModifiedProperties();
        }
    }
}
