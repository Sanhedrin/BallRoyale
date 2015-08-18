using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[AddComponentMenu("BallGame Scripts/UI/Health Text")]
public class HealthText : MonoBehaviour
{
    private static int s_PlayerID = 0;

    public GameObject Player { get; set; }

    private Text m_HealthText;

    // Use this for initialization
    void Start()
    {
        transform.position = new Vector2(transform.position.x + (Screen.width / 4) * (s_PlayerID++), transform.position.y);

        m_HealthText = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateHealth(int i_NewHealth)
    {
        m_HealthText.text = i_NewHealth.ToString();
    }
}
