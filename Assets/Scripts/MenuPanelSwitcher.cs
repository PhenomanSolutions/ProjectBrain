using UnityEngine;
using UnityEngine.UI;

public class MenuPanelSwitcher : MonoBehaviour
{

    public GameObject PanelToClose;
    public GameObject PanelToShow;
    public string ShowTrigger;
    public string CloseTrigger;

	void Start () {
        var btn = GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);
    }

    void TaskOnClick()
    {
        setPanelState(PanelToClose, CloseTrigger, false);
        setPanelState(PanelToShow, ShowTrigger, true);
    }

    void setPanelState(GameObject obj, string animation, bool setToActive)
    {
        if (obj)
        {
            if (string.IsNullOrEmpty(animation))
            {
                obj.SetActive(false);
            }
            else
            {
                triggerAnimation(obj, animation);
            }
            setInteractable(obj, setToActive);
        }
    }

    void triggerAnimation(GameObject obj, string name)
    {
        var animator = obj.GetComponent<Animator>();
        if (animator)
        {
            animator.SetTrigger(name);
        }
    }

    void setInteractable(GameObject obj, bool state)
    {
        var canvasGroup = obj.GetComponent<CanvasGroup>();
        if (canvasGroup)
        {
            canvasGroup.interactable = state;
        }
    }
}
