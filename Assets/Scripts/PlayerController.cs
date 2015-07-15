using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using Assets.Scripts;

public partial class PlayerController : NetworkBehaviour
{
    [SyncVar]
    private bool m_Grounded = false;

    /// <summary>
    /// Never set to this directly, use setPlayerHealth() instead.
    /// </summary>
    [SyncVar(hook="OnHealthChanged")]
    private int m_Health = 0;
    private void OnHealthChanged(int i_NewHealth)
    {
        m_Health = i_NewHealth < k_MaxHealth ? i_NewHealth : k_MaxHealth;
    }

    private const int k_MaxHealth = 999;

    /// <summary>
    /// How the knockback should be calculated from the health. The lower this is, the higher the knockback scaling becomes.
    /// </summary>
    private const int k_HealthKnockbackStep = 750;

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

    private Text m_PlayerHealthText;

    [SyncVar]
    private int m_ConnectionID;

    private delegate void IDAssignedEventHandler(object sender, IDEventArgs e);
    private event IDAssignedEventHandler IDAssigned;

    const int k_ProjectilePooledAmount = 10;
    List<GameObject> m_ProjectilePool;

	// Use this for initialization
	void Start ()
    {
        m_Rigidbody = GetComponent<Rigidbody>();

        IDAssigned += new IDAssignedEventHandler(PlayerController_IDAssigned);

        if (isLocalPlayer)
        {
            CmdRequestID();
        }
        else
        {
            AssignTextForPlayer(m_ConnectionID);
        }

        Random.seed = System.DateTime.Now.Millisecond;
 
        //Creating the pool of projectiles
        m_ProjectilePool = new List<GameObject>();
        for (int i = 0; i < k_ProjectilePooledAmount; i++)
        {
            GameObject obj = Instantiate(m_ProjectilePrefab);
            obj.SetActive(false);
            m_ProjectilePool.Add(obj);
        }
	}
	
	// Update is called once per frame, non-physics updates should be writen here.
    void Update()
    {
        if (isLocalPlayer && Input.GetButtonDown(ConstNames.FireButton))
        {
            Shoot();
        }

        if (m_PlayerHealthText)
        {
            m_PlayerHealthText.text = m_Health.ToString();
        }
    }

    // FixedUpdate is called before any physics calculation, this is where the physics code should go(as seen on the Unity tutorial)
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

    /// <summary>
    /// Requests the server to assign client an ID
    /// </summary>
    [Command]
    private void CmdRequestID()
    {
        m_ConnectionID = NetworkManager.singleton.numPlayers;
        RpcIDAssignResponse(m_ConnectionID);
    }

    /// <summary>
    /// A response to the ID assignment request.
    /// </summary>
    /// <param name="i_IDAssignment">The ID this player was assigned</param>
    [ClientRpc]
    private void RpcIDAssignResponse(int i_IDAssignment)
    {
        OnIDAssigned(new IDEventArgs(i_IDAssignment));
    }

    //
    private void OnIDAssigned(IDEventArgs e)
    {
        if (IDAssigned != null)
        {
            IDAssigned.Invoke(this, e);
        } 
    }

    private void PlayerController_IDAssigned(object sender, IDEventArgs e)
    {
        AssignTextForPlayer(e.ID);
    }

    /// <summary>
    /// Assigns the text object holding the health of a player to the corresponding player ID.
    /// </summary>
    private void AssignTextForPlayer(int i_PlayerID)
    {
        GameObject text = GameObject.Find(string.Format("Player{0}HP", i_PlayerID));
        text.SetActive(true);
        m_PlayerHealthText = text.GetComponent<Text>();
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

        if (m_PlayerHealthText)
        {
            m_PlayerHealthText.text = m_Health.ToString();
        }
    }
    
    /// <summary>
    /// Increases the velocity of the pushed player by the amount of damage they've taken.
    /// This is a coroutine and is only called on the server.
    /// </summary>
    /// <param name="i_PushedPlayer">Player to push</param>
    private IEnumerator serverPushPlayer(GameObject i_PushedPlayer)
    {
        if (isServer)
        {
            yield return null;
            yield return null;
            PlayerController pushedPlayer = i_PushedPlayer.GetComponent<PlayerController>();
            pushedPlayer.m_Rigidbody.velocity *= (1 + (float)pushedPlayer.m_Health / (float)PlayerController.k_HealthKnockbackStep);
        }
    }

    private void handleDeath()
    {
        Transform spawnPoint = NetworkManager.singleton.startPositions[Random.Range(0, NetworkManager.singleton.startPositions.Count)];
        transform.position = spawnPoint.position;

        m_Health = 0;
        initializePhysicsInfo();
    }

    private void initializePhysicsInfo()
    {
        m_Rigidbody.velocity = Vector3.zero;
    }

    void OnCollisionEnter(Collision i_CollisionInfo)
    {
        if (isServer)
        {
            m_Health++;

            if (i_CollisionInfo.gameObject.layer == LayerMask.NameToLayer(ConstNames.KillBoxLayer))
            {
                handleDeath();
            }

            if (i_CollisionInfo.gameObject.layer == LayerMask.NameToLayer(ConstNames.FloorLayer))
            {
                m_Grounded = true;
            }

            if (i_CollisionInfo.gameObject.CompareTag(ConstNames.PlayerTag))
            {
                StartCoroutine(serverPushPlayer(i_CollisionInfo.collider.gameObject));
            }
        }
    }

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
