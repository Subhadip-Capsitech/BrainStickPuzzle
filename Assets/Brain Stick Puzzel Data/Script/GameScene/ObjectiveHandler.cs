using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveHandler : MonoBehaviour
{
    public Text txtUseAllMoves;
    public GameObject MovesRightImage, MovesWrongImage;
    public Text txtUseAllMatchMustPart;
    public GameObject AllMatchMustPartRightImage, AllMatchMustPartWrongImage;
    public Text txtGoaltext;
    public Text youHave;
    public GameObject MatchePartObject;
    public GameObject GoalRightImage, GoalWrongImage;

    void Start()
    {
        Invoke("UpdateObjectivePopup", 0.2f);
    }

    public void UpdateObjectivePopup()
    {
        UpdatePartOfMatches();
        UpdateShapeDetail();
        UpdateUseMoves();
    }

    void UpdateUseMoves()
    {
        txtUseAllMoves.text = "Use all moves";
        bool ok = MatchesGrid.Instance.UsedMoves == GameManager.CurrentLevelData.TotalMoves;
        MovesRightImage.SetActive(ok);
        MovesWrongImage.SetActive(!ok);
    }

    void UpdatePartOfMatches()
    {
        if (MatchesGrid.Instance.currentLevel.gameType == GameType.Square)
        {
            txtUseAllMatchMustPart.text = "All matches must be part of squares";
            bool ok = MatchesGrid.Instance.GetTotalMatchInGamePlay() == MatchesGrid.Instance.GetTotalSquareMatches();
            AllMatchMustPartRightImage.SetActive(ok);
            AllMatchMustPartWrongImage.SetActive(!ok);
        }
        else if (MatchesGrid.Instance.currentLevel.gameType == GameType.Triangle)
        {
            txtUseAllMatchMustPart.text = "All matches must be part of triangle";
            bool ok = MatchesGrid.Instance.GetTotalMatchInGamePlay() == MatchesGrid.Instance.GetTotalTrigleMatches();
            AllMatchMustPartRightImage.SetActive(ok);
            AllMatchMustPartWrongImage.SetActive(!ok);
        }

    }

    // Correct the equation
    void UpdateShapeDetail()
    {
        if (GameManager.CurrentLevelData.gameType == GameType.Square)
        {
            txtGoaltext.text = "Make " + GameManager.CurrentLevelData.TotalShape + " squares";
            youHave.text = "(you have " + MatchesGrid.Instance.GetTotalSquareShape() + ")";
            bool ok = GameManager.CurrentLevelData.TotalShape == MatchesGrid.Instance.GetTotalSquareShape();
            GoalRightImage.SetActive(ok);
            GoalWrongImage.SetActive(!ok);
        }
        else if (GameManager.CurrentLevelData.gameType == GameType.Triangle)
        {
            txtGoaltext.text = "Make " + GameManager.CurrentLevelData.TotalShape + " triangle";
            youHave.text = "(you have " + MatchesGrid.Instance.GetTotalTriangleShape() + ")";
            bool ok = GameManager.CurrentLevelData.TotalShape == MatchesGrid.Instance.GetTotalTriangleShape();
            GoalRightImage.SetActive(ok);
            GoalWrongImage.SetActive(!ok);
        }
        else
        {
            MatchePartObject.SetActive(false);
            txtGoaltext.text = "Correct the equation";
            bool ok = MatchesGrid.Instance.IsEquationRight();
            GoalRightImage.SetActive(ok);
            GoalWrongImage.SetActive(!ok);
        }
    }
}
