using UnityEngine;
using UnityEngine.SceneManagement;

public class KeepOnLoad : MonoBehaviour
{
    public int Time = 5;
    public string Name = "";

    void Awake() { 
        //DontDestroyOnLoad(this.gameObject);
        //SceneManager.activeSceneChanged += OnSceneChanged;
    }

    void OnSceneChanged(Scene a, Scene b)
    {
        if (b.name != Name)
        {
            Destroy(this.gameObject, Time);
        }
    }
}
