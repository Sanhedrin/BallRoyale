using UnityEngine;
using System.Collections;

public class PhaseShiftSkill : Skill
{
    public override void Activate()
    {
        gameObject.layer = LayerMask.NameToLayer(ConstNames.PhasedLayer);
    }
}
