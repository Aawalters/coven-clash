using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void Start()
    {
        musicSource.clip = backgroundMusic;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }
}
