using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public static LevelData CurrentLevelData;
    public static bool openLevelSelectionMenu = false;
    public static bool openLevelPackSelectionMenu = false;
    public static bool IsSound
    {
        get
        {
            return PlayerPrefs.GetInt("IsSound", 1) == 1;
        }
        set
        {
            PlayerPrefs.SetInt("IsSound", value ? 1 : 0);
        }
    }
    public static int Hints
    {
        get { return PlayerPrefs.GetInt("Hints", 3); }
        set
        {
            PlayerPrefs.SetInt("Hints", value);

            var hintText = GameObject.FindWithTag("HintCount");
            if (hintText != null)
            {
                hintText.GetComponent<Text>().text = value.ToString();
            }
        }
    }

    public static bool IsAdRemoved
    {
        get { return PlayerPrefs.GetInt("IsAdRemoved") == 1; }
        set { PlayerPrefs.SetInt("IsAdRemoved", value ? 1 : 0); }
    }
}