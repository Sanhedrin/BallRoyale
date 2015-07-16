using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerNetworkStatus : NetworkBehaviour 
{
    void Start()
    {
    }

    private override void OnStopClient(this NetworkManager i_NetManager)
    {
        NetworkManager.singleton.OnStopClient();
    }
}
