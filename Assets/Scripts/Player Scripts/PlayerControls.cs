﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

/// <summary>
/// Manages player input and their effect.
/// </summary>
[RequireComponent(typeof(PlayerScript))]
public class PlayerControls : NetworkBehaviour 
{
    [SyncVar]
    private bool m_Grounded = false;

    //Exposing this member will help us save performance by removing GetComponent() calls which are very expensive.
    [HideInInspector]
    public Rigidbody m_Rigidbody;

    // Instantiate a prefab with an attached Missile script
    [SerializeField]
    private GameObject m_ProjectilePrefab;

    [SerializeField]
    private float m_MoveSpeed = 20;

    [SerializeField]
    private float m_JumpSpeed = 350;

    [SerializeField]
    private float m_ProjectileSpeed = 30;

    const int k_ProjectilePooledAmount = 10;
    List<GameObject> m_ProjectilePool;

	// Use this for initialization
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();

        //Creating the pool of projectiles
        m_ProjectilePool = new List<GameObject>();
        for (int i = 0; i < k_ProjectilePooledAmount; i++)
        {
            GameObject obj = Instantiate(m_ProjectilePrefab);
            obj.SetActive(false);
            m_ProjectilePool.Add(obj);
        }
	}

    void FixedUpdate()
    {
        if (isLocalPlayer)
        {
            //Query for the current state of relevent movement axes: (returns values from -1 to +1)
            float moveHorizontal = Input.GetAxis(ConstNames.HorizontalAxis);
            float moveVertical = Input.GetAxis(ConstNames.VerticalAxis);
            bool jump = Input.GetButtonDown(ConstNames.JumpButton) && m_Grounded;

            CmdMovementManagement(moveHorizontal, moveVertical, jump);
        }
    }

	// Update is called once per frame
    void Update()
    {
        if (isLocalPlayer && Input.GetButtonDown(ConstNames.FireButton))
        {
            Shoot();
        }
	}

    /// <summary>
    /// Applies the movement on the server, which will then sync through the networkID to all the clients.
    /// </summary>
    /// <param name="i_Horizontal">Amount of horizontal movement.</param>
    /// <param name="i_Vertical">Amount of vertical movement.</param>
    /// <param name="i_Jump">Indicates if the player should jump.</param>
    [Command]
    private void CmdMovementManagement(float i_Horizontal, float i_Vertical, bool i_Jump)
    {
        Vector3 movement = new Vector3(i_Horizontal, 0.0f, i_Vertical);
        m_Rigidbody.AddForce(movement * m_MoveSpeed * m_Rigidbody.mass);

        if (i_Jump)
        {
            m_Rigidbody.AddForce(Vector3.up * m_JumpSpeed * m_Rigidbody.mass);
        }
    }

    void Shoot()
    {
        Vector3 velocityDir = m_Rigidbody.velocity.normalized;

        if (Vector3.zero != velocityDir)
        {
            for (int i = 0; i < k_ProjectilePooledAmount; i++)
            {
                GameObject currObj = m_ProjectilePool[i];

                if (!currObj.activeInHierarchy)
                {
                    currObj.transform.position = transform.position + velocityDir;
                    currObj.transform.rotation = Quaternion.LookRotation(velocityDir);
                    currObj.transform.Rotate(new Vector3(0, 90, 0)); // rotate the missle by 90 degrees on the y axes
                    currObj.GetComponent<Rigidbody>().velocity = velocityDir * m_ProjectileSpeed;
                    currObj.SetActive(true);
                    break;
                }
            }
        }
    }

    //Movement is completely done on the server and then sent to the clients after calculations, so there's no sense in
    //checking for collision on the client, when the server already deals with it and nullifies client movement upon collision checks
    void OnCollisionEnter(Collision i_CollisionInfo)
    {
        if (isServer)
        {
            if (i_CollisionInfo.gameObject.layer == LayerMask.NameToLayer(ConstNames.FloorLayer))
            {
                m_Grounded = true;
            }
        }
    }

    //Movement is completely done on the server and then sent to the clients after calculations, so there's no sense in
    //checking for collision on the client, when the server already deals with it and nullifies client movement upon collision checks
    void OnCollisionExit(Collision i_CollisionInfo)
    {
        if (isServer)
        {
            if (i_CollisionInfo.gameObject.layer == LayerMask.NameToLayer(ConstNames.FloorLayer))
            {
                m_Grounded = false;
            }
        }
    }
}