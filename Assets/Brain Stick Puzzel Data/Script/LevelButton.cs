using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class LevelButton : MonoBehaviour
{
    private int rows;
    private int cols;
    private Vector2 gridSize;
    private RectTransform rt;

    public RectTransform gridTr;
    public PointClass squrePointPrefab, triangelPointPrefab;
    public Matches m_MatchesPrefab;
    public EquationLevelData m_EquationLevelPrefab;
    public List<Matches> allMatches = new List<Matches>();

    public PointClass[,] m_GridPoints;
    [HideInInspector]
    public LevelData currentLevel;

    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        rt = GetComponent<RectTransform>();
        gridSize = new Vector2(rt.rect.width * 0.7f, rt.rect.width * 0.7f);

        currentLevel = Resources.Load<LevelData>(LevelManager.Intance.CurrentLevelPack.LevelsPath + transform.GetSiblingIndex());

        if (currentLevel == null) yield break;

        rows = currentLevel.totalRows;
        cols = currentLevel.totalCols;
        m_GridPoints = new PointClass[cols, rows];

        if (currentLevel.gameType == GameType.Equation)
        {
            EquationLevel();

            Timer.Schedule(this, 0.1f, () =>
            {
                foreach (var match in allMatches)
                {
                    match.GetComponent<Button>().enabled = false;
                }
            });
        }
        else
        {
            if (currentLevel.gameType == GameType.Square)
                SquareGrid(rows, cols);
            else
                TriangleGrid(rows, cols);

            // Align the grid
            float minX = int.MaxValue, minY = int.MaxValue, maxX = int.MinValue, maxY = int.MinValue;
            foreach (var match in allMatches)
            {
                float x1 = match.start.GetComponent<RectTransform>().anchoredPosition.x;
                float y1 = match.start.GetComponent<RectTransform>().anchoredPosition.y;

                float x2 = match.end.GetComponent<RectTransform>().anchoredPosition.x;
                float y2 = match.end.GetComponent<RectTransform>().anchoredPosition.y;

                if (x1 < minX) minX = x1;
                if (x1 > maxX) maxX = x1;

                if (x2 < minX) minX = x2;
                if (x2 > maxX) maxX = x2;

                if (y1 < minY) minY = y1;
                if (y1 > maxY) maxY = y1;

                if (y2 < minY) minY = y2;
                if (y2 > maxY) maxY = y2;
            }

            float width = (maxX - minX);
            float height = (maxY - minY);
            Vector3 center = new Vector3(0.5f * (minX + maxX), 0.5f * (minY + maxY));

            gridTr.sizeDelta = new Vector2(width, height);
            gridTr.localPosition = -center + Vector3.down * 35f;
        }
    }

    void EquationLevel()
    {
        var equationData = Instantiate(m_EquationLevelPrefab, gridTr);
        equationData.transform.localScale = Vector3.one;

        equationData.Num1 = currentLevel.Number1;
        equationData.Num2 = currentLevel.Number2;
        equationData.Num3 = currentLevel.Number3;
        equationData.EquationSign = currentLevel.EquationSign;

        equationData.matches = allMatches;
        equationData.SetupEquation();
        equationData.SetupOprator();
        equationData.SetMaxWidth(gridSize.x);
    }

    void SquareGrid(int rows, int cols)
    {
        float cellSize = Mathf.Min(gridSize.x / (cols - 1), gridSize.y / (rows - 1));

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                //add the cell size so that no two cells will have the same x and y position
                Vector2 pos = new Vector2(x * cellSize, y * cellSize);
                //instantiate the game object, at position pos, with rotation set to identity
                PointClass cO = Instantiate(squrePointPrefab, gridTr);
                cO.transform.localScale = Vector3.one;
                cO.GetComponent<RectTransform>().anchoredPosition = pos;
                cO.posX = x;
                cO.posY = y;
                cO.name = "point " + x + "," + y;

                //set the parent of the cell to GRID so you can move the cells together with the grid;
                cO.transform.SetParent(transform);
                m_GridPoints[x, y] = cO;
            }
        }

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                SquarePoint temp = (SquarePoint)m_GridPoints[x, y];
                if (IsValidPointPos(x + 1, y))
                {
                    temp.Right = (SquarePoint)m_GridPoints[x + 1, y];
                }
                if (IsValidPointPos(x - 1, y))
                {
                    temp.Left = (SquarePoint)m_GridPoints[x - 1, y];
                }
                if (IsValidPointPos(x, y + 1))
                {
                    temp.Up = (SquarePoint)m_GridPoints[x, y + 1];
                }
                if (IsValidPointPos(x, y - 1))
                {
                    temp.Down = (SquarePoint)m_GridPoints[x, y - 1];
                }
            }
        }

        PlaceSquareMatches();
    }

    void TriangleGrid(int rows, int cols)
    {
        float cellSize = Mathf.Min(gridSize.x / (cols - 1), gridSize.y / (rows - 1));
        float triOffset = cellSize / -2f;

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                if (y % 2 != 0)
                {
                    Vector2 pos = new Vector2(x * cellSize + triOffset, y * cellSize * Mathf.Sin(60 * Mathf.Deg2Rad));
                    PointClass cO = Instantiate(triangelPointPrefab, gridTr);
                    cO.transform.localScale = Vector3.one;
                    cO.GetComponent<RectTransform>().anchoredPosition = pos;
                    cO.posX = x;
                    cO.posY = y;
                    cO.name = "point " + x + "," + y;

                    m_GridPoints[x, y] = cO;
                }
                else
                {
                    Vector2 pos = new Vector2(x * cellSize, y * cellSize * Mathf.Sin(60 * Mathf.Deg2Rad));
                    PointClass cO = Instantiate(triangelPointPrefab, gridTr);
                    cO.transform.localScale = Vector3.one;
                    cO.GetComponent<RectTransform>().anchoredPosition = pos;
                    cO.posX = x;
                    cO.posY = y;
                    cO.name = "point " + x + "," + y;

                    m_GridPoints[x, y] = cO;
                }
            }
        }
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {

                TrianglePoint temp = (TrianglePoint)m_GridPoints[x, y];
                if (IsValidPointPos(x + 1, y))
                {
                    temp.Right = (TrianglePoint)m_GridPoints[x + 1, y];
                }
                if (IsValidPointPos(x - 1, y))
                {
                    temp.Left = (TrianglePoint)m_GridPoints[x - 1, y];
                }
                if (y % 2 != 0)
                {

                    if (IsValidPointPos(x - 1, y + 1))
                    {
                        temp.LeftUp = (TrianglePoint)m_GridPoints[x - 1, y + 1];
                    }
                    if (IsValidPointPos(x - 1, y - 1))
                    {
                        temp.LeftDown = (TrianglePoint)m_GridPoints[x - 1, y - 1];
                    }
                    if (IsValidPointPos(x, y + 1))
                    {
                        temp.RightUp = (TrianglePoint)m_GridPoints[x, y + 1];
                    }
                    if (IsValidPointPos(x, y - 1))
                    {
                        temp.RightDown = (TrianglePoint)m_GridPoints[x, y - 1];
                    }
                }
                else
                {
                    if (IsValidPointPos(x, y + 1))
                    {
                        temp.LeftUp = (TrianglePoint)m_GridPoints[x, y + 1];
                    }
                    if (IsValidPointPos(x, y - 1))
                    {
                        temp.LeftDown = (TrianglePoint)m_GridPoints[x, y - 1];
                    }
                    if (IsValidPointPos(x + 1, y + 1))
                    {
                        temp.RightUp = (TrianglePoint)m_GridPoints[x + 1, y + 1];
                    }
                    if (IsValidPointPos(x + 1, y - 1))
                    {
                        temp.RightDown = (TrianglePoint)m_GridPoints[x + 1, y - 1];
                    }
                }
            }
        }

        PlaceTriangleMatches();
    }

    Matches CreateMatchPlace(PointClass start, PointClass end, bool isEmpty = false)
    {
        Matches m_Matches = Instantiate(m_MatchesPrefab, gridTr);
        m_Matches.transform.localScale = Vector3.one;
        m_Matches.start = start;
        m_Matches.end = end;
        m_Matches.Load(isEmpty);
      
        return m_Matches;
    }

    void PlaceSquareMatches()
    {
        foreach (var matchData in currentLevel.matches)
        {
            SquarePoint temp = (SquarePoint)m_GridPoints[matchData.startXY.x, matchData.startXY.y];

            if (matchData.squareSide == Side.Right)
            {
                if (temp.Right != null && temp.RightMatches == null)
                {
                    temp.RightMatches = CreateMatchPlace(temp, temp.Right, matchData.isEmpty);
                    allMatches.Add(temp.RightMatches);
                }
            }
            if (matchData.squareSide == Side.RightUp)
            {
                if (temp.Up != null && temp.UpMatches == null)
                {
                    temp.UpMatches = CreateMatchPlace(temp, temp.Up, matchData.isEmpty);
                    allMatches.Add(temp.UpMatches);
                }
            }
        }
    }

    void PlaceTriangleMatches()
    {
        foreach (var matchData in currentLevel.matches)
        {
            TrianglePoint temp = (TrianglePoint)m_GridPoints[matchData.startXY.x, matchData.startXY.y];

            if (matchData.squareSide == Side.Left)
            {
                if (temp.Left != null && temp.LeftMatches == null)
                {
                    temp.LeftMatches = CreateMatchPlace(temp, temp.Left, matchData.isEmpty);
                    allMatches.Add(temp.LeftMatches);
                }
            }
            if (matchData.squareSide == Side.Right)
            {
                if (temp.Right != null && temp.RightMatches == null)
                {
                    temp.RightMatches = CreateMatchPlace(temp, temp.Right, matchData.isEmpty);
                    allMatches.Add(temp.RightMatches);
                }
            }
            if (matchData.squareSide == Side.LeftUp)
            {
                if (temp.LeftUp != null && temp.LeftUpMatches == null)
                {
                    temp.LeftUpMatches = CreateMatchPlace(temp, temp.LeftUp, matchData.isEmpty);
                    allMatches.Add(temp.LeftUpMatches);
                }
            }
            if (matchData.squareSide == Side.RightUp)
            {
                if (temp.RightUp != null && temp.RightUpMatches == null)
                {
                    temp.RightUpMatches = CreateMatchPlace(temp, temp.RightUp, matchData.isEmpty);
                    allMatches.Add(temp.RightUpMatches);

                }
            }

            if (matchData.squareSide == Side.LeftDown)
            {
                if (temp.LeftDown != null && temp.LeftDownMatches == null)
                {
                    temp.LeftDownMatches = CreateMatchPlace(temp, temp.LeftDown, matchData.isEmpty);
                    allMatches.Add(temp.LeftDownMatches);

                }
            }
            if (matchData.squareSide == Side.RightDown)
            {
                if (temp.RightDown != null && temp.RightDownMatches == null)
                {
                    temp.RightDownMatches = CreateMatchPlace(temp, temp.RightDown, matchData.isEmpty);
                    allMatches.Add(temp.RightDownMatches);
                }
            }
        }
    }

    bool IsValidPointPos(int x, int y)
    {
        if (x < 0 || y < 0)
        {
            return false;
        }

        if (x >= cols || y >= rows)
            return false;

        return true;
    }
}
