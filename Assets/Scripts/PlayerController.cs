using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public partial class PlayerController : NetworkBehaviour
{
    [SyncVar]
    private bool m_Grounded = false;

    [SyncVar]
    public int m_Health = 0;
    private const int k_MaxHealth = 999;

    /// <summary>
    /// How the knockback should be calculated from the health. The lower this is, the higher the knockback scaling becomes.
    /// </summary>
    private const int k_HealthKnockbackStep = 100;

    //Exposing this member will help us save performance by removing GetComponent() calls which are very expensive.
    [HideInInspector]
    public Rigidbody m_Rigidbody;

    // Instantiate a prefab with an attached Missile script
    public GameObject m_ProjectilePrefab;

    [SerializeField]
    private float m_MoveSpeed = 20;
    
    [SerializeField]
    private float m_JumpSpeed = 350;
    
    [SerializeField]
    private float m_ProjectileSpeed = 30;

	// Use this for initialization
	void Start ()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame, non-physics updates should be writen here.
    void Update()
    {
        Vector3 velocityDir = m_Rigidbody.velocity;
        
        if (Input.GetButtonDown("Fire1") && Vector3.zero != velocityDir)
        {
            GameObject clone = Instantiate(m_ProjectilePrefab, transform.position + velocityDir.normalized, transform.rotation) as GameObject;
            clone.GetComponent<Rigidbody>().velocity = velocityDir.normalized * m_ProjectileSpeed;
        }
    }

    // FixedUpdate is called before any physics calculation, this is where the physics code should go(as seen on the Unity tutorial)
    void FixedUpdate()
    {
        if (isLocalPlayer)
        {
            //Query for the current state of relevent movement axes: (returns values from -1 to +1)
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");
            bool jump = Input.GetButtonDown("Jump") && m_Grounded;

            CmdMovementManagement(moveHorizontal, moveVertical, jump);
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

    void OnCollisionEnter(Collision i_CollisionInfo)
    {
        if (i_CollisionInfo.gameObject.layer == LayerMask.NameToLayer("Floor"))
        {
            m_Grounded = true;
        }

        if (i_CollisionInfo.gameObject.CompareTag("Player"))
        {
            if (isServer)
            {
                StartCoroutine(serverPushPlayer(i_CollisionInfo.collider.gameObject));
            }
        }
    }

    void OnCollisionExit(Collision i_CollisionInfo)
    {
        if (i_CollisionInfo.gameObject.layer == LayerMask.NameToLayer("Floor"))
        {
            m_Grounded = false;
        }
    }
}
