using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

//[AddComponentMenu("BallGame Scripts/Object Scripts/Projectile")]
public class Obstical : NetworkBehaviour
{
    private Rigidbody m_OtherRigidBody;
    
    [SerializeField]
    protected float m_LifeTime = 2f;

    [SerializeField]
    private float m_ExplosionForce = 1000f;

    [SerializeField]
    private float m_ExplosionRadius = 1000f;

    protected virtual void OnEnable()
    {
        if (isServer)
        {
            Debug.Log("666");
            StartCoroutine(DestroyObsticl(m_LifeTime));
        }
    }

    [Server]
    IEnumerator DestroyObsticl(float i_StartIn)
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
        Debug.Log("דימה לא חבר");
        if (isServer)
        {
            if (i_Other.tag == ConstParams.PlayerTag)
            {
                m_OtherRigidBody = i_Other.GetComponent<Rigidbody>();


                m_OtherRigidBody.AddForce(Vector3.up * 2000);
                gameObject.SetActive(false);
            }
        }
    }
}
