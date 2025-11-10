using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetRemovable : MonoBehaviour
{
    public bool isRemovable;

    private void Start()
    {
        GetComponent<Matches>().IsRemovable = isRemovable;
    }
}
