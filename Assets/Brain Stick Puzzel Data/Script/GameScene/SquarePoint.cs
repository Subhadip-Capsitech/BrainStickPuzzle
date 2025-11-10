using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquarePoint : PointClass
{
    public SquarePoint Up, Down, Left, Right;
    private Matches _LeftMatches, _RightMatches, _UpMatches, _DownMatches;

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

    public Matches UpMatches
    {
        get { return _UpMatches; }
        set
        {
            _UpMatches = value;
            if (Up != null && Up.DownMatches != value)
                Up.DownMatches = value;
        }
    }

    public Matches DownMatches
    {
        get { return _DownMatches; }
        set
        {
            _DownMatches = value;
            if (Down != null && Down.UpMatches != value)
                Down.UpMatches = value;
        }
    }

    public bool CheckStepRight(int step)
    {
        bool isPlaced = RightMatches != null && RightMatches.IsPlaced();
        if (isPlaced && step > 1) return Right.CheckStepRight(step - 1);
        else return isPlaced; 
    }

    public bool CheckStepLeft(int step)
    {
        bool isPlaced = LeftMatches != null && LeftMatches.IsPlaced();
        if (isPlaced && step > 1) return Left.CheckStepLeft(step - 1);
        else return isPlaced;
    }

    public bool CheckStepUp(int step)
    {
        bool isPlaced = UpMatches != null && UpMatches.IsPlaced();
        if (isPlaced && step > 1) return Up.CheckStepUp(step - 1);
        else return isPlaced;
    }

    public bool CheckStepDown(int step)
    {
        bool isPlaced = DownMatches != null && DownMatches.IsPlaced();
        if (isPlaced && step > 1) return Down.CheckStepDown(step - 1);
        else return isPlaced;
    }

    public SquarePoint GetStepRight(int step)
    {
        return step > 1 ? Right.GetStepRight(step - 1) : Right;
    }

    public SquarePoint GetStepLeft(int step)
    {
        return step > 1 ? Left.GetStepLeft(step - 1) : Left;
    }

    public SquarePoint GetStepUp(int step)
    {
        return step > 1 ? Up.GetStepUp(step - 1) : Up;
    }

    public SquarePoint GetStepDown(int step)
    {
        return step > 1 ? Down.GetStepDown(step - 1) : Down;
    }

    public List<Matches> GetRightMatches(int step)
    {
        var matches = new List<Matches>();
        var current = this;
        for(int i = 0; i < step; i++)
        {
            matches.Add(current.RightMatches);
            current = current.Right;
        }
        return matches;
    }

    public List<Matches> GetLeftMatches(int step)
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

    public List<Matches> GetUpMatches(int step)
    {
        var matches = new List<Matches>();
        var current = this;
        for (int i = 0; i < step; i++)
        {
            matches.Add(current.UpMatches);
            current = current.Up;
        }
        return matches;
    }

    public List<Matches> GetDownMatches(int step)
    {
        var matches = new List<Matches>();
        var current = this;
        for (int i = 0; i < step; i++)
        {
            matches.Add(current.DownMatches);
            current = current.Down;
        }
        return matches;
    }
}
