using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[AddComponentMenu("BallGame Scripts/Utilities/Text Removal")]
public class TextScript : MonoBehaviour
{
    public Text Text;

    // Use this for initialization
    void Start()
    {
        Text.text = "";
    }

    // Update is called once per frame
    void Update()
    {

    }
}
