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
}
