using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

//[AddComponentMenu("BallGame Scripts/Object Scripts/Projectile")]
public abstract class Obstacle : NetworkBehaviour
{
    protected Rigidbody m_OtherRigidBody;
    
    [SerializeField]
    protected float m_LifeTime = 11f;
    [SerializeField]
    protected int k_DamageToPlyer = 100;

    protected virtual void OnEnable()
    {

    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    [ClientRpc]
    protected void RpcActivateObstical(bool i_Activate)
    {
        gameObject.SetActive(i_Activate);
    }
}
