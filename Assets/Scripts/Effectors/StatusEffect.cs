using UnityEngine;
using System.Collections;
using System;

public abstract class StatusEffect
{
    //Set Activation time in inheriting effect
    protected int m_EffectTime = 0;
    public int EffectTime
    {
        get
        {
            return m_EffectTime;
        }
    }

    protected DateTime m_LastStarted;
    public DateTime LastStarted
    {
        get
        {
            return m_LastStarted;
        }
    }

    public abstract void ActivateEffect(Rigidbody i_EffectedRigidBody);
    public abstract void RevertEffect(Rigidbody i_EffectedRigidBody);
}