using UnityEngine;
using System.Collections;

public class DashEffect :  StatusEffect
{
    private const int k_ForceBoost = 10;

    public override void ActivateEffect(Rigidbody i_EffectedRigidBody)
    {
        i_EffectedRigidBody.AddForce(i_EffectedRigidBody.velocity.normalized * k_ForceBoost);
    }

    public override void RevertEffect(Rigidbody i_EffectedRigidBody)
    {
        //No Revert Effect Needed for Dash Effect
    } 
}
