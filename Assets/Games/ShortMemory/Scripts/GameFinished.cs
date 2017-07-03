using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameFinished : MonoBehaviour
{
    [SerializeField]
    private GameObject gameFinishedPanel;

    [SerializeField]
    private Animator gameFinishedAnimator;

    [SerializeField]
    private Text gameScore;

    public void ShowGameFinishedPanel(int score)
    {
        StartCoroutine(ShowPanel(score));
    }

    public void HideGameFinishedPanel()
    {
        StartCoroutine(HidePanel());
    }

    IEnumerator ShowPanel(int score)
    {
        gameScore.text = "Your score  " + score.ToString();
        gameFinishedPanel.SetActive(true);
        gameFinishedAnimator.Play("FadeIn");

        yield return new WaitForSeconds(1.5f);
    }

    IEnumerator HidePanel()
    {
        gameFinishedAnimator.Play("FadeOut");

        yield return new WaitForSeconds(1f);

        gameFinishedPanel.SetActive(false);
    }
}
