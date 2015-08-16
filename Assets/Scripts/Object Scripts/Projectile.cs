using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;

[AddComponentMenu("BallGame Scripts/Object Scripts/Projectile")]
public class Projectile : Obstacle
{
   
    
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
        RpcActivetObstical(false);
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
            PlayerScript player = i_Other.GetComponent<PlayerScript>();
            Rigidbody otherRigidBody = i_Other.GetComponent<Rigidbody>();
            PlayerControls playerControls = i_Other.GetComponent<PlayerControls>();
            bool slowEffectFound = false;

            player.CmdDealDamage(k_DamageToPlyer);

            foreach (StatusEffect effect in playerControls.ActiveEffects)
            {
                if (effect is SlowEffect)
                {
                    slowEffectFound = true;
                    effect.ActivateEffect(otherRigidBody);
                    break;
                }
            }

            if (!slowEffectFound)
            {
                SlowEffect slowEffect = new SlowEffect();

                playerControls.ActiveEffects.Add(slowEffect);
                slowEffect.ActivateEffect(otherRigidBody);
            }
            RpcActivetObstical(false);
        }
    }
}
