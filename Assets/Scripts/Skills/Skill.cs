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
            if (Input.GetButtonDown(ConstNames.FireButton) && m_PlayerObject.isLocalPlayer)
            {
                Activate();
            }
        }
    }

    [ClientRpc]
    private void RpcAttachSkill(GameObject i_Player)
    {
        transform.SetParent(i_Player.transform);
        m_PlayerObject = transform.root.GetComponentInChildren<PlayerControls>();
        m_AttachedToPlayer = true;
        m_Collider.enabled = false;
        m_Renderer.enabled = false;

        OnAttachedToPlayer();
    }

    [ClientRpc]
    private void RpcDetachSkill()
    {
        transform.SetParent(null);
        m_PlayerObject = null;
        m_AttachedToPlayer = false;
        m_Collider.enabled = true;
        m_Renderer.enabled = true;

        OnDetachedFromPlayer();
    }

    [Server]
    private IEnumerator skillUpTime(float i_Seconds)
    {
        yield return new WaitForSeconds(i_Seconds);

        RpcDetachSkill();
        ObjectPoolManager.Instance.GetPoolForObject(eObjectPoolNames.PowerupItem);
    }

    private void OnTriggerEnter(Collider i_Collider)
    {
        if (isServer)
        {
            if (!m_AttachedToPlayer)
            {
                RpcAttachSkill(i_Collider.gameObject);
                StartCoroutine(skillUpTime(m_DurationInSeconds));
            }
        }
    }
}
