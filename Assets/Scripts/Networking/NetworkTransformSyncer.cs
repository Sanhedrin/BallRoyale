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
//    public int StateID { get { return GetValue<int>("StateID", 0); } set { SetValue<int>("StateID", value); } }
//    public int LatestInputID { get { return GetValue<int>("InputID", 0); } set { SetValue<int>("InputID", value); } }
//    public float UpdateTime { get { return GetValue<float>("UpdateTime", 0); } set { SetValue<float>("UpdateTime", value); } }

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
//[NetworkSettings(channel = 2, sendInterval = 1 / ConstParams.NetTransformSyncRate)]
//public class NetworkTransformSyncer : NetworkBehaviour
//{
//    private Rigidbody m_Rigidbody;
//    private PlayerControls m_Player;

//    public int StateID { get; private set; }
//    public int InputStateID { get; private set; }

//    private Vector3 m_LastRecordedVelocity;
//    private Vector3 m_LastRecordedScaler;
//    private Vector3 m_LastRecordedAngularVelocity;

//    private float m_AverageTimePerSyncs = 0;
//    private float m_LastUpdateTimeReceieved;

//    private List<NetworkState> m_NetworkStates = new List<NetworkState>();
//    private List<ControlCommandsCollection> m_SentCommands = new List<ControlCommandsCollection>();
    
//    private void Awake()
//    {
//        m_Player = GetComponent<PlayerControls>();
//        m_Rigidbody = GetComponent<Rigidbody>();

//        InputStateID = 0;

//        if (!isLocalPlayer && !isServer)
//        {
//            m_Rigidbody.useGravity = false;
//        }
//        else
//        {
//            m_Rigidbody.useGravity = true;
//        }
//    }

//    [ServerCallback]
//    private void Start()
//    {
//        StartCoroutine(syncTimer());
//    }

//    [ClientCallback]
//    private void Update()
//    {
//        if ((isLocalPlayer || isServer) && m_Rigidbody && !m_Rigidbody.useGravity)
//        {
//            m_Rigidbody.useGravity = true;
//        }

//        smoothNonLocalPlayerPosition();
//    }

//    private void smoothNonLocalPlayerPosition()
//    {
//        if (!isLocalPlayer && !isServer)
//        {
//            //Checking how far we are between the 2 frames based on a sync rate.
//            if (m_NetworkStates.Count >= 3)
//            {
//                int lerpingIndex = 0;
//                float completePercent = 5;

//                for (int i = 0; i < m_NetworkStates.Count - 1 && completePercent > 1; ++i)
//                {
//                    completePercent = ((Time.time - (m_NetworkStates[m_NetworkStates.Count - 1].UpdateTime - m_NetworkStates[0 + lerpingIndex].UpdateTime)) - m_NetworkStates[0 + lerpingIndex].UpdateTime) / m_AverageTimePerSyncs;
//                }

//                //If we're still not done interpolating the current state.
//                if (completePercent <= 1)
//                {
//                    Vector3 lastPos = transform.position;
//                    Vector3 lastRot = transform.rotation.eulerAngles;
//                    Vector3 lastScale = transform.localScale;

//                    Vector3 newScale = Vector3.Lerp(m_NetworkStates[0 + lerpingIndex].Scale, m_NetworkStates[1 + lerpingIndex].Scale, completePercent);
//                    Vector3 newPos = Vector3.Lerp(m_NetworkStates[0 + lerpingIndex].Position, m_NetworkStates[1 + lerpingIndex].Position, completePercent);
//                    Quaternion newRot = Quaternion.Slerp(Quaternion.Euler(m_NetworkStates[0 + lerpingIndex].Rotation), Quaternion.Euler(m_NetworkStates[1 + lerpingIndex].Rotation), completePercent);

//                    transform.position = newPos;
//                    m_Rigidbody.velocity = (newPos - transform.position) / Time.deltaTime;
//                    transform.rotation = newRot;
//                    transform.localScale = newScale;

//                    m_LastRecordedScaler = (transform.localScale - lastScale);
//                    m_LastRecordedVelocity = (transform.position - lastPos);
//                    m_LastRecordedAngularVelocity = (transform.rotation.eulerAngles - lastRot);
//                }
//                //Extrpolation
//                else
//                {
//                    StateID = m_NetworkStates[1].StateID;

