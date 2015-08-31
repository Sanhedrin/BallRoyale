using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

public static class ExtensionMethods {

    public static NetworkInstanceId NetID<T>(this T i_Object) where T : Component
    {
        NetworkInstanceId netId = new NetworkInstanceId();

        try
        {
            netId = i_Object.GetComponent<NetworkBehaviour>().netId;
        }
        catch (Exception e)
        {
            Debug.LogError(string.Format("{0} is not a network object: {1}", i_Object, e.Message));
        }

        return netId;
    }

    public static bool IsEmpty(this ICollection i_Collection)
    {
        return i_Collection.Count == 0;
    }

    public static void ShiftLeft<T>(this List<T> i_List)
    {
        for (int i = 0; i < i_List.Count - 1; ++i)
        {
            i_List[i] = i_List[i + 1];
        }

        i_List.RemoveAt(i_List.Count - 1);
    }

    public static float AverageUpdateTime(this SyncList<NetworkState> i_NetStates)
    {
        float average = 0;

        for (int i = 1; i < i_NetStates.Count; ++i)
        {
            average += i_NetStates[i].UpdateTime - i_NetStates[i-1].UpdateTime;
        }

        average /= i_NetStates.Count;

        return average;
    }
}
