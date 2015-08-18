using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using System;
using LgOctEngine.CoreClasses;

public class NetworkState : LgJsonDictionary
{
    public Vector3 Rotation { get { return GetValue<Vector3>("Rot", Vector3.zero); } set { SetValue<Vector3>("Rot", value); } }
    public Vector3 Position { get { return GetValue<Vector3>("Pos", Vector3.zero); } set { SetValue<Vector3>("Pos", value); } }
    public Vector3 Scale { get { return GetValue<Vector3>("Scale", Vector3.zero); } set { SetValue<Vector3>("Scale", value); } }
    public int StateID { get { return GetValue<int>("ID", 0); } set { SetValue<int>("ID", value); } }
    public float UpdateTime = 0;

    public override bool Equals(object i_Obj)
    {
        NetworkState otherState = i_Obj as NetworkState;

        return Position.Equals(otherState.Position) && Rotation.Equals(otherState.Rotation);
    }

    public override int GetHashCode()
    {
        return StateID;
    }
}

[AddComponentMenu("BallGame Scripts/Network/Network Player State Syncher")]
[NetworkSettings(channel=2, sendInterval=1/ConstParams.NetTransformSyncRate)]
public class NetworkTransformSyncer : NetworkBehaviour
{
    private Rigidbody m_Rigidbody;
    private PlayerControls m_Player;
    
    public static int StateID { get; private set; }

    private NetworkState[] m_NetworkStates = new NetworkState[20];
    private int m_LatestStateArrayIndex = 0;
    private Vector3 m_LastRecordedVelocity;
    private Vector3 m_LastRecordedScaler;
    private Vector3 m_LastRecordedAngularVelocity;
    private float m_AverageTimePerSyncs = 0;

    [SerializeField]
    private float m_ExtrapolationThreshold = 0.01f;
    [SerializeField]
    private float m_ObjectSnapThreshold = 0.5f;
    [SerializeField]
    private float m_PlayerSnapThreshold = 0.5f;
    
    [ServerCallback]
    private void Start()
    {
        StartCoroutine(syncTimer());
    }

    [ClientCallback]
    private void Update()
    {
        if (isLocalPlayer && m_Rigidbody && !m_Rigidbody.useGravity)
        {
            m_Rigidbody.useGravity = true;
        }

        if (!isServer)
        {
            //We can't interpolate nor extrapolate yet if we haven't found 2 positions so far.
            if (m_LatestStateArrayIndex >= 2)
            {
                //First, we'll try to interpolate between the 2 newest frames

                //Checking how far we are between the 2 frames based on a sync rate.
                float completePercent = (Time.time - m_NetworkStates[m_LatestStateArrayIndex - 1].UpdateTime) / (m_AverageTimePerSyncs + 2 * Time.deltaTime);//(1 / ConstParams.NetTransformSyncRate);

                //If we're still not done interpolating the current state.
                if (completePercent <= 1)
                {
                    Vector3 lastPos = transform.position;
                    Vector3 lastRot = transform.rotation.eulerAngles;
                    Vector3 lastScale = transform.localScale;

                    Vector3 newScale = Vector3.Lerp(m_NetworkStates[m_LatestStateArrayIndex - 2].Scale, m_NetworkStates[m_LatestStateArrayIndex - 1].Scale, completePercent);
                    Vector3 newPos = Vector3.Lerp(m_NetworkStates[m_LatestStateArrayIndex - 2].Position, m_NetworkStates[m_LatestStateArrayIndex - 1].Position, completePercent);
                    Quaternion newRot = Quaternion.Lerp(Quaternion.Euler(m_NetworkStates[m_LatestStateArrayIndex - 2].Rotation), Quaternion.Euler(m_NetworkStates[m_LatestStateArrayIndex - 1].Rotation), completePercent);

                    if (Vector3.Distance(transform.position, newPos) > (isLocalPlayer ? m_PlayerSnapThreshold : m_ObjectSnapThreshold))
                    {
                        transform.position = newPos;
                        transform.rotation = newRot;
                        transform.localScale = newScale;
                    }

                    m_LastRecordedScaler = (transform.localScale - lastScale);
                    m_LastRecordedVelocity = (transform.position - lastPos);
                    m_LastRecordedAngularVelocity = (transform.rotation.eulerAngles - lastRot);
                }
                //If we still didn't get a new state to interpolate to, we'll have to extrapolate instead.
                else
                {
                    if (m_LastRecordedVelocity.magnitude < m_ExtrapolationThreshold)
                    {
                        m_Rigidbody.isKinematic = true;
                        m_Rigidbody.velocity = Vector3.zero;
                        m_Rigidbody.Sleep();
                        m_Rigidbody.isKinematic = false;
                    }
                    else
                    {
                        //TODO: Implement extrapolation.
                        //m_Rigidbody.AddForce(m_LastRecordedVelocity.normalized * m_Player.MoveSpeed);
                        //m_Rigidbody.velocity = m_LastRecordedVelocity * Time.deltaTime;
                        //transform.Rotate(m_LastRecordedAngularVelocity * Time.deltaTime);
                    }
                }
            }
        }
    }