//                    m_Rigidbody.velocity = m_LastRecordedVelocity;
//                }
//            }
//        }
//    }

//    [Client]
//    private void updateLocalPlayerPosition(NetworkState i_State)
//    {
//        //We should remove all past input states that have already been processed
//        while (m_SentCommands.Count > 0 && m_SentCommands[0].StateSentAt < i_State.LatestInputID)
//        {
//            m_SentCommands.RemoveAt(0);
//        }

//        //Cancel out past physics effects.
//        m_Rigidbody.isKinematic = true;
//        m_Rigidbody.velocity = Vector3.zero;
//        m_Rigidbody.Sleep();
//        m_Rigidbody.isKinematic = false;

//        //And for the input requests we haven't processed yet, we'll apply them again to the newly received position.
//        transform.position = i_State.Position;
//        transform.rotation = Quaternion.Euler(i_State.Rotation);
//        transform.localScale = i_State.Scale;
//        StateID = i_State.StateID;

//        foreach (ControlCommandsCollection command in m_SentCommands)
//        {
//            if (command.StateSentAt <= i_State.LatestInputID)
//            {
//                m_Player.MovePlayer(command.HorizontalMovement, command.VerticalMovement, command.Jump, command.Break);
//            }
//            else
//            {
//                break;
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
//        state.UpdateTime = Time.time;

//        if (initialState || m_NetworkStates.Count < 1 || state.UpdateTime > m_NetworkStates[m_NetworkStates.Count - 1].UpdateTime)
//        {
//            shouldSync = true;

//            state.StateID = StateID++;
//            state.LatestInputID = InputStateID;

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

//            if (m_NetworkStates.Count < 1 || (state.StateID > m_NetworkStates[m_NetworkStates.Count - 1].StateID && state.UpdateTime > m_LastUpdateTimeReceieved))
//            {
//                m_LastUpdateTimeReceieved = state.UpdateTime;
//                state.UpdateTime = Time.time;

//                if (!isLocalPlayer)
//                {
//                    addNewState(state);
//                }
//                else
//                {
//                    updateLocalPlayerPosition(state);
//                }
//            }

//            //No need to apply a new state if nothing changed.
//            if (initialState)
//            {
//                if (m_NetworkStates.Count > 0)
//                {
//                    StateID = m_NetworkStates[0].StateID;
//                }
//            }

//            //In addition to adding the new state, we'll calculate the average time between syncs over the past buffered states
//            //to help make interpolation more consistent
//            m_AverageTimePerSyncs = m_NetworkStates.AverageUpdateTime();
//        }
//        catch (Exception e)
//        {
//            Debug.LogError("Error with network Deserialize: " + e.Message);
//        }
//    }

//    /// <summary>
//    /// Goes through our state list and finds out which state to use to accomodate the user's state.
//    /// </summary>
//    public NetworkState GetStateAtID(int i_StateID)
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
//            m_NetworkStates.RemoveAt(0);
//        }
//    }

//    private void addNewState(NetworkState i_State)
//    {

//        while (m_NetworkStates.Count > 3)
//        {
//            m_NetworkStates.ShiftLeft();
//        }

//        m_NetworkStates.Add(i_State);
//    }

//    [Client]
//    public void SendNetCommand(ControlCommandsCollection i_ControlInput)
//    {
//        if (isLocalPlayer)
//        {
//            if (i_ControlInput.IsNewInput)
//            {
//                i_ControlInput.StateSentAt = ++InputStateID;

//                if (!isServer)
//                {
//                    m_SentCommands.Add(i_ControlInput);
//                    CmdMovePlayer(i_ControlInput.HorizontalMovement, i_ControlInput.VerticalMovement, i_ControlInput.Jump, i_ControlInput.Break);
//                }
             
//                m_Player.MovePlayer(i_ControlInput.HorizontalMovement, i_ControlInput.VerticalMovement, i_ControlInput.Jump, i_ControlInput.Break);
//            }
//        }
//    }

//    [Command]
//    private void CmdMovePlayer(float i_Horizontal, float i_Vertical, bool i_Jump, bool i_BreakButton)
//    {
//        InputStateID++;
//        m_Player.MovePlayer(i_Horizontal, i_Vertical, i_Jump, i_BreakButton);
//    }
//}