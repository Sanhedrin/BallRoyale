using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

[AddComponentMenu("BallGame Scripts/Object Scripts/Projectile")]
public class Projectile : NetworkBehaviour
{
    private Rigidbody m_OtherRigidBody;

    private float m_LifeTime = 2f;

    [SerializeField]
    private float m_ExplosionForce = 1000f;

    [SerializeField]
    private float m_ExplosionRadius = 1000f;

    void OnEnable()
    {
        if (isServer)
        {
            StartCoroutine(DestroyProjectile(m_LifeTime));
        }
    }

    [Server]
    IEnumerator DestroyProjectile(float i_StartIn)
    {
        yield return new WaitForSeconds(i_StartIn);
        gameObject.SetActive(false);
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    void OnTriggerEnter(Collider i_Other)
    {
        if (isServer)
        {
            if (i_Other.tag == ConstNames.PlayerTag)
            {
                m_OtherRigidBody = i_Other.GetComponent<Rigidbody>();

                //TODO: improve the explosion effect/behaviour
                m_OtherRigidBody.AddExplosionForce(m_ExplosionForce, transform.position, m_ExplosionRadius);
                gameObject.SetActive(false);
            }
        }
    }
}
