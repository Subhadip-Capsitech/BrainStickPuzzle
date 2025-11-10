using UnityEngine;
using System.Collections;
using System;

public class Music : MonoBehaviour
{
    public AudioSource audioSource;
    public enum Type { Main_1, Main_2 };

    [HideInInspector]
    public AudioClip[] musicClips;

    private Type currentType;

    public static Music instance;

    private void Awake()
    {
        if (instance == null) instance = this;

        int length = Enum.GetNames(typeof(Type)).Length;
        currentType = (Type)UnityEngine.Random.Range(0, length);
    }

    public bool IsEnabled()
    {
        return PlayerPrefs.GetInt("music_enabled", 1) == 1;
    }

    public void SetEnabled(bool enabled, bool updateMusic = false)
    {
        PlayerPrefs.SetInt("music_enabled", enabled ? 1 : 0);
        if (updateMusic)
            UpdateSetting();
    }

    public void Play(Type type)
    {
        if (currentType != type || !audioSource.isPlaying)
        {
            StartCoroutine(PlayNewMusic(type));
        }
    }

    private IEnumerator PlayNewMusic(Type type)
    {
        while (audioSource.volume >= 0.1f)
        {
            audioSource.volume -= 0.2f;
            yield return new WaitForSeconds(0.1f);
        }

        audioSource.Stop();
        currentType = type;
        audioSource.clip = musicClips[(int)type];

        if (IsEnabled())
        {
            audioSource.Play();
        }
        audioSource.volume = 1;
    }

    private void UpdateSetting()
    {
        if (audioSource == null) return;
        if (IsEnabled())
            Play(currentType);
        else
            audioSource.Stop();
    }

    private float lastMusicTime = int.MinValue;

    public void PlayAMusic()
    {
        if (Time.time - lastMusicTime < 180)
        {
            audioSource.UnPause();
            return;
        }

        int length = Enum.GetNames(typeof(Type)).Length;

        var newType = (Type)(((int)currentType + 1) % length);
        lastMusicTime = Time.time;

        Play(newType);
    }
}
