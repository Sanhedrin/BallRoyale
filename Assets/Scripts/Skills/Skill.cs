using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

[DisallowMultipleComponent]
public abstract class Skill : NetworkBehaviour
{
    public abstract void Activate();

    public void Update()
    {
        if (isLocalPlayer && Input.GetButtonDown(ConstNames.FireButton))
        {
            Activate();
        }
    }
}
