using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
    void OnTriggerEnter(Collider i_Other)
    {
        if ((i_Other.GetHashCode() != this.GetHashCode()) && i_Other.tag == "Player")
        {
            
        }
    }
}
