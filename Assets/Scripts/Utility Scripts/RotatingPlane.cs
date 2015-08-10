using UnityEngine;
using System.Collections;

[AddComponentMenu("BallGame Scripts/Utilities/Rotating Plane")]
public class RotatingPlane : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, Time.deltaTime * 2, 0, Space.World);
    }
}
