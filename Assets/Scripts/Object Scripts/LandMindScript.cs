using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class LandMindScript : Obstacle {

    [SerializeField]
    private float m_activMinTimer = 1f;
    [SerializeField]
    private float k_forceOfMine = 500f;
    

    Collider m_Collider = null;
    Renderer m_Renderer = null;                     


    void Start()
    {
        m_Collider = GetComponent<Collider>();
        m_Renderer = GetComponent<Renderer>();             
    }

    protected override void OnEnable()
    {
        if (m_Renderer && m_Collider)
        {
            m_Renderer.enabled = true;
            m_Collider.enabled = false;
        }

        StartCoroutine(ActivatMin(m_activMinTimer, m_LifeTime));
    }

    [Server]
    IEnumerator ActivatMin(float i_activMinTimer, float i_DestroyTimer)
    {
        yield return new WaitForSeconds(i_activMinTimer);

        m_Renderer.enabled = false;
        m_Collider.enabled = true;

        yield return new WaitForSeconds(i_DestroyTimer);

        RpcActivateObstical(false);
    }

    [ServerCallback]
    void OnTriggerEnter(Collider i_Other)
    {
        if (i_Other.tag == ConstParams.PlayerTag)
        {
            m_OtherRigidBody = i_Other.GetComponent<Rigidbody>();

            m_OtherRigidBody.AddForce(Vector3.up * k_forceOfMine);

            i_Other.GetComponent<PlayerScript>().CmdDealDamage(k_DamageToPlyer); 

            RpcActivateObstical(false);
        }
    }

}
