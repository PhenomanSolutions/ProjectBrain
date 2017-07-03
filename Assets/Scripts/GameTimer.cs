using UnityEngine;

public class GameTimer : MonoBehaviour
{
    public int StartTime = 0;

    public int Warning = 10;

    public float Current { get; private set; }
    public bool Finished { get; private set; }

    public bool IsAtWarning
    {
        get { return Current <= Warning; }
    }

    private bool isActive = false;
    private float _delay = 0;

    public void Start()
    {
        Current = StartTime;
    }

    public void Update()
    {
        if (_delay <= 0)
        {
            if (isActive && Current > 0)
            {
                Finished = false;
                Current -= Time.deltaTime;
            }
            else if (Current <= 0)
            {
                Current = 0;
                Finished = true;
                isActive = false;
            }
        }
        else
        {
            _delay -= Time.deltaTime;
        }
    }

    public void BeginTimer(float delay = 0)
    {
        _delay = delay;
        isActive = true;
    }

    public void StopTimer()
    {
        isActive = false;
        Current = StartTime;
    }

    public void PauseTimer()
    {
        isActive = false;
    }

    public void ResetTimer(int time = 0)
    {
        if (time > 0)
        {
            StartTime = time;
        }
        Finished = false;
        Current = StartTime;
    }
}
