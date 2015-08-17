using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

[AddComponentMenu("BallGame Scripts/Skills/Phase Shift")]
public sealed class PhaseShiftSkill : Skill
{
    private Renderer m_PlayerRender;

    private bool m_isActive = false;

    [SerializeField]
    private float m_PhaseShiftDuration = 4;

    protected override void OnAttachedToPlayer()
    {
        m_PlayerRender = m_PlayerObject.GetComponent<Renderer>();            
    }

    protected override void OnDetachedFromPlayer()
    {
        m_PlayerRender.enabled = true;
        m_PlayerObject.gameObject.layer = LayerMask.NameToLayer(ConstParams.PlayerLayer);
        Debug.Log(LayerMask.LayerToName(m_PlayerObject.gameObject.layer));
    }


    public override void Activate()
    {
        if (!m_isActive)
        {
            Debug.Log("in PhaseShift");
            m_isActive = true;
            StartCoroutine(phaseShiftTime(m_PhaseShiftDuration));
            RpcEnterPhaseShift();
        }
    }

    [ClientRpc]
    private void RpcEnterPhaseShift()
    {
        if (!m_PlayerObject.isLocalPlayer) // only the local playeer can see himself
        {
            m_PlayerRender.enabled = false;
        }

        m_PlayerObject.gameObject.layer = LayerMask.NameToLayer(ConstParams.PhasedLayer);
        Debug.Log(LayerMask.LayerToName(m_PlayerObject.gameObject.layer));
    }

    [Server]
    private IEnumerator phaseShiftTime(float i_Seconds)
    {
        yield return new WaitForSeconds(i_Seconds);

        RpcDetachSkill();
    }

}
