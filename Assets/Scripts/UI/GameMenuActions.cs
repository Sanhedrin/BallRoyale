using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[AddComponentMenu("BallGame Scripts/UI/Game Menu Actions")]
public class GameMenuActions : MonoBehaviour
{
    public UnityEngine.UI.InputField HostIP;

    public void StartHosting()
    {
        NetworkManager.singleton.StartHost();
    }

    public void JoinGame()
    {
        //if (string.IsNullOrEmpty(HostIP.text))
        //    return;

        //System.Net.IPAddress IPCheck;

        //if (!System.Net.IPAddress.TryParse(HostIP.text, out IPCheck))
        //{
        //    return;
        //}

        if (HostIP.text != "")
        {
            NetworkManager.singleton.networkAddress = HostIP.text;
        }

        NetworkManager.singleton.StartClient();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
