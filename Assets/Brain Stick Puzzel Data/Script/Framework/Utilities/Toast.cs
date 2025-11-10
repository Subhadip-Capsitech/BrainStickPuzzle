using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class Toast : MonoBehaviour
{
    public RectTransform backgroundTransform;
    public RectTransform messageTransform;

    [HideInInspector]
    public bool isShowing = false;

    private Queue<AToast> queue = new Queue<AToast>();

    public static Toast instance;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    private class AToast
    {
        public string msg;
        public float duration;
        public float requestTime;
        public AToast(string msg, float duration)
        {
            this.msg = msg;
            this.duration = duration;
            requestTime = Time.time;
        }
    }

    public void SetMessage(string msg)
    {
        messageTransform.GetComponent<Text>().text = msg;
        Timer.Schedule(this, 0, () =>
        {
            backgroundTransform.sizeDelta = new Vector2(messageTransform.GetComponent<Text>().preferredWidth + 60, backgroundTransform.sizeDelta.y);
        });
    }

    private void Show(AToast aToast)
    {
        foreach(Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
        SetMessage(aToast.msg);
        GetComponent<Animator>().SetBool("show", true);
        Invoke("Hide", aToast.duration);
        isShowing = true;
    }

    public void ShowMessage(string msg, float duration = 2f)
    {
        AToast aToast = new AToast(msg, duration);
        queue.Enqueue(aToast);

        ShowOldestToast();
    }

    private void Hide()
    {
        GetComponent<Animator>().SetBool("show", false);
        Invoke("CompleteHiding", 0.75f);
    }

    private void CompleteHiding()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
        isShowing = false;
        ShowOldestToast();
    }

    private void ShowOldestToast()
    {
        if (queue.Count == 0) return;
        if (isShowing) return;

        AToast current = queue.Dequeue();
        if (current.requestTime < Time.time - 2)
        {
            ShowOldestToast();
            return;
        }
        Show(current);
    }
}
