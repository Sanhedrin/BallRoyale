using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
    private Rigidbody m_OtherRigidBody;

    private GameObject m_Launcher;

    [SerializeField]
    float m_ExplosionForce = 1000f;

    [SerializeField]
    float m_ExplosionRadius = 1000f;

    void OnTriggerEnter(Collider i_Other)
    {
        //Needs more work...
        
        //if (i_Other.tag == "Player")
        //{
        //    m_OtherRigidBody = i_Other.GetComponent<Rigidbody>();

        //    m_OtherRigidBody.AddExplosionForce(m_ExplosionForce, transform.position, m_ExplosionRadius);

        //    Destroy(this);
        //}
    }
}
