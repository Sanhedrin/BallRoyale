using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using System;

[Serializable]
public struct NetworkState
{
    public Vector3 Position { get; set; }
    public Quaternion Rotation { get; set; }
    public int StateTime { get; set; }
}

[AddComponentMenu("BallGame Scripts/Network/Network Player State Syncher")]
[NetworkSettings(channel=2, sendInterval=1/ConstParams.NetTransformSyncRate)]
public class NetworkTransformSyncher : NetworkBehaviour
{
    private static int m_SynchID = 0;
    private static int SynchID
    {
        get
        {
            return m_SynchID++;
        }
    }

    private NetworkState[] m_NetworkStates = new NetworkState[20];
    private int m_LatestStateIndex = 0;

    [ServerCallback]
    private void Start()
    {
        StartCoroutine(SyncTimer());
    }

    private IEnumerator SyncTimer()
    {
        for (; ; )
        {
            yield return new WaitForSeconds(1 / ConstParams.NetTransformSyncRate);
            SetDirtyBit(1);
        }
    }

    public override bool OnSerialize(NetworkWriter writer, bool initialState)
    {
        NetworkState state = new NetworkState() { Position = transform.position, Rotation = transform.rotation, StateTime = SynchID };

        writer.Write(state.Position);
        writer.Write(state.Rotation);
        writer.Write(state.StateTime);

        bool serialized = base.OnSerialize(writer, initialState);
        return true;
    }

    public override void OnDeserialize(NetworkReader reader, bool initialState)
    {
        NetworkState state = new NetworkState()
        {
            Position = reader.ReadVector3(),
            Rotation = reader.ReadQuaternion(),
            StateTime = reader.ReadInt32()
        };

        transform.position = state.Position;
        transform.rotation = state.Rotation;
    }
}
