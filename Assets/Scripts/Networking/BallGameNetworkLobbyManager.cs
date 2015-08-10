﻿using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

[AddComponentMenu("BallGame Scripts/Network/Ball Game Network Lobby Manager")]
public class BallGameNetworkLobbyManager : NetworkLobbyManager {

    private static Dictionary<int, int> m_ConnectionIDToLocalID = new Dictionary<int, int>();
    private static Dictionary<int, PlayerScript> m_ConnectionIDToPlayer = new Dictionary<int, PlayerScript>();

    /// <summary>
    /// Requests the server to assign client an ID
    /// </summary>
    public static void CmdRequestID(int i_NetID, PlayerScript i_RequestingPlayer)
    {
        int connectionID = i_RequestingPlayer.connectionToClient.connectionId;
        int assignedID = m_ConnectionIDToLocalID[connectionID];

        if (!m_ConnectionIDToPlayer.ContainsKey(connectionID))
        {
            m_ConnectionIDToPlayer.Add(connectionID, i_RequestingPlayer);
        }

        i_RequestingPlayer.RpcIDAssignmentResponse(assignedID);
    }

    public override void OnServerReady(NetworkConnection conn)
    {
        if (numPlayers == 0)
        {
            m_ConnectionIDToLocalID.Add(conn.connectionId, numPlayers + 1);
        }

        base.OnServerReady(conn);
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        m_ConnectionIDToLocalID.Add(conn.connectionId, numPlayers + 1);
        base.OnServerConnect(conn);
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
    }
}
