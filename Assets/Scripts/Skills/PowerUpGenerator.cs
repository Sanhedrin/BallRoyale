using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PowerUpGenerator : MonoBehaviour
{
    private float width;
    private float height;

    // Use this for initialization
    void Start()
    {
        CalcSpawnableArea();
        StartCoroutine(GeneratePowerup());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void CalcSpawnableArea()
    {
        width = (transform.localScale.x - 4) / 2;
        height = (transform.localScale.z - 4) / 2;
    }

    private IEnumerator GeneratePowerup()
    {
        for (;;)
        {
            yield return new WaitForSeconds(Random.Range(4, 6));

            var pu = ObjectPoolManager.Instance.GetPoolForObject(eObjectPoolNames.PowerupItem).PullObject();
            pu.transform.position = new Vector3(Random.Range(-width, width), 1, Random.Range(-height, height));
        }
    }
}
