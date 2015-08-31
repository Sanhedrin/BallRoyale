//using UnityEngine;
//using System.Collections;
//using UnityEngine.Networking;
//using System.Collections.Generic;
//using System;
//using System.Linq;
//using LgOctEngine.CoreClasses;

//public class NetworkState : LgJsonDictionary
//{
//    public Vector3 Rotation { get { return GetValue<Vector3>("Rot", Vector3.zero); } set { SetValue<Vector3>("Rot", value); } }
//    public Vector3 Position { get { return GetValue<Vector3>("Pos", Vector3.zero); } set { SetValue<Vector3>("Pos", value); } }
//    public Vector3 Scale { get { return GetValue<Vector3>("Scale", Vector3.zero); } set { SetValue<Vector3>("Scale", value); } }
//    public int StateID { get { return GetValue<int>("ID", 0); } set { SetValue<int>("ID", value); } }
//    public float UpdateTime = 0;

//    public override bool Equals(object i_Obj)
//    {
//        NetworkState otherState = i_Obj as NetworkState;

//        return Position.Equals(otherState.Position) && Rotation.Equals(otherState.Rotation);
//    }

//    public override int GetHashCode()
//    {
//        return StateID;
//    }
//}

//[AddComponentMenu("BallGame Scripts/Network/Network Player State Syncher")]
//[NetworkSettings(channel=2, sendInterval=1/ConstParams.NetTransformSyncRate)]
//public class NetworkTransformSyncer : NetworkBehaviour
//{
//    private Rigidbody m_Rigidbody;
//    private PlayerControls m_Player;
   
//    public int StateID { get; private set; }

//    private List<NetworkState> m_NetworkStates = new List<NetworkState>();
//    private Vector3 m_LastRecordedVelocity;
//    private Vector3 m_LastRecordedScaler;
//    private Vector3 m_LastRecordedAngularVelocity;
//    private float m_AverageTimePerSyncs = 0;

//    [SerializeField]
//    private float m_ExtrapolationThreshold = 0.01f;
//    [SerializeField]
//    private float m_ObjectSnapThreshold = 0.5f;
//    [SerializeField]
//    private float m_PlayerSnapThreshold = 0.5f;
    
//    [ServerCallback]
//    private void Start()
//    {
//        StartCoroutine(syncTimer());
//    }

//    [ClientCallback]
//    private void Update()
//    {
//        smoothPosition();
//    }

//    private void smoothPosition()
//    {
//        if (isLocalPlayer && m_Rigidbody && !m_Rigidbody.useGravity)
//        {
//            m_Rigidbody.useGravity = true;
//        }

//        if (!isServer)
//        {
//            //First, we'll try to interpolate between the state the server held on us on this stateID and the next
//            NetworkState sourceState = GetStateAt(StateID);
//            shiftToState(sourceState);

//            //We can't interpolate nor extrapolate yet if we haven't found 2 positions so far.
//            if (m_NetworkStates.Count >= 2)
//            {
//                if (sourceState != null)
//                {
//                    //Checking how far we are between the 2 frames based on a sync rate.
//                    float completePercent = (Time.time - (m_NetworkStates[m_NetworkStates.Count - 1].UpdateTime - m_NetworkStates[1].UpdateTime) - m_NetworkStates[0].UpdateTime) / (m_AverageTimePerSyncs);//(1 / ConstParams.NetTransformSyncRate);
                    
//                    //If we're still not done interpolating the current state.
//                    if (completePercent <= 1)
//                    {
//                        Vector3 lastPos = transform.position;
//                        Vector3 lastRot = transform.rotation.eulerAngles;
//                        Vector3 lastScale = transform.localScale;

//                        Vector3 newScale = Vector3.Lerp(m_NetworkStates[0].Scale, m_NetworkStates[1].Scale, completePercent);
//                        Vector3 newPos = Vector3.Lerp(m_NetworkStates[0].Position, m_NetworkStates[1].Position, completePercent);
//                        Quaternion newRot = Quaternion.Slerp(Quaternion.Euler(m_NetworkStates[0].Rotation), Quaternion.Euler(m_NetworkStates[1].Rotation), completePercent);

//                        //If we need to snap to the position on a bigger sway in position than allowed.
//                        if (Vector3.Distance(transform.position, newPos) > (isLocalPlayer ? m_PlayerSnapThreshold : m_ObjectSnapThreshold))
//                        {
//                            transform.position = newPos;
//                        }
//                        //Otherwise, we'll smoothly interpolate our velocity to reach the next position in time.
//                        //We only do this for the non local player objects to try and keep a smooth experience for
//                        //each player, and only slingshot if we need to snap back.
//                        else if (!isLocalPlayer)
//                        {
//                            //We have our current position and the target position for the current frame as our
//                            //waypoints, and we know that deltaTime is the time between updates, so we have the
//                            //distance and time, and only need to calculate the speed to move at between them.
//                            //S = D/T
//                            m_Rigidbody.velocity = (newPos - transform.position) / Time.deltaTime;
//                        }

