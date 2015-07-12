﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public partial class PlayerController : NetworkBehaviour
{
    private bool m_Grounded = true;
    private Rigidbody m_RigidBody;

    [SerializeField]
    private float m_MoveSpeed = 20;
    [SerializeField]
    private float m_JumpSpeed = 350;

	// Use this for initialization
	void Start ()
    {
        m_RigidBody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame, non-physics updates should be writen here.
    void Update()
    {

    }

    // FixedUpdate is called before any physics calculation, this is where the physics code should go(as seen on the Unity tutorial)
    void FixedUpdate()
    {
        if (!isLocalPlayer)
            return;

        //Query for the current state of relevent movement axes: (returns values from -1 to +1)
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        CmdMovementManagmnt(moveHorizontal, moveVertical);
    }

    [Command]
    private void CmdMovementManagmnt(float i_Horizontal, float i_Vertical)
    {
        Vector3 movement = new Vector3(i_Horizontal, 0.0f, i_Vertical);
        m_RigidBody.AddForce(movement * m_MoveSpeed * m_RigidBody.mass);

        //if (Input.GetKey(KeyCode.LeftArrow))
        //{
        //    m_RigidBody.AddForce(Vector3.left * m_MoveSpeed);
        //}
        //if (Input.GetKey(KeyCode.RightArrow))
        //{
        //    m_RigidBody.AddForce(Vector3.right * m_MoveSpeed);
        //}
        //if (Input.GetKey(KeyCode.UpArrow))
        //{
        //    m_RigidBody.AddForce(Vector3.forward * m_MoveSpeed);
        //}
        //if (Input.GetKey(KeyCode.DownArrow))
        //{
        //    m_RigidBody.AddForce(Vector3.back * m_MoveSpeed);
        //}
        if (Input.GetKeyDown(KeyCode.Space) && m_Grounded)
        {
            m_RigidBody.AddForce(Vector3.up * m_JumpSpeed);
        }
    }

    public void OnCollisionEnter(Collision i_TheCollision)
    {
        if (i_TheCollision.gameObject.CompareTag("Plain"))
        {
            m_Grounded = true;
        }

        // IN TESTING!
        if (i_TheCollision.gameObject.CompareTag("Player"))
        {
            var otherPlayer = i_TheCollision.collider.gameObject;
            var pushVector = i_TheCollision.gameObject.transform.position - m_RigidBody.transform.position * m_RigidBody.velocity.sqrMagnitude * 100;

            CmdPushPlayer(otherPlayer, pushVector);
        }
    }

    [Command]
    public void CmdPushPlayer(GameObject player, Vector3 force)
    {
        player.GetComponent<Rigidbody>().AddForce(force);
    }

    public void OnCollisionExit(Collision i_TheCollision)
    {
        if (i_TheCollision.gameObject.CompareTag("Plain"))
        {
            m_Grounded = false;
        }
    }
}
