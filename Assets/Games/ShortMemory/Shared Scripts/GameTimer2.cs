using UnityEngine;

public class GameTimer2 : MonoBehaviour
{
    private int startTime = 0;
    private int warning = 10;
    private bool isActive = false;
    private float delay = 0;

    public float Current { get; private set; }
    public bool Finished { get; private set; }

    public bool IsAtWarning
    {
        get { return Current <= warning; }
    }

    void Update()
    {
        if (delay <= 0)
        {
            if (isActive && Current > 0)
            {
                Finished = false;
                Current -= Time.deltaTime;
            }
            else if (Current <= 0)
            {
                Finished = true;
                isActive = false;
                Current = 0;
            }
        }
        else
        {
            delay -= Time.deltaTime;
        }
    }

    public void StartTimer(float delay)
    {
        this.delay = delay;
        isActive = true;
    }

    public void PauseTimer()
    {
        isActive = false;
    }

    public void ResetTimer(int time)
    {
        if (time > 0)
        {
            startTime = time;
        }
        Finished = false;
        Current = startTime;
    }
}
