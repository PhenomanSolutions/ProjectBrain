using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class CalculatorEngine : MonoBehaviour
{

    #region ENUMS
    public enum MathActions
    {
        Plus = 1,
        Minus,
        Divide,
        Multiply
    }
    #endregion

    #region CLASSES
    public class Equation
    {
        public int A { get; set; }
        public int B { get; set; }
        public MathActions Action { get; set; }
        public int Answer { get; set; }

        /// <summary>
        /// Check user answer
        /// </summary>
        /// <param name="answer">User answer</param>
        /// <returns>True if answer is correct</returns>
        public bool IsAnswerCorrect(int answer)
        {
            return Answer == answer;
        }

        public int CalculateAnswer()
        {
            switch (Action)
            {
                case MathActions.Plus:
                    Answer = A + B;
                    break;
                case MathActions.Minus:
                    Answer = A - B;
                    break;
                case MathActions.Divide:
                    Answer = A / B;
                    break;
                case MathActions.Multiply:
                    Answer = A * B; ;
                    break;
                default:
                    Answer = 0;
                    break;
            }
            return Answer;
        }

        /// <summary>
        /// check for Integer answer, only Division can have non-int answer
        /// </summary>
        public bool IsValid
        {
            get
            {
                switch (Action)
                {
                    case MathActions.Divide: return (B != 0 && ((A / (double)B) % 1) <= Double.Epsilon);
                    case MathActions.Minus: return A > B;
                    case MathActions.Plus: return true;
                    case MathActions.Multiply: return true;
                    default: return false;
                }
            }
        }

        public string ToDebugString()
        {
            string action;
            switch (Action)
            {
                case CalculatorEngine.MathActions.Plus:
                    action = "+";
                    break;
                case CalculatorEngine.MathActions.Minus:
                    action = "-";
                    break;
                case CalculatorEngine.MathActions.Divide:
                    action = "/";
                    break;
                case CalculatorEngine.MathActions.Multiply:
                    action = "*";
                    break;
                default:
                    action = string.Empty;
                    break;
            }
            return String.Format("{0} {1} {2} = {3}", A, action, B, Answer);
        }

        public override string ToString()
        {
            string action;
            switch (Action)
            {
                case MathActions.Plus:
                    action = "+";
                    break;
                case MathActions.Minus:
                    action = "-";
                    break;
                case MathActions.Divide:
                    action = "/";
                    break;
                case MathActions.Multiply:
                    action = "*";
                    break;
                default:
                    action = string.Empty;
                    break;
            }
            return String.Format("{0} {1} {2}", A, action, B);
        }
    }
    #endregion

    #region PRIVATE
    private readonly HashSet<string> _history = new HashSet<string>();
    private const double CorrectValue = 250;
    private const double InCorrectValue = 180;
    private DateTime? lastAnswered;
    private double thinkedTime;
    private int comboAnswerCount;
    private int minimalAnswerValue = 10;
    private Random random = new Random();

    private Equation generate(int level)
    {
        var numberCountLevel = level / 10;
        //var numberDifficultyLevel = level % 10;
        var action = GetAction();

        var leftLength = (action == MathActions.Divide || action == MathActions.Multiply) ? 10 : 100;
        var rightLength = (action == MathActions.Divide || action == MathActions.Multiply) ? 10 : 100;

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

        Equation eq = null;

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

            eq = new Equation
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

    private MathActions GetAction()
    {
        return (MathActions)random.Next(1, 5);
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
            result = Level*val;
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
    public Equation GetEquation()
    {
        return generate(Level);
    }

    /// <summary>
    /// Apply user answer
    /// </summary>
    /// <param name="eq">Current Equation</param>
    /// <param name="answer">User answer</param>
    /// <returns>True if answer is correct</returns>
    public double ApplyAnswer(Equation eq, int answer)
    {
        if (lastAnswered == null)
        {
            lastAnswered = DateTime.Now;
        }

        thinkedTime = (DateTime.Now - lastAnswered.Value).TotalMilliseconds;
        lastAnswered = DateTime.Now;

        var isCorrect = eq.IsAnswerCorrect(answer);
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