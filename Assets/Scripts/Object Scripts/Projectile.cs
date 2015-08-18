using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;

[AddComponentMenu("BallGame Scripts/Object Scripts/Projectile")]
public class Projectile : Obstacle
{
    [SerializeField]
    private const int k_ProjectileDamage = 50;

    protected override void OnEnable()
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
        RpcActivateObstical(false);
    }

    [ServerCallback]
    void OnTriggerEnter(Collider i_Other)
    {
        if (i_Other.tag == ConstParams.PlayerTag)
        {
            PlayerScript player = i_Other.GetComponent<PlayerScript>();
            Rigidbody otherRigidBody = i_Other.GetComponent<Rigidbody>();
            PlayerControls playerControls = i_Other.GetComponent<PlayerControls>();

            player.CmdDealDamage(k_ProjectileDamage);
            playerControls.RpcAddSlowEffect();
            RpcActivateObstical(false);
        }
    }
}
