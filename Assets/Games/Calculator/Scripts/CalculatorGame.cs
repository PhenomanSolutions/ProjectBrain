using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CalculatorGame : MonoBehaviour
{

    public Text timerText;
    public Text equationText;
    public Text scoresText;
    public Text levelText;
    public Text progressPlaceholder;
    public GameObject statusIndicator;
    public GameObject textProgress;
    public GameObject gameContainer;
    public GameObject gameOverContainer;
    public GameObject menuContainer;
    public CalculatorEngine gameEngine;
    public GameTimer gameTimer;
    public int StartTime = 120;

    private ScoresIOManager scoresIOManager;
    private bool timerBlinking;
    private CalculatorEngine.Equation currentEquation = null;
    private string currentEquationStr = null;
    private const string defaultUserAnswer = "?";
    private string currentHumanAnswer = defaultUserAnswer;

    private bool gameOverPlayed = false;
    private bool IsGameOver = false;
    private bool IsPaused = false;

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
                equationText.text = currentEquationStr + currentHumanAnswer;
                EventSystem.current.SetSelectedGameObject(gameObject);
            }
            else
            {
                if (!IsPaused)
                {
                    IsGameOver = true;
                }
            }
        }
        else
        {
            if (IsGameOver && !gameOverPlayed) //If GameOver run only once
            {
                gameOverPlayed = true;
                gameContainer.GetComponent<Animator>().SetTrigger("GameOver");

                gameOverContainerVisibility(true);

                scoresIOManager.SubmitNewScore("calculator", new ScoresIOManager.ScoreItem
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

        currentEquation = null;
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
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            HumanEnterNumber(-1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha0) || Input.GetKeyDown(KeyCode.Keypad0))
        {
            HumanEnterNumber(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            HumanEnterNumber(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            HumanEnterNumber(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
        {
            HumanEnterNumber(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
        {
            HumanEnterNumber(4);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5))
        {
            HumanEnterNumber(5);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6))
        {
            HumanEnterNumber(6);
        }
        if (Input.GetKeyDown(KeyCode.Alpha7) || Input.GetKeyDown(KeyCode.Keypad7))
        {
            HumanEnterNumber(7);
        }
        if (Input.GetKeyDown(KeyCode.Alpha8) || Input.GetKeyDown(KeyCode.Keypad8))
        {
            HumanEnterNumber(8);
        }
        if (Input.GetKeyDown(KeyCode.Alpha9) || Input.GetKeyDown(KeyCode.Keypad9))
        {
            HumanEnterNumber(9);
        }
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            HumanAnswer();
        }
    }

    void UpdateEquation()
    {
        if (currentEquation == null)
        {
            currentHumanAnswer = defaultUserAnswer;
            currentEquation = gameEngine.GetEquation();
            currentEquationStr = String.Format("{0} = ", currentEquation);
        }
    }

    public void HumanEnterNumber(int value)
    {
        if (value >= 0)
        {
            if (currentHumanAnswer.Length < 5)
            {
                if (currentHumanAnswer == defaultUserAnswer || currentHumanAnswer == "0")
                {
                    currentHumanAnswer = value.ToString();
                }
                else
                {
                    currentHumanAnswer += value.ToString();
                }
            }
        }
        else
        {
            if (currentHumanAnswer.Length > 0)
            {
                currentHumanAnswer = currentHumanAnswer.Remove(currentHumanAnswer.Length - 1, 1);
            }
        }

        currentHumanAnswer = currentHumanAnswer.Trim();

        if (currentHumanAnswer.Length == 0)
        {
            currentHumanAnswer = defaultUserAnswer;
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
    public void HumanAnswer()
    {
        applyAnswer(currentHumanAnswer == defaultUserAnswer ? -1 : int.Parse(currentHumanAnswer));
    }

    private void applyAnswer(int answer)
    {
        var result = (int) gameEngine.ApplyAnswer(currentEquation, answer);

        AnimateScoreProgress(result);
        AnimateScoreIndicator(result > 0);
        currentEquation = null;
    }

    public void HumanAnswer(int answer)
    {
        applyAnswer(answer);
    }

    public void MachineAnswer(bool status)
    {
        var answer = currentEquation.Answer;
        if (!status)
        {
            answer++;
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
