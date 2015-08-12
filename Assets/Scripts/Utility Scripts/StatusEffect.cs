using UnityEngine;
using System.Collections;
using System;

public abstract class StatusEffect
{
    //Set Activation time in inheriting effect
    protected int ActivationTime = 0;
    protected DateTime m_LastStarted;

    public abstract void CmdActivateEffect(Rigidbody i_EffectedRigidBody);
    public abstract void CmdRevertEffect(Rigidbody i_EffectedRigidBody);
}
