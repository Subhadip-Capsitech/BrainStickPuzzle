using UnityEngine;
using UnityEditor;

public class GameWiseWindowEditor
{
    [MenuItem("GameWise Games/Reset game")]
    static void ResetGame()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }

    [MenuItem("GameWise Games/Unlock all packs")]
    static void UnlockAllPacks()
    {
        var levelManager = Object.FindObjectOfType<LevelManager>();
        if (levelManager != null)
        {
            int numPacks = levelManager.PackList.Count;
            PlayerPrefs.SetInt("CompletedPackIndex", numPacks - 1);
            PlayerPrefs.Save();
        }
    }
}