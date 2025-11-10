using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointClass : MonoBehaviour
{
	public int posX, posY;
    public bool inLevelEditor = false;

    public bool HasMatch(Matches match)
    {
        if (match == null) return false;

        if (inLevelEditor)
        {
            var matchStatus = match.GetComponent<MatchesStatus>();
            bool isHintAdd = matchStatus.isHintAddMatch;
            bool isHintRemove = matchStatus.isHintRemoveMatch;
            if (isHintAdd) return true;
            if (isHintRemove) return false;
            if (matchStatus.isDelete || matchStatus.isEmpty) return false;
        }

        return match.GetChild() != null;
    }
}
