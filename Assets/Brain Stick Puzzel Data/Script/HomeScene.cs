using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MS;

public class HomeScene : MonoBehaviour
{
    public Toggle musicToggle;
    public Toggle soundToggle;
    public GameObject homePage, packPage, levelPage, settingPage;
    public RectTransform rt;

    [Header("Level Pack Selection")]
    public Transform LPS_Container;
    public Button LPS_LockedItem, LPS_UnlockedItem;

    [Header("Level Selection")]
    public Transform LS_Container;
    public Button LS_LockedItem, LS_PlayedItem, LS_PlayingItem;
    public static HomeScene Intance;
    public ScrollRect PackScrollview, LevelScrollView;

    private enum Page { Home, Pack, Level, Game };
    private Page page = Page.Home;
    private float screenWidth;

    private void Start()
    {
        Intance = this;

        if (!GameManager.openLevelSelectionMenu)
            LevelManager.Intance.CurrentLevelPack = LevelManager.Intance.PackList[LevelManager.Intance.CompletedPack];

        SetupLevelPackSelection();
        SetupLevelSelection();

        musicToggle.isOn = Music.instance.IsEnabled();
        musicToggle.onValueChanged.AddListener((arg0) => Music.instance.SetEnabled(arg0, true));


        soundToggle.isOn = Sound.instance.IsEnabled();
        soundToggle.onValueChanged.AddListener((arg0) => Sound.instance.SetEnabled(arg0));

        PackScrollview.enabled = true;
        LevelScrollView.enabled = true;
        screenWidth = rt.rect.width;

        if (GameManager.openLevelSelectionMenu)
        {
            GameManager.openLevelSelectionMenu = false;

            packPage.transform.localPosition = new Vector3(-screenWidth * 0.3f, 0);
            levelPage.transform.localPosition = Vector3.zero;
            page = Page.Level;
            UpdateLevelContentPosition();
        }
        else if (GameManager.openLevelPackSelectionMenu)
        {
            GameManager.openLevelSelectionMenu = false;

            packPage.transform.localPosition = Vector3.zero;
            levelPage.transform.localPosition = new Vector3(screenWidth, 0);
            page = Page.Pack;
            UpdatePackContentPosition();
        }
        else
        {
            packPage.transform.localPosition = new Vector3(screenWidth, 0);
            levelPage.transform.localPosition = new Vector3(screenWidth, 0);
            page = Page.Home;
        }

        Music.instance.PlayAMusic();
    }

    private void UpdateLevelContentPosition()
    {
        Timer.Schedule(this, 0, () =>
        {
            ShowChildInScrollView(LevelScrollView, LevelManager.Intance.CurrentLevelPack.CompletedLevels);
        });
    }

    private void UpdatePackContentPosition()
    {
        Timer.Schedule(this, 0, () =>
        {
            ShowChildInScrollView(PackScrollview, LevelManager.Intance.CompletedPack);
        });
    }

    void Update()
    {
        if (Popup.current != null && Popup.current.closeOnEsc && Popup.current.isOpen && Input.GetKeyUp(KeyCode.Escape))
        {
            Popup.current.Close();
        }

        screenWidth = rt.rect.width;
    }

