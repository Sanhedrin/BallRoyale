using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

[AddComponentMenu("BallGame Scripts/Skills/Fire")]
public sealed class FireSkil : Skill
{
    private float m_ProjectileSpeed = 30f;
    private Rigidbody m_PlayerRigidbody;

    protected override void OnAttachedToPlayer()
    {
        m_PlayerRigidbody = m_PlayerObject.GetComponent<Rigidbody>();
    }

    protected override void OnDetachedFromPlayer()
    {
        m_PlayerRigidbody = null;
    }

    [Server]
    public override void Activate()
    {
        Shoot();
    }

    private void Shoot()
    {
        Vector3 velocityDir = m_PlayerRigidbody.velocity.normalized * m_PlayerObject.transform.localScale.x;

        if (Vector3.zero != velocityDir)
        {
            GameObject currObj = ObjectPoolManager.Instance.GetPoolForObject(eObjectPoolNames.Bullet).PullObject();

            currObj.transform.position = m_PlayerObject.transform.position + velocityDir;
            currObj.transform.rotation = Quaternion.LookRotation(velocityDir);
            currObj.transform.Rotate(new Vector3(0, 90, 0)); // rotate the missle by 90 degrees on the y axes
            Rigidbody rigBody = currObj.GetComponent<Rigidbody>();
            rigBody.velocity = velocityDir * m_ProjectileSpeed;
            RpcActivateObject(rigBody.NetID(), true);
        }
    }
}
