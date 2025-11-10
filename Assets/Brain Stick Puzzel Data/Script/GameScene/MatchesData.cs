using UnityEngine;

[System.Serializable]
public class MatchesData
{
    public Vector2Int startXY;
    public Side squareSide;
    public bool isEmpty = false;
}

public enum Side
{
    Right = 0,
    Left = 1,
    RightUp = 2,
    RightDown = 3,
    LeftUp = 4,
    LeftDown = 5
}

