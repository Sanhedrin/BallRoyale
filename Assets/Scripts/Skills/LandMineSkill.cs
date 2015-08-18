using UnityEngine;
using System.Collections;
using UnityEngine.Networking;


public class LandMineSkill : Skill {

    private Rigidbody m_PlayerRigidbody;
    private bool m_coldownForlayingNextMine = true;

    [SerializeField]
    private float m_coldownTimerForLyingMins = 1f;

    protected override void OnAttachedToPlayer()
    {
        m_PlayerRigidbody = m_PlayerObject.GetComponent<Rigidbody>();
        m_coldownForlayingNextMine = true;
    }

    protected override void OnDetachedFromPlayer()
    {
        m_PlayerRigidbody = null;
    }

    [Server]
    public override void Activate()
    {
        if (m_PlayerObject.Grounded && m_coldownForlayingNextMine)
        {
            setMine();
        }
    }

    private void setMine()
    {             
            GameObject currObj = ObjectPoolManager.Instance.GetPoolForObject(eObjectPoolNames.LandMine).PullObject();

            currObj.transform.position = m_PlayerObject.transform.position;

            StartCoroutine(coldownActivatMine(m_coldownTimerForLyingMins));
               
            RpcActivateObject(currObj.transform.NetID(), true);       
    }

    [Server]
    private IEnumerator coldownActivatMine(float i_Seconds)
    {
        m_coldownForlayingNextMine = false;
        yield return new WaitForSeconds(i_Seconds);
        m_coldownForlayingNextMine = true;     
    }

}
