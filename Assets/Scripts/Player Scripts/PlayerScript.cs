using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

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
    }

    private const int k_MaxHealth = 999;
    private const int k_DamageReduction = 10;

    [SerializeField]
    private Texture[] m_PlayerTextures;

    /// <summary>
    /// How the knockback should be calculated from the health. The lower this is, the higher the knockback scaling becomes.
    /// </summary>
    private const int k_HealthKnockbackStep = 150;

    //Exposing this member will help us save performance by removing GetComponent() calls which are very expensive.
    [HideInInspector]
    public Rigidbody m_Rigidbody;
    private Transform m_PlayerUICanvas;
    private HealthText m_HealthText;

    public int PlayerID { get; private set; }

	// Use this for initialization
	void Start ()
    {
        m_Rigidbody = GetComponent<Rigidbody>();

        //Attach a health indicator to this player;
        m_PlayerUICanvas = GameObject.FindGameObjectWithTag(ConstParams.PlayerUICanvasTag).transform;
        m_HealthText = (Instantiate(Resources.Load(ConstParams.HealthTextObject), new Vector3(Screen.width / 8, Screen.height / 8, 0), Quaternion.identity) as GameObject).GetComponent<HealthText>();
        m_HealthText.transform.SetParent(m_PlayerUICanvas);
        m_HealthText.Player = gameObject;

        //Get the player's ID
        PlayerID = GameObject.FindObjectsOfType<HealthText>().Length;

        //Change texture to fit player ID
        GetComponent<Renderer>().material.mainTexture = m_PlayerTextures[PlayerID - 1];

        Random.seed = System.DateTime.Now.Millisecond;
	}
	
	// Update is called once per frame, non-physics updates should be writen here.
    void Update()
    {
        if (m_HealthText)
        {
            m_HealthText.UpdateHealth(m_Health);
        }
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
        if (m_HealthText)
        {
            m_HealthText.GetComponent<Text>().text = "";
            m_HealthText.gameObject.SetActive(false);
        }

    }

    //Movement is completely done on the server and then sent to the clients after calculations, so there's no sense in
    //checking for collision on the client, when the server already deals with it and nullifies client movement upon collision checks
    [ServerCallback]
    void OnCollisionEnter(Collision i_CollisionInfo)
    {
        if (i_CollisionInfo.gameObject.layer == LayerMask.NameToLayer(ConstParams.KillBoxLayer))
        {
            serverRespawnPlayer();
        }

        if (i_CollisionInfo.gameObject.CompareTag(ConstParams.PlayerTag))
        {
            StartCoroutine(serverPushPlayer(i_CollisionInfo.collider.gameObject));

            int baseDamage = (int)(i_CollisionInfo.relativeVelocity.sqrMagnitude - i_CollisionInfo.collider.GetComponent<Rigidbody>().velocity.sqrMagnitude);
            baseDamage /= k_DamageReduction;

            if (baseDamage > 0)
            {
                CmdDealDamage(baseDamage);
            }
        }
    }

    [Command]
    public void CmdDealDamage(int i_BaseDamage)
    {
        m_Health += i_BaseDamage;
    }
}
