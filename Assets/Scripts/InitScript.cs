using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InitScript : MonoBehaviour
{
    [Serializable]
    public class ScoreInfoText
    {
        public Text ScoresText;
        public Text LevelText;
        public string Key;
    }

    public ScoreInfoText[] Fields;

    private ScoresIOManager scoresIOManager;

	void Awake () {
	    scoresIOManager = GameObject.Find("ScoresIOManager").GetComponent<ScoresIOManager>();
	}

    void Start()
    {
        foreach (var field in Fields)
        {
            if (!string.IsNullOrEmpty(field.Key))
            {
                var scoresInfo = scoresIOManager.GetMaxScore(field.Key);
                if (field.ScoresText)
                {
                    field.ScoresText.text = scoresInfo != null ? scoresInfo.Scores.ToString("0") : "0";
                }
                if (field.LevelText)
                {
                    field.LevelText.text = scoresInfo != null ? scoresInfo.Level.ToString("0") : "1";
                }
            }
        }
    }
}
