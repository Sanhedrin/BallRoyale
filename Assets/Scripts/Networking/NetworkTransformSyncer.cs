using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using System;
using System.Linq;
using LgOctEngine.CoreClasses;

public struct NetworkState
{
    public Vector3 Rotation;
    public Vector3 Position;
    public Vector3 Scale;
    public int StateID;
    public float UpdateTime;

    public override bool Equals(object i_Obj)
    {
        NetworkState otherState = (NetworkState)i_Obj;

        return Position.Equals(otherState.Position) && Rotation.Equals(otherState.Rotation);
    }

    public override int GetHashCode()
    {
        return StateID;
    }
}

public class SyncListNetState : SyncListStruct<NetworkState>
{
}

[AddComponentMenu("BallGame Scripts/Network/Network Player State Syncher")]
[NetworkSettings(channel=2, sendInterval=1/ConstParams.NetTransformSyncRate)]
public class NetworkTransformSyncer : NetworkBehaviour
{
    private Rigidbody m_Rigidbody;
    private PlayerControls m_Player;
   
    public int StateID { get; private set; }

    private SyncListNetState m_NetworkStates = new SyncListNetState();
    private Vector3 m_LastRecordedVelocity;
    private Vector3 m_LastRecordedScaler;
    private Vector3 m_LastRecordedAngularVelocity;

    [SerializeField]
    private float m_ExtrapolationThreshold = 0.01f;
    [SerializeField]
    private float m_ObjectSnapThreshold = 0.5f;
    [SerializeField]
    private float m_PlayerSnapThreshold = 0.5f;
    
    [ServerCallback]
    private void Start()
    {
        m_Player = GetComponent<PlayerControls>();
        m_Rigidbody = GetComponent<Rigidbody>();

        StartCoroutine(syncTimer());
    }

    [ClientCallback]
    private void Update()
    {
        //Called on all clients.
        Debug.Log(m_NetworkStates.Count);
        smoothPosition();

        if (m_NetworkStates.Count > 0 && StateID > m_NetworkStates[0].StateID)
        {
            CmdUpdateLatestClientState(StateID);
        }
    }

    [Server]
    private IEnumerator syncTimer()
    {
        for (; ; )
        {
            yield return new WaitForSeconds(1 / ConstParams.NetTransformSyncRate);
            addNewState();
        }
    }

    private void smoothPosition()
    {
        if (isLocalPlayer && m_Rigidbody && !m_Rigidbody.useGravity)
        {
            m_Rigidbody.useGravity = true;
        }

        if (!isServer)
        {
            //First, we'll try to interpolate between the state the server held on us on this stateID and the next
            NetworkState sourceState = GetStateAt(StateID);

            //We can't interpolate nor extrapolate yet if we haven't found 2 positions so far.
            if (m_NetworkStates.Count >= 2)
            {
                if (!sourceState.Equals(default(NetworkState)))
                {
                    //Checking how far we are between the 2 frames based on a sync rate.
                    float completePercent = (Time.time - (m_NetworkStates[m_NetworkStates.Count - 1].UpdateTime - m_NetworkStates[1].UpdateTime) - m_NetworkStates[0].UpdateTime) / (1 / ConstParams.NetTransformSyncRate);
                    
                    //If we're still not done interpolating the current state.
                    if (completePercent <= 1)
                    {
                        Vector3 lastPos = transform.position;
                        Vector3 lastRot = transform.rotation.eulerAngles;
                        Vector3 lastScale = transform.localScale;

                        Vector3 newScale = Vector3.Lerp(m_NetworkStates[0].Scale, m_NetworkStates[1].Scale, completePercent);
                        Vector3 newPos = Vector3.Lerp(m_NetworkStates[0].Position, m_NetworkStates[1].Position, completePercent);
                        Quaternion newRot = Quaternion.Slerp(Quaternion.Euler(m_NetworkStates[0].Rotation), Quaternion.Euler(m_NetworkStates[1].Rotation), completePercent);

                        //If we need to snap to the position on a bigger sway in position than allowed.
                        if (Vector3.Distance(transform.position, newPos) > (isLocalPlayer ? m_PlayerSnapThreshold : m_ObjectSnapThreshold))
                        {
                            transform.position = newPos;
                        }
                        //Otherwise, we'll smoothly interpolate our velocity to reach the next position in time.
                        //We only do this for the non local player objects to try and keep a smooth experience for
                        //each player, and only slingshot if we need to snap back.
                        else if (!isLocalPlayer)
                        {
                            //We have our current position and the target position for the current frame as our
                            //waypoints, and we know that deltaTime is the time between updates, so we have the
                            //distance and time, and only need to calculate the speed to move at between them.
                            //S = D/T
                            m_Rigidbody.velocity = (newPos - transform.position) / Time.deltaTime;
                        }

                        if (!isLocalPlayer)
                        {
                            transform.rotation = newRot;
                            transform.localScale = newScale;
                        }

                        m_LastRecordedScaler = (transform.localScale - lastScale);
                        m_LastRecordedVelocity = (transform.position - lastPos);
                        m_LastRecordedAngularVelocity = (transform.rotation.eulerAngles - lastRot);
                    }
                    else
                    {
                        //If we're done interpolating to the current state, we'll try to move on to the next.
                        if (m_NetworkStates.Count > 1)
                        {
                           ++StateID;
                        }
                        //If we still didn't get a new state to interpolate to, we'll have to extrapolate instead.
                        else
                        {
                            //if (m_LastRecordedVelocity.magnitude < m_ExtrapolationThreshold)
                            //{
                            //    m_Rigidbody.isKinematic = true;
                            //    m_Rigidbody.velocity = Vector3.zero;
                            //    m_Rigidbody.Sleep();
                            //    m_Rigidbody.isKinematic = false;
                            //}
                            //else
                            //{
                            //    //TODO: Implement extrapolation.
                            //    //m_Rigidbody.AddForce(m_LastRecordedVelocity.normalized * m_Player.MoveSpeed);
                            //    //m_Rigidbody.velocity = m_LastRecordedVelocity * Time.deltaTime;
                            //    //transform.Rotate(m_LastRecordedAngularVelocity * Time.deltaTime);
                            //}
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Goes through our state list and finds out which state to use to accomodate the user's time.
    /// </summary>
    public NetworkState GetStateAt(int i_StateID)
    {
        NetworkState netState = new NetworkState();

        for (int i = 0; i < m_NetworkStates.Count; ++i)
        {
            if (m_NetworkStates[i].StateID == i_StateID)
            {
                netState = m_NetworkStates[i];
                break;
            }
        }

        return netState;
    }

    [Command]
    private void CmdUpdateLatestClientState(int i_StateID)
    {
        //Delete all the older states than the client's state that are no longer relevant.
        while (m_NetworkStates.Count > 1 && m_NetworkStates[0].StateID < i_StateID)
        {
            m_NetworkStates.RemoveAt(0);
        }
    }

    [Server]
    private void addNewState()
    {
        NetworkState state = new NetworkState();
        state.Position = transform.position;
        state.Rotation = transform.rotation.eulerAngles;
        state.Scale = transform.localScale;
        state.UpdateTime = Time.time;

        //if (m_NetworkStates.Count < 1 || !state.Equals(m_NetworkStates[m_NetworkStates.Count - 1]))
        {
            state.StateID = StateID++;
            m_NetworkStates.Add(state);
        }
    }
}
