using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MS;

public class LevelDesignerGrid : MonoBehaviour
{
    public static LevelDesignerGrid Instance;

    private int rows;
    private int cols;
    private Vector2 gridSize;
    private Vector2 cellSize;

    private RectTransform rootCanvas;

    public PointClass squrePointPrefab, triangelPointPrefab;

    public PointClass[,] m_GridPoints;
    public Matches m_MatchesPrefab;
    public EquationLevelData m_EquationLevelPrefab;
    public List<Matches> allMatches = new List<Matches>();

    [Header("UI")]
    public LevelData currentLevel;
    EquationLevelData equationData;
    public List<MatchesData> matchesDataList = new List<MatchesData>();

    void Start()
    {
        Instance = this;
        rootCanvas = GameObject.FindWithTag("RootCanvas").GetComponent<RectTransform>();
    }

    public void SetupLevel()
    {
        gridSize = new Vector2(rootCanvas.rect.width * 0.9f, rootCanvas.rect.height * 0.58f);

        if (currentLevel.gameType == GameType.Equation)
        {
            EquationLevel();
        }
        else if (currentLevel.gameType == GameType.Square || currentLevel.gameType == GameType.Triangle)
        {
            rows = currentLevel.totalRows;
            cols = currentLevel.totalCols;
            m_GridPoints = new PointClass[cols, rows];
                
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

            GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
            GetComponent<RectTransform>().localPosition = -center + Vector3.down * 350f;
        }
    }

    void EquationLevel()
    {
        equationData = Instantiate(m_EquationLevelPrefab, transform);
        equationData.transform.localScale = Vector3.one;
        equationData.transform.localPosition += Vector3.down * 300f;

        equationData.Num1 = currentLevel.Number1;
        equationData.Num2 = currentLevel.Number2;
        equationData.Num3 = currentLevel.Number3;
        equationData.EquationSign = currentLevel.EquationSign;

        equationData.matches = allMatches;
        equationData.SetupEquation();
        equationData.SetupOprator();
        equationData.SetMaxWidth(gridSize.x * 0.95f);
    }

