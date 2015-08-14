using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using System;

[NetworkSettings(channel=2, sendInterval=1/ConstParams.NetTransformSyncRate)]
public class NetworkTransformSyncer : NetworkBehaviour
{
    [SerializeField]
    private float m_PredictionErrorThreshold;
    [SerializeField]
    private float m_SnapThreshold;
    private float m_TimeAtLastUpdate;
    private float m_TimeAtMostRecentUpdate;
    private Vector3 m_LastPosition;

    [SyncVar(hook="OnNewPositionReceived")]
    private Vector3 m_MostRecentPosition;
    private void OnNewPositionReceived(Vector3 i_NewPosition)
    {
        if (Time.time != m_TimeAtMostRecentUpdate)
        {
            m_TimeAtLastUpdate = m_TimeAtMostRecentUpdate;
            m_TimeAtMostRecentUpdate = Time.time;
            m_LastPosition = m_MostRecentPosition;
            m_MostRecentPosition = i_NewPosition;

            Debug.Log(m_TimeAtLastUpdate + " " + m_TimeAtMostRecentUpdate);
        }
    }

    private Rigidbody m_Rigidbody;

	// Use this for initialization
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();

        if (!isServer)
        {
            m_LastPosition = m_MostRecentPosition = transform.position;
            m_TimeAtMostRecentUpdate = m_TimeAtLastUpdate = Time.time;
        }
	}

    void FixedUpdate()
    {
        if (isServer)
        {
            m_MostRecentPosition = transform.position;
        }
    }

	// Update is called once per frame
    void Update()
    {
        if (!isServer)
        {
            if (Vector3.Distance(m_LastPosition, transform.position) > m_SnapThreshold)
            {
                m_Rigidbody.velocity = Vector3.zero;
                transform.position = m_LastPosition;
            }
            else if (Vector3.Distance(m_LastPosition, transform.position) > m_PredictionErrorThreshold)
            {
                m_Rigidbody.velocity = m_LastPosition - transform.position;
                transform.position = Vector3.Lerp(m_LastPosition, m_MostRecentPosition, (m_TimeAtMostRecentUpdate - Time.time) / (m_TimeAtMostRecentUpdate - m_TimeAtLastUpdate));
            }
       }
	}
}
