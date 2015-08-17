using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

[DisallowMultipleComponent]
public abstract class Skill : NetworkBehaviour
{
    protected PlayerControls m_PlayerObject;

    private Renderer m_Renderer;
    private Collider m_Collider;

    private bool m_AttachedToPlayer = false;

    [SerializeField]
    protected float m_DurationInSeconds;


    public abstract void Activate();
    protected abstract void OnAttachedToPlayer();
    protected abstract void OnDetachedFromPlayer();

    public virtual void Start()
    {
        m_Renderer = GetComponent<Renderer>();
        m_Collider = GetComponent<Collider>();
    }

    public virtual void Update()
    {
        if (m_AttachedToPlayer)
        {
            if (Input.GetButtonDown(ConstParams.FireButton) && m_PlayerObject.isLocalPlayer)
            {
                m_PlayerObject.ActivateSkill();
            }
        }
    }

    [ClientRpc]
    private void RpcAttachSkill(NetworkInstanceId i_PlayerID)
    {
        GameObject player = ClientScene.FindLocalObject(i_PlayerID);
        transform.SetParent(player.transform);
        m_PlayerObject = transform.root.GetComponentInChildren<PlayerControls>();
        m_AttachedToPlayer = true;
        m_Collider.enabled = false;
        //m_Renderer.enabled = false;

        transform.FindChild(ConstParams.BoxPrefab).gameObject.SetActive(false);

        OnAttachedToPlayer();
    }

    [ClientRpc]
    protected void RpcDetachSkill()
    {
        transform.position = new Vector3(-1337, -1337, -1337);
        transform.SetParent(null);
        m_AttachedToPlayer = false;
        m_Collider.enabled = true;
        //m_Renderer.enabled = true;

        OnDetachedFromPlayer();
        m_PlayerObject = null;
        gameObject.SetActive(false);
    }

    [Server]
    private IEnumerator skillUpTime(float i_Seconds)
    {
        yield return new WaitForSeconds(i_Seconds);

        RpcDetachSkill();
    }

    [ServerCallback]
    private void OnTriggerEnter(Collider i_Collider)
    {
        if (!m_AttachedToPlayer)
        {
            RpcAttachSkill(i_Collider.NetID());
            StartCoroutine(skillUpTime(m_DurationInSeconds));
        }
    }

    [ClientRpc]
    protected void RpcActivateObject(NetworkInstanceId i_GameObjectID, bool i_Activate)
    {
        ClientScene.FindLocalObject(i_GameObjectID).SetActive(i_Activate);
    }
}
