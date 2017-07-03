using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePaused : MonoBehaviour
{

    [SerializeField]
    private GameObject gamePausedPanel;

    [SerializeField]
    private Animator gamePausedAnimator;

    public void ShowGamePausedPanel()
    {
        StartCoroutine(ShowPanel());
    }

    public void HideGamePausedPanel()
    {
        StartCoroutine(HidePanel());
    }

    IEnumerator ShowPanel()
    {
        gamePausedPanel.SetActive(true);
        gamePausedAnimator.Play("SlideIn");

        yield return new WaitForSeconds(0.2f);
    }

    IEnumerator HidePanel()
    {
        gamePausedAnimator.Play("SlideOut");

        yield return new WaitForSeconds(0.2f);

        gamePausedPanel.SetActive(false);
    }
}
