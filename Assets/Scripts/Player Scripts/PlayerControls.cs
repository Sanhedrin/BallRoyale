using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using System;

/// <summary>
/// Manages player input and their effect.
/// </summary>
[RequireComponent(typeof(PlayerScript))]
[AddComponentMenu("BallGame Scripts/Player/Player Script")]
[NetworkSettings(channel=1, sendInterval=0.2f)]
public class PlayerControls : NetworkBehaviour
{
    [SyncVar]
    private bool m_Grounded = false;
    public bool Grounded
    {
        get { return m_Grounded; }
    }

    //Exposing this member will help us save performance by removing GetComponent() calls which are very expensive.
    [HideInInspector]
    public Rigidbody m_Rigidbody;
    //private NetworkTransformSyncer m_NetTrans;

    [SerializeField]
    private float m_MoveSpeed = 20;
    public float MoveSpeed { get { return m_MoveSpeed; } set { m_MoveSpeed = value; } }

    [SerializeField]
    private float m_JumpSpeed = 350;

    public List<StatusEffect> ActiveEffects { get; private set; }

    public bool IsStunned
    {
        get
        {
            bool stunActive = false;

            foreach (StatusEffect effect in ActiveEffects)
            {
                if (effect is StunEffect)
                {
                    stunActive = true;
                    break;
                }
            }

            return stunActive;
        }
    }

    // Use this for initialization
    void Start()
    {
        ActiveEffects = new List<StatusEffect>();
        m_Rigidbody = GetComponent<Rigidbody>();
        //m_NetTrans = GetComponent<NetworkTransformSyncer>();
    }

    void FixedUpdate()
    {
        if (isLocalPlayer && !IsStunned)
        {
            //Query for the current state of relevent movement axes: (returns values from -1 to +1)
            ControlCommandsCollection controlInput = new ControlCommandsCollection()
            {
                HorizontalMovement = Input.GetAxis(ConstParams.HorizontalAxis),
                VerticalMovement = Input.GetAxis(ConstParams.VerticalAxis),
                Jump = Input.GetButtonDown(ConstParams.JumpButton) && m_Grounded,
                Break = Input.GetButton(ConstParams.BreakButton) && m_Grounded
            };

            //m_NetTrans.SendNetCommand(controlInput);
            CmdMovePlayer(controlInput.HorizontalMovement, controlInput.VerticalMovement, controlInput.Jump, controlInput.Break);
        }
    }

    [Command]
    public void CmdMovePlayer(float i_Horizontal, float i_Vertical, bool i_Jump, bool i_BreakButton)
    {
        if (!IsStunned)
        {
            if (i_BreakButton)
            {
                m_Rigidbody.drag = ConstParams.BreakDrag;
            }
            else
            {
                m_Rigidbody.drag = ConstParams.BaseDrag;
            }

            Vector3 movement = new Vector3(i_Horizontal, 0.0f, i_Vertical);
            m_Rigidbody.AddForce(movement * m_MoveSpeed * m_Rigidbody.mass);

            if (i_Jump && m_Grounded)
            {
                m_Rigidbody.AddForce(Vector3.up * m_JumpSpeed * m_Rigidbody.mass);
            }
        }
    }

        
    // Update is called once per frame
    void Update()
    {
        if (isServer)
        {
            effectTerminationCheckUp();
        }
    }

    [Server]
    private void effectTerminationCheckUp()
    {
        foreach (StatusEffect effect in ActiveEffects)
        {
            TimeSpan timePassed = DateTime.Now - effect.LastStarted;
            if (timePassed.TotalSeconds >= (double)effect.EffectTime)
            {
                ActiveEffects.Remove(effect);
                effect.Deactivate(this);
            }
        }
    }

    //Movement is completely done on the server and then sent to the clients after calculations, so there's no sense in
    //checking for collision on the client, when the server already deals with it and nullifies client movement upon collision checks
    [ServerCallback]
    void OnCollisionEnter(Collision i_CollisionInfo)
    {
        if (i_CollisionInfo.gameObject.layer == LayerMask.NameToLayer(ConstParams.FloorLayer))
        {
            m_Grounded = true;
        }
    }

    //Movement is completely done on the server and then sent to the clients after calculations, so there's no sense in
    //checking for collision on the client, when the server already deals with it and nullifies client movement upon collision checks
    [ServerCallback]
    void OnCollisionExit(Collision i_CollisionInfo)
    {
        if (i_CollisionInfo.gameObject.layer == LayerMask.NameToLayer(ConstParams.FloorLayer))
        {
            m_Grounded = false;
        }
    }

    [Client]
    public void ActivateSkill()
    {
        CmdActivateSkill();
    }

    [Command]
    private void CmdActivateSkill()
    {
        GetComponentInChildren<Skill>().Activate();
    }

    [ClientRpc]
    public void RpcAddSlowEffect()
    {
        bool slowEffectFound = false;

        foreach (StatusEffect effect in ActiveEffects)
        {
            if (effect is SlowEffect)
            {
                slowEffectFound = true;
                effect.RefreshTimer();
                break;
            }
        }

        if (!slowEffectFound)
        {
            SlowEffect slowEffect = new SlowEffect();

            ActiveEffects.Add(slowEffect);
            slowEffect.Activate(this);
        }
    }
}