    void SetupLevelPackSelection()
    {
        for (int i = 0; i < LevelManager.Intance.PackList.Count; i++)
        {
            int index = i;
            if (LevelManager.Intance.CompletedPack >= i)
            {
                Button temp = Instantiate(LPS_UnlockedItem, LPS_Container);
                temp.onClick.AddListener(() => OnUnLockLevelPack(index));
                temp.GetComponentsInChildren<Text>()[0].text = LevelManager.Intance.PackList[i].LevelPackName;
               // temp.GetComponentsInChildren<Text>()[1].text = LevelManager.Intance.PackList[i].CompletedLevelInPercent + "% levels";

                Image fillImg = temp.transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).GetComponent<Image>();
                Image glow = temp.transform.GetChild(1).transform.GetChild(0).GetComponent<Image>();
                float completed = LevelManager.Intance.PackList[i].CompletedLevels;
                float total = LevelManager.Intance.PackList[i].TotalLevels;


                fillImg.fillAmount = completed / total;
                if (completed == total)
                {
                    glow.fillAmount = 1;
                }
                else
                {
                    glow.fillAmount = 0;
                }


            }
            else
            {
                Button temp = Instantiate(LPS_LockedItem, LPS_Container);
                temp.GetComponentsInChildren<Text>()[0].text = LevelManager.Intance.PackList[i].LevelPackName;
              //  temp.GetComponentsInChildren<Text>()[1].text = LevelManager.Intance.PackList[i].TotalLevels + " levels";
            }
        }
    }

    void OnUnLockLevelPack(int index)
    {
        int lastIndex = LevelManager.Intance.CurrentLevelPackIndex;
        LevelManager.Intance.CurrentLevelPack = LevelManager.Intance.PackList[index];

        if (index != lastIndex)
        {
            SetupLevelSelection();
            Timer.Schedule(this, 0f, GoToLevelScreen);
        }
        else
        {
            GoToLevelScreen();
        }
    }

    void SetupLevelSelection()
    {
        for (int i = LS_Container.childCount - 1; i >= 0; i--)
        {
            Destroy(LS_Container.GetChild(i).gameObject);
        }
        for (int i = 0; i < LevelManager.Intance.CurrentLevelPack.TotalLevels; i++)
        {
            int index = i;

            if (LevelManager.Intance.CurrentLevelPack.CompletedLevels > i)
            {
                Button temp = Instantiate(LS_PlayedItem, LS_Container);
                temp.GetComponentsInChildren<Text>()[0].text = "Level " + (i + 1);
                temp.onClick.AddListener(() => OnPlayUnlockedLevel(index));
            }
            else if (LevelManager.Intance.CurrentLevelPack.CompletedLevels < i)
            {
                Button temp = Instantiate(LS_LockedItem, LS_Container);
                temp.GetComponentsInChildren<Text>()[0].text = "Level " + (i + 1);
            }
            else
            {
                Button temp = Instantiate(LS_PlayingItem, LS_Container);
                temp.GetComponentsInChildren<Text>()[0].text = "Level " + (i + 1);
                temp.onClick.AddListener(() => OnPlayUnlockedLevel(index));
            }
        }
    }

    void OnPlayUnlockedLevel(int index)
    {
        LevelManager.Intance.CurrentLevelIndex = index;
        ScreenFader.instance.GotoScene("GameScene");

        Sound.instance.PlayButton();
    }

    //    public void OnOtherApp()
    //    {
    //#if UNITY_ANDROID
    //        Application.OpenURL("https://play.google.com/store/apps/developer?id=VOODOO");
    //#elif UNITY_IOS
    //        Application.OpenURL("");
    //#else
    //        Application.OpenURL("https://play.google.com/store/apps/developer?id=VOODOO");
    //#endif

    //        Sound.instance.PlayButton();
    //    }

    public void GoToPackScreen()
    {
        if (page == Page.Home)
        {
            LeanTween.moveLocalX(homePage.gameObject, -screenWidth * 0.2f, 0.3f).setEase(LeanTweenType.easeOutQuad);
            LeanTween.moveLocalX(packPage.gameObject, 0, 0.3f).setEase(LeanTweenType.easeOutQuad);
        }
        else if (page == Page.Level)
        {
            LeanTween.moveLocalX(levelPage.gameObject, screenWidth, 0.3f).setEase(LeanTweenType.easeOutQuad);
            LeanTween.moveLocalX(packPage.gameObject, 0, 0.3f).setEase(LeanTweenType.easeOutQuad);
        }

        page = Page.Pack;
        UpdatePackContentPosition();

        Sound.instance.PlayButton();
    }

    public void GoToLevelScreen()
    {
        if (page == Page.Pack)
        {
            LeanTween.moveLocalX(packPage.gameObject, -screenWidth * 0.2f, 0.3f).setEase(LeanTweenType.easeOutQuad);
            LeanTween.moveLocalX(levelPage.gameObject, 0, 0.3f).setEase(LeanTweenType.easeOutQuad);
        }

        page = Page.Level;
        UpdateLevelContentPosition();
        Sound.instance.PlayButton();
    }

    public void OnBackClick()
    {
        if (page == Page.Pack)
        {
            homePage.transform.localPosition = new Vector3(-screenWidth * 0.3f, 0);
            LeanTween.moveLocalX(homePage.gameObject, 0, 0.3f).setEase(LeanTweenType.easeOutQuad);
            LeanTween.moveLocalX(packPage.gameObject, screenWidth, 0.3f).setEase(LeanTweenType.easeOutQuad);

            page = Page.Home;
            Sound.instance.PlayButton();
        }
        else if (page == Page.Level)
        {
            GoToPackScreen();
        }

    }

    public static void ShowChildInScrollView(ScrollRect scrollRect, int index, int direction = 1, Transform content = null)
    {
        if (content == null) content = scrollRect.content;
        if (content.childCount == 0) return;
        index = Mathf.Clamp(index, 0, content.childCount - 1);

        if (scrollRect.vertical)
        {
            var child = content.GetChild(index);
            float localY = child.GetComponent<RectTransform>().anchoredPosition.y;
            float scrollHeight = scrollRect.GetComponent<RectTransform>().rect.height;
            float contentY;

            if (direction == 1)
            {
                contentY = -localY - scrollHeight / 2;
                float maxY = scrollRect.content.rect.height - scrollHeight;
                contentY = Mathf.Max(Mathf.Min(contentY, maxY), 0);
            }
            else
            {

                contentY = -(localY - scrollHeight / 2);
                float minY = -(scrollRect.content.rect.height - scrollHeight);
                contentY = Mathf.Min(Mathf.Max(minY, contentY), 0);
            }
            scrollRect.content.GetComponent<RectTransform>().anchoredPosition = new Vector2(scrollRect.content.GetComponent<RectTransform>().anchoredPosition.x, contentY);
        }
        else
        {
            var child = content.GetChild(index);
            float localX = child.GetComponent<RectTransform>().anchoredPosition.x;
            float scrollWidth = scrollRect.GetComponent<RectTransform>().rect.width;
            float contentX;

            if (direction == 1)
            {
                contentX = -localX - scrollWidth / 2;
                float maxX = scrollRect.content.rect.width - scrollWidth;
                contentX = Mathf.Max(Mathf.Min(contentX, maxX), 0);
            }
            else
            {
                contentX = -(localX - scrollWidth / 2);
                float minX = -(scrollRect.content.rect.width - scrollWidth);
                contentX = Mathf.Min(Mathf.Max(minX, contentX), 0);
            }

            scrollRect.content.GetComponent<RectTransform>().anchoredPosition = new Vector2(contentX, scrollRect.content.GetComponent<RectTransform>().anchoredPosition.y);
        }
    }

    public void PlayButton()
    {
        Sound.instance.PlayButton();
    }
}
