using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeButtonHandler : MonoBehaviour
{
    public void OnButtonPressed()
    {
        SceneManager.LoadScene("HomeScene");
        var cam = GameObject.FindGameObjectWithTag("MainCamera");
        if (cam != null)
        {Debug.Log("gugdfus");
            cam.GetComponent<HomeScene>().OnBackClick();
        }
    }
}