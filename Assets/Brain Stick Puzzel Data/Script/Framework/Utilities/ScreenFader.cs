using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class ScreenFader : MonoBehaviour
{
    public const float DURATION = 0.25f;
    public Animator anim;
    public Image image;

    public static ScreenFader instance;

    private void Awake()
    {
        if (instance == null) instance = this;
        GTil.Init(this);
    }

    public void FadeOut(Action onComplete)
    {
        anim.SetTrigger("fade_out");
        image.enabled = true;
        Timer.Schedule(this, DURATION, () =>
        {
            onComplete?.Invoke();
        });
    }

    public void FadeIn(Action onComplete)
    {
        anim.SetTrigger("fade_in");
        Timer.Schedule(this, DURATION, () =>
        {
            image.enabled = false;
            onComplete?.Invoke();
        });
    }

    public void GotoScene(string sceneName)
    {
        if (!IsIdle()) return;

        FadeOut(() =>
        {
            SceneManager.LoadScene(sceneName);
        });
    }

    public bool IsIdle()
    {
        return anim.GetCurrentAnimatorStateInfo(0).IsName("Idle");
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("ScreenFader_Out"))
        {
            FadeIn(null);
        }
    }
}
