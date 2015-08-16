using UnityEngine;
using System.Collections;
using System;

public class SlowEffect : StatusEffect
{
    private const float k_AddedDrag = 10000.0f;
    private const int k_SlowTime = 5;

    public SlowEffect()
    {
        m_EffectTime = k_SlowTime;
    }

    public override void Activate(PlayerControls i_PlayerControls)
    {
        RefreshTimer();
        i_PlayerControls.MoveSpeed = ConstParams.SlowedMoveSpeed;
    }

    public override void Deactivate(PlayerControls i_PlayerControls)
    {
        i_PlayerControls.MoveSpeed = ConstParams.BaseMoveSpeed;
    }
}
