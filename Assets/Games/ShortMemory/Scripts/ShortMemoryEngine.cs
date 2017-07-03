using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShortMemoryEngine : MonoBehaviour
{
    private const double CorrectValue = 25;
    private const double InCorrectValue = 15;
    private const int minimalAnswerValue = 100;
    private int comboAnswerCount;

    public int Score { get; private set; }

    public void ResetScore()
    {
        Score = 0;
    }

    public LevelInfo GetLevelData(int level)
    {
        var hiddenButtonCount = 0;
        var gridSize = 0;
        var buttonCount = 0;

        switch (level)
        {
            case 1:
                hiddenButtonCount = 2;
                buttonCount = 9;
                gridSize = 3;
                break;
            case 2:
                hiddenButtonCount = 3;
                buttonCount = 9;
                gridSize = 3;
                break;
            case 3:
                hiddenButtonCount = 3;
                buttonCount = 16;
                gridSize = 4;
                break;
            case 4:
                hiddenButtonCount = 4;
                buttonCount = 16;
                gridSize = 4;
                break;
            case 5:
                hiddenButtonCount = 5;
                buttonCount = 16;
                gridSize = 4;
                break;
            case 6:
                hiddenButtonCount = 5;
                buttonCount = 25;
                gridSize = 5;
                break;
            case 7:
                hiddenButtonCount = 6;
                buttonCount = 25;
                gridSize = 5;
                break;
            case 8:
                hiddenButtonCount = 7;
                buttonCount = 25;
                gridSize = 5;
                break;
            case 9:
                hiddenButtonCount = 8;
                buttonCount = 25;
                gridSize = 5;
                break;
            case 10:
                hiddenButtonCount = 8;
                buttonCount = 36;
                gridSize = 6;
                break;
            case 11:
                hiddenButtonCount = 9;
                buttonCount = 36;
                gridSize = 6;
                break;
            case 12:
                hiddenButtonCount = 10;
                buttonCount = 36;
                gridSize = 6;
                break;
            case 13:
                hiddenButtonCount = 11;
                buttonCount = 36;
                gridSize = 6;
                break;
            case 14:
                hiddenButtonCount = 12;
                buttonCount = 36;
                gridSize = 6;
                break;
            case 15:
                hiddenButtonCount = 12;
                buttonCount = 49;
                gridSize = 7;
                break;
            case 16:
                hiddenButtonCount = 13;
                buttonCount = 49;
                gridSize = 7;
                break;
            case 17:
                hiddenButtonCount = 14;
                buttonCount = 49;
                gridSize = 7;
                break;
            case 18:
                hiddenButtonCount = 15;
                buttonCount = 49;
                gridSize = 7;
                break;
            case 19:
                hiddenButtonCount = 16;
                buttonCount = 49;
                gridSize = 7;
                break;
            case 20:
                hiddenButtonCount = 17;
                buttonCount = 49;
                gridSize = 7;
                break;
        }

        return new LevelInfo { ButtonCount = buttonCount, GridSize = gridSize, HiddenButtonCount = hiddenButtonCount };
    }

    public class LevelInfo
    {
        public int HiddenButtonCount { get; set; }
        public int GridSize { get; set; }
        public int ButtonCount { get; set; }
    }

    public int CalculateScore(bool isCorrect, int level, int guessedCount)
    {
        var result = 0d;

        if (isCorrect)
        {
            var val = level * CorrectValue + Math.Sqrt(guessedCount) * 10;
            if (val < minimalAnswerValue)
            {
                val = minimalAnswerValue;
            }
            val += Math.Sqrt(comboAnswerCount);
            result = val;            

            comboAnswerCount += 1;

            Score += Convert.ToInt32(result);
        }
        else
        {
            comboAnswerCount = 0;

            result = level * InCorrectValue - Math.Sqrt(guessedCount) * 10;

            Score -= (int)result;

            Score = (Score < 0 ? 0 : Score);
        }
        
        return Convert.ToInt32(result);
    }
}
