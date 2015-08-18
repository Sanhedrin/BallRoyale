using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

[AddComponentMenu("BallGame Scripts/Skills/Dash")]
public class DashSkill : Skill
{
    private Rigidbody m_PlayerRigidBody;

    private Vector3 m_SpeedBeforeDash;

    [SerializeField]
    private int m_ForceBoost = 10;

    protected override void OnAttachedToPlayer()
    {
        m_PlayerRigidBody = m_PlayerObject.GetComponent<Rigidbody>();
    }

    protected override void OnDetachedFromPlayer()
    {
        m_PlayerRigidBody.velocity = m_SpeedBeforeDash;
        m_PlayerRigidBody = null;
    }

    public override void Activate()
    {
        Debug.Log("Dash Activated");
        m_SpeedBeforeDash = m_PlayerRigidBody.velocity;
        m_PlayerRigidBody.AddForce(m_PlayerRigidBody.velocity.normalized * m_ForceBoost);
    }

}
