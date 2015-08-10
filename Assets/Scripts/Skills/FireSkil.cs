using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class FireSkil : Skill
{
    private float m_ProjectileSpeed = 30f;

    public override void Activate()
    {
        CmdShoot();
    }

    [Command]
    private void CmdShoot()
    {
        Vector3 velocityDir = GetComponent<Rigidbody>().velocity.normalized * transform.localScale.x;

        if (Vector3.zero != velocityDir)
        {
            GameObject currObj = ObjectPoolManager.Instance.GetPoolForObject(eObjectPoolNames.Bullet).PullObject();

            currObj.transform.position = transform.position + velocityDir;
            currObj.transform.rotation = Quaternion.LookRotation(velocityDir);
            currObj.transform.Rotate(new Vector3(0, 90, 0)); // rotate the missle by 90 degrees on the y axes
            currObj.GetComponent<Rigidbody>().velocity = velocityDir * m_ProjectileSpeed;
            RpcActivateBullet(currObj);
        }
    }

    [ClientRpc]
    private void RpcActivateBullet(GameObject i_gameObj)
    {
        Debug.Log("I'm here! Witness me!");
        i_gameObj.SetActive(true);
    }

}
