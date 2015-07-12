using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
    private Rigidbody m_OtherRigidBody;

    [SerializeField]
    float m_LifeTime = 5;

    [SerializeField]
    float m_ExplosionForce = 1000f;

    [SerializeField]
    float m_ExplosionRadius = 1000f;

    void Start()
    {
        Destroy(gameObject, m_LifeTime);
    }

    void OnTriggerEnter(Collider i_Other)
    {
        if (i_Other.tag == "Player")
        {
            m_OtherRigidBody = i_Other.GetComponent<Rigidbody>();

            m_OtherRigidBody.AddExplosionForce(m_ExplosionForce, transform.position, m_ExplosionRadius);
            Destroy(gameObject);
        }
    }
}
