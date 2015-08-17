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

    public override void Activate(PlayerControls i_PlayerControls)
    {
        i_PlayerControls.Rigidbody.velocity = Vector3.zero;
    }

    public override void Deactivate(PlayerControls i_PlayerControls)
    {

    }
}
