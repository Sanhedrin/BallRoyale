using UnityEngine;
using System.Collections;
using System;

public class SlowEffect : StatusEffect
{
    private const int k_AddedDrag = 5;
    private const int k_SlowTime = 2;

    public SlowEffect()
    {
        m_EffectTime = k_SlowTime;
    }

    public override void ActivateEffect(Rigidbody i_EffectedRigidBody)
    {
        m_LastStarted = DateTime.Now;
        i_EffectedRigidBody.drag += k_AddedDrag;
    }

    public override void RevertEffect(Rigidbody i_EffectedRigidBody)
    {
        i_EffectedRigidBody.drag -= k_AddedDrag;
    }
}
