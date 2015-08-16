using UnityEngine;
using System.Collections;
using UnityEngine.Networking;


public class LandMineSkill : Skill {

    private Rigidbody m_PlayerRigidbody;

    protected override void OnAttachedToPlayer()
    {
        m_PlayerRigidbody = m_PlayerObject.GetComponent<Rigidbody>();
    }

    protected override void OnDetachedFromPlayer()
    {
        m_PlayerRigidbody = null;
    }

    [Server]
    public override void Activate()
    {
        Shoot();
    }

    private void Shoot()
    {             
            GameObject currObj = ObjectPoolManager.Instance.GetPoolForObject(eObjectPoolNames.LandMide).PullObject();

            currObj.transform.position = m_PlayerObject.transform.position;
               
            RpcActivateObject(currObj.transform.NetID(), true);       
    }

}
