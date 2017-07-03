using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class ScoresIOManager : MonoBehaviour {

    [Serializable]
    public class ScoreItem
    {
        public DateTime DateTime { get; set; }
        public double Scores { get; set; }
        public int Level { get; set; }
        public int Key { get; set; }
    }

    [Serializable]
    public class PlayerInfo
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string DeviceId { get; set; }
        public Dictionary<string, List<ScoreItem>> Scores { get; set; }
    }

    private string settingsPath;

    void Awake()
    {
        settingsPath = String.Format("{0}/playerInfo.dat", Application.persistentDataPath);
    }
    
    private void checkInit(PlayerInfo info = null)
    {
        if (!File.Exists(settingsPath))
        {
            var bf = new BinaryFormatter();
            var file = File.Open(settingsPath, FileMode.OpenOrCreate);
            var playerInfo = info ?? new PlayerInfo
            {
                Firstname = Guid.NewGuid().ToString(),
                Lastname = Guid.NewGuid().ToString(),
                DeviceId = SystemInfo.deviceUniqueIdentifier,
                Scores = new Dictionary<string, List<ScoreItem>>()
            };
            bf.Serialize(file, playerInfo);
            file.Close();
            file.Dispose();
        }
    }

    private PlayerInfo getPlayerInfo()
    {
        var bf = new BinaryFormatter();
        var file = File.Open(settingsPath, FileMode.Open);
        var playerInfo = (PlayerInfo)bf.Deserialize(file);
        file.Close();
        file.Dispose();
        return playerInfo;
    }

    public void SubmitNewScore(string key, ScoreItem scoresInfo)
    {
        checkInit();
        
        var playerInfo = getPlayerInfo();

        if (!playerInfo.Scores.ContainsKey(key))
        {
            playerInfo.Scores.Add(key, new List<ScoreItem>());
        }

        playerInfo.Scores[key].Add(scoresInfo);

        File.Delete(settingsPath);
        checkInit(playerInfo);
    }

    public IEnumerable<ScoreItem> GetScores(string key)
    {
        checkInit();
        var playerInfo = getPlayerInfo();

        return playerInfo.Scores.ContainsKey(key) ? playerInfo.Scores[key] : new List<ScoreItem>();
    }

    public ScoreItem GetMaxScore(string key)
    {
        var scores = GetScores(key).ToList();
        if (scores.Any())
        {
            var maxScore = scores.Max(p => p.Scores);
            return scores.Where(p => Math.Abs(p.Scores - maxScore) < 0.0001)
                .OrderByDescending(p => p.DateTime)
                .FirstOrDefault();
        }
        return null;
    }
}
