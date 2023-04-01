using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TD_AudioManager : MonoBehaviour
{
    public AudioSource SharedAudioSource;
    public AudioSource MusicAudioSource;
    public AudioSource FXAudioSource;

    public static TD_AudioManager instance;

    public AudioClip MainMenuMusic;
    public AudioClip BasicLevelMusic;
    public AudioClip BossMusic;

    public AudioClip GameWinClip;
    public AudioClip GameLoseClip;

    public AudioClip UIMouseOverSound;
    public AudioClip UIClickSound;

    private float musicVolume = 1f;
    private float sfxVolume = 1f;
    private float mainVolume = 1f;

    private void Awake()
    {

    }

    private void OnEnable()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);

        if (!SharedAudioSource) SharedAudioSource = GetComponent<AudioSource>();
        DontDestroyOnLoad(gameObject);
        musicVolume = PlayerPrefs.GetFloat("music_volume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("sfx_volume", 1f);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayMusic(AudioClip clip)
    {
        if (SharedAudioSource) {
            SharedAudioSource.Stop();
            SharedAudioSource.PlayOneShot(clip, musicVolume);
        }
    }

    public void PlayClip(AudioClip clip, Vector3 atPosition)
    {
        if (clip) AudioSource.PlayClipAtPoint(clip, atPosition, sfxVolume);
    }

    public void SetMainVolume(float toVolume)
    {
        toVolume = Mathf.Clamp(toVolume, 0f, 1f);
        SharedAudioSource.volume = toVolume;
    }

    public void SetMusicVolume(float toVolume)
    {
        toVolume = Mathf.Clamp(toVolume, 0f, 1f * SharedAudioSource.volume);
        if (MusicAudioSource) MusicAudioSource.volume = toVolume;
        else SetMainVolume(toVolume);
    }

    public void SetFXVolume(float toVolume)
    {
        toVolume = Mathf.Clamp(toVolume, 0f, 1f) * SharedAudioSource.volume;
        if (FXAudioSource) FXAudioSource.volume = toVolume;
        else SetMainVolume(toVolume);
    }
}
