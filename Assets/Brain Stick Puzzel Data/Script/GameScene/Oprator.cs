using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oprator : MonoBehaviour
{
    public Matches Horizontal, Vertical, LeftCross, RightCross;

    [HideInInspector]
    public List<Matches> matches;

    public void SetOperator(EquationSign sign)
    {
        if (sign == EquationSign.Plus)
        {
            Destroy(LeftCross.gameObject);
            Destroy(RightCross.gameObject);

            matches.Add(Horizontal);
            matches.Add(Vertical);
        }
        else if (sign == EquationSign.Minus)
        {
            Vertical.DestroyChild();
            Destroy(LeftCross.gameObject);
            Destroy(RightCross.gameObject);

            matches.Add(Horizontal);
            matches.Add(Vertical);
        }
        else if (sign == EquationSign.Multiply)
        {
            Destroy(Vertical.gameObject);
            Destroy(Horizontal.gameObject);

            matches.Add(LeftCross);
            matches.Add(RightCross);
        }
        else if (sign == EquationSign.Division)
        {
            LeftCross.DestroyChild();
            Destroy(Vertical.gameObject);
            Destroy(Horizontal.gameObject);

            matches.Add(LeftCross);
            matches.Add(RightCross);
        }
    }

    public EquationSign GetOperator()
    {
        if (LeftCross == null && RightCross == null && Horizontal.IsPlaced() && Vertical.IsPlaced())
        {
            return EquationSign.Plus;
        }
        else if (LeftCross == null && RightCross == null && Horizontal.IsPlaced() && Vertical.IsPlaced() == false)
        {
            return EquationSign.Minus;
        }
        else if (Vertical == null && Horizontal == null && LeftCross.IsPlaced() && RightCross.IsPlaced())
        {
            return EquationSign.Multiply;
        }
        else if (Vertical == null && Horizontal == null && LeftCross.IsPlaced() == false && RightCross.IsPlaced())
        {
            return EquationSign.Division;
        }
        else
        {
            return EquationSign.noSign;
        }
    }
}
