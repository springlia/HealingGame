using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class bgmManager : MonoBehaviour
{
    AudioSource audioSource;
    public AudioClip bgm;

    public Scrollbar bgmSlider;
    public Scrollbar sfxSlider;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        audioSource.clip = bgm;
        audioSource.loop = true;
        audioSource.Play();

        bgmSlider.value = audioSource.volume;
        sfxSlider.value = GameManager.Instance.audioSource.volume;
    }

    public void SetBGMVolume(float value)
    {
        audioSource.volume = value;
    }

    public void SetSFXVolume(float value)
    {
        GameManager.Instance.audioSource.volume = value;
    }
}
