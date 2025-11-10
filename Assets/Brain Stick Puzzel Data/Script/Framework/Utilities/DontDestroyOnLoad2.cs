using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad2 : MonoBehaviour
{
    public static DontDestroyOnLoad2 instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