    void SquareGrid(int rows, int cols)
    {
        float cellSize = Mathf.Min(gridSize.x / (cols - 1), gridSize.y / (rows - 1));
        if (cellSize > rootCanvas.rect.height / 6f) cellSize = rootCanvas.rect.height / 6f;

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                //add the cell size so that no two cells will have the same x and y position
                Vector2 pos = new Vector2(x * cellSize, y * cellSize);
                //instantiate the game object, at position pos, with rotation set to identity
                PointClass cO = Instantiate(squrePointPrefab, transform);
                cO.transform.localScale = Vector3.one;
                cO.GetComponent<RectTransform>().anchoredPosition = pos;
                cO.posX = x;
                cO.posY = y;
                cO.name = "point " + x + "," + y;
                cO.inLevelEditor = true;

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
                    MatchesData m = new MatchesData();
                    m.startXY = new Vector2Int(x, y);
                    m.squareSide = Side.Right;
                    currentLevel.matches.Add(m);
                }
                if (IsValidPointPos(x - 1, y))
                {
                    temp.Left = (SquarePoint)m_GridPoints[x - 1, y];
                    MatchesData m = new MatchesData();
                    m.startXY = new Vector2Int(x, y);
                    m.squareSide = Side.Left;
                    currentLevel.matches.Add(m);
                }
                if (IsValidPointPos(x, y + 1))
                {
                    temp.Up = (SquarePoint)m_GridPoints[x, y + 1];

                    MatchesData m = new MatchesData();
                    m.startXY = new Vector2Int(x, y);
                    m.squareSide = Side.RightUp;
                    currentLevel.matches.Add(m);
                }
                if (IsValidPointPos(x, y - 1))
                {
                    temp.Down = (SquarePoint)m_GridPoints[x, y - 1];

                    MatchesData m = new MatchesData();
                    m.startXY = new Vector2Int(x, y);
                    m.squareSide = Side.RightDown;
                    currentLevel.matches.Add(m);
                }
            }
        }

        PlaceSquareMatches();
    }

    void TriangleGrid(int rows, int cols)
    {
        float cellSize = Mathf.Min(gridSize.x / (cols - 1), gridSize.y / (rows - 1));
        if (cellSize > rootCanvas.rect.height / 6f) cellSize = rootCanvas.rect.height / 6f;
        float triOffset = cellSize / -2f;

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                if (y % 2 != 0)
                {
                    Vector2 pos = new Vector2(x * cellSize + triOffset, y * cellSize * Mathf.Sin(60 * Mathf.Deg2Rad));
                    PointClass cO = Instantiate(triangelPointPrefab, transform);
                    cO.transform.localScale = Vector3.one;
                    cO.GetComponent<RectTransform>().anchoredPosition = pos;
                    cO.posX = x;
                    cO.posY = y;
                    cO.name = "point " + x + "," + y;
                    cO.inLevelEditor = true;

                    m_GridPoints[x, y] = cO;
                }
                else
                {
                    Vector2 pos = new Vector2(x * cellSize, y * cellSize * Mathf.Sin(60 * Mathf.Deg2Rad));
                    PointClass cO = Instantiate(triangelPointPrefab, transform);
                    cO.transform.localScale = Vector3.one;
                    cO.GetComponent<RectTransform>().anchoredPosition = pos;
                    cO.posX = x;
                    cO.posY = y;
                    cO.name = "point " + x + "," + y;
                    cO.inLevelEditor = true;

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

                    MatchesData m = new MatchesData();
                    m.startXY = new Vector2Int(x, y);
                    m.squareSide = Side.Right;
                    currentLevel.matches.Add(m);
                }

                if (IsValidPointPos(x - 1, y))
                {
                    temp.Left = (TrianglePoint)m_GridPoints[x - 1, y];

                    MatchesData m = new MatchesData();
                    m.startXY = new Vector2Int(x, y);
                    m.squareSide = Side.Left;
                    currentLevel.matches.Add(m);
                }

                if (y % 2 != 0)
                {
                    if (IsValidPointPos(x - 1, y + 1))
                    {
                        temp.LeftUp = (TrianglePoint)m_GridPoints[x - 1, y + 1];

                        MatchesData m = new MatchesData();
                        m.startXY = new Vector2Int(x, y);
                        m.squareSide = Side.LeftUp;
                        currentLevel.matches.Add(m);
                    }
                    if (IsValidPointPos(x - 1, y - 1))
                    {
                        temp.LeftDown = (TrianglePoint)m_GridPoints[x - 1, y - 1];
                        MatchesData m = new MatchesData();
                        m.startXY = new Vector2Int(x, y);
                        m.squareSide = Side.LeftDown;
                        currentLevel.matches.Add(m);
                    }
                    if (IsValidPointPos(x, y + 1))
                    {
                        temp.RightUp = (TrianglePoint)m_GridPoints[x, y + 1];
                        MatchesData m = new MatchesData();
                        m.startXY = new Vector2Int(x, y);
                        m.squareSide = Side.RightUp;
                        currentLevel.matches.Add(m);
                    }
                    if (IsValidPointPos(x, y - 1))
                    {
                        temp.RightDown = (TrianglePoint)m_GridPoints[x, y - 1];
                        MatchesData m = new MatchesData();
                        m.startXY = new Vector2Int(x, y);
                        m.squareSide = Side.RightDown;
                        currentLevel.matches.Add(m);
                    }
                }
                else
                {
                    if (IsValidPointPos(x, y + 1))
                    {
                        temp.LeftUp = (TrianglePoint)m_GridPoints[x, y + 1];
                        MatchesData m = new MatchesData();
                        m.startXY = new Vector2Int(x, y);
                        m.squareSide = Side.LeftUp;
                        currentLevel.matches.Add(m);
                    }
                    if (IsValidPointPos(x, y - 1))
                    {
                        temp.LeftDown = (TrianglePoint)m_GridPoints[x, y - 1];
                        MatchesData m = new MatchesData();
                        m.startXY = new Vector2Int(x, y);
                        m.squareSide = Side.LeftDown;
                        currentLevel.matches.Add(m);
                    }
                    if (IsValidPointPos(x + 1, y + 1))
                    {
                        temp.RightUp = (TrianglePoint)m_GridPoints[x + 1, y + 1];
                        MatchesData m = new MatchesData();
                        m.startXY = new Vector2Int(x, y);
                        m.squareSide = Side.RightUp;
                        currentLevel.matches.Add(m);
                    }
                    if (IsValidPointPos(x + 1, y - 1))
                    {
                        temp.RightDown = (TrianglePoint)m_GridPoints[x + 1, y - 1];
                        MatchesData m = new MatchesData();
                        m.startXY = new Vector2Int(x, y);
                        m.squareSide = Side.RightDown;
                        currentLevel.matches.Add(m);
                    }
                }
            }
        }

        PlaceTriangleMatches();
    }

    Matches CreateMatchPlace(PointClass start, PointClass end, bool isEmpty = false)
    {
        Matches m_Matches = Instantiate(m_MatchesPrefab, transform);
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
                    matchesDataList.Add(matchData);
                    allMatches.Add(temp.RightMatches);
                }
            }
            if (matchData.squareSide == Side.RightUp)
            {
                if (temp.Up != null && temp.UpMatches == null)
                {
                    temp.UpMatches = CreateMatchPlace(temp, temp.Up, matchData.isEmpty);
                    matchesDataList.Add(matchData);
                    allMatches.Add(temp.UpMatches);
                }
            }
        }
    }

    void PlaceTriangleMatches()
    {
        foreach(var matchData in currentLevel.matches)
        {
            TrianglePoint temp = (TrianglePoint)m_GridPoints[matchData.startXY.x, matchData.startXY.y];

            if (matchData.squareSide == Side.Left)
            {
                if (temp.Left != null && temp.LeftMatches == null)
                {
                    temp.LeftMatches = CreateMatchPlace(temp, temp.Left, matchData.isEmpty);
                    allMatches.Add(temp.LeftMatches);
                    matchesDataList.Add(matchData);
                }
            }
            if (matchData.squareSide == Side.Right)
            {
                if (temp.Right != null && temp.RightMatches == null)
                {
                    temp.RightMatches = CreateMatchPlace(temp, temp.Right, matchData.isEmpty);
                    allMatches.Add(temp.RightMatches);
                    matchesDataList.Add(matchData);
                }
            }
            if (matchData.squareSide == Side.LeftUp)
            {
                if (temp.LeftUp != null && temp.LeftUpMatches == null)
                {
                    temp.LeftUpMatches = CreateMatchPlace(temp, temp.LeftUp, matchData.isEmpty);
                    allMatches.Add(temp.LeftUpMatches);
                    matchesDataList.Add(matchData);
                }
            }
            if (matchData.squareSide == Side.RightUp)
            {
                if (temp.RightUp != null && temp.RightUpMatches == null)
                {
                    temp.RightUpMatches = CreateMatchPlace(temp, temp.RightUp, matchData.isEmpty);
                    allMatches.Add(temp.RightUpMatches);
                    matchesDataList.Add(matchData);
                }
            }

            if (matchData.squareSide == Side.LeftDown)
            {
                if (temp.LeftDown != null && temp.LeftDownMatches == null)
                {
                    temp.LeftDownMatches = CreateMatchPlace(temp, temp.LeftDown, matchData.isEmpty);
                    allMatches.Add(temp.LeftDownMatches);
                    matchesDataList.Add(matchData);
                }
            }
            if (matchData.squareSide == Side.RightDown)
            {
                if (temp.RightDown != null && temp.RightDownMatches == null)
                {
                    temp.RightDownMatches = CreateMatchPlace(temp, temp.RightDown, matchData.isEmpty);
                    allMatches.Add(temp.RightDownMatches);
                    matchesDataList.Add(matchData);
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

    public int GetTotalTriangleShape()
    {
        int total = 0;
        for (int i = 1; i <= rows; i++)
        {
            total += (CountTriangle(i));
        }
        return total;
    }

    public int GetTotalSquareShape()
    {
        int total = 0;
        for (int i = 1; i <= rows; i++)
        {
            total += (CountSquare(i));
        }
        return total;
    }

    int CountSquare(int size)
    {
        int totalSquare = 0;
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                SquarePoint temp = (SquarePoint)m_GridPoints[x, y];
                if (temp.CheckStepRight(size))
                {
                    temp = temp.GetStepRight(size);
                    if (temp.CheckStepUp(size))
                    {
                        temp = temp.GetStepUp(size);
                        if (temp.CheckStepLeft(size))
                        {
                            temp = temp.GetStepLeft(size);
                            if (temp.CheckStepDown(size))
                            {
                                totalSquare++;
                            }
                        }
                    }
                }
            }
        }
        return totalSquare;
    }

    int CountTriangle(int size)
    {
        int totalTriangle = 0;
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                TrianglePoint temp = (TrianglePoint)m_GridPoints[x, y];
                if (temp.CheckStepRight(size))
                {
                    TrianglePoint tempUp = temp.GetStepRight(size);
                    TrianglePoint tempDown = temp.GetStepRight(size);

                    if (tempUp.CheckStepLeftUp(size))
                    {
                        tempUp = tempUp.GetStepLeftUp(size);
                        if (tempUp.CheckStepLeftDown(size))
                        {
                            totalTriangle++;
                        }
                    }
                    if (tempDown.CheckStepLeftDown(size))
                    {
                        tempDown = tempDown.GetStepLeftDown(size);
                        if (tempDown.CheckStepLeftUp(size))
                        {
                            totalTriangle++;
                        }
                    }
                }
            }
        }
        return totalTriangle;
    }

    public int GetTotalMatchInGamePlay()
    {
        return allMatches.FindAll(x => x.IsPlaced()).Count;
    }

    public int GetTotalSquareMatches()
    {
        List<Matches> list = new List<Matches>();

        for (int i = 1; i <= rows; i++)
        {
            List<Matches> temp = GetSquareMatches(i);
            temp.ForEach((obj) =>
            {
                if (!list.Exists((a) => a.Equals(obj)))
                {
                    list.Add(obj);
                }
            });
        }
        return list.Count;
    }

    public int GetTotalTrigleMatches()
    {
        List<Matches> list = new List<Matches>();
        for (int i = 1; i <= rows; i++)
        {
            List<Matches> temp = GetTriangleMatches(i);
            temp.ForEach((obj) =>
            {
                if (!list.Exists((a) => a != null && a.Equals(obj)))
                {
                    list.Add(obj);
                }
            });
        }

        return list.Count;
    }

    List<Matches> GetSquareMatches(int size)
    {
        List<Matches> list = new List<Matches>();
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                SquarePoint temp = (SquarePoint)m_GridPoints[x, y];

                if (temp.CheckStepRight(size))
                {
                    List<Matches> tempList = temp.GetRightMatches(size);
                    temp = temp.GetStepRight(size);

                    if (temp.CheckStepUp(size))
                    {
                        tempList.AddRange(temp.GetUpMatches(size));
                        temp = temp.GetStepUp(size);

                        if (temp.CheckStepLeft(size))
                        {
                            tempList.AddRange(temp.GetLeftMatches(size));
                            temp = temp.GetStepLeft(size);
                            if (temp.CheckStepDown(size))
                            {
                                tempList.AddRange(temp.GetDownMatches(size));
                                list.AddRange(tempList);
                            }
                        }
                    }
                }
            }
        }
        return list;
    }

    List<Matches> GetTriangleMatches(int size)
    {
        List<Matches> list = new List<Matches>();
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {

                TrianglePoint temp = (TrianglePoint)m_GridPoints[x, y];
                if (temp.CheckStepRight(size))
                {
                    List<Matches> tempListUp = temp.GetMatchesRight(size);
                    List<Matches> tempListDown = temp.GetMatchesRight(size);

                    TrianglePoint tempUp = temp.GetStepRight(size);
                    TrianglePoint tempDown = temp.GetStepRight(size);


                    if (tempUp.CheckStepLeftUp(size))
                    {
                        tempListUp.AddRange(tempUp.GetMatchesLeftUp(size));
                        tempUp = tempUp.GetStepLeftUp(size);

                        if (tempUp.CheckStepLeftDown(size))
                        {
                            tempListUp.AddRange(tempUp.GetMatchesLeftDown(size));
                            list.AddRange(tempListUp);
                        }
                    }
                    if (tempDown.CheckStepLeftDown(size))
                    {
                        tempListDown.AddRange(tempDown.GetMatchesLeftDown(size));
                        tempDown = tempDown.GetStepLeftDown(size);
                        if (tempDown.CheckStepLeftUp(size))
                        {
                            tempListDown.AddRange(tempDown.GetMatchesLeftUp(size));
                            list.AddRange(tempListDown);
                        }
                    }
                }
            }
        }
        return list;
    }

    public string GetEquationContent()
    {
        string number1 = GetDigits(equationData.Number1);
        string number2 = GetDigits(equationData.Number2);
        string number3 = GetDigits(equationData.Number3);
        var sign = equationData.OpratorTransform.GetComponentInChildren<Oprator>().GetOperator();

        if (sign == EquationSign.Plus) return number1 + " + " + number2 + " = " + number3;
        else if (sign == EquationSign.Minus) return number1 + " - " + number2 + " = " + number3;
        else if (sign == EquationSign.Multiply) return number1 + " x " + number2 + " = " + number3;
        else if (sign == EquationSign.Division) return number1 + " / " + number2 + " = " + number3;
        else return null;
    }

    public bool IsEquationRight()
    {
        string number1 = GetDigits(equationData.Number1);
        string number2 = GetDigits(equationData.Number2);
        string number3 = GetDigits(equationData.Number3);
        var sign = equationData.OpratorTransform.GetComponentInChildren<Oprator>().GetOperator();

        int num1, num2, num3;
        if (!int.TryParse(number1, out num1))
        {
            return false;
        }

        if (!int.TryParse(number2, out num2))
        {
            return false;
        }

        if (!int.TryParse(number3, out num3))
        {
            return false;
        }

        float result = int.MaxValue;
        if (sign == EquationSign.Plus) result = num1 + num2;
        else if (sign == EquationSign.Minus) result = num1 - num2;
        else if (sign == EquationSign.Multiply) result = num1 * num2;
        else if (sign == EquationSign.Division) result = num1 / num2;

        return result == num3;
    }

    private string GetDigits(Transform numberTr)
    {
        string digits = "";
        foreach (Transform child in numberTr)
        {
            int digit = child.GetComponent<Digit>().GetDigit();
            if (digit == -2)
            {
                digits += "*";
                continue;
            }

            if (digit == -1 && child.GetSiblingIndex() == 0 && numberTr.childCount > 1)
            {
                digit = 0;
            }

            digits += digit.ToString();
        }
        return digits;
    }
}
