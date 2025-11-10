using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MatchesStatus : MonoBehaviour, IPointerClickHandler
{
    public Image childMatch;
    public bool isDelete = false;
    public bool isEmpty = false;
    public bool isHintAddMatch = false;
    public bool isHintRemoveMatch = false;
    public Color isHintAddColor, isHintRemoveColor, isNoneHintColor;

    public void OnClick()
    {
        if (GetComponent<Matches>().IsRemovable)
            LevelDesigner.instance.OnMatchClick(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
            LevelDesigner.instance.OnMatchRightClick(this);

        else if (eventData.button == PointerEventData.InputButton.Middle)
            LevelDesigner.instance.OnMatchMiddleClick(this);
    }
}