using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MS;
using System.Text;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class LevelDesigner : MonoBehaviour
{

    public GameObject levelBase;
    public Button newBtn, okBtn, saveBtn;
    public Dropdown DD_GameMode, DD_GameType, DD_Oprator;
    public InputField IF_Move, IF_Num1, IF_Num2, IF_Num3, IF_Shape;
    public Slider S_Row, S_Col;
    public Text L_Row, L_Col;
    public GameObject ShapePanel, EquationPanel;
    public Text messageText;


    public LevelDesignerGrid grid = null;
    public LevelDesignerGrid _prefabGrid;
    public Transform rootCanvas;
    public static LevelDesigner instance;
    public Popup matchSettingPopup;
    public MatchesStatus currentMS;
    public Toggle isDelete, isEmpty, isAddHint, isRemoveHint;

    public List<int> lstAddHint = new List<int>();
    public List<int> lstRemoveHint = new List<int>();

    void Start()
    {
        instance = this;
        SetupUIListner();
    }

    void SetupUIListner()
    {
        DD_GameType.onValueChanged.AddListener((arg0) =>
        {
            if (arg0 == 0)
            {
                ShapePanel.SetActive(false);
                EquationPanel.SetActive(true);
            }
            else
            {
                ShapePanel.SetActive(true);
                EquationPanel.SetActive(false);
                S_Row.value = 4;
                S_Col.value = 4;
            }
        });

        DD_GameMode.onValueChanged.AddListener(OnGameModeChanged);

        S_Row.onValueChanged.AddListener((arg0) =>
        {
            L_Row.text = "Row  : " + arg0;
        });

        S_Col.onValueChanged.AddListener((arg0) =>
        {
            L_Col.text = "Col  : " + arg0;
        });

        levelBase.SetActive(false);

        isDelete.onValueChanged.AddListener((obj =>
        {
            if (obj) isEmpty.isOn = false;
            UpdateOptions();
        }));

        isEmpty.onValueChanged.AddListener((obj =>
        {
            if (obj) isDelete.isOn = false;
            UpdateOptions();
        }));

        isAddHint.onValueChanged.AddListener((obj =>
        {
            if (obj && DD_GameType.value != (int)GameType.Equation) UpdateOptions();
        }));

        isRemoveHint.onValueChanged.AddListener((obj =>
        {
            if (obj && DD_GameType.value != (int)GameType.Equation) UpdateOptions();
        }));
    }

    private void UpdateOptions()
    {
        if (!isEmpty.isOn) isAddHint.isOn = false;
        if (isEmpty.isOn || isDelete.isOn) isRemoveHint.isOn = false;
    }

    private void OnGameModeChanged(int mode)
    {
        var allMatches = LevelDesignerGrid.Instance.allMatches;
        foreach(var match in allMatches)
        {
            var matchStatus = match.GetComponent<MatchesStatus>();
            if (matchStatus != null)
            {
                matchStatus.isHintAddMatch = false;
                matchStatus.isHintRemoveMatch = false;

                UpdateMatchUI(matchStatus);
                UpdateUI();
            }
        }
    }

    public void OnNewBtn()
    {
        if (grid != null)
        {
            Destroy(grid.gameObject);
        }

        grid = Instantiate(_prefabGrid, rootCanvas);
        grid.transform.localScale = Vector3.one;

        newBtn.gameObject.SetActive(false);
        saveBtn.gameObject.SetActive(false);
        levelBase.SetActive(true);
        okBtn.gameObject.SetActive(true);
        DD_GameMode.interactable = true;
        DD_GameType.interactable = true;
        DD_Oprator.interactable = true;
        IF_Num1.interactable = true;
        IF_Num2.interactable = true;
        IF_Num3.interactable = true;

        S_Row.interactable = true;
        S_Col.interactable = true;

        lstAddHint.Clear();
        lstRemoveHint.Clear();

        IF_Move.text = "0";
        IF_Shape.text = "0";
        messageText.gameObject.SetActive(true);
        messageText.text = "";
    }

    public void OnOKBtn()
    {
        if (DD_GameType.value == (int)GameType.Equation)
        {
            if (IF_Num1.text == "" || IF_Num2.text == "" || IF_Num3.text == "") return;
            if (int.Parse(IF_Num1.text) < 0) return;
            if (int.Parse(IF_Num2.text) < 0) return;
            if (int.Parse(IF_Num3.text) < 0) return;
        }

        newBtn.gameObject.SetActive(true);
        saveBtn.gameObject.SetActive(true);

        okBtn.gameObject.SetActive(false);
        DD_GameType.interactable = false;

        S_Row.interactable = false;
        S_Col.interactable = false;
        SetUpLevel();

        Timer.Schedule(this, 0.2f, UpdateUI);
    }

    public void OnSaveBtn()
    {
        string warning = CanSave();
        if (warning != null)
        {
            ClearLog();
            Debug.LogWarning(warning);
            return;
        }

#if UNITY_EDITOR

        string path = EditorUtility.SaveFilePanel("Save Level", "Assets/MyData/Resources/Levels", "0", "asset");
        if (path != null && path != "")
        {
            path = "Assets" + path.Replace(Application.dataPath, "");

            var currentLevel = LevelDesignerGrid.Instance.currentLevel;
            var matches = currentLevel.matches = new List<MatchesData>();
            matches.AddRange(LevelDesignerGrid.Instance.matchesDataList);
            currentLevel.SolvedMatchesIndex.Clear();
            lstAddHint.Clear();
            lstRemoveHint.Clear();

            var allMatches = LevelDesignerGrid.Instance.allMatches;

            if (DD_GameType.value != (int)GameType.Equation)
            {
                for (int i = 0; i < matches.Count; i++)
                {
                    var matchStatus = allMatches[i].GetComponent<MatchesStatus>();
                    matches[i].isEmpty = matchStatus.isEmpty;

                    if (DD_GameMode.value == (int)GameMode.Add)
                    {
                        if (matchStatus.isHintAddMatch)
                        {
                            currentLevel.SolvedMatchesIndex.Add(i);
                        }
                    }
                    if (DD_GameMode.value == (int)GameMode.Remove)
                    {
                        if (matchStatus.isHintRemoveMatch)
                        {
                            currentLevel.SolvedMatchesIndex.Add(i);
                        }
                    }

                    if (DD_GameMode.value == (int)GameMode.Move)
                    {
                        if (matchStatus.isHintRemoveMatch)
                        {
                            lstRemoveHint.Add(i);
                        }
                        if (matchStatus.isHintAddMatch)
                        {
                            lstAddHint.Add(i);
                        }
                    }
                }

                if (DD_GameMode.value == (int)GameMode.Move)
                {
                    for (int i = 0; i < lstAddHint.Count; i++)
                    {
                        currentLevel.SolvedMatchesIndex.Add(lstRemoveHint[i]);
                        currentLevel.SolvedMatchesIndex.Add(lstAddHint[i]);
                    }
                }

                //Delete matches
                for (int i = matches.Count - 1; i >= 0; i--)
                {
                    if (allMatches[i].GetComponent<MatchesStatus>().isDelete)
                    {
                        currentLevel.matches.RemoveAt(i);

                        for(int j =0; j < currentLevel.SolvedMatchesIndex.Count; j++)
                        {
                            if (currentLevel.SolvedMatchesIndex[j] > i)
                            {
                                currentLevel.SolvedMatchesIndex[j]--;
                            }
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < allMatches.Count; i++)
                {
                    var matchStatus = allMatches[i].GetComponent<MatchesStatus>();
                    if (DD_GameMode.value == (int)GameMode.Add)
                    {
                        if (matchStatus.isHintAddMatch)
                        {
                            currentLevel.SolvedMatchesIndex.Add(i);
                        }
                    }
                    if (DD_GameMode.value == (int)GameMode.Remove)
                    {
                        if (matchStatus.isHintRemoveMatch)
                        {
                            currentLevel.SolvedMatchesIndex.Add(i);
                        }
                    }

                    if (DD_GameMode.value == (int)GameMode.Move)
                    {
                        if (matchStatus.isHintRemoveMatch)
                        {
                            lstRemoveHint.Add(i);
                        }
                        if (matchStatus.isHintAddMatch)
                        {
                            lstAddHint.Add(i);
                        }
                    }
                }

                if (DD_GameMode.value == (int)GameMode.Move)
                {
                    for (int i = 0; i < lstAddHint.Count; i++)
                    {
                        currentLevel.SolvedMatchesIndex.Add(lstRemoveHint[i]);
                        currentLevel.SolvedMatchesIndex.Add(lstAddHint[i]);
                    }
                }
            }

            grid.currentLevel.gameType = (GameType)DD_GameType.value;
            currentLevel.gameMode = (GameMode)DD_GameMode.value;
            currentLevel.TotalMoves = int.Parse(IF_Move.text);
            currentLevel.TotalShape = int.Parse(IF_Shape.text);

            if ((GameType)DD_GameType.value == GameType.Equation)
            {
                currentLevel.totalRows = 0;
                currentLevel.totalCols = 0;
                currentLevel.TotalShape = 0;
            }
            else
            {
                currentLevel.Number1 = "0";
                currentLevel.Number2 = "0";
                currentLevel.Number3 = "0";
                currentLevel.EquationSign = EquationSign.noSign;
            }

            CreateOrReplaceAsset(currentLevel, path);

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = currentLevel;
        }
#endif
    }

    private T CreateOrReplaceAsset<T>(T asset, string path) where T : ScriptableObject
    {
#if UNITY_EDITOR
        T existingAsset = AssetDatabase.LoadAssetAtPath<T>(path);

        if (existingAsset == null)
        {
            AssetDatabase.CreateAsset(asset, path);
            existingAsset = asset;
        }
        else
        {
            EditorUtility.CopySerialized(asset, existingAsset);
        }

        return existingAsset;
#else
        return null;
#endif
    }

    public void SetUpLevel()
    {
        grid.currentLevel = ScriptableObject.CreateInstance("LevelData") as LevelData;
        grid.currentLevel.gameType = (GameType)DD_GameType.value;
        grid.currentLevel.gameMode = (GameMode)DD_GameMode.value;
        grid.currentLevel.EquationSign = (EquationSign)DD_Oprator.value;
        grid.currentLevel.TotalMoves = int.Parse(IF_Move.text);
        grid.currentLevel.Number1 = IF_Num1.text;
        grid.currentLevel.Number2 = IF_Num2.text;
        grid.currentLevel.Number3 = IF_Num3.text;

        grid.currentLevel.TotalShape = int.Parse(IF_Shape.text);
        grid.currentLevel.totalRows = (int)S_Row.value;
        grid.currentLevel.totalCols = (int)S_Col.value;
        grid.SetupLevel();
    }

    private void LoadDialog(MatchesStatus m)
    {
        currentMS = m;

        isDelete.isOn = currentMS.isDelete;
        isEmpty.isOn = currentMS.isEmpty;
        isAddHint.isOn = currentMS.isHintAddMatch;
        isRemoveHint.isOn = currentMS.isHintRemoveMatch;

        isDelete.gameObject.SetActive(true);
        isEmpty.gameObject.SetActive(DD_GameType.value != 0);
        isAddHint.gameObject.SetActive(DD_GameMode.value != 1);
        isRemoveHint.gameObject.SetActive(DD_GameMode.value != 2);

        if (DD_GameType.value == (int)GameType.Equation)
        {
            isDelete.gameObject.SetActive(false);

            var match = m.GetComponent<Matches>();
            if (match.GetChild() == null) isRemoveHint.gameObject.SetActive(false);
            else isAddHint.gameObject.SetActive(false);
        }
    }

    public void OnMatchClick(MatchesStatus m)
    {
        LoadDialog(m);

        if (isDelete.gameObject.activeSelf ||
            isEmpty.gameObject.activeSelf ||
            isAddHint.gameObject.activeSelf ||
            isRemoveHint.gameObject.activeSelf)
        {
            matchSettingPopup.Open();
        }
    }

    public void OnMatchRightClick(MatchesStatus m)
    {
        LoadDialog(m);
        if (isDelete.gameObject.activeSelf)
        {
            isDelete.isOn = !isDelete.isOn;
            SaveToMatchStatus();
            UpdateUI();
        }
    }

    public void OnMatchMiddleClick(MatchesStatus m)
    {
        LoadDialog(m);
        if (isEmpty.gameObject.activeSelf)
        {
            isEmpty.isOn = !isEmpty.isOn;
            SaveToMatchStatus();
            UpdateUI();
        }
    }

    private void SaveToMatchStatus()
    {
        currentMS.isDelete = isDelete.isOn;
        currentMS.isEmpty = isEmpty.isOn;
        currentMS.isHintAddMatch = isAddHint.isOn;
        currentMS.isHintRemoveMatch = isRemoveHint.isOn;

        UpdateMatchUI(currentMS);
    }

    private void UpdateMatchUI(MatchesStatus matchStatus)
    {
        if (matchStatus.isDelete)
        {
            matchStatus.childMatch.color = Color.clear;
            matchStatus.GetComponent<Image>().color = Color.clear;
        }
        else if (matchStatus.isEmpty)
        {
            matchStatus.childMatch.color = Color.clear;
        }
        else
        {
            matchStatus.childMatch.color = Color.white;
        }

        if (matchStatus.isHintAddMatch)
        {
            matchStatus.GetComponent<Image>().color = matchStatus.isHintAddColor;
        }
        else if (matchStatus.isHintRemoveMatch)
        {
            matchStatus.childMatch.color = matchStatus.isHintRemoveColor;
        }
        else if (!matchStatus.isDelete)
        {
            matchStatus.GetComponent<Image>().color = matchStatus.isNoneHintColor;
        }
    }

    private bool hasIssue;
    private void PrintMessage()
    {
        hasIssue = false;
        StringBuilder sb = new StringBuilder();
        if (DD_GameType.value == (int)GameType.Square)
        {
            sb.Append("All matches must be part of squares: ");
            bool ok = grid.GetTotalMatchInGamePlay() == grid.GetTotalSquareMatches();
            sb.Append(ok ? "Yes" : "No");
            if (!ok) hasIssue = true;
        }
        else if (DD_GameType.value == (int)GameType.Triangle)
        {
            sb.Append("All matches must be part of triangles: ");
            bool ok = grid.GetTotalMatchInGamePlay() == grid.GetTotalTrigleMatches();
            sb.Append(ok ? "Yes" : "No");
            if (!ok) hasIssue = true;
        }
        else if (DD_GameType.value == (int)GameType.Equation)
        {
            bool ok = grid.IsEquationRight();
            sb.Append(grid.GetEquationContent() + " : " + (ok ? "Yes" : "No"));
            if (!ok) hasIssue = true;
        }

        if (DD_GameMode.value == (int)GameMode.Move)
        {
            sb.AppendLine();
            sb.Append("Total Add == Total Remove: " + (totalAdd == totalRemove ? "Yes" : "No"));
            if (totalAdd != totalRemove) hasIssue = true;
        }

        messageText.text = sb.ToString();
    }

    int totalRemove = 0, totalAdd = 0;
    private void UpdateLevelTarget()
    {
        if (DD_GameType.value == (int)GameType.Square)
        {
            IF_Shape.text = grid.GetTotalSquareShape().ToString();
        }
        else if (DD_GameType.value == (int)GameType.Triangle)
        {
            IF_Shape.text = grid.GetTotalTriangleShape().ToString();
        }

        var allMatches = LevelDesignerGrid.Instance.allMatches;
        totalRemove = totalAdd = 0;

        foreach (var match in allMatches)
        {
            var matchStatus = match.GetComponent<MatchesStatus>();
            if (matchStatus.isHintAddMatch) totalAdd++;
            else if (matchStatus.isHintRemoveMatch) totalRemove++;
        }

        int totalMove = DD_GameMode.value == (int)GameMode.Add || DD_GameMode.value == (int)GameMode.Move ? totalAdd : totalRemove;
        IF_Move.text = totalMove.ToString();
    }

    private string CanSave()
    {
        if (IF_Move.text == "0")
        {
            return "Total Move = 0 ??" + "\nYou need to do something first";
        }
        else if (hasIssue)
        {
            return "Please look at the message on the screen.\nThen fix it";
        }

        return null;
    }

    public void ClearLog()
    {
#if UNITY_EDITOR
        var assembly = Assembly.GetAssembly(typeof(Editor));
        var type = assembly.GetType("UnityEditor.LogEntries");
        var method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
#endif
    }

    private void UpdateUI()
    {
        UpdateLevelTarget();
        PrintMessage();
    }

    public void OnPopupOk()
    {
        SaveToMatchStatus();
        UpdateUI();

        currentMS = null;
        matchSettingPopup.Close();
    }
}
