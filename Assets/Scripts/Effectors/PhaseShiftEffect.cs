using UnityEngine;
using System.Collections;
using System;


public class PhaseShiftEffect : StatusEffect
{
    public override void ActivateEffect(Rigidbody i_EffectedRigidBody)
    {
        m_LastStarted = DateTime.Now;
        Renderer render = i_EffectedRigidBody.GetComponent<Renderer>();
        if (!render.GetComponent<PlayerScript>().isLocalPlayer)
        {
            render.enabled = false;
        }

        i_EffectedRigidBody.gameObject.layer = LayerMask.NameToLayer(ConstParams.PhasedLayer);
    }

    public override void RevertEffect(Rigidbody i_EffectedRigidBody)
    {
        Renderer render = i_EffectedRigidBody.GetComponent<Renderer>();
        render.enabled = true;
        i_EffectedRigidBody.gameObject.layer = LayerMask.NameToLayer(ConstParams.PlayerLayer);
    }
}

