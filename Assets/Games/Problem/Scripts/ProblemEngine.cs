using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProblemEngine : MonoBehaviour {
    
    #region PRIVATE
    private readonly HashSet<string> _history = new HashSet<string>();
    private const double CorrectValue = 250;
    private const double InCorrectValue = 180;
    private DateTime? lastAnswered;
    private double thinkedTime;
    private int comboAnswerCount;
    private int minimalAnswerValue = 10;
    private System.Random random = new System.Random();

    private CalculatorEngine.Equation generate(int level)
    {
        var numberCountLevel = level / 10;
        //var numberDifficultyLevel = level % 10;
        var action = GetAction();

        var leftLength = (action == CalculatorEngine.MathActions.Divide || action == CalculatorEngine.MathActions.Multiply) ? 10 : 100;
        var rightLength = (action == CalculatorEngine.MathActions.Divide || action == CalculatorEngine.MathActions.Multiply) ? 10 : 100;

        while (numberCountLevel-- > 0)
        {
            //Adding number length depending on level
            if (leftLength > rightLength)
            {
                rightLength *= 10;
            }
            else
            {
                leftLength *= 10;
            }
        }

        //var simpleNumber = numberDifficultyLevel < 5;

        CalculatorEngine.Equation eq = null;

        while (eq == null || !eq.IsValid || _history.Contains(eq.ToString()))
        {
            var leftNumber = random.Next(leftLength / 10, leftLength);
            var rightNumber = random.Next(rightLength / 10, rightLength);

            //TODO: FIX THIS PART (HANGING)
            //            while (simpleNumber && (leftNumber % 5 != 0 || rightNumber % 5 != 0))
            //            {
            //                leftNumber = random.Next(leftLength / 10, leftLength);
            //                rightNumber = random.Next(rightLength / 10, rightLength);
            //            }

            eq = new CalculatorEngine.Equation
            {
                A = leftNumber,
                B = rightNumber,
                Action = action
            };
        }

        eq.CalculateAnswer();
        _history.Add(eq.ToString());

        return eq;
    }

    private CalculatorEngine.MathActions GetAction()
    {
        return (CalculatorEngine.MathActions)random.Next(1, 5);
    }

    /// <summary>
    /// Calculate Scores depending on last answer
    /// <remarks>Good to have Scores Calculation independend</remarks> 
    /// </summary>
    /// <param name="lastAnswerCorrect">Last answer</param>
    private double CalculateScores(bool lastAnswerCorrect)
    {
        var result = 0d;

        if (lastAnswerCorrect)
        {
            var val = Math.Abs(CorrectValue - (thinkedTime / 1000)) + Correct;
            if (val < minimalAnswerValue)
            {
                val = minimalAnswerValue;
            }
            val += Combo;
            result = Level * val;
            Scores = (Scores + (int)result);

            comboAnswerCount += 1;
        }
        else
        {
            comboAnswerCount = 0;

            var val = InCorrectValue + (thinkedTime / 1000) + InCorrect;

            result = Level * val;

            result *= -1; //For animation result we need MINUS result
            var _scores = Scores + (int)result;

            Scores = (_scores < 0 ? 0 : _scores);
        }

        return result;
    }

    /// <summary>
    /// Calculates Scores and Level in correct order, depending on last answer
    /// </summary>
    /// <param name="lastAnswerCorrect">Last answer</param>
    private double CalculateScoresAndLevel(bool lastAnswerCorrect)
    {
        var result = CalculateScores(lastAnswerCorrect);
        LevelCalculator.CalculateLevel(Scores);
        return result;
    }
    #endregion

    public GameLevelCalculator LevelCalculator;

    public double Combo
    {
        get { return Math.Sqrt(comboAnswerCount / 3.0); }
    }

    public int Correct { get; private set; }
    public int InCorrect { get; private set; }

    public double Scores { get; private set; }

    public int Level
    {
        get { return LevelCalculator.Level; }
    }

    /// <summary>
    /// Geting Unique and Valid Equation
    /// </summary>
    /// <returns>Equation</returns>
    public List<CalculatorEngine.Equation> GetEquations()
    {
        var arr = new List<CalculatorEngine.Equation>()
        {
            generate(Level)
        };
        
        while (arr.Count < 2)
        {
            var eq = generate(Level);
            if (arr.All(p => p.Answer != eq.Answer))
            {
                arr.Add(eq);
            }
        }
        return arr;
    }

    /// <summary>
    /// Apply user answer
    /// </summary>
    /// <param name="eq">Current Equation</param>
    /// <param name="answer">User answer</param>
    /// <returns>True if answer is correct</returns>
    public double ApplyAnswer(List<CalculatorEngine.Equation> eq, bool greaterThan, int answer)
    {
        if (lastAnswered == null)
        {
            lastAnswered = DateTime.Now;
        }

        thinkedTime = (DateTime.Now - lastAnswered.Value).TotalMilliseconds;
        lastAnswered = DateTime.Now;

        var isCorrect = (greaterThan ? eq.Max(p => p.Answer) : eq.Min(p => p.Answer)) == answer;
        if (isCorrect)
        {
            Correct++;
        }
        else
        {
            InCorrect++;
        }

        return CalculateScoresAndLevel(isCorrect);
    }

    public void NewGame()
    {
        if (!LevelCalculator)
        {
            throw new Exception("LevelCalculator is REQUIRED field.");
        }

        _history.Clear();
        LevelCalculator.StartNew();
        Scores = 0;
        comboAnswerCount = 0;
        Correct = 0;
        InCorrect = 0;
        lastAnswered = null;
    }
}