//                        if (!isLocalPlayer)
//                        {
//                            transform.rotation = newRot;
//                            transform.localScale = newScale;
//                        }

//                        m_LastRecordedScaler = (transform.localScale - lastScale);
//                        m_LastRecordedVelocity = (transform.position - lastPos);
//                        m_LastRecordedAngularVelocity = (transform.rotation.eulerAngles - lastRot);
//                    }
//                    else
//                    {
//                        //If we're done interpolating to the current state, we'll try to move on to the next.
//                        if (m_NetworkStates.Count > 1)
//                        {
//                           //++StateID;
//                        }
//                        //If we still didn't get a new state to interpolate to, we'll have to extrapolate instead.
//                        else
//                        {
//                            //if (m_LastRecordedVelocity.magnitude < m_ExtrapolationThreshold)
//                            //{
//                            //    m_Rigidbody.isKinematic = true;
//                            //    m_Rigidbody.velocity = Vector3.zero;
//                            //    m_Rigidbody.Sleep();
//                            //    m_Rigidbody.isKinematic = false;
//                            //}
//                            //else
//                            //{
//                            //    //TODO: Implement extrapolation.
//                            //    //m_Rigidbody.AddForce(m_LastRecordedVelocity.normalized * m_Player.MoveSpeed);
//                            //    //m_Rigidbody.velocity = m_LastRecordedVelocity * Time.deltaTime;
//                            //    //transform.Rotate(m_LastRecordedAngularVelocity * Time.deltaTime);
//                            //}
//                        }
//                    }
//                }
//            }
//        }
//    }

//    private IEnumerator syncTimer()
//    {
//        for (; ; )
//        {
//            yield return new WaitForSeconds(1 / ConstParams.NetTransformSyncRate);
//            SetDirtyBit(1);
//        }
//    }

//    public override bool OnSerialize(NetworkWriter writer, bool initialState)
//    {
//        bool shouldSync = false;

//        NetworkState state = LgJsonNode.Create<NetworkState>();
//        state.Position = transform.position;
//        state.Rotation = transform.rotation.eulerAngles;
//        state.Scale = transform.localScale;

//        if (initialState || !state.Equals(m_NetworkStates[m_NetworkStates.Count - 1]))
//        {
//            shouldSync = true;

//            state.StateID = StateID++;

//            string serializedNetState = state.Serialize();
//            writer.Write(serializedNetState);

//            addNewState(state);
//        }

//        return shouldSync;
//    }

//    public override void OnDeserialize(NetworkReader reader, bool initialState)
//    {
//        try
//        {
//            string serializedNetState = reader.ReadString();

//            NetworkState state = LgJsonNode.CreateFromJsonString<NetworkState>(serializedNetState);

//            //No need to apply a new state if nothing changed.
//            if (initialState || m_NetworkStates[m_NetworkStates.Count - 1].UpdateTime != Time.time)
//            {
//                state.UpdateTime = Time.time;
//                addNewState(state);

//                if (initialState)
//                {
//                    m_Player = GetComponent<PlayerControls>();
//                    m_Rigidbody = GetComponent<Rigidbody>();

//                    if (!isLocalPlayer)
//                    {
//                        m_Rigidbody.useGravity = false;
//                    }

//                    StateID = m_NetworkStates[0].StateID;
//                }
//                else
//                {
//                    ++StateID;
//                }

//                //In addition to adding the new state, we'll calculate the average time between syncs over the past buffered states
//                //to help make interpolation more consistent
//                m_AverageTimePerSyncs = m_NetworkStates.AverageUpdateTime();
//            }
//        }
//        catch (Exception e)
//        {
//            Debug.LogError("Error with network Deserialize: " + e.Message);
//        }
//    }

//    /// <summary>
//    /// Goes through our state list and finds out which state to use to accomodate the user's time.
//    /// </summary>
//    public NetworkState GetStateAt(int i_StateID)
//    {
//        NetworkState netState = null;

//        for (int i = 0; i < m_NetworkStates.Count; ++i)
//        {
//            if (m_NetworkStates[i].StateID == i_StateID)
//            {
//                netState = m_NetworkStates[i];
//                break;
//            }
//        }

//        return netState;
//    }

//    private void shiftToState(NetworkState i_StateToShiftTo)
//    {
//        while (i_StateToShiftTo != null && m_NetworkStates[0].StateID < i_StateToShiftTo.StateID)
//        {
//            m_NetworkStates.ShiftLeft();
//        }
//    }

//    private void addNewState(NetworkState i_State)
//    {
//        if (m_NetworkStates.Count > 0)
//        {
//            Debug.Log(m_NetworkStates.Count + " " + m_NetworkStates[0].StateID + " " + m_NetworkStates[m_NetworkStates.Count - 1].StateID + " " + StateID);
//        }
//        while (m_NetworkStates.Count > 0 && m_NetworkStates[0].StateID < StateID)
//        {
//            Debug.Log("Shift");
//            m_NetworkStates.ShiftLeft();
//        }

//        m_NetworkStates.Add(i_State);
//    }
//}
