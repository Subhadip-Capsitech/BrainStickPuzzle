using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Assets : MonoBehaviour
{
    public Sprite matchNormal, matchUnremovable, matchMoved;

    public static Assets instance;

    private void Awake()
    {
        instance = this;
    }
}
