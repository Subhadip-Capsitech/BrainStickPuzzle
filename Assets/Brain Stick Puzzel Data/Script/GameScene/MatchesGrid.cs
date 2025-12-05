using MS;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MatchesGrid : MonoBehaviour
{
    public static MatchesGrid Instance;
   
    private int rows;
    private int cols;
    private Vector2 gridSize;
    public float cellSize = 200f;

    public RectTransform rootCanvas;
    public PointClass squrePointPrefab, triangelPointPrefab;

    public PointClass[,] m_GridPoints;
    public Matches m_MatchesPrefab;
    public GameObject m_ChildMatchPrefab;
    public EquationLevelData m_EquationLevelPrefab;
    public int usedMoves = 0;
    public Transform listMatchTransform;
    public List<Matches> allMatches = new List<Matches>();
    public List<GameObject> listNewMatches = new List<GameObject>();
    public List<GameObject> listEmptyTransform = new List<GameObject>();

    [Header("UI")]
    public Popup afterLevelDonePopUp;
    public Popup gameOverPopUp;
    public Popup hintPopup;
    public Text gameOverTitleTxt;
    public Text txtNumMove, txtLevelNo, txtInstructionText, hintTotalTxt;
    public Color matchMovedColor, matchMovedColorInEquation;
    EquationLevelData equationData;
    public ObjectiveHandler objectiveHandlerScript;
    int hintCounter = 0;

    [HideInInspector]
    public LevelData currentLevel;

    public int UsedMoves
    {
        get { return usedMoves; }
        set { usedMoves = value; txtNumMove.text = value + "/" + currentLevel.TotalMoves; }
    }

    void Start()
    {
        Instance = this;

        gridSize = new Vector2(rootCanvas.rect.width * 0.9f, rootCanvas.rect.height * 0.55f);
        SetupLevel();
        hintCounter = 0;
        hintTotalTxt.text = GameManager.Hints.ToString();

        if (PlayerPrefs.HasKey("played_game"))
        {
            Timer.Schedule(this, 0.8f, () =>
            {
                AdmobController.instance.ShowInterstitial();
            });
        }

        Music.instance.PlayAMusic();
    }
    #region Level Setup
    void SetupLevel()
    {
        GameManager.CurrentLevelData = Resources.Load<LevelData>(LevelManager.Intance.CurrentLevelPack.LevelsPath + LevelManager.Intance.CurrentLevelIndex);
        if (GameManager.CurrentLevelData == null)
        {
            Debug.LogError("Level Not Found at : " + LevelManager.Intance.CurrentLevelPack.LevelsPath + LevelManager.Intance.CurrentLevelIndex);
            return;
        }
        else
        {
            currentLevel = GameManager.CurrentLevelData;
        }

        txtLevelNo.text = (LevelManager.Intance.CurrentLevelIndex + 1) + "";
        gameOverTitleTxt.text = "Level " + (LevelManager.Intance.CurrentLevelIndex + 1);
        UsedMoves = 0;

        if (currentLevel.gameType == GameType.Equation)
        {
            EquationLevel();
        }
        else
        {
            rows = currentLevel.totalRows;
            cols = currentLevel.totalCols;
            m_GridPoints = new PointClass[cols, rows];

            string matches = currentLevel.TotalMoves > 1 ? "matches" : "match";
            string type = currentLevel.gameType.ToString().ToLower() + (currentLevel.TotalShape > 1 ? "s" : "");
            txtInstructionText.text = currentLevel.gameMode.ToString() + " " + currentLevel.TotalMoves + " " + matches + " to get " + currentLevel.TotalShape + " " + type;

            if (currentLevel.gameType == GameType.Square)
                SquareGrid(rows, cols);
            else
                TriangleGrid(rows, cols);

            if (currentLevel.gameMode == GameMode.Add)
            {
                SpawnChildMatches(currentLevel.TotalMoves);
            }

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
            GetComponent<RectTransform>().localPosition = -center + Vector3.up * 25f;
        }

        // Change the sprites of matches in Add gamemode.
        if (currentLevel.gameMode == GameMode.Add)
        {
            foreach (var match in allMatches)
            {
                if (match.GetChild() != null) match.IsRemovable = false;
            }

            if (currentLevel.gameType == GameType.Equation)
            {
                var equalMatches = GameObject.FindGameObjectsWithTag("EqualMatch");
                foreach (var match in equalMatches)
                {
                    if (match.GetComponent<Matches>().GetChild() != null)
                        match.GetComponent<Matches>().IsRemovable = false;
                }
            }
        }
    }
    #endregion

    #region Equation/Tringle..Ect Level Setup
    void EquationLevel()
    {
        equationData = Instantiate(m_EquationLevelPrefab, transform);
        equationData.transform.localScale = Vector3.one;

        equationData.Num1 = currentLevel.Number1;
        equationData.Num2 = currentLevel.Number2;
        equationData.Num3 = currentLevel.Number3;
        equationData.EquationSign = currentLevel.EquationSign;

        string matches = currentLevel.TotalMoves > 1 ? "matches" : "match";
        txtInstructionText.text = currentLevel.gameMode.ToString() + " " + currentLevel.TotalMoves + " " + matches + " to fix this equation";

        equationData.matches = allMatches;
        equationData.SetupEquation();
        equationData.SetupOprator();
        equationData.SetMaxWidth(gridSize.x * 0.95f);

        if (currentLevel.gameMode == GameMode.Add)
        {
            SpawnChildMatches(currentLevel.TotalMoves);
        }
    }

    void SquareGrid(int rows, int cols)
    {
        cellSize = Mathf.Min(gridSize.x / (cols - 1), gridSize.y / (rows - 1));
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
        cellSize = Mathf.Min(gridSize.x / (cols - 1), gridSize.y / (rows - 1));
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
                    cO.transform.SetParent(transform);
                    cO.posX = x;
                    cO.posY = y;
                    cO.name = "point " + x + "," + y;

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
        Matches m_Matches = Instantiate(m_MatchesPrefab, transform);
        m_Matches.transform.localScale = Vector3.one;

        m_Matches.start = start;
        m_Matches.end = end;

        // 🔥 AUTO-CREATE CHILD MATCH IF MISSING
        if (!isEmpty && m_Matches.childMatch == null)
        {
            m_Matches.childMatch = Instantiate(m_ChildMatchPrefab, m_Matches.transform);
            m_Matches.childMatch.transform.localPosition = Vector3.zero;
        }

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
    #endregion

    #region Spawn Matches

    void SpawnChildMatches(int noOfSpawn)
    {
        for (int i = 0; i < noOfSpawn; i++)
        {
            GameObject m_MatchesFill = Instantiate(m_ChildMatchPrefab, listMatchTransform);
            listNewMatches.Add(m_MatchesFill);

            m_MatchesFill.GetComponent<RectTransform>().rotation = Quaternion.Euler(0f, 0f, 60f);
            m_MatchesFill.transform.localPosition = (listNewMatches.Count - 1) * Vector3.right * 50f;

            var height = m_MatchesFill.GetComponent<Image>().sprite.bounds.size.y * cellSize / m_MatchesFill.GetComponent<Image>().sprite.bounds.size.x;
            m_MatchesFill.GetComponent<RectTransform>().sizeDelta = new Vector2(cellSize, height);

            var scale = (200f / cellSize) * Vector3.one;
            m_MatchesFill.transform.localScale = scale;
        }
    }
    #endregion

    #region OnClick Match

    public void OnClickMatch(Matches _match, bool isCheckTweenRunning = true)
    {
        if (isComplete) return;
        if (!_match.IsRemovable) return;

        if (LeanTween.tweensRunning > 0 && isCheckTweenRunning)
        {
            return;
        }
        if (_match.GetChild() != null)
        {
            if (currentLevel.gameMode == GameMode.Remove && UsedMoves == currentLevel.TotalMoves)
            {
                return;
            }

            if (currentLevel.gameMode == GameMode.Move)
            {
                if (_match.isReserved && GetNumEmptyReservedMatches() == currentLevel.TotalMoves) return;
                _match.SetSprite(Assets.instance.matchNormal);
            }

            listNewMatches.Add(_match.GetChild());

            _match.GetChild().transform.SetParent(listMatchTransform);
            LeanTween.rotateZ(_match.GetChild(), 60f, .2f).setEase(LeanTweenType.easeInOutSine);
            LeanTween.moveLocal(_match.GetChild(), (listNewMatches.Count - 1) * Vector3.right * 50f, .3f);

            var scale = (200f / cellSize) * Vector3.one;
            LeanTween.scale(_match.GetChild().gameObject, scale, 0.2f);
            _match.childMatch = null;


            if (currentLevel.gameMode == GameMode.Remove)
            {
                UsedMoves++;

            }
            else if (currentLevel.gameMode == GameMode.Add)
            {
                UsedMoves--;
            }

            else if (currentLevel.gameMode == GameMode.Move)
            {
                if (_match.isReserved)
                {
                    // change color
                    _match.GetComponent<Image>().color =
                        currentLevel.gameType == GameType.Equation ? matchMovedColorInEquation : matchMovedColor;
                }

                UsedMoves = GetNumEmptyReservedMatches() - listNewMatches.Count;
            }

            CheckIsLevelCompleted();
        }
        else
        {
            if (listNewMatches.Count != 0)
            {

                listNewMatches[listNewMatches.Count - 1].transform.SetParent(_match.gameObject.transform);
                _match.childMatch = listNewMatches[listNewMatches.Count - 1];

                LeanTween.scale(_match.GetChild().gameObject, Vector3.one, 0.2f);
                LeanTween.moveLocal(listNewMatches[listNewMatches.Count - 1], Vector3.zero, .3f).setEase(LeanTweenType.easeInOutSine);
                LeanTween.rotateLocal(listNewMatches[listNewMatches.Count - 1], Vector3.zero, .2f).setEase(LeanTweenType.easeInOutSine);
                listNewMatches.RemoveAt(listNewMatches.Count - 1);

                if (currentLevel.gameMode == GameMode.Remove)
                {
                    UsedMoves--;
                }
                else if (currentLevel.gameMode == GameMode.Add)
                {
                    UsedMoves++;
                }
                else if (currentLevel.TotalMoves == UsedMoves)
                {
                    CheckIsLevelCompleted();
                }
                else if (currentLevel.gameMode == GameMode.Move)
                {
                    _match.SetSprite(_match.isReserved ? Assets.instance.matchNormal : Assets.instance.matchMoved);
                    UsedMoves = GetNumEmptyReservedMatches() - listNewMatches.Count;
                }

                CheckIsLevelCompleted();
            }
        }
        objectiveHandlerScript.UpdateObjectivePopup();
    }
    #endregion


    
    public int GetNumEmptyReservedMatches()
    {
        return allMatches.FindAll(x => x.IsPlaced() == false && x.isReserved).Count;
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

    //so you can see the width and height of the grid on editor
    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, gridSize);
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
        return allMatches.FindAll(x => x.GetChild() != null).Count;
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

    public bool IsEquationRight()
    {
        string number1 = GetDigits(equationData.Number1);
        string number2 = GetDigits(equationData.Number2);
        string number3 = GetDigits(equationData.Number3);
        var sign = equationData.OpratorTransform.GetComponentInChildren<Oprator>().GetOperator();

        if (!int.TryParse(number1, out int num1))
        {
            return false;
        }

        if (!int.TryParse(number2, out int num2))
        {
            return false;
        }

        if (!int.TryParse(number3, out int num3))
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

    public void CheckIsLevelCompleted()
    {
        if (currentLevel.gameType == GameType.Square)
        {
            if (UsedMoves == currentLevel.TotalMoves)
            {
                if (GetTotalMatchInGamePlay() == GetTotalSquareMatches())
                {
                    if (currentLevel.TotalShape == GetTotalSquareShape())
                    {
                        AfterLevelDone();
                        return;
                    }
                }

                GameOver(false);
            }
        }
        else if (currentLevel.gameType == GameType.Triangle)
        {
            if (UsedMoves == currentLevel.TotalMoves)
            {
                if (GetTotalMatchInGamePlay() == GetTotalTrigleMatches())
                {
                    if (currentLevel.TotalShape == GetTotalTriangleShape())
                    {
                        AfterLevelDone();
                        return;
                    }
                }
                GameOver(false);
            }
        }
        else if (currentLevel.gameType == GameType.Equation)
        {
            if (UsedMoves == currentLevel.TotalMoves)
            {
                if (IsEquationRight())
                {
                    AfterLevelDone();
                    return;
                }

                GameOver(false);
            }
        }
    }


    public bool isComplete;
    public void AfterLevelDone()
    {
        if (isComplete) return;
        isComplete = true;

        txtInstructionText.text = "You have completed the level";

        Timer.Schedule(this, 0.6f, () =>
        {
            Debug.Log("Game Win Popup Open");
            afterLevelDonePopUp.Open();
            LevelManager.Intance.SetLevelCompletedInCurrentPack(LevelManager.Intance.CurrentLevelIndex);
        });

        Timer.Schedule(this, 0.8f, () =>
        {
            if (AdmobController.instance != null)
                AdmobController.instance.ShowInterstitial();
        });

        PlayerPrefs.SetInt("played_game", 1);
    }

    public void OnHintClick()
    {
        if (isComplete) return;
        Sound.instance.PlayButton();

        if (GameManager.Hints > 0)
        {
            if (currentLevel.gameMode == GameMode.Add)
            {
                foreach (Matches item in allMatches)
                {
                    if (!item.isReserved && item.GetChild() != null)
                    {
                        OnClickMatch(item, false);
                    }
                }
                OnClickMatch(allMatches[currentLevel.SolvedMatchesIndex[hintCounter]], false);
                allMatches[currentLevel.SolvedMatchesIndex[hintCounter]].isReserved = true;
                allMatches[currentLevel.SolvedMatchesIndex[hintCounter]].IsRemovable = false;
                hintCounter++;
                GameManager.Hints--;
            }
            else if (currentLevel.gameMode == GameMode.Remove)
            {
                foreach (Matches item in allMatches)
                {
                    if (item.isReserved && item.GetChild() == null)
                    {
                        OnClickMatch(item, false);
                    }
                }
                OnClickMatch(allMatches[currentLevel.SolvedMatchesIndex[hintCounter]], false);
                allMatches[currentLevel.SolvedMatchesIndex[hintCounter]].isReserved = true;
                allMatches[currentLevel.SolvedMatchesIndex[hintCounter]].IsRemovable = false;
                hintCounter++;
                GameManager.Hints--;
            }
            else
            {
                foreach (Matches item in allMatches)
                {
                    if (!item.isReserved && item.GetChild() != null)
                    {
                        OnClickMatch(item, false);
                    }
                }
                foreach (Matches item in allMatches)
                {
                    if (item.isReserved && item.GetChild() == null)
                    {
                        OnClickMatch(item, false);
                    }
                }

                //hint for move
                OnClickMatch(allMatches[currentLevel.SolvedMatchesIndex[hintCounter]], false);
                allMatches[currentLevel.SolvedMatchesIndex[hintCounter]].isReserved = true;
                allMatches[currentLevel.SolvedMatchesIndex[hintCounter]].IsRemovable = false;
                hintCounter++;
                OnClickMatch(allMatches[currentLevel.SolvedMatchesIndex[hintCounter]], false);
                allMatches[currentLevel.SolvedMatchesIndex[hintCounter]].isReserved = true;
                allMatches[currentLevel.SolvedMatchesIndex[hintCounter]].IsRemovable = false;
                hintCounter++;

                GameManager.Hints--;
            }
        }
        else
        {
            hintPopup.Open();
        }
    }

    public void OnBackBtnClick()
    {
        GameManager.openLevelSelectionMenu = true;
        ScreenFader.instance.GotoScene("HomeScene");

        Sound.instance.PlayButton();
    }

    public void OnRestartBtnClick()
    {
        ScreenFader.instance.GotoScene("GameScene");
        Sound.instance.PlayButton();
    }

    public void OnRestartBtnClickInGameOver()
    {
        ScreenFader.instance.GotoScene("GameScene");
    }

    public void OnMenuBtnClick()
    {
        GameManager.openLevelSelectionMenu = true;
        ScreenFader.instance.GotoScene("HomeScene");
    }

    public void OnNextBtnClick()
    {
        if (LevelManager.Intance.CurrentLevelPack.TotalLevels > (LevelManager.Intance.CurrentLevelIndex + 1))
        {
            LevelManager.Intance.CurrentLevelIndex++;
            ScreenFader.instance.GotoScene("GameScene");
        }
        else
        {
            GameManager.openLevelPackSelectionMenu = true;
            ScreenFader.instance.GotoScene("HomeScene");
        }
    }

    public void PlayButton()
    {
        Sound.instance.PlayButton();
    }

    public void GameOver(bool isSuccess = false)
    {
        if (isComplete) return;
        isComplete = true;


        if (isSuccess)
        {
            txtInstructionText.text = "You have completed the level!";
        }
        else
        {
            txtInstructionText.text = "No moves left! Try again.";
        }

        Timer.Schedule(this, 0.6f, () =>
        {
            Debug.Log("Game Over Popup Open");
            gameOverPopUp.Open();

            if (isSuccess)
            {
                LevelManager.Intance.SetLevelCompletedInCurrentPack(LevelManager.Intance.CurrentLevelIndex);
            }
        });

        // Optional ad
        Timer.Schedule(this, 0.4f, () =>
        {
            if (AdmobController.instance != null)
                AdmobController.instance.ShowInterstitial();
        });

        PlayerPrefs.SetInt("played_game", 1);
    }

    public void RearrangeInventory()
    {
        for (int i = 0; i < listNewMatches.Count; i++)
        {
            GameObject stick = listNewMatches[i];
            if (!stick) continue;

            // Smooth slot position
            Vector3 target = (i * Vector3.right * 50f);

            LeanTween.moveLocal(stick, target, 0.25f).setEase(LeanTweenType.easeInOutSine);
            LeanTween.rotateLocal(stick, new Vector3(0, 0, 60f), 0.2f);
        }
    }

    public void RemoveMatch(Matches _match)
    {
        if (!_match.IsRemovable) return;

        if (currentLevel.gameMode == GameMode.Remove && UsedMoves == currentLevel.TotalMoves)
            return;

        if (currentLevel.gameMode == GameMode.Move)
        {
            if (_match.isReserved && GetNumEmptyReservedMatches() == currentLevel.TotalMoves)
                return;

            _match.SetSprite(Assets.instance.matchNormal);
        }

        GameObject child = _match.GetChild();
        listNewMatches.Add(child);

        child.transform.SetParent(listMatchTransform);

        LeanTween.rotateZ(child, 60f, .2f).setEase(LeanTweenType.easeInOutSine);
        LeanTween.moveLocal(child, (listNewMatches.Count - 1) * Vector3.right * 50f, .3f);

        var scale = (200f / cellSize) * Vector3.one;
        LeanTween.scale(child, scale, 0.2f);

        _match.childMatch = null;

        if (currentLevel.gameMode == GameMode.Remove)
            UsedMoves++;
        else if (currentLevel.gameMode == GameMode.Add)
            UsedMoves--;
        else if (currentLevel.gameMode == GameMode.Move)
        {
            if (_match.isReserved)
            {
                _match.GetComponent<Image>().color =
                    currentLevel.gameType == GameType.Equation ?
                    matchMovedColorInEquation : matchMovedColor;
            }

            UsedMoves = GetNumEmptyReservedMatches() - listNewMatches.Count;
        }

        CheckIsLevelCompleted();
        objectiveHandlerScript.UpdateObjectivePopup();
    }

    public void AddMatch(Matches _match)
    {
        if (listNewMatches.Count == 0) return;

        GameObject last = listNewMatches[listNewMatches.Count - 1];

        last.transform.SetParent(_match.transform);

        LeanTween.scale(last, Vector3.one, 0.2f);
        LeanTween.moveLocal(last, Vector3.zero, .3f).setEase(LeanTweenType.easeInOutSine);
        LeanTween.rotateLocal(last, Vector3.zero, .2f).setEase(LeanTweenType.easeInOutSine);

        _match.childMatch = last;

        listNewMatches.RemoveAt(listNewMatches.Count - 1);

        if (currentLevel.gameMode == GameMode.Remove)
            UsedMoves--;
        else if (currentLevel.gameMode == GameMode.Add)
            UsedMoves++;
        else if (currentLevel.gameMode == GameMode.Move)
        {
            _match.SetSprite(_match.isReserved ? Assets.instance.matchNormal : Assets.instance.matchMoved);
            UsedMoves = GetNumEmptyReservedMatches() - listNewMatches.Count;
        }

        CheckIsLevelCompleted();
        objectiveHandlerScript.UpdateObjectivePopup();
    }

}