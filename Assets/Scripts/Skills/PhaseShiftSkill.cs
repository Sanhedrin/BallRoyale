using UnityEngine;
using System.Collections;

[AddComponentMenu("BallGame Scripts/Skills/Phase Shift")]
public sealed class PhaseShiftSkill : Skill
{
    protected override void OnAttachedToPlayer()
    {
        
    }

    protected override void OnDetachedFromPlayer()
    {
        
    }

    public override void Activate()
    {
        m_PlayerObject.gameObject.layer = LayerMask.NameToLayer(ConstParams.PhasedLayer);
    }
}
