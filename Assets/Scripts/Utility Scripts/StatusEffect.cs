using UnityEngine;
using System.Collections;
using System;

public abstract class StatusEffect
{
    //Implement static ctor in order to set activation time.
    public static int ActivationTime = 0;
    protected DateTime m_LastStarted;

    public abstract void CmdActivateEffect(Rigidbody i_EffectedRigidBody);
    public abstract void CmdRevertEffect(Rigidbody i_EffectedRigidBody);
}
