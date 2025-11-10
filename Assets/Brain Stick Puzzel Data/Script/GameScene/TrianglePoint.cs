using System.Collections.Generic;
using UnityEngine;

public class TrianglePoint : PointClass
{
    public TrianglePoint Left, Right, LeftUp, LeftDown, RightUp, RightDown;
    private Matches _LeftMatches, _RightMatches, _LeftUpMatches, _LeftDownMatches, _RightUpMatches, _RightDownMatches;

    public Matches LeftMatches
    {
        get { return _LeftMatches; }
        set
        {
            _LeftMatches = value;
            if (Left != null && Left.RightMatches != value)
                Left.RightMatches = value;
        }
    }

    public Matches RightMatches
    {
        get { return _RightMatches; }
        set
        {
            _RightMatches = value;
            if (Right != null && Right.LeftMatches != value)
                Right.LeftMatches = value;
        }
    }

    public Matches RightUpMatches
    {
        get { return _RightUpMatches; }
        set
        {
            _RightUpMatches = value;
            if (RightUp != null && RightUp.LeftDownMatches != value)
                RightUp.LeftDownMatches = value;
        }
    }

    public Matches LeftUpMatches
    {
        get { return _LeftUpMatches; }
        set
        {
            _LeftUpMatches = value;
            if (LeftUp != null && LeftUp.RightDownMatches != value)
                LeftUp.RightDownMatches = value;
        }
    }

    public Matches LeftDownMatches
    {
        get { return _LeftDownMatches; }
        set
        {
            _LeftDownMatches = value;
            if (LeftDown != null && LeftDown.RightUpMatches != value)
                LeftDown.RightUpMatches = value;
        }
    }

    public Matches RightDownMatches
    {
        get { return _RightDownMatches; }
        set
        {
            _RightDownMatches = value;
            if (RightDown != null && RightDown.LeftUpMatches != value)
                RightDown.LeftUpMatches = value;
        }
    }

    void Update()
    {
        if (Left != null)
        {
            Debug.DrawLine(transform.position, Left.transform.position);
        }
        if (Right != null)
        {
            Debug.DrawLine(transform.position, Right.transform.position);
        }
        if (LeftUp != null)
        {
            Debug.DrawLine(transform.position, LeftUp.transform.position);
        }
        if (RightUp != null)
        {
            Debug.DrawLine(transform.position, RightUp.transform.position);
        }
        if (RightDown != null)
        {
            Debug.DrawLine(transform.position, RightDown.transform.position);
        }
        if (LeftDown != null)
        {
            Debug.DrawLine(transform.position, LeftDown.transform.position);
        }
    }

    public bool CheckStepRight(int step)
    {
        bool hasMatch = RightMatches != null && RightMatches.IsPlaced();
        if (hasMatch && step > 1) return Right.CheckStepRight(step - 1);
        else return hasMatch;
    }

    public bool CheckStepLeft(int step)
    {
        bool hasMatch = LeftMatches != null && LeftMatches.IsPlaced();
        if (hasMatch && step > 1) return Left.CheckStepLeft(step - 1);
        else return hasMatch;
    }

    public bool CheckStepRightUp(int step)
    {
        bool hasMatch = RightUpMatches != null && RightUpMatches.IsPlaced();
        if (hasMatch && step > 1) return RightUp.CheckStepRightUp(step - 1);
        else return hasMatch;
    }

    public bool CheckStepLeftUp(int step)
    {
        bool hasMatch = LeftUpMatches != null && LeftUpMatches.IsPlaced();
        if (hasMatch && step > 1) return LeftUp.CheckStepLeftUp(step - 1);
        else return hasMatch;
    }

    public bool CheckStepRightDown(int step)
    {
        bool hasMatch = RightDownMatches != null && RightDownMatches.IsPlaced();
        if (hasMatch && step > 1) return RightDown.CheckStepRightDown(step - 1);
        else return hasMatch;
    }

    public bool CheckStepLeftDown(int step)
    {
        bool hasMatch = LeftDownMatches != null && LeftDownMatches.IsPlaced();
        if (hasMatch && step > 1) return LeftDown.CheckStepLeftDown(step - 1);
        else return hasMatch;
    }

    public TrianglePoint GetStepRight(int step)
    {
        return step > 1 ? Right.GetStepRight(step - 1) : Right;
    }

    public TrianglePoint GetStepLeft(int step)
    {
        return step > 1 ? Left.GetStepLeft(step - 1) : Left;
    }

    public TrianglePoint GetStepRightUp(int step)
    {
        return step > 1 ? RightUp.GetStepRightUp(step - 1) : RightUp;
    }

    public TrianglePoint GetStepLeftUp(int step)
    {
        return step > 1 ? LeftUp.GetStepLeftUp(step - 1) : LeftUp;
    }

    public TrianglePoint GetStepRightDown(int step)
    {
        return step > 1 ? RightDown.GetStepRightDown(step - 1) : RightDown;
    }

    public TrianglePoint GetStepLeftDown(int step)
    {
        return step > 1 ? LeftDown.GetStepLeftDown(step - 1) : LeftDown;
    }

    public List<Matches> GetMatchesRight(int step)
    {
        var matches = new List<Matches>();
        var current = this;
        for (int i = 0; i < step; i++)
        {
            matches.Add(current.RightMatches);
            current = current.Right;
        }
        return matches;
    }

    public List<Matches> GetMatchesLeft(int step)
    {
        var matches = new List<Matches>();
        var current = this;
        for (int i = 0; i < step; i++)
        {
            matches.Add(current.LeftMatches);
            current = current.Left;
        }
        return matches;
    }

    public List<Matches> GetMatchesRightUp(int step)
    {
        var matches = new List<Matches>();
        var current = this;
        for (int i = 0; i < step; i++)
        {
            matches.Add(current.RightUpMatches);
            current = current.RightUp;
        }
        return matches;
    }

    public List<Matches> GetMatchesLeftUp(int step)
    {
        var matches = new List<Matches>();
        var current = this;
        for (int i = 0; i < step; i++)
        {
            matches.Add(current.LeftUpMatches);
            current = current.LeftUp;
        }
        return matches;
    }

    public List<Matches> GetMatchesRightDown(int step)
    {
        var matches = new List<Matches>();
        var current = this;
        for (int i = 0; i < step; i++)
        {
            matches.Add(current.RightDownMatches);
            current = current.RightDown;
        }
        return matches;
    }

    public List<Matches> GetMatchesLeftDown(int step)
    {
        var matches = new List<Matches>();
        var current = this;
        for (int i = 0; i < step; i++)
        {
            matches.Add(current.LeftDownMatches);
            current = current.LeftDown;
        }
        return matches;
    }
}
