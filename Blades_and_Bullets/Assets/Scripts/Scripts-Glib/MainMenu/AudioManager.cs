using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip buttonClick;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private const string MusicKey = "MusicVolume";
    private const string SFXKey = "SFXVolume";

    private void Start()
    {
        float savedMusicVolume = PlayerPrefs.GetFloat(MusicKey, 1f); // loads saved music volume, defaults to full volume
        float savedSFXVolume = PlayerPrefs.GetFloat(SFXKey, 1f);

        musicSlider.value = savedMusicVolume; // sets music slider to saved value
        sfxSlider.value = savedSFXVolume;

        musicSource.volume = savedMusicVolume; // applies saved music volume
        sfxSource.volume = savedSFXVolume;

        musicSlider.onValueChanged.AddListener(SetMusicVolume); // calls setmusicvolume when the music slider moves
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);

        musicSource.clip = menuMusic; // assigns the menu music clip to the music source
        musicSource.Play();
    }

    public void SetMusicVolume(float value) // receives slider value from 0 to 1
    {
        musicSource.volume = value;
        PlayerPrefs.SetFloat(MusicKey, value); // saves the music volume
    }

    public void SetSFXVolume(float value)
    {
        sfxSource.volume = value;
        PlayerPrefs.SetFloat(SFXKey, value);
    }

    public void PlayButtonClick()
    {
        sfxSource.PlayOneShot(buttonClick);
    }

    public IEnumerator FadeOutMusic(float duration) // fades menu music down before intro video
    {
        float startVolume = musicSource.volume;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0f, time / duration);
            yield return null;
        }

        musicSource.volume = 0f;
        musicSource.Stop();
    }
}