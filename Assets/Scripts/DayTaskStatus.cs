using System;
using UnityEngine;
using Random = System.Random;

public class DayTaskStatus : MonoBehaviour
{

    public GameObject DayPrefab;
    public int DaysCount = 7;

    private DayOfWeek todaysDay = DateTime.Now.DayOfWeek;

    void Start()
    {
        foreach (Transform child in gameObject.transform)
        {
            Destroy(child.gameObject);
        }

        var currentDay = todaysDay;
        var r = new Random();

        for (var i = 0; i < DaysCount; i++)
        {
            currentDay = getNextDay(currentDay);
            var dayItem = Instantiate(DayPrefab, gameObject.transform, false);
            dayItem.name = currentDay.ToString();
            dayItem.GetComponent<DayItem>().DayName = currentDay;

            if (currentDay == todaysDay)
            {
                dayItem.GetComponent<DayItem>().Status = DayItem.DaylyTaskState.Today;
            }
            else
            {
                dayItem.GetComponent<DayItem>().Status = r.Next(100) % 2 == 0 ? DayItem.DaylyTaskState.Done : DayItem.DaylyTaskState.NotDone;
            }
        }
    }

    DayOfWeek getNextDay(DayOfWeek day)
    {
        switch (day)
        {
            case DayOfWeek.Monday: return DayOfWeek.Tuesday;
            case DayOfWeek.Tuesday: return DayOfWeek.Wednesday;
            case DayOfWeek.Wednesday: return DayOfWeek.Thursday;
            case DayOfWeek.Thursday: return DayOfWeek.Friday;
            case DayOfWeek.Friday: return DayOfWeek.Saturday;
            case DayOfWeek.Saturday: return DayOfWeek.Sunday;
            case DayOfWeek.Sunday: return DayOfWeek.Monday;
            default: return DayOfWeek.Monday;
        }
    }

	void Update () {
		
	}
}
