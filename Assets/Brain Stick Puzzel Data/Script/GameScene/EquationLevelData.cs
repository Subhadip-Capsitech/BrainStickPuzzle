using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquationLevelData : MonoBehaviour
{
    public Transform Number1, Number2, Number3, OpratorTransform;
    public Digit DigitPrefab;
    public Oprator OpratorPrefab;

    [HideInInspector]
    public string Num1, Num2, Num3;
    [HideInInspector]
    public EquationSign EquationSign;
    [HideInInspector]
    public List<Matches> matches;

    private float maxWidth;

    private IEnumerator Start()
    {
        transform.localScale = Vector3.zero;

        yield return new WaitForSeconds(0.05f);

        transform.GetChild(0).GetComponent<ContentSizeFitter>().enabled = false;
        transform.GetChild(1).GetComponent<ContentSizeFitter>().enabled = false;

        yield return 0;

        transform.GetChild(0).GetComponent<ContentSizeFitter>().enabled = true;
        transform.GetChild(1).GetComponent<ContentSizeFitter>().enabled = true;

        yield return 0;

        float line1Width = transform.GetChild(0).GetComponent<RectTransform>().rect.width;
        float line2Width = transform.GetChild(1).GetComponent<RectTransform>().rect.width;
        float width = Mathf.Max(line1Width, line2Width);
        float scale = Mathf.Min(1, maxWidth / width);
        transform.localScale = scale * Vector3.one;
    }

    public void SetMaxWidth(float maxWidth)
    {
        this.maxWidth = maxWidth;
    }

    public void SetupOprator()
    {
        Oprator op = Instantiate(OpratorPrefab, OpratorTransform);
        op.matches = matches;
        op.SetOperator(EquationSign);
    }

    public void SetupEquation()
    {
        foreach (char item in Num1)
        {
            Digit m_Digit = Instantiate(DigitPrefab, Number1);
            m_Digit.transform.localScale = Vector3.one;
            m_Digit.matches = matches;
            m_Digit.SetMyDigit(int.Parse(item.ToString()));
        }
        foreach (char item in Num2)
        {
            Digit m_Digit = Instantiate(DigitPrefab, Number2);
            m_Digit.transform.localScale = Vector3.one;
            m_Digit.matches = matches;
            m_Digit.SetMyDigit(int.Parse(item.ToString()));
        }
        foreach (char item in Num3)
        {
            Digit m_Digit = Instantiate(DigitPrefab, Number3);
            m_Digit.transform.localScale = Vector3.one;
            m_Digit.matches = matches;
            m_Digit.SetMyDigit(int.Parse(item.ToString()));
        }
    }
}

public enum EquationSign
{
    Plus,
    Minus,
    Multiply,
    Division,
    noSign
}
