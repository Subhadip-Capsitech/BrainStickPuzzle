//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class LevelManager : MonoBehaviour
//{
//    public static LevelManager Intance;

//    public List<LevelPack> PackList;

//    public int CurrentLevelIndex = 0;
//    public int CurrentLevelPackIndex = 0;

//    public LevelPack CurrentLevelPack
//    {
//        get { return PackList[CurrentLevelPackIndex]; }
//        set { CurrentLevelPackIndex = PackList.IndexOf(value); }
//    }

//    public int CompletedPack
//    {
//        get { return PlayerPrefs.GetInt("CompletedPackIndex", 0); }
//        set { PlayerPrefs.SetInt("CompletedPackIndex", value); }
//    }

//    void Awake()
//    {
//        if (Intance == null)
//        {
//            Intance = this;
//            DontDestroyOnLoad(this);
//        }
//        else
//        {
//            Destroy(gameObject);
//        }
//    }

//    // -----------------------------
//    // Mark level as completed
//    // -----------------------------
//    public void SetLevelCompleted(LevelPack levelPack, int levelIndex)
//    {
//        levelPack.CompletedLevels =
//            Mathf.Max(levelPack.CompletedLevels, levelIndex + 1);

//        if (levelPack.IsPackCompleted &&
//            CompletedPack == PackList.IndexOf(levelPack))
//        {
//            CompletedPack++;
//        }
//    }

//    public void SetLevelCompletedInCurrentPack(int levelIndex)
//    {
//        SetLevelCompleted(CurrentLevelPack, levelIndex);
//    }

//    // -----------------------------
//    // UNLOCK EVERYTHING
//    // -----------------------------
//    public void UnlockAllLevels()
//    {
//        // Unlock all packs
//        CompletedPack = PackList.Count;

//        // Unlock all levels inside each pack
//        foreach (var pack in PackList)
//        {
//            pack.CompletedLevels = pack.TotalLevels;
//        }

//        PlayerPrefs.Save();
//        Debug.Log("All packs and levels UNLOCKED");
//    }
//}


//// =============================================================
//// Level Pack Class
//// =============================================================
//[System.Serializable]
//public class LevelPack
//{
//    public string LevelPackName = "Pack ";
//    public string LevelsPath;
//    public int TotalLevels = 20;

//    public int CompletedLevels
//    {
//        get { return PlayerPrefs.GetInt("CompletedLevel_" + LevelPackName, 0); }
//        set { PlayerPrefs.SetInt("CompletedLevel_" + LevelPackName, value); }
//    }

//    public int CompletedLevelInPercent
//    {
//        get { return (int)(CompletedLevels / (float)TotalLevels * 100f); }
//    }

//    public bool IsPackCompleted
//    {
//        get { return CompletedLevels >= TotalLevels; }
//    }
//}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Intance;

    public List<LevelPack> PackList;

    public int CurrentLevelIndex = 0;
    public int CurrentLevelPackIndex = 0;

    public LevelPack CurrentLevelPack
    {
        get { return PackList[CurrentLevelPackIndex]; }
        set { CurrentLevelPackIndex = PackList.IndexOf(value); }
    }

    public int CompletedPack
    {
        get { return PlayerPrefs.GetInt("CompletedPackIndex", 0); }
        set { PlayerPrefs.SetInt("CompletedPackIndex", value); }
    }

    void Awake()
    {
        if (Intance == null)
        {
            Intance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetLevelCompleted(LevelPack levelPack, int levelIndex)
    {
        levelPack.CompletedLevels = levelPack.CompletedLevels < (levelIndex + 1) ? (levelIndex + 1) : levelPack.CompletedLevels;
        if (levelPack.IsPackCompleted && CompletedPack == PackList.IndexOf(levelPack))
        {
            CompletedPack++;
        }
    }

    public void SetLevelCompletedInCurrentPack(int levelIndex)
    {
        SetLevelCompleted(CurrentLevelPack, levelIndex);
    }
}

[System.Serializable]
public class LevelPack
{
    public string LevelPackName = "Pack ";
    public string LevelsPath;
    public int TotalLevels = 20;
    public int CompletedLevels
    {
        get { return PlayerPrefs.GetInt("CompletedLevel_" + LevelPackName, 0); }
        set { PlayerPrefs.SetInt("CompletedLevel_" + LevelPackName, value); }
    }

    public int CompletedLevelInPercent
    {
        get { return (int)(CompletedLevels / (float)TotalLevels * 100); }
    }

    public bool IsPackCompleted
    {
        get { return TotalLevels == CompletedLevels; }
    }
}
