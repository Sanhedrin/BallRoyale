using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

[AddComponentMenu("BallGame Scripts/Network/Ball Game Network Lobby Manager")]
public class BallGameNetworkLobbyManager : NetworkLobbyManager 
{
    public override void OnServerReady(NetworkConnection conn)
    {
        base.OnServerReady(conn);
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
    }
}
