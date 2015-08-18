using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using Assets.Scripts;
using Assets.Scripts.Player_Scripts;

/// <summary>
/// The base script for the player will manage the display and stats of the player, as well as network synchronization.
/// </summary>
[AddComponentMenu("BallGame Scripts/Player/Player Controls")]
public class PlayerScript : NetworkBehaviour
{
    /// <summary>
    /// The health of the player, make sure to only assign from server side.
    /// </summary>
    [SyncVar(hook="OnHealthChanged")]
    private int m_Health = 0;
    private void OnHealthChanged(int i_NewHealth)
    {
        m_Health = i_NewHealth < k_MaxHealth ? i_NewHealth : k_MaxHealth;
        
        if (!m_PlayerHealthText)
        {
            Debug.LogError("Health not connected");
        }
    }

    private const int k_MaxHealth = 999;
    private const int k_MaxDeaths = 3;
    private const int k_DamageReduction = 10;
    private const float k_KillCreditTimeLimit = 1f;

    [SerializeField]
    private Texture[] m_PlayerTextures;

    /// <summary>
    /// How the knockback should be calculated from the health. The lower this is, the higher the knockback scaling becomes.
    /// </summary>
    private const int k_HealthKnockbackStep = 150;

    //Exposing this member will help us save performance by removing GetComponent() calls which are very expensive.
    [HideInInspector]
    public Rigidbody m_Rigidbody;

    private Text m_PlayerHealthText;

    [SyncVar]
    private int m_ConnectionID = -1;

    [SyncVar]
    private int m_DeathCount = 0;

    [SyncVar]
    private int m_KillCount = 0;

    private GameObject m_LastColidingPlayer;
    private DateTime m_LastPlayerCollision;

    private delegate void IDAssignedEventHandler(object sender, IDEventArgs e);
    private event IDAssignedEventHandler IDAssigned;

	// Use this for initialization
	void Start ()
    {
        m_Rigidbody = GetComponent<Rigidbody>();

        IDAssigned += new IDAssignedEventHandler(PlayerController_IDAssigned);

        if (isLocalPlayer)
        {
            CmdRequestID();
        }
        else if(m_ConnectionID > 0)
        {
            OnIDAssigned(new IDEventArgs(m_ConnectionID));
        }

        Random.seed = System.DateTime.Now.Millisecond;
	}
	
	// Update is called once per frame, non-physics updates should be writen here.
    void Update()
    {
        if (m_PlayerHealthText)
        {
            m_PlayerHealthText.text = m_Health.ToString();
        }
    }

    [Command]
    public void CmdRequestID()
    {
        BallGameNetworkManager.RequestIDFromServer((int)netId.Value, this);
    }

    [ClientRpc]
    public void RpcIDAssignmentResponse(int i_ID)
    {
        OnIDAssigned(new IDEventArgs(i_ID));
    }

    /// <summary>
    /// Raising the IDAssigned event.
    /// </summary>
    /// <param name="e">IDEventArgs containing the ID assigned to the player by the server.</param>
    private void OnIDAssigned(IDEventArgs e)
    {
        if (IDAssigned != null)
        {
            IDAssigned.Invoke(this, e);
        }
    }
        
    //Activates all methods based on ID assignment
    private void PlayerController_IDAssigned(object sender, IDEventArgs e)
    {
        m_ConnectionID = e.ID;
        AssignTextForPlayer(e.ID);
        AssignTextureForPlayer(e.ID);
    }

    private void AssignTextureForPlayer(int i_PlayerID)
    {
        GetComponent<Renderer>().material.mainTexture = m_PlayerTextures[i_PlayerID - 1];
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
    /// Increases the velocity of the pushed player by the amount of damage they've taken..
    /// </summary>
    /// <param name="i_PushedPlayer">Player to push</param>
    [Server]
    private IEnumerator serverPushPlayer(GameObject i_PushedPlayer)
    {
        yield return null;
        yield return null;
        PlayerScript pushedPlayer = i_PushedPlayer.GetComponent<PlayerScript>();
        pushedPlayer.m_Rigidbody.velocity *= (1 + (float)pushedPlayer.m_Health / (float)PlayerScript.k_HealthKnockbackStep);
    }

    /// <summary>
    /// Once the player flies off the screen, this will handle respawning.
    /// </summary>
    [Server]
    private void serverRespawnPlayer()
    {
        Transform spawnPoint = NetworkManager.singleton.startPositions[Random.Range(0, NetworkManager.singleton.startPositions.Count)];
        transform.position = spawnPoint.position;

        m_Health = 0;
        serverInitializePhysicsInfo();
    }

    /// <summary>
    /// Initializes physics for a respawned player.
    /// </summary>
    [Server]
    private void serverInitializePhysicsInfo()
    {
        m_Rigidbody.velocity = Vector3.zero;
    }

    void OnDestroy()
    {
        if (!m_PlayerHealthText)
        {
            return;
        }

        m_PlayerHealthText.text = "";
        m_PlayerHealthText.gameObject.SetActive(false);
    }

    //Movement is completely done on the server and then sent to the clients after calculations, so there's no sense in
    //checking for collision on the client, when the server already deals with it and nullifies client movement upon collision checks
    [ServerCallback]
    void OnCollisionEnter(Collision i_CollisionInfo)
    {
        if (i_CollisionInfo.gameObject.layer == LayerMask.NameToLayer(ConstParams.KillBoxLayer))
        {
            editPlayersKills();
            if (m_DeathCount >= k_MaxDeaths)
            {
                gameObject.SetActive(false);
            }

            serverRespawnPlayer();
        }

        if (i_CollisionInfo.gameObject.CompareTag(ConstParams.PlayerTag))
        {
            m_LastColidingPlayer = i_CollisionInfo.collider.gameObject;
            StartCoroutine(killCreditTimeLimit());
            StartCoroutine(serverPushPlayer(m_LastColidingPlayer));
            handleDamage(i_CollisionInfo);
        }
    }

    private IEnumerator killCreditTimeLimit()
    {
        yield return new WaitForSeconds(k_KillCreditTimeLimit);

        m_LastColidingPlayer = null;
    }

    private void editPlayersKills()
    {
        if (m_LastColidingPlayer != null)
        {
            m_LastColidingPlayer.GetComponent<PlayerScript>().m_KillCount += 1;
        }
        m_LastColidingPlayer = null;
        m_DeathCount += 1;
    }

    private void handleDamage(Collision i_CollisionInfo)
    {
        int baseDamage = (int)(i_CollisionInfo.relativeVelocity.sqrMagnitude - i_CollisionInfo.collider.GetComponent<Rigidbody>().velocity.sqrMagnitude);
        baseDamage /= k_DamageReduction;
        if (baseDamage > 0)
        {
            CmdDealDamage(baseDamage);
        }
    }

    [Command]
    public void CmdDealDamage(int i_BaseDamage)
    {
        m_Health += i_BaseDamage;
    }
}