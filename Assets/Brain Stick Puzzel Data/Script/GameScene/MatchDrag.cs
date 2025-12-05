using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class MatchDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Matches match;

    private RectTransform draggedChild;
    private Transform originalParent;
    private Canvas canvas;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        match = GetComponentInParent<Matches>();

        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    //──────────────────────────────────────────────
    // BEGIN DRAG
    //──────────────────────────────────────────────
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (MatchesGrid.Instance == null) return;
        if (!match.IsRemovable) return;

        GameObject childObj = match.GetChild();
        if (childObj == null) return;   // SLOT EMPTY → cannot drag

        // Real child we are dragging
        draggedChild = childObj.GetComponent<RectTransform>();
        originalParent = draggedChild.parent;

        // Remove from slot and add into inventory
        MatchesGrid.Instance.RemoveMatch(match);

        // Now move to canvas for dragging
        draggedChild.SetParent(canvas.transform, true);

        canvasGroup.blocksRaycasts = false;
    }

    //──────────────────────────────────────────────
    // DRAG
    //──────────────────────────────────────────────
    public void OnDrag(PointerEventData eventData)
    {
        if (draggedChild != null)
            draggedChild.position = eventData.position;
    }

    //──────────────────────────────────────────────
    // END DRAG
    //──────────────────────────────────────────────
    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        Matches target = RaycastMatch(eventData);

        // CASE 1: Dropped on valid empty slot
        if (target != null && target.GetChild() == null)
        {
            MatchesGrid.Instance.AddMatch(target);
        }
        else
        {
            // Case 2: Go back to inventory
            ReturnToInventory();
        }

        MatchesGrid.Instance.RearrangeInventory();
        MatchesGrid.Instance.objectiveHandlerScript.UpdateObjectivePopup();

        draggedChild = null;
        originalParent = null;
    }

    //──────────────────────────────────────────────
    // RAYCAST SLOT
    //──────────────────────────────────────────────
    private Matches RaycastMatch(PointerEventData eventData)
    {
        List<RaycastResult> hits = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, hits);

        foreach (var hit in hits)
        {
            Matches m = hit.gameObject.GetComponent<Matches>();
            if (m != null) return m;
        }
        return null;
    }

    //──────────────────────────────────────────────
    // RETURN TO INVENTORY
    //──────────────────────────────────────────────
    private void ReturnToInventory()
    {
        draggedChild.SetParent(MatchesGrid.Instance.listMatchTransform);
    }
}
