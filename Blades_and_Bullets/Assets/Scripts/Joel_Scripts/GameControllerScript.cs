using System;
using System.Collections;
using Game.Collectibles.Player;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameControllerScript : MonoBehaviour
{
    public static GameControllerScript Instance {get; private set;}
    [SerializeField] private Image abilityBarImage;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private TextMeshProUGUI currentScoreText;
    [SerializeField] private TMP_InputField dataHighScoreInputPanel;
    [SerializeField] private RectTransform bombIconArea;
    [SerializeField] private GameObject bombPrefab;
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private TextMeshProUGUI currentWave;
    public static EventHandler AbilityActiveStatus; 
    
    public static EventHandler OnPlayerDeath; 
    [SerializeField] private Slider hpSlider;
    private float _currentPlayerHp = 1f;
    private int _currentScore = 0;
    private int _HighScore = 0;
    private int currentWaveNumber = 1;
    
    public static EventHandler<OnHighScoreDataGatheredArgs> OnNewHighScoreChange;

    public class OnHighScoreDataGatheredArgs : EventArgs
    {
        public int newHighScore;
        public string nickName;
    }
    
    
    public static EventHandler<OnChangeWaveTimerArgs> OnChangeWaveTimer;

    public class OnChangeWaveTimerArgs : EventArgs
    {
        public float newTimer;
        public bool isPlayingValue;
    }
    
    
    private enum HighScoreAchieved
    {
        NoHighScore,
        NewHighScore
    }
    private HighScoreAchieved _highSoreState = HighScoreAchieved.NoHighScore;


    private void Awake()
    {
        PlayerResourceInventory.OnSendPlayerData +=OnSendPlayerData;
    }

    private void OnSendPlayerData(object sender, PlayerResourceInventory.OnSendPlayerDataArgs e)
    {
        UpdateBomb(e.BombsRemaining);
        UpdateLives(e.LivesRemaining);
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    void Start()
    {
        Instance = this;
        abilityBarImage.type = Image.Type.Filled;
        abilityBarImage.fillMethod = Image.FillMethod.Vertical;
        abilityBarImage.fillAmount = 0f;
        Player.ModifyAbilityCooldown +=ModifyAbilityCooldown;
        // Player.OnPlayerGetsHit += PlayerGetsHit;
        SlashScript.OnSlashingSomething += OnSlashingSomething;
        SavedDataJSON.OnHighScoreDataGathered +=OnHighScoreDataGathered;
        WaveTimeline.onWaveRepeat += OnWaveRepeat;
        StartCoroutine(CurrentWave());

    }

    private void OnWaveRepeat(object sender, EventArgs e)
    {
        StartCoroutine(CurrentWave());
    }

    private IEnumerator CurrentWave()
    {
        currentWave.text = $"Current Wave: {currentWaveNumber}";
        currentWave.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        currentWaveNumber++;
        currentWave.gameObject.SetActive(false);
        //go back to main menu scene

    }

    private void OnHighScoreDataGathered(object sender, SavedDataJSON.OnHighScoreDataGatheredArgs e)
    {
        _HighScore = e.highScore;
        highScoreText.text =  ($"HighScore: {_HighScore.ToString("D6")}");
    }

    private void OnSlashingSomething(object sender, EventArgs e)
    {
        _currentScore += 100;
        ScoreChange(_currentScore);
    }

    void OnDestroy()
    {
        Player.ModifyAbilityCooldown -=ModifyAbilityCooldown;
        // Player.OnPlayerGetsHit -= PlayerGetsHit;
        SlashScript.OnSlashingSomething -= OnSlashingSomething;
        SavedDataJSON.OnHighScoreDataGathered -=OnHighScoreDataGathered;
        PlayerResourceInventory.OnSendPlayerData -=OnSendPlayerData;
        WaveTimeline.onWaveRepeat -= OnWaveRepeat;
    }

    // private void PlayerGetsHit(object sender, EventArgs e)
    // {
    //     _currentPlayerHp -= .15f;
    //     hpSlider.value = _currentPlayerHp;
    // if (_currentPlayerHp < .25f) HpDropsToZero();
    // }

    private void ScoreChange(int scoreChange)
    {
        currentScoreText.text =  ($"Score: {scoreChange.ToString("D6")}");
        if (scoreChange >= _HighScore)
        {
            _HighScore = scoreChange;
            highScoreText.text =  ($"HighScore: {_HighScore.ToString("D6")}");
            _highSoreState =  HighScoreAchieved.NewHighScore;
        }
    }

    private void ModifyAbilityCooldown(object sender, Player.ModifyAbilityCooldownArgs e)
    {
        if (e.changeAmount > 0f)
        {
            abilityBarImage.fillAmount += e.changeAmount;
            if (abilityBarImage.fillAmount >= 1f) AbilityActiveStatus?.Invoke(this, EventArgs.Empty);
            
        }
        else abilityBarImage.fillAmount = e.changeAmount;
    }

    private void HpDropsToZero()
    { 
        OnChangeWaveTimer?.Invoke(this, new OnChangeWaveTimerArgs{newTimer = 0, isPlayingValue = false});
        OnPlayerDeath?.Invoke(this, EventArgs.Empty);
        gameOverText.gameObject.SetActive(true);
        if (_highSoreState.Equals(HighScoreAchieved.NewHighScore))
        {
            dataHighScoreInputPanel.gameObject.SetActive(true);
            dataHighScoreInputPanel.Select();
        }
        else StartCoroutine(FinishGameScene(3f));
    }

    public void InputFieldDataHighScore(string input)
    {
        
        OnNewHighScoreChange?.Invoke(this, new OnHighScoreDataGatheredArgs{nickName = input, newHighScore = _HighScore});
        StartCoroutine(FinishGameScene(1f));
        dataHighScoreInputPanel.gameObject.SetActive(false);
    }
    
    
    public void LoadMainMenu()
    {
        StartCoroutine(_LoadCredits());
        

        IEnumerator _LoadCredits()
        {
            yield return new WaitForSeconds(2f);
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync("MainMenuGood");
            while(!loadOperation!.isDone) yield return null;
        }
    }
    
    private IEnumerator FinishGameScene(float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
        //go back to main menu scene
        LoadMainMenu();
    }
    
    
    private void UpdateBomb(int bombsRemaining)
    {
        int currentBombs = bombIconArea.childCount;
        int maxCount = Mathf.Max(currentBombs, bombsRemaining);

        for (int i = 0; i < maxCount; i++)
        {
            if (i >= bombsRemaining && i < currentBombs)
            {
                Destroy(bombIconArea.GetChild(i).gameObject);
            }
            else if (i >= currentBombs && i < bombsRemaining)
            {
                Instantiate(bombPrefab, bombIconArea);
            }
        }
    }

    private void UpdateLives(int livesRemaining)
    {
        float normalized = (float)livesRemaining / 6.0f;
        _currentPlayerHp = 0.1f + normalized * (1f - 0.1f);
        hpSlider.value = _currentPlayerHp;
        if (_currentPlayerHp < .20)
        {
            HpDropsToZero();
        }
    }

    public float GetPlayerHP()
    {
        return _currentPlayerHp;
    }

}
