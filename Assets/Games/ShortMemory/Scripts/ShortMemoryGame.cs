using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class ShortMemoryGame : MonoBehaviour
{
    [SerializeField]
    private GameFinished gameFinished;

    [SerializeField]
    private ShortMemoryEngine gameEngine;

    [SerializeField]
    private GamePaused gamePaused;

    [SerializeField]
    private Text scoreTxt;

    [SerializeField]
    private Text levelTxt;

    [SerializeField]
    private Text timerTxt;

    [SerializeField]
    private Transform buttonsHolder;

    [SerializeField]
    private Button button;

    private List<Button> gameButtons;
    private List<int> hiddenButtonIndexes;
    private List<Button> hiddenButtons;

    [SerializeField]
    private GameTimer2 gameTimer;

    [SerializeField]
    private int gamePlayTime;

    private int level;
    private int gridSize;
    private int buttonCount;
    private int hiddenButtonCount;

    private int correctClickCount;
    private int correctCompleted;

    private Sprite[] buttonSprites;

    private bool isGameOver = false;
    private bool isGamePaused = false;

    void Awake()
    {
        Screen.fullScreen = true;

        buttonSprites = Resources.LoadAll<Sprite>("GameAssets/Buttons4");        
        StartNewGame();
    }

    void Update()
    {
        if (!isGameOver && !isGamePaused)
        {
            timerTxt.text = gameTimer.Current.ToString(gameTimer.IsAtWarning ? "0.0" : "0");

            if (gameTimer.IsAtWarning && !gameTimer.Finished)
            {
                //timerTxt.GetComponent<Animator>().Play("Blink", -1, 0);
            }
        }

        if (gameTimer.Finished && !isGameOver)
        {
            //timerTxt.GetComponent<Animator>().Stop();
            timerTxt.text = "0";

            isGameOver = true;
            EraseButtons();
            gameFinished.ShowGameFinishedPanel(gameEngine.Score);
        }
    }

    public void PauseGame()
    {
        if (!isGamePaused)
        {
            isGamePaused = true;
            gameTimer.PauseTimer();
            gamePaused.ShowGamePausedPanel();
        }
        else
        {
            isGamePaused = false;
            gameTimer.StartTimer(1f);
            gamePaused.HideGamePausedPanel();
        }
    }

    public void RestartGame()
    {
        StartCoroutine(StartGame());        
    }


    private void StartNewGame()
    {
        this.level = 1;
        isGameOver = false;
        isGamePaused = false;
        gameEngine.ResetScore();
        UpdateLevelInfo();        

        this.correctClickCount = 0;
        this.correctCompleted = 0;
        this.gameTimer.ResetTimer(gamePlayTime);

        EraseButtons();
        
        LayoutButtons();

        CreateHiddenButtons(this.hiddenButtonCount);

        ResetScore();

        StartCoroutine(HighlightButtonsOnStart());

        gameTimer.StartTimer(1);
        EnableButtons();
    }

    private void UpdateLevelInfo()
    {
        var levelInfo = gameEngine.GetLevelData(this.level);

        this.gridSize = levelInfo.GridSize;
        this.hiddenButtonCount = levelInfo.HiddenButtonCount;
        this.buttonCount = levelInfo.ButtonCount;
    }

    private void ResetScore()
    {
        scoreTxt.text = "0";
        levelTxt.text = "1";
    }

    private void CreateHiddenButtons(int hiddenCount)
    {
        for (int i = 0; i < hiddenCount; i++)
        {
            int rnd = Random.Range(0, buttonCount);

            while (hiddenButtonIndexes.Contains(rnd))
            {
                rnd = Random.Range(0, buttonCount);
            }

            hiddenButtonIndexes.Add(rnd);

            hiddenButtons.Add(this.gameButtons[rnd]);
        }
    }

    private void EraseButtons()
    {
        foreach (Transform item in buttonsHolder)
        {
            Destroy(item.gameObject);
        }

        gameButtons = new List<Button>();
        hiddenButtonIndexes = new List<int>();
        hiddenButtons = new List<Button>();
    }

    private void LayoutButtons()
    {
        for (int i = 0; i < this.buttonCount; i++)
        {
            Button btn = Instantiate(button);
            btn.gameObject.name = i.ToString();           
        
            btn.gameObject.transform.SetParent(buttonsHolder, false);

            btn.image.sprite = buttonSprites[5];

            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => OnButtonClick());
            btn.gameObject.SetActive(true);
            btn.interactable = false;
            gameButtons.Add(btn);
        }

        var grid = buttonsHolder.GetComponent<GridLayoutGroup>();
        grid.constraintCount = this.gridSize;
        
        if (level >= 15)
        {
            grid.spacing = new Vector2(8, 8);
            grid.cellSize = new Vector2(145, 145);
        }
        else if (level >= 10)
        {
            grid.spacing = new Vector2(10, 10);
            grid.cellSize = new Vector2(165, 165);
        }
    }

    private void OnButtonClick()
    {
        var clickedIndex = int.Parse(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name);
        var clickedButton = gameButtons[clickedIndex];

        if (hiddenButtonIndexes.Contains(clickedIndex))
        {
            correctClickCount++;

            hiddenButtons.Remove(clickedButton);            

            if (correctClickCount == hiddenButtonIndexes.Count)
            {
                clickedButton.image.sprite = this.buttonSprites[1];
                clickedButton.onClick.RemoveAllListeners();
                StartCoroutine(StartNewGameAfterSuccess());
            }
            else
            {
                clickedButton.image.sprite = this.buttonSprites[0];
                clickedButton.onClick.RemoveAllListeners();
            }
        }
        else
        {
            clickedButton.image.sprite = this.buttonSprites[4];

            StartCoroutine(StartNewGameAfterFailure());
        }
    }

    public void StartGameWithLevelUp()
    {
        EraseButtons();

        correctCompleted++;       

        level++;       

        var res = int.Parse(scoreTxt.text);
        //scoreTxt.text = (res + this.gameEngine.CalculateScore(true, this.level, correctClickCount)).ToString();
        scoreTxt.text = this.gameEngine.Score.ToString();
        levelTxt.text = this.level.ToString();

        correctClickCount = 0;

        if (isGameOver)
        {
            return;
        }

        UpdateLevelInfo();

        LayoutButtons();
        CreateHiddenButtons(this.hiddenButtonCount);
        StartCoroutine(HighlightButtonsOnStart());
    }

    public void StartGameWithLevelDown()
    {
        EraseButtons();

        if (correctCompleted > 0)
        {
            correctCompleted = 0;
        }

        var res = int.Parse(scoreTxt.text);

        //var positiveResult = res - this.gameEngine.CalculateScore(false, this.level, correctClickCount);
        scoreTxt.text = this.gameEngine.Score.ToString();
        //scoreTxt.text = (positiveResult < 0 ? 0 : positiveResult).ToString();
        levelTxt.text = this.level.ToString();

        correctClickCount = 0;

        if (isGameOver)
        {
            return;
        }

        UpdateLevelInfo();
   
        LayoutButtons();
        CreateHiddenButtons(this.hiddenButtonCount);
        StartCoroutine(HighlightButtonsOnStart());
    }    

    IEnumerator HighlightButtonsOnStart()
    {
        yield return new WaitForSeconds(0.4f);

        foreach (var item in hiddenButtons)
        {
            item.image.sprite = this.buttonSprites[0];
        }

        yield return new WaitForSeconds(0.8f);

        foreach (var item in hiddenButtons)
        {
            item.image.sprite = this.buttonSprites[5];
        }

        EnableButtons();
    }

    IEnumerator StartNewGameAfterFailure()
    {
        DisableButtons();

        this.gameEngine.CalculateScore(false, this.level, correctClickCount);

        foreach (var item in hiddenButtons)
        {
            item.image.sprite = this.buttonSprites[2];
        }

        yield return new WaitForSeconds(1);

        StartGameWithLevelDown();
    }

    IEnumerator StartNewGameAfterSuccess()
    {
        DisableButtons();

        this.gameEngine.CalculateScore(true, this.level, correctClickCount);

        yield return new WaitForSeconds(0.7f);

        StartGameWithLevelUp();
    }

    IEnumerator StartGame()
    {
        if (isGamePaused)
        {
            gamePaused.HideGamePausedPanel();
        }
        else
        {
            gameFinished.HideGameFinishedPanel();
        }

        yield return new WaitForSeconds(1f);

        StartNewGame();
    }

    private void EnableButtons()
    {
        foreach (var item in gameButtons)
        {
            item.interactable = true;
        }
    }

    private void DisableButtons()
    {
        foreach (var item in gameButtons)
        {
            item.interactable = false;
        }
    }
}
