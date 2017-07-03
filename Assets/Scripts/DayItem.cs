using System;
using UnityEngine;
using UnityEngine.UI;

public class DayItem : MonoBehaviour
{
    #region ENUMS
    public enum DaylyTaskState
    {
        Done = 1,
        Today,
        Pending,
        NotDone
    }
    #endregion

    public DayOfWeek DayName;
    public DaylyTaskState Status = DaylyTaskState.Pending;

    public Color pendingColor = new Color(1, 1, 1, 0.3f);
    public Color doneColor = new Color(0.721f, 0.968f, 0.349f, 1);
    public Color notDoneColor = new Color(0.968f, 0.349f, 0.349f, 1);
    public Color todayColor = new Color(1, 1, 1, 1);

    public Sprite DoneIcon;
    public Sprite NotDoneIcon;
    public Sprite PendingIcon;
    public Sprite TodayIcon;

    private Image imageSource;
    private Image iconImageSource;
    private Text nameText;

    void Start()
    {
        imageSource = GetComponent<Image>();
        nameText = gameObject.GetComponentInChildren<Text>();
        var images = gameObject.GetComponentsInChildren<Image>();
        foreach (var icon in images)
        {
            if (icon.name == "Icon")
            {
                iconImageSource = icon;
                break;
            }
        }
    }

    void Update()
    {
        nameText.text = getDayName();
        imageSource.color = getDayColor();

        if (iconImageSource)
        {
            var icon = getDayIcon();
            if (icon)
            {
                iconImageSource.sprite = icon;
            }
        }
    }

    Color getDayColor()
    {
        switch (Status)
        {
            case DaylyTaskState.Done:
                return doneColor;
            case DaylyTaskState.Pending:
                return pendingColor;
            case DaylyTaskState.NotDone:
                return notDoneColor;
            case DaylyTaskState.Today:
                return todayColor;
            default:
                return pendingColor;
        }
    }

    string getDayName()
    {
        switch (DayName)
        {
            case DayOfWeek.Monday:
                return "MON";
            case DayOfWeek.Tuesday:
                return "TUE";
            case DayOfWeek.Wednesday:
                return "WED";
            case DayOfWeek.Thursday:
                return "THU";
            case DayOfWeek.Friday:
                return "FRI";
            case DayOfWeek.Saturday:
                return "SAT";
            case DayOfWeek.Sunday:
                return "SUN";
            default:
                return "";
        }
    }

    Sprite getDayIcon()
    {
        switch (Status)
        {
            case DaylyTaskState.Done:
                return DoneIcon;
            case DaylyTaskState.Pending:
                return PendingIcon;
            case DaylyTaskState.NotDone:
                return NotDoneIcon;
            case DaylyTaskState.Today:
                return TodayIcon;
            default:
                return PendingIcon;
        }
    }
}
