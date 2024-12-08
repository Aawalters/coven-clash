using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [Header("-------- Audio Source --------")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("-------- Audio Clips --------")]
    public AudioClip backgroundMusic;
    public AudioClip menuMusic;
    public AudioClip click;
    public AudioClip turretFire;
    public AudioClip enemyHit;
    public AudioClip turretUpgrade;
    public AudioClip turretSell;
    public AudioClip turretBuy;

    [Header("-------- Audio Settings --------")]
    [SerializeField] Slider musicVolumeSlider;
    [SerializeField] Slider sfxVolumeSlider;

    private static AudioManager instance;

    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<AudioManager>();
                if (instance == null)
                {
                    Debug.LogError("AudioManager instance not found in the scene!");
                }
            }
            return instance;
        }
    }
    private void Awake()
    {
        // Singleton Pattern to persist between scenes
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

     private void Start()
    {
        // load saved volume settings / set default values
        float savedMusicVolume = PlayerPrefs.GetFloat("MusicVolume", 1.0f);
        float savedSFXVolume = PlayerPrefs.GetFloat("SFXVolume", 1.0f);

        musicSource.volume = savedMusicVolume;
        SFXSource.volume = savedSFXVolume;

        if (musicVolumeSlider != null) musicVolumeSlider.value = savedMusicVolume;
        if (sfxVolumeSlider != null) sfxVolumeSlider.value = savedSFXVolume;

        // Attach listeners to sliders (if they are set)
        if (musicVolumeSlider != null)
            musicVolumeSlider.onValueChanged.AddListener(ChangeMusicVolume);
        if (sfxVolumeSlider != null)
            sfxVolumeSlider.onValueChanged.AddListener(ChangeSFXVolume);

        // Start background music
        musicSource.clip = backgroundMusic;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }

    public void ChangeMusicVolume(float value)
    {
        musicSource.volume = value;
        PlayerPrefs.SetFloat("MusicVolume", value); // Save the volume
        PlayerPrefs.Save();
    }

    public void ChangeSFXVolume(float value)
    {
        SFXSource.volume = value;
        PlayerPrefs.SetFloat("SFXVolume", value); // Save the volume
        PlayerPrefs.Save();
    }
}