using UnityEngine;
using UnityEngine.UI;
public class QuitePanel_script : MonoBehaviour
{
    public Slider musicSlider;
    public Slider sfxSlider;

    private void Start()
    {
        if (musicSlider != null && sfxSlider != null)
        {
            // Ucitaj prethodne vrednosti
            musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
            sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0.75f);

            // Dodaj slušaoce za promene slidera
            musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        }
    }

    private void OnMusicVolumeChanged(float value)
    {
        SoundManager.instance.SetMusicVolume(value);
    }

    private void OnSFXVolumeChanged(float value)
    {
        SoundManager.instance.SetSFXVolume(value);
    }
}
