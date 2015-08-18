using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Networking;

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

    public static void ShiftLeft(this Array i_Array)
    {
        for (int i = 0; i < i_Array.Length - 1; ++i)
        {
            i_Array.SetValue(i_Array.GetValue(i + 1), i);
        }

        i_Array.SetValue(null, i_Array.Length - 1);
    }

    public static float AverageUpdateTime(this NetworkState[] i_NetStates, int i_Length)
    {
        float average = 0;

        for (int i = 1; i < i_Length; ++i)
        {
            average += i_NetStates[i].UpdateTime - i_NetStates[i-1].UpdateTime;
        }

        average /= i_Length;

        return average;
    }
}
