using UnityEngine;
using System.Collections;
using System;

public class StunEffect : StatusEffect
{
    public const int k_StunTime = 1;

    public StunEffect()
    {
        m_EffectTime = k_StunTime;
    }

    public override void ActivateEffect(Rigidbody i_EffectedRigidBody)
    {
        i_EffectedRigidBody.velocity = Vector3.zero;
    }
    
    public override void RevertEffect(Rigidbody i_EffectedRigidBody)
    {
    }
}
