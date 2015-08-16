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

    public abstract void Activate(PlayerControls i_PlayerControls);
    public abstract void Deactivate(PlayerControls i_PlayerControls);

    public void RefreshTimer()
    {
        m_LastStarted = DateTime.Now;
    }
}