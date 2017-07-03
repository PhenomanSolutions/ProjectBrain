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

        yield return new WaitForSeconds(1f);
    }

    IEnumerator HidePanel()
    {
        gamePausedAnimator.Play("SlideOut");

        yield return new WaitForSeconds(1f);

        gamePausedPanel.SetActive(false);
    }
}
