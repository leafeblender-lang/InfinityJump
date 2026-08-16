using UnityEngine;
using UnityEngine.Audio;
/*
public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public AudioSource effectsSource;
    public AudioClip dumping;
    public AudioClip backgroundMusic;
    public AudioClip coinCollect;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); //ostace kroz scene
        }
        else
        {
            Destroy(gameObject);    
        }
    }

    private void PlaySound(AudioClip clip)
    {
        effectsSource.PlayOneShot(clip);
       
    }

    public void ZvukOdskoka()
    {
        PlaySound(dumping);
    }
    public void ZvukPrikupljenogCoina()
    {
        PlaySound(coinCollect);
    }




}*/
public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public AudioSource musicSource;    // AudioSource za pozadinsku muziku
    public AudioSource effectsSource;  // AudioSource za SFX

    public AudioClip dumping;
    public AudioClip backgroundMusic;
    public AudioClip coinCollect;

    public AudioMixer audioMixer;

    private void Awake()
    {
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
        float savedSFX = PlayerPrefs.GetFloat("SFXVolume", 0.75f);
        SetSFXVolume(savedSFX);

        float savedMusic = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        SetMusicVolume(savedMusic);

        // Start pozadinske muzike
        musicSource.clip = backgroundMusic;
        musicSource.loop = true;
        musicSource.Play();
    }

    private void PlaySound(AudioClip clip)
    {
        effectsSource.PlayOneShot(clip);
    }

    public void ZvukOdskoka()
    {
        PlaySound(dumping);
    }

    public void ZvukPrikupljenogCoina()
    {
        PlaySound(coinCollect);
    }

    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20);
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20);
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }
}
