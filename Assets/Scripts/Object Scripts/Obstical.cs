using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

//[AddComponentMenu("BallGame Scripts/Object Scripts/Projectile")]
public class Obstacle : NetworkBehaviour
{
    private Rigidbody m_OtherRigidBody;
    
    [SerializeField]
    protected float m_LifeTime = 2f;

    protected virtual void OnEnable()
    {
        if (isServer)
        {
            StartCoroutine(DestroyObstacle(m_LifeTime));
        }
    }

    [Server]
    IEnumerator DestroyObstacle(float i_StartIn)
    {
        yield return new WaitForSeconds(i_StartIn);
        gameObject.SetActive(false);
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    [ServerCallback]
    void OnTriggerEnter(Collider i_Other)
    {
        if (i_Other.tag == ConstParams.PlayerTag)
        {
            m_OtherRigidBody = i_Other.GetComponent<Rigidbody>();

            m_OtherRigidBody.AddForce(Vector3.up * 2000);
            gameObject.SetActive(false);
        }
    }
}
