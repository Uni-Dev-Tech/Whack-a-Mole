using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private AudioSource soundSource;
    public AudioClip click, tapOnEnemy, tapOnAntiEnemy, lose, addLive, loseLive, bonus;

    public AudioSource musicSource;
    public AudioClip[] music;

    static public SoundManager instance;
    private void Awake()
    {
        if(SoundManager.instance != null)
        {
            Destroy(gameObject);
            return;
        }
        SoundManager.instance = this;
    }
    private void Start()
    {
        soundSource = GetComponent<AudioSource>();
        musicSource.clip = music[Random.Range(0, music.Length)];
        //Первый запуск игры
        if (!PlayerPrefs.HasKey("musicVolume"))
            musicSource.volume = 0.5f;
        if (!PlayerPrefs.HasKey("soundVolume"))
            soundSource.volume = 0.5f;
        // Когда настройки уже имеются
        if (PlayerPrefs.HasKey("musicVolume"))
            musicSource.volume = PlayerPrefs.GetFloat("musicVolume");
        if (PlayerPrefs.HasKey("soundVolume"))
            soundSource.volume = PlayerPrefs.GetFloat("soundVolume");

        musicSource.Play();
    }
    /// <summary>
    /// Метод воспроизводящий дорожку
    /// </summary>
    /// <param name="requiredClip">необходимая дорожка</param>
    public void PlaySound(AudioClip requiredClip)
    {
        soundSource.clip = requiredClip;
        soundSource.Play();
    }
    /// <summary>
    /// Метод изменяющий и сохраняющий настройки звука
    /// </summary>
    /// <param name="newVolume">Новое значение</param>
    public void ChangeSoundVolume(float newVolume)
    {
        soundSource.volume = newVolume;
        PlayerPrefs.SetFloat("soundVolume", newVolume);
    }
    /// <summary>
    /// Метод изменяющий и сохраняющий настройки музыки
    /// </summary>
    /// <param name="newVolume">Новое значение</param>
    public void ChangeMusicVolume(float newVolume)
    {
        musicSource.volume = newVolume;
        PlayerPrefs.SetFloat("musicVolume", newVolume);
    }
}
