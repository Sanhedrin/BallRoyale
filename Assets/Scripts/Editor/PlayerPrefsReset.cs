using UnityEngine;
using UnityEditor;
using System.Collections;

public class PlayerPrefsReset : EditorWindow {

    [MenuItem("Edit/Reset Playerprefs")]
    public static void DeletePlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
}
