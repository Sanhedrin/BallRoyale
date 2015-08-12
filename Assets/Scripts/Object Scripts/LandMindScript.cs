using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class LandMindScript : Obstical {

    [SerializeField]
    private float m_activMinTimer = 1f;

    protected override void OnEnable()
    {
        StartCoroutine(ActivatMin(m_activMinTimer));
    }

    [Server]
    protected IEnumerator ActivatMin(float i_activMinTimer)
    {
        yield return new WaitForSeconds(i_activMinTimer);
        
    }


}
