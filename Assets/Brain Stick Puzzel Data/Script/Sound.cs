using UnityEngine;
using System.Collections;
using System;

public class Sound : MonoBehaviour
{
    public AudioSource audioSource;
    public enum Button { Default };
    public enum Others {  };

    [HideInInspector]
    public AudioClip[] buttonClips;
    [HideInInspector]
    public AudioClip[] otherClips;

    public static Sound instance;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    private void Start()
    {
        UpdateSetting();
    }

    public bool IsMuted()
    {
        return !IsEnabled();
    }

    public bool IsEnabled()
    {
        return PlayerPrefs.GetInt("sound_enabled", 1) == 1;
    }

    public void SetEnabled(bool enabled)
    {
        PlayerPrefs.SetInt("sound_enabled", enabled ? 1 : 0);
        UpdateSetting();
    }

    public void Play(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    public void Play(AudioSource audioSource)
    {
        if (IsEnabled())
        {
            audioSource.Play();
        }
    }

    public void PlayButton(Button type = Button.Default)
    {
        int index = (int)type;
        audioSource.PlayOneShot(buttonClips[index]);
    }

    public void Play(Others type, float volume = 1)
    {
        int index = (int)type;
        audioSource.volume = volume;
        audioSource.PlayOneShot(otherClips[index]);
    }

    public void UpdateSetting()
    {
        audioSource.mute = IsMuted();
    }
}