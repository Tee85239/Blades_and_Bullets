using System;
using TMPro;
using UnityEngine;

public class PrintAllHighScores : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI highScoreText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SavedDataJSON.OnAllHighScoreDataGathered += OnAllHighScoreDataGathered;
        
    }

    private void OnDestroy()
    {
        SavedDataJSON.OnAllHighScoreDataGathered -= OnAllHighScoreDataGathered;
    }

    private void OnAllHighScoreDataGathered(object sender, SavedDataJSON.OnAllHighScoreDataGatheredArgs e)
    {
        SavedDataJSON.HighScoreEntry[] highScores = e.AllHighScore;

        highScoreText.text = "";
        int leaderboardPos = 1;
        foreach (SavedDataJSON.HighScoreEntry highScore in highScores)
        {
            highScoreText.text += leaderboardPos +" - "+highScore.name + ": " + highScore.score + "\n";
            leaderboardPos++;
        }
        
        
    }


}
