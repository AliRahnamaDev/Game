using System.Collections.Generic;
using UnityEngine;

public enum SFXType { Jump, Hit, Dash, OpenDoor, Coin, Die, Block, Slide, Switch, Click }

[System.Serializable]
public class SFXEntry
{
    public SFXType type;
    public AudioClip clip;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource backgroundMusicSource;
    public AudioSource sfxSource;

    [Header("SFX Clips")]
    public List<SFXEntry> sfxEntries;

    private Dictionary<SFXType, AudioClip> sfxDict;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeSFXDict();
            LoadVolumes();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeSFXDict()
    {
        sfxDict = new Dictionary<SFXType, AudioClip>();
        foreach (var entry in sfxEntries)
        {
            if (!sfxDict.ContainsKey(entry.type))
                sfxDict[entry.type] = entry.clip;
        }
    }

    private void LoadVolumes()
    {
        float bgVol = PlayerPrefs.GetFloat("BackgroundVolume", 0.8f);
        float sfxVol = PlayerPrefs.GetFloat("SFXVolume", 0.8f);
        SetBackgroundVolume(bgVol);
        SetSFXVolume(sfxVol);
    }

    public void SetBackgroundVolume(float volume)
    {
        backgroundMusicSource.volume = volume;
        PlayerPrefs.SetFloat("BackgroundVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    public void PlaySFX(SFXType type)
    {
        if (sfxDict != null && sfxDict.TryGetValue(type, out AudioClip clip))
        {
            if (clip != null)
                sfxSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning("SFX not found: " + type);
        }
    }
}