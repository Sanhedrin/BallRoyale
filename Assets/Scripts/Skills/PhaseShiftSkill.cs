using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

[AddComponentMenu("BallGame Scripts/Skills/Phase Shift")]
public sealed class PhaseShiftSkill : Skill
{
    private Renderer m_render;

    private bool m_isActive = false;

    [SerializeField]
    protected float m_PhaseShiftDuration;

    protected override void OnAttachedToPlayer()
    {
        m_render = m_PlayerObject.GetComponent<Renderer>();            
    }

    protected override void OnDetachedFromPlayer()
    {
        m_render.enabled = true;
        m_PlayerObject.gameObject.layer = LayerMask.NameToLayer(ConstParams.PlayerLayer);
    }

    [Server]
    public override void Activate()
    {
        if (!m_isActive)
        {
            Debug.Log("in PhaseShift");
            m_isActive = true;
            StartCoroutine(phaseShiftTime(m_PhaseShiftDuration));
            enterPhaseShift();
        }
    }

    private void enterPhaseShift()
    {
        if (!m_render.GetComponent<PlayerScript>().isLocalPlayer) // only the local playeer can see himself
        {
            m_render.enabled = false;
        }

        m_PlayerObject.gameObject.layer = LayerMask.NameToLayer(ConstParams.PhasedLayer);
    }

    [Server]
    private IEnumerator phaseShiftTime(float i_Seconds)
    {
        yield return new WaitForSeconds(i_Seconds);

        OnDetachedFromPlayer();
    }

}
