using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BackKeyHandler : MonoBehaviour {

    public bool tryBackAnimation = false;
    public UnityEvent OnBackKey;

    void Awake()
    {
        if (OnBackKey == null)
            OnBackKey = new UnityEvent();
    }

    void Update ()
    {
        if (IsInteractable() && Input.GetKeyUp(KeyCode.Escape))
	    {
            OnBackKey.Invoke();
	        if (tryBackAnimation)
	        {
	            BackAnimation();
	        }
        }
	}

    void BackAnimation()
    {
        var buttons = gameObject.GetComponentsInChildren<Button>();
        foreach (var btn in buttons)
        {
            if (btn.tag == "BackBtn")
            {
                btn.onClick.Invoke();
                break;
            }
        }
    }

    bool IsInteractable()
    {
        var canvasGroup = GetComponent<CanvasGroup>();
        return (canvasGroup == null || canvasGroup.interactable);
    }
}
