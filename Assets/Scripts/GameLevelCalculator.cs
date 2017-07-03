using UnityEngine;

public class GameLevelCalculator : MonoBehaviour {

    #region PRIVATES
    private float Multiplier
    {
        get
        {
            switch (Speed)
            {
                case ChangeSpeed.Slow: return 2.0f;
                case ChangeSpeed.Normal: return 1.5f;
                case ChangeSpeed.Fast: return 1.3f;
                default: return 1.5f;
            }
        }
    }
    #endregion

    public enum ChangeSpeed
    {
        Slow = 1,
        Normal,
        Fast
    }

	public int Level { get; private set; }

    public ChangeSpeed Speed = ChangeSpeed.Normal;
    
    public double UntilNext { get; private set; }

    public void CalculateLevel(double scores)
    {
        while (scores > UntilNext)
        {
            UntilNext *= Multiplier;
            Level++;
        }
    }

    public void StartNew(int start = 1000)
    {
        Level = 1;
        UntilNext = start;
    }
}
