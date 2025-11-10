using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Digit : MonoBehaviour
{
    public int myDigit;
    public Matches Middle, Up, Down, LeftUp, LeftDown, RightUp, RightDown;

    [HideInInspector]
    public List<Matches> matches;

    public void SetMyDigit(int digit)
    {
        myDigit = digit;
        switch (digit)
        {
            case 0:
                Middle.DestroyChild();

                matches.Add(Middle);
                matches.Add(Up);
                matches.Add(Down);
                matches.Add(LeftUp);
                matches.Add(LeftDown);
                matches.Add(RightUp);
                matches.Add(RightDown);

                break;
            case 1:
                Middle.DestroyChild();
                Up.DestroyChild();
                Down.DestroyChild();
                LeftUp.DestroyChild();
                LeftDown.DestroyChild();

                matches.Add(Middle);
                matches.Add(Up);
                matches.Add(Down);
                matches.Add(LeftUp);
                matches.Add(LeftDown);
                matches.Add(RightUp);
                matches.Add(RightDown);

                break;
            case 2:
                LeftUp.DestroyChild();
                RightDown.DestroyChild();

                matches.Add(Middle);
                matches.Add(Up);
                matches.Add(Down);
                matches.Add(LeftUp);
                matches.Add(LeftDown);
                matches.Add(RightUp);
                matches.Add(RightDown);

                break;
            case 3:
                LeftUp.DestroyChild();
                LeftDown.DestroyChild();

                matches.Add(Middle);
                matches.Add(Up);
                matches.Add(Down);
                matches.Add(LeftUp);
                matches.Add(LeftDown);
                matches.Add(RightUp);
                matches.Add(RightDown);

                break;
            case 4:
                Up.DestroyChild();
                Down.DestroyChild();
                LeftDown.DestroyChild();

                matches.Add(Middle);
                matches.Add(Up);
                matches.Add(Down);
                matches.Add(LeftUp);
                matches.Add(LeftDown);
                matches.Add(RightUp);
                matches.Add(RightDown);

                break;
            case 5:
                RightUp.DestroyChild();
                LeftDown.DestroyChild();

                matches.Add(Middle);
                matches.Add(Up);
                matches.Add(Down);
                matches.Add(LeftUp);
                matches.Add(LeftDown);
                matches.Add(RightUp);
                matches.Add(RightDown);

                break;
            case 6:
                RightUp.DestroyChild();

                matches.Add(Middle);
                matches.Add(Up);
                matches.Add(Down);
                matches.Add(LeftUp);
                matches.Add(LeftDown);
                matches.Add(RightUp);
                matches.Add(RightDown);

                break;
            case 7:
                Middle.DestroyChild();
                Down.DestroyChild();
                LeftUp.DestroyChild();
                LeftDown.DestroyChild();

                matches.Add(Middle);
                matches.Add(Up);
                matches.Add(Down);
                matches.Add(LeftUp);
                matches.Add(LeftDown);
                matches.Add(RightUp);
                matches.Add(RightDown);

                break;
            case 8:
                matches.Add(Middle);
                matches.Add(Up);
                matches.Add(Down);
                matches.Add(LeftUp);
                matches.Add(LeftDown);
                matches.Add(RightUp);
                matches.Add(RightDown);

                break;
            case 9:
                LeftDown.DestroyChild();

                matches.Add(Middle);
                matches.Add(Up);
                matches.Add(Down);
                matches.Add(LeftUp);
                matches.Add(LeftDown);
                matches.Add(RightUp);
                matches.Add(RightDown);

                break;
 
            default:
                break;
        }
    }

    public int GetDigit()
    {
        if (Middle.IsPlaced() == false && LeftUp.IsPlaced() && LeftDown.IsPlaced() &&
            RightUp.IsPlaced() && RightDown.IsPlaced() && Up.IsPlaced() && Down.IsPlaced())
        {
            return 0;
        }
        else if (Middle.IsPlaced() == false && LeftUp.IsPlaced() == false && LeftDown.IsPlaced() == false &&
               RightUp.IsPlaced() && RightDown.IsPlaced() && Up.IsPlaced() == false && Down.IsPlaced() == false)
        {
            return 1;
        }
        else if (Middle.IsPlaced() && LeftUp.IsPlaced() == false && LeftDown.IsPlaced() &&
               RightUp.IsPlaced() && RightDown.IsPlaced() == false && Up.IsPlaced() && Down.IsPlaced())
        {
            return 2;
        }
        else if (Middle.IsPlaced() && LeftUp.IsPlaced() == false && LeftDown.IsPlaced() == false &&
                RightUp.IsPlaced() && RightDown.IsPlaced() && Up.IsPlaced() && Down.IsPlaced())
        {
            return 3;
        }
        else if (Middle.IsPlaced() && LeftUp.IsPlaced() && LeftDown.IsPlaced() == false &&
                RightUp.IsPlaced() && RightDown.IsPlaced() && Up.IsPlaced() == false && Down.IsPlaced() == false)
        {
            return 4;
        }
        else if (Middle.IsPlaced() && LeftUp.IsPlaced() && LeftDown.IsPlaced() == false &&
                RightUp.IsPlaced() == false && RightDown.IsPlaced() && Up.IsPlaced() && Down.IsPlaced())
        {
            return 5;
        }
        else if (Middle.IsPlaced() && LeftUp.IsPlaced() && LeftDown.IsPlaced() &&
                RightUp.IsPlaced() == false && RightDown.IsPlaced() && Up.IsPlaced() && Down.IsPlaced())
        {
            return 6;
        }
        else if (Middle.IsPlaced() == false && LeftUp.IsPlaced() == false && LeftDown.IsPlaced() == false &&
                RightUp.IsPlaced() && RightDown.IsPlaced() && Up.IsPlaced() && Down.IsPlaced() == false)
        {
            return 7;
        }
        else if (Middle.IsPlaced() && LeftUp.IsPlaced() && LeftDown.IsPlaced() &&
                RightUp.IsPlaced() && RightDown.IsPlaced() && Up.IsPlaced() && Down.IsPlaced())
        {
            return 8;
        }
        else if (Middle.IsPlaced() && LeftUp.IsPlaced() && LeftDown.IsPlaced() == false &&
                RightUp.IsPlaced() && RightDown.IsPlaced() && Up.IsPlaced() && Down.IsPlaced())
        {
            return 9;
        }
        else if (Middle.IsPlaced() == false &&
                Up.IsPlaced() == false &&
                Down.IsPlaced() == false &&
                LeftUp.IsPlaced() == false &&
                LeftDown.IsPlaced() == false &&
                RightUp.IsPlaced() == false &&
                RightDown.IsPlaced() == false)
        {
            return -1;
        }

        return -2;
    }
}
