using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(Image))]
public class Matches : MonoBehaviour
{
    [HideInInspector]
    public PointClass start, end;
    public GameObject childMatch;

    public bool _isRemoveable = true;
    public bool IsRemovable
    {
        set { _isRemoveable = value; UpdateUI(); }
        get {return _isRemoveable; }
    }
    public bool isReserved = true;
    public bool inLevelEditor = false;

    public void Load(bool isEmpty = false)
    {
        RectTransform matchesRect = GetComponent<RectTransform>();
        matchesRect.position = start.GetComponent<RectTransform>().position;

        // Set angle
        float angle = 0;
        Vector3 dir = end.GetComponent<RectTransform>().position - start.GetComponent<RectTransform>().position;
        dir = end.transform.InverseTransformDirection(dir);
        angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        matchesRect.rotation = Quaternion.Euler(0, 0, angle);

        //Set size and position
        float width = Mathf.Abs(Vector2.Distance(start.GetComponent<RectTransform>().anchoredPosition, end.GetComponent<RectTransform>().anchoredPosition));
        float height = 60;
        matchesRect.anchoredPosition = start.GetComponent<RectTransform>().anchoredPosition;
        if (GetComponent<Image>().sprite != null)
        {
            height = (GetComponent<Image>().sprite.bounds.size.y * width) / GetComponent<Image>().sprite.bounds.size.x;
        }
        matchesRect.sizeDelta = new Vector2(width, height);

        if (isEmpty)
        {
            DestroyChild();
        }
        else
        {
            childMatch.GetComponent<RectTransform>().sizeDelta = matchesRect.sizeDelta;
        }
        name = start.name + "  -->  " + end.name;
    }

    private void UpdateUI()
    {
        if (inLevelEditor) return;
        if (MatchesGrid.Instance == null) return;

        if (childMatch != null)
        {
           SetSprite(IsRemovable ? Assets.instance.matchNormal : Assets.instance.matchUnremovable);
        }
    }

    public void SetSprite(Sprite sprite)
    {
        childMatch.GetComponent<Image>().sprite = sprite;
    }

    public void DestroyChild()
    {
        Destroy(childMatch);
        childMatch = null;
        isReserved = false;
    }

    public GameObject GetChild()
    {
        return childMatch;
    }

    public bool IsPlaced()
    {
        if (inLevelEditor)
        {
            var matchStatus = GetComponent<MatchesStatus>();
            bool isHintAdd = matchStatus.isHintAddMatch;
            bool isHintRemove = matchStatus.isHintRemoveMatch;
            if (isHintAdd) return true;
            if (isHintRemove) return false;
            if (matchStatus.isDelete || matchStatus.isEmpty) return false;
        }

        return childMatch != null;
    }

    public void OnClickMatch()
    {
        MatchesGrid.Instance.OnClickMatch(this);
    }
}
