using UnityEngine;

public class GameplayAudioManager : MonoBehaviour
{
    public static GameplayAudioManager Instance { get; private set; } // global scene access point for gameplay audio

    [SerializeField] private AudioSource sfxSource; // plays gameplay sound effects through one controlled source
    [SerializeField] private AudioSource musicSource; // handles gameplay music

    private const string SFXKey = "SFXVolume"; // must match the main menu sfx playerprefs key

    public void SetMusicVolume(float volume) // changes gameplay music volume
    {
        if (musicSource == null) // checks if no music source was assigned
        {
            return; // exits safely if gameplay music is not set up yet
        }

        musicSource.volume = volume; // applies new music volume
    }

    public void SetSFXVolume(float volume) // changes gameplay sfx volume
    {
        if (sfxSource == null) // checks if no sfx source was assigned
        {
            return; // exits safely if gameplay sfx is not set up
        }

        sfxSource.volume = volume; // applies new sfx volume
    }

    private void Awake()
    {
        Instance = this; // stores this manager so other scripts can call it easily
    }

    private void Start()
    {
        sfxSource.volume = PlayerPrefs.GetFloat(SFXKey, 1f); // applies saved sfx volume from the main menu options slider
    }

    public void PlaySFX(AudioClip clip, float volumeScale = 1f) // plays a sound effect with optional volume boost
    {
        if (clip == null) // checks if no clip was provided
        {
            return; // exits safely
        }

        sfxSource.PlayOneShot(clip, volumeScale); // plays clip using the requested volume scale
    }

    public void PlayLoop(AudioClip clip)
    {
        if (clip == null)
        {
            return;
        }

        sfxSource.clip = clip; // assigns the clip for looping playback
        sfxSource.loop = true; // enables looping
        sfxSource.Play(); // starts the loop
    }

    public void StopLoop() // stops the current looping sound effect
    {
        sfxSource.Stop(); // stops the currently playing loop
        sfxSource.loop = false; // disables looping for future sounds
        sfxSource.clip = null; // clears the loop clip reference
    }

    public void MuteAllOtherAudioSources(float volume) // lowers every audiosource in the scene except this manager's sfx source
    {
        AudioSource[] audioSources = FindObjectsByType<AudioSource>(FindObjectsInactive.Exclude, FindObjectsSortMode.None); // finds all active audiosources in the scene

        foreach (AudioSource audioSource in audioSources) // loops through every audiosource found
        {
            if (audioSource == sfxSource) // checks if this is the manager source used for death sound
            {
                continue; // skips this source so the death sound stays audible
            }

            audioSource.volume = volume; // lowers or mutes enemy sfx and music sources
        }
    }
}