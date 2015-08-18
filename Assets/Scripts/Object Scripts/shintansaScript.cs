//using UnityEngine;
//using System.Collections;
//using UnityEngine.Networking;
//using System.Collections.Generic;

//public class shintansaScript : Obstacle {

//    GameObject m_plyerUsingMe;
//    List<int> m_idofPlyersHit = null;

//    protected override void OnEnable()
//    {
//        transform.localScale = m_plyerUsingMe.transform.localScale * 1.3f;

//        if(m_idofPlyersHit == null)
//        {
//            m_idofPlyersHit = new List<int>();
//        }
//        m_idofPlyersHit.Clear();
//    }

//    void Update()
//    {
//        transform.position = m_plyerUsingMe.transform.position;

//        if(transform.localScale.x > m_plyerUsingMe.transform.localScale.x *2.5)
//        {
//            transform.localScale = m_plyerUsingMe.transform.localScale * 1.3f;
//        }
//        else
//        {
//            transform.localScale = transform.localScale * 1.3f;
//        }
//    }

//    [ServerCallback]
//    void OnTriggerEnter(Collider i_Other)
//    {
//        if (i_Other.tag == ConstParams.PlayerTag)
//        {
//            m_OtherRigidBody = i_Other.GetComponent<Rigidbody>();

//            m_OtherRigidBody.AddForce(m_OtherRigidBody.GetComponent<Rigidbody>().velocity);

//            int otherPlyerNetId = (int)i_Other.NetID().Value;

//            if (!m_idofPlyersHit.Contains(otherPlyerNetId))
//            {
//                i_Other.GetComponent<PlayerScript>().CmdDealDamage(k_DamageToPlyer);
//                m_idofPlyersHit.Add(otherPlyerNetId);
//            }            
//        }
//    }	
//}
