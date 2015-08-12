using UnityEngine;
using System.Collections;
using System;

public class StunEffect : StatusEffect
{
    public const int k_StunTime = 1;

    public StunEffect()
    {
        ActivationTime = k_StunTime;
    }

    public override void CmdActivateEffect(Rigidbody i_EffectedRigidBody)
    {
        i_EffectedRigidBody.velocity = Vector3.zero;
    }

    public override void CmdRevertEffect(Rigidbody i_EffectedRigidBody)
    {
    }
}