    private IEnumerator syncTimer()
    {
        for (; ; )
        {
            yield return new WaitForSeconds(1 / ConstParams.NetTransformSyncRate);
            SetDirtyBit(1);
        }
    }

    public override bool OnSerialize(NetworkWriter writer, bool initialState)
    {
        bool shouldSync = false;

        NetworkState state = LgJsonNode.Create<NetworkState>();
        state.Position = transform.position;
        state.Rotation = transform.rotation.eulerAngles;
        state.Scale = transform.localScale;

        if (initialState || !state.Equals(m_NetworkStates[m_LatestStateArrayIndex - 1]))
        {
            shouldSync = true;
            state.StateID = StateID++;

            string serializedNetState = state.Serialize();
            writer.Write(serializedNetState);

            addNewState(state);
        }

        return shouldSync;
    }

    public override void OnDeserialize(NetworkReader reader, bool initialState)
    {
        try
        {
            string serializedNetState = reader.ReadString();

            NetworkState state = LgJsonNode.CreateFromJsonString<NetworkState>(serializedNetState);

            //No need to apply a new state if nothing changed.
            if (initialState || m_NetworkStates[m_LatestStateArrayIndex - 1].UpdateTime != Time.time)
            {
                addNewState(state);
                StateID = m_NetworkStates[0].StateID;
                state.UpdateTime = Time.time;

                if (initialState)
                {
                    m_Player = GetComponent<PlayerControls>();
                    m_Rigidbody = GetComponent<Rigidbody>();

                    if (!isLocalPlayer)
                    {
                        m_Rigidbody.useGravity = false;
                    }
                }

                //In addition to adding the new state, we'll calculate the average time between syncs over the past buffered states
                //to help make interpolation more consistent
                m_AverageTimePerSyncs = m_NetworkStates.AverageUpdateTime(m_LatestStateArrayIndex);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error with network Deserialize: " + e.Message);
        }
    }

    /// <summary>
    /// Goes through our state list and finds out which state to use to accomodate the user's time.
    /// </summary>
    public NetworkState GetStateAt(int i_StateID)
    {
        NetworkState netState = null;

        for (int i = 0; i < m_LatestStateArrayIndex; ++i)
        {
            if (m_NetworkStates[i].StateID == i_StateID)
            {
                netState = m_NetworkStates[i];
                break;
            }
        }

        return netState;
    }

    private void addNewState(NetworkState i_State)
    {
        if (m_LatestStateArrayIndex >= m_NetworkStates.Length)
        {
            m_NetworkStates.ShiftLeft();
            --m_LatestStateArrayIndex;
        }

        m_NetworkStates[m_LatestStateArrayIndex++] = i_State;
    }
}
