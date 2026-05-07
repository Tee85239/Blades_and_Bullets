using System;
using System.Collections.Generic;
using System.IO;
using Unity.Properties;
using UnityEngine;

public class SavedDataJSON : MonoBehaviour
{
    [Serializable]
    public class HighScoreEntry
    {
        public string name;
        public int score;

        public HighScoreEntry(string name, int score)
        {
            this.name = name;
            this.score = score;
        }
    }

    [Serializable]
    private class HighScoreData
    {
        public HighScoreEntry[] currentHighScore;
    }

    public static EventHandler<OnHighScoreDataGatheredArgs> OnHighScoreDataGathered;

    public class OnHighScoreDataGatheredArgs : EventArgs
    {
        public int highScore;
    }
    
    public static EventHandler<OnAllHighScoreDataGatheredArgs> OnAllHighScoreDataGathered;

    public class OnAllHighScoreDataGatheredArgs : EventArgs
    {
        public HighScoreEntry[] AllHighScore;
    }
    
    

    private static HighScoreData data;

    private string path =>
        Application.persistentDataPath + Path.AltDirectorySeparatorChar + "SavedHighScore.json";

    void Start()
    {
        
        data = new HighScoreData();

        GameControllerScript.OnNewHighScoreChange += OnNewHighScoreChange;
        
        LoadHighScore();
        OnAllHighScoreDataGathered?.Invoke(this, new OnAllHighScoreDataGatheredArgs { AllHighScore = data.currentHighScore });
    }
    
    

    private void OnNewHighScoreChange(object sender, GameControllerScript.OnHighScoreDataGatheredArgs e)
    {
        AddNewHighScore(e.nickName, e.newHighScore);

        SaveHighScore(data);

        OnHighScoreDataGathered?.Invoke(
            this,
            new OnHighScoreDataGatheredArgs { highScore = data.currentHighScore[0].score }
        );
    }
    
    public void ClearHighScoreJSON()
    {
        data.currentHighScore = GetDefaultHighScores();
    
        SaveHighScore(data);
        OnAllHighScoreDataGathered?.Invoke(this, new OnAllHighScoreDataGatheredArgs { AllHighScore = data.currentHighScore });
    }
    
    public void BackToMainMenu()
    {
       gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        GameControllerScript.OnNewHighScoreChange -= OnNewHighScoreChange;
    }

    private void AddNewHighScore(string name, int score)
    {
        List<HighScoreEntry> scores = new List<HighScoreEntry>(data.currentHighScore);

        scores.Add(new HighScoreEntry(name, score));

        scores.Sort((a, b) => b.score.CompareTo(a.score));

        scores.RemoveRange(5, scores.Count - 5);

        data.currentHighScore = scores.ToArray();
    }

    private HighScoreEntry[] SortAllScores()
    {
        Array.Sort(data.currentHighScore, (a, b) => b.score.CompareTo(a.score));

        return data.currentHighScore;
    }

    private void SaveHighScore(HighScoreData newHighScore)
    {
        string json = JsonUtility.ToJson(newHighScore, true);

        using (StreamWriter writer = new StreamWriter(path))
        {
            writer.Write(json);
        }
    }

    private void LoadHighScore()
    {
        if (!File.Exists(path))
        {
            data.currentHighScore = GetDefaultHighScores();
            SaveHighScore(data);
        }

        string json = string.Empty;

        using (StreamReader reader = new StreamReader(path))
        {
            json = reader.ReadToEnd();
        }

        data = JsonUtility.FromJson<HighScoreData>(json) ?? new HighScoreData();

        data.currentHighScore ??= GetDefaultHighScores();

        while (data.currentHighScore.Length < 5)
        {
            List<HighScoreEntry> scores = new List<HighScoreEntry>(data.currentHighScore);
            scores.Add(new HighScoreEntry("---", 0));
            data.currentHighScore = scores.ToArray();
        }

        data.currentHighScore = SortAllScores();

        SaveHighScore(data);

        OnHighScoreDataGathered?.Invoke(
            this,
            new OnHighScoreDataGatheredArgs { highScore = data.currentHighScore[0].score }
        );
    }

    private HighScoreEntry[] GetDefaultHighScores()
    {
        return new HighScoreEntry[]
        {
            new HighScoreEntry("AAA", 0),
            new HighScoreEntry("BBB", 0),
            new HighScoreEntry("CCC", 0),
            new HighScoreEntry("DDD", 0),
            new HighScoreEntry("EEE", 0)
        };
    }
}