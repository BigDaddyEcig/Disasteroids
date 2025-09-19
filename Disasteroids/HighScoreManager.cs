using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Disasteroids
{
    public class HighScoreEntry
    {
        public string PlayerName { get; set; } = "";
        public int Score { get; set; }
        public DateTime Date { get; set; }
        public int Level { get; set; }
    }
    
    public class HighScoreManager
    {
        private List<HighScoreEntry> highScores;
        private const int MAX_HIGH_SCORES = 10;
        private const string HIGH_SCORE_FILE = "highscores.json";
        
        public HighScoreManager()
        {
            highScores = new List<HighScoreEntry>();
            LoadHighScores();
        }
        
        public void LoadHighScores()
        {
            try
            {
                if (File.Exists(HIGH_SCORE_FILE))
                {
                    string json = File.ReadAllText(HIGH_SCORE_FILE);
                    var loadedScores = JsonSerializer.Deserialize<List<HighScoreEntry>>(json);
                    if (loadedScores != null)
                    {
                        highScores = loadedScores;
                    }
                }
            }
            catch (Exception)
            {
                // If loading fails, start with empty list
                highScores = new List<HighScoreEntry>();
            }
            
            // Ensure we have some default scores if file doesn't exist or is empty
            if (highScores.Count == 0)
            {
                CreateDefaultHighScores();
            }
        }
        
        private void CreateDefaultHighScores()
        {
            highScores.Add(new HighScoreEntry { PlayerName = "ACE", Score = 50000, Date = DateTime.Now.AddDays(-30), Level = 8 });
            highScores.Add(new HighScoreEntry { PlayerName = "PILOT", Score = 40000, Date = DateTime.Now.AddDays(-25), Level = 7 });
            highScores.Add(new HighScoreEntry { PlayerName = "HERO", Score = 30000, Date = DateTime.Now.AddDays(-20), Level = 6 });
            highScores.Add(new HighScoreEntry { PlayerName = "STAR", Score = 25000, Date = DateTime.Now.AddDays(-15), Level = 5 });
            highScores.Add(new HighScoreEntry { PlayerName = "NOVA", Score = 20000, Date = DateTime.Now.AddDays(-10), Level = 4 });
            highScores.Add(new HighScoreEntry { PlayerName = "COMET", Score = 15000, Date = DateTime.Now.AddDays(-8), Level = 4 });
            highScores.Add(new HighScoreEntry { PlayerName = "ROCKET", Score = 12000, Date = DateTime.Now.AddDays(-6), Level = 3 });
            highScores.Add(new HighScoreEntry { PlayerName = "SPACE", Score = 10000, Date = DateTime.Now.AddDays(-4), Level = 3 });
            highScores.Add(new HighScoreEntry { PlayerName = "ORBIT", Score = 8000, Date = DateTime.Now.AddDays(-2), Level = 2 });
            highScores.Add(new HighScoreEntry { PlayerName = "MOON", Score = 5000, Date = DateTime.Now.AddDays(-1), Level = 2 });
        }
        
        public void SaveHighScores()
        {
            try
            {
                string json = JsonSerializer.Serialize(highScores, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(HIGH_SCORE_FILE, json);
            }
            catch (Exception)
            {
                // Ignore save errors - game should continue
            }
        }
        
        public bool IsHighScore(int score)
        {
            return highScores.Count < MAX_HIGH_SCORES || score > highScores.Min(h => h.Score);
        }
        
        public int GetHighScoreRank(int score)
        {
            var sortedScores = highScores.OrderByDescending(h => h.Score).ToList();
            for (int i = 0; i < sortedScores.Count; i++)
            {
                if (score > sortedScores[i].Score)
                {
                    return i + 1; // 1-based ranking
                }
            }
            return sortedScores.Count + 1;
        }
        
        public void AddHighScore(string playerName, int score, int level)
        {
            var newEntry = new HighScoreEntry
            {
                PlayerName = playerName.ToUpper(),
                Score = score,
                Date = DateTime.Now,
                Level = level
            };
            
            highScores.Add(newEntry);
            
            // Sort by score (descending) and keep only top scores
            highScores = highScores.OrderByDescending(h => h.Score).Take(MAX_HIGH_SCORES).ToList();
            
            SaveHighScores();
        }
        
        public List<HighScoreEntry> GetHighScores()
        {
            return highScores.OrderByDescending(h => h.Score).ToList();
        }
        
        public int GetHighestScore()
        {
            return highScores.Count > 0 ? highScores.Max(h => h.Score) : 0;
        }
        
        public void ClearHighScores()
        {
            highScores.Clear();
            CreateDefaultHighScores();
            SaveHighScores();
        }
    }
}
