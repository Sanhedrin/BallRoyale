using UnityEngine;
using System.Collections;
using System;


public class PhaseShiftEffect : StatusEffect
{
    public override void ActivateEffect(Rigidbody i_EffectedRigidBody)
    {
        MeshRenderer Renderer = i_EffectedRigidBody.GetComponent<MeshRenderer>();       
    }

    public override void RevertEffect(Rigidbody i_EffectedRigidBody)
    {
        
    }

}

