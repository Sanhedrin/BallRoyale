using UnityEngine;
using System.Collections;
using System;


public class PhaseShiftEffect : StatusEffect
{
    public override void ActivateEffect(Rigidbody i_EffectedRigidBody)
    {
        Renderer render = i_EffectedRigidBody.GetComponent<Renderer>();
        m_LastStarted = DateTime.Now;
        if (!render.GetComponent<PlayerScript>().isLocalPlayer)
        {
            render.enabled = false;
        }  
    }

    public override void RevertEffect(Rigidbody i_EffectedRigidBody)
    {
        Renderer render = i_EffectedRigidBody.GetComponent<Renderer>();
        render.enabled = true;
    }

}

