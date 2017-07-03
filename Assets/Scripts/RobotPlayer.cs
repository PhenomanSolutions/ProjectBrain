using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SocialPlatforms;
using Random = System.Random;

public class RobotPlayer : MonoBehaviour
{
    #region PUBLIC
    /// <summary>
    /// Answering frequency: seconds
    /// </summary>
    public int Frequency = 3;

    public int FrequencyFault = 0;

    public bool Enabled = false;

    public GameTimer timer;

    [Range(0, 100)]
    public float probability = 50.0f;

    public UnityEvent OnCorrect;

    public UnityEvent OnIncorrect;
    #endregion

    #region PRIVATE
    private float lastPlayed = 0;

    private float currentDistance = 0;

    private float nestAnswerInSeconds;

    private Random random = new Random();

    private int correct = 0;

    private int inCorrect = 0;
    #endregion

    void Start()
    {
        nestAnswerInSeconds = Frequency;
    }

    void Awake()
    {
        if (OnCorrect == null)
            OnCorrect = new UnityEvent();

        if (OnIncorrect == null)
            OnIncorrect = new UnityEvent();
    }

    void Update()
    {
        if (Enabled && !timer.Finished && timer.Current > 1)
        {
            currentDistance += Time.deltaTime;
            if (Mathf.Abs(lastPlayed - currentDistance) >= nestAnswerInSeconds)
            {
                var min = Frequency - FrequencyFault;
                nestAnswerInSeconds = random.Next((min < 0 ? 0 : min), Frequency + FrequencyFault);
                lastPlayed = currentDistance;

                if (chooseAnswer())
                {
                    _onCorrect();
                    Debug.Log("Correct: #" + (correct + inCorrect));
                }
                else
                {
                    _onIncorrect();
                    Debug.Log("InCorrect: #" + (correct + inCorrect));
                }
            }
        }
    }

    private bool chooseAnswer()
    {
        return probability >= 100 || (probability > random.Next(100));
    }

    private void _onCorrect()
    {
        correct++;
        OnCorrect.Invoke();
    }

    private void _onIncorrect()
    {
        inCorrect++;
        OnIncorrect.Invoke();
    }
}
