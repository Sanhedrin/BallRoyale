using UnityEngine;
using System.Collections;
using System;

public class SlowEffect : StatusEffect
{
    
    private const int k_AddedDrag = 5;

    static SlowEffect()
    {
        ActivationTime = 2;
    }

    public override void CmdActivateEffect(Rigidbody i_EffectedRigidBody)
    {
        m_LastStarted = DateTime.Now;
        i_EffectedRigidBody.drag += k_AddedDrag;
    }

    public override void CmdRevertEffect(Rigidbody i_EffectedRigidBody)
    {
        i_EffectedRigidBody.drag -= k_AddedDrag;
    }
}
