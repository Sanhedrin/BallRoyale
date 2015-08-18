using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking.Types;
using UnityEngine.UI;

[AddComponentMenu("BallGame Scripts/UI/Game Menu Actions")]
public class GameMenuActions : MonoBehaviour
{
    public UnityEngine.UI.InputField HostIP;

    List<MatchDesc> matchList = new List<MatchDesc>();
    bool matchCreated;
    NetworkMatch NetworkMatch;

    List<Button> ServerList;

    public void Awake()
    {
        NetworkMatch = gameObject.AddComponent<NetworkMatch>();
        //StartCoroutine(RefreshServers());
    }

    private IEnumerator RefreshServers()
    {
        for (; ; )
        {
            yield return new WaitForSeconds(5);

            NetworkMatch.ListMatches(0, 20, "", OnMatchList);
        }
    }

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

    public void CreateRoom()
    {
        CreateMatchRequest create = new CreateMatchRequest();
        create.name = string.Format("BallRoom {0}", Random.Range(10001, 99999));
        create.size = 4;
        create.advertise = true;
        create.password = "";

        var rn = GameObject.Find("RoomNumber");
        rn.GetComponent<Text>().text = create.name;

        NetworkMatch.CreateMatch(create, OnMatchCreate);
    }

    public void OnMatchCreate(CreateMatchResponse matchResponse)
    {
        if (matchResponse.success)
        {
            Debug.Log("Create match succeeded");
            matchCreated = true;
            Utility.SetAccessTokenForNetwork(matchResponse.networkId, new NetworkAccessToken(matchResponse.accessTokenString));
            NetworkServer.Listen(new MatchInfo(matchResponse), 9000);
        }
        else
        {
            Debug.LogError("Create match failed");
        }
    }

    public void OnMatchList(ListMatchResponse matchListResponse)
    {
        if (matchListResponse.success && matchListResponse.matches != null && !matchList.Equals(matchListResponse.matches))
        {
            matchList = matchListResponse.matches;

            var sl = GameObject.Find("ServerList");

            foreach (var match in matchList)
            {
                var server = new GameObject("button", typeof(RectTransform));
                server.AddComponent<Button>();
                server.AddComponent<CanvasRenderer>();
                server.AddComponent<Image>();


                var text = new GameObject("text", typeof(RectTransform));
                text.AddComponent<CanvasRenderer>();
                text.AddComponent<Text>();
                var txt = text.GetComponent<Text>();
                txt.text = match.name;
                txt.font = Resources.FindObjectsOfTypeAll<Font>()[0];
                txt.color = Color.black;

                text.transform.SetParent(server.transform);
                server.transform.SetParent(sl.transform);
            }
        }
    }

    public void OnMatchJoined(JoinMatchResponse matchJoin)
    {
        if (matchJoin.success)
        {
            Debug.Log("Join match succeeded");
            if (matchCreated)
            {
                Debug.LogWarning("Match already set up, aborting...");
                return;
            }
            Utility.SetAccessTokenForNetwork(matchJoin.networkId, new NetworkAccessToken(matchJoin.accessTokenString));
            NetworkClient myClient = new NetworkClient();
            myClient.RegisterHandler(MsgType.Connect, OnConnected);
            myClient.Connect(new MatchInfo(matchJoin));
        }
        else
        {
            Debug.LogError("Join match failed");
        }
    }

    public void OnConnected(NetworkMessage msg)
    {
        Debug.Log("Connected!");
        
    }
}
