using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewLevel", menuName = "CreateLevel/Level", order = 1)]
public class LevelData : ScriptableObject
{
    public int totalRows;
    public int totalCols;
    public int TotalMoves;
    public int TotalShape;
    public GameMode gameMode = 0;
    public GameType gameType = 0;
    public List<MatchesData> matches = new List<MatchesData>();
    public List<int> SolvedMatchesIndex = new List<int>();
    public string Number1, Number2, Number3;
    public EquationSign EquationSign;
}

public enum GameType
{
    Equation,
    Square,
    Triangle
}

public enum GameMode
{
    Move,
    Remove,
    Add
}
