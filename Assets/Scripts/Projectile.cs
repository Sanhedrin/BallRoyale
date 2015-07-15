using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
    private Rigidbody m_OtherRigidBody;

    private float m_LifeTime = 2f;

    [SerializeField]
    private float m_ExplosionForce = 1000f;

    [SerializeField]
    private float m_ExplosionRadius = 1000f;

    void OnEnable()
    {
        StartCoroutine(DestroyProjectile(m_LifeTime));
    }

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
        if (i_Other.tag == "Player")
        {
            m_OtherRigidBody = i_Other.GetComponent<Rigidbody>();

            //TODO: improve the explosion effect/behaviour
            m_OtherRigidBody.AddExplosionForce(m_ExplosionForce, transform.position, m_ExplosionRadius);
            gameObject.SetActive(false);
        }
    }
}
