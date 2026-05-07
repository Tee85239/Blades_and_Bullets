using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject highScoresPanel;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private GameObject introVideoPanel;
    [SerializeField] private VideoPlayer introVideoPlayer;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Sprite defaultBackground;
    [SerializeField] private Sprite playHoverBackground;
    [SerializeField] private Sprite quitHoverBackground;
    [SerializeField] private string gameSceneName = "Stable_BladesAndBullets";
    [SerializeField] private float musicFadeDuration = 1.5f;

    [SerializeField] private bool useIntroVideo = false;

    public void SetDefaultBackground()
    {
        backgroundImage.sprite = defaultBackground;
    }

    public void SetPlayHoverBackground()
    {
        backgroundImage.sprite = playHoverBackground;
    }

    public void SetQuitHoverBackground()
    {
        backgroundImage.sprite = quitHoverBackground;
    }
    private void Start()
    {
        introVideoPanel.SetActive(false);
        introVideoPlayer.loopPointReached += LoadGameSceneAfterVideo; // runs loadgamesceneaftervideo when the video finishes
        ShowMainMenu();
    }
    public void PlayIntroVideo()
    {
        if (!useIntroVideo)
        {
            SceneManager.LoadScene(gameSceneName);
            return;
        }

        StartCoroutine(PlayIntroVideoRoutine());
    }
    private IEnumerator PlayIntroVideoRoutine() // controls the full transition from menu to intro video
    {
        mainMenuPanel.SetActive(false);
        highScoresPanel.SetActive(false);
        optionsPanel.SetActive(false); 
        creditsPanel.SetActive(false);

        yield return StartCoroutine(audioManager.FadeOutMusic(musicFadeDuration));

        introVideoPanel.SetActive(true);
        introVideoPlayer.Prepare(); // preloads/decodes the video before playback begins

        yield return new WaitUntil(() => introVideoPlayer.isPrepared); // waits until unity says the video is ready

        introVideoPlayer.Play();
    }
    private void LoadGameSceneAfterVideo(VideoPlayer videoPlayer)
    {
        if (videoPlayer.frameCount > 0 && videoPlayer.frame < (long)(videoPlayer.frameCount - 1)) // checks for false early end events
        {
            return; // ignores bad early end calls from video decoding/timestamp issues
        }

        SceneManager.LoadScene(gameSceneName);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        highScoresPanel.SetActive(false);
        optionsPanel.SetActive(false);
        creditsPanel.SetActive(false);
    }

    public void ShowHighScores()
    {
        
        highScoresPanel.SetActive(true);
        
    }

    public void ShowOptions()
    {
        mainMenuPanel.SetActive(false);
        highScoresPanel.SetActive(false);
        optionsPanel.SetActive(true);
        creditsPanel.SetActive(false);
    }

    public void ShowCredits()
    {
        mainMenuPanel.SetActive(false);
        highScoresPanel.SetActive(false);
        optionsPanel.SetActive(false);
        creditsPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    private void OnDestroy()
    {
        introVideoPlayer.loopPointReached -= LoadGameSceneAfterVideo; // removes the video end listener to avoid stale references
    }
}