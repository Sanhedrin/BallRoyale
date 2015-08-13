using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;

[AddComponentMenu("BallGame Scripts/Object Scripts/Projectile")]
public class Projectile : NetworkBehaviour
{
    private float m_LifeTime = 2f;
    const int k_ProjectileDamage = 100;

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

    [ServerCallback]
    void OnTriggerEnter(Collider i_Other)
    {
        if (i_Other.tag == ConstParams.PlayerTag)
        {
            PlayerScript player = i_Other.GetComponent<PlayerScript>();
            bool slowEffectFound = false;
            Rigidbody otherRigidBody = i_Other.GetComponent<Rigidbody>();
            PlayerControls playerControls = i_Other.GetComponent<PlayerControls>();

            player.CmdDealDamage(k_ProjectileDamage);

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
        }
    }
}
