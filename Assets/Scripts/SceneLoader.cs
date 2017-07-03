using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    void Start()
    {
        Screen.fullScreen = false;
    }

    private string mainMenuName = "MainMenu";

    public void LoadScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void NavToMainMenu()
    {
        LoadScene(mainMenuName);
    }
}
