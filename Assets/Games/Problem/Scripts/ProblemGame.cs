using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = System.Random;

public class ProblemGame : MonoBehaviour
{

    public Text timerText;
    public Text scoresText;
    public Text levelText;
    public Text progressPlaceholder;
    public GameObject textProgress;
    public GameObject gameContainer;
    public GameObject gameOverContainer;
    public GameObject statusIndicator;
    public GameObject menuContainer;
    public ProblemEngine gameEngine;
    public GameTimer gameTimer;
    public int StartTime = 120;

    public Text signText;
    public Text answer1Text;
    public Text answer2Text;

    private bool timerBlinking;
    private ScoresIOManager scoresIOManager;
    private List<CalculatorEngine.Equation> currentEquations = null;
    private bool signGreaterThan = false;

    private bool gameOverPlayed = false;
    private bool IsGameOver = false;
    private bool IsPaused = false;
    private Random r = new Random();

    // Use this for initialization
    void Start()
    {
        if (!gameTimer)
        {
            throw new Exception("Game Timer is REQUIRED field.");
        }
        if (!gameEngine)
        {
            throw new Exception("Game Engine is REQUIRED field.");
        }

        scoresIOManager = GameObject.Find("ScoresIOManager").GetComponent<ScoresIOManager>();

        NewGame();
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsGameOver && !IsPaused)
        {
            if (UpdateTimer())
            {
                if (levelText)
                {
                    levelText.text = gameEngine.Level.ToString(CultureInfo.InvariantCulture);
                }

                if (scoresText)
                {
                    scoresText.text = gameEngine.Scores.ToString(CultureInfo.InvariantCulture);
                }

                UpdateEquation();
                HandleKeyboardInput();

                signText.text = signGreaterThan ? "GREATER" : "LOWER";
                if (currentEquations != null && currentEquations.Count == 2)
                {
                    answer1Text.text = currentEquations[0].ToString();
                    answer2Text.text = currentEquations[1].ToString();
                }

                EventSystem.current.SetSelectedGameObject(gameObject);
            }
            else {
                if (!IsPaused && !gameOverPlayed)
                {
                    IsGameOver = true;
                }
            }
        }
        else
        {
            if (IsGameOver && !gameOverPlayed)
            {
                gameOverPlayed = true;
                gameContainer.GetComponent<Animator>().SetTrigger("GameOver");

                gameOverContainerVisibility(true);

                scoresIOManager.SubmitNewScore("problem", new ScoresIOManager.ScoreItem()
                {
                    Level = gameEngine.Level,
                    Scores = gameEngine.Scores,
                    DateTime = DateTime.Now,
                });
            }
        }
    }

    public void Menu(bool status)
    {
        menuContainer.GetComponent<Animator>().SetTrigger(status ? "SlideIn" : "SlideOut");
        gameContainer.GetComponent<Animator>().SetTrigger(status ? "GameOver" : "NewGame");

        IsPaused = status;
        if (status)
        {
            gameTimer.PauseTimer();
        }
        else
        {
            gameTimer.BeginTimer();
        }
    }

    public void ToggleMenu()
    {
        Menu(!IsPaused);
    }

    private void gameOverContainerVisibility(bool status)
    {
        if (status)
        {
            gameOverContainer.GetComponent<CanvasGroup>().alpha = 0;
            gameOverContainer.SetActive(true);
            gameOverContainer.transform.FindChild("FinalScores").GetComponent<Text>().text =
                gameEngine.Scores.ToString(CultureInfo.InvariantCulture);
            gameOverContainer.GetComponent<Animator>().SetTrigger("FadeIn");
        }
        else
        {
            gameOverContainer.SetActive(false);
            gameOverContainer.GetComponent<CanvasGroup>().alpha = 0;
        }
    }

    public void NewGame()
    {
        if (IsPaused)
        {
            Menu(false);
        }

        gameEngine.NewGame();
        gameTimer.ResetTimer(StartTime);

        if (gameOverPlayed)
        {
            gameOverContainerVisibility(false);
            gameContainer.GetComponent<Animator>().SetTrigger("NewGame");
            gameOverPlayed = false;
        }

        currentEquations = null;
        UpdateEquation();
        gameTimer.BeginTimer(1);
        IsGameOver = false;
    }

    bool UpdateTimer()
    {
        timerText.text = gameTimer.Current.ToString(gameTimer.IsAtWarning ? "0.0" : "0");
        timerText.color = gameTimer.IsAtWarning ? Color.red : Color.white;

        if (!timerBlinking && gameTimer.IsAtWarning && !gameTimer.Finished)
        {
            timerBlinking = true;
            timerText.GetComponent<Animator>().SetTrigger("Blink");
        }
        else if ((timerBlinking && !gameTimer.IsAtWarning) || (timerBlinking && gameTimer.Finished))
        {
            timerBlinking = false;
            timerText.GetComponent<Animator>().SetTrigger("Idle");
        }

        return !gameTimer.Finished;
    }

    void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            HumanAnswer(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
        {
            HumanAnswer(4);
        }
        if (Input.GetKeyDown(KeyCode.Alpha7) || Input.GetKeyDown(KeyCode.Keypad7))
        {
            HumanAnswer(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha9) || Input.GetKeyDown(KeyCode.Keypad9))
        {
            HumanAnswer(2);
        }
    }

    void UpdateEquation()
    {
        if (currentEquations == null)
        {
            currentEquations = gameEngine.GetEquations();
            signGreaterThan = (r.Next(100)%2 == 0);
        }
    }
    
    private string toStringAction(CalculatorEngine.MathActions action)
    {
        switch (action)
        {
            case CalculatorEngine.MathActions.Plus:
                return "+";
            case CalculatorEngine.MathActions.Minus:
                return "-";
            case CalculatorEngine.MathActions.Divide:
                return "/";
            case CalculatorEngine.MathActions.Multiply:
                return "*";
            default: return string.Empty;
        }
    }

    #region ANSWERING
    private void applyAnswer(int answer)
    {
        if (answer > 0 && answer < 5)
        {
            var result = (int) gameEngine.ApplyAnswer(currentEquations, signGreaterThan, currentEquations[answer - 1].Answer);
            AnimateScoreProgress(result);
            AnimateScoreIndicator(result > 0);
            currentEquations = null;
        }
    }

    public void HumanAnswer(int answer)
    {
        applyAnswer(answer);
    }

    public void MachineAnswer(bool status)
    {
        int answer = 0;
        if (status)
        {
            answer = signGreaterThan ? currentEquations.Max(p => p.Answer) : currentEquations.Min(p => p.Answer);
        }
        else
        {
            answer = signGreaterThan ? currentEquations.Min(p => p.Answer) : currentEquations.Max(p => p.Answer);
        }

        applyAnswer(answer);
    }

    public void AnimateScoreProgress(int scores)
    {
        if (progressPlaceholder)
        {
            var textObject = Instantiate(textProgress, gameObject.transform.parent) as GameObject;

            var textComponent = textObject.GetComponentInChildren<Text>().GetComponent<Text>();
            textComponent.color = (scores < 0 ? Color.red : Color.green);
            textComponent.text = String.Format("{0}{1}", (scores < 0 ? "-" : "+"), Math.Abs(scores));
            textComponent.transform.position = progressPlaceholder.transform.position;

            Destroy(textObject, 5);
        }
    }

    public void AnimateScoreIndicator(bool status)
    {
        if (statusIndicator)
        {
            var animator = statusIndicator.GetComponent<Animator>();
            if (animator)
            {
                animator.SetTrigger(status ? "Correct" : "Incorrect");
                animator.SetTrigger("Idle");
            }
        }
    }
    #endregion
}
